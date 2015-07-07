class DictionaryViewer {
    private headwordDescriptionContainer: string;
    private paginationContainer: string;
    private headwordListContainer: string;
    private pagination: Pagination;
    private categoriesSelect: DropDownSelect;
    private currentQuery: string;
    private recordCount: number;
    private searchUrl: string;
    private pageSize = 20;

    constructor(headwordListContainer: string, paginationContainer: string, headwordDescriptionContainer: string) {
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
        this.pagination = new Pagination(paginationContainer);
    }

    public createViewer(recordCount: number, searchUrl: string, categories: DropDownSelect, query: string = null) {
        this.categoriesSelect = categories;
        this.currentQuery = query;
        this.recordCount = recordCount;
        this.searchUrl = searchUrl;

        var pageCount = Math.ceil(this.recordCount / this.pageSize);
        this.pagination.createPagination(pageCount, this.searchAndDisplay.bind(this));
    }

    private searchAndDisplay(pageNumber: number) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: this.searchUrl,
            data: JSON.stringify({
                query: this.currentQuery,
                page: pageNumber,
                pageSize: this.pageSize,
                selectedBookIds: [4] //TODO get from categories DropDownSelect
            }),
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

        var listUl = document.createElement("ul");
        var descriptionsDiv = document.createElement("div");
        for (var i = 0; i < headwords.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords[i];

            var headwordSpan = document.createElement("span");
            headwordSpan.innerText = record.Headword;
            $(headwordSpan).addClass("dictionary-result-headword");

            var favoriteGlyphSpan = document.createElement("span");
            $(favoriteGlyphSpan).addClass("glyphicon")
                .addClass("glyphicon-star-empty")
                .addClass("dictionary-result-headword-favorite");

            headwordLi.appendChild(headwordSpan);
            headwordLi.appendChild(favoriteGlyphSpan);

            for (var j = 0; j < record.Dictionaries.length; j++) {
                var dictionary = record.Dictionaries[j];

                var aLink = document.createElement("a");
                aLink.href = "?guid=" + dictionary.BookGuid + "&xmlEntryId=" + dictionary.XmlEntryId;
                aLink.innerHTML = dictionary.BookAcronym;
                $(aLink).addClass("dictionary-result-headword-book");

                var descriptionDiv = document.createElement("div");
                $(descriptionDiv).addClass("loading");
                this.getAndShowHeadwordDescription(record.Headword, dictionary, descriptionDiv);

                var commentsDiv = document.createElement("div");
                var commentsLink = document.createElement("a");
                commentsLink.innerText = "Připomínky";
                commentsLink.href = "#";
                $(commentsDiv).addClass("dictionary-entry-comments");
                commentsDiv.appendChild(commentsLink);

                var dictionaryDiv = document.createElement("div");
                var dictionaryLink = document.createElement("a");
                dictionaryLink.innerText = dictionary.BookAcronym; //TODO full name
                dictionaryLink.href = "?guid=" + dictionary.BookGuid;
                $(dictionaryDiv).addClass("dictionary-entry-name");
                dictionaryDiv.appendChild(dictionaryLink);

                headwordLi.appendChild(aLink);
                descriptionsDiv.appendChild(descriptionDiv);
                descriptionsDiv.appendChild(commentsDiv);
                descriptionsDiv.appendChild(dictionaryDiv);
                descriptionsDiv.appendChild(document.createElement("hr"));
            }
            
            listUl.appendChild(headwordLi);
        }

        $(this.headwordListContainer).append(listUl);
        $(this.headwordDescriptionContainer).append(descriptionsDiv);
    }

    private getAndShowHeadwordDescription(headword: string, headwordInfo: IHeadwordBookInfo, container: HTMLDivElement) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescription",
            data: {
                bookGuid: headwordInfo.BookGuid,
                xmlEntryId: headwordInfo.XmlEntryId
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                $(container).empty();
                $(container).removeClass("loading");
                container.innerHTML = response;
            },
            error: () => {
                $(container).empty();
                $(container).removeClass("loading");
                container.innerText = "Chyba při náčítání hesla '" + headword + "'.";
            }
        });
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
}

interface IHeadwordBookInfo {
    BookGuid: string;
    BookAcronym: string;
    BookVersionId: string;
    XmlEntryId: string;
}

interface IHeadword {
    Headword: string;
    Dictionaries: Array<IHeadwordBookInfo>;
}