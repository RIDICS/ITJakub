class KeyTableResponsiblePersonEditor extends KeyTableEditorBase{
    private readonly util: EditorsUtil;

    constructor() {
        super();
        this.util = new EditorsUtil();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new responsible person (editor)");
        $(".rename-key-table-entry").text("Rename responsible person (editor)");
        $(".delete-key-table-entry").text("Delete responsible person (editor)");
    };
}