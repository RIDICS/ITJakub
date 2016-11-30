$(document).ready(() => {
    var newProjectDialog = new BootstrapDialogWrapper({
        element: $("#new-project-dialog"),
        autoClearInputs: true
    });
    
    $("#new-project-button").click(() => {
        newProjectDialog.show();
    });
});