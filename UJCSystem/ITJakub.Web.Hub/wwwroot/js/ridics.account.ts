$(document.documentElement).ready(() => {
    var accountManager = new AccountManager();
    accountManager.init();
});

class AccountManager {
    init() {
        $("#account-edit-button").click((event) => {
            event.preventDefault();
            $(".editable").prop("readonly", false);
            $("#account-view-button-panel").addClass("hide");
            $("#account-editor-button-panel").removeClass("hide");
        });

        $("#account-cancel-button").click(() => {
            event.preventDefault();
            $(".editable").prop("readonly", true);
            $("#account-editor-button-panel").addClass("hide");
            $("#account-view-button-panel").removeClass("hide");
            //OR reload page?
        });
    }
}
