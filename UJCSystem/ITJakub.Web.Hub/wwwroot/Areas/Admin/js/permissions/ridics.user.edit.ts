$(document.documentElement).ready(() => {
    var userEditor = new UserEditor();
    userEditor.init();
});

class UserEditor {
    private readonly resetPassword: JQuery;
    private readonly userId: number;
    private readonly client: PermissionApiClient;
    private readonly errorHandler: ErrorHandler;

    constructor() {
        this.resetPassword = $("#resetPassword");
        this.userId = Number(getQueryStringParameterByName("userId"));
        this.client = new PermissionApiClient();
        this.errorHandler = new ErrorHandler();
    }

    init() {
        var alertHolder = $("#resetPasswordAlertHolder");

        this.resetPassword.on("click", (event) => {
            event.preventDefault();
            alertHolder.empty();

            this.client.resetUserPassword(this.userId).done(() => {
                const errorAlert = new AlertComponentBuilder(AlertType.Success)
                    .addContent(localization.translate("PasswordResetSuccess", "PermissionJs").value);
                alertHolder.empty().append(errorAlert.buildElement());
            }).fail(error => {
                const errorAlert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(this.errorHandler.getErrorMessage(error));
                alertHolder.empty().append(errorAlert.buildElement());
            });
        });
    }
}