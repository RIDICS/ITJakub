$(document.documentElement).ready(() => {
    var userId = $("#selected-item-div").data("user-id");
    var roleList = new ListWithPagination(`Permission/GetRolesByUser?userId=${userId}`, 10, "role", ViewType.Partial, true);
    roleList.init();
});
