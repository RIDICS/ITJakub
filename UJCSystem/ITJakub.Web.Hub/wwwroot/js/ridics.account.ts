$(document.documentElement).ready(() => {
    var accountManager = new AccountManager();
    accountManager.init();
});

class AccountManager {
    private readonly userId: number;
    private readonly oldEmailValue: string;
    private readonly emailContactType = "Email";
    private readonly successContactUpdateAlert: JQuery;
    private readonly confirmCodeSendAlert: JQuery;
    private readonly successConfirmContactAlert: JQuery;
    private readonly confirmContactDescriptionAlert: JQuery;
    private readonly errorContactUpdateAlert: JQuery;
    private readonly errorConfirmContactAlert: JQuery;
    private readonly emailIsNotVerifiedTitle: JQuery;
    private readonly confirmEmailPanel: JQuery;
    private readonly confirmEmailPanelBody: JQuery;

    private readonly resendConfirmCodeBtn: JQuery;
    private readonly confirmEmailCodeInput: JQuery;
    private readonly confirmEmailSubmit: JQuery;

    constructor() {
        this.userId = $("#userId").data("id");
        this.oldEmailValue = String($("#oldEmailValue").val());

        this.successContactUpdateAlert = $("#successContactUpdate");
        this.confirmCodeSendAlert = $("#confirmCodeSend");
        this.successConfirmContactAlert = $("#successConfirmContact");
        this.confirmContactDescriptionAlert = $("#confirmContactDescription");

        this.errorContactUpdateAlert = $("#errorContactUpdate");
        this.errorConfirmContactAlert = $("#errorConfirmContact");

        this.emailIsNotVerifiedTitle = $(".email-warning");

        this.confirmEmailPanel = $("#confirmEmailPanel");
        this.confirmEmailPanelBody = $("#confirmEmailPanelBody");

        this.resendConfirmCodeBtn = $("#resendConfirmCode");
        this.confirmEmailCodeInput = $("#confirmEmailCodeInput");
        this.confirmEmailSubmit = $("#confirmEmailSubmit");
    }

    init() {
        $("#account-edit-button").click((event) => {
            event.preventDefault();
            $(".editable").prop("readonly", false);
            $("#account-view-button-panel").addClass("hide");
            $("#account-editor-button-panel").removeClass("hide");
        });

        $("#account-cancel-button").click((event) => {
            event.preventDefault();
            $(".editable").prop("readonly", true);
            $("#account-editor-button-panel").addClass("hide");
            $("#account-view-button-panel").removeClass("hide");
        });

        $("#updateEmailSubmit").click((event) => {
            event.preventDefault();
            this.sendUpdateContactRequest();
        });

        this.confirmEmailSubmit.click((event) => {
            event.preventDefault();
            this.sendConfirmContactRequest();
        });

        this.resendConfirmCodeBtn.click((event) => {
            event.preventDefault();
            this.hideAlert(this.confirmCodeSendAlert);
            this.hideAlert(this.errorConfirmContactAlert);

            this.resendConfirmCode(this.emailContactType).then((response) => {
                if (response.hasOwnProperty("message")) {
                    this.showAlert(this.errorConfirmContactAlert.text(response.message));
                } else {
                    this.showAlert(this.confirmCodeSendAlert);
                }
            });
        });
    }

    sendUpdateContactRequest() {
        var email = $("#emailInput").val() as string;
        this.updateContact(this.emailContactType, email).then((response) => {
            this.hideAlert(this.errorContactUpdateAlert);
            if (response.hasOwnProperty("message")) {
                this.showAlert(this.errorContactUpdateAlert.text(response.message));
            } else if (response === "same-email") {
                this.showAlert(this.errorContactUpdateAlert.text(localization.translate("SameEmail", "Account").value));
            } else if (response === "empty-email") {
                this.showAlert(
                    this.errorContactUpdateAlert.text(localization.translate("EmptyEmail", "Account").value));
            } else {
                this.showAlert(this.successContactUpdateAlert);
                this.emailIsNotVerifiedTitle.removeClass("hide");
                this.confirmEmailPanel.switchClass("panel-default", "panel-warning");
                this.confirmEmailPanel.removeClass("hide");
                this.showAlert(this.confirmCodeSendAlert);
                this.confirmEmailPanelBody.collapse("show");
            }
        });
    }

    sendConfirmContactRequest() {
        var confirmCode = this.confirmEmailCodeInput.val() as string;
        this.confirmContact(this.emailContactType, confirmCode).then((response) => {
            this.hideAlert(this.errorConfirmContactAlert);
            this.hideAlert(this.confirmCodeSendAlert);
            this.hideAlert(this.successConfirmContactAlert);

            if (response.hasOwnProperty("message")) {
                this.showAlert(this.errorConfirmContactAlert.text(response.message));
            } else if (response === false) {
                this.showAlert(
                    this.errorConfirmContactAlert.text(localization.translate("ConfirmCodeNotValid", "Account").value));
            } else {
                this.hideAlert(this.confirmContactDescriptionAlert);
                this.showAlert(this.successConfirmContactAlert);
                this.emailIsNotVerifiedTitle.addClass("hide");
                this.confirmEmailPanel.switchClass("panel-warning", "panel-default");
                this.hideAlert(this.confirmCodeSendAlert);
                this.showAlert(this.successConfirmContactAlert);

                this.resendConfirmCodeBtn.addClass("disabled");
                this.confirmEmailCodeInput.addClass("disabled");
                this.confirmEmailSubmit.addClass("disabled");
            }
        });
    }

    updateContact(contactType: string, newContactValue: string): JQueryPromise<any> {
        return this.post(getBaseUrl() + "Account/UpdateContact",
            JSON.stringify({
                NewContactValue: newContactValue,
                ContactType: contactType,
                oldContactValue: this.oldEmailValue
            }));
    }

    confirmContact(contactType: string, confirmCode: string): JQueryPromise<any> {
        return this.post(getBaseUrl() + "Account/ConfirmUserContact",
            JSON.stringify({ confirmCode: confirmCode, ContactType: contactType, userId: this.userId }));
    }

    resendConfirmCode(contactType: string): JQueryPromise<any> {
        return this.post(getBaseUrl() + "Account/ResendConfirmCode",
            JSON.stringify({ ContactType: contactType, userId: this.userId }));
    }

    private showAlert(alert: JQuery) {
        alert.removeClass("hide").addClass("in");
    }

    private hideAlert(alert: JQuery) {
        alert.removeClass("in").addClass("hide");
    }

    private post(url: string, data: string) {
        return $.ajax({
            type: "POST",
            traditional: true,
            url: url,
            data: data,
            dataType: "json",
            contentType: "application/json"
        });
    }
}