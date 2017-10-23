class PageStructure {
    private readonly commentArea: CommentArea;
    private readonly util: Util;
    private readonly main: TextEditorMain;
    private readonly editor: Editor;

    constructor(commentArea: CommentArea, util: Util, main: TextEditorMain, editor: Editor) {
        this.commentArea = commentArea;
        this.util = util;
        this.main = main;
        this.editor = editor;
    }

    createPage(pageNumber: number) {
        const pageEl = $(`*[data-page="${pageNumber}"]`);
        if (pageEl.length) {
            const isEditingMode = this.editor.getIsEditingMode();
            const showPageNumber = this.main.getShowPageNumbers();
            let elm = "";
            const pageName = pageEl.data("page-name");
            let invisibleClass = "";
            if (!showPageNumber) {
                invisibleClass = "invisible";
            }
            elm += `<div class="page-number text-center ${invisibleClass}">[${pageName}]</div><div class="col-xs-7 composition-area"><div class="loading composition-area-loading"></div><div class="page">`;
            if (!isEditingMode) {
                elm += `<div class="viewer"><span class="rendered-text"></span></div></div></div>`;
            }
            if (isEditingMode) {
                elm += `<div class="editor"><textarea class="plain-text"></textarea></div></div></div>`;
            }
            const html = $.parseHTML(elm);
            $(pageEl).append(html);
            if (!isEditingMode) {
                this.appendRenderedText(pageNumber, showPageNumber);
            }
            if (isEditingMode) {
                this.appendPlainText(pageNumber);
            }
            this.commentArea.asyncConstructCommentArea(pageNumber,
                true,
                true);
        } else {
            console.log("You are requesting to create a page for which element does not exist");
        }
    }

    appendRenderedText(textId: number, showPageNumber: boolean) {
        const renderedText = this.util.loadRenderedText(textId);
        const pageEl = $(`*[data-page="${textId}"]`);
        const compositionAreaDiv = pageEl.find(".rendered-text");
        renderedText.done((data: IPageText) => {
            const pageBody = data.text;
            $(compositionAreaDiv).append(pageBody);
            pageEl.css("min-height", "0");
            var event = $.Event("pageConstructed", { page: textId });
            compositionAreaDiv.trigger(event);
            $(pageEl).find(".loading").hide();
        });
        renderedText.fail(() => {
            $(compositionAreaDiv).text("Failed to load content");
            pageEl.css("min-height", "0");
            $(pageEl).find(".loading").hide();
        });

    }

    appendPlainText(pageNumber: number) {
        const plainText = this.util.loadPlainText(pageNumber);
        const pageEl = $(`*[data-page="${pageNumber}"]`);
        const textAreaEl = $(pageEl.find(".plain-text"));
        plainText.done((data: IPageText) => {
            textAreaEl.val(data.text);
            var event = $.Event("pageConstructed", { page: pageNumber });
            textAreaEl.trigger(event);
            $(pageEl).find(".loading").hide();
        });
        plainText.fail(() => {
            textAreaEl.val("Failed to load content.");
            $(pageEl).find(".loading").hide();
        });

    }
}