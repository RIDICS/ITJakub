class CommentInput {
    private readonly commentArea: CommentArea;
    private readonly util: Util;

    constructor(commentArea: CommentArea, util: Util) {
        this.commentArea = commentArea;
        this.util = util;
    }

    /**
* Detects buttons click and sends data to server in format {comment id, comment nestedness, comment page, person's name, comment body, order of nested comment, time}
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
        const buttonSend = $("#commentFinish");
        const buttonClose = $(".close-form-input");
        buttonClose.on("click",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                this.toggleCommentInputPanel();
                buttonClose.off();
            });
        buttonSend.on("click",
            (event: JQueryEventObject) => {
                event.stopImmediatePropagation();
                var commentText = commentTextArea.val() as string;
                if (commentText === "") {
                    alert("Comment is empty. Please fill it");
                } else {
                    var response = "";
                    const comment: ICommentStructureReply = {
                        id: id,
                        text: commentText,
                        parentCommentId: parentCommentId,
                        textReferenceId: textReferenceId
                    };
                    $.post(`${serverAddress}admin/project/SaveComment`,
                        {
                            comment: comment,
                            textId: textId
                        },
                        (data: string) => {
                            response = data;
                            if (response === "Written") {
                                this.toggleCommentInputPanel();
                                alert("Successfully sent");
                            } else if (response === "Error") {
                                console.log("Sent empty comment. This is not normal");
                            }
                        }
                    ).done(() => {
                        commentTextArea.val("");
                        this.commentArea.reloadCommentArea(textId);
                    });
                    buttonSend.off();
                }
            });
    }

    processRespondToCommentClick() {
        $("#project-resource-preview").on("click",
            "button.respond-to-comment",
            (event: JQueryEventObject) => { // Process click on "Respond" button
                const target = event.target as HTMLElement;
                const pageRow =
                    $(target).parents(".comment-area").parent(".page-row");
                var textId = $(pageRow).data("page") as number;
                const textReferenceIdWithText = $(target).parent().siblings(".main-comment").attr("id");
                const parentCommentId =
                    $(target).parent().siblings(".main-comment").data("parent-comment-id") as number;
                var textReferenceId = textReferenceIdWithText.replace("-comment", "");
                if (textReferenceId !== null && typeof textReferenceId !== "undefined") {
                    this.addCommentFromCommentArea(textReferenceId, textId, parentCommentId);
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
                if (addSigns) {
                    var uniqueNumberLength = textReferenceId.length;
                    markSize = uniqueNumberLength + 2; // + $ + %
                    output = `$${textReferenceId}%${selectedText}%${textReferenceId}$`;
                    cm.replaceSelection(output);
                    cm.setSelection({ line: selectionStartLine, ch: selectionStartChar }, //setting caret
                        { line: selectionEndLine, ch: selectionEndChar + 2 * markSize });
                }
            });
            return ajaxTextReferenceId;
        }
    }

    toggleCommentInputPanel() {
        const jTextInputPanel = $(".text-edit-panel");
        jTextInputPanel.toggle();
    }

    private addCommentFromCommentArea(textRefernceId: string, textId: number, parentCommentId: number) {
        const id = 0; //creating comment
        this.processCommentSendClick(textId, textRefernceId, id, parentCommentId);
        this.toggleCommentInputPanel();
    }
}