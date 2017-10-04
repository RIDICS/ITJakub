class PageStructure {
    private readonly commentArea: CommentArea;
    private readonly util: Util;
    private readonly main: TextEditorMain;

    constructor(commentArea: CommentArea, util: Util, main:TextEditorMain) {
        this.commentArea = commentArea;
        this.util = util;
        this.main = main;
    }

    createPage(pageNumber: number) {
        const showPageNumber = this.main.getShowPageNumbers();
        console.log("trying to create page " + pageNumber);//TODO debug
        let elm = "";
        const pageEl = $(`*[data-page="${pageNumber}"]`);
        elm += "<div class=\"col-xs-7 composition-area\">";
        elm += `<div class="page">`;
        elm += "<div class=\"viewer\">";
        elm += `<span class="rendered-text"></span>`;
        elm += "</div>";
        elm += "</div>";
        elm += "</div>";
        const html = $.parseHTML(elm);
        $(pageEl).append(html);
        this.appendRenderedText(pageNumber, showPageNumber);
        this.commentArea.constructCommentArea(pageNumber);
        this.commentArea.collapseIfCommentAreaIsTall(pageNumber,
            true,
            true); //collapse section on page load, collapse nested comments on page load
        this.commentArea.toggleAreaSizeIconHide(pageNumber);
    }

    appendRenderedText(pageNumber: number, showPageNumber: boolean) {
        var displayStyle;
        if (showPageNumber) {
            displayStyle = "display: block;";
        } else {
            displayStyle = "display: none;";
        }
        const renderedText = this.util.loadRenderedText(pageNumber);
        renderedText.done((data: string) => {
            if (data !== "error-no-file") {
                const pageEl = $(`*[data-page="${pageNumber}"]`);
                const compositionAreaDiv = pageEl.find(".rendered-text");
                const pageBody = `<div class="page-number" style="${displayStyle}">[${pageNumber}]</div>${data}`;
                $(compositionAreaDiv).append(pageBody);
            }
        });

    }
}