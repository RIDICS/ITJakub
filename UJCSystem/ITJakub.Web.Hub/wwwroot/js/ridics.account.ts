﻿$(document.documentElement).ready(() => {
    var accountManager = new AccountManager();
    accountManager.init();
});

class AccountManager {
    private readonly client: AccountApiClient;
    private readonly errorHandler: ErrorHandler;

    private basicDataForm: JQuery;
    private passwordForm: JQuery;
    private setTwoFactorForm: JQuery;
    private changeTwoFactorProviderForm: JQuery;

    private readonly accountSection: JQuery;
    private readonly passwordSection: JQuery;
    private readonly twoFactorSection: JQuery;
    private readonly userCodeSection: JQuery;
    //Email confirm
    private actualEmailValue: string;
    private newEmailValue: string;

    private readonly emailContactType = "Email";
    private readonly newEmailInputSelector = "#emailInput";
    private readonly alertHolderSelector = ".alert-holder";

    private readonly confirmContactDescriptionAlert: JQuery;

    private readonly emailIsNotVerifiedTitle: JQuery;
    private readonly updateEmailPanel: JQuery;
    private readonly confirmEmailPanel: JQuery;
    private readonly confirmEmailPanelBody: JQuery;

    private readonly resendConfirmCodeBtn: JQuery;
    private readonly confirmEmailCodeInput: JQuery;
    private readonly confirmEmailSubmit: JQuery;

    constructor() {
        this.client = new AccountApiClient();
        this.errorHandler = new ErrorHandler();

        this.accountSection = $("#updateAccount");
        this.passwordSection = $("#updatePassword");
        this.twoFactorSection = $("#updateTwoFactorVerification");
        this.userCodeSection = $("#userCode");

        this.actualEmailValue = String($("#oldEmailValue").val());

        this.confirmContactDescriptionAlert = $("#confirmContactDescription");

        this.emailIsNotVerifiedTitle = $(".email-warning");
        this.updateEmailPanel = $("#updateEmailPanel");
        this.confirmEmailPanel = $("#confirmEmailPanel");
        this.confirmEmailPanelBody = $("#confirmEmailPanelBody");

        this.resendConfirmCodeBtn = $("#resendConfirmCode");
        this.confirmEmailCodeInput = $("#confirmEmailCodeInput");
        this.confirmEmailSubmit = $("#confirmEmailSubmit");
    }

    init() {
        $("#updateEmailSubmit").on("click",(event) => {
            event.preventDefault();
            this.sendUpdateContactRequest();
        });

        this.confirmEmailSubmit.on("click", (event) => {
            event.preventDefault();
            this.sendConfirmContactRequest();
        });

        this.resendConfirmCodeBtn.on("click", (event) => {
            event.preventDefault();
            const alertHolder = this.confirmEmailPanel.find(this.alertHolderSelector);
            alertHolder.empty();

            this.client.resendConfirmCode(this.emailContactType).done(() => {
                const alert = new AlertComponentBuilder(AlertType.Success).addContent(localization
                    .translateFormat("ConfirmCodeSend", [this.actualEmailValue], "Account").value).buildElement();
                alertHolder.empty().append(alert);
            }).fail((response) => {
                const alert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(this.errorHandler.getErrorMessage(response)).buildElement();
                alertHolder.empty().append(alert);
            });
        });

        this.initAccountDataForm();
        this.initPasswordForm();
        this.initTwoFactorSettingsForm();
        this.initRegenerateUserCodeForm();
        
        $("#updateTwoFactorVerificationButton").on("click",
            () => {
                var loader = lv.create(null, "lv-circles sm lv-mid lvt-3");
                this.twoFactorSection.empty();
                this.twoFactorSection.append(loader.getElement());
                this.client.getTwoFactor().done((response) => {
                    this.twoFactorSection.html(response);
                    this.initTwoFactorSettingsForm();
                }).fail((response) => {
                    this.twoFactorSection.html(response.responseText);
                    this.initTwoFactorSettingsForm();
                });
            });
    }

    initAccountDataForm() {
        $("#account-edit-button").on("click", (event) => {
            event.preventDefault();
            $(".editable").prop("readonly", false);
            $("#account-view-button-panel").addClass("hide");
            $("#account-editor-button-panel").removeClass("hide");
        });

        $("#account-cancel-button").on("click", (event) => {
            event.preventDefault();
            $(".editable").prop("readonly", true);
            $("#account-editor-button-panel").addClass("hide");
            $("#account-view-button-panel").removeClass("hide");
        });

        this.basicDataForm = $("#updateBasicDataForm");
        const alertHolder = this.basicDataForm.find(this.alertHolderSelector);

        this.basicDataForm.on("submit",
            (event) => {
                event.preventDefault();
                alertHolder.empty();

                if (this.basicDataForm.valid()) {
                    this.client.updateAccount(this.basicDataForm.serialize())
                        .done((response) => {
                            this.accountSection.html(response);
                            this.initAccountDataForm();
                        })
                        .fail((response) => {
                            const alert = new AlertComponentBuilder(AlertType.Error)
                                .addContent(this.errorHandler.getErrorMessage(response)).buildElement();
                            alertHolder.empty().append(alert);
                        });
                }
            });
    }

    initPasswordForm() {
        this.passwordForm = $("#updatePasswordForm");
        const alertHolder = this.passwordForm.find(this.alertHolderSelector);

        this.passwordForm.on("submit",
            (event) => {
                event.preventDefault();
                alertHolder.empty();

                if (this.passwordForm.valid()) {
                    this.client.updatePassword(this.passwordForm.serialize())
                        .done((response) => {
                            this.passwordSection.html(response);
                            this.initPasswordForm();
                        })
                        .fail((response) => {
                            const alert = new AlertComponentBuilder(AlertType.Error)
                                .addContent(this.errorHandler.getErrorMessage(response)).buildElement();
                            alertHolder.empty().append(alert);
                        });
                }
            });
    }

    initTwoFactorSettingsForm() {
        this.setTwoFactorForm = $("#setTwoFactorForm");
        this.changeTwoFactorProviderForm = $("#changeTwoFactorProviderForm");
        const alertHolder = this.setTwoFactorForm.find(this.alertHolderSelector);

        this.setTwoFactorForm.on("submit",
            (event) => {
                event.preventDefault();
                alertHolder.empty();

                if (this.setTwoFactorForm.valid()) {
                    this.client.setTwoFactor(this.setTwoFactorForm.serialize())
                        .done((response) => {
                            this.twoFactorSection.html(response);
                            this.initTwoFactorSettingsForm();
                        })
                        .fail((response) => {
                            const alert = new AlertComponentBuilder(AlertType.Error)
                                .addContent(this.errorHandler.getErrorMessage(response)).buildElement();
                            alertHolder.empty().append(alert);
                        });
                }
            });

        this.changeTwoFactorProviderForm.on("submit",
            (event) => {
                event.preventDefault();
                alertHolder.empty();

                if (this.changeTwoFactorProviderForm.valid()) {
                    this.client.changeTwoFactorProvider(this.changeTwoFactorProviderForm.serialize())
                        .done((response) => {
                            this.twoFactorSection.html(response);
                            this.initTwoFactorSettingsForm();
                        })
                        .fail((response) => {
                            const alert = new AlertComponentBuilder(AlertType.Error)
                                .addContent(this.errorHandler.getErrorMessage(response)).buildElement();
                            alertHolder.empty().append(alert);
                        });
                }
            });
    }

    sendUpdateContactRequest() {
        this.newEmailValue = $(this.newEmailInputSelector).val() as string;
        const alertHolder = this.updateEmailPanel.find(this.alertHolderSelector);
        alertHolder.empty();
        this.client.updateContact(this.emailContactType, this.newEmailValue, this.actualEmailValue).done(() => {
            this.emailIsNotVerifiedTitle.removeClass("hide");
            this.confirmEmailPanel.switchClass("panel-default", "panel-warning");
            this.confirmEmailPanel.removeClass("hide");
            this.confirmEmailPanelBody.collapse("show");

            const alert = new AlertComponentBuilder(AlertType.Success).addContent(localization
                .translateFormat("SuccessContactUpdate", [this.newEmailValue], "Account").value).buildElement();
            alertHolder.empty().append(alert);
            this.actualEmailValue = this.newEmailValue;
        }).fail((response) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(response)).buildElement();
            alertHolder.empty().append(alert);
        });
    }

    sendConfirmContactRequest() {
        const confirmCode = this.confirmEmailCodeInput.val() as string;
        const alertHolder = this.confirmEmailPanel.find(this.alertHolderSelector);
        alertHolder.empty();
        this.client.confirmContact(this.emailContactType, confirmCode).done((response) => {
            if (response === false) {
                const alert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(localization.translate("ConfirmCodeNotValid", "Account").value).buildElement();
                alertHolder.empty().append(alert);
            } else {
                this.hideAlert(this.confirmContactDescriptionAlert);

                const alert = new AlertComponentBuilder(AlertType.Success)
                    .addContent(localization.translate("SuccessConfirmContact", "Account").value).buildElement();
                alertHolder.empty().append(alert);

                this.emailIsNotVerifiedTitle.addClass("hide");
                this.confirmEmailPanel.switchClass("panel-warning", "panel-default");
                
                this.resendConfirmCodeBtn.prop("disabled", true);
                this.confirmEmailCodeInput.prop("readonly", true);
                this.confirmEmailSubmit.prop("disabled", true);
            }
        }).fail((response) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(response)).buildElement();
            alertHolder.empty().append(alert);
        });
    }

    initRegenerateUserCodeForm() {
        const regenerateUserCodeForm = $("#regenerateUserCode");
        const alertHolder = regenerateUserCodeForm.find(this.alertHolderSelector);
        regenerateUserCodeForm.on("submit", (event) => {
            event.preventDefault();
            alertHolder.empty();
            
            this.client.regenerateUserCode()
                .done((response) => {
                    this.userCodeSection.html(response);
                    this.initRegenerateUserCodeForm();
                })
                .fail((response) => {
                    const alert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(this.errorHandler.getErrorMessage(response)).buildElement();
                    alertHolder.empty().append(alert);
                });
        });
    }

    private hideAlert(alert: JQuery) {
        alert.removeClass("in").addClass("hide");
    }
}