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
        this.searchBox = new ConcreteInstanceSearchBox("#mainSearchInput", "Permission", this.getPrintableItem );
    }

    public getPrintableItem(item: IUser): IPrintableItem {
        var printableUser: IPrintableItem = {
            Name: item.UserName,
            Description: item.Email
        };
        return printableUser;
    }

    public make() {
        $(this.mainContainer).empty();
        this.searchBox.setDataSet("User");
        this.searchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (selectedExists || this.searchBox.getInputValue() === "") {
                $("#loadButton").removeClass("hidden");
            } else {
                $("#loadButton").addClass("hidden");
            }

            if (selectionConfirmed) {
                var selectedItem = <IUser>this.searchBox.getValue();
                this.loadUser(selectedItem);
            }
        });

        $("#loadButton").click(() => {
            var selectedItem = <IUser>this.searchBox.getValue();
            this.loadUser(selectedItem);
        });

        $("#save-user").click(() => {
            this.saveUser();
        });
    }

    private loadUser(user: IUser) {
        this.currentSelectedItem = user;
        $(".content").removeClass("hidden");
        $("#specificUserUsername").text(user.UserName);
        $("#specificUserEmail").text(user.Email);
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
                    UserName: token,
                    Email: description
                };
                this.loadUser(user);
            }
        });
    }
}

interface IUser {
    Id: number;
    UserName: string;
    Email: string;
}