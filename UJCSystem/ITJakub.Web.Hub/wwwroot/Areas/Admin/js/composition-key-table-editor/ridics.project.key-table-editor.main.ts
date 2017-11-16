﻿///<reference path="../editors-common-base/ridics.project.editors.util.ts" />

$(document).ready(() => {
    const main = new KeyTableEditorMain();
    main.init();
});

class KeyTableEditorMain {

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
        case KeyTableEditorType.ResponsiblePersonEditor:
            const editorPersonEditor = new KeyTableResponsiblePersonEditor();
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
        default:
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Invalid editor type");
            $("#project-layout-content").empty().append(error.buildElement());
        }
    }
}