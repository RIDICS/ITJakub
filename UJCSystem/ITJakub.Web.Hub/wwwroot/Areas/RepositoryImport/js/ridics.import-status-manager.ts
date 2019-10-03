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

        this.timer = setInterval(() => {
                this.client.getImportStatus().done((data) => {
                    let completed = true;

                    for (let key in data) {
                        if (data.hasOwnProperty(key)) {
                            const alertElement = $(`#alert-${data[key].externalRepositoryId}`);

                            let processed = 0;
                            if (data[key].totalProjectsCount > 0) {
                                processed = Math.round((data[key].processedProjectsCount / data[key].totalProjectsCount) * 100);
                            }

                            progressBar.set(processed, 100);
                            progressBar.setLabel(processed + "%");

                            alertElement.text(localization.translateFormat("ImportStatus", [data[key].processedProjectsCount, data[key].totalProjectsCount], "RepositoryImport").value);

                            if (data[key].isCompleted) {
                                if (data[key].faultedMessage != null) {
                                    alertElement.addClass("alert-danger");
                                    loadingBar.style.borderColor = "#d9534f";
                                    $(".lv-determinate_bordered_line > div").css("background", "#d9534f");
                                    $(".lv-determinate_bordered_line[data-label]").css("color", "#d9534f");
                                    progressBar.removeLabel();
                                    alertElement.html(data[key].faultedMessage);
                                } else {
                                    alertElement.addClass("alert-success");
                                    loadingBar.style.borderColor = "#5cb85c";
                                    $(".lv-determinate_bordered_line > div").css("background", "#5cb85c");
                                    $(".lv-determinate_bordered_line[data-label]").css("color", "#5cb85c");
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