class CommentArea {
    private readonly util: Util;
    private readonly gui: TextEditorGui;

    constructor(util: Util, gui: TextEditorGui) {
        this.util = util;
        this.gui = gui;
    }

    /**
 * Creates html structure of comment area. Input must be sorted by comment id, nestedness, nested comment order. Returns JQuery object.
 * @param {string[][]} content  - Array or arrays of comment values in format {comment id, comment nestedness, comment page, person's name, comment body, order of nested comment, time}
 * @param {Number} pageNUmber - Number of page, which comments relates to.
 */
    private constructCommentAreaHtml(content: ICommentSctucture[], commentAreaEl: JQuery) {
        if (content === null) {
            return null;
        }
        const textId = commentAreaEl.parent(".page-row").data("page") as number;
        const threadsContainerStart = `<div class="threads-container">`;
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
        const threadsContainerEnd = "</div>";
        var nested: boolean = false;
        var commentTextId: number = 0;
        var textReferenceId: string = "";
        var id: number = 0;
        var picture: string = "";
        var currentId: string = "";
        var needToCloseTag: boolean = false;
        var name: string = "";
        var surname: string = "";
        var body: string = "";
        var orderOfNestedComment: number = 0;
        var numberOfComments = content.length;
        var areaContent: string = "";
        areaContent += threadsContainerStart;
        for (let i = 0; i < numberOfComments; i++) {
            id = content[i].id;
            textReferenceId = content[i].textReferenceId;
            if ((i > 0 && currentId !== textReferenceId) || i === numberOfComments
            ) { //at the start of a new thread, close previous one
                needToCloseTag = true;
            }
            currentId = textReferenceId;
            nested = content[i].nested;
            commentTextId = content[i].textId;
            name = content[i].name;
            surname = content[i].surname;
            body = content[i].text;
            picture = content[i].picture;
            orderOfNestedComment = content[i].order;
            var time = content[i].time;
            var timeUtc = new Date(time);
            var commentImage =
                `<a href="#"><img alt="48x48" class="media-object" src="${picture
                    }" style="width: 48px; height: 48px;"></a>`;
            var mainCommentLeftPartStart =
                `<div class="media-left main-comment" id="${textReferenceId}-comment" data-parent-comment-id="${id}">`;
            var commentName = `<h5 class="media-heading">${name} ${surname}</h5>`;
            var mainCommentBody =
                `<p class="comment-body">${body}</p><button class="respond-to-comment">Respond</button>`;
            var nestedCommentBody = `<p class="comment-body">${body}</p>`;
            if (commentTextId === textId) {
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
                    areaContent += `<div class="text-center id-in-comment-area text-muted">Commentary ${textReferenceId
                        }</div>`;
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
            if (commentTextId !== textId) {
                console.log(`Something is wrong. Page numbers are not equal. ${commentTextId} ${textId}`);
            }
        }
        areaContent += threadsContainerEnd;
        var html = $.parseHTML(areaContent);
        return html;
    }

    /**
     * Collapses comment area, adds buttons to enlarge
     * @param pageNumber - Number of page where to collapse comment area
     * @param sectionCollapsed - Whether section has to be collapsed
     * @param nestedCommentCollapsed - Whether nested comments have to be collapsed
     */
    collapseIfCommentAreaIsTall = (
        commentAreaEl: JQuery,
        sectionCollapsed: boolean,
        nestedCommentCollapsed: boolean) => {
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
        const compositionAreaEl = commentAreaEl.siblings(".composition-area");
        const compositionAreaHeight = compositionAreaEl.height();
        const commentAreaHeight = commentAreaEl.height();
        const commentAreaMinHeight = 170;
        if (commentAreaHeight > compositionAreaHeight
        ) {
            if (sectionCollapsed) {
                commentAreaEl.toggleClass("comment-area-collapsed");
                commentAreaEl.height(compositionAreaHeight);
            }
            const children = commentAreaEl.children(".media-list");
            const commentsHeight = children.prop("scrollHeight");
            if (commentsHeight > commentAreaHeight || commentsHeight > commentAreaMinHeight) {
                children.each((index: number, childNode: Element) => {
                    const threads = $(childNode);
                    if (nestedCommentCollapsed) {
                        threads.children(".media").children(".media-body").children(".media")
                            .addClass("nested-comment-collapsed");
                    }
                    threads.append(`<p class="text-center">
                                        <i class="fa fa-bars fa-lg toggle-nested-comments" aria-hidden="true" title="Toggle nested comments"></i>
                                    </p>`);
                });
            }
            commentAreaEl.append(html);
        }
        this.toggleAreaSizeIconHide(
            commentAreaEl); //collapse section on page load, collapse nested comments on page load
    }

    updateCompositionAreaHeight(pageEl: JQuery) {
        const compositionAreaEl = pageEl.children(".composition-area");
        const commentAreaEl = pageEl.children(".comment-area");
        commentAreaEl.height(compositionAreaEl.height());
    }

    toggleAreaSizeIconHide(commentAreaContainer: JQuery) {
        const commentAreaContainerHeight = commentAreaContainer.height();
        const threadsEl = commentAreaContainer.children(".threads-container");
        const commentsHeight = threadsEl.prop("scrollHeight");
        const ellipsisIconExpand = commentAreaContainer.find(".expand-icon");
        const ellipsisIconCollapse = commentAreaContainer.find(".collapse-icon");
        console.log(commentAreaContainer.parent(".page-row").data("page-name"));
        console.log(commentsHeight);
        console.log(commentAreaContainerHeight);
        if (commentsHeight <= commentAreaContainerHeight || typeof commentsHeight === "undefined") {
            ellipsisIconExpand.hide();
            ellipsisIconCollapse.hide();
        }
        if (commentsHeight > commentAreaContainerHeight) {
            if (!($(ellipsisIconCollapse).is(":visible") || $(ellipsisIconExpand).is(":visible"))) {
                ellipsisIconExpand.show();
            }
        }
    }

    /**
 * Loads contents of files with comments in a page from the server.
 * @param {Number} pageNumber Number of page for which to load comment file contents
 * @param {boolean} sectionCollapsed Whether comment area has to be collapsed
 * @param {boolean} nestedCommentCollapsed Whether nested comments have to be collapsed
 */
    asyncConstructCommentArea(commentAreaEl: JQuery): JQueryXHR {
        const pageRowEl = commentAreaEl.parent(".page-row");
        const pageName = pageRowEl.data("page-name") as string;
        const textId = pageRowEl.data("page") as number;
        const ajax = $.post(`${this.util.getServerAddress()}admin/project/LoadCommentFile`,
            { textId: textId });
        ajax.done(
            (fileContent: ICommentSctucture[]) => {
                if (fileContent.length > 0) {
                    this.loadCommentFile(fileContent, commentAreaEl);
                } else {
                    commentAreaEl.text("No comments yet."); //TODO style
                }
            });
        ajax.fail(() => {
            this.gui.showMessageDialog("Error", `Failed to construct comment area for page ${pageName}`);
        });
        return ajax;
    }

    private loadCommentFile(contentStringArray: ICommentSctucture[],
        commentAreaEl: JQuery) {
        if (contentStringArray !== null && typeof contentStringArray !== "undefined") {
            this.parseLoadedCommentFiles(contentStringArray, commentAreaEl);
        }
    }

    /**
     * Parses comments to construct comment area later.
     * @param {ICommentSctucture[]} content Array of comments
     * @param {number} pageNumber Number of page comments are on
     * @param {boolean} sectionCollapsed Whether comment area has to be collapsed
     * @param nestedCommentCollapsed Whether nested comments have to be collapsed
     */
    private parseLoadedCommentFiles(content: ICommentSctucture[],
        commentAreaEl: JQuery) {
        if (content !== null && typeof content !== "undefined") {
            if (content.length > 0) {
                content.sort((a, b) => { //sort by id, ascending
                    if (a.id < b.id) {
                        return -1;
                    }
                    if (a.id === b.id) {
                        return 0;
                    }
                    if (a.id > b.id) {
                        return 1;
                    }
                });

                let id = content[0].id;
                const indexes: number[] = [];
                for (let i = 0; i < content.length; i++) {
                    const currentId = content[i].id;
                    if (currentId !== id) {
                        indexes.push(i);
                        id = currentId;
                    }
                }
                const sortedContent = this.util.splitArrayToArrays(content, indexes);
                this.constructCommentArea(sortedContent, commentAreaEl);
            }
        }
    }

    private constructCommentArea = (content: ICommentSctucture[],
        commentAreaEl: JQuery) => {
        const html = this.constructCommentAreaHtml(content, commentAreaEl);
        $(commentAreaEl).append(html);
    }

    private destroyCommentArea(commentArea: JQuery) {
        $(commentArea).remove();
    }

    processToggleNestedCommentClick() {
        $("#project-resource-preview").on("click",
            ".toggle-nested-comments",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                const editorPageContainer = ".pages-start";
                var target = $(event.target as HTMLElement);
                var parentComment = target.parents(".media-list");
                var commentArea = parentComment.parent(".comment-area");
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
                this.toggleAreaSizeIconHide(commentArea);
            });
    }

    processToggleCommentAresSizeClick() {
        $("#project-resource-preview").on("click",
            ".toggleCommentViewAreaSize",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                const target = $(event.target as HTMLElement);
                const commentArea = target.parents(".comment-area");
                const commentsEl = commentArea.children(".media-list");
                const commentsHeight = commentsEl.height();
                const textCenter = $(target).parents(".ellipsis-container");
                const compositionArea = commentArea.siblings(".composition-area");
                const compositionAreaHeight = compositionArea.height();
                $(commentArea).toggleClass("comment-area-collapsed");
                if (commentArea.hasClass("comment-area-collapsed")) {
                    $(commentArea).height(compositionAreaHeight);
                } else {
                    if (commentsHeight > commentArea.height()) {
                        $(commentArea).height("auto");
                    }
                }
                $(textCenter).children(".ellipsis").children(".expand-icon").toggle();
                $(textCenter).children(".ellipsis").children(".collapse-icon").toggle();
            });
    }


    reloadCommentArea(textId: number) {
        const commentAreaEl = $(`*[data-page="${textId}"]`).children(".comment-area");
        this.destroyCommentArea(commentAreaEl);
        const sectionWasCollapsed = $(commentAreaEl).hasClass("comment-area-collapsed");
        const nestedCommentsCollapsed = $(commentAreaEl).children(".media-list").children(".media")
            .children(".media-body").children(".media")
            .hasClass(
                "nested-comment-collapsed"); // if nested comments section was collapsed, collapse it after reload too.
        this.asyncConstructCommentArea(commentAreaEl);
        this.collapseIfCommentAreaIsTall(commentAreaEl, sectionWasCollapsed, nestedCommentsCollapsed);
        const buttonAreaSize = $(".toggleCommentViewAreaSize");
        buttonAreaSize.off();
        this.processToggleCommentAresSizeClick();
        const buttonNestedComments = $(".toggle-nested-comments");
        buttonNestedComments.off();
        this.processToggleNestedCommentClick();
    }
}