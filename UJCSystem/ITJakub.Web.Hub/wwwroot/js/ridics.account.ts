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

        $("#account-cancel-button").click((event) => {
            event.preventDefault();
            $(".editable").prop("readonly", true);
            $("#account-editor-button-panel").addClass("hide");
            $("#account-view-button-panel").removeClass("hide");
        });

        $("#updateEmail").click((event) => {
            event.preventDefault();
            this.sendUpdateContactRequest();
        });
    }

    sendUpdateContactRequest() {
        var email = $("#emailInput").val() as string;
        this.updateContact("Email", email).then((response) => {
            console.log(response);
        });
        
    }

    updateContact(contactType: string, newContactValue: string): JQueryPromise<any> {
       return $.post(getBaseUrl() + "Account/UpdateContact", JSON.stringify({ NewContactValue: newContactValue, ContactType: contactType }));
    }

    confirmContact() {
        
    }

    resendConfirmCode() {
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
