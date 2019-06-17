$(document.documentElement).ready(() => {
    var userList = new UserList();
    userList.init();
});

class UserList {
    private pagination: Pagination;
    private pageSize: number;
    private totalCount: number;

    constructor() {
        this.pagination = new Pagination({
            container: document.getElementById("pagination") as HTMLDivElement,
            pageClickCallback: this.loadPage.bind(this)
        });
    }

    public init() {
        var $pagingInfo = $("#list-container .paging-info");
        this.pageSize = $pagingInfo.data("page-size");
        this.totalCount = $pagingInfo.data("total-count");

        this.pagination.make(this.totalCount, this.pageSize);
    }

    private loadPage(pageNumber: number) {
        var parameters = {
            start: (pageNumber - 1) * this.pageSize,
            count: this.pageSize,
            partial: true
        }
        var url = getBaseUrl() + "Permission/UserPermission?" + $.param(parameters);

        var $listContainer = $("#list-container");

        $listContainer
            .html("<div class=\"loader\"></div>")
            .load(url, null, (responseText, textStatus, xmlHttpRequest) => {
                if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                    var alert = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("ListError", "Admin").value);
                    $listContainer
                        .empty()
                        .append(alert.buildElement());
                }
            });
    }
}