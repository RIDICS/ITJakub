class CooperationManager {
    private readonly projectPermissionManager: ProjectPermissionManager;
    private readonly projectId: number;

    constructor(projectId: number) {
        this.projectId = projectId;
        this.projectPermissionManager = new ProjectPermissionManager(false, projectId);
    }
    
    public init() {
        this.projectPermissionManager.init(true);
    }
}