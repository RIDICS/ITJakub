class CommentArea {
    private readonly util: Util;

    constructor(util: Util) {
        this.util = util;
    }

    /**
 * Creates html structure of comment area. Input must be sorted by comment id, nestedness, nested comment order. Returns JQuery object.
 * @param {string[][]} content  - Array or arrays of comment values in format {comment id, comment nestedness, comment page, person's name, comment body, order of nested comment, time}
 * @param {Number} pageNUmber - Number of page, which comments relates to.
 */
    constructCommentAreaHtml(content: ICommentSctucture[], pageNumber: number) {
        if (content === null) {
            return null;
        }
        const sectionStart = `<div class="col-xs-5 comment-area">`;
        const sectionEnd = `</div>`;
        const threadStart = `<ul class="media-list">`;
        const threadEnd = `</ul>`;
        const listStart = `<li class="media">`;
        const listEnd = "</li>";
        const commentLeftPartEnd = "</div>";
        const commentBodyStart = "<div class=\"media-body\">";
        const mainCommentBodyEnd = "</div>";
        const nestedCommentStart = "<div class=\"media\">";
        const nestedCommentLeftPartStart = "<div class=\"media-left nested-comment\">";
        const nestedCommentBodyEnd = "</div>";
        const nestedCommentEnd = "</div>";
        var nested: boolean = false;
        var page: number = 0;
        var id: string = "";
        var picture: string = "";
        var currentId: string = "";
        var needToCloseTag: boolean = false;
        var name: string = "";
        var body: string = "";
        var orderOfNestedComment: number = 0;
        var numberOfComments = content.length;
        var areaContent: string = "";
        areaContent += sectionStart;
        for (let i = 0; i < numberOfComments; i++) {
            id = content[i].id;
            if ((i > 0 && currentId !== id) || i === numberOfComments
            ) { //at the start of a new thread, close previous one
                needToCloseTag = true;
            }
            currentId = id;
            nested = (content[i].nested); //string to boolean
            page = content[i].page;
            name = content[i].name;
            body = content[i].body;
            picture = content[i].picture;
            orderOfNestedComment = content[i].order;
            var time = content[i].time;
            var timeUtc = new Date(time);
            var commentImage =
                `<a href="#"><img alt="48x48" class="media-object" src="${picture
                    }" style="width: 48px; height: 48px;"></a>`;
            var mainCommentLeftPartStart = `<div class="media-left main-comment" id="${id}-comment">`;
            var commentName = `<h5 class="media-heading">${name}</h5>`;
            var mainCommentBody =
                `<p class="comment-body">${body}</p><button class="respond-to-comment">Respond</button>`;
            var nestedCommentBody = `<p class="comment-body">${body}</p>`;
            if (page === pageNumber) {
                if (needToCloseTag) {
                    areaContent += mainCommentBodyEnd;
                    areaContent += listEnd;
                    areaContent += threadEnd;
                    areaContent += "<hr class=\"thread-divider\">";
                    needToCloseTag = false;
                }
                if (!nested) { //creating main comment structure
                    areaContent += threadStart;
                    areaContent += listStart;
                    areaContent += mainCommentLeftPartStart;
                    areaContent += commentImage;
                    areaContent += commentLeftPartEnd;
                    areaContent += commentBodyStart;
                    areaContent += `<div class="text-center id-in-comment-area text-muted">Commentary ${id}</div>`;
                    areaContent += commentName;
                    areaContent += `<p class="replied-on text-muted">On ${timeUtc.toDateString()} at ${timeUtc
                        .toTimeString()
                        .split(" ")[0]}</p>`; //only date and time, no timezone
                    areaContent += mainCommentBody;
                }
                if (nested) { //creating nested comment structure
                    areaContent += nestedCommentStart;
                    areaContent += nestedCommentLeftPartStart;
                    areaContent += commentImage;
                    areaContent += commentLeftPartEnd;
                    areaContent += commentBodyStart;
                    areaContent += commentName;
                    areaContent += `<p class="replied-on text-muted">On ${timeUtc.toDateString()} at ${timeUtc
                        .toTimeString().split(" ")[0]}</p>`; //only date and time, no timezone
                    areaContent += nestedCommentBody;
                    areaContent += nestedCommentBodyEnd;
                    areaContent += nestedCommentEnd;
                }
            }
            if (page !== pageNumber) {
                console.log(`Something is wrong. Page numbers are not equal. ${page} ${pageNumber}`);//TODO debug
            }
        }

        areaContent += sectionEnd;
        var html = $.parseHTML(areaContent);
        if (pageNumber % 2 === 0) {
            $(html).addClass("comment-area-collapsed-even"); //style even and odd comment sections separately
        } else {
            $(html).addClass("comment-area-collapsed-odd");
        }
        return html;
    }

    /**
     * Collapses comment area, adds buttons to enlarge
     * @param pageNumber - Number of page where to collapse comment area
     * @param sectionCollapsed - Whether section has to be collapsed
     * @param nestedCommentCollapsed - Whether nested comments have to be collapsed
     */
    collapseIfCommentAreaIsTall = (pageNumber: number, sectionCollapsed: boolean, nestedCommentCollapsed: boolean) => {
        var numberOfNestedComments = this.util.getNestedCommentsNumber(pageNumber);
        var areaContent = "";
        const ellipsisStart = "<div class=\"ellipsis-container\">";
        const ellipsisBodyStart = "<div class=\"ellipsis toggleCommentViewAreaSize\">";
        const ellipsisIconExpand =
            "<i class=\"fa fa-expand expand-icon fa-lg\" aria-hidden=\"true\" title=\"Expand comment area\"></i>";
        const ellipsisIconCollapse =
            "<i class=\"fa fa-compress collapse-icon fa-lg\" aria-hidden=\"true\" title=\"Collapse comment area\"></i>";
        const ellipsisBodyEnd = "</div>";
        const ellipsisEnd = "</div>";
        areaContent += ellipsisStart;
        areaContent += ellipsisBodyStart;
        areaContent += ellipsisIconExpand;
        areaContent += ellipsisIconCollapse;
        areaContent += ellipsisBodyEnd;
        areaContent += ellipsisEnd;
        const html = $.parseHTML(areaContent);
        const compositionArea = $(`*[data-page="${pageNumber}"]`).children(".composition-area");
        const compositionAreaHeight = $(compositionArea).height();
        const commentArea = compositionArea.siblings(".comment-area");
        const commentAreaHeight = $(commentArea).height();
        if (commentAreaHeight > compositionAreaHeight && commentAreaHeight > 170
        ) { // 140px - min comment area height, 170px to fit main comment and one typical response
            if (sectionCollapsed) {
                $(commentArea).toggleClass("comment-area-collapsed");
            }
            var children = $(commentArea).children(".media-list");
            children.each((index: number, childNode: Node) => {
                if (numberOfNestedComments[index] > 2) {
                    const child = childNode as Element;
                    if (nestedCommentCollapsed) {
                        $(child).children(".media").children(".media-body").children(".media")
                            .addClass("nested-comment-collapsed");
                    }
                    $(child).append(`<p class="text-center">
                                         <i class="fa fa-bars fa-lg toggle-nested-comments" aria-hidden="true" title="Toggle nested comments"></i>
                                     </p>`);
                }
            });
            $(commentArea).append(html);
        }
    }

    toggleAreaSizeIconHide(pageNumber: number) {
        const commentAreaCollapsedMaxHeight = 170;
        const compositionArea = $(`*[data-page="${pageNumber}"]`).children(".composition-area");
        const commentArea = compositionArea.siblings(".comment-area");
        const commentAreaHeight = $(commentArea).prop("scrollHeight");
        const ellipsisIconExpand = commentArea.find(".expand-icon");
        const ellipsisIconCollapse = commentArea.find(".collapse-icon");
        if (commentAreaHeight <= commentAreaCollapsedMaxHeight) {
            ellipsisIconExpand.hide();
            ellipsisIconCollapse.hide();
        }
        if (commentAreaHeight > commentAreaCollapsedMaxHeight) {
            if (!($(ellipsisIconCollapse).is(":visible") || $(ellipsisIconExpand).is(":visible"))) {
                ellipsisIconExpand.show();
            }
        }
    }

    constructCommentArea = (pageNumber: number) => {
        const content = this.util.parseLoadedCommentFiles(pageNumber);
        const html = this.constructCommentAreaHtml(content, pageNumber);
        const pageRow = $(`*[data-page="${pageNumber}"]`);;
        $(pageRow).append(html);
    }

    destroyCommentArea(pageNumber: number) {
        const page = $(`*[data-page="${pageNumber}"]`);
        const commentArea = page.children(".composition-area").siblings(".comment-area");
        $(commentArea).remove();
    }

    processToggleNestedCommentClick() {
        $(document).on("click", ".toggle-nested-comments",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                const editorPageContainer = ".tab-content";
                var target = $(event.target as HTMLElement);
                var parentComment = target.parents(".media-list");
                var commentArea = parentComment.parent(".comment-area");
                var compositionArea = commentArea.siblings(".composition-area");
                var pageNumber = compositionArea.children(".page").data("page") as number;
                var nestedComments = target.parent().siblings(".media").children(".media-body").children(".media");
                if ($(nestedComments).hasClass("nested-comment-collapsed")) {
                    const container = target.parents(".comment-area");
                    if ($(container).hasClass("comment-area-collapsed")) {
                        $(".nested-comment-opened").addClass("nested-comment-collapsed");
                        $(".nested-comment-opened").removeClass("nested-comment-opened");
                    }
                    $(nestedComments).removeClass("nested-comment-collapsed");
                    $(nestedComments).addClass("nested-comment-opened");
                } else {
                    $(nestedComments).removeClass("nested-comment-opened");
                    $(nestedComments).addClass("nested-comment-collapsed");
                    const container = target.parents(".comment-area");
                    const editorPageContainerEl = $(editorPageContainer);
                    if ($(container).hasClass("comment-area-collapsed")) {
                        container.animate({
                            scrollTop:
                                $(parentComment).offset().top - container.offset().top + container.scrollTop()
                        });
                    } else {
                        const scroll =
                        {
                            scrollTop: $(parentComment).offset().top -
                                editorPageContainerEl.offset().top +
                                editorPageContainerEl.scrollTop()
                        };
                        $(`${editorPageContainer}`).animate(scroll);
                    }
                }
                this.toggleAreaSizeIconHide(pageNumber);
            });
    }

    processToggleCommentAresSizeClick() {
        $(document).on("click", ".toggleCommentViewAreaSize",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                var target = $(event.target as HTMLElement);
                var commentViewArea = target.parents(".comment-area");
                var textCenter = $(target).parents(".ellipsis-container");
                $(commentViewArea).toggleClass("comment-area-collapsed");
                $(textCenter).children(".ellipsis").children(".expand-icon").toggle();
                $(textCenter).children(".ellipsis").children(".collapse-icon").toggle();
            });
    }


    reloadCommentArea(page: number) {
        this.destroyCommentArea(page);
        this.constructCommentArea(page);
        const commentAreaEl = $(`*[data-page="${page}"]`).children(".composition-area").siblings(".comment-area");
        const sectionWasCollapsed = $(commentAreaEl).hasClass("comment-area-collapsed");
        const nestedCommentsCollapsed = $(commentAreaEl).children(".media-list").children(".media")
            .children(".media-body").children(".media")
            .hasClass(
                "nested-comment-collapsed"); // if nested comments section was collapsed, collapse it after reload too.
        this.collapseIfCommentAreaIsTall(page, sectionWasCollapsed, nestedCommentsCollapsed);
        const buttonAreaSize = $(".toggleCommentViewAreaSize");
        buttonAreaSize.off();
        this.processToggleCommentAresSizeClick();
        const buttonNestedComments = $(".toggle-nested-comments");
        buttonNestedComments.off();
        this.processToggleNestedCommentClick();
    }
}