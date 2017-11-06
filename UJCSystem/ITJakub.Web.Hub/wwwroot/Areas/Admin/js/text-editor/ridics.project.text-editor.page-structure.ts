class PageStructure {
    private readonly commentArea: CommentArea;
    private readonly util: EditorsUtil;
    private readonly main: TextEditorMain;
    private readonly editor: Editor;
    private readonly gui: TextEditorGui;

    constructor(commentArea: CommentArea, util: EditorsUtil, main: TextEditorMain, editor: Editor, gui: TextEditorGui) {
        this.commentArea = commentArea;
        this.util = util;
        this.main = main;
        this.editor = editor;
        this.gui = gui;
    }

    loadSection(targetEl: JQuery) {
        if (targetEl.hasClass("composition-area")) {
            const isEditingMode = this.editor.getIsEditingMode();
            if (isEditingMode) {
                this.appendPlainText(targetEl.parent(".page-row"));
            }
            if (!isEditingMode) {
                this.appendRenderedText(targetEl.parent(".page-row"));
            }
        }

        if (targetEl.hasClass("comment-area")) {
            this.commentArea.asyncConstructCommentArea(targetEl);
        }
    }

    loadPage(pageEl: JQuery) {
        const commentArea = pageEl.children(".comment-area");
        const isEditingMode = this.editor.getIsEditingMode();
        if (isEditingMode) {
            const ajax = this.appendPlainText(pageEl);
            const ajax2 = this.commentArea.asyncConstructCommentArea(commentArea);
            $.when(ajax, ajax2).done(() => {
                this.commentArea.collapseIfCommentAreaIsTall(commentArea, true, true);
            });
        }
        if (!isEditingMode) {
            const ajax = this.appendRenderedText(pageEl);
            const ajax2 = this.commentArea.asyncConstructCommentArea(commentArea);
            $.when(ajax, ajax2).done(() => {
                this.commentArea.collapseIfCommentAreaIsTall(commentArea, true, true);
            });
        }
    }

    private appendRenderedText(pageEl: JQuery): JQueryXHR {
        const textId = pageEl.data("page") as number;
        const renderedText = this.util.loadRenderedText(textId);
        const compositionAreaDiv = pageEl.find(".rendered-text");
        renderedText.done((data: IPageText) => {
            const compositionAreaEl = pageEl.children(".composition-area");
            const pageBody = data.text;
            const id = data.id;
            const versionNumber = data.versionNumber;
            compositionAreaEl.attr({ "data-id": id, "data-version-number": versionNumber });
            compositionAreaDiv.append(pageBody);
            pageEl.css("min-height", "0");
            var event = $.Event("pageConstructed", { page: textId });
            compositionAreaDiv.trigger(event);
        });
        renderedText.fail(() => {
            const pageName = pageEl.data("page-name");
            this.gui.showMessageDialog("Fail", `Failed to load page ${pageName}`);
            $(compositionAreaDiv).text();
            pageEl.css("min-height", "0");
        });
        renderedText.always(() => {
            pageEl.find(".loading").hide();
        });
        return renderedText;
    }

    private appendPlainText(pageEl: JQuery): JQueryXHR {
        const textId = pageEl.data("page") as number;
        const plainText = this.util.loadPlainText(textId);
        const textAreaEl = $(pageEl.find(".plain-text"));
        plainText.done((data: IPageText) => {
            textAreaEl.val(data.text);
            const compositionAreaEl = pageEl.children(".composition-area");
            const id = data.id;
            const versionNumber = data.versionNumber;
            compositionAreaEl.attr({ "data-id": id, "data-version-number": versionNumber });
            var event = $.Event("pageConstructed", { page: textId });
            textAreaEl.trigger(event);
        });
        plainText.fail(() => {
            textAreaEl.val("Failed to load content.");
        });
        plainText.always(() => {
            pageEl.find(".loading").hide();
        });
        return plainText;
    }
}