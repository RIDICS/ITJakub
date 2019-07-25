class AccountApiClient extends WebHubApiClient  {

    public updateContact(contactType: string, newContactValue: string, oldContactValue: string): JQuery.jqXHR<string> {
        return this.post(this.getAccountControllerUrl() + "UpdateContact",
            JSON.stringify({
                NewContactValue: newContactValue,
                ContactType: contactType,
                oldContactValue: oldContactValue
            }));
    }

    public confirmContact(contactType: string, confirmCode: string): JQuery.jqXHR<boolean> {
        return this.post(this.getAccountControllerUrl() + "ConfirmUserContact",
            JSON.stringify({ confirmCode: confirmCode, ContactType: contactType}));
    }

    public resendConfirmCode(contactType: string): JQuery.jqXHR<string> {
        return this.post(this.getAccountControllerUrl() + "ResendConfirmCode",
            JSON.stringify({ ContactType: contactType}));
    }

    public updatePassword(passwordForm: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "UpdatePassword",
            passwordForm,
            this.formContentType,
            this.htmlDataType
        );
    }

    public updateAccount(accountDataForm: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "UpdateAccount",
            accountDataForm,
            this.formContentType,
            this.htmlDataType
        );
    }

    public setTwoFactor(twoFactorDataForm: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "SetTwoFactor",
            twoFactorDataForm,
            this.formContentType,
            this.htmlDataType
        );
    }

    public changeTwoFactorProvider(twoFactorDataForm: string): JQuery.jqXHR {
        return this.post(this.getAccountControllerUrl() + "ChangeTwoFactorProvider",
            twoFactorDataForm,
            this.formContentType,
            this.htmlDataType
        );
    }

    private getAccountControllerUrl(): string {
        return getBaseUrl() + "Account/";
    }
}