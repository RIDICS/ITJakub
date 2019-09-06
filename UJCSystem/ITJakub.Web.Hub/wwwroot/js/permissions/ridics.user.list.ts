$(document.documentElement).ready(() => {
    var userList = new ListWithPagination("Permission/UserPermission", "user", ViewType.Partial, true);
    userList.init();
});
