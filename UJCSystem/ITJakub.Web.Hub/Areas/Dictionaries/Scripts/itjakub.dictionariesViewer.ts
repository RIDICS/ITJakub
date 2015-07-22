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
    private dictionariesMetadataList: IBookListDictionary;
    private showPageCallback: (pageNumber: number) => void;
    private advancedSearchJson: string;

    constructor(headwordListContainer: string, paginationContainer: string, headwordDescriptionContainer: string, lazyLoad: boolean) {
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
        this.isLazyLoad = lazyLoad;
        this.pagination = new Pagination(paginationContainer);

        window.matchMedia("print").addListener(mql => {
            if (mql.matches) {
                this.loadAllHeadwords();
            }
        });
    }

    public createViewer(recordCount: number, showPageCallback: (pageNumber: number) => void, pageSize: number, advancedSearchJson: string = null) {
        this.recordCount = recordCount;
        this.showPageCallback = showPageCallback;
        this.pageSize = pageSize;
        this.advancedSearchJson = advancedSearchJson;

        var pageCount = Math.ceil(this.recordCount / this.pageSize);
        this.pagination.createPagination(pageCount, this.searchAndDisplay.bind(this));
    }

    public goToPage(pageNumber: number) {
        this.pagination.goToPage(pageNumber);
    }

    private searchAndDisplay(pageNumber: number) {
        this.isRequestToPrint = false;
        this.showPageCallback(pageNumber);
    }

    public showHeadwords(headwords: IHeadwordList) {
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

            var favoriteGlyphSpan = document.createElement("span");
            $(favoriteGlyphSpan).addClass("glyphicon")
                .addClass("glyphicon-star-empty")
                .addClass("dictionary-result-headword-favorite");

            headwordLi.appendChild(headwordSpan);
            headwordLi.appendChild(favoriteGlyphSpan);

            for (var j = 0; j < record.Dictionaries.length; j++) {
                var dictionary = record.Dictionaries[j];
                var dictionaryMetadata = this.dictionariesMetadataList[dictionary.BookXmlId];

                // create description
                var mainHeadwordDiv = document.createElement("div");
                var descriptionDiv = document.createElement("div");
                $(descriptionDiv).addClass("loading")
                    .addClass("dictionary-entry-description-container");

                if (this.isLazyLoad) {
                    this.prepareLazyLoad(mainHeadwordDiv);
                } else {
                    this.getAndShowHeadwordDescription(record.Headword, dictionary.BookXmlId, dictionary.EntryXmlId, descriptionDiv);
                }

                var commentsDiv = document.createElement("div");
                var commentsLink = document.createElement("a");
                $(commentsLink).text("Připomínky");
                commentsLink.href = "#";
                $(commentsDiv).addClass("dictionary-entry-comments");
                commentsDiv.appendChild(commentsLink);

                var dictionaryDiv = document.createElement("div");
                var dictionaryLink = document.createElement("a");
                $(dictionaryLink).text(dictionaryMetadata.BookTitle);
                dictionaryLink.href = "?guid=" + dictionary.BookXmlId;
                $(dictionaryDiv).addClass("dictionary-entry-name");
                dictionaryDiv.appendChild(dictionaryLink);

                
                mainHeadwordDiv.appendChild(descriptionDiv);
                mainHeadwordDiv.appendChild(commentsDiv);
                mainHeadwordDiv.appendChild(dictionaryDiv);
                mainHeadwordDiv.appendChild(document.createElement("hr"));
                mainHeadwordDiv.setAttribute("data-entry-index", String(this.headwordDescriptionDivs.length));
                this.headwordDescriptionDivs.push(mainHeadwordDiv);
                this.dictionariesInfo.push(dictionary);
                this.headwordList.push(record.Headword);

                descriptionsDiv.appendChild(mainHeadwordDiv);

                // create link
                var aLink = document.createElement("a");
                aLink.href = "#";
                aLink.innerHTML = dictionaryMetadata.BookAcronym;
                aLink.setAttribute("data-entry-index", String(this.headwordDescriptionDivs.length-1));
                $(aLink).addClass("dictionary-result-headword-book");
                this.createLinkListener(aLink, record.Headword, dictionary, descriptionDiv);

                headwordLi.appendChild(aLink);
            }
            
            listUl.appendChild(headwordLi);
        }

        $(this.headwordListContainer).append(listUl);
        $(this.headwordDescriptionContainer).append(descriptionsDiv);
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
        $(container).removeClass("loading");
        container.innerHTML = response;
        if (this.isRequestToPrint)
            this.print();
    }

    private showLoadError(headword: string, container: HTMLDivElement) {
        $(container).empty();
        $(container).removeClass("loading");
        $(container).text("Chyba při náčítání hesla '" + headword + "'.");
        if (this.isRequestToPrint)
            this.print();
    }

    private getAndShowHeadwordDescription(headword: string, bookGuid: string, xmlEntryId: string, container: HTMLDivElement) {
        if (this.advancedSearchJson == null)
            this.getAndShowHeadwordDescriptionBasic(headword, bookGuid, xmlEntryId, container);
        else
            this.getAndShowHeadwordDescriptionFromSearch(headword, bookGuid, xmlEntryId, container);
    }

    private getAndShowHeadwordDescriptionBasic(headword: string, bookGuid: string, xmlEntryId: string, container: HTMLDivElement) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescription",
            data: {
                bookGuid: bookGuid,
                xmlEntryId: xmlEntryId
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.showLoadHeadword(response, container);
            },
            error: () => {
                this.showLoadError(headword, container);
            }
        });
    }

    private getAndShowHeadwordDescriptionFromSearch(headword: string, bookGuid: string, xmlEntryId: string, container: HTMLDivElement) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescriptionFromSearch",
            data: {
                json: this.advancedSearchJson,
                bookGuid: bookGuid,
                xmlEntryId: xmlEntryId
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.showLoadHeadword(response, container);
            },
            error: () => {
                this.showLoadError(headword, container);
            }
        });
    }

    private loadHeadwordDescription(index: number) {
        var mainDescriptionDiv = this.headwordDescriptionDivs[index];
        var headword = this.headwordList[index];
        var dictionaryInfo = this.dictionariesInfo[index];
        var descriptionContainer = $(".dictionary-entry-description-container", mainDescriptionDiv).get(0);

        $(mainDescriptionDiv).unbind("appearing");
        $(mainDescriptionDiv).removeClass("lazy-loading");
        this.getAndShowHeadwordDescription(headword, dictionaryInfo.BookXmlId, dictionaryInfo.EntryXmlId, <HTMLDivElement>descriptionContainer);
    }

    private isAllLoaded(): boolean {
        var descriptions = $(this.headwordDescriptionContainer);
        var notLoaded = $(".loading", descriptions);
        var notLoadedVisible = notLoaded.parent(":not(.hidden)");
        return notLoadedVisible.length === 0;
    }

    private loadAllHeadwords() {
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
}

interface IHeadwordBookInfo {
    BookXmlId: string;
    EntryXmlId: string;
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
    BookAcronym: string;
    BookTitle: string;
}

interface IBookListDictionary {
    [bookXmlId: string]: IDictionaryContract;
}