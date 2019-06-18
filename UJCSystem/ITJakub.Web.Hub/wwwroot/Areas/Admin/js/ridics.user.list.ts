$(document.documentElement).ready(() => {
    var userList = new UserList();
    userList.init();
});

class UserList {
    private readonly firstPageNumber = 1;
    private readonly userPageSize = 5;
    private readonly pagingInfoSelector = ".paging-info";
    private readonly listContainerSelector = "#list-container";
 
    private searchForm: JQuery;
    private resetSearchForm: JQuery;
    private pagination: Pagination;
    private pageSize: number;
    private totalCount: number;

    private search: string;

    constructor() {
        this.pagination = new Pagination({
            container: document.getElementById("pagination") as HTMLDivElement,
            pageClickCallback: this.loadPage.bind(this)
        });
    }

    public init() {
        this.searchForm = $(".search-form");
        this.searchForm.submit((event) => {
            const searchValue = this.searchForm.find(".search-value").val() as string;
            event.preventDefault();
            this.search = searchValue;
            this.loadPage(this.firstPageNumber);
        });

        let startPage = this.pagination.getCurrentPage();

        this.initPagination();

        if (typeof startPage === "undefined") {

            const uriSearch = new URI(window.location.href).search(true);
            const startItem = uriSearch.start;
            startPage = this.computeInitPage(this.pageSize, startItem);

            if (isNaN(startPage)) {
                startPage = this.firstPageNumber;
            }
        }

        this.renderPaginationContainer(startPage);
        this.setUri(this.computeStartItem(this.pageSize, startPage), this.pageSize);
    }


    private initPagination() {
        const pagingInfo = $(this.listContainerSelector + " " + this.pagingInfoSelector);
        if (pagingInfo.length !== 0) {
            this.pageSize = pagingInfo.data("page-size");
            this.totalCount = pagingInfo.data("total-count");
        } else {
            this.pageSize = this.userPageSize;
            this.totalCount = 0;
        }
    }

    private loadPage(pageNumber: number) {
        const start = this.computeStartItem(this.pageSize, pageNumber);
        const parameters = {
            start: start,
            count: this.pageSize,
            partial: true,
            search: this.search
        }
        const url = getBaseUrl() + "Permission/UserPermission?" + $.param(parameters);

        const $listContainer = $(this.listContainerSelector);

        $listContainer
            .html("<div class=\"loader\"></div>")
            .load(url,
                null,
                (responseText, textStatus, xmlHttpRequest) => {
                    if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                        this.totalCount = 0;
                        var alert = new AlertComponentBuilder(AlertType.Error).addContent(localization
                            .translate("ListError", "Admin").value);
                        $listContainer
                            .empty()
                            .append(alert.buildElement());
                    }

                    this.initPagination();

                    this.setUri(start, this.pageSize, this.search);
                    this.renderPaginationContainer(pageNumber);

                    if (this.search) {
                        this.resetSearchForm = $(".reset-search-form");
                        this.resetSearchForm.submit((event) => {
                            this.searchForm.find(".search-value").val("");
                            event.preventDefault();
                            this.removeSearchFromUri();
                            this.search = null;
                            this.loadPage(this.firstPageNumber);
                        });
                    }

                });
    }

    private computeInitPage(itemsPerPage: number, startItem: number): number {
        return Math.ceil(startItem / itemsPerPage + 1);
    }

    private computeStartItem(numberPerPage: number, currentPage: number) {
        return numberPerPage * (currentPage - 1);
    }

    private setUri(startItemNumber: number, itemsOnPageCount: number, search: string = null) {
        const newUri = new URI(window.location.href).search((query) => {
            query.start = startItemNumber;
            query.count = itemsOnPageCount;
            if (search != null) {
                query.search = search;
            }
        }).toString();

        history.replaceState(null, null, newUri);
    }

    private removeSearchFromUri() {
        const newUri = new URI(window.location.href).removeSearch("search").toString();
        history.replaceState(null, null, newUri);
    }

    private renderPaginationContainer(activePage: number) {
        this.pagination.make(this.totalCount,this.pageSize, activePage);
    }
}