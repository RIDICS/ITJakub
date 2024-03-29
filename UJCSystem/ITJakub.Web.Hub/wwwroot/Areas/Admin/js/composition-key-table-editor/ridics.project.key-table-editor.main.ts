﻿$(document.documentElement).ready(() => {
    const main = new KeyTableEditorMain();
    main.init();
});

class KeyTableEditorMain {

    init() {
        const editorsList = $(".list-group");
        const editorsListItems = editorsList.children(".key-table-editor-selection");
        const firstEditorListItem = editorsListItems.first();
        const editorId = firstEditorListItem.data("editor-type") as number;
        this.initEditor(editorId);
        firstEditorListItem.addClass("active");
        this.switchOnClick();
        const $splitterButton = $("#splitter-button");
        $splitterButton.click(() => {
            const $leftMenu = $("#left-menu");
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
                const targetEl = $(event.target as Node as Element);
                const target: KeyTableEditorType = targetEl.data("editor-type");
                targetEl.addClass("active");
                targetEl.siblings(".key-table-editor-selection").removeClass("active");
                this.initEditor(target);
            });
    }

    private initEditor(editor: KeyTableEditorType) {
        switch (editor) {
        case KeyTableEditorType.Genre:
            const genreEditor = new KeyTableGenreEditor();
            genreEditor.init();
            break;
        case KeyTableEditorType.Category:
            const categoryEditor = new KeyTableCategoryEditor();
            categoryEditor.init();
            break;
        case KeyTableEditorType.Kind:
            const kindEditor = new KeyTableKindEditor();
            kindEditor.init();
            break;
        case KeyTableEditorType.ResponsiblePerson:
            const personEditor = new KeyTableResponsiblePerson();
            personEditor.init();
            break;
        case KeyTableEditorType.ResponsiblePersonType:
            const editorPersonEditor = new KeyTableResponsiblePersonType();
            editorPersonEditor.init();
            break;
        case KeyTableEditorType.Keyword:
            const keywordEditor = new KeyTableKeywordEditor();
            keywordEditor.init();
            break;
        case KeyTableEditorType.LiteraryOriginal:
            const literaryOriginalEditor = new KeyTableLiteraryOriginalEditor();
            literaryOriginalEditor.init();
            break;
        case KeyTableEditorType.OriginalAuthor:
            const originalAuthorEditor = new KeyTableOriginalAuthorEditor();
            originalAuthorEditor.init();
            break;
        case KeyTableEditorType.Term:
            const termEditor = new KeyTableTermEditor();
            termEditor.init();
            break;
        default:
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Invalid editor type");
            $("#project-layout-content").empty().append(error.buildElement());
        }
    }
}

class KeyTableTermEditor extends KeyTableEditorBase {
    init() {
        $("#project-layout-content").find("*").off();
        this.createEntryButtonEl.text("Create");
        this.changeEntryButtonEl.text("Rename");
        this.deleteEntryButtonEl.text("Delete");
        this.titleEl.text("Terms");
        this.unbindEventsDialog();

        const listEl = $(".selectable-list-div");
        listEl.empty();
        listEl.text("Terms editing is not supported. Terms are created automatically during DOCX import.");

        this.initPagination(0, 1, () => {});
    }
}
