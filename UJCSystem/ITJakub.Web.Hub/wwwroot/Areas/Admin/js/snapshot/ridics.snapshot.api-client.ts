class SnapshotApiClient extends WebHubApiClient {

    public getVersionList(resourceId: number): JQuery.jqXHR<IResourceInfo[]> {
        return this.get(URI(this.getSnapshotControllerUrl() + "VersionList").search((query) => {
            query.resourceId = resourceId
        }).toString());
    }

    //TODO create Snapshot controller
    private getSnapshotControllerUrl() {
        return getBaseUrl() + "Admin/Project/";
    }
}

