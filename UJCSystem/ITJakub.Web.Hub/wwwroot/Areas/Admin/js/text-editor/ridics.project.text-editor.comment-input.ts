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
    processCommentSendClick(nested: boolean,
        page: number,
        commentId: string,
        orderOfNestedComment: number,
        time: number) {
        var serverAddress = this.util.getServerAddress(); //TODO debug
        var commentTextArea = $("#commentInput");
        var nameTextArea = $("#nameInput");
        var surnameTextArea = $("#surnameInput");
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
                var nameText = nameTextArea.val() as string;
                var surnameText = surnameTextArea.val() as string;
                if (commentText === "" || nameText === "" || surnameText === "") {
                    alert("Comment or name is empty. Please fill both of them");
                } else {
                    var response = "";
                    const comment: ICommentSctucture = { //TODO change picture url to actual one, escape characters
                        id: commentId,
                        picture: "http://lorempixel.com/48/48",
                        nested: nested,
                        textId: page,
                        name: nameText,
                        surname: surnameText,
                        body: commentText,
                        order: orderOfNestedComment,
                        time: time
                    };
                    $.post(`${serverAddress}admin/project/SaveComment`, //check what does async affect
                        {
                            comment: comment
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
                        nameTextArea.val("");
                        surnameTextArea.val("");
                        this.commentArea.reloadCommentArea(page);
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
                var page = $(pageRow).data("page") as number;
                const uniqueIdWithText = $(target).parent().siblings(".main-comment").attr("id");
                var uniqueId = uniqueIdWithText.replace("-comment", "");
                if (uniqueId !== null && typeof uniqueId !== "undefined") {
                    const responses = $(target).siblings(".media").length;
                    this.addCommentFromCommentArea(uniqueId, page, responses + 1);
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
        const idRegExpString =
            "\\d+";
        const customCommentarySign =
            selectedText.match(
                new RegExp(
                    `\\$${idRegExpString}\\%`)); //searching on one side only because of the same amount of characters.
        if (!addSigns) {
            output = selectedText.replace(
                new RegExp(`\\$${idRegExpString}\\%`),
                "");
            output = output.replace(
                new RegExp(`\\%${idRegExpString}\\$`),
                "");
            markSize = customCommentarySign[0].length;
            cm.replaceSelection(output);
            cm.setSelection({ line: selectionStartLine, ch: selectionStartChar },
                { line: selectionEndLine, ch: selectionEndChar - markSize });
        } else {
            const ajax = this.util.getNewCommentId();
            ajax.done((data: string) => {
                const uniqueNumber = data;
                if (addSigns) {
                    var uniqueNumberLength = uniqueNumber.toString().length;
                    markSize = uniqueNumberLength + 2; // + $ + %
                    output = `$${uniqueNumber}%${selectedText}%${uniqueNumber}$`;
                    cm.replaceSelection(output);
                    cm.setSelection({ line: selectionStartLine, ch: selectionStartChar }, //setting caret
                        { line: selectionEndLine, ch: selectionEndChar + 2 * markSize });
                }
            });
            return ajax;
        }
    }

    toggleCommentInputPanel() {
        const jTextInputPanel = $(".text-edit-panel");
        jTextInputPanel.toggle();
    }

    private addCommentFromCommentArea(id: string, page: number, number: number) {
        const nested = true;
        const time = Date.now();
        this.processCommentSendClick(nested, page, id, number, time);
        this.toggleCommentInputPanel();
    }
}