class ListWithPagination {
    private readonly firstPageNumber = 1;
    private readonly pagingInfoSelector = ".paging-info";

    private readonly urlPath: string;
    private readonly selector: string;
    private readonly viewType: ViewType;
    private readonly saveStateToUrl: boolean;
    private readonly pageLoadCallback: (list: ListWithPagination) => void;
    private readonly contextForCallback;
    private readonly listContainerSelector;
    private readonly pagination: Pagination;
    private readonly searchForm: JQuery;
    private readonly adminApiClient = new AdminApiClient();

    private resetSearchForm: JQuery;
    private pageSize: number;
    private totalCount: number;
    private search: string;

    constructor(urlPath: string, selector: string, viewType: ViewType, saveStateToUrl: boolean);
    constructor(urlPath: string, selector: string, viewType: ViewType, saveStateToUrl: boolean, pageLoadCallback: (list?: ListWithPagination) => void, contextForCallback: any);
    constructor(urlPath: string, selector: string, viewType: ViewType, saveStateToUrl: boolean, pageLoadCallback?: (list: ListWithPagination) => void, contextForCallback?: any) {
        this.urlPath = urlPath;
        this.selector = selector;
        this.viewType = viewType;
        this.saveStateToUrl = saveStateToUrl;
        this.pageLoadCallback = pageLoadCallback;
        this.contextForCallback = contextForCallback;
        this.listContainerSelector = `#${this.selector}ListContainer`;
        this.pagination = new Pagination({
            container: document.getElementById(selector + "Pagination") as HTMLDivElement,
            pageClickCallback: this.loadPage.bind(this)
        });
        this.searchForm = $(`.${this.selector}-search-form`);
        this.search = null;
    }

    public init() {
        this.searchForm.off("submit");
        this.searchForm.on("submit", (event) => {
            const searchValue = this.searchForm.find(".search-value").val() as string;
            event.preventDefault();
            this.search = searchValue;
            this.loadPage(this.firstPageNumber);
        });

        const resetSearchButton = this.searchForm.find(`.reset-search-button`);
        resetSearchButton.click(() => {
            if (this.search == null) {
                this.searchForm.find(".search-value").val("");
            } else {
                this.resetSearch();
            }
        });

        let startPage = this.pagination.getCurrentPage();

        this.initPagination();

        if (typeof startPage === "undefined") {
            if (this.saveStateToUrl) {
                const uriSearch = new URI(window.location.href).search(true);
                const startItem = uriSearch.start;
                startPage = this.computeInitPage(this.pageSize, startItem);

                if (isNaN(startPage)) {
                    startPage = this.firstPageNumber;
                }
            } else {
                startPage = this.firstPageNumber;
            }
        }

        this.renderPaginationContainer(startPage);
        this.setUri(this.computeStartItem(this.pageSize, startPage), this.pageSize);
    }

    public reloadPage() {
        this.loadPage(this.pagination.getCurrentPage());
    }

    public clear(emptyListMessage: string) {
        const section = $(`#${this.selector}-section .section`);
        this.setSearchFormDisabled();
        $(`#${this.selector}Pagination`).empty();

        const container = section.find(`.list-container`);
        const infoAlert = new AlertComponentBuilder(AlertType.Info).addContent(emptyListMessage);
        container.empty().append(infoAlert.buildElement());
    }

    public setSearchFormDisabled(disabled = true) {
        const section = $(`#${this.selector}-section .section`);
        const searchForm = section.find(`.${this.selector}-search-form`);
        searchForm.find("input").prop("disabled", disabled);
        searchForm.find("submit").prop("disabled", disabled);
        searchForm.find("button").prop("disabled", disabled);
    }

    private initPagination() {
        const pagingInfo = $(this.listContainerSelector + " " + this.pagingInfoSelector);
        if (pagingInfo.length !== 0) {
            this.pageSize = pagingInfo.data("page-size");
            this.totalCount = pagingInfo.data("total-count");
        } else {
            this.pageSize = 1;
            this.totalCount = 0;
        }
    }

    private loadPage(pageNumber: number) {
        const start = this.computeStartItem(this.pageSize, pageNumber);
        const url = new URI(getBaseUrl() + this.urlPath).search((query) => {
            query.start = start;
            query.count = this.pageSize;
            query.viewType = this.viewType;
            if (this.search != null) {
                query.search = this.search;
            }
        }).toString();

        const $listContainer = $(this.listContainerSelector);
        $listContainer.html("<div class=\"loader\"></div>");

        this.adminApiClient.getHtmlPageByUrl(url).done((response) => {
            $listContainer.html(String(response));
            this.initPagination();

            this.setUri(start, this.pageSize, this.search);
            this.renderPaginationContainer(pageNumber);

            if (this.search) {
                this.resetSearchForm = $listContainer.find(".reset-search-form");
                this.resetSearchForm.submit((event) => {
                    event.preventDefault();
                    this.resetSearch();
                });
            }

            if (typeof this.pageLoadCallback !== "undefined") {
                this.pageLoadCallback.call(this.contextForCallback, this);
            }
        }).fail(() => {
            var alert = new AlertComponentBuilder(AlertType.Error).addContent(localization
                .translate("ListError", "PermissionJs").value);
            $listContainer
                .empty()
                .append(alert.buildElement());
        });
    }

    private resetSearch() {
        this.searchForm.find(".search-value").val("");
        this.removeSearchFromUri();
        this.search = null;
        this.loadPage(this.firstPageNumber);
    }

    private computeInitPage(itemsPerPage: number, startItem: number): number {
        return Math.ceil(startItem / itemsPerPage + 1);
    }

    private computeStartItem(numberPerPage: number, currentPage: number) {
        return numberPerPage * (currentPage - 1);
    }

    private setUri(startItemNumber: number, itemsOnPageCount: number, search: string = null) {
        if (this.saveStateToUrl) {
            const newUri = new URI(window.location.href).search((query) => {
                query.start = startItemNumber;
                query.count = itemsOnPageCount;
                if (search != null) {
                    query.search = search;
                }
            }).toString();
            history.replaceState(null, null, newUri);
        }
    }

    private removeSearchFromUri() {
        if (this.saveStateToUrl) {
            const newUri = new URI(window.location.href).removeSearch("search").toString();
            history.replaceState(null, null, newUri);
        }
    }

    private renderPaginationContainer(activePage: number) {
        this.pagination.make(this.totalCount, this.pageSize, activePage);
    }
}

enum ViewType {
    Full,
    Partial,
    Widget,
}