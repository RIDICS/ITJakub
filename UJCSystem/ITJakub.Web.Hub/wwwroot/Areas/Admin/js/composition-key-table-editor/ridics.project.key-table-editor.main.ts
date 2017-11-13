///<reference path="../editors-common-base/ridics.project.editors.util.ts" />

$(document).ready(() => {
    const main = new KeyTableEditorMain();
    main.init();
});

class KeyTableEditorMain {
    private readonly viewManager: KeyTableViewManager;

    constructor() {
        this.viewManager = new KeyTableViewManager();
    }

    init() {
        this.switchOnClick();
        this.viewManager.loadEditor(KeyTableEditorType.Genre);
    }

    private switchOnClick() {
        $(".left-menu").on("click",
            ".key-table-editor-selection",
            (event) => {
                const target: KeyTableEditorType = $(event.target).data("editor-type");
                this.initEditor(target);
            });
    }

    private initEditor(editor: KeyTableEditorType) {
        switch (editor) {
        case KeyTableEditorType.Genre:
            this.viewManager.loadEditor(editor);
            const genreEditor = new KeyTableGenreEditor();
            genreEditor.init();
            break;
        case KeyTableEditorType.Category:
            this.viewManager.loadEditor(editor);
            const categoryEditor = new KeyTableCategoryEditor();
            categoryEditor.init();
            break;
        case KeyTableEditorType.Kind:
            this.viewManager.loadEditor(editor);
            const kindEditor = new KeyTableKindEditor();
            kindEditor.init();
            break;
        case KeyTableEditorType.ResponsiblePerson:
            this.viewManager.loadEditor(editor);
            const personEditor = new KeyTableResponsiblePersonEditor();
            personEditor.init();
            break;
        default:
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Invalid editor type");
            $("#project-layout-content").empty().append(error.buildElement());
        }
    }
}