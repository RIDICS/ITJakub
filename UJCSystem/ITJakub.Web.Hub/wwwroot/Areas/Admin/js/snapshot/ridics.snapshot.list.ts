class SnapshotList {
    private readonly projectId: number;
    
    constructor(projectId: number) {
        this.projectId = projectId;
    }

    init() {
        const snapshotList = new ListWithPagination(`Admin/Publication/SnapshotList?projectId=${this.projectId}`, "snapshot", ViewType.Partial, true, true);
        snapshotList.init();
    }
}