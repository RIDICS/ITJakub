class SnapshotList {
    private readonly projectId: number;
    
    constructor(projectId: number) {
        this.projectId = projectId;
    }

    init() {
        const snapshotList = new ListWithPagination(`Admin/Project/SnapshotList?projectId=${this.projectId}`, "snapshot", ViewType.Partial, true);
        snapshotList.init();
    }
}