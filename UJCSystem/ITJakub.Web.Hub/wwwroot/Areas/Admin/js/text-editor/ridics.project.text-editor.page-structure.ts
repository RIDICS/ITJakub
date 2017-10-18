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
            elm += `<div class="page-number text-center ${invisibleClass}">[${pageName}]</div>`;
            elm += "<div class=\"col-xs-7 composition-area\">";
            elm += `<div class="page">`;

            if (!isEditingMode) {
                elm += "<div class=\"viewer\">";
                elm += `<span class="rendered-text"></span>`;
                elm += "</div>";
            }
            if (isEditingMode) {
                elm += "<div class=\"editor\">";
                elm += `<textarea class="plain-text"></textarea>`;
                elm += "</div>";
            }
            elm += "</div>";
            elm += "</div>";
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
                $(pageEl).children(".image-placeholder").hide();
        });
        renderedText.fail(() => {
            $(compositionAreaDiv).text("Failed to load content");
            pageEl.css("min-height", "0");
            $(pageEl).children(".image-placeholder").hide();
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
                $(pageEl).children(".image-placeholder").hide();
        });
        plainText.fail(() => {
            textAreaEl.val("Failed to load content.");
            $(pageEl).children(".image-placeholder").hide();
        });

    }
}