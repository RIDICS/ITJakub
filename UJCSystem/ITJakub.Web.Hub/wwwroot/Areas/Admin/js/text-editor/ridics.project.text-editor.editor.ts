class Editor {
    private currentPageNumber = 0; //default initialisation
    private editingMode = false;
    private originalContent = "";
    private simplemde: SimpleMDE;
    private readonly commentInput: CommentInput;
    private readonly util: Util;

    constructor(commentInput: CommentInput, util: Util) {
        this.commentInput = commentInput;
        this.util = util;
    }

    getCurrentPageNumber() {
        return this.currentPageNumber;
    }

    getIsEditingMode() {
        return this.editingMode;
    }

    addCommentFromEditor = (editor: SimpleMDE) => {
        const currentPageNumber = this.getCurrentPageNumber();
        const time = Date.now();
        const nested = false;
        const nestedCommentOrder = 0;
        const ajax = (this.commentInput).addCommentSignsAndReturnCommentNumber(editor);
        ajax.done((data: string) => {
            const commentId = data;
            this.commentInput.processCommentSendClick(nested,
                currentPageNumber,
                commentId,
                nestedCommentOrder,
                time);
            this.commentInput.toggleCommentInputPanel();
        });
    }

    processAreaSwitch = () => {
        var editorExistsInTab = false;

        $("#project-resource-preview").on("click",
            ".editor",
            (e: JQueryEventObject) => { //dynamically instantiating SimpleMDE editor on textarea
                if (this.editingMode) {
                    const thisClass = this;
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
                            this.createConfirmationDialog(() => {
                                    this.simplemde.toTextArea();
                                    this.simplemde = null;
                                    this.currentPageNumber = pageNumber;
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
                            thisClass.simplemde.toTextArea();
                            thisClass.simplemde = null;
                            thisClass.currentPageNumber = pageNumber;
                            thisClass.addEditor(jEl);
                            thisClass.originalContent = thisClass.simplemde.value();
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
                        this.createConfirmationDialog(
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
                        thisClass.toggleDivAndTextarea();
                    }
                }

            });
    }

    private createConfirmationDialog(onClose: Function, onCancel: Function, onSave: Function) {
        $("#save-confirmation-dialog").dialog({
            resizable: false,
            height: "auto",
            width: 400,
            modal: true,
            dialogClass: "save-confirmation-dialogue",
            buttons: [
                {
                    text: "Close without saving",
                    click: function() {
                        onClose();
                        $(this).dialog("close");
                    },
                    class: "btn btn-default save-confirmation-dialogue-button"
                },
                {
                    text: "Cancel",
                    click: function() {
                        $(this).dialog("close");
                        onCancel();
                    },
                    class: "btn btn-default save-confirmation-dialogue-button"
                }, {
                    text: "Save",
                    click: function() {
                        onSave();
                        $(this).dialog("close");
                    },
                    class: "btn btn-default save-confirmation-dialogue-button"
                }
            ]
        });
    }

    private saveContents(textId: number, contents: string) {
        console.log(textId); //TODO add logic
        console.log(contents);
        this.originalContent = contents;
    }

    private addEditor(jEl: JQuery) {
        const editor = jEl.find(".editor");
        const textAreaEl = $(editor).children("textarea");
        const textId = jEl.data("page") as number;
        const simpleMdeOptions: SimpleMDE.Options = {
            element: textAreaEl[0],
            autoDownloadFontAwesome: false,
            spellChecker: false,
            mode: "gfm",
            toolbar: [
                "bold", "italic", "heading", "|", "quote", "preview", {
                    name: "comment",
                    action: this.addCommentFromEditor,
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
                            /(([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12})/,
                            false)) {
                        return null;
                    }

                }
            }));

        this.simplemde.codemirror.addOverlay("comment");
    }

    toggleDivAndTextarea = () => {
        var pageRow = $(".lazyloaded");
        if (this.editingMode) { // changing div to textarea here
            pageRow.each((index: number, child: Element) => {
                const pageNumber = $(child).data("page") as number;
                const page = $(child).children(".composition-area").children(".page");
                const placeholderSpinner = $(child).children(".image-placeholder");
                placeholderSpinner.show();
                const plainTextAjax = this.util.loadPlainText(pageNumber);
                const viewerElement = $(page).children(".viewer");
                viewerElement.remove();
                this.createEditorAreaBody(page[0], plainTextAjax);
            });
        } else { // changing textarea to div here
            pageRow.each((index: number, child: Element) => {
                const pageNumber = $(child).data("page") as number;
                const page = $(child).children(".composition-area").children(".page");
                const placeholderSpinner = $(child).children(".image-placeholder");
                placeholderSpinner.show();
                const renderedTextAjax = this.util.loadRenderedText(pageNumber);
                const editorElement = $(page).children(".editor");
                editorElement.remove();
                this.createViewerAreaBody(page[0], renderedTextAjax);
            });
        }
    }

    private createEditorAreaBody(child: Element, ajax: JQueryXHR) {
        ajax.done((data: string) => {
            const placeHolderSpinner = $(child).parent(".composition-area").siblings(".image-placeholder");
            const plainText = data;
            const elm = `<div class="editor"><textarea class="textarea-plain-text">${plainText}</textarea></div>`;
            $(child).append(elm);
            placeHolderSpinner.hide();
        });
    }

    private createViewerAreaBody(child: Element, ajax: JQueryXHR) {
        ajax.done((data: IPageText) => {
            const placeHolderSpinner = $(child).parent(".composition-area").siblings(".image-placeholder");
            const renderedText = data.text;
            const elm = `<div class="viewer">${renderedText}</div>`;
            $(child).append(elm);
            placeHolderSpinner.hide();
        });
    }
}