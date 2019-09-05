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
            this.projectList = new ListWithPagination("Permission/ProjectPermission", "project", ViewType.Widget, true, this.reInit, this);
            
        } else {
            this.projectList = list;
        }
        this.projectList.init();
        this.initSearchBox();
        this.reInit();
    }

    public reInit() {
        $(".project-row").on("click", (event) => {
            $(event.currentTarget as Node as Element).addClass("active").siblings().removeClass("active");
            var selectedProjectId = $(event.currentTarget as Node as Element).data("project-id");
            this.loadRoles(selectedProjectId);
        });

        $("form.project-search-form").on("submit", () => {
            this.clearSections();
        });

        $("#projectPagination a").on("click", () => {
            this.clearSections();
        });
    }

    private loadRoles(projectId: number) {
        const roleSection = $("#role-section .section");
        const container = roleSection.find(".list-container");
        const searchForm = roleSection.find(".role-search-form");
        searchForm.find("input.search-value").val("");
        container.html("<div class=\"lv-dots sm lv-mid\"></div>");
        roleSection.removeClass("hide");

        this.client.getRolesByProject(projectId).done(response => {
            container.html(response as string);
            this.initRemoveRoleFromProjectButton();
        }).fail((error) => {
            const errorAlert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error, localization.translate("ListError", "PermissionJs").value));
            container.empty().append(errorAlert.buildElement());
        }).always(() => {
            this.roleList = new ListWithPagination(`Permission/RolesByProject?projectId=${projectId}`,
                "role",
                ViewType.Widget,
                false,
                this.initRemoveRoleFromProjectButton,
                this);
            this.roleList.init();
            this.roleList.setSearchFormDisabled(false);
            $("#addPermissionButton").removeClass("disabled");
        });
    }
    
    private initRemoveRoleFromProjectButton() {
        $(".remove-role").on("click", (event) => {
            event.stopPropagation();
            const roleRow = $(event.currentTarget as Node as Element).parents(".role-row");
            const alert = roleRow.find(".alert");
            alert.hide();

            const roleId = roleRow.data("role-id");
            const projectId = $(".project-row.active").data("project-id");
            this.client.removeProjectFromRole(projectId, roleId).done(() => {
                this.roleList.reloadPage();
            }).fail((error) => {
                alert.text(this.errorHandler.getErrorMessage(error, localization.translate("RemoveProjectFromRoleError", "PermissionJs").value));
                alert.show();
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

        const addProjectPermissionToRoleBtn = $("#addPermission");
        if (addProjectPermissionToRoleBtn.data("init") === false) {
            addProjectPermissionToRoleBtn.data("init", true);

            const addProjectPermissionModal = $("#addProjectPermissionToRoleDialog");
            const roleError = $("#addProjectToRoleError");
            
            $("#addPermissionButton").on("click", (event) => {
                event.preventDefault();
                const role = $(".project-row.active");
                $("#specificProjectName").text(role.find(".name").text());
                addProjectPermissionModal.modal();
            });

            addProjectPermissionModal.on("hidden.bs.modal", () => {
                roleError.empty();
                this.currentRoleSelectedItem = null;
                addProjectPermissionModal.find("#roleSearchInput").val("");
                addProjectPermissionModal.find("#selectedRole").text(localization.translate("RoleIsNotSelected", "PermissionJs").value);
            });

            addProjectPermissionToRoleBtn.on("click", () => {
                roleError.empty();
                const projectId = $(".project-row.active").data("project-id");
     
                if (typeof this.currentRoleSelectedItem == "undefined" || this.currentRoleSelectedItem == null) {
                    const errorAlert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(localization.translate("RoleIsNotSelected", "PermissionJs").value);
                    roleError.empty().append(errorAlert.buildElement());
                    return;
                }
                else {
                    const roleId = this.currentRoleSelectedItem.id;
                    this.client.addProjectToRole(projectId, roleId).done(() => {
                        this.roleList.reloadPage();
                        addProjectPermissionModal.modal("hide");
                    }).fail((error) => {
                        const errorAlert = new AlertComponentBuilder(AlertType.Error)
                            .addContent(this.errorHandler.getErrorMessage(error,
                                localization.translate("AddProjectToRoleError", "PermissionJs").value));
                        roleError.empty().append(errorAlert.buildElement());
                    });
                }
            });
        }
    }

    private clearSections() {
        this.roleList.clear(localization.translate("ProjectIsNotSelected", "PermissionJs").value);
        $("#addPermissionButton").addClass("disabled");
    }
}