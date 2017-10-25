class PageStructure {
    private readonly commentArea: CommentArea;
    private readonly util: Util;
    private readonly main: TextEditorMain;
    private readonly editor: Editor;
    private readonly gui: TextEditorGui;

    constructor(commentArea: CommentArea, util: Util, main: TextEditorMain, editor: Editor, gui:TextEditorGui) {
        this.commentArea = commentArea;
        this.util = util;
        this.main = main;
        this.editor = editor;
        this.gui = gui;
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

    private appendRenderedText(textId: number, showPageNumber: boolean){
        const renderedText = this.util.loadRenderedText(textId);
        const pageEl = $(`*[data-page="${textId}"]`);
        const compositionAreaDiv = pageEl.find(".rendered-text");
        renderedText.done((data: IPageText) => {
            const compositionAreaEl = pageEl.children(".composition-area");           
            const pageBody = data.text;
            const id = data.id;
            const versionNumber = data.versionNumber;
            compositionAreaEl.attr({ "data-id": id, "data-version-number": versionNumber });
            $(compositionAreaDiv).append(pageBody);
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
            $(pageEl).find(".loading").hide();
        });
    }

    private appendPlainText(pageNumber: number){
        const plainText = this.util.loadPlainText(pageNumber);
        const pageEl = $(`*[data-page="${pageNumber}"]`);
        const textAreaEl = $(pageEl.find(".plain-text"));
        plainText.done((data: IPageText) => {
            textAreaEl.val(data.text);
            const compositionAreaEl = pageEl.children(".composition-area");
            const id = data.id;
            const versionNumber = data.versionNumber;
            compositionAreaEl.attr({ "data-id": id, "data-version-number": versionNumber });
            var event = $.Event("pageConstructed", { page: pageNumber });
            textAreaEl.trigger(event);
        });
        plainText.fail(() => {
            textAreaEl.val("Failed to load content.");
        });
        plainText.always(() => {
            $(pageEl).find(".loading").hide();
        });
    }
}