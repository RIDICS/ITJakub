$(document.documentElement).ready(() => {
    var roleList = new ListWithPagination("Permission/RolePermission", 10, "role", ViewType.Partial, true);
    roleList.init();
});