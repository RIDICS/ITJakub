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
            if(!dataLoaded)
            {
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
    }
}