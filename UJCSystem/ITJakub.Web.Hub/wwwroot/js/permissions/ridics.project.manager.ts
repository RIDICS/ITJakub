$(document.documentElement).ready( () => {
    const projectManager = new ProjectManager();
    projectManager.init();
});

class ProjectManager {
    private readonly projectPermissionManager: ProjectPermissionManager;
    private projectList: ListWithPagination;

    constructor() {
        this.projectPermissionManager = new ProjectPermissionManager();
    }

    public init(list?: ListWithPagination) {
        if (list == null) {
            this.projectList = new ListWithPagination("Permission/ProjectPermission", "project", ViewType.Widget, true, false, this.reInit, this);
            
        } else {
            this.projectList = list;
        }
        this.projectList.init();
      
        this.projectPermissionManager.init();
        this.reInit();
    }

    public reInit() {
        $(".project-row").on("click", (event) => {
            $(event.currentTarget as Node as Element).addClass("active").siblings().removeClass("active");
            const selectedProjectId = $(event.currentTarget as Node as Element).data("project-id");
            this.projectPermissionManager.loadRoles(selectedProjectId);
            this.projectPermissionManager.clearPermissionSection();
        });

        $("form.project-search-form").on("submit", () => {
            this.projectPermissionManager.clearSections();
        });

        $("#projectPagination a").on("click", () => {
            this.projectPermissionManager.clearSections();
        });
    }
}