class PageStructure {
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
            const pageRow = targetEl.parent(".page-row");
            const isEditingMode = pageRow.data(this.editor.getEditModeSelector());

            let ajax;
            if (isEditingMode) {
                ajax = this.appendPlainText(pageRow);
            }
            if (!isEditingMode) {
                ajax = this.appendRenderedText(pageRow);
            }

            ajax.always(() => {
                this.commentArea.updateCommentAreaHeight(pageRow);
            });
        }

        if (targetEl.hasClass("comment-area")) {
            this.commentArea.asyncConstructCommentArea(targetEl);
        }
    }

    loadPage(pageEl: JQuery) {
        const commentArea = pageEl.children(".comment-area");
        const isEditingMode = pageEl.data(this.editor.getEditModeSelector());
        if (isEditingMode) {
            const ajax = this.appendPlainText(pageEl);
            const ajax2 = this.commentArea.asyncConstructCommentArea(commentArea);
            $.when(ajax, ajax2).done(() => {
                this.commentArea.updateCommentAreaHeight(pageEl);
                this.commentArea.collapseIfCommentAreaIsTall(commentArea, true, true);
            });
        }
        if (!isEditingMode) {
            const ajax = this.appendRenderedText(pageEl);
            const ajax2 = this.commentArea.asyncConstructCommentArea(commentArea);
            $.when(ajax, ajax2).done(() => {
                this.commentArea.updateCommentAreaHeight(pageEl);
                this.commentArea.collapseIfCommentAreaIsTall(commentArea, true, true);
            });
        }
    }

    private appendRenderedText(pageEl: JQuery): JQuery.jqXHR<ITextWithContent> {
        const pageId = pageEl.data("page-id") as number;
        const renderedText = this.util.loadRenderedText(pageId);
        const compositionAreaDiv = pageEl.find(".rendered-text");
        renderedText.done((data: ITextWithContent) => {
            if (data == null) {
                var infoAlert = new AlertComponentBuilder(AlertType.Info)
                    .addContent(localization.translate("PageDoesNotContainText", "RidicsProject").value)
                    // TODO add button for creating text
                    .buildElement();
                compositionAreaDiv.empty().append(infoAlert);
            } else {
                const pageBody = data.text;
                const id = data.id;
                const versionId = data.versionId;
                const versionNumber = data.versionNumber;
                const compositionAreaEl = pageEl.children(".composition-area");
                compositionAreaEl.attr({ "data-id": id, "data-version-id": versionId, "data-version-number": versionNumber } as JQuery.PlainObject);
                compositionAreaDiv.empty().append(pageBody);
                pageEl.css("min-height", "0")
                    .attr("data-text-id", id)
                    .data("text-id", id);

                if (pageEl.hasClass("comment-never-loaded")) {
                    const commentAreaEl = $(".comment-area", pageEl);
                    this.commentArea.asyncConstructCommentArea(commentAreaEl);
                }
            }

            var event = $.Event("pageConstructed");
            compositionAreaDiv.trigger(event, { pageId: pageId } as IPageConstructedEventData);
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
        const pageId = pageEl.data("page-id") as number;
        const plainText = this.util.loadPlainText(pageId);
        const textAreaEl = $(pageEl.find(".plain-text"));
        plainText.done((data: ITextWithContent) => {
            textAreaEl.val(data.text);
            const compositionAreaEl = pageEl.children(".composition-area");
            const id = data.id;
            const versionId = data.versionId;
            const versionNumber = data.versionNumber;
            compositionAreaEl.attr({ "data-id": id, "data-version-id": versionId, "data-version-number": versionNumber } as JQuery.PlainObject);
            var event = $.Event("pageConstructed");
            textAreaEl.trigger(event, { pageId: pageId } as IPageConstructedEventData);
            if (pageEl.hasClass("init-editor")) {
                pageEl.removeClass("init-editor");
                this.editor.addEditor(pageEl);
            }

            pageEl.attr("data-text-id", id)
                .data("text-id", id);
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

interface IPageConstructedEventData {
    pageId: number;
}