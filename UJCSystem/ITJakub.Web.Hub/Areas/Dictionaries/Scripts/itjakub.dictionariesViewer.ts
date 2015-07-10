class DictionaryViewer {
    private headwordDescriptionContainer: string;
    private paginationContainer: string;
    private headwordListContainer: string;
    private pagination: Pagination;
    private selectedBookIds: number[];
    private currentQuery: string;
    private recordCount: number;
    private searchUrl: string;
    private pageSize: number;
    private isLazyLoad: boolean;
    private isRequestToPrint = false;
    private headwordDescriptionDivs: HTMLDivElement[];
    private dictionariesInfo: IHeadwordBookInfo[];
    private headwordList: string[];

    constructor(headwordListContainer: string, paginationContainer: string, headwordDescriptionContainer: string, lazyLoad: boolean = false) {
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

    public createViewer(recordCount: number, searchUrl: string, selectedBookIds: number[], query: string = null, pageSize: number = 50) {
        this.selectedBookIds = selectedBookIds;
        this.currentQuery = query;
        this.recordCount = recordCount;
        this.searchUrl = searchUrl;
        this.pageSize = pageSize;

        var pageCount = Math.ceil(this.recordCount / this.pageSize);
        this.pagination.createPagination(pageCount, this.searchAndDisplay.bind(this));
    }

    public goToPage(pageNumber: number) {
        this.pagination.goToPage(pageNumber);
    }

    private searchAndDisplay(pageNumber: number) {
        this.isRequestToPrint = false;
        $.ajax({
            type: "GET",
            traditional: true,
            url: this.searchUrl,
            data: {
                selectedBookIds: this.selectedBookIds,
                query: this.currentQuery,
                page: pageNumber,
                pageSize: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.showHeadwords(response);
            }
        });
    }

    private showHeadwords(headwords: IHeadword[]) {
        $(this.headwordListContainer).empty();
        $(this.headwordDescriptionContainer).empty();
        this.headwordDescriptionDivs = [];
        this.dictionariesInfo = [];
        this.headwordList = [];

        var listUl = document.createElement("ul");
        var descriptionsDiv = document.createElement("div");
        for (var i = 0; i < headwords.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords[i];

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

                // create description
                var mainHeadwordDiv = document.createElement("div");
                var descriptionDiv = document.createElement("div");
                $(descriptionDiv).addClass("loading")
                    .addClass("dictionary-entry-description-container");

                if (this.isLazyLoad) {
                    this.prepareLazyLoad(mainHeadwordDiv);
                } else {
                    this.getAndShowHeadwordDescription(record.Headword, dictionary.BookGuid, dictionary.XmlEntryId, descriptionDiv);
                }

                var commentsDiv = document.createElement("div");
                var commentsLink = document.createElement("a");
                $(commentsLink).text("Připomínky");
                commentsLink.href = "#";
                $(commentsDiv).addClass("dictionary-entry-comments");
                commentsDiv.appendChild(commentsLink);

                var dictionaryDiv = document.createElement("div");
                var dictionaryLink = document.createElement("a");
                $(dictionaryLink).text(dictionary.BookTitle);
                dictionaryLink.href = "?guid=" + dictionary.BookGuid;
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
                aLink.innerHTML = dictionary.BookAcronym;
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

    private getAndShowHeadwordDescription(headword: string, bookGuid: string, xmlEntryId: string, container: HTMLDivElement) {
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
                $(container).empty();
                $(container).removeClass("loading");
                container.innerHTML = response;
                if (this.isRequestToPrint)
                    this.print();
            },
            error: () => {
                $(container).empty();
                $(container).removeClass("loading");
                $(container).text("Chyba při náčítání hesla '" + headword + "'.");
                if (this.isRequestToPrint)
                    this.print();
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
        this.getAndShowHeadwordDescription(headword, dictionaryInfo.BookGuid, dictionaryInfo.XmlEntryId, <HTMLDivElement>descriptionContainer);
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

    public print() {
        // check if all entries are loaded
        this.isRequestToPrint = false;
        if (!this.isAllLoaded()) {
            this.isRequestToPrint = true;

            if (this.isLazyLoad) {
                this.loadAllHeadwords();
            }

            return;
        }

        var printWindow = window.open("", "", "left=0,top=0,toolbar=0,scrollbars=0,status=0");

        var headwordsHtml = $(this.headwordDescriptionContainer).html();
        printWindow.document.write(headwordsHtml);
        printWindow.document.close();

        var styleCss =
            ".hidden {display: none;}" +
            ".dictionary-entry-comments {display: none;}";

        var styleElement = document.createElement("style");
        styleElement.innerHTML = styleCss;
        printWindow.document.head.appendChild(styleElement);
        
        printWindow.document.title = "Heslové stati";
        printWindow.focus();
        printWindow.print();
        printWindow.close(); 
    }
}

class Pagination {
    private pageClickCallback: (pageNumber: number) => void;
    private paginationContainer: string;
    private maxPageElements: number;
    private pageCount: number;
    private currentPage: number;
    private usePaginationDots = false;

    constructor(paginationContainer: string, maxVisiblePageElements = 7) {
        this.maxPageElements = maxVisiblePageElements;
        this.paginationContainer = paginationContainer;
    }

    public createPagination(pageCount: number, pageClickCallback: (pageNumber: number) => void) {
        this.pageCount = pageCount;
        this.pageClickCallback = pageClickCallback;
        
        $(this.paginationContainer).empty();

        var paginationUl = document.createElement("ul");
        $(paginationUl).addClass("pagination")
            .addClass("pagination-sm");

        var previousPageLi = this.createPageElement("&laquo;", "previous");
        paginationUl.appendChild(previousPageLi);

        for (var i = 1; i <= pageCount; i++) {
            var pageLi = this.createPageElement(String(i), i);
            paginationUl.appendChild(pageLi);
        }

        var nextPageLi = this.createPageElement("&raquo;", "next");
        paginationUl.appendChild(nextPageLi);

        this.usePaginationDots = pageCount > this.maxPageElements;
        if (this.usePaginationDots) {
            $(paginationUl.children[1]).after(this.createThreeDots());
            $(paginationUl.children[this.pageCount]).after(this.createThreeDots());
        }

        $(this.paginationContainer).append(paginationUl);

        this.updateCurrentPage(1);
    }

    private updateCurrentPage(newPageNumber: number) {
        this.getCurrentPageElement().removeClass("active");
        this.currentPage = newPageNumber;
        this.getCurrentPageElement().addClass("active");
        this.updateVisiblePageElements();

        this.pageClickCallback(newPageNumber);
    }

    private createPageElement(label: string, pageNumber: any): HTMLLIElement {
        var pageLi = document.createElement("li");
        var pageLink = document.createElement("a");
        pageLink.innerHTML = label;
        pageLink.href = "#";
        pageLink.setAttribute("data-page-number", pageNumber);

        $(pageLink).click(event => {
            event.preventDefault();
            var pageValue = $(event.target).data("page-number");
            var pageNumber: number;
            switch (pageValue) {
                case "previous":
                    pageNumber = this.currentPage - 1;
                    break;
                case "next":
                    pageNumber = this.currentPage + 1;
                    break;
                default:
                    pageNumber = Number(pageValue);
                    break;
            }

            if (pageNumber > 0 && pageNumber <= this.pageCount) {
                this.updateCurrentPage(pageNumber);
            }
        });

        pageLi.appendChild(pageLink);
        return pageLi;
    }

    private createThreeDots(): HTMLLIElement {
        var element = document.createElement("li");
        $(element).addClass("disabled")
            .addClass("three-dots");

        var contentElement = document.createElement("span");
        contentElement.innerHTML = "&hellip;";

        element.appendChild(contentElement);
        return element;
    }

    private getCurrentPageElement(): JQuery {
        var selector = "li:has(*[data-page-number=\"" + this.currentPage + "\"])";
        return $(selector);
    }

    private updateVisiblePageElements() {
        if (!this.usePaginationDots)
            return;

        var pageNumber = this.currentPage;
        var centerVisibleIndex = (this.maxPageElements - 1) / 2;
        var paginationListUl = $(this.paginationContainer).children().children();
        for (var i = 2; i < paginationListUl.length - 2; i++) {
            $(paginationListUl[i]).addClass("hidden");
        }

        var visibleInCenter = this.maxPageElements - 4; //two buttons on each side always visible

        var leftDotsHidden = false;
        var rightDotsHidden = false;
        var threeDotsElements = $(".three-dots");
        threeDotsElements.addClass("hidden");

        if (pageNumber > centerVisibleIndex) {
            threeDotsElements.first().removeClass("hidden");
            leftDotsHidden = true;
        }

        if (pageNumber < this.pageCount - centerVisibleIndex) {
            threeDotsElements.last().removeClass("hidden");
            rightDotsHidden = true;
        }

        if (!leftDotsHidden) {
            for (var j = 0; j < visibleInCenter + 1; j++) {
                $(paginationListUl[j + 3]).removeClass("hidden");
            }
        }
        else if (!rightDotsHidden) {
            for (var l = 0; l < visibleInCenter + 1; l++) {
                $(paginationListUl[this.pageCount - l]).removeClass("hidden");
            }
        }
        else {
            var centerIndex = this.currentPage + 1;
            $(paginationListUl[centerIndex]).removeClass("hidden");
            var iterations = (visibleInCenter - 1) / 2;
            for (var k = 1; k <= iterations; k++) {
                $(paginationListUl[centerIndex - k]).removeClass("hidden");
                $(paginationListUl[centerIndex + k]).removeClass("hidden");
            }
        }
    }

    public goToPage(pageNumber: number) {
        if (pageNumber > 0 && pageNumber <= this.pageCount) {
            this.updateCurrentPage(pageNumber);
        }
    }
}

interface IHeadwordBookInfo {
    BookGuid: string;
    BookAcronym: string;
    BookTitle: string;
    XmlEntryId: string;
}

interface IHeadword {
    Headword: string;
    Dictionaries: Array<IHeadwordBookInfo>;
}