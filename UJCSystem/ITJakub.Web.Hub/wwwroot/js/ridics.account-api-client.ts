class AccountApiClient extends WebHubApiClient  {

    public updateContact(contactType: string, newContactValue: string, oldContactValue: string): JQuery.Promise<string, JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "UpdateContact",
            JSON.stringify({
                NewContactValue: newContactValue,
                ContactType: contactType,
                oldContactValue: oldContactValue
            }));
    }

    public confirmContact(contactType: string, confirmCode: string): JQuery.Promise<boolean, JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "ConfirmUserContact",
            JSON.stringify({ confirmCode: confirmCode, ContactType: contactType}));
    }

    public resendConfirmCode(contactType: string): JQuery.Promise<string> {
        return this.post(this.getAccountControllerUrl() + "ResendConfirmCode",
            JSON.stringify({ ContactType: contactType}));
    }

    public updatePassword(passwordForm: string): JQuery.Promise<JQuery.jqXHR, JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "UpdatePassword",
            passwordForm,
            this.formContentType
        );
    }

    public updateAccount(accountDataForm: string): JQuery.Promise<JQuery.jqXHR, JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "UpdateAccount",
            accountDataForm,
            this.formContentType
        );
    }

    public setTwoFactor(twoFactorDataForm: string): JQuery.Promise<JQuery.jqXHR, JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "SetTwoFactor",
            twoFactorDataForm,
            this.formContentType
        );
    }

    public changeTwoFactorProvider(twoFactorDataForm: string): JQuery.Promise<JQuery.jqXHR, JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "ChangeTwoFactorProvider",
            twoFactorDataForm,
            this.formContentType
        );
    }

    private getAccountControllerUrl(): string {
        return getBaseUrl() + "Account/";
    }
}