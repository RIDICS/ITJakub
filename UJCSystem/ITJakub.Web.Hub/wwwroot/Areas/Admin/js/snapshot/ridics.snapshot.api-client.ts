﻿class SnapshotApiClient extends WebHubApiClient {

    getVersionList(resourceId: number): JQuery.jqXHR<IResourceVersion[]> {
        return this.get(URI(this.getSnapshotControllerUrl() + "VersionList").search((query) => {
            query.resourceId = resourceId
        }).toString());
    }

    getText(textId: number): JQuery.jqXHR<ITextWithContent> {
        return this.get(URI(this.getSnapshotControllerUrl() + "GetText").search((query) => {
            query.textId = textId
        }).toString());
    }

    getAudio(trackId: number): JQuery.jqXHR<string> {
        return this.get(URI(this.getSnapshotControllerUrl() + "GetRecordings").search((query) => {
            query.trackId = trackId
        }).toString());
    }

    getImageUrl(imageId: number): string {
        return URI(this.getSnapshotControllerUrl() + "GetImage").search((query) => {
            query.imageId = imageId
        }).toString();
    }

    //TODO create Snapshot controller
    private getSnapshotControllerUrl() {
        return getBaseUrl() + "Admin/Project/";
    }
}

