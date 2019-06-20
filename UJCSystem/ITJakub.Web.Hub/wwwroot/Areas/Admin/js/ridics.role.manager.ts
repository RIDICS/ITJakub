$(document.documentElement).ready(() => {
    var roleManager = new RoleManager();
    roleManager.init();

    var groupList = new ListWithPagination("Permission/GroupPermission", 10, "role", ViewType.Widget, roleManager.init);
    groupList.init();
});

class RoleManager {
    private readonly roleSelector = ".role-row";

    public init() {
        $(this.roleSelector).click((event) => {
            const roleId = $(event.currentTarget).data("role-id");

            $.ajax({
                type: "GET",
                traditional: true,
                url: URI(getBaseUrl() + "Permission/UsersByRole").search(query => {
                    query.roleId = roleId;
                }).toString(),
                success: (response) => {
                    $("#user-list-container").html(response);
                    $("#user-section").removeClass('hide');
                },
            });
        });
    }
}