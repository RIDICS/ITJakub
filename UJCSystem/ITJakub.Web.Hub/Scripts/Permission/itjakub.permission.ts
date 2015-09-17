$(document).ready(() => {
    var permissionEditor = new UserPermissionEditor("#mainContainer");
    permissionEditor.make();
});

class UserPermissionEditor {
    private mainContainer: string;
    private searchBox: ConcreteInstanceSearchBox;
    private currentSelectedItem: IUser;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.searchBox = new ConcreteInstanceSearchBox("#mainSearchInput", "Permission", this.getDefaultSuggestionTemplate );
    }

    private getDefaultSuggestionTemplate(item: IUser): string {
        return "<div><div class=\"suggestion\" style='font-weight: bold'>" + item.Username + "</div><div class=\"description\">" + item.Email + "</div></div>";
    }

    public make() {
        $(this.mainContainer).empty();
        this.searchBox.setDataSet("Token");
        this.searchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (selectedExists || this.searchBox.getInputValue() === "") {
                $("#createUserButton").addClass("hidden");
                $("#loadButton").removeClass("hidden");
            } else {
                $("#createUserButton").removeClass("hidden");
                $("#loadButton").addClass("hidden");
            }

            if (selectionConfirmed) {
                var selectedItem = this.searchBox.getValue();
                var user: IUser = {
                    Id: selectedItem.Id,
                    Username: selectedItem.Text,
                    Email: selectedItem.Description
                };
                this.loadUser(user);
            }
        });

        $("#loadButton").click(() => {
            var selectedItem = this.searchBox.getValue();
            var user: IUser = {
                Id: selectedItem.Id,
                Username: selectedItem.Text,
                Email: selectedItem.Description
            };
            this.loadUser(user);
        });

        $("#createUserButton").click(() => {
            this.showAddNewUser();
        });

        $("#save-user").click(() => {
            this.saveUser();
        });
    }

    private loadUser(user: IUser) {
        this.currentSelectedItem = user;
        $(".content").removeClass("hidden");
        $("#specificUserUsername").text(user.Username);
        $("#specificUserEmail").text(user.Email);

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/GetTokenCharacteristic",
            data: {
                tokenId: user.Id
            },
            dataType: "json",
            contentType: "application/json",
            success: (list) => {
 
            }
        });
    }

    private showAddNewUser() {
        var tokenName = this.searchBox.getInputValue();
        $("#new-token").val(tokenName);
        $("#new-token-description").val("");

        $("#newTokenDialog").modal({
            show: true,
            backdrop: "static"
        });
    }

    private saveUser() {
        var token = $("#new-token").val();
        var description = $("#new-token-description").val();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/CreateToken",
            data: {
                token: token,
                description: description
            },
            dataType: "json",
            contentType: "application/json",
            success: (newTokenId) => {
                $("#newTokenDialog").modal("hide");
                $("#addNewTokenButton").addClass("hidden");
                this.searchBox.reload();

                var user: IUser = {
                    Id: newTokenId,
                    Username: token,
                    Email: description
                };
                this.loadUser(user);
            }
        });
    }
}

interface IUser {
    Id: number;
    Username: string;
    Email: string;
}