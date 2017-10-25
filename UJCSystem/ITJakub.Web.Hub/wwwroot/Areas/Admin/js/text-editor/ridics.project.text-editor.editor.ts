class Editor {
    private currentPageNumber = 0; //default initialisation
    private editingMode = false;
    private originalContent = "";
    private simplemde: SimpleMDE;
    private readonly commentInput: CommentInput;
    private readonly util: Util;
    private readonly gui: TextEditorGui;

    constructor(commentInput: CommentInput, util: Util, gui: TextEditorGui) {
        this.commentInput = commentInput;
        this.util = util;
        this.gui = gui;
    }

    private userIsEnteringText = false;

    getCurrentPageNumber() {
        return this.currentPageNumber;
    }

    getIsEditingMode() {
        return this.editingMode;
    }

    private toggleCommentFromEditor = (editor: SimpleMDE, userIsEnteringText: boolean) => {
        if (userIsEnteringText) {
            $(".preloading-pages-spinner").show();
            const textId = this.getCurrentPageNumber();
            const ajax = (this.commentInput).toggleCommentSignsAndReturnCommentNumber(editor, true);
            ajax.done((data: string) => {
                const textReferenceId = data;
                const id = 0; //creating comment
                const parentComment = 0; //creating comment
                this.commentInput.processCommentSendClick(textId, textReferenceId, id, parentComment);
                const commentInputDialogEl = $(".comment-input-dialog");
                commentInputDialogEl.on("click", ".comment-input-dialog-close-button",
                    (event: JQueryEventObject) => {
                        event.stopImmediatePropagation();
                        this.userIsEnteringText = !this.userIsEnteringText;
                        this.commentInput.toggleCommentSignsAndReturnCommentNumber(editor, false);
                        commentInputDialogEl.off();
                    });
            });
            ajax.fail(() => {
                this.gui.showMessageDialog("Error", "Comment addition failed");
            });
            ajax.always(() => {
                $(".preloading-pages-spinner").hide();
            });
        } else {
            const commentInputDialogEl = $(".comment-input-dialog");
            (this.commentInput).toggleCommentSignsAndReturnCommentNumber(editor, false);
            commentInputDialogEl.dialog("close");
        }
    }

    processAreaSwitch = () => {
        var editorExistsInTab = false;
        $("#project-resource-preview").on("click",
            ".editor",
            (e: JQueryEventObject) => { //dynamically instantiating SimpleMDE editor on textarea
                if (this.editingMode) {
                    let pageDiffers = false;
                    const jElSelected = e.target as HTMLElement;
                    const jEl = $(jElSelected).closest(".page-row");
                    const pageNumber = jEl.data("page") as number;
                    if (pageNumber !== this.currentPageNumber) {
                        pageDiffers = true;
                    }
                    const editorEl = $(".CodeMirror");
                    editorExistsInTab = (editorEl.length !== 0);
                    if (!editorExistsInTab) {
                        if (typeof this.simplemde !== "undefined" && this.simplemde !== null) {
                            this.simplemde.toTextArea();
                            this.simplemde = null;
                        }
                        this.addEditor(jEl);
                        this.originalContent = this.simplemde.value();
                    }
                    if (editorExistsInTab && pageDiffers) {
                        const contentBeforeClose = this.simplemde.value();
                        if (contentBeforeClose !== this.originalContent) {
                            const dialogEl = $("#save-confirmation-dialog");
                            const editorPageName = editorEl.parents(".page-row").data("page-name");
                            dialogEl.text(
                                `There's an open editor in page ${editorPageName
                                }. Are you sure you want to close it without saving?`);
                            this.gui.createConfirmationDialog(() => {
                                    this.simplemde.toTextArea();
                                    this.simplemde = null;
                                    this.addEditor(jEl);
                                    this.originalContent = this.simplemde.value();
                                },
                                () => {
                                    const textareaEl = jEl.find(".editor").children(".textarea-plain-text");
                                    textareaEl.trigger("blur");
                                    this.simplemde.codemirror.focus();
                                },
                                () => {
                                    this.saveContents(this.currentPageNumber, contentBeforeClose);
                                });
                        } else if (contentBeforeClose === this.originalContent) {
                            this.simplemde.toTextArea();
                            this.simplemde = null;
                            this.addEditor(jEl);
                            this.originalContent = this.simplemde.value();
                        }
                    }
                }
            });

        $("#project-resource-preview").on("click",
            ".editing-mode-button",
            () => {
                const thisClass = this;
                this.editingMode = !this.editingMode;
                if (typeof this.simplemde === "undefined" || this.simplemde === null) {
                    this.toggleDivAndTextarea();
                }
                if (typeof this.simplemde !== "undefined" && !this.editingMode && this.simplemde !== null) {
                    const contentBeforeClose = this.simplemde.value();
                    if (contentBeforeClose !== this.originalContent) {
                        const dialogEl = $("#save-confirmation-dialog");
                        const editorEl = $(".CodeMirror");
                        const editorPageName = editorEl.parents(".page-row").data("page-name");
                        dialogEl.text(
                            `There's an open editor in page ${editorPageName
                            }. Are you sure you want to close it without saving?`);
                        this.gui.createConfirmationDialog(
                            () => {
                                this.simplemde.toTextArea();
                                this.simplemde = null;
                                this.toggleDivAndTextarea();
                            },
                            () => {
                                this.editingMode = !this.editingMode; //Switch back to editing mode on cancel
                            },
                            () => {
                                this.saveContents(this.currentPageNumber, contentBeforeClose);
                                thisClass.simplemde.toTextArea();
                                thisClass.simplemde = null;
                                thisClass.toggleDivAndTextarea();
                            });
                    } else if (contentBeforeClose === this.originalContent) {
                        thisClass.simplemde.toTextArea();
                        thisClass.simplemde = null;
                        thisClass.toggleDivAndTextarea();
                    }
                }

            });
    }

    private saveContents(textId: number, contents: string) {
        const pageEl = $(`*[data-page="${textId}"]`);
        const compositionArea = pageEl.children(".composition-area");
        const id = compositionArea.data("id");
        const versionNumber = compositionArea.data("version-number");
        const request: IPageTextBase = {
            id: id,
            text: contents,
            versionNumber: versionNumber
        };
        const saveAjax = this.util.savePlainText(textId, request);
        saveAjax.done(() => {
            this.gui.showMessageDialog("Success!", "Your changes have been successfully saved.");
            this.originalContent = contents;
        });
        saveAjax.fail(() => {
            if (saveAjax.status === 409) {
                this.gui.showMessageDialog("Fail", "Failed to save your changes due to version conflict.");
            } else {
                this.gui.showMessageDialog("Fail", "There was an error while saving your changes.");
            }
        });
    }

    private addEditor(jEl: JQuery) {
        const editor = jEl.find(".editor");
        const textAreaEl = $(editor).children("textarea");
        const textId = jEl.data("page") as number;
        this.currentPageNumber = textId;
        const simpleMdeOptions: SimpleMDE.Options = {
            element: textAreaEl[0],
            autoDownloadFontAwesome: false,
            spellChecker: false,
            mode: "gfm",
            toolbar: [
                "bold", "italic", "heading", "|", "quote", "preview", {
                    name: "comment",
                    action: ((editor) => {
                        this.userIsEnteringText = !this.userIsEnteringText;
                        this.toggleCommentFromEditor(editor, this.userIsEnteringText);
                    }),
                    className: "fa fa-comment",
                    title: "Add comment"
                }, "|", {
                    name: "save",
                    action: (editor) => { this.saveContents(textId, editor.value()) },
                    className: "fa fa-floppy-o",
                    title: "Save"
                }
            ]
        };
        this.simplemde = new SimpleMDE(simpleMdeOptions);
        this.simplemde.defineMode("comment",
            () => ({
                token(stream: any) {
                    if (stream.match(
                        /(\$([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\%)/)
                    ) {
                        return "comment-start";
                    }
                    if (stream.match(
                        /(\%([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\$)/)
                    ) {
                        return "comment-end";
                    }
                    while (stream.next() != null &&
                        !stream.match(
                            /([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}/,
                            false)) {
                        return null;
                    }

                }
            }));

        this.simplemde.codemirror.addOverlay("comment");
        this.simplemde.codemirror.focus();
    }

    toggleDivAndTextarea = () => {
        var pageRow = $(".lazyloaded");
        if (this.editingMode) { // changing div to textarea here
            pageRow.each((index: number, child: Element) => {
                const pageNumber = $(child).data("page") as number;
                const compositionAreaEl = $(child).children(".composition-area");
                const placeholderSpinner = $(child).find(".loading");
                placeholderSpinner.show();
                const plainTextAjax = this.util.loadPlainText(pageNumber);
                this.createEditorAreaBody(compositionAreaEl, plainTextAjax);
            });
        } else { // changing textarea to div here
            pageRow.each((index: number, child: Element) => {
                const pageNumber = $(child).data("page") as number;
                const compositionAreaEl = $(child).children(".composition-area");
                const placeholderSpinner = $(child).find(".loading");
                placeholderSpinner.show();
                const renderedTextAjax = this.util.loadRenderedText(pageNumber);
                this.createViewerAreaBody(compositionAreaEl, renderedTextAjax);
            });
        }
    }

    private createEditorAreaBody(compositionAreaEl: JQuery, ajax: JQueryXHR) {
        const child = compositionAreaEl.children(".page");
        ajax.done((data: IPageText) => {
            const editorElement = child.children(".editor");
            if (editorElement.length) {
                editorElement.remove();
            }
            const viewerElement = child.children(".viewer");
            viewerElement.remove();
            const placeHolderSpinner = $(child).siblings(".loading");
            const plainText = data.text;
            const id = data.id;
            const versionNumber = data.versionNumber;
            compositionAreaEl.attr({ "data-id": id, "data-version-number": versionNumber });
            const elm = `<div class="editor"><textarea class="textarea-plain-text">${plainText}</textarea></div>`;
            if (this.editingMode) {
                $(child).append(elm);
            }
            placeHolderSpinner.hide();
        });
        ajax.fail(() => {
            const placeHolderSpinner = $(child).siblings(".loading");
            if (this.editingMode) {
                const elm =
                    `<div class="editor"><textarea class="textarea-plain-text">Failed to load data from server</textarea></div>`;
                $(child).append(elm);
            }
            placeHolderSpinner.hide();
        });
    }

    private createViewerAreaBody(compositionAreaEl: JQuery, ajax: JQueryXHR) {
        const child = compositionAreaEl.children(".page");
        ajax.done((data: IPageText) => {
            const viewerElement = child.children(".viewer");
            if (viewerElement.length) {
                viewerElement.remove();
            }
            const editorElement = child.children(".editor");
            editorElement.remove();
            const placeHolderSpinner = $(child).siblings(".loading");
            const renderedText = data.text;
            const id = data.id;
            const versionNumber = data.versionNumber;
            compositionAreaEl.attr({ "data-id": id, "data-version-number": versionNumber });
            const elm = `<div class="viewer">${renderedText}</div>`;
            if (!this.editingMode) {
                $(child).append(elm);
            }
            placeHolderSpinner.hide();
        });
        ajax.fail(() => {
            const placeHolderSpinner = $(child).siblings(".loading");
            if (!this.editingMode) {
                const elm = `<div class="viewer">Failed to load data from server</div>`;
                $(child).append(elm);
            }
            placeHolderSpinner.hide();
        });
    }
}