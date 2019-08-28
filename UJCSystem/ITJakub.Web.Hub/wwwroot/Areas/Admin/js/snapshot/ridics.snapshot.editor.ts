class SnapshotEditor {
    private readonly $container: JQuery;

    constructor(panelElement: JQuery) {
        this.$container = panelElement;
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
    }
}