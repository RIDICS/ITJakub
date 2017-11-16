///<reference path="../editors-common-base/ridics.project.editors.util.ts" />

$(document).ready(() => {
    const main = new KeyTableEditorMain();
    main.init();
});

class KeyTableEditorMain {
    private readonly viewManager: KeyTableUtilManager;

    constructor() {
        this.viewManager = new KeyTableUtilManager();
    }

    init() {
        this.switchOnClick();
        var $splitterButton = $("#splitter-button");
        $splitterButton.click(() => {
            var $leftMenu = $("#left-menu");
            if ($leftMenu.is(":visible")) {
                $leftMenu.hide("slide", { direction: "left" });
                $splitterButton.html("<span class=\"glyphicon glyphicon-menu-right\"></span>");
            } else {
                $leftMenu.show("slide", { direction: "left" });
                $splitterButton.html("<span class=\"glyphicon glyphicon-menu-left\"></span>");
            }
        });
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
        case KeyTableEditorType.ResponsiblePersonEditor:
                this.viewManager.loadEditor(editor);
                //TODO
                break;
        case KeyTableEditorType.Keyword:
            this.viewManager.loadEditor(editor);
            //TODO
            break;
        case KeyTableEditorType.LiteraryOriginal:
            this.viewManager.loadEditor(editor);
            //TODO
            break;
        case KeyTableEditorType.OriginalAuthor:
            this.viewManager.loadEditor(editor);
            //TODO
            break;
        default:
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Invalid editor type");
            $("#project-layout-content").empty().append(error.buildElement());
        }
    }
}