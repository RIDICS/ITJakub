class KeyTableKindEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new literary kind");
        $(".rename-key-table-entry").text("Rename literary kind");
        $(".delete-key-table-entry").text("Delete literary kind");
        this.util.getLitararyKindList().done((data) => {
            console.log(data);
        });
    }
}