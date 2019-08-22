$(document.documentElement).ready(() => {
    var externalRepositoryConfiguration = new ExternalRepositoryConfiguration();
    externalRepositoryConfiguration.init();
});

class ExternalRepositoryConfiguration {
    private readonly errorHandler: ErrorHandler;
    private readonly client: RepositoryImportApiClient;

    constructor() {
        this.errorHandler = new ErrorHandler();
        this.client = new RepositoryImportApiClient();
    }

    init() {
        $(".repository-detail").click((e) => {
            const repositoryId = $(e.target as Node as Element).data("repository-id");
            this.client.getExternalRepositoryDetail(repositoryId).done((response) => {
                $(`#repository-${repositoryId} .bib-table:last-child`).html(response);
            });
        });

        $("#resourceType").change((e) => {
            const api = $(e.target as Node as Element).children("option:selected").text();
            //TODO test
            const config = String($(".repository-configuration").val());

            this.client.loadApiConfiguration(api, config).done((response) => {
                $("#apiType").val(api);
                $("#apiOptions").html(response);
                this.initOaiPmh();
            });
        });

        $("#resourceType").change();
    }

    initOaiPmh() {
        $("#oaiPmhConnect").click(() => {
            const config = String($(".repository-configuration").val());
            const resourceUrl = String($("#oaiPmhResourceUrl").val());
            //TODO escape URL?
            this.client.connectOaiPmh(resourceUrl, config).done((response) => {
                $("#oaiPmhConfig").html(response);
            });
        });
    }
}