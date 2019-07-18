$(document.documentElement).ready(() => {
    var roleManager = new RoleManager();
    roleManager.init();
});

class RoleManager {
    private searchBox: SingleSetTypeaheadSearchBox<IUserDetail>;
    private currentUserSelectedItem: IUserDetail;
    public client;
    public userList;
    public roleList: ListWithPagination;

    constructor() {
        this.searchBox = new SingleSetTypeaheadSearchBox<IUserDetail>("#mainSearchInput",
            "Permission",
            this.getFullNameString,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(this.getFullNameString(item),
                item.email));
        this.client = new WebHubApiClient();
    }

    public init(list?: ListWithPagination) {
        if (list == null) {
            this.roleList = new ListWithPagination("Permission/RolePermission", 10, "role", ViewType.Widget, true, undefined, this);
            
        } else {
            this.roleList = list;
        }
        this.roleList.init();

        $(".role-row").click((event) => {
            $((event.currentTarget) as any).addClass("active").siblings().removeClass("active");

            var selectedRoleId = $((event.currentTarget) as any).data("role-id");
            this.loadUsers(selectedRoleId);
            this.loadPermissions(selectedRoleId);
        });

        this.initCreateRoleModal();
        this.initRemoveRoleButtons();
        this.initSearchBox();
    }

    private loadUsers(roleId: number) {
        var container = $("#user-list-container");
        container.html("<div class=\"loader\"></div>");

        this.client.getUsersByRole(roleId).then(response => {
            container.html((response) as any);
            this.userList = new ListWithPagination(`Permission/UsersByRole?roleId=${roleId}`,
                10,
                "user",
                ViewType.Widget,
                false,
                this.initRemoveUserFromRoleButton,
                this);
            this.userList.init();
            this.initRemoveUserFromRoleButton();
            $("#user-section .section").removeClass("hide");
        });
    }

    private loadPermissions(roleId: number) {
        var container = $("#permission-list-container");
        container.html("<div class=\"loader\"></div>");

        this.client.getPermissionsByRole(roleId).then(response => {
            container.html(response);
            var permissionList = new ListWithPagination(`Permission/RolePermissionList?roleId=${roleId}`,
                10,
                "permission",
                ViewType.Widget,
                false,
                this.initPermissionManaging,
                this);
            permissionList.init();
            this.initPermissionManaging();
            $("#permission-section .section").removeClass("hide");
        });
    }

    private initPermissionManaging() {
        $(".permission-checkbox input[type=checkbox]").change((event) => {
            var roleId = $(".role-row.active").data("role-id");
            var specialPermissionId = $((event.currentTarget) as any).parent("td").data("permission-id");

            if ($((event.currentTarget) as any).is(":checked")) {
                this.client.addSpecialPermissionToRole(roleId, specialPermissionId);
            } else {
                this.client.removeSpecialPermissionToRole(roleId, specialPermissionId);
            }
        });
    }

    private initRemoveUserFromRoleButton() {
        $(".remove-user-from-role").click((event) => {
            var userId = $((event.currentTarget) as any).data("user-id");
            var roleId = $(".role-row.active").data("role-id");
            this.client.removeUserFromRole(userId, roleId).then(() => {
                this.userList.reloadPage();
            });
        });
    }

    private initRemoveRoleButtons() {
        $(".remove-role").click((event) => {
            event.stopPropagation();
            var roleId = $((event.currentTarget) as any).parents("tr.role-row").data("role-id");
            this.client.deleteRole(roleId).then(() => {
                this.roleList.reloadPage();
            });
        });
    }

    private initSearchBox() {
        this.searchBox.setDataSet("User");
        this.searchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (!selectedExists || this.searchBox.getInputValue() === "") {

            }

            if (selectionConfirmed) {
                this.currentUserSelectedItem = this.searchBox.getValue();
                var userBox = $("#selectedUser");
                var name = this.getFullNameString(this.currentUserSelectedItem);
                userBox.text(name);
                userBox.data("user-id", this.currentUserSelectedItem.id);
            }
        });

        const addUserToRoleBtn = $("#add-user-to-role");
        if (addUserToRoleBtn.data("init") === false) {
            addUserToRoleBtn.data("init", true);

            $("#addRoleButton").click(() => {
                var role = $(".role-row.active");
                $("#specificRoleName").text(role.find(".name").text());
                $("#specificRoleDescription").text(role.find(".description").text());
                $("#addToRoleDialog").modal();
            });

            addUserToRoleBtn.click(() => {
                var roleId = $(".role-row.active").data("role-id");
                var userId: number;
                if (typeof this.currentUserSelectedItem == "undefined")
                    userId = $("#selectedUser").data("user-id");
                else {
                    userId = this.currentUserSelectedItem.id;
                }

                this.client.addUserToRole(userId, roleId).then(() => {
                    this.userList.reloadPage();
                    $("#addToRoleDialog").modal("hide");
                });
            });
        }
    }

    private getFullNameString(user: IUser): string {
        return `${user.userName} - ${user.firstName} - ${user.lastName}`;
    }

    private initCreateRoleModal() {
        $("#createRoleButton").click(() => {
            $("#createRoleModal").modal();
        });

        $("#create-role").click(() => {
            var roleName = $("#new-role-name").val() as string;
            var roleDescription = $("#new-role-description").val() as string;
            this.client.createRole(roleName, roleDescription).then(() => {
                $("#createRoleModal").modal("hide");
                this.roleList.reloadPage();
            });
        });
    }
}