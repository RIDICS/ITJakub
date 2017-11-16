class KeyTableLiteraryOriginalEditor extends KeyTableEditorBase {
    private readonly util: EditorsUtil;

    constructor() {
        super();
        this.util = new EditorsUtil();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new literary original");
        $(".rename-key-table-entry").text("Rename literary original");
        $(".delete-key-table-entry").text("Delete literary original");
    };
}