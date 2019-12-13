class ProjectPermissionManager {
    private readonly searchBox: MultiSetTypeaheadSearchBox<IRole>;
    private readonly client: PermissionApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly savePermissionButtonSelector: string;
    private readonly isUserTypeaheadEnabled: boolean;
    private projectId: number;
    private currentRoleSelectedItem: IRole;
    private roleList: ListWithPagination;
    private permissionPanel: JQuery<HTMLElement>;

    constructor(isUserTypeaheadEnabled: boolean, projectId: number = null) {
        this.isUserTypeaheadEnabled = isUserTypeaheadEnabled;
        this.projectId = projectId;
        this.searchBox = new MultiSetTypeaheadSearchBox<IRole>("#roleSearchInput", "Permission",
            (item) => item.name,
            (item) => MultiSetTypeaheadSearchBox.getDefaultSuggestionTemplateMulti(item.name, item.description));
        this.client = new PermissionApiClient();
        this.errorHandler = new ErrorHandler();
        this.savePermissionButtonSelector = "#saveProjectPermissions";
    }

    public init(clearPermissions = false, saveStateToUrl = true) {
        if (this.projectId != null) {
            this.roleList = new ListWithPagination(`Admin/Project/CooperationList?projectId=${this.projectId}`,
                "role",
                ViewType.Widget,
                saveStateToUrl,
                false,
                this.initRoleClicks,
                this);
            this.roleList.init();
            this.initRoleClicks();
        }
        this.initSearchBox();
        this.permissionPanel = $("#project-permission-section");
        this.initPermissionsSaving();
        if(clearPermissions) {
            this.permissionPanel.find(".section").removeClass("hide");
            this.clearPermissionSection();   
        }
    }

    public setProjectId(projectId: number) {
        this.projectId = projectId;
    }

    public clearSections() {
        this.clearRoleSection();
        this.clearPermissionSection();
    }

    public clearRoleSection() {
        $("#addPermissionButton").addClass("disabled");
        this.roleList.clear(localization.translate("ProjectIsNotSelected", "PermissionJs").value);
    }

    public clearPermissionSection() {
        const alertHolder = this.permissionPanel.find(".alert-holder");
        this.permissionPanel.find(".sub-content").empty();
        const errorAlert = new AlertComponentBuilder(AlertType.Info).addContent(localization.translate("RoleIsNotSelected", "PermissionJs").value);
        alertHolder.empty().append(errorAlert.buildElement());
        $("#saveProjectPermissions").addClass("hide");
    }
    
    public reloadRoles() {
        this.roleList.reloadPage();
    }

    public loadRoles(projectId: number) {
        this.projectId = projectId;
        const roleSection = $("#role-section .section");
        const container = roleSection.find(".list-container");
        const searchForm = roleSection.find(".role-search-form");
        searchForm.find("input.search-value").val("");
        container.html("<div class=\"lv-dots md lv-mid\"></div>");
        roleSection.removeClass("hide");

        this.client.getRolesByProject(projectId).done(response => {
            container.html(response as string);
        }).fail((error) => {
            const errorAlert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error, localization.translate("ListError", "PermissionJs").value));
            container.empty().append(errorAlert.buildElement());
        }).always(() => {
            this.roleList = new ListWithPagination(`Permission/RolesByProject?projectId=${projectId}`,
                "role",
                ViewType.Widget,
                false,
                false,
                this.initRoleClicks,
                this);
            this.roleList.init();
            this.roleList.setSearchFormDisabled(false);
            $("#addPermissionButton").removeClass("disabled");
            this.initRoleClicks();
        });
    }

    private initRemoveRoleFromProjectButton() {
        $(".remove-role").on("click", (event) => {
            event.stopPropagation();
            const roleRow = $(event.currentTarget as Node as Element).parents(".role-row");
            const alert = roleRow.find(".alert");
            alert.hide();

            const roleId = roleRow.data("role-id");
            this.client.removeProjectFromRole(this.projectId, roleId).done(() => {
                this.roleList.reloadPage();
                this.clearPermissionSection();
            }).fail((error) => {
                alert.text(this.errorHandler.getErrorMessage(error, localization.translate("RemoveProjectFromRoleError", "PermissionJs").value));
                alert.show();
            });
        });
    }

    private initRoleClicks(): void {
        $(".role-row").on("click", (event) => {
            $(event.currentTarget as Node as Element).addClass("active").siblings().removeClass("active");
            const roleRow = $(event.currentTarget as Node as Element);
            const roleId = roleRow.data("role-id");
            const body = $("#project-permission-section .panel-body");
            const subContent = body.find(".sub-content");
            const alertHolder = body.find(".alert-holder");
            const saveButton = body.find(this.savePermissionButtonSelector);
            saveButton.addClass("hide");
            subContent.empty().append(`<div class="loader"></div>`);
            alertHolder.empty();

            $("#project-permission-section .section").removeClass("hide");
            this.client.getPermissionForRoleAndBook(this.projectId, roleId).done((result) => {
                subContent.html(result);
                saveButton.removeClass("hide");
            }).fail((error) => {
                const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                subContent.empty();
                alertHolder.empty().append(alert);
            });
        });

        $("#rolePagination a").on("click", () => {
            this.clearPermissionSection();
        });

        $("form.role-search-form").on("submit", () => {
            this.clearPermissionSection();
        });

        this.initRemoveRoleFromProjectButton();
    }

    private initSearchBox() {
        this.searchBox.addDataSet("Role", localization.translate("Groups", "PermissionJs").value);
        if (this.isUserTypeaheadEnabled) {
            this.searchBox.addDataSet("SingleUserGroup", localization.translate("Users", "PermissionJs").value);
        }        

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
                const savingIcon = addProjectPermissionToRoleBtn.find(".saving-icon");
                roleError.empty();
                if (typeof this.currentRoleSelectedItem == "undefined" || this.currentRoleSelectedItem == null) {
                    const errorAlert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(localization.translate("RoleIsNotSelected", "PermissionJs").value);
                    roleError.empty().append(errorAlert.buildElement());
                    return;
                } else {
                    savingIcon.removeClass("hide");
                    const requestContract = this.getRequestContract(this.currentRoleSelectedItem.id, addProjectPermissionModal);
                    this.client.addProjectToRole(requestContract).done(() => {
                        this.roleList.reloadPage();
                        this.clearPermissionSection();
                        addProjectPermissionModal.modal("hide");
                    }).fail((error) => {
                        const errorAlert = new AlertComponentBuilder(AlertType.Error)
                            .addContent(this.errorHandler.getErrorMessage(error,
                                localization.translate("AddProjectToRoleError", "PermissionJs").value));
                        roleError.empty().append(errorAlert.buildElement());
                    }).always(() => {
                        savingIcon.addClass("hide");
                    });
                }
            });
        }
    }

    private initPermissionsSaving() {
        $(this.savePermissionButtonSelector).off();
        $(this.savePermissionButtonSelector).on("click", () => {
            const roleId = $(".role-row.active").data("role-id");
            const alertHolder = this.permissionPanel.find(".alert-holder");
            alertHolder.empty();
            const requestContract = this.getRequestContract(roleId, this.permissionPanel);
            this.client.updateOrAddProjectToRole(requestContract).done(() => {
                const errorAlert = new AlertComponentBuilder(AlertType.Success)
                    .addContent(localization.translate("ChangesSavedSuccessfully", "PermissionJs").value);
                alertHolder.empty().append(errorAlert.buildElement());
                alertHolder.find(".alert").delay(3000).fadeOut(2000);
            }).fail((error) => {
                const errorAlert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(this.errorHandler.getErrorMessage(error));
                alertHolder.empty().append(errorAlert.buildElement());
            });
        });
    }

    private getRequestContract(roleId: number, context: JQuery): IAddProjectToUserGroupRequest {
        const addProjectToRole: IAddProjectToUserGroupRequest = {
            roleId: roleId,
            permissionsConfiguration: this.getPermissionsConfiguration(context),
        };
        return addProjectToRole;
    }

    public getPermissionsConfiguration(context: JQuery): IPermissionsConfiguration
    {
        return {
            bookId: this.projectId,
            showPublished: context.find(`input[name="show-published"]`).is(":checked"),
            readProject: context.find(`input[name="read-project"]`).is(":checked"),
            adminProject: context.find(`input[name="admin-project"]`).is(":checked"),
            editProject: context.find(`input[name="edit-project"]`).is(":checked"),
        };
    }
}