class PageStructure {
    private readonly commentArea: CommentArea;
    private readonly util: Util;

    constructor(commentArea: CommentArea, util: Util) {
        this.commentArea = commentArea;
        this.util = util;
    }

    createPage(pageNumber: number) {
        console.log("trying to create page " + pageNumber);//TODO debug
        let elm = "";
        const renderedText = this.util.loadRenderedText(pageNumber);
        const pageEl = $(`*[data-pageq="${pageNumber}"]`);
        elm += "<div class=\"col-xs-7 composition-area\">";
        elm += `<div class="page" data-page="${pageNumber}">`;
        elm += "<div class=\"viewer\">";
        elm += `<span>${renderedText}</span>`;
        elm += "</div>";
        elm += "</div>";
        elm += "</div>";
        const html = $.parseHTML(elm);
        $(pageEl).append(html);
        this.commentArea.constructCommentArea(pageNumber);
        this.commentArea.collapseIfCommentAreaIsTall(pageNumber,
            true,
            true); //collapse section on page load, collapse nested comments on page load
        this.commentArea.toggleAreaSizeIconHide(pageNumber);
    }
}