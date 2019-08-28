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
            if (typeof repositoryId != "undefined") {
                const repositoryDetail = $(`#repository-${repositoryId} .bib-table:last-child`);
                this.client.getExternalRepositoryDetail(repositoryId).done((response) => {
                    repositoryDetail.html(response);
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                    repositoryDetail.empty().append(alert);
                });
            }
        });

        $("#resourceType").change((e) => {
            const api = $(e.target as Node as Element).children("option:selected").text();
            const config = String($(".repository-configuration").val());
            const apiOptions = $("#apiOptions");

            this.client.loadApiConfiguration(api, config).done((response) => {
                $("#apiType").val(api);
                apiOptions.html(response);
                this.initOaiPmh();
            }).fail((error) => {
                const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                apiOptions.empty().append(alert);
            });
        });

        $("#resourceType").change();
    }

    initOaiPmh() {
        $("#oaiPmhConnect").click((event) => {
            const button = $(event.target as Node as Element);
            const loading = button.children(".loading-small-button");
            button.addClass("disabled");
            loading.removeClass("hide");

            const config = String($(".repository-configuration").val());
            const resourceUrl = encodeURI(String($("#oaiPmhResourceUrl").val()));
            const oaiPmhConfig = $("#oaiPmhConfig");
            this.client.connectOaiPmh(resourceUrl, config).done((response) => {
                oaiPmhConfig.html(response);
            }).fail((error) => {
                const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                oaiPmhConfig.empty().append(alert);
            }).always(() => {
                button.removeClass("disabled");
                loading.addClass("hide");
            });
        });
    }
}