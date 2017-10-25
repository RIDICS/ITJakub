class CommentInput {
    private readonly commentArea: CommentArea;
    private readonly util: Util;
    private readonly gui: TextEditorGui;

    constructor(commentArea: CommentArea, util: Util, gui: TextEditorGui) {
        this.commentArea = commentArea;
        this.util = util;
        this.gui = gui;
    }

    /**
* Detects buttons click and sends data to server according to ICommentStructureReply
* @param {boolean} nested - Whether the comment is a nested one
* @param {Number} page  - Page, where comment occured
* @param {string} commentId  - Comment id
* @param {Number} orderOfNestedComment - Order of a nested comment in a list of nested comments
* @param {Number} time - UTC UNIX time when comment was made
*/
    processCommentSendClick(
        textId: number,
        textReferenceId: string,
        id: number,
        parentCommentId: number) {
        var serverAddress = this.util.getServerAddress();
        var commentTextArea = $("#commentInput");
        const commentInputDialog = $(".comment-input-dialog");
        commentInputDialog.on("click", ".send-comment-button",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                var commentText = commentTextArea.val() as string;
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
                        commentInputDialog.dialog("close");
                        this.gui.showMessageDialog("Success", "Successfully sent");
                        commentTextArea.val("");
                        this.commentArea.reloadCommentArea(textId);
                        commentInputDialog.off();
                    });
                    sendAjax.fail(() => {
                        this.gui.showMessageDialog("Error", "Sending failed. Server error.");
                    });
                }
            });
    }

    processRespondToCommentClick() {
        $("#project-resource-preview").on("click",
            "button.respond-to-comment",
            (event: JQueryEventObject) => { // Process click on "Respond" button
                const target = $(event.target);
                const pageRow =
                    target.parents(".comment-area").parent(".page-row");
                var textId = $(pageRow).data("page") as number;
                const textReferenceIdWithText = $(target).parent().siblings(".main-comment").attr("id");
                const parentCommentId =
                    target.parent().siblings(".main-comment").data("parent-comment-id") as number;
                var textReferenceId = textReferenceIdWithText.replace("-comment", "");
                if (textReferenceId !== null && typeof textReferenceId !== "undefined") {
                    this.addCommentFromCommentArea(textReferenceId, textId, parentCommentId, target);
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
        } else {
            const ajaxTextReferenceId = this.util.createTextRefereceId();
            ajaxTextReferenceId.done((data: string) => {
                const textReferenceId = data;
                this.gui.showCommentInputDialog();
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

    private addCommentFromCommentArea(textRefernceId: string,
        textId: number,
        parentCommentId: number,
        jElement: JQuery) {
        const id = 0; //creating comment
        const elm = `<textarea class="respond-to-comment-textarea textarea-no-resize"></textarea>`;
        jElement.after(elm);
        jElement.remove();
        const textareaEl = $(".respond-to-comment-textarea");
        textareaEl.focus();
        this.processCommentReply(textId, textRefernceId, id, parentCommentId, textareaEl);
    }

    private processCommentReply(
        textId: number,
        textReferenceId: string,
        id: number,
        parentCommentId: number,
        textAreaEl: JQuery) {
        var serverAddress = this.util.getServerAddress();
        textAreaEl.on("focusout",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                var commentText = textAreaEl.val() as string;
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
                        this.gui.showMessageDialog("Success", "Successfully sent");
                        textAreaEl.val("");
                        textAreaEl.off();
                        this.commentArea.reloadCommentArea(textId);
                    });
                    sendAjax.fail(() => {
                        this.gui.showMessageDialog("Error", "Sending failed. Server error.");
                    });
                }
            });
    }
}