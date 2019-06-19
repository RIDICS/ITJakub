$(document.documentElement).ready(() => {
    var userList = new ListWithPagination("Permission/GroupPermission", 10);
    userList.init();
});