$(document.documentElement).ready(() => {
    var externalRepositoryImport = new ImportStatusManager();
    externalRepositoryImport.init();
});

class ImportStatusManager {
    private readonly errorHandler: ErrorHandler;
    private readonly client: RepositoryImportApiClient;
    private timer: number;
    private refreshInterval = 5000;

    constructor() {
        this.errorHandler = new ErrorHandler();
        this.client = new RepositoryImportApiClient();
    }

    init() {
        this.timer = setInterval(() => {
                this.client.getImportStatus().done((data) => {
                    let completed = true;
                    for (let key in data) {
                        if (data.hasOwnProperty(key)) {
                            const progressBar = $(`#repository-${data[key].externalRepositoryId}`);
                            const alertElement = $(`#alert-${data[key].externalRepositoryId}`);

                            let processed = 0;
                            if (data[key].totalProjectsCount > 0) {
                                processed = Math.round(data[key].processedProjectsCount / data[key].totalProjectsCount * 100);
                            }

                            progressBar.css(`width`, `${processed}%`);
                            progressBar.html(processed + "%");

                            alertElement.text(localization.translateFormat("ImportStatus", [data[key].processedProjectsCount, data[key].totalProjectsCount], "RepositoryImport").value);

                            if (data[key].isCompleted) {
                                if (data[key].faultedMessage != null) {
                                    progressBar.addClass("progress-bar-danger");
                                    alertElement.addClass("alert-danger");
                                    alertElement.html(data[key].faultedMessage);
                                } else {
                                    alertElement.addClass("alert-success");
                                    progressBar.addClass("progress-bar-success");
                                }

                                alertElement.removeClass("alert-info");
                                progressBar.removeClass("active");
                                $(`.cancel-import-button[data-id=\"${data[key].externalRepositoryId}\"]`).addClass("disabled");
                            } else {
                                completed = false;
                            }
                        }
                    }

                    if (completed) {
                        this.stop();
                    }
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                    $(".alert-holder").empty().append(alert);
                });
            },
            this.refreshInterval);

        $(".cancel-import-button").click((e) => {
            const button = $(e.target as Node as Element);
            const repositoryId = button.data("id");
            button.addClass("disabled");
            this.client.cancelImportTask(repositoryId);
        });
    }

    stop() {
        clearInterval(this.timer);
    }
}