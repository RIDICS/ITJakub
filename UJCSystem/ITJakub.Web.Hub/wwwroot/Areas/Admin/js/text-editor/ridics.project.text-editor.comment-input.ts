class CommentInput {
    private readonly commentArea: CommentArea;
    private readonly util: EditorsApiClient;
    private readonly alertHolderSelector = ".alert-holder";

    readonly commentPattern = `komentar-`;
    readonly commentRegexExpr = `(${this.commentPattern}\\w+)`;

    constructor(commentArea: CommentArea, util: EditorsApiClient) {
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
     * @param commentText
     * @param alertHolder
     */
    processCommentSendClick(
        textId: number,
        textReferenceId: string,
        id: number,
        parentCommentId: number,
        commentText: string,
        alertHolder: JQuery<HTMLElement>) {

        alertHolder.empty();
        const comment: ICommentStructureReply = {
            id: id,
            text: commentText,
            parentCommentId: parentCommentId,
            textReferenceId: textReferenceId
        };

        this.util.createComment(textId, comment).done(() => {
            const alert = new AlertComponentBuilder(AlertType.Success)
                .addContent(localization.translate("CommentCreateSuccess", "RidicsProject").value)
                .buildElement();
            alertHolder.empty().append(alert);
            $(alert).delay(3000).fadeOut(2000);

            const deferred = this.commentArea.reloadCommentArea(textId);
            deferred.done(() => {
                const pageEl = $(`[data-text-id="${textId}"]`);
                this.commentArea.collapseIfCommentAreaContentOverflows(pageEl.children(".comment-area"));//collapse section fully when updating section height initially
            });
        }).fail(() => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate("CommentCreateFail", "RidicsProject").value)
                .buildElement();
            alertHolder.empty().append(alert);
        });
    }

    private processEditCommentClick() {
        $("#project-resource-preview").on("click", ".edit-comment, .edit-root-comment", (event) => {
            const target = $(event.target as HTMLElement);
            const commentActionsRowEl = target.parents(".comment-actions-row");

            let mainCommentContentEl: JQuery<HTMLElement>;
            let editedCommentBody: JQuery<HTMLElement>;
            editedCommentBody = commentActionsRowEl.parent(".media-body");
            if (target.hasClass("edit-root-comment")) {
                mainCommentContentEl = editedCommentBody;
            } else {
                mainCommentContentEl = editedCommentBody.parents(".media-body");
            }

            const mainCommentLeftHeader = mainCommentContentEl.siblings(".main-comment");
            const parentCommentId = mainCommentLeftHeader.data("parent-comment-id");
            const textReferenceId = mainCommentLeftHeader.data("text-reference-id");
            const textId = mainCommentLeftHeader.parents(".page-row").data("text-id");
            const commentTextEl = editedCommentBody.children(".comment-body");
            const commentText = commentTextEl.text();
            commentActionsRowEl.after(`<textarea cols="40" rows="3" class="textarea-no-resize edit-comment-textarea">${commentText}</textarea>`);
            commentActionsRowEl.hide();
            commentTextEl.hide();
            const jTextareaEl = $(".edit-comment-textarea");
            jTextareaEl.focus();
            const commentId = parseInt(editedCommentBody.attr("data-comment-id"));
            this.processCommentReply(textId, textReferenceId, commentId, parentCommentId, jTextareaEl, target);
        });
    }

    private processRespondToCommentClick() {
        $("#project-resource-preview").on("click",
            "button.respond-to-comment",
            (event) => { // Process click on "Respond" button
                const target = $(event.target as HTMLElement);
                const pageRow = target.parents(".comment-area").parent(".page-row");
                var textId = $(pageRow).data("text-id") as number;
                const textReferenceId = target.parents(".media-body").siblings(".main-comment").data("text-reference-id");
                const parentCommentId = target.parents(".media-body").siblings(".main-comment").data("parent-comment-id") as number;
                //creating comment (commentId = null)
                if (textReferenceId !== null && typeof textReferenceId !== "undefined") {
                    this.addCommentFromCommentArea(textReferenceId, textId, null, parentCommentId, target);
                } else {
                    console.log("Something is wrong. This comment doesn't have an id.");
                }
            });
    }

    toggleCommentSignsAndCreateComment(codeMirror: CodeMirror.Doc, addSigns: boolean, commentText?: string, textId?: number, saveTextFunction?: (textId: number, text: string, mode: SaveTextModeType) => JQuery.jqXHR<ISaveTextResponse>, thisForCallback?) {
        let output: string;
        let markSize: number;
        const selectedText = codeMirror.getSelection();
        const selectionStartChar = codeMirror.getCursor("from").ch;
        const selectionStartLine = codeMirror.getCursor("from").line;
        const selectionEndChar = codeMirror.getCursor("to").ch;
        const selectionEndLine = codeMirror.getCursor("to").line;
        const alertHolder = $(".CodeMirror").parents(".page-row").find(this.alertHolderSelector);
        alertHolder.empty();
        const customCommentarySign = selectedText.match(new RegExp(`\\$${this.commentRegexExpr}\\%`)); //searching on one side only because of the same amount of characters.
        if (!addSigns) {
            if (customCommentarySign) {
                output = selectedText.replace(new RegExp(`\\$${this.commentRegexExpr}\\%`), "");
                output = output.replace(new RegExp(`\\%${this.commentRegexExpr}\\$`), "");
                markSize = customCommentarySign[0].length;
                codeMirror.replaceSelection(output);
                codeMirror.setSelection({line: selectionStartLine, ch: selectionStartChar},
                    {line: selectionEndLine, ch: selectionEndChar - markSize});
            }
        } else {
            this.util.createTextReferenceId(textId).done((commentId) => {
                if (addSigns) {
                    const textReferenceId = `${this.commentPattern}${commentId}`;
                    const uniqueNumberLength = textReferenceId.length;
                    markSize = uniqueNumberLength + 2; // + $ + %
                    output = `$${textReferenceId}%${selectedText}%${textReferenceId}$`;
                    const originalText = codeMirror.getValue();
                    codeMirror.replaceSelection(output);

                    saveTextFunction.call(thisForCallback, textId, codeMirror.getValue(), SaveTextModeType.ValidateOnlySyntax).done((response: ISaveTextResponse) => {
                        if (!response.isValidationSuccess) {
                            codeMirror.setValue(originalText);
                            const alert = new AlertComponentBuilder(AlertType.Error)
                                .addContent(localization.translate("CommentSyntaxError", "RidicsProject").value)
                                .buildElement();
                            alertHolder.empty().append(alert);
                            return;
                        }

                        //creating comment (commentId = null)
                        this.processCommentSendClick(textId, textReferenceId, null, null, commentText, alertHolder);
                        codeMirror.setSelection({line: selectionStartLine, ch: selectionStartChar}, //setting caret
                            {line: selectionEndLine, ch: selectionEndChar + 2 * markSize});
                    }).fail(() => {
                        codeMirror.setValue(originalText);
                        const alert = new AlertComponentBuilder(AlertType.Error)
                            .addContent(localization.translate("CommentCreateFail", "RidicsProject").value)
                            .buildElement();
                        alertHolder.empty().append(alert);
                    });
                }
            }).fail(() => {
                const alert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(localization.translate("CommentCreateFail", "RidicsProject").value)
                    .buildElement();
                alertHolder.empty().append(alert);
            });
        }
    }

    private addCommentFromCommentArea(textReferenceId: string,
                                      textId: number,
                                      id: number,
                                      parentCommentId: number,
                                      buttonEl: JQuery) {
        const elm = this.commentArea.constructCommentInputAreaHtml();
        buttonEl.parents(".comment-actions-row").hide();
        buttonEl.parents(".media-body").append(elm);
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
        var commentTextOriginal = textAreaEl.val() as string;
        textAreaEl.on("focusout",
            (event: JQuery.Event) => {
                event.stopImmediatePropagation();
                const commentText = textAreaEl.val() as string;
                if (commentText === commentTextOriginal) {
                    const actionsRow = jEl.parents(".comment-actions-row");
                    actionsRow.show();
                    actionsRow.siblings(".comment-body").show();

                    if (commentId == null) {
                        textAreaEl.parent(".media-body").parent(".media").remove();
                    } else {
                        textAreaEl.remove();
                    }
                } else {
                    const comment: ICommentStructureReply = {
                        id: commentId,
                        text: commentText,
                        parentCommentId: parentCommentId,
                        textReferenceId: textReferenceId
                    };
                    if (commentId == null) {
                        const sendAjax = this.util.createComment(textId, comment);
                        this.onCommentSendRequest(sendAjax, textAreaEl, textId);
                    } else {
                        const sendAjax = this.util.editComment(commentId, comment);
                        this.onCommentSendRequest(sendAjax, textAreaEl, textId, true);
                    }
                }
            });
    }

    private onCommentSendRequest(sendAjax: JQueryXHR, textAreaEl: JQuery, textId: number, isEditRequest = false) {
        const alertHolder = textAreaEl.parents(".page-row").find(this.alertHolderSelector);
        alertHolder.empty();

        sendAjax.done(() => {
            const alert = new AlertComponentBuilder(AlertType.Success)
                .addContent(localization.translate(isEditRequest ? "CommentUpdateSuccess" : "CommentCreateSuccess", "RidicsProject").value)
                .buildElement();
            alertHolder.empty().append(alert);
            $(alert).delay(3000).fadeOut(2000);
            
            textAreaEl.val("");
            textAreaEl.off();
            this.commentArea.reloadCommentArea(textId);
        });
        sendAjax.fail(() => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate(isEditRequest ? "CommentUpdateFail" : "CommentCreateFail", "RidicsProject").value)
                .buildElement();
            alertHolder.empty().append(alert);
        });
    }
}