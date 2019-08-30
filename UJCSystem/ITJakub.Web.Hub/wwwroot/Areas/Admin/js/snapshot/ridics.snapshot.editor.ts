class SnapshotEditor {
    private readonly $container: JQuery;
    private readonly client: SnapshotApiClient;

    constructor(panelElement: JQuery) {
        this.$container = panelElement;
        this.client = new SnapshotApiClient();
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
            switch (resourceType) {
            case ResourceType.Audio:
                {

                }
                break;
            case ResourceType.Image:
                {
                    //TODO refactor
                    const imageViewer = new ImageViewerContentAddition(new EditorsUtil());
                    imageViewer.addImageContent(resourcePreviewModal.find(".modal-body"), 561);
                }
                break;
            case ResourceType.Text:
                {

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