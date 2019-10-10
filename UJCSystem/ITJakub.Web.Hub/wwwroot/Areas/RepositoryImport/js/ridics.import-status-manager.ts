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
        const loadingBar = document.createElement('div');
        $(loadingBar).addClass("lv-determinate_bordered_line lv-mid lvb-2");
        $("#progress_bar").append(loadingBar);
        const progressBar = lv.create(loadingBar);
        progressBar.setLabel("0%");
        const spinner = document.createElement('div');
        $(spinner).addClass("lv-bars sm lvl-3");
        $("#extra-spinner").append(spinner);
        const spinnerElement = lv.create(spinner);

        this.timer = setInterval(() => {
                this.client.getImportStatus().done((data) => {
                    let completed = true;

                    for (let key in data) {
                        if (data.hasOwnProperty(key)) {
                            const alertElement = $(`#alert-${data[key].externalRepositoryId}`);

                            let processed = 0;
                            if (data[key].totalProjectsCount > 0) {
                                processed = Math.round((data[key].processedProjectsCount / data[key].totalProjectsCount) * 1000) / 10;
                            }

                            progressBar.set(processed, 100);
                            progressBar.setLabel(processed + "%");

                            alertElement.text(localization.translateFormat("ImportStatus", [data[key].processedProjectsCount, data[key].totalProjectsCount], "RepositoryImport").value);

                            if (data[key].isCompleted) {
                                if (data[key].faultedMessage != null) {
                                    alertElement.addClass("alert-danger");
                                    $(loadingBar).addClass("cancel_import");
                                    spinnerElement.remove();
                                    alertElement.html(data[key].faultedMessage);
                                } else {
                                    alertElement.addClass("alert-success");
                                    $(loadingBar).addClass("success_import");
                                    spinnerElement.remove();
                                }

                                alertElement.removeClass("alert-info");
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