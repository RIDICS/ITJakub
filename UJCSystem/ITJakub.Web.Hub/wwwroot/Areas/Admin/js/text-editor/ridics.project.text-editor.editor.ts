class Editor {
    private currentTextId = 0; //default initialisation
    private editingMode = false;
    private originalContent = "";
    private simplemde: any; //TODO temporary
    private readonly commentInput: CommentInput;
    private readonly util: EditorsUtil;
    private readonly commentArea: CommentArea;
    private commentInputDialog: BootstrapDialogWrapper;

    constructor(commentInput: CommentInput, util: EditorsUtil, commentArea: CommentArea) {
        this.commentInput = commentInput;
        this.util = util;
        this.commentArea = commentArea;

        this.commentInputDialog = new BootstrapDialogWrapper({
            element: $("#comment-input-dialog"),
            autoClearInputs: true,
            submitCallback: this.onSendButtonClick.bind(this)
        });

        bootbox.setLocale("cs"); //TODO get actual locale 
    }

    private userIsEnteringText = false;

    getCurrentTextId() {
        return this.currentTextId;
    }

    getIsEditingMode() {
        return this.editingMode;
    }

    init() {
        this.processAreaSwitch();
    }

    private onSendButtonClick(text: string) {
        const textId = this.getCurrentTextId();
        const textReferenceId = (this.commentInput).toggleCommentSignsAndReturnCommentNumber(this.simplemde, true);
        const id = 0; //creating comment
        const parentComment = null; //creating comment
        this.commentInput.processCommentSendClick(textId, textReferenceId, id, parentComment, text);
    }

    private toggleCommentFromEditor = (editor: SimpleMDE, userIsEnteringText: boolean) => {
        if (userIsEnteringText) {
            bootbox.prompt({
                title: "Please enter your comment here:",
                inputType: "textarea",
                buttons: {
                    confirm: {
                        className: "btn-default"
                    },
                    cancel: {
                        className: "btn-default",
                        callback: () => {
                        }
                    }
                },
                callback: (result) => {
                    if (result === null) {
                        this.userIsEnteringText = !this.userIsEnteringText;
                    } else {
                        this.onSendButtonClick(result);
                    }
                }
            });
        } else {
            (this.commentInput).toggleCommentSignsAndReturnCommentNumber(editor, false);
        }
    }

    private processAreaSwitch = () => {
        var editorExistsInTab = false;
        $("#project-resource-preview").on("click",
            ".editor",
            (e: JQueryEventObject) => { //dynamically instantiating SimpleMDE editor on textarea
                if (this.editingMode) {
                    let pageDiffers = false;
                    const elSelected = e.target as HTMLElement;
                    const jElSelected = $(elSelected).closest(".page-row");
                    const textId = jElSelected.data("page") as number;
                    if (textId !== this.currentTextId) {
                        pageDiffers = true;
                    }
                    const editorEl = $(".CodeMirror");
                    editorExistsInTab = (editorEl.length !== 0);
                    if (!editorExistsInTab) {
                        if (typeof this.simplemde !== "undefined" && this.simplemde !== null) {
                            this.simplemde.toTextArea();
                            this.simplemde = null;
                        }
                        this.addEditor(jElSelected);
                        this.originalContent = this.simplemde.value();
                    }
                    if (editorExistsInTab && pageDiffers) {
                        const previousPageEl = $(`*[data-page="${this.currentTextId}"]`);
                        const contentBeforeClose = this.simplemde.value();
                        if (contentBeforeClose !== this.originalContent) {
                            const editorPageName = editorEl.parents(".page-row").data("page-name");
                            bootbox.dialog({
                                title: "Warning",
                                message: `There's an open editor in page ${editorPageName
                                    }. Are you sure you want to close it without saving?`,
                                buttons: {
                                    confirm: {
                                        label: "Close",
                                        className: "btn-default",
                                        callback: () => {
                                            this.editorChangePage(previousPageEl, jElSelected);
                                        }
                                    },
                                    cancel: {
                                        label: "Cancel",
                                        className: "btn-default",
                                        callback: () => {
                                            const textareaEl = jElSelected.find(".editor")
                                                .children(".textarea-plain-text");
                                            textareaEl.trigger("blur");
                                            this.simplemde.codemirror.focus();
                                        }
                                    },
                                    save: {
                                        label: "Save and close",
                                        className: "btn-default",
                                        callback: () => {
                                            this.saveContents(this.currentTextId, contentBeforeClose);
                                        }
                                    }
                                }
                            });
                        } else if (contentBeforeClose === this.originalContent) {
                            this.editorChangePage(previousPageEl, jElSelected);
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
                        const editorEl = $(".CodeMirror");
                        const editorPageName = editorEl.parents(".page-row").data("page-name");
                        bootbox.dialog({
                            title: "Warning",
                            message: `There's an open editor in page ${editorPageName
                                }. Are you sure you want to close it without saving?`,
                            buttons: {
                                confirm: {
                                    label: "Close",
                                    className: "btn-default",
                                    callback: () => {
                                        this.simplemde.toTextArea();
                                        this.simplemde = null;
                                        this.toggleDivAndTextarea();
                                    }
                                },
                                cancel: {
                                    label: "Cancel",
                                    className: "btn-default",
                                    callback: () => {
                                        this.editingMode = !this.editingMode; //Switch back to editing mode on cancel
                                    }
                                },
                                save: {
                                    label: "Save and close",
                                    className: "btn-default",
                                    callback: () => {
                                        this.saveContents(this.currentTextId, contentBeforeClose);
                                        thisClass.simplemde.toTextArea();
                                        thisClass.simplemde = null;
                                        thisClass.toggleDivAndTextarea();
                                    }
                                }
                            }
                        });
                    } else if (contentBeforeClose === this.originalContent) {
                        thisClass.simplemde.toTextArea();
                        thisClass.simplemde = null;
                        thisClass.toggleDivAndTextarea();
                    }
                }
            });
    }

    private editorChangePage(previousPageEl: JQuery, currentPageEl: JQuery) {
        const previousPageCommentAreaEl = previousPageEl.children(".comment-area");
        this.simplemde.toTextArea();
        this.simplemde = null;
        this.commentArea.updateCommentAreaHeight(previousPageEl);
        this.commentArea.toggleAreaSizeIconHide(previousPageCommentAreaEl);
        this.addEditor(currentPageEl);
        this.originalContent = this.simplemde.value();
    }

    private saveContents(textId: number, contents: string) {
        const pageEl = $(`*[data-page="${textId}"]`);
        const compositionArea = pageEl.children(".composition-area");
        const id = compositionArea.data("id");
        const versionNumber = compositionArea.data("version-number");
        const request: ICreateTextVersion = {
            id: id,
            text: contents,
            versionNumber: versionNumber
        };
        const saveAjax = this.util.savePlainText(textId, request);
        saveAjax.done(() => {
            bootbox.alert({
                title: "Success!",
                message: "Your changes have been successfully saved.",
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
            this.originalContent = contents;
        });
        saveAjax.fail(() => {
            if (saveAjax.status === 409) {
                bootbox.alert({
                    title: "Fail",
                    message: "Failed to save your changes due to version conflict.",
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
            } else {
                bootbox.alert({
                    title: "Fail",
                    message: "There was an error while saving your changes.",
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
            }
        });
    }

    private openMarkdownHelp() {
        const url = "#"; //TODO add link to actual markdown help
        window.open(url, "_blank");
    }

    private addEditor(jEl: JQuery) {
        const editorEl = jEl.find(".editor");
        const textAreaEl = editorEl.children("textarea");
        const textId = jEl.data("page") as number;
        this.currentTextId = textId;
        const simpleMdeOptions: SimpleMDE.Options = {
            element: textAreaEl[0],
            autoDownloadFontAwesome: false,
            spellChecker: false,
            mode: "gfm",
            toolbar: [
                "bold", "italic", "|", "unordered-list", "ordered-list", "|", "heading-1", "heading-2", "heading-3",
                "|", "quote", "preview", "horizontal-rule", "|", {
                    name: "comment",
                    action: ((editor) => {
                        this.userIsEnteringText = !this.userIsEnteringText;
                        this.toggleCommentFromEditor(editor, this.userIsEnteringText);
                    }),
                    className: "fa fa-comment",
                    title: "Add comment"
                }, {
                    name: "help",
                    action: (() => { this.openMarkdownHelp(); }),
                    className: "fa fa-question-circle",
                    title: "Markdown help"
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
        this.commentArea.updateCommentAreaHeight(jEl);
        this.commentArea.toggleAreaSizeIconHide(jEl.children(".comment-area"));
    }

    toggleDivAndTextarea = () => {
        const pageRow = $(".page-row");
        const lazyloadedCompositionEl = pageRow.children(".composition-area");
        if (pageRow.hasClass("lazyloaded") && !lazyloadedCompositionEl.hasClass("lazyloaded")) {
            lazyloadedCompositionEl.addClass("lazyload");
        }
        if (lazyloadedCompositionEl.hasClass("lazyloaded")) {
            lazyloadedCompositionEl.removeClass("lazyloaded");
            lazyloadedCompositionEl.addClass("lazyload");
        }
        if (this.editingMode) { // changing div to textarea here
            pageRow.each((index: number, child: Element) => {
                const pageEl = $(child);
                const compositionAreaEl = pageEl.children(".composition-area");
                this.commentArea.updateCommentAreaHeight(pageEl);
                const placeholderSpinner = pageEl.find(".loading");
                placeholderSpinner.show();
                this.createEditorAreaBody(compositionAreaEl);
            });
        } else { // changing textarea to div here
            pageRow.each((index: number, child: Element) => {
                const pageEl = $(child);
                const compositionAreaEl = pageEl.children(".composition-area");
                this.commentArea.updateCommentAreaHeight(pageEl);
                const placeholderSpinner = pageEl.find(".loading");
                placeholderSpinner.show();
                this.createViewerAreaBody(compositionAreaEl);
            });
        }
    }

    private createEditorAreaBody(compositionAreaEl: JQuery) {
        const child = compositionAreaEl.children(".page");
        const editorElement = child.children(".editor");
        if (editorElement.length) {
            editorElement.remove();
        }
        const viewerElement = child.children(".viewer");
        viewerElement.remove();
        if (this.editingMode) {
            const elm = `<div class="editor"><textarea class="plain-text textarea-no-resize"></textarea></div>`;
            child.append(elm);
        }
    }

    private createViewerAreaBody(compositionAreaEl: JQuery) {
        const child = compositionAreaEl.children(".page");
        const viewerElement = child.children(".viewer");
        if (viewerElement.length) {
            viewerElement.remove();
        }
        const editorElement = child.children(".editor");
        editorElement.remove();
        if (!this.editingMode) {
            const elm = `<div class="viewer"><span class="rendered-text"></span></div>`;
            child.append(elm);
        }
    }
}