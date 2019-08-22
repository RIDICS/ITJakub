$(document.documentElement).ready(() => {
    var filteringExpressionTable = new FilteringExpressionTable();
    filteringExpressionTable.init();

    var externalRepositoryImportList = new ExternalRepositoryImportList();
    externalRepositoryImportList.init();
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
            $.ajax({
                type: "GET",
                dataType: "html",
                url: `${getBaseUrl()}RepositoryImport/FilteringExpressionSet/AddFilteringExpressionRow`,
                success: (partialView) => {
                    $("#filteringExpressions> tbody:last-child").append(partialView);
                    this.initRemoveButtons();
                }
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

class ExternalRepositoryImportList {
    private readonly errorHandler: ErrorHandler;
    private readonly client: RepositoryImportApiClient;

    constructor() {
        this.errorHandler = new ErrorHandler();
        this.client = new RepositoryImportApiClient();
    }

    init() {
        const checkboxes = $(".repositories input:checkbox");

        $("#selectAllRepositories").click(() => {
            checkboxes.prop("checked", $(".repositories input:checkbox:not(:checked)").length > 0);
        });
            
        checkboxes.click(() => {
            const button = $("#startImportBtn");
            if ($(".repositories input:checkbox:checked").length > 0) {
                button.removeClass("disabled");
            } else {
                button.addClass("disabled");
            }
        });
    }
}
