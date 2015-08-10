/// <reference path="../../../Scripts/Plugins/itjakub.plugins.pagination.ts" />
class DictionaryViewer {
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
    private addNewFavoriteCallback: (bookId: string, entryXmlId: string) => void;
    private removeFavoriteCallback: (bookId: string, entryXmlId: string) => void;
    private searchCriteria: string;
    private isCriteriaJson: boolean;
    private defaultPageNumber: number;

    constructor(headwordListContainer: string, paginationContainer: string, headwordDescriptionContainer: string, lazyLoad: boolean) {
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
        this.isLazyLoad = lazyLoad;
        this.pagination = new Pagination(paginationContainer);
    }

    public createViewer(recordCount: number, showPageCallback: (pageNumber: number) => void, pageSize: number, searchCriteria: string = null,
        isCriteriaJson: boolean = false)
    {
        this.recordCount = recordCount;
        this.showPageCallback = showPageCallback;
        this.pageSize = pageSize;
        this.searchCriteria = searchCriteria;
        this.isCriteriaJson = isCriteriaJson;

        if (this.defaultPageNumber)
            this.pagination.createPagination(this.recordCount, this.pageSize, this.searchAndDisplay.bind(this), this.defaultPageNumber);
        else
            this.pagination.createPagination(this.recordCount, this.pageSize, this.searchAndDisplay.bind(this));
    }

    public setDefaultPageNumber(pageNumber: number) {
        this.defaultPageNumber = pageNumber;
    }

    public goToPage(pageNumber: number) {
        this.pagination.goToPage(pageNumber);
    }

    public setFavoriteCallback(addNewFavoriteCallback: (bookId: string, entryXmlId: string) => void, removeFavoriteCallback: (bookId: string, entryXmlId: string) => void) {
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
            if (headwordDictionaryInfo.BookXmlId === favoriteHeadword.BookId && headwordDictionaryInfo.EntryXmlId === favoriteHeadword.EntryXmlId)
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

    private getHeadwordIndex(bookId: string, entryXmlId: string): number {
        for (var i = 0; i < this.dictionariesInfo.length; i++) {
            var dictionaryInfo = this.dictionariesInfo[i];
            if (dictionaryInfo.BookXmlId === bookId && dictionaryInfo.EntryXmlId === entryXmlId)
                return i;
        }
        return -1;
    }

    private searchAndDisplay(pageNumber: number) {
        $("#cancelFilter").addClass("hidden");
        this.isRequestToPrint = false;
        if (this.recordCount === 0) {
            $(this.headwordListContainer).empty();
            $(this.headwordDescriptionContainer).empty();

            var noEntryFoundDiv = document.createElement("div");
            $(noEntryFoundDiv).text("Žádné výsledky k zobrazení");
            $(noEntryFoundDiv).addClass("dictionary-list-empty");
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
        var loadingDiv = document.createElement("div");

        $(backgroundDiv1).addClass("dictionary-loading");
        $(backgroundDiv2).addClass("dictionary-loading");
        $(backgroundDiv1).append(loadingDiv);
        $(loadingDiv).addClass("loader");
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
        this.dictionariesMetadataList = headwords.BookList;

        var listUl = document.createElement("ul");
        var descriptionsDiv = document.createElement("div");
        for (var i = 0; i < headwords.HeadwordList.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords.HeadwordList[i];

            var headwordSpan = document.createElement("span");
            $(headwordSpan).text(record.Headword);
            $(headwordSpan).addClass("dictionary-result-headword");
            headwordLi.appendChild(headwordSpan);

            if (this.addNewFavoriteCallback != null) {
                var isFavorite = this.isHeadwordFavoriteFromArray(record.Dictionaries);
                var favoriteGlyphSpan = document.createElement("span");
                favoriteGlyphSpan.setAttribute("data-entry-index", String(this.headwordDescriptionDivs.length));
                $(favoriteGlyphSpan).addClass("glyphicon")
                    .addClass(isFavorite ? "glyphicon-star" : "glyphicon-star-empty")
                    .addClass("dictionary-result-headword-favorite");
                $(favoriteGlyphSpan).click(event => {
                    var index: number = $(event.target).data("entry-index");
                    this.addNewFavoriteHeadword(index);
                });

                headwordLi.appendChild(favoriteGlyphSpan);
            }
            
            var dictionaryListDiv = document.createElement("div");
            $(dictionaryListDiv).addClass("dictionary-result-book-list");

            for (var j = 0; j < record.Dictionaries.length; j++) {
                var dictionary = record.Dictionaries[j];
                var dictionaryMetadata = this.dictionariesMetadataList[dictionary.BookXmlId];
                var currentIndex = this.headwordDescriptionDivs.length;

                // create description
                var mainHeadwordDiv = document.createElement("div");

                if (dictionary.Image) {
                    var imageCheckBoxDiv = document.createElement("div");
                    var imageCheckBox = document.createElement("input");
                    var imageIconSpan = document.createElement("span");
                    var imageCheckBoxLabel = document.createElement("label");

                    imageCheckBox.type = "checkbox";
                    imageCheckBox.autocomplete = "off";
                    $(imageCheckBox).change(event => {
                        this.updateImageVisibility(<HTMLInputElement>event.target);
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
                $(mainHeadwordDiv).addClass("loading-background");
                $(descriptionDiv).addClass("dictionary-entry-description-container");
                
                var commentsDiv = document.createElement("div");
                var commentsLink = document.createElement("a");
                $(commentsLink).text("Připomínky");
                commentsLink.href = "Feedback?bookId=" + dictionaryMetadata.BookXmlId
                    + "&versionId=" + dictionaryMetadata.BookVersionXmlId
                    + "&entryId=" + dictionary.EntryXmlId
                    + "&headword=" + record.Headword
                    + "&dictionary=" + encodeURIComponent(dictionaryMetadata.BookTitle);
                $(commentsDiv).addClass("dictionary-entry-comments");
                commentsDiv.appendChild(commentsLink);

                var dictionaryDiv = document.createElement("div");
                var dictionaryLink = document.createElement("a");
                $(dictionaryLink).text(dictionaryMetadata.BookTitle);
                dictionaryLink.href = "?bookId=" + dictionary.BookXmlId;
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
                this.headwordList.push(record.Headword);

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
                aLink.innerHTML = dictionaryMetadata.BookAcronym;
                aLink.setAttribute("data-entry-index", String(currentIndex));
                $(aLink).addClass("dictionary-result-headword-book");
                this.createLinkListener(aLink, record.Headword, dictionary, descriptionDiv);

                dictionaryListDiv.appendChild(aLink);
            }

            headwordLi.appendChild(dictionaryListDiv);
            listUl.appendChild(headwordLi);
        }

        $(this.headwordListContainer).append(listUl);
        $(this.headwordDescriptionContainer).append(descriptionsDiv);
    }

    private updateImageVisibility(checkBox: HTMLInputElement) {
        var mainDiv = $(checkBox).closest("[data-entry-index]");
        var imageContainer = $(".dictionary-entry-image", mainDiv);
        if (checkBox.checked) {
            if (imageContainer.hasClass("hidden")) {
                imageContainer.removeClass("hidden");
                return;
            }

            var index = $(mainDiv).data("entry-index");
            var entryInfo = this.dictionariesInfo[index];
            var bookVersionXmlId = this.dictionariesMetadataList[entryInfo.BookXmlId].BookVersionXmlId;
            var imageLink = getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordImage?bookXmlId=" + entryInfo.BookXmlId + "&bookVersionXmlId=" + bookVersionXmlId + "&fileName=" + entryInfo.Image;
            var imageElement = document.createElement("img");
            imageElement.setAttribute("src", imageLink);
            imageContainer.append(imageElement);

            $(imageContainer).addClass("loading");
            imageElement.onload = () => {
                $(imageContainer).removeClass("loading");
            };
            imageElement.onerror = () => {
                $(imageContainer).removeClass("loading");
                $(imageContainer).empty();

                var errorDiv = document.createElement("div");
                $(errorDiv).text("Chyba při načítání obrázku k heslu '" + this.headwordList[index] + "'.");
                $(errorDiv).addClass("entry-load-error");

                imageContainer.append(errorDiv);
            };

        } else {
            imageContainer.addClass("hidden");
        }
    }

    private addNewFavoriteHeadword(index: number) {
        var dictionaryInfo = this.dictionariesInfo[index];
        this.addNewFavoriteCallback(dictionaryInfo.BookXmlId, dictionaryInfo.EntryXmlId);
    }

    private createLinkListener(aLink: HTMLAnchorElement, headword: string, headwordInfo: IHeadwordBookInfo, container: HTMLDivElement) {
        $(aLink).click(event => {
            event.preventDefault();
            var index: number = $(event.target).data("entry-index");
            var headwordDiv = this.headwordDescriptionDivs[index];

            for (var k = 0; k < this.headwordDescriptionDivs.length; k++) {
                $(this.headwordDescriptionDivs[k]).addClass("hidden");
            }
            $(headwordDiv).removeClass("hidden");
            $("#cancelFilter").removeClass("hidden");

            if ($(headwordDiv).hasClass("lazy-loading")) {
                this.loadHeadwordDescription(index);
            }
        });
    }

    private prepareLazyLoad(mainDescriptionElement: HTMLDivElement) {
        $(mainDescriptionElement).addClass("lazy-loading");
        $(mainDescriptionElement).bind("appearing", event => {
            var descriptionDiv = event.target;
            var index = $(descriptionDiv).data("entry-index");
            this.loadHeadwordDescription(index);
        });
    }

    private showLoadHeadword(response: string, container: HTMLDivElement) {
        $(container).empty();
        $(container).parent().removeClass("loading-background");
        container.innerHTML = response;
        if (this.isRequestToPrint)
            this.print();
    }

    private showLoadError(headword: string, container: HTMLDivElement) {
        $(container).empty();
        $(container).parent().removeClass("loading-background");

        var errorDiv = document.createElement("div");
        $(errorDiv).text("Chyba při náčítání hesla '" + headword + "'.");
        $(errorDiv).addClass("entry-load-error");

        container.appendChild(errorDiv);

        if (this.isRequestToPrint)
            this.print();
    }

    private loadImageOnError(index: number, container: HTMLDivElement) {
        $(container).empty();
        $(container).parent().removeClass("loading-background");

        var mainDiv = this.headwordDescriptionDivs[index];
        var headwordDescriptionContainer = $(".dictionary-entry-description-container", mainDiv);
        var toggleButtonLabel = $(".dictionary-entry-image-switch label", mainDiv);
        var checkBox = $("input", toggleButtonLabel);

        var headwordLabelSpan = document.createElement("span");
        $(headwordLabelSpan).addClass("entry-image-header");
        $(headwordLabelSpan).text(this.headwordList[index]);
        headwordDescriptionContainer.append(headwordLabelSpan);

        if (checkBox.length !== 0 && !(<HTMLInputElement>checkBox.get(0)).checked) {
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
                bookGuid: headwordInfo.BookXmlId,
                xmlEntryId: headwordInfo.EntryXmlId
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.showLoadHeadword(response, container);
            },
            error: () => {
                if (!headwordInfo.Image) {
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
                bookGuid: headwordInfo.BookXmlId,
                xmlEntryId: headwordInfo.EntryXmlId
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.showLoadHeadword(response, container);
            },
            error: () => {
                if (!headwordInfo.Image) {
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
        this.getAndShowHeadwordDescription(index, <HTMLDivElement>descriptionContainer);
    }

    private isAllLoaded(): boolean {
        var descriptions = $(this.headwordDescriptionContainer);
        var notLoaded = $(".loading-background", descriptions);
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
        for (var i = 0; i < this.headwordDescriptionDivs.length; i++) {
            $(this.headwordDescriptionDivs[i]).removeClass("hidden");
        }
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
        
        $("link, style").each((index, element) => {
            $(doc.head).append($(element).clone());
        });

        var css = "body { background-color: white; padding: 0 10px; }"
            + ".dictionary-result-headword-favorite { display: none; }"
            + ".dictionary-result-book-list { margin-left: 16px; }";
        var style = doc.createElement("style");
        style.type = "text/css";
        style.appendChild(document.createTextNode(css));
        doc.head.appendChild(style);

        printWindow.focus();
        $(printWindow).ready(() => {
            printWindow.print();
        });
    }
}

interface IHeadwordBookInfo {
    BookXmlId: string;
    EntryXmlId: string;
    Image: string;
}

interface IHeadword {
    Headword: string;
    Dictionaries: Array<IHeadwordBookInfo>;
}

interface IHeadwordList {
    BookList: IBookListDictionary;
    HeadwordList: Array<IHeadword>;
}

interface IDictionaryContract {
    BookXmlId: string;
    BookVersionXmlId: string;
    BookAcronym: string;
    BookTitle: string;
}

interface IBookListDictionary {
    [bookXmlId: string]: IDictionaryContract;
}