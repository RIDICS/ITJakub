class SnapshotApiClient extends WebHubApiClient {

    getVersionList(resourceId: number): JQuery.jqXHR<IResourceVersion[]> {
        return this.get(URI(this.getSnapshotControllerUrl() + "VersionList").search((query) => {
            query.resourceId = resourceId;
        }).toString());
    }

    getText(textVersionId: number): JQuery.jqXHR<ITextWithContent> {
        return this.get(URI(this.getSnapshotControllerUrl() + "GetText").search((query) => {
            query.textVersionId = textVersionId;
        }).toString());
    }

    getAudio(trackVersionId: number): JQuery.jqXHR<string> {
        return this.get(URI(this.getSnapshotControllerUrl() + "GetRecordings").search((query) => {
            query.trackVersionId = trackVersionId;
        }).toString());
    }

    getImageUrl(imageVersionId: number): string {
        return URI(this.getSnapshotControllerUrl() + "GetImage").search((query) => {
            query.imageVersionId = imageVersionId;
        }).toString();
    }

    createSnapshot(projectId: number, comment: string, defaultBookType: string, bookTypes: string[], resourceVersionIds: number[]): JQuery.jqXHR<any> {
        return this.post(this.getSnapshotControllerUrl() + "NewSnapshot",
            JSON.stringify({
                projectId: projectId,
                comment: comment,
                defaultBookType: defaultBookType,
                bookTypes: bookTypes,
                resourceVersionIds: resourceVersionIds
            }));
    }

    private getSnapshotControllerUrl() {
        return getBaseUrl() + "Admin/Publication/";
    }
}

