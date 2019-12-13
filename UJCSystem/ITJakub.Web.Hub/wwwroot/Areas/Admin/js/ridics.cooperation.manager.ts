class CooperationManager {
    private readonly projectPermissionManager: ProjectPermissionManager;
    private readonly errorHandler: ErrorHandler;
    private readonly client: PermissionApiClient;
    private readonly projectId: number;

    constructor(projectId: number) {
        this.projectId = projectId;
        this.projectPermissionManager = new ProjectPermissionManager(false, projectId);
        this.errorHandler = new ErrorHandler();
        this.client = new PermissionApiClient();
    }
    
    public init() {
        this.projectPermissionManager.init(true, false);
        this.initAddProjectToUserDialog();
    }

    public initAddProjectToUserDialog() {
        const submitPermissions = $("#addPermissionToUser");
        const addProjectPermissionModal = $("#addProjectPermissionToUserDialog");
        const errorAlertHolder = $("#addProjectToUserError");
        
        $("#addPermissionToUserButton").on("click", (event) => {
            event.preventDefault();
            addProjectPermissionModal.modal();
        });

        addProjectPermissionModal.on("hidden.bs.modal", () => {
            errorAlertHolder.empty();
            addProjectPermissionModal.find("#userSearchInput").val("");
        });

        submitPermissions.on("click", () => {
            errorAlertHolder.empty();
            const userCodeInput = String($("#userCodeInput").val());
            if(userCodeInput !== "")
            {
                this.addPermissionsOnProjectToUser(userCodeInput, addProjectPermissionModal).done(() => {
                    this.projectPermissionManager.reloadRoles();
                    this.projectPermissionManager.clearPermissionSection();
                    addProjectPermissionModal.modal("hide");
                }).fail((error) => {
                    const errorAlert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(this.errorHandler.getErrorMessage(error,
                            localization.translate("AddProjectToUserError", "PermissionJs").value));
                    errorAlertHolder.empty().append(errorAlert.buildElement());
                });
            }
            else {
                const errorAlert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(localization.translate("UserCodeIsNotEntered", "PermissionJs").value);
                errorAlertHolder.empty().append(errorAlert.buildElement());
                return;
            } 
        });        
    }

    private addPermissionsOnProjectToUser(userCode: string, context: JQuery): JQueryXHR {
        const addProjectToUser = {
            userCode: userCode,
            permissionsConfiguration: this.projectPermissionManager.getPermissionsConfiguration(context),
        };

        return this.client.addProjectToSingleUser(addProjectToUser);
    }
}