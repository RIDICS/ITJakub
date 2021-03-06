﻿class DictionaryViewer {
    private readonly errorHandler: ErrorHandler;
    private headwordDescriptionContainer: string;
    private paginationContainer: string;
    private headwordListContainer: string;
    private pagination: Pagination;
    private recordCount: number;
    private pageSize: number;
    private isLazyLoad: boolean;
    private isRequestToPrint = false;
    private headwordDescriptionDivs: HTMLDivElement[];
    private dictionariesInfo: IHeadwordBookInfo[];
    private headwordList: string[];
    private favoriteHeadwordList: IDictionaryFavoriteHeadword[];
    private dictionariesMetadataList: IBookListDictionary;
    private showPageCallback: (pageNumber: number) => void;
    private addNewFavoriteCallback: (headwordName: string, headwordId: number, callback: (favoriteHeadwordId: number) => void) => void;
    private removeFavoriteCallback: (favoriteId) => void;
    private searchCriteria: string;
    private isCriteriaJson: boolean;
    private defaultPageNumber: number;
    private localization: Localization;

    constructor(headwordListContainer: string, paginationContainer: string, headwordDescriptionContainer: string, lazyLoad: boolean) {
        this.errorHandler = new ErrorHandler();
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
        this.isLazyLoad = lazyLoad;
        this.pagination = new Pagination({
            container: $(this.paginationContainer).get(0) as Node as HTMLDivElement,
            pageClickCallback: this.searchAndDisplay.bind(this),
            callPageClickCallbackOnInit: true,
            maxVisibleElements: 11,
            showInput: true
        });
        this.localization = localization;
    }

    public createViewer(recordCount: number, showPageCallback: (pageNumber: number) => number, pageSize: number, searchCriteria: string = null,
        isCriteriaJson: boolean = false)
    {
        this.recordCount = recordCount;
        this.showPageCallback = showPageCallback;
        this.pageSize = pageSize;
        this.searchCriteria = searchCriteria;
        this.isCriteriaJson = isCriteriaJson;

        if (this.defaultPageNumber)
            this.pagination.make(this.recordCount, this.pageSize, this.defaultPageNumber);
        else
            this.pagination.make(this.recordCount, this.pageSize);

        $(".disable-on-search-error").prop("disabled", false);
    }

    public setDefaultPageNumber(pageNumber: number) {
        this.defaultPageNumber = pageNumber;
    }

    public goToPage(pageNumber: number) {
        this.pagination.goToPage(pageNumber);
    }

    public setFavoriteCallback(addNewFavoriteCallback: (headwordName: string, headwordId: number, callback: (favoriteHeadwordId: number) => void) => void, removeFavoriteCallback: (favoriteId) => void) {
        this.addNewFavoriteCallback = addNewFavoriteCallback;
        this.removeFavoriteCallback = removeFavoriteCallback;
    }

    public setFavoriteHeadwordList(list: Array<IDictionaryFavoriteHeadword>) {
        this.favoriteHeadwordList = list;
        var favoriteIcons = $(".glyphicon-star", $(this.headwordListContainer));
        favoriteIcons.removeClass("glyphicon-star");
        favoriteIcons.addClass("glyphicon-star-empty");

        if (!this.dictionariesInfo)
            return;

        for (var i = 0; i < this.dictionariesInfo.length; i++) {
            var headwordDictionaryInfo = this.dictionariesInfo[i];
            
            if (this.isHeadwordFavorite(headwordDictionaryInfo)) {
                var favoriteIcon = $(".glyphicon[data-entry-index=\"" + i + "\"]", $(this.headwordListContainer));
                favoriteIcon.removeClass("glyphicon-star-empty");
                favoriteIcon.addClass("glyphicon-star");
            }
        }
    }

    private isHeadwordFavorite(headwordDictionaryInfo: IHeadwordBookInfo): boolean {
        for (var i = 0; i < this.favoriteHeadwordList.length; i++) {
            var favoriteHeadword = this.favoriteHeadwordList[i];
            if(headwordDictionaryInfo.headwordId === favoriteHeadword.headwordId)
                return true;
        }
        return false;
    }

    private isHeadwordFavoriteFromArray(array: Array<IHeadwordBookInfo>): boolean {
        for (var i = 0; i < array.length; i++) {
            if (this.isHeadwordFavorite(array[i]))
                return true;
        }
        return false;
    }

    private getFavoriteHeadword(headwordDictionaryInfo: IHeadwordBookInfo): IDictionaryFavoriteHeadword {
        for (let i = 0; i < this.favoriteHeadwordList.length; i++) {
            const favoriteHeadword = this.favoriteHeadwordList[i];
            if(headwordDictionaryInfo.headwordId === favoriteHeadword.headwordId)
                return favoriteHeadword;
        }
        return null;
    }

    private getFavoriteHeadwordFromArray(array: Array<IHeadwordBookInfo>): IDictionaryFavoriteHeadword {
        for (var i = 0; i < array.length; i++) {
            const favoriteHeadword = this.getFavoriteHeadword(array[i]);
            if (favoriteHeadword != null)
                return favoriteHeadword;
        }
        return null;
    }

    private searchAndDisplay(pageNumber: number) {
        $("#cancelFilter").addClass("hidden");
        this.isRequestToPrint = false;
        if (this.recordCount === 0) {
            $(this.headwordListContainer).empty();
            $(this.headwordDescriptionContainer).empty();

            var noEntryFoundDiv = document.createElement("div");
            noEntryFoundDiv.innerHTML = this.localization.translate("NoEntryFound", "Dictionaries").value;
            noEntryFoundDiv.classList.add("dictionary-list-empty");
            $(this.headwordListContainer).append(noEntryFoundDiv);

            return;
        }

        this.showLoadingBars();
        this.showPageCallback(pageNumber);
    }

    private showLoadingBars() {
        if ($(".dictionary-loading", $(this.headwordListContainer)).length > 0)
            return;

        var backgroundDiv1 = document.createElement("div");
        var backgroundDiv2 = document.createElement("div");

        var loadingDiv1 = lv.create(null, "lv-dots md lv-mid loading-headword");
        var loadingDiv2 = lv.create(null, "lv-dots md lv-mid loading-headword");

        $(backgroundDiv1).addClass("dictionary-loading");
        $(backgroundDiv2).addClass("dictionary-loading");
        $(backgroundDiv1).append(loadingDiv1.getElement());
        $(backgroundDiv2).append(loadingDiv2.getElement());

        $(this.headwordListContainer).append(backgroundDiv1);
        $(this.headwordDescriptionContainer).append(backgroundDiv2);
    }

    public showLoading() {
        $(this.headwordDescriptionContainer).empty();
        $(this.headwordListContainer).empty();
        $(this.paginationContainer).empty();
        this.showLoadingBars();
    }

    public showHeadwords(headwords: IHeadwordList) {
        $("#cancelFilter").addClass("hidden");
        $(this.headwordListContainer).empty();
        $(this.headwordDescriptionContainer).empty();
        this.headwordDescriptionDivs = [];
        this.dictionariesInfo = [];
        this.headwordList = [];
        this.dictionariesMetadataList = headwords.bookList;

        var listUl = document.createElement("ul");
        var descriptionsDiv = document.createElement("div");
        for (var i = 0; i < headwords.headwordList.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords.headwordList[i];

            var headwordSpan = document.createElement("span");
            $(headwordSpan).text(record.headword);
            $(headwordSpan).addClass("dictionary-result-headword");
            headwordLi.appendChild(headwordSpan);
            this.createAllHeadwordsLinkListener(headwordSpan);

            if (this.addNewFavoriteCallback != null) {
                const favoriteHeadword = this.getFavoriteHeadwordFromArray(record.dictionaries);
                const isFavorite = favoriteHeadword != null;
                const favoriteGlyphSpan = document.createElement("span");
                favoriteGlyphSpan.setAttribute("data-entry-index", String(this.headwordDescriptionDivs.length));
                
                if(isFavorite) {
                    $(favoriteGlyphSpan).data("favorite-headword-id", favoriteHeadword.id);
                }
                $(favoriteGlyphSpan).addClass("glyphicon")
                    .addClass(isFavorite ? "glyphicon-star" : "glyphicon-star-empty")
                    .addClass("dictionary-result-headword-favorite");
                $(favoriteGlyphSpan).on("click", event => {
                    this.favoriteHeadwordClick(event.target as Node as Element);
                });

                headwordLi.appendChild(favoriteGlyphSpan);
            }
            
            var dictionaryListDiv = document.createElement("div");
            $(dictionaryListDiv).addClass("dictionary-result-book-list");
            for (var j = 0; j < record.dictionaries.length; j++) {
                var dictionary = record.dictionaries[j];
                var dictionaryMetadata = this.dictionariesMetadataList[dictionary.bookId];
                var currentIndex = this.headwordDescriptionDivs.length;

                // create description
                var mainHeadwordDiv = document.createElement("div");

                var bar = lv.create(null, "lv-circles lv-mid sm");
                $(mainHeadwordDiv).append(bar.getElement());

                if (dictionary.pageId) { //image may be exists
                    var imageCheckBoxDiv = document.createElement("div");
                    var imageCheckBox = document.createElement("input");
                    var imageIconSpan = document.createElement("span");
                    var imageCheckBoxLabel = document.createElement("label");

                    imageCheckBox.type = "checkbox";
                    imageCheckBox.autocomplete = "off";
                    $(imageCheckBox).change(event => {
                        this.updateImageVisibility(event.target as Node as HTMLInputElement);
                    });

                    imageCheckBoxDiv.setAttribute("data-toggle", "buttons");
                    $(imageCheckBoxDiv).addClass("dictionary-entry-image-switch")
                        .addClass("btn-group");
                    
                    $(imageIconSpan).addClass("glyphicon")
                        .addClass("glyphicon-picture");
                    
                    $(imageCheckBoxLabel).addClass("btn")
                        .addClass("btn-primary");

                    imageCheckBoxLabel.appendChild(imageCheckBox);
                    imageCheckBoxLabel.appendChild(imageIconSpan);
                    imageCheckBoxDiv.appendChild(imageCheckBoxLabel);

                    mainHeadwordDiv.appendChild(imageCheckBoxDiv);
                }

                var imageContainerDiv = document.createElement("div");
                $(imageContainerDiv).addClass("dictionary-entry-image");

                var descriptionDiv = document.createElement("div");
                
                $(descriptionDiv).addClass("dictionary-entry-description-container");
                
                var commentsDiv = document.createElement("div");
                var commentsLink = document.createElement("a");
                $(commentsLink).text(this.localization.translate("Feedback", "Dictionaries").value);
                commentsLink.href = "Feedback?bookId=" + dictionaryMetadata.id
                    + "&headwordVersionId=" + dictionary.headwordVersionId
                    + "&headword=" + record.headword
                    + "&dictionary=" + encodeURIComponent(dictionaryMetadata.title);
                $(commentsDiv).addClass("dictionary-entry-comments");
                commentsDiv.appendChild(commentsLink);

                var dictionaryDiv = document.createElement("div");
                var dictionaryLink = document.createElement("a");
                $(dictionaryLink).text(dictionaryMetadata.title);
                dictionaryLink.href = "List?search=" + encodeURIComponent(dictionaryMetadata.title);
                $(dictionaryDiv).addClass("dictionary-entry-name");
                dictionaryDiv.appendChild(dictionaryLink);

                
                mainHeadwordDiv.appendChild(descriptionDiv);
                mainHeadwordDiv.appendChild(imageContainerDiv);
                mainHeadwordDiv.appendChild(commentsDiv);
                mainHeadwordDiv.appendChild(dictionaryDiv);
                mainHeadwordDiv.appendChild(document.createElement("hr"));
                mainHeadwordDiv.setAttribute("data-entry-index", String(currentIndex));
                this.headwordDescriptionDivs.push(mainHeadwordDiv);
                this.dictionariesInfo.push(dictionary);
                this.headwordList.push(record.headword);

                if (this.isLazyLoad) {
                    this.prepareLazyLoad(mainHeadwordDiv);
                } else {
                    this.getAndShowHeadwordDescription(currentIndex, descriptionDiv);
                }

                descriptionsDiv.appendChild(mainHeadwordDiv);

                // create link
                if (j > 0) {
                    var delimiterSpan = document.createElement("span");
                    $(delimiterSpan).text(" | ");
                    dictionaryListDiv.appendChild(delimiterSpan);
                }

                var aLink = document.createElement("a");
                aLink.href = "#";
                aLink.innerHTML = dictionaryMetadata.sourceAbbreviation;
                aLink.setAttribute("data-entry-index", String(currentIndex));
                $(aLink).addClass("dictionary-result-headword-book");
                this.createLinkListener(aLink, record.headword, dictionary, descriptionDiv);

                dictionaryListDiv.appendChild(aLink);
            }

            headwordLi.appendChild(dictionaryListDiv);
            listUl.appendChild(headwordLi);
        }

        $(this.headwordListContainer).append(listUl);
        $(this.headwordDescriptionContainer).append(descriptionsDiv);

        const searchButton = $("#searchButton");
        if(searchButton.data("favorite-headword-trigger"))
        {
            searchButton.data("favorite-headword-trigger", false);
            const headwordQuery = $("#searchbox").val();
            const elem = $(`#headwordList .dictionary-result-headword`);
            for (var k = 0; k < elem.length; k++) {
                const rowElem = $(elem[k]);
                if (rowElem.text() === headwordQuery) {
                    rowElem.trigger("click");
                }
            }
        }
    }

    private updateImageVisibility(checkBox: HTMLInputElement) {
        var mainDiv = $(checkBox).closest("[data-entry-index]");
        var imageContainer = $(".dictionary-entry-image", mainDiv);
        if (checkBox.checked) {
            if (imageContainer.hasClass("hidden")) {
                imageContainer.removeClass("hidden");
                return;
            }
            var imageLoader = lv.create(null, "lv-circles tiny lv-mid lvt-1");
            $(imageContainer).append(imageLoader.getElement());

            var index = $(mainDiv).data("entry-index");
            var entryInfo = this.dictionariesInfo[index];
            var imageLink = getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordImage?pageId=" + entryInfo.pageId;
            var imageElement = document.createElement("img");
            imageElement.setAttribute("src", imageLink);

            imageElement.onload = () => {
                imageContainer.append(imageElement);
                imageLoader.remove();
            };
            imageElement.onerror = () => {
                $(imageContainer).empty();

                var errorDiv = document.createElement("div");
                //$(errorDiv).text("Chyba při načítání obrázku k heslu '" + this.headwordList[index] + "'.");
                $(errorDiv).text(this.localization.translateFormat("ImageTermLoadError", new Array<string>(this.headwordList[index]) , "Dictionaries").value);
                $(errorDiv).addClass("entry-load-error");

                imageContainer.append(errorDiv);
            };

        } else {
            imageContainer.addClass("hidden");
        }
    }

    private favoriteHeadwordClick(element: Element) {
        var index: number = $(element).data("entry-index");
        var isFavorite = $(element).hasClass("glyphicon-star");
        var dictionaryInfo = this.dictionariesInfo[index];
        const headwordName = $(element).siblings(".dictionary-result-headword").text().trim();
 
        
        if (isFavorite) {
            $(element).removeClass("glyphicon-star");
            $(element).addClass("glyphicon-star-empty");
            this.removeFavoriteCallback($(element).data("favorite-headword-id"));
        } else {
            this.addNewFavoriteCallback(headwordName, dictionaryInfo.headwordId, (favoriteHeadwordId: number) => {
                $(element).removeClass("glyphicon-star-empty");
                $(element).addClass("glyphicon-star");
                $(element).data("favorite-headword-id", favoriteHeadwordId)
            });            
        }
    }

    private createLinkListener(aLink: HTMLAnchorElement, headword: string, headwordInfo: IHeadwordBookInfo, container: HTMLDivElement) {
        $(aLink).click(event => {
            event.preventDefault();
            var index: number = $(event.target as Node as Element).data("entry-index");
            var headwordDiv = this.headwordDescriptionDivs[index];

            for (var k = 0; k < this.headwordDescriptionDivs.length; k++) {
                $(this.headwordDescriptionDivs[k]).addClass("hidden");
            }
            $(headwordDiv).removeClass("hidden");
            $("#cancelFilter").removeClass("hidden");

            if ($(headwordDiv).hasClass("lazy-loading")) {
                this.loadHeadwordDescription(index);
            }

            var headwordItem = $(event.target as Node as Element).closest("li");
            $(headwordItem).siblings().removeClass("dictionary-headword-highlight");
            $(headwordItem).addClass("dictionary-headword-highlight");
        });
    }

    private createAllHeadwordsLinkListener(aLink: HTMLElement) {
        $(aLink).on("click", event => {
            event.preventDefault();

            const headwordItem = $(event.target as Node as Element).closest("li");
            const links = headwordItem.find("a.dictionary-result-headword-book").toArray();

            for (var k = 0; k < this.headwordDescriptionDivs.length; k++) {
                $(this.headwordDescriptionDivs[k]).addClass("hidden");
            }

            for (let link of links) {
                var index: number = $(link).data("entry-index");
                var headwordDiv = this.headwordDescriptionDivs[index];
                $(headwordDiv).removeClass("hidden");

                if ($(headwordDiv).hasClass("lazy-loading")) {
                    this.loadHeadwordDescription(index);
                }
            }

            $("#cancelFilter").removeClass("hidden");

            $(headwordItem).siblings().removeClass("dictionary-headword-highlight");
            $(headwordItem).addClass("dictionary-headword-highlight");
        });
    }

    private prepareLazyLoad(mainDescriptionElement: HTMLDivElement) {
        $(mainDescriptionElement).addClass("lazy-loading");
        $(mainDescriptionElement).bind("appearing", event => {
            var descriptionDiv = event.target;
            var index = $(descriptionDiv as Node as Element).data("entry-index");
            this.loadHeadwordDescription(index);
        });
    }

    private showLoadHeadword(response: string, container: HTMLDivElement) {
        $(container).empty();
        $(container).siblings().remove('.lv-circles');
        container.innerHTML = response;
        if (this.isRequestToPrint)
            this.print();
    }

    private showLoadError(headword: string, container: HTMLDivElement) {
        $(container).empty();
        $(container).siblings().remove('.lv-circles');

        var errorDiv = document.createElement("div");
        //$(errorDiv).text("Chyba při náčítání hesla '" + headword + "'.");
        $(errorDiv).text(this.localization.translateFormat("ImageTermLoadError", new Array<string>(headword), "Dictionaries").value);
        $(errorDiv).addClass("entry-load-error");

        container.appendChild(errorDiv);

        if (this.isRequestToPrint)
            this.print();
    }

    private loadImageOnError(index: number, container: HTMLDivElement) {
        $(container).empty();
        $(container).siblings().remove('.lv-circles');

        var mainDiv = this.headwordDescriptionDivs[index];
        var headwordDescriptionContainer = $(".dictionary-entry-description-container", mainDiv);
        var toggleButtonLabel = $(".dictionary-entry-image-switch label", mainDiv);
        var checkBox = $("input", toggleButtonLabel);

        var headwordLabelSpan = document.createElement("span");
        $(headwordLabelSpan).addClass("entry-image-header");
        $(headwordLabelSpan).text(this.headwordList[index]);
        headwordDescriptionContainer.append(headwordLabelSpan);

        if (checkBox.length !== 0 && !(checkBox.get(0) as Node as HTMLInputElement).checked) {
            toggleButtonLabel.trigger("click");
        }

        if (this.isRequestToPrint)
            this.print();
    }

    private getAndShowHeadwordDescription(headwordIndex: number, container: HTMLDivElement) {
        if (this.searchCriteria == null)
            this.getAndShowHeadwordDescriptionBasic(headwordIndex, container);
        else
            this.getAndShowHeadwordDescriptionFromSearch(headwordIndex, container);
    }

    private getAndShowHeadwordDescriptionBasic(headwordIndex: number, container: HTMLDivElement) {
        var headword = this.headwordList[headwordIndex];
        var headwordInfo = this.dictionariesInfo[headwordIndex];
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescription",
            data: {
                headwordId: headwordInfo.headwordId
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.showLoadHeadword(response, container);
            },
            error: () => {
                if (!headwordInfo.pageId) {
                    this.showLoadError(headword, container);
                } else {
                    this.loadImageOnError(headwordIndex, container);
                }
            }
        });
    }

    private getAndShowHeadwordDescriptionFromSearch(headwordIndex: number, container: HTMLDivElement) {
        var headword = this.headwordList[headwordIndex];
        var headwordInfo = this.dictionariesInfo[headwordIndex];
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescriptionFromSearch",
            data: {
                criteria: this.searchCriteria,
                isCriteriaJson: this.isCriteriaJson,
                headwordId: headwordInfo.headwordId
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.showLoadHeadword(response, container);
            },
            error: () => {
                if (!headwordInfo.pageId) {
                    this.showLoadError(headword, container);
                } else {
                    this.loadImageOnError(headwordIndex, container);
                }
            }
        });
    }

    private loadHeadwordDescription(index: number) {
        var mainDescriptionDiv = this.headwordDescriptionDivs[index];
        var descriptionContainer = $(".dictionary-entry-description-container", mainDescriptionDiv).get(0);
        
        $(mainDescriptionDiv).unbind("appearing");
        $(mainDescriptionDiv).removeClass("lazy-loading");
        this.getAndShowHeadwordDescription(index, descriptionContainer as Node as HTMLDivElement);
    }

    private isAllLoaded(): boolean {
        var descriptions = $(this.headwordDescriptionContainer);
        var notLoaded = $(".lv-circles", descriptions);
        var notLoadedVisible = notLoaded.filter(":not(.hidden)");
        return notLoadedVisible.length === 0;
    }

    public loadAllHeadwords() {
        for (var i = 0; i < this.headwordDescriptionDivs.length; i++) {
            var descriptionDiv = this.headwordDescriptionDivs[i];
            if ($(descriptionDiv).hasClass("lazy-loading") && !$(descriptionDiv).hasClass("hidden")) {
                this.loadHeadwordDescription(i);
            }
        }
    }

    private showPrintModal() {
        if (this.isRequestToPrint)
            return;

        $("#print-modal").modal({
            backdrop: "static",
            show: true
        });
    }

    private hidePrintModal() {
        $("#print-modal").modal("hide");
    }

    public cancelFilter() {
        $("li", $(this.headwordListContainer)).removeClass("dictionary-headword-highlight");
        $(this.headwordDescriptionDivs).removeClass("hidden");
    }

    public print() {
        // check if all entries are loaded
        if (!this.isAllLoaded()) {
            this.showPrintModal();
            this.isRequestToPrint = true;

            if (this.isLazyLoad) {
                this.loadAllHeadwords();
            }

            return;
        }

        this.isRequestToPrint = false;
        this.hidePrintModal();

        window.print();
    }

    public printList() {
        var listHtml = $(this.headwordListContainer).html();
        var pageHtml = $(this.paginationContainer).html();
        var printWindow = window.open("", "", "toolbar=no,location=no,status=no,menubar=no,scrollbars=yes");
        var doc = printWindow.document;
        
        doc.write("<div>");
        doc.write(listHtml);
        doc.write("</div><div>");
        doc.write(pageHtml);
        doc.write("</div>");
        doc.close();

        $(".pagination-input input", doc).val(this.pagination.getCurrentPage());
        
        $("link, style").each((index, element) => {
            $(doc.head).append($(element as Node as Element).clone());
        });

        var css = "body { background-color: white; padding: 0 10px; }"
            + ".dictionary-result-headword-favorite { display: none; }"
            + ".dictionary-result-book-list { margin-left: 16px; }"
            + "ul { list-style: none; padding-left: 0; }";
        var style = doc.createElement("style");
        style.type = "text/css";
        style.appendChild(document.createTextNode(css));
        doc.head.appendChild(style);
        
        printWindow.focus();

        $(printWindow.document.documentElement).ready(() => {
            //hack: not exist event CSSready
            setTimeout(()=> { printWindow.print(); }, 2000);
        });
    }

    public showErrors(jqXhrError: JQueryXHR) {
        const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(jqXhrError));
        $(this.headwordListContainer).empty().append(alert.buildElement());
        $(this.paginationContainer).empty();
        $(this.headwordDescriptionContainer).empty().append(alert.buildElement());
        $(".disable-on-search-error").prop("disabled", true);
    }
}

interface IBookListDictionary {
    [bookXmlId: number]: IBookContract;
}