class KeyTableOriginalAuthorEditor extends KeyTableEditorBase {
    private readonly util: EditorsUtil;

    constructor() {
        super();
        this.util = new EditorsUtil();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new original author");
        $(".rename-key-table-entry").text("Rename original author");
        $(".delete-key-table-entry").text("Delete original author");
    };
}