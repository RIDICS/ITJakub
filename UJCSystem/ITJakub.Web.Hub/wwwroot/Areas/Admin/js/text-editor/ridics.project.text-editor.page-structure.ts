﻿class PageStructure {
    private readonly commentArea: CommentArea;
    private readonly util: EditorsApiClient;
    private readonly main: TextEditorMain;
    private readonly editor: Editor;

    constructor(commentArea: CommentArea, util: EditorsApiClient, main: TextEditorMain, editor: Editor) {
        this.commentArea = commentArea;
        this.util = util;
        this.main = main;
        this.editor = editor;
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

    private appendRenderedText(pageEl: JQuery): JQuery.jqXHR<ITextWithContent> {
        const textId = pageEl.data("page") as number;
        const renderedText = this.util.loadRenderedText(textId);
        const compositionAreaDiv = pageEl.find(".rendered-text");
        renderedText.done((data: ITextWithContent) => {
            const pageBody = data.text;
            const id = data.id;
            const versionId = data.versionId;
            const versionNumber = data.versionNumber;
            const compositionAreaEl = pageEl.children(".composition-area");
            compositionAreaEl.attr({ "data-id": id, "data-version-id": versionId, "data-version-number": versionNumber } as JQuery.PlainObject);
            compositionAreaDiv.empty().append(pageBody);
            pageEl.css("min-height", "0");
            var event = $.Event("pageConstructed", { page: textId });
            compositionAreaDiv.trigger(event);
        });
        renderedText.fail(() => {
            const pageName = pageEl.data("page-name");
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translateFormat("PageLoadFailed", [pageName], "RidicsProject").value)
                .buildElement();
            compositionAreaDiv.empty().append(alert);
            pageEl.css("min-height", "0");
        });
        renderedText.always(() => {
            pageEl.find(".loading").hide();
        });
        return renderedText;
    }

    private appendPlainText(pageEl: JQuery): JQuery.jqXHR<ITextWithContent> {
        const textId = pageEl.data("page") as number;
        const plainText = this.util.loadPlainText(textId);
        const textAreaEl = $(pageEl.find(".plain-text"));
        plainText.done((data: ITextWithContent) => {
            textAreaEl.val(data.text);
            const compositionAreaEl = pageEl.children(".composition-area");
            const id = data.id;
            const versionId = data.versionId;
            const versionNumber = data.versionNumber;
            compositionAreaEl.attr({ "data-id": id, "data-version-id": versionId, "data-version-number": versionNumber } as JQuery.PlainObject);
            var event = $.Event("pageConstructed", { page: textId });
            textAreaEl.trigger(event);
        });
        plainText.fail(() => {
            textAreaEl.val(localization.translate("ContentLoadFailed", "RidicsProject").value);
        });
        plainText.always(() => {
            pageEl.find(".loading").hide();
        });
        return plainText;
    }
}