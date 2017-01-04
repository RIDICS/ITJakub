$(document).ready(() => {
    var projectList = new ProjectList();
    projectList.init();
});

class ProjectList {
    private projectManager: ProjectManager;
    private newProjectDialog: BootstrapDialogWrapper;
    private deleteProjectDialog: BootstrapDialogWrapper;
    private pagination: Pagination;
    private projectIdForDelete: number;
    private pageSize: number;
    private totalCount: number;

    constructor() {
        this.projectManager = new ProjectManager();

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

        this.pagination = new Pagination({
            container: $("#pagination"),
            pageClickCallback: this.loadPage.bind(this)
        });
    }

    public init() {
        $("#new-project-button").click((event) => {
            this.newProjectDialog.show();

            event.preventDefault();
        });

        $(".project-item .delete-button").click((event) => {
            var $projectItem = $(event.currentTarget).closest(".project-item");
            this.projectIdForDelete = Number($projectItem.data("project-id"));
            var projectName = $projectItem.data("project-name");

            $("#delete-project-name").text(projectName);
            this.deleteProjectDialog.show();

            event.preventDefault();
        });

        var $pagingInfo = $("#list-container .paging-info");
        this.pageSize = $pagingInfo.data("page-size");
        this.totalCount = $pagingInfo.data("total-count");

        this.pagination.make(this.totalCount, this.pageSize);
    }

    private createNewProject() {
        var projectName = $("#new-project-name").val();
        this.projectManager.createProject(projectName, (newId, error) => {
            if (error != null) {
                this.newProjectDialog.showError();
                return;
            }

            window.location.href = getBaseUrl() + "Admin/Project/Project?id=" + newId;
        });
    }

    private deleteProject() {
        this.projectManager.deleteProject(this.projectIdForDelete, error => {
            if (error != null) {
                this.deleteProjectDialog.showError();
                return;
            }

            window.location.reload(true);
        });
    }

    private loadPage(pageNumber: number) {
        var parameters = {
            start: (pageNumber-1) * this.pageSize,
            count: this.pageSize
        }
        var url = getBaseUrl() + "Admin/Project/ProjectListContent?" + $.param(parameters);

        var $listContainer = $("#list-container");

        $listContainer
            .html("<div class=\"loader\"></div>")
            .load(url, null, (responseText, textStatus, xmlHttpRequest) => {
                if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                    var alert = new AlertComponentBuilder(AlertType.Error).addContent("Chyba při načítání strany se seznamem projektů");
                    $listContainer
                        .empty()
                        .append(alert.buildElement());
                }
            });
    }
}