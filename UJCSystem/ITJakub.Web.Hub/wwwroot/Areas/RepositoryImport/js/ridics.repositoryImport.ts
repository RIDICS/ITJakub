$(document.documentElement).ready(() => {
    var filteringExpressionTable = new FilteringExpressionTable();
    filteringExpressionTable.init();
});

class FilteringExpressionTable {
    init() {    
        $("#addFilteringExpressionRow").click(() => {
            $.ajax({
                type: "GET",
                dataType: "html",
                url: `${getBaseUrl()}RepositoryImport/FilteringExpressionSet/AddFilteringExpressionRow`,
                success: (partialView) => {
                    $('#filteringExpressions> tbody:last-child').append(partialView);
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