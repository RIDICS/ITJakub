class SnapshotEditor {
    private readonly $container: JQuery;
    private readonly client: SnapshotApiClient;
    private readonly imageViewer: ImageViewerContentAddition;

    constructor(panelElement: JQuery) {
        this.$container = panelElement;
        this.client = new SnapshotApiClient();
        this.imageViewer = new ImageViewerContentAddition(new EditorsUtil());
    }

    public init() {
        $(".include-all").click((event) => {
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

            table.find(".include-all").prop("checked", isAllChecked);
        });

        $(".select-version").click((event) => {
            const selectBox = $(event.currentTarget);
            const dataLoaded = $(event.currentTarget).data("loaded");
            if (!dataLoaded) {
                const resourceId = selectBox.parents(".resource-row").data("id");
                this.client.getVersionList(resourceId).done((data) => {
                    for (let resource of data) {
                        const option = new Option(resource.versionNumber, String(resource.resourceVersionId));
                        $(option).html(resource.versionNumber);
                        selectBox.append(option);
                    }
                    selectBox.data("loaded", true);
                }).fail((error) => {
                    console.log(error);
                    //TODO error, where to place it? Gui.something
                });
            }
        });

        $(".resource-preview").click((event) => {
            const resourceRow = $(event.currentTarget).parents(".resource-row");
            const resourceId = resourceRow.data("id");
            const resourceType = Number(resourceRow.parents(".publish-resource-panel").data("resource-type"));
            const resourcePreviewModal = $("#resourcePreviewModal");
            const modalBody = resourcePreviewModal.find(".modal-body");
            switch (resourceType) {
            case ResourceType.Audio:
                    {
                        this.client.getAudio(resourceId).done((response) => {
                            modalBody.html(response);
                        }).fail((error) => {
                            //TODO error
                        });
                    }
                break;
            case ResourceType.Image:
                {
                    const imageUrl = this.client.getImageUrl(resourceId);
                    this.imageViewer.addImageContent(modalBody, imageUrl);
                }
                break;
            case ResourceType.Text:
                {
                    this.client.getText(resourceId).done((response) => {
                        modalBody.html(response.text);
                    }).fail((error) => {
                        //TODO error
                    });
                }
                break;
            default:
                {
                    //TODO error unknown resource type
                }
            }
            resourcePreviewModal.modal("show");
        });
    }
}