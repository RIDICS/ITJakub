class RepositoryImportApiClient extends WebHubApiClient {

    public getExternalRepositoryDetail(repositoryId: number): JQuery.jqXHR<string> {
        return this.get(URI(this.getExternalRepositoryControllerUrl() + "Detail").search(query => {
            query.id = repositoryId;
        }).toString());
    }

    public connectOaiPmh(url: string, config: string): JQuery.jqXHR<string> {
        return this.get(URI(this.getExternalRepositoryControllerUrl() + "OaiPmhConnect").search(query => {
            query.url = url,
            query.config = config;
        }).toString());
    }

    public loadApiConfiguration(api: string, config: string): JQuery.jqXHR<string> {
        if (config === "undefined") {
            config = "";
        }

        return this.get(URI(this.getExternalRepositoryControllerUrl() + "LoadApiConfiguration").search(
            query => {
                query.api = api,
                query.config = config;
            }).toString());
    }

    private getExternalRepositoryControllerUrl(): string {
        return this.getRepositoryImportAreaUrl() + "ExternalRepository/";
    }

    private getRepositoryImportAreaUrl(): string {
        return getBaseUrl() + "RepositoryImport/";
    }
}