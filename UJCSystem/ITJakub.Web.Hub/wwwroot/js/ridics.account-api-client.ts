class AccountApiClient extends WebHubApiClient  {

    public updateContact(contactType: string, newContactValue: string, oldContactValue: string): Promise<JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "UpdateContact",
            JSON.stringify({
                NewContactValue: newContactValue,
                ContactType: contactType,
                oldContactValue: oldContactValue
            }));
    }

    public confirmContact(contactType: string, confirmCode: string): Promise<JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "ConfirmUserContact",
            JSON.stringify({ confirmCode: confirmCode, ContactType: contactType}));
    }

    public resendConfirmCode(contactType: string): Promise<JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "ResendConfirmCode",
            JSON.stringify({ ContactType: contactType}));
    }

    public updatePassword(passwordForm: string): Promise<JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "/UpdatePassword",
            passwordForm,
            "application/x-www-form-urlencoded"
        );
    }

    public updateAccount(accountDataForm: string): Promise<JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "/UpdateAccount",
            accountDataForm,
            "application/x-www-form-urlencoded"
        );
    }

    public setTwoFactor(twoFactorDataForm: string): Promise<JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "/SetTwoFactor",
            twoFactorDataForm,
            "application/x-www-form-urlencoded"
        );
    }

    public changeTwoFactorProvider(twoFactorDataForm: string): Promise<JQuery.jqXHR> {
        return this.post(this.getAccountControllerUrl() + "/ChangeTwoFactorProvider",
            twoFactorDataForm,
            "application/x-www-form-urlencoded"
        );
    }

    private getAccountControllerUrl(): string {
        return getBaseUrl() + "Account/";
    }
}