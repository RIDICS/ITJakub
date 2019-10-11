$(document.documentElement).ready(() => {
    var filteringExpressionTable = new FilteringExpressionTable();
    filteringExpressionTable.init();
});

class FilteringExpressionTable {
    private readonly errorHandler: ErrorHandler;
    private readonly client: RepositoryImportApiClient;

    constructor() {
        this.errorHandler = new ErrorHandler();
        this.client = new RepositoryImportApiClient();
    }

    init() {
        $("#addFilteringExpressionRow").click(() => {
            this.client.getFilteringExpressionRow().done((response) => {
                $("#filteringExpressions> tbody:last-child").append(response);
                this.initRemoveButtons();
            });
        });

        this.initRemoveButtons();
    }

    initRemoveButtons() {
        $(".remove-expression").click((event) => {
            const targetEl = $(event.target as Node as Element);
            const row = targetEl.closest("tr");
            row.remove();
        });
    }
}