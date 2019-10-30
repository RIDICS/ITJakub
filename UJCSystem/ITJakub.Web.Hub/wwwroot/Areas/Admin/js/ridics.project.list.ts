$(document.documentElement).ready(() => {
    const projectList = new ProjectList();
    projectList.init();
});

class ProjectList {
    private readonly projectListUrl = "Admin/Project/List";
    private projectClient: ProjectClient;
    private projectList: ListWithPagination;
    private newProjectDialog: BootstrapDialogWrapper;
    private deleteProjectDialog: BootstrapDialogWrapper;
    private projectIdForDelete: number;

    constructor() {
        this.projectClient = new ProjectClient();

        this.newProjectDialog = new BootstrapDialogWrapper({
            element: $("#new-project-dialog"),
            autoClearInputs: true,
            submitCallback: this.createNewProject.bind(this)
        });

        this.deleteProjectDialog = new BootstrapDialogWrapper({
            element: $("#delete-project-dialog"),
            autoClearInputs: false,
            submitCallback: this.deleteProject.bind(this)
        });
    }

    public init() {
        $("#new-project-button").click((event) => {
            this.newProjectDialog.show();
            event.preventDefault();
        });
        
        $("#projectOwnerFilter").change((event) => {
            const value = $(event.currentTarget).val();
            console.log(value);
            const url = new URI(this.projectListUrl).search((query) => {
                query.projectOwnerType = value;
            }).toString();
            
            this.projectList.setNewUrlPath(url);
            this.projectList.loadFirstPage();
        });
        
        this.projectList = new ListWithPagination(this.projectListUrl, "project", ViewType.Widget, true, false, this.reinitProjectListButtons, this);
        this.projectList.init();
        
        this.reinitProjectListButtons();
    }

    public reinitProjectListButtons() {
        $(".project-item .delete-button").click((event) => {
            const $projectItem = $(event.currentTarget as Node as Element).closest(".project-item");
            this.projectIdForDelete = Number($projectItem.data("project-id"));
            const projectName = $projectItem.data("project-name");

            $("#delete-project-name").text(projectName);
            this.deleteProjectDialog.show();

            event.preventDefault();
        });
    }

    private createNewProject() {
        const projectName = $("#new-project-name").val() as string;
        if(projectName.length == 0)
        {
            this.newProjectDialog.showError(localization.translate("EmptyProjectNameError", "Admin").value);
            return;
        }
        
        const selectedBookTypes = [];
        $(`input[name="bookType"]`).each((i, elem) => {
            if($(elem).is(":checked"))
            {
                selectedBookTypes.push( BookTypeEnum[$(elem).data("book-type")])
            }            
        });

        if(selectedBookTypes.length == 0)
        {
            this.newProjectDialog.showError(localization.translate("NoSelectedModuleForForum", "Admin").value);
            return;
        }
                
        this.projectClient.createProject(projectName, selectedBookTypes, (newId, error) => {
            if (error != null) {
                this.newProjectDialog.showError();
                return;
            }

            window.location.href = getBaseUrl() + "Admin/Project/Project?id=" + newId;
        });
    }

    private deleteProject() {
        this.projectClient.deleteProject(this.projectIdForDelete, error => {
            if (error != null) {
                this.deleteProjectDialog.showError();
                return;
            }
            
            this.projectList.reloadPage();
        });
    }
}