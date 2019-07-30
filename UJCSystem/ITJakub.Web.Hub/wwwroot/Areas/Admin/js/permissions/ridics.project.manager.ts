$(document.documentElement).ready(() => {
    var projectManager = new ProjectManager();
    projectManager.init();
});

class ProjectManager {
    private readonly searchBox: SingleSetTypeaheadSearchBox<IRole>;
    private readonly client: PermissionApiClient;
    private readonly errorHandler: ErrorHandler;
    private currentRoleSelectedItem: IRole;
    private roleList: ListWithPagination;
    private projectList: ListWithPagination;

    constructor() {
        this.searchBox = new SingleSetTypeaheadSearchBox<IRole>("#roleSearchInput", "Permission",
            (item) => item.name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.name, item.description));
        this.client = new PermissionApiClient();
        this.errorHandler = new ErrorHandler();
    }

    public init(list?: ListWithPagination) {
        if (list == null) {
            this.projectList = new ListWithPagination("Permission/ProjectPermission", 10, "project", ViewType.Widget, true, undefined, this);
            
        } else {
            this.projectList = list;
        }
        this.projectList.init();

        $(".project-row").on("click", (event) => {
            $(event.currentTarget as Node as Element).addClass("active").siblings().removeClass("active");
            var selectedProjectId = $(event.currentTarget as Node as Element).data("project-id");
            this.loadRoles(selectedProjectId);
        });

        $("form.role-search-form").submit(() => {
            this.clearSections();
        });

        $("#role-pagination a").on("click", () => {
            this.clearSections();
        });

        this.initSearchBox();
    }

    private loadRoles(projectId: number) {
        var container = $("#role-list-container");
        container.html("<div class=\"loader\"></div>");

        this.client.getRolesByProject(projectId).done(response => {
            container.html(response as string);
            this.roleList = new ListWithPagination(`Permission/RolesByProject?projectId=${projectId}`,
                10,
                "role",
                ViewType.Widget,
                false,
                this.initRemoveRoleFromProjectButton,
                this);
            this.roleList.init();
            this.initRemoveRoleFromProjectButton();
            $("#role-section .section").removeClass("hide");
        });
    }
    
    private initRemoveRoleFromProjectButton() {
        $(".remove-role").on("click", (event) => {
            event.stopPropagation();
            const roleRow = $(event.currentTarget as Node as Element).parents(".role-row");
            const roleId = roleRow.data("role-id");
            const projectId = $(".project-row.active").data("project-id");
            this.client.removeProjectFromRole(projectId, roleId).done(() => {
                this.roleList.reloadPage();
            });
        });
    }

    private initSearchBox() {
        this.searchBox.setDataSet("Role");
        this.searchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (selectionConfirmed) {
                this.currentRoleSelectedItem = this.searchBox.getValue();
                const roleBox = $("#selectedRole");
                const name = this.currentRoleSelectedItem.name;
                roleBox.text(name);
                roleBox.data("role-id", this.currentRoleSelectedItem.id);
            }
        });

        const addPermissionToRoleBtn = $("#addPermission");
        if (addPermissionToRoleBtn.data("init") === false) {
            addPermissionToRoleBtn.data("init", true);

            $("#addPermissionButton").on("click", (event) => {
                event.preventDefault();
                const role = $(".project-row.active");
                $("#specificProjectName").text(role.find(".name").text());
                $("#addProjectPermissionToRoleDialog").modal();
            });

            addPermissionToRoleBtn.on("click", () => {
                const projectId = $(".project-row.active").data("project-id");
                let roleId: number;
                if (typeof this.currentRoleSelectedItem == "undefined")
                    roleId = $("#selectedRole").data("role-id");
                else {
                    roleId = this.currentRoleSelectedItem.id;
                }

                this.client.addProjectToRole(projectId, roleId).done(() => {
                    this.roleList.reloadPage();
                    $("#addProjectPermissionToRoleDialog").modal("hide");
                });
            });
        }
    }
}