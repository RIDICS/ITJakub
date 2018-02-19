/// <reference path="../../../../../node_modules/@types/codemirror/index.d.ts" />

class CommentInput {
    private readonly commentArea: CommentArea;
    private readonly util: EditorsUtil;

    constructor(commentArea: CommentArea, util: EditorsUtil) {
        this.commentArea = commentArea;
        this.util = util;
    }

    init() {
        this.processRespondToCommentClick();
        this.processEditCommentClick();
    }

    /**
* Detects buttons click and sends data to server according to ICommentStructureReply
* @param {Number} textId  - Text Id of page, where comment occured
* @param {string} textReferenceId  - Comment thread unique id to connect composition area and comment area
* @param {Number} id - Unique comment id 
* @param {Number} parentCommentId - Unique id of parent comment
* @param {JQuery} dialogEl - Dialog element to display result message about send
* @param {string} text - Comment body
*/
    processCommentSendClick(
        textId: number,
        textReferenceId: string,
        id: number,
        parentCommentId: number, commentText: string) {
        const serverAddress = this.util.getServerAddress();
        if (!commentText) {
            bootbox.alert({
                title: "Warning",
                message: "Comment is empty. Please fill it",
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
        } else {
            const comment: ICommentStructureReply = {
                id: id,
                text: commentText,
                parentCommentId: parentCommentId,
                textReferenceId: textReferenceId
            };
            const sendAjax = $.post(`${serverAddress}Admin/ContentEditor/SaveComment`,
                {
                    comment: comment,
                    textId: textId
                }
            );
            sendAjax.done(() => {
                bootbox.alert({
                    title: "Success",
                    message: "Successfully sent",
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
                const deferred = this.commentArea.reloadCommentArea(textId);
                deferred.done(() => {
                    const pageEl = $(`[data-page="${textId}"]`);
                    this.commentArea.collapseIfCommentAreaContentOverflows(pageEl.children(".comment-area"));//collapse section fully when updating section height initially
                });
            });
            sendAjax.fail(() => {
                bootbox.alert({
                    title: "Error",
                    message: "Sending failed. Server error.",
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
            });
        }
    }

    private processEditCommentClick() {
        $("#project-resource-preview").on("click", ".edit-comment", (event: JQuery.Event) => {
            const target = $(event.target as HTMLElement);
            const commentActionsRowEl = target.parents(".comment-actions-row");
            const commentBody = commentActionsRowEl.siblings(".media-body");
            const mainCommentContentEl = commentBody.parents(".media-body");
            const mainCommentLeftHeader = mainCommentContentEl.siblings(".main-comment");
            const parentCommentId = mainCommentLeftHeader.data("parent-comment-id");
            const textReferenceId = mainCommentLeftHeader.attr("id").replace("-comment", "");
            const textId = mainCommentLeftHeader.parents(".page-row").data("page");
            const commentTextEl = commentBody.children(".comment-body");
            const commentText = commentTextEl.text();
            commentActionsRowEl.hide();
            commentTextEl.hide();
            commentBody.append(`<textarea cols="40" rows="3" class="textarea-no-resize edit-comment-textarea">${commentText}</textarea>`);
            const jTextareaEl = $(".edit-comment-textarea");
            jTextareaEl.focus();
            const commentId = parseInt(commentBody.attr("data-comment-id"));
            this.processCommentReply(textId, textReferenceId, commentId, parentCommentId, jTextareaEl, commentActionsRowEl);
        });
    }

    private processRespondToCommentClick() {
        $("#project-resource-preview").on("click",
            "button.respond-to-comment",
            (event: JQuery.Event) => { // Process click on "Respond" button
                const target = $(event.target as HTMLElement);
                const pageRow =
                    target.parents(".comment-area").parent(".page-row");
                var textId = $(pageRow).data("page") as number;
                const textReferenceIdWithText = target.parent().siblings(".main-comment").attr("id");
                const parentCommentId =
                    target.parent().siblings(".main-comment").data("parent-comment-id") as number;
                const id = 0; //creating comment
                var textReferenceId = textReferenceIdWithText.replace("-comment", "");
                if (textReferenceId !== null && typeof textReferenceId !== "undefined") {
                    this.addCommentFromCommentArea(textReferenceId, textId, id, parentCommentId, target);
                } else {
                    console.log("Something is wrong. This comment doesn't have an id.");
                }
            });
    }

    toggleCommentSignsAndReturnCommentNumber(editor: SimpleMDE, addSigns: boolean): string {
        const cm = editor.codemirror as CodeMirror.Doc;
        let output = "";
        let markSize: number;
        const selectedText = cm.getSelection();
        const selectionStartChar = cm.getCursor("from").ch;
        const selectionStartLine = cm.getCursor("from").line;
        const selectionEndChar = cm.getCursor("to").ch;
        const selectionEndLine = cm.getCursor("to").line;
        const guidRegExpString =
            "([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}";
        const customCommentarySign =
            selectedText.match(
                new RegExp(
                    `\\$${guidRegExpString}\\%`)); //searching on one side only because of the same amount of characters.
        if (!addSigns) {
            if (customCommentarySign) {
                output = selectedText.replace(
                    new RegExp(`\\$${guidRegExpString}\\%`),
                    "");
                output = output.replace(
                    new RegExp(`\\%${guidRegExpString}\\$`),
                    "");
                markSize = customCommentarySign[0].length;
                cm.replaceSelection(output);
                cm.setSelection({ line: selectionStartLine, ch: selectionStartChar },
                    { line: selectionEndLine, ch: selectionEndChar - markSize });
            }
            return null;
        } else {
            const textReferenceId = this.util.createTextRefereceId();
                if (addSigns) {
                    const uniqueNumberLength = textReferenceId.length;
                    markSize = uniqueNumberLength + 2; // + $ + %
                    output = `$${textReferenceId}%${selectedText}%${textReferenceId}$`;
                    cm.replaceSelection(output);
                    cm.setSelection({ line: selectionStartLine, ch: selectionStartChar }, //setting caret
                        { line: selectionEndLine, ch: selectionEndChar + 2 * markSize });
                }
                return textReferenceId;
        }
    }

    private addCommentFromCommentArea(textReferenceId: string,
        textId: number,
        id: number,
        parentCommentId: number,
        buttonEl: JQuery) {
        const elm = `<textarea class="respond-to-comment-textarea textarea-no-resize"></textarea>`;
        buttonEl.after(elm);
        buttonEl.hide();
        const textareaEl = $(".respond-to-comment-textarea");
        textareaEl.focus();
        this.processCommentReply(textId, textReferenceId, id, parentCommentId, textareaEl, buttonEl);
    }

    private processCommentReply(
        textId: number,
        textReferenceId: string,
        commentId: number,
        parentCommentId: number,
        textAreaEl: JQuery,
        jEl: JQuery) {
        var serverAddress = this.util.getServerAddress();
        var commentTextOriginal = textAreaEl.val() as string;
        textAreaEl.on("focusout",
            (event: JQuery.Event) => {
                event.stopImmediatePropagation();
                var commentText = textAreaEl.val() as string;
                if (commentText === commentTextOriginal) {
                    jEl.show();
                    textAreaEl.remove();
                } else {
                    const comment: ICommentStructureReply = {
                        id: commentId,
                        text: commentText,
                        parentCommentId: parentCommentId,
                        textReferenceId: textReferenceId
                    };
                    if (commentId === 0) {
                        const sendAjax = $.post(`${serverAddress}Admin/ContentEditor/SaveComment`,
                            {
                                comment: comment,
                                textId: textId
                            }
                        );
                        this.onCommentSendRequest(sendAjax, textAreaEl, textId);
                    } else {
                        const sendAjax = $.post(`${serverAddress}Admin/ContentEditor/UpdateComment`,
                            {
                                comment: comment,
                                commentId: commentId
                            }
                        );
                        this.onCommentSendRequest(sendAjax, textAreaEl, textId);
                    }
                }
            });
    }

    private onCommentSendRequest(sendAjax:JQueryXHR, textAreaEl:JQuery, textId:number) {
        sendAjax.done(() => {
            bootbox.alert({
                title: "Success",
                message: "Successfully sent",
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
            textAreaEl.val("");
            textAreaEl.off();
            this.commentArea.reloadCommentArea(textId);
        });
        sendAjax.fail(() => {
            bootbox.alert({
                title: "Error",
                message: "Sending failed. Server error.",
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
        });
    }
}