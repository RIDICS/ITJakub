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

    public getImportStatus(): JQuery.jqXHR {
        return this.get(this.getRepositoryImportControllerUrl() + "GetImportStatus");
    }

    public cancelImportTask(repositoryId: number) {
        return this.get(URI(this.getRepositoryImportControllerUrl() + "CancelImport").search(query => {
            query.id = repositoryId;
        }).toString());
    }

    public getFilteringExpressionRow(): JQuery.jqXHR<string> {
        return this.get(this.getFilteringExpressionSetControllerUrl() + "AddFilteringExpressionRow");
    }

    private getExternalRepositoryControllerUrl(): string {
        return this.getRepositoryImportAreaUrl() + "ExternalRepository/";
    }

    private getRepositoryImportControllerUrl(): string {
        return this.getRepositoryImportAreaUrl() + "RepositoryImport/";
    }

    private getFilteringExpressionSetControllerUrl(): string {
        return this.getRepositoryImportAreaUrl() + "FilteringExpressionSet/";
    }

    private getRepositoryImportAreaUrl(): string {
        return getBaseUrl() + "RepositoryImport/";
    }
}