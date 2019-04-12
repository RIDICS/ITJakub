$(document.documentElement).ready(() => {
    var filteringExpressionTable = new FilteringExpressionTable();
    filteringExpressionTable.init();

    var externalRepositoryConfiguration = new ExternalRepositoryConfiguration();
    externalRepositoryConfiguration.init();

    var externalRepositoryImportList = new ExternalRepositoryImportList();
    externalRepositoryImportList.init();
});

class FilteringExpressionTable {
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

class ExternalRepositoryConfiguration {
    init() {
        $(".repository-detail").click((e) => {
            const repositoryId = $(e.target as Node as Element).data("repository-id");
            $.ajax({
                type: "GET",
                dataType: "html",
                url: `${getBaseUrl()}RepositoryImport/ExternalRepository/Detail?id=${repositoryId}`,
                success: (partialView) => {
                    $(`#repository-${repositoryId} .bib-table:last-child`).html(partialView);
                }
            });
        });

        $(".ResourceType").change((e) => {
            const api = $(e.target as Node as Element).children("option:selected").text();
            const config = $(".repository-configuration").val();
            
            let configParam = "";
            if (config !== "undefined") {
                configParam = `&config=${config}`;
            }

            $.ajax({
                    type: "GET",
                    url: `${getBaseUrl()}RepositoryImport/ExternalRepository/LoadApiConfiguration?api=${api}${configParam}`,
                    dataType: "html",
                    success: (data) => {
                        $("#apiType").val(api);
                        $("#apiOptions").html(data);
                        this.initOaiPmh();
                    }
                });
        });

        $(".ResourceType").change();
    }

    initOaiPmh() {
        $("#OaiPmhConnect").click(() => {
            const config = $(".repository-configuration").val();
            $.ajax({
                type: "GET",
                url: `${getBaseUrl()}RepositoryImport/ExternalRepository/OaiPmhConnect?url=${$("#OaiPmhResourceUrl")
                    .val()}&config=${config}`,
                dataType: "html",
                success: (data) => {
                    $("#oaiPmhConfig").html(data);
                }
            });
        });
    }
}

class ExternalRepositoryImportList {
    init() {
        const checkboxes = $(".repositories input:checkbox");

        $("#select-all-repositories").click(() => {
            if ($(".repositories input:checkbox:not(:checked)").length > 0) {
                checkboxes.prop('checked', true);
            } else {
                checkboxes.prop('checked', false);
            }
        });
            
        checkboxes.click(() => {
            const button = $("#start-import-btn");
            if ($(".repositories input:checkbox:checked").length > 0) {
                button.removeClass("disabled");
            } else {
                button.addClass("disabled");
            }
        });
    }
}
