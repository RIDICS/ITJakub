$(document).ready(() => {
    var newProjectDialog = new BootstrapDialogWrapper({
        element: $("#new-project-dialog"),
        autoClearInputs: true
    });

    var deleteProjectDialog = new BootstrapDialogWrapper({
        element: $("#delete-project-dialog"),
        autoClearInputs: false
    });
    
    $("#new-project-button").click((event) => {
        newProjectDialog.show();

        event.preventDefault();
    });

    $(".project-item .delete-button").click((event) => {
        var $projectItem = $(event.currentTarget).closest(".project-item");
        var projectId = Number($projectItem.data("project-id"));
        var projectName = $projectItem.data("project-name");

        $("#delete-project-name").text(projectName);
        deleteProjectDialog.show();

        event.preventDefault();
    });
});