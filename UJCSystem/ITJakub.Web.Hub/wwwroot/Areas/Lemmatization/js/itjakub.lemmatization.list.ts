class LemmatizationList {
    private paginationContainer: string;
    private container: string;
    private pagination: Pagination;
    private tokenCount: number;
    private tokenPerPage = 50;
    private tableBody: HTMLTableSectionElement;

    constructor(container: string, paginationContainer: string) {
        this.paginationContainer = paginationContainer;
        this.container = container;
    }

    public make() {
        var table = document.createElement("table");
        var thead = document.createElement("thead");
        var headerTr = document.createElement("tr");
        var th1 = document.createElement("th");
        var th2 = document.createElement("th");
        var tbody = document.createElement("tbody");
        this.tableBody = tbody;
        
        $(th1).addClass("column-token")
            .text("Token");
        $(th2).addClass("column-description")
            .text("Popis");
        $(headerTr)
            .append(th1)
            .append(th2);
        $(thead).append(headerTr);
        $(table)
            .addClass("lemmatization-table")
            .append(thead)
            .append(tbody);

        this.pagination = new Pagination(this.paginationContainer, 15);
        
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/GetTokenCount",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: (tokenCount) => {
                this.tokenCount = tokenCount;
                this.pagination.createPagination(this.tokenCount, this.tokenPerPage, this.loadPage.bind(this));
            }
        });

        $(this.container).append(table);
    }
    
    private loadPage(pageNumber: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/GetTokenList",
            data: {
                start: pageNumber - 1,
                count: this.tokenPerPage
            },
            dataType: "json",
            contentType: "application/json",
            success: (tokenList: Array<IToken>) => {
                this.createTableContent(tokenList);
            }
        });
    }

    private createTableContent(tokenList: Array<IToken>) {
        $(this.tableBody).empty();

        for (var i = 0; i < tokenList.length; i++) {
            var token = tokenList[i];
            var tr = document.createElement("tr");
            var td1 = document.createElement("td");
            var td2 = document.createElement("td");
            var tokenLink = document.createElement("a");

            tokenLink.setAttribute("href", getBaseUrl() + "Lemmatization/Lemmatization?tokenId=" + token.id);
            $(tokenLink).text(token.text);
            $(td1).append(tokenLink);
            $(td2).text(token.description);

            $(tr).append(td1)
                .append(td2);

            $(this.tableBody).append(tr);
        }
    }
}