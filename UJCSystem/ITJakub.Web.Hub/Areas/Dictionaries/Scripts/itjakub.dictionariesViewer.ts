class DictionaryViewer {
    private headwordDescriptionContainer: string;
    private paginationContainer: string;
    private headwordListContainer: string;
    private currentQuery: string;
    private pageSize = 20;
    private maxPageElements = 7;
    private recordCount: number;
    private pageCount: number;
    private currentPage: number;
    private usePaginationDots = false;

    constructor(headwordListContainer: string, paginationContainer: string, headwordDescriptionContainer: string) {
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
    }

    public search(query: string) {
        this.currentQuery = query;
        this.searchAndDisplay(1);

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetSearchResultCount",
            data: {
                query: this.currentQuery
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                //TODO save and display result count (headwords and fulltext)
                this.recordCount = 150;
                this.pageCount = Math.ceil(this.recordCount / this.pageSize);
                this.createPagination(this.pageCount);
            }
        });
    }

    private createThreeDots(): HTMLLIElement {
        var element = document.createElement("li");
        $(element).addClass("disabled")
            .addClass("three-dots");

        var contentElement = document.createElement("span");
        contentElement.innerText = "...";

        element.appendChild(contentElement);
        return element;
    }

    private getCurrentPageElement(): JQuery {
        var selector = "li:has(*[data-page-number=\"" + this.currentPage + "\"])";
        return $(selector);
    }

    private searchAndDisplay(pageNumber: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchHeadword",
            data: {
                query: this.currentQuery,
                page: pageNumber,
                pageSize: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.display(pageNumber, response);
            }
        });
    }

    private display(pageNumber: number, headwords: IHeadword[]) {
        this.getCurrentPageElement().removeClass("active");
        this.currentPage = pageNumber;
        this.showHeadwords(headwords);
        this.getCurrentPageElement().addClass("active");
        this.updateVisiblePageElements();
    }

    private updateVisiblePageElements() {
        if (!this.usePaginationDots)
            return;

        var pageNumber = this.currentPage;
        var centerVisibleIndex = (this.maxPageElements - 1) / 2;
        var paginationListUl = $(this.paginationContainer).children().children();
        for (var i = 2; i < paginationListUl.length-2; i++) {
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
            for (var j = 0; j < visibleInCenter+1; j++) {
                $(paginationListUl[j+3]).removeClass("hidden");
            }
        }
        else if (!rightDotsHidden) {
            for (var l = 0; l < visibleInCenter + 1; l++) {
                $(paginationListUl[this.pageCount-l]).removeClass("hidden");
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

    public showHeadwords(headwords: IHeadword[]) {
        $(this.headwordListContainer).empty();
        $(this.headwordDescriptionContainer).empty();

        var listUl = document.createElement("ul");
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
                
                headwordLi.appendChild(aLink);
            }
            
            listUl.appendChild(headwordLi);
        }

        $(this.headwordListContainer).append(listUl);
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

            if (pageNumber > 0 && pageNumber <= this.pageCount)
                this.searchAndDisplay(pageNumber);
        });

        pageLi.appendChild(pageLink);
        return pageLi;
    }

    public createPagination(pageCount: number) {
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
    }

    private showHeadwordDescription(headword: IHeadword) {
        //TODO
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