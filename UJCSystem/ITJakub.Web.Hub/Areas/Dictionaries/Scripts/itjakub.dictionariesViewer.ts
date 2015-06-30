class DictionaryViewer {
    private headwordDescriptionContainer: string;
    private paginationContainer: string;
    private headwordListContainer: string;
    private currentQuery: string;
    private pageSize = 20;
    private pageCount: number;

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
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetResultCountOfSearch",
            data: {
                query: this.currentQuery
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                //TODO save and display result count (headwords and fulltext)
                this.pageCount = 19;
                this.createPagination(this.pageCount);
            }
        });
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
                this.showHeadwords(response);
            }
        });
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
                aLink.innerText = dictionary.BookAcronym;
                $(aLink).addClass("dictionary-result-headword-book");
                
                headwordLi.appendChild(aLink);
            }
            
            listUl.appendChild(headwordLi);
        }

        $(this.headwordListContainer).append(listUl);
    }

    private createPageElement(label: string): HTMLLIElement {
        var pageLi = document.createElement("li");
        var pageA = document.createElement("a");
        pageA.innerText = label;

        $(pageA).click(event => {
            var clickedLink: HTMLLinkElement = <HTMLLinkElement>(event.target);
            var pageNumber: number = Number(clickedLink.innerText);
            //TODO don't use innerHTML
            this.searchAndDisplay(pageNumber);
        });

        pageLi.appendChild(pageA);
        return pageLi;
    }

    public createPagination(recordCount: number) {
        $(this.paginationContainer).empty();

        var paginationUl = document.createElement("ul");
        $(paginationUl).addClass("pagination")
            .addClass("pagination-sm");

        var pageCount = Math.ceil(recordCount / this.pageSize);

        var previousPageLi = this.createPageElement("&laquo;");
        paginationUl.appendChild(previousPageLi);

        for (var i = 1; i <= pageCount; i++) {
            var pageLi = this.createPageElement(String(i));
            paginationUl.appendChild(pageLi);
        }

        var nextPageLi = this.createPageElement("&raquo;");
        paginationUl.appendChild(nextPageLi);

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