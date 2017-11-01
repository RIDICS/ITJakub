class CommentInput {
    private readonly commentArea: CommentArea;
    private readonly util: Util;
    private readonly gui: TextEditorGui;

    constructor(commentArea: CommentArea, util: Util, gui: TextEditorGui) {
        this.commentArea = commentArea;
        this.util = util;
        this.gui = gui;
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
*/
    processCommentSendClick(
        textId: number,
        textReferenceId: string,
        id: number,
        parentCommentId: number,
        dialogEl: JQuery) {
        const serverAddress = this.util.getServerAddress();
        var commentTextArea = $("#commentInput");
        const commentText = commentTextArea.val() as string;
        if (commentText === "") {
            this.gui.showMessageDialog("Warning", "Comment is empty. Please fill it");
        } else {
            const comment: ICommentStructureReply = {
                id: id,
                text: commentText,
                parentCommentId: parentCommentId,
                textReferenceId: textReferenceId
            };
            const sendAjax = $.post(`${serverAddress}admin/project/SaveComment`,
                {
                    comment: comment,
                    textId: textId
                }
            );
            sendAjax.done(() => {
                dialogEl.dialog("close");
                this.gui.showMessageDialog("Success", "Successfully sent");
                commentTextArea.val("");
                this.commentArea.reloadCommentArea(textId);
            });
            sendAjax.fail(() => {
                this.gui.showMessageDialog("Error", "Sending failed. Server error.");
            });
        }
    }

    private processEditCommentClick() {
        $("#project-resource-preview").on("click", ".edit-comment", (event: JQueryEventObject) => {
            const target = $(event.target);
            target.hide();
            const commentActionsRowEl = target.parents(".comment-actions-row");
            const commentBody = commentActionsRowEl.siblings(".media-body");
            const mainCommentContentEl = commentBody.parents(".media-body");
            const mainCommentLeftHeader = mainCommentContentEl.siblings(".main-comment");
            const parentCommentId = mainCommentLeftHeader.data("parent-comment-id");
            const textReferenceId = mainCommentLeftHeader.attr("id").replace("-comment", "");
            const textId = mainCommentLeftHeader.parents(".page-row").data("page");
            const commentTextEl = commentBody.children(".comment-body");
            const commentText = commentTextEl.text();
            commentTextEl.hide();
            commentBody.append(`<textarea cols="40" rows="3" class="textarea-no-resize edit-comment-textarea">${commentText}</textarea>`);
            const jTextareaEl = $(".edit-comment-textarea");
            jTextareaEl.focus();
            const commentId = parseInt(commentBody.attr("data-comment-id"));
            this.processCommentReply(textId, textReferenceId, commentId, parentCommentId, jTextareaEl, target);
        });
    }

    private processRespondToCommentClick() {
        $("#project-resource-preview").on("click",
            "button.respond-to-comment",
            (event: JQueryEventObject) => { // Process click on "Respond" button
                const target = $(event.target);
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

    toggleCommentSignsAndReturnCommentNumber(editor: SimpleMDE, addSigns: boolean): JQueryXHR {
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
            return null;
        } else {
            const ajaxTextReferenceId = this.util.createTextRefereceId();
            ajaxTextReferenceId.done((data: string) => {
                const textReferenceId = data;
                if (addSigns) {
                    const uniqueNumberLength = textReferenceId.length;
                    markSize = uniqueNumberLength + 2; // + $ + %
                    output = `$${textReferenceId}%${selectedText}%${textReferenceId}$`;
                    cm.replaceSelection(output);
                    cm.setSelection({ line: selectionStartLine, ch: selectionStartChar }, //setting caret
                        { line: selectionEndLine, ch: selectionEndChar + 2 * markSize });
                }
            });
            ajaxTextReferenceId.fail(() => {
                this.gui.showMessageDialog("Error", "Comment creation failed");
            });
            return ajaxTextReferenceId;
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
        id: number,
        parentCommentId: number,
        textAreaEl: JQuery,
        jEl: JQuery) {
        var serverAddress = this.util.getServerAddress();
        var commentTextOriginal = textAreaEl.val() as string;
        textAreaEl.on("focusout",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                var commentText = textAreaEl.val() as string;
                if (commentText === commentTextOriginal) {
                    jEl.show();
                    textAreaEl.detach();
                } else {
                    const comment: ICommentStructureReply = {
                        id: id,
                        text: commentText,
                        parentCommentId: parentCommentId,
                        textReferenceId: textReferenceId
                    };
                    if (id === 0) {
                        const sendAjax = $.post(`${serverAddress}admin/project/SaveComment`,
                            {
                                comment: comment,
                                textId: textId
                            }
                        );
                        this.onCommentSendRequest(sendAjax, textAreaEl, textId);
                    } else {
                        const sendAjax = $.post(`${serverAddress}admin/project/UpdateComment`,
                            {
                                comment: comment,
                                textId: textId
                            }
                        );
                        this.onCommentSendRequest(sendAjax, textAreaEl, textId);
                    }
                }
            });
    }

    private onCommentSendRequest(sendAjax:JQueryXHR, textAreaEl:JQuery, textId:number) {
        sendAjax.done(() => {
            this.gui.showMessageDialog("Success", "Successfully sent");
            textAreaEl.val("");
            textAreaEl.off();
            this.commentArea.reloadCommentArea(textId);
        });
        sendAjax.fail(() => {
            this.gui.showMessageDialog("Error", "Sending failed. Server error.");
        });
    }
}