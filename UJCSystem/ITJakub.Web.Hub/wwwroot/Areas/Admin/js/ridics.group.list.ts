$(document.documentElement).ready(() => {
    var groupList = new ListWithPagination("Permission/GroupPermission", 10, "role");
    groupList.init();
});