$(document.documentElement).ready(() => {
    var externalRepositoryImportList = new ExternalRepositoryImportList();
    externalRepositoryImportList.init();
});

class ExternalRepositoryImportList {
    init() {
        const checkboxes = $("#repositories input:checkbox");

        $("#selectAllRepositories").click(() => {
            checkboxes.prop("checked", $("#repositories input:checkbox:not(:checked)").length > 0).change();
        });
            
        checkboxes.on("change", () => {
            const button = $("#startImportBtn");
            if ($("#repositories input:checkbox:checked").length > 0) {
                button.removeClass("disabled");
            } else {
                button.addClass("disabled");
            }
        });
    }
}
