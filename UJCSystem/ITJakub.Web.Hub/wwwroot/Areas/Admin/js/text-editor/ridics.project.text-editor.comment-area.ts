﻿class CommentArea {
    private readonly util: EditorsApiClient;
    private readonly adminApiClient = new AdminApiClient();
    private editor: Editor;

    constructor(util: EditorsApiClient) {
        this.util = util;
    }

    init() {
        this.processToggleCommentAresSizeClick();
        this.processToggleNestedCommentClick();
    }

    initCommentsDeleting(editor: Editor) {
        this.editor = editor;
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
        const textId = commentAreaEl.parents(".page-row").data("text-id") as number;
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
            let commentImage;
            if (picture == null) {
                commentImage =
                    `<div><i class="fa fa-4x fa-user media-object"></i></div>`;
            } else {
                commentImage =
                    `<a href="#"><img alt="48x48" class="media-object" src="${picture
                        }" style="width: 48px; height: 48px;"></a>`;
            }

            var mainCommentLeftPartStart =
                `<div class="media-left main-comment" data-text-reference-id="${textReferenceId}" data-parent-comment-id="${id}">`;
            var commentName = `<h5 class="media-heading">${name} ${surname}</h5>`;
            var mainCommentBody =
                `<p class="comment-body">${body}</p>
                <div class="comment-actions-row">
                    <div class="btn-group">
                        <button class="respond-to-comment">${localization.translate("Respond", "RidicsProject").value}</button>
                        <button type="button" class="edit-root-comment">${localization.translate("Edit", "RidicsProject").value}</button>
                        <button type="button" class="delete-root-comment">${localization.translate("Delete", "RidicsProject").value}</button>
                    </div>
                </div>`;

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
                    areaContent += `<div class="text-center id-in-comment-area text-muted">${localization.translate("Commentary", "RidicsProject").value} ${textReferenceId
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
                    areaContent += `<div class="row comment-actions-row">
                                        <div class="col-xs-8"></div>
                                        <div class="col-xs-4">
                                            <div class="btn-group">
                                                <button type="button" class="edit-comment">${localization.translate("Edit", "RidicsProject").value}</button>
                                                <button type="button" class="delete-comment">${localization.translate("Delete", "RidicsProject").value}</button>
                                            </div>
                                        </div>
                                    </div>`;
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
            `<i class="fa fa-expand expand-icon fa-lg" aria-hidden="true" title="${localization.translate("ExpandCommentArea", "RidicsProject").value}"></i>`;
        const ellipsisIconCollapse =
            `<i class="fa fa-compress collapse-icon fa-lg" aria-hidden="true" title="${localization.translate("CollapseCommentArea", "RidicsProject").value}"></i>`;
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

    collapseIfCommentAreaContentOverflows = (
        commentAreaEl: JQuery) => {
        var areaContent = "";
        const ellipsisStart = "<div class=\"ellipsis-container\">";
        const ellipsisBodyStart = "<div class=\"ellipsis toggleCommentViewAreaSize\">";
        const ellipsisIconExpand =
            `<i class="fa fa-expand expand-icon fa-lg" aria-hidden="true" title="${localization.translate("ExpandCommentArea", "RidicsProject").value}"></i>`;
        const ellipsisIconCollapse =
            `<i class="fa fa-compress collapse-icon fa-lg" aria-hidden="true" title="${localization.translate("CollapseCommentArea", "RidicsProject").value}"></i>`;
        const ellipsisBodyEnd = "</div>";
        const ellipsisEnd = "</div>";
        areaContent += ellipsisStart;
        areaContent += ellipsisBodyStart;
        areaContent += ellipsisIconExpand;
        areaContent += ellipsisIconCollapse;
        areaContent += ellipsisBodyEnd;
        areaContent += ellipsisEnd;
        const html = $.parseHTML(areaContent);
        const commentAreaHeight = commentAreaEl.height();
        const threadsContainer = commentAreaEl.children(".threads-container");
        const threadsHeight = threadsContainer.height();
        if (threadsHeight > commentAreaHeight
        ) {
                commentAreaEl.addClass("comment-area-collapsed");
                threadsContainer.height(commentAreaHeight);
                this.collapseNestedComments(commentAreaEl);
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
                                           <i class="fa fa-bars fa-lg toggle-nested-comments" aria-hidden="true" title="${localization.translate("CollapseCommentArea", "RidicsProject").value}"></i>
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
    asyncConstructCommentArea(commentAreaEl: JQuery): JQuery.Promise<ICommentSctucture[]> {
        var deferredResult = $.Deferred<ICommentSctucture[]>();
        const pageRowEl = commentAreaEl.parent(".page-row");
        const pageName = pageRowEl.data("page-name") as string;
        const textId = pageRowEl.data("text-id") as number;
        commentAreaEl.empty();
        if (!textId) { // textId doesn't have to be specified (text is not created yet)
            const alert = new AlertComponentBuilder(AlertType.Info).addContent(localization.translate("CommentNotLoadedMissingText", "RidicsProject").value);
            commentAreaEl.append(alert.buildElement());
            deferredResult.reject();
            return deferredResult.promise();
        }

        pageRowEl.removeClass("comment-never-loaded");
        const ajax = this.adminApiClient.loadCommentFile(textId);
        ajax.done(
            (fileContent: ICommentSctucture[]) => {
                if (fileContent.length) {
                    if (fileContent !== null && typeof fileContent !== "undefined") {
                        this.parseLoadedCommentFiles(fileContent, commentAreaEl);
                    }
                } else {
                    const placeHolderText =
                        `<div class="comment-placeholder-container"><div class="comment-placeholder-text">${localization.translate("NoComments", "RidicsProject").value}</div></div>`;
                    commentAreaEl.append(placeHolderText);
                }
                deferredResult.resolve(fileContent);
            });
        ajax.fail(() => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translateFormat("LoadCommentsFailed", [pageName], "RidicsProject").value).buildElement();
            commentAreaEl.append(alert);
            deferredResult.reject();
        });
        return deferredResult.promise();
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
                this.processDeleteCommentClick(commentAreaEl);
            }
        }
    }

    private constructCommentArea(content: ICommentSctucture[], commentAreaEl: JQuery) {
        const html = this.constructCommentAreaHtml(content, commentAreaEl);
        commentAreaEl.append(html);
    }

    private processDeleteCommentClick(commentAreaEl: JQuery) {
        commentAreaEl.find(".delete-comment, .delete-root-comment").on("click", (event) => {
            const target = $(event.target as Node as HTMLElement);
            const isEditingModeEnabled = target.parents(".page-row").find(".viewer").length === 0;
            const commentActionsRowEl = target.parents(".comment-actions-row");

            let confirmMessage = localization.translate("DeleteCommentConfirm", "RidicsProject").value;
            let commentId: number;

            if (target.hasClass("delete-root-comment")) {
                commentId = parseInt(commentActionsRowEl.parents(".media-body").attr("data-comment-id"));

                if (isEditingModeEnabled) {
                    confirmMessage = `<div class="alert alert-warning">
                                        <i class="fa fa-warning"></i> ${localization.translate("DeleteCommentWarning", "RidicsProject").value}
                                    </div>
                                    <div class="bootbox-body">
                                       ${localization.translate("DeleteCommentConfirm", "RidicsProject").value}
                                    </div>`;
                }
            } else {
                commentId = parseInt(commentActionsRowEl.siblings(".media-body").attr("data-comment-id"));
            }

            bootbox.confirm({
                title: localization.translate("ConfirmTitle", "RidicsProject").value,
                message: confirmMessage,
                buttons: {
                    confirm: {
                        className: "btn-default"
                    },
                    cancel: {
                        className: "btn-default"
                    }
                },
                callback: (result) => {
                    if (!result) {
                        return;
                    }

                    // delete child comment:
                    if (!target.hasClass("delete-root-comment")) {
                        const deleteAjax = this.util.deleteComment(commentId);
                        this.onCommentDeleteRequest(deleteAjax, target);
                        return;
                    }

                    // delete root comment in reading mode:
                    const pageRow = target.parents(".page-row");
                    if (!isEditingModeEnabled) {
                        const deleteAjax = this.util.deleteRootComment(commentId);
                        this.onCommentDeleteRequest(deleteAjax, target);
                        return;
                    }

                    // delete root comment in editing mode:
                    const codeMirror = this.editor.getSimpleMdeEditor().codemirror;
                    const originalText = codeMirror.getValue();
                    const textId = Number(pageRow.data("text-id"));

                    this.editor.saveText(textId, originalText, SaveTextModeType.ValidateOnlySyntax).done(
                        (response: ISaveTextResponse) => {
                            if (!response.isValidationSuccess) {
                                codeMirror.setValue(originalText);
                                bootbox.alert({
                                    title: localization.translate("Fail", "RidicsProject").value,
                                    message: localization.translate("CommentSyntaxError", "RidicsProject")
                                        .value,
                                    buttons: {
                                        ok: {
                                            className: "btn-default"
                                        }
                                    }
                                });
                                return;
                            }

                            const deleteAjax = this.util.deleteRootComment(commentId).done(() => {
                                this.editor.reloadCurrentEditorArea();
                            });
                            this.onCommentDeleteRequest(deleteAjax, target);
                        });
                }
            });
        });
    }

    private onCommentDeleteRequest(deleteAjax: JQueryXHR, targetButton: JQuery) {
        deleteAjax.done(() => {
            const textId = targetButton.parents(".page-row").data("text-id");
            bootbox.alert({
                title: localization.translate("Success", "RidicsProject").value,
                message: localization.translate("CommentDeleteSuccess", "RidicsProject").value,
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
            this.reloadCommentArea(textId);
        });
        deleteAjax.fail(() => {
            bootbox.alert({
                title: localization.translate("Fail", "RidicsProject").value,
                message: localization.translate("CommentDeleteFail", "RidicsProject").value,
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
        });
    }

    private processToggleNestedCommentClick() {
        $("#project-resource-preview").on("click",
            ".toggle-nested-comments",
            (event) => {
                event.stopImmediatePropagation();
                const editorPageContainer = ".pages-start";
                var target = $(event.target as Node as HTMLElement);
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
                            } as JQuery.PlainObject);
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
            (event) => {
                event.stopImmediatePropagation();
                const target = $(event.target as Node as HTMLElement);
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
        var deferred = $.Deferred();
        const commentAreaEl = $(`*[data-text-id="${textId}"]`).children(".comment-area");
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
            deferred.resolve();
        });
        commentAreaAjax.fail(() => {
            deferred.reject();
        });
        const buttonAreaSize = $(".toggleCommentViewAreaSize");
        buttonAreaSize.off();
        const buttonNestedComments = $(".toggle-nested-comments");
        buttonNestedComments.off();
        this.init();
        return deferred;
    }
}