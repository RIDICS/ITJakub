class KeyTableResponsiblePerson extends KeyTableEditorBase{
    private readonly util: EditorsUtil;

    constructor() {
        super();
        this.util = new EditorsUtil();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new responsible person");
        $(".rename-key-table-entry").text("Rename responsible person");
        $(".delete-key-table-entry").text("Delete responsible person");
    };
}