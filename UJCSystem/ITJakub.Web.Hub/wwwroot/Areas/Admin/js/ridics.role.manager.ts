$(document.documentElement).ready(() => {
    var roleManager = new RoleManager();
    roleManager.init();

    var groupList = new ListWithPagination("Permission/GroupPermission", 10, "role", ViewType.Widget, roleManager.init);
    groupList.init();
});

class RoleManager {
    public init() {
        $(".role-row").click((event) => {
            const roleId = $(event.currentTarget).data("role-id");

            $.ajax({
                type: "GET",
                traditional: true,
                url: URI(getBaseUrl() + "Permission/UsersByRole").search(query => {
                    query.roleId = roleId;
                }).toString(),
                success: (response) => {
                    $("#user-list-container").html(response);
                    var userList = new ListWithPagination(`Permission/UsersByRole?roleId=${roleId}`, 10, "user", ViewType.Widget);
                    userList.init();
                    $("#user-section").removeClass('hide');
                },
            });
        });
    }
}