class CommentArea {
    private readonly util: EditorsUtil;
    private readonly gui: TextEditorGui;

    constructor(util: EditorsUtil, gui: TextEditorGui) {
        this.util = util;
        this.gui = gui;
    }

    init() {
        this.processToggleCommentAresSizeClick();
        this.processToggleNestedCommentClick();
        this.processDeleteCommentClick();
    }

    /**
 * Creates html structure of comment area. Input must be sorted by comment id, nestedness, nested comment order. Returns JQuery object.
 * @param {ICommentSctucture[]} content  - Array or comments.
 * @param {JQuery} commentAreaEl - Comment area element for which to construct area..
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
            id = content[i].id;
            orderOfNestedComment = content[i].order;
            var unixTimeMilliseconds = content[i].time;
            var timeUtc = new Date(unixTimeMilliseconds);
            var commentBodyStart = `<div class="media-body" data-comment-id=${id}>`;
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
                    areaContent += `<div class="row comment-actions-row"><div class="col-xs-8"></div><div class="col-xs-4"><div class="btn-group"><button type="button" class="edit-comment">Edit</button><button type="button" class="delete-comment">Delete</button></div></div></div>`;
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
     * @param {JQuery} commentAreaEl - Comment area element which needs collapsing.
     * @param {boolean} sectionCollapsed - Whether section has to be collapsed
     * @param {boolean} nestedCommentCollapsed - Whether nested comments have to be collapsed
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
        if (commentAreaHeight > compositionAreaHeight
        ) {
            if (sectionCollapsed) {
                commentAreaEl.addClass("comment-area-collapsed");
                commentAreaEl.height(compositionAreaHeight);
            }
            if (nestedCommentCollapsed) {
                this.collapseNestedComments(commentAreaEl);
            }
            const threadsContainer = commentAreaEl.children(".threads-container");
            threadsContainer.append(html);
        }
        this.toggleAreaSizeIconHide(
            commentAreaEl); //collapse section on page load, collapse nested comments on page load
    }

    private collapseNestedComments(commentAreaEl: JQuery) {
        const threadsContainer = commentAreaEl.children(".threads-container");
        const children = threadsContainer.children(".media-list");
        const commentsHeight = children.prop("scrollHeight");
        const commentAreaHeight = commentAreaEl.height();
        if (children.length) {
            if (commentsHeight > commentAreaHeight) {
                children.each((index: number, childNode: Element) => {
                    const thread = $(childNode);
                    const nestedComments = thread.children(".media").children(".media-body").children(".media");
                    if (nestedComments.length) {
                        nestedComments.addClass("nested-comment-collapsed");
                        thread.append(`<p class="text-center toggle-nested-comments-icon-container">
                                           <i class="fa fa-bars fa-lg toggle-nested-comments" aria-hidden="true" title="Toggle nested comments"></i>
                                       </p>`);
                    }
                });
            }
        }
    }

    updateCommentAreaHeight(pageEl: JQuery) {
        const compositionAreaEl = pageEl.children(".composition-area");
        const commentAreaEl = pageEl.children(".comment-area");
        const compositionAreaHeight = compositionAreaEl.height();
        const commentAreaHeight = commentAreaEl.height();
        if (commentAreaEl.hasClass("comment-area-collapsed")) {
            commentAreaEl.height(compositionAreaHeight);
        } else {
            if (commentAreaHeight < compositionAreaHeight) {
                commentAreaEl.height(compositionAreaHeight);
            } else {
                commentAreaEl.height("auto");
            }
        }
    }

    toggleAreaSizeIconHide(commentAreaContainer: JQuery) {
        const commentAreaContainerHeight = commentAreaContainer.height();
        const threadsEl = commentAreaContainer.children(".threads-container");
        const commentsHeight = threadsEl.prop("scrollHeight");
        const ellipsisIconExpand = commentAreaContainer.find(".expand-icon");
        const ellipsisIconCollapse = commentAreaContainer.find(".collapse-icon");
        if (commentsHeight < commentAreaContainerHeight || typeof commentsHeight === "undefined") {
            ellipsisIconExpand.hide();
            ellipsisIconCollapse.hide();
        }
        if (commentsHeight > commentAreaContainerHeight) {
            if (!(ellipsisIconCollapse.is(":visible") || ellipsisIconExpand.is(":visible"))) {
                if (commentAreaContainer.hasClass("comment-area-collapsed")) {
                    ellipsisIconExpand.show();
                }
            }
        }
        if (commentsHeight === commentAreaContainerHeight) {
            if ((ellipsisIconCollapse.is(":visible") || !ellipsisIconExpand.is(":visible"))) {
                if (!commentAreaContainer.hasClass("comment-area-collapsed")) {
                    ellipsisIconCollapse.show();
                }
            }
        }
    }

/**
 * Loads contents of files with comments in a page from the server.
 * @param {JQuery} commentAreaEl Comment area element for which to construct structure
 */
    asyncConstructCommentArea(commentAreaEl: JQuery): JQueryXHR {
        const pageRowEl = commentAreaEl.parent(".page-row");
        const pageName = pageRowEl.data("page-name") as string;
        const textId = pageRowEl.data("page") as number;
        const ajax = $.post(`${this.util.getServerAddress()}Admin/ContentEditor/LoadCommentFile`,
            { textId: textId });
        ajax.done(
            (fileContent: ICommentSctucture[]) => {
                if (fileContent.length) {
                    if (fileContent !== null && typeof fileContent !== "undefined") {
                        this.parseLoadedCommentFiles(fileContent, commentAreaEl);
                    }
                } else {
                    const placeHolderText =
                        `<div class="comment-placeholder-container"><div class="comment-placeholder-text">No comments yet.</div></div>`;
                    commentAreaEl.append(placeHolderText);
                }
            });
        ajax.fail(() => {
            this.gui.showMessageDialog("Error", `Failed to construct comment area for page ${pageName}`);
        });
        return ajax;
    }

    /**
     * Parses comments to construct comment area later.
     * @param {ICommentSctucture[]} content Array of comments
     * @param {JQuery} commentAreaEl Comment area element of page comments are in
     */
    private parseLoadedCommentFiles(content: ICommentSctucture[],
        commentAreaEl: JQuery) {
        if (content !== null && typeof content !== "undefined") {
            if (content.length) {
                content.sort((a, b) => { //sort by textReferenceId, ascending
                    if (a.textReferenceId < b.textReferenceId) {
                        return -1;
                    }
                    if (a.textReferenceId === b.textReferenceId) {
                        return 0;
                    }
                    if (a.textReferenceId > b.textReferenceId) {
                        return 1;
                    }
                });

                let textReferenceId = content[0].textReferenceId;
                const indexes: number[] = [];
                for (let i = 0; i < content.length; i++) {
                    const currentTextReferenceId = content[i].textReferenceId;
                    if (currentTextReferenceId !== textReferenceId) {
                        indexes.push(i);
                        textReferenceId = currentTextReferenceId;
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
        commentAreaEl.append(html);
    }

    private processDeleteCommentClick() {
        $("#project-resource-preview").on("click", ".delete-comment", (event: JQueryEventObject) => {
            const target = $(event.target);
            const commentActionsRowEl = target.parents(".comment-actions-row");
            const commentId = parseInt(commentActionsRowEl.siblings(".media-body").attr("data-comment-id"));
            console.log(commentId);
            this.gui.createDeleteConfirmationDialog(() => {
                const deleteAjax = this.util.deleteComment(commentId);
                deleteAjax.done(() => {
                    const textId = commentActionsRowEl.parents(".page-row").data("page");
                    this.reloadCommentArea(textId);
                });
                deleteAjax.fail(() => {
                    this.gui.showMessageDialog("Fail", "Failed to delete this comment.");
                });
            });
        });
    }

    private processToggleNestedCommentClick() {
        $("#project-resource-preview").on("click",
            ".toggle-nested-comments",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                const editorPageContainer = ".pages-start";
                var target = $(event.target as HTMLElement);
                var parentComment = target.parents(".media-list");
                var commentArea = parentComment.parent(".threads-container").parent(".comment-area");
                var nestedComments = target.parent(".toggle-nested-comments-icon-container").siblings(".media")
                    .children(".media-body").children(".media");
                if (nestedComments.hasClass("nested-comment-collapsed")) {
                    const container = target.parents(".comment-area");
                    if (container.hasClass("comment-area-collapsed")) {
                        $(".nested-comment-opened").addClass("nested-comment-collapsed");
                        $(".nested-comment-opened").removeClass("nested-comment-opened");
                    }
                    nestedComments.removeClass("nested-comment-collapsed");
                    $(nestedComments).addClass("nested-comment-opened");
                } else {
                    nestedComments.removeClass("nested-comment-opened");
                    nestedComments.addClass("nested-comment-collapsed");
                    const container = target.parents(".comment-area");
                    const editorPageContainerEl = $(editorPageContainer);
                    if (container.hasClass("comment-area-collapsed")) {
                        const scrollToMainComment = $(parentComment).offset().top -
                            container.offset().top +
                            container.scrollTop();
                        if (scrollToMainComment < container.scrollTop()) {
                            container.animate({
                                scrollTop: scrollToMainComment
                            });
                        }
                    } else {
                        const scrollToMainCommentWhileExpanded = $(parentComment).offset().top -
                            editorPageContainerEl.offset().top +
                            editorPageContainerEl.scrollTop();
                        if (scrollToMainCommentWhileExpanded < $(editorPageContainer).scrollTop()) {
                            const scroll =
                            {
                                scrollTop: scrollToMainCommentWhileExpanded
                            };
                            $(editorPageContainer).animate(scroll);
                        }
                    }
                }
                this.toggleAreaSizeIconHide(commentArea);
            });
    }

    private processToggleCommentAresSizeClick() {
        $("#project-resource-preview").on("click",
            ".toggleCommentViewAreaSize",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                const target = $(event.target as HTMLElement);
                const commentArea = target.parents(".comment-area");
                const commentAreaHeight = commentArea.height();
                const threadsContainer = commentArea.children(".threads-container");
                const commentsHeight = threadsContainer.height();
                const textCenter = target.parents(".ellipsis-container");
                const compositionArea = commentArea.siblings(".composition-area");
                const compositionAreaHeight = compositionArea.height();
                commentArea.toggleClass("comment-area-collapsed");
                const collapseIconEl = textCenter.children(".ellipsis").children(".collapse-icon");
                const expandIconEl = textCenter.children(".ellipsis").children(".expand-icon");
                if (commentArea.hasClass("comment-area-collapsed")) {
                    commentArea.height(compositionAreaHeight);
                    expandIconEl.show();
                    collapseIconEl.hide();
                } else {
                    expandIconEl.hide();
                    collapseIconEl.show();
                    if (commentsHeight > commentAreaHeight) {
                        commentArea.height("auto");
                    }
                }
            });
    }


    reloadCommentArea(textId: number) {
        const commentAreaEl = $(`*[data-page="${textId}"]`).children(".comment-area");
        const threadsContainer = commentAreaEl.children(".threads-container");
        threadsContainer.remove();
        if (commentAreaEl.children(".comment-placeholder-container").length) {
            commentAreaEl.children(".comment-placeholder-container").remove();
        }
        const sectionWasCollapsed = commentAreaEl.hasClass("comment-area-collapsed");
        const threadsEl = threadsContainer.children(".media-list");
        const nestedCommentsEl = threadsEl.children(".media")
            .children(".media-body").children(".media");
        const nestedCommentsCollapsed = nestedCommentsEl.hasClass(
            "nested-comment-collapsed"); // if nested comments section was collapsed, collapse it after reload too.
        const commentAreaAjax = this.asyncConstructCommentArea(commentAreaEl);
        commentAreaAjax.done(() => {
            this.collapseIfCommentAreaIsTall(commentAreaEl, sectionWasCollapsed, nestedCommentsCollapsed);
        });
        const buttonAreaSize = $(".toggleCommentViewAreaSize");
        buttonAreaSize.off();
        const buttonNestedComments = $(".toggle-nested-comments");
        buttonNestedComments.off();
        this.init();
    }
}