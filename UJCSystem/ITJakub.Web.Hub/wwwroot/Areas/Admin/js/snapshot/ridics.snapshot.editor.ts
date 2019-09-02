class SnapshotEditor {
    private readonly $container: JQuery;
    private readonly client: SnapshotApiClient;
    private readonly imageViewer: ImageViewerContentAddition;
    private readonly errorHandler: ErrorHandler;

    constructor(panelElement: JQuery) {
        this.$container = panelElement;
        this.client = new SnapshotApiClient();
        this.imageViewer = new ImageViewerContentAddition(new EditorsUtil());
        this.errorHandler = new ErrorHandler();
    }

    public init() {
        $(".include-all-checkbox").click((event) => {
            const checkbox = $(event.currentTarget);
            const table = checkbox.parents(".table");
            const isChecked = checkbox.is(":checked");
            table.find(".include-checkboxes input").prop("checked", isChecked);
        });

        $(".include-checkboxes input").click((event) => {
            const table = $(event.currentTarget).parents(".table");

            let isAllChecked = true;
            table.find(".include-checkboxes input").each((index, elem) => {
                if (!(elem as Node as HTMLInputElement).checked) {
                    isAllChecked = false;
                }
            });

            table.find(".include-all-checkbox").prop("checked", isAllChecked);
        });

        $(".select-version").click((event) => {
            const selectBox = $(event.currentTarget);
            const dataLoaded = $(event.currentTarget).data("loaded");
            if (!dataLoaded) {
                const resourceId = selectBox.parents(".resource-row").data("id");
                this.client.getVersionList(resourceId).done((data) => {
                    for (let i = 1; i < data.length; i++) { // skip first element - already loaded latest version
                        const resource = data[i];
                        const option = new Option(resource.versionNumber, String(resource.resourceVersionId));
                        $(option).html(resource.versionNumber);
                        $(option).data("author", resource.author);
                        $(option).data("comment", resource.comment);
                        $(option).data("created", resource.created);
                        $(option).data("name", resource.name);
                        selectBox.append(option);
                    }
                    selectBox.data("loaded", true);
                }).fail((error) => {
                    console.log(error);
                    //TODO error, where to place it? Gui.something
                });
            }
        });

        $(".select-version").on("change", (event) => {
            const selectBox = $(event.currentTarget);
            const resourceRow = selectBox.parents(".resource-row");
            const selectedVersion = selectBox.find("option:selected");
            resourceRow.data("version-id", selectedVersion.val());
            resourceRow.find(".author").text(selectedVersion.data("author"));
            resourceRow.find(".comment").text(selectedVersion.data("comment"));
            resourceRow.find(".created").text(selectedVersion.data("created"));
            resourceRow.find(".name").text(selectedVersion.data("name"));
        });

        $(".resource-preview").click((event) => {
            const resourceRow = $(event.currentTarget).parents(".resource-row");
            const resourceVersionId = resourceRow.data("version-id");
            const resourceName = resourceRow.find(".name").text();
            const resourceType = Number(resourceRow.parents(".publish-resource-panel").data("resource-type"));
            const resourcePreviewModal = $("#resourcePreviewModal");

            const modalBody = resourcePreviewModal.find(".modal-body");
            modalBody.html(`<div class="loader"></div>`);

            const modalTitle = resourcePreviewModal.find(".modal-title");
            modalTitle.text(resourceName);
            
            switch (resourceType) {
            case ResourceType.Audio:
                    {
                        this.client.getAudio(resourceVersionId).done((response) => {
                            modalBody.html(response);
                        }).fail((error) => {
                            const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                            modalBody.html(alert);
                        });
                    }
                break;
            case ResourceType.Image:
                {
                    const imageUrl = this.client.getImageUrl(resourceVersionId);
                    this.imageViewer.addImageContent(modalBody, imageUrl);
                }
                break;
            case ResourceType.Text:
                {
                    this.client.getText(resourceVersionId).done((response) => {
                        modalBody.html(response.text);
                    }).fail((error) => {
                        const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                        modalBody.html(alert);
                    });
                }
                break;
            default:
                {
                    const alert = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("UnsupportedResourceType","Admin").value).buildElement();
                    modalBody.html(alert);
                }
            }
            resourcePreviewModal.modal("show");
        });
    }
}