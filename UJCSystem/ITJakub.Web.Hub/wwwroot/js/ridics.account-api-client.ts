class AccountApiClient extends WebHubApiClient  {

    public updateContact(contactType: string, newContactValue: string, oldContactValue: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "UpdateContact",
            JSON.stringify({
                NewContactValue: newContactValue,
                ContactType: contactType,
                oldContactValue: oldContactValue
            }));
    }

    public confirmContact(contactType: string, confirmCode: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "ConfirmUserContact",
            JSON.stringify({ confirmCode: confirmCode, ContactType: contactType}));
    }

    public resendConfirmCode(contactType: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "ResendConfirmCode",
            JSON.stringify({ ContactType: contactType}));
    }

    public updatePassword(passwordForm: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "UpdatePassword",
            passwordForm,
            "application/x-www-form-urlencoded"
        );
    }

    public updateAccount(accountDataForm: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "UpdateAccount",
            accountDataForm,
            "application/x-www-form-urlencoded"
        );
    }

    public setTwoFactor(twoFactorDataForm: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "SetTwoFactor",
            twoFactorDataForm,
            "application/x-www-form-urlencoded"
        );
    }

    public changeTwoFactorProvider(twoFactorDataForm: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "ChangeTwoFactorProvider",
            twoFactorDataForm,
            "application/x-www-form-urlencoded"
        );
    }

    private getAccountControllerUrl(): string {
        return getBaseUrl() + "Account/";
    }
}