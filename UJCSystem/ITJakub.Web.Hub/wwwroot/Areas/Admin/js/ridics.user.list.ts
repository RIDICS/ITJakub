$(document.documentElement).ready(() => {
    var userList = new ListWithPagination("Permission/UserPermission", 10, "user");
    userList.init();
});
