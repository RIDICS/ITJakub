$(document.documentElement).ready(() => {
    var accountManager = new AccountManager();
    accountManager.init();
});

class AccountManager {
    private readonly client: AccountApiClient;

    private accountDataForm: JQuery;
    private passwordForm: JQuery;
    private setTwoFactorForm: JQuery;
    private changeTwoFactorProviderForm: JQuery;

    private accountSection: JQuery;
    private passwordSection: JQuery;
    private twoFactorSection: JQuery;
    //Email confirm
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
        this.client = new AccountApiClient();

        this.accountSection = $("#update-account");
        this.passwordSection = $("#update-password");
        this.twoFactorSection = $("#update-two-factor-verification");

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

            this.client.resendConfirmCode(this.emailContactType).then((response) => {
                if (response.hasOwnProperty("message")) {
                    this.showAlert(this.errorConfirmContactAlert.text(((response) as any).message));
                } else {
                    this.showAlert(this.confirmCodeSendAlert);
                }
            });
        });

        this.initAccountDataForm();
        this.initPasswordForm();
        this.initTwoFactorSettingsForm();
    }

    initAccountDataForm() {
        this.accountDataForm = $("#updateAccountForm");
       
        this.accountDataForm.on("submit",
            (event) => {
                event.preventDefault();
                if (this.accountDataForm.valid()) {
                    this.client.updateAccount(this.accountDataForm.serialize())
                        .then((response) => {
                            this.accountSection.html(response.responseText);
                            this.initAccountDataForm();
                        })
                        .catch((response) => {
                            this.accountSection.html(response.responseText);
                            this.initAccountDataForm();
                        });
                }
            });
    }

    initPasswordForm() {
        this.passwordForm = $("#updatePasswordForm");
        
        this.passwordForm.on("submit",
            (event) => {
                event.preventDefault();
                if (this.passwordForm.valid()) {
                    this.client.updatePassword(this.passwordForm.serialize())
                        .then((response) => {
                            this.passwordSection.html(response.responseText);
                            this.initPasswordForm();
                        })
                        .catch((response) => {
                            this.passwordSection.html(response.responseText);
                            this.initPasswordForm();
                        });
                }
            });
    }

    initTwoFactorSettingsForm() {
        this.setTwoFactorForm = $("#setTwoFactorForm");
        this.changeTwoFactorProviderForm = $("#changeTwoFactorProviderForm");

        this.setTwoFactorForm.on("submit",
            (event) => {
                event.preventDefault();
                if (this.setTwoFactorForm.valid()) {
                    this.client.setTwoFactor(this.setTwoFactorForm.serialize())
                        .then((response) => {
                            this.twoFactorSection.html((response.responseText) as any);
                            this.initTwoFactorSettingsForm();
                        })
                        .catch((error) => {
                            this.twoFactorSection.html((error.responseText) as any);
                            this.initTwoFactorSettingsForm();
                        });
                }
            });

        this.changeTwoFactorProviderForm.on("submit",
            (event) => {
                event.preventDefault();
                if (this.changeTwoFactorProviderForm.valid()) {
                    this.client.changeTwoFactorProvider(this.changeTwoFactorProviderForm.serialize())
                        .then((response) => {
                            this.twoFactorSection.html((response.responseText) as any);
                            this.initTwoFactorSettingsForm();
                        })
                        .catch((error) => {
                            this.twoFactorSection.html((error.responseText) as any);
                            this.initTwoFactorSettingsForm();
                        });
                }
            });
    }


    sendUpdateContactRequest() {
        var email = $("#emailInput").val() as string;
        this.client.updateContact(this.emailContactType, email, this.oldEmailValue).then((response) => {
            this.hideAlert(this.errorContactUpdateAlert);
            if (response.hasOwnProperty("message")) {
                this.showAlert(this.errorContactUpdateAlert.text(((response) as any).message));
            } else if (String(response) === "same-email") {
                this.showAlert(this.errorContactUpdateAlert.text(localization.translate("SameEmail", "Account").value));
            } else if (String(response) === "empty-email") {
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
        this.client.confirmContact(this.emailContactType, confirmCode).then((response) => {
            this.hideAlert(this.errorConfirmContactAlert);
            this.hideAlert(this.confirmCodeSendAlert);
            this.hideAlert(this.successConfirmContactAlert);

            if (response.hasOwnProperty("message")) {
                this.showAlert(this.errorConfirmContactAlert.text(response.responseText));
            } else if (String(response) === "false") {
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

    private showAlert(alert: JQuery) {
        alert.removeClass("hide").addClass("in");
    }

    private hideAlert(alert: JQuery) {
        alert.removeClass("in").addClass("hide");
    }
}