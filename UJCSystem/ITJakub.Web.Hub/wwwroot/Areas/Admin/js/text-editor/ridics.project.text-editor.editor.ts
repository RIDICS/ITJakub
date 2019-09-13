class Editor {
    private currentTextId = 0; //default initialisation
    private editingMode = false;
    private originalContent = "";
    private simplemde: IExtendedSimpleMDE;
    private readonly commentInput: CommentInput;
    private readonly util: EditorsUtil;
    private readonly commentArea: CommentArea;
    private commentInputDialog: BootstrapDialogWrapper;
    private isPreviewRendering = false;

    private commentIdPattern =
        "([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}";

    constructor(commentInput: CommentInput, util: EditorsUtil, commentArea: CommentArea) {
        this.commentInput = commentInput;
        this.util = util;
        this.commentArea = commentArea;

        this.commentInputDialog = new BootstrapDialogWrapper({
            element: $("#comment-input-dialog"),
            autoClearInputs: true,
            submitCallback: this.onSendButtonClick.bind(this)
        });

        bootbox.setLocale("cs");
    }

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
        const cm = this.simplemde.codemirror as CodeMirror.Doc;
        const textReferenceId = (this.commentInput).toggleCommentSignsAndReturnCommentNumber(cm, true);
        const id = 0; //creating comment
        const parentComment = null; //creating comment
        this.commentInput.processCommentSendClick(textId, textReferenceId, id, parentComment, text);
    }

    private toggleCommentFromEditor = (editor: SimpleMDE, userIsEnteringText: boolean) => {
        const cm = editor.codemirror as CodeMirror.Doc;
        if (userIsEnteringText) {
            const commentText = cm.getSelection();
            const commentBeginRegex = new RegExp(`(\\$${this.commentIdPattern}\\%)`);
            const commentEndRegex = new RegExp(`(\\%${this.commentIdPattern}\\$)`);
            if (commentBeginRegex.test(commentText) || commentEndRegex.test(commentText)) {
                bootbox.alert({
                    title: localization.translate("Warning", "RidicsProject").value,
                    message: localization.translate("OverlappingComment", "RidicsProject").value,
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
                return;
            }
            bootbox.prompt({
                title: localization.translate("EnterComment", "RidicsProject").value,
                inputType: "textarea",
                buttons: {
                    confirm: {
                        className: "btn-default"
                    },
                    cancel: {
                        className: "btn-default",
                        callback: () => {
                            this.toggleCommentFromEditor(editor, false);
                        }
                    }
                },
                callback: (result) => {
                    if (!result) {
                        this.toggleCommentFromEditor(editor, false);
                    } else {
                        this.onSendButtonClick(result);
                    }
                }
            });
        } else {
            (this.commentInput).toggleCommentSignsAndReturnCommentNumber(cm, false);
        }
    }

    private processAreaSwitch = () => {
        var editorExistsInTab = false;

        $(".page-toolbar .edit-page").click((event) => {
            const button = $(event.currentTarget);
            button.addClass("hide");
            const pageRow = button.parents(".page-row");
            this.editingMode = true;
            this.togglePageRows(pageRow);
          //  pageRow.find(".editor").click();
        });

        $("#project-resource-preview").on("click", ".editor", (e) => { //dynamically instantiating SimpleMDE editor on textarea
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
                            title: localization.translate("Warning", "RidicsProject").value,
                            message: localization.translateFormat("CloseEditedPage", [editorPageName], "RidicsProject").value, 
                            buttons: {
                                confirm: {
                                    label: localization.translate("CloseWithoutSaving", "RidicsProject").value,
                                    className: "btn-default",
                                    callback: () => {
                                        this.editorChangePage(previousPageEl, jElSelected);
                                    }
                                },
                                cancel: {
                                    label: localization.translate("Cancel", "RidicsProject").value,
                                    className: "btn-default",
                                    callback: () => {
                                        const textareaEl = jElSelected.find(".editor")
                                            .children(".textarea-plain-text");
                                        textareaEl.trigger("blur");
                                        this.simplemde.codemirror.focus();
                                    }
                                },
                                save: {
                                    label: localization.translate("SaveAndClose", "RidicsProject").value,
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

        $("#project-resource-preview").on("click", ".editing-mode-button", () => {
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
                        title: localization.translate("Warning", "RidicsProject").value,
                        message: localization.translateFormat("CloseEditedPage", [editorPageName], "RidicsProject").value, 
                        buttons: {
                            confirm: {
                                label: localization.translate("CloseWithoutSaving", "RidicsProject").value,
                                className: "btn-default",
                                callback: () => {
                                    this.simplemde.toTextArea();
                                    this.simplemde = null;
                                    this.toggleDivAndTextarea();
                                }
                            },
                            cancel: {
                                label: localization.translate("Cancel", "RidicsProject").value,
                                className: "btn-default",
                                callback: () => {
                                    this.editingMode = !this.editingMode; //Switch back to editing mode on cancel
                                }
                            },
                            save: {
                                label: localization.translate("SaveAndClose", "RidicsProject").value,
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
                title: localization.translate("Success", "RidicsProject").value,
                message: localization.translate("ChangesSaveSuccess", "RidicsProject").value,
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
                    title: localization.translate("Fail", "RidicsProject").value,
                    message: localization.translate("ChangesSaveConflict", "RidicsProject").value,
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
            } else {
                bootbox.alert({
                    title: localization.translate("Fail", "RidicsProject").value,
                    message: localization.translate("ChangesSaveFail", "RidicsProject").value,
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
        const url = "#";
        window.open(url, "_blank");
    }

    private addEditor(jEl: JQuery) {
        const editorEl = jEl.find(".editor");
        const textAreaEl = editorEl.children("textarea");
        const textId = parseInt(jEl.data("page") as string);
        this.currentTextId = textId;
        const simpleMdeOptions: SimpleMDE.Options = {
            element: textAreaEl[0],
            autoDownloadFontAwesome: false,
            spellChecker: false,
            mode: "gfm",
            toolbar: [ //TODO use SimpleMdeTools
                {
                    name: "save",
                    action: (editor) => { this.saveContents(textId, editor.value()) },
                    className: "fa fa-floppy-o",
                    title: "Save"
                }, "|", "bold", "italic", "|", "unordered-list", "ordered-list", "|", "heading-1", "heading-2", "heading-3",
                "|", "quote",
                {
                    name: "preview",
                    action: (editor: SimpleMDE) => {
                        simpleMdeOptions.previewRender = (plainText: string, preview: HTMLElement) => {
                            this.previewRemoteRender(plainText, preview);
                            return "<div class=\"loading\"></div>";
                        };
                        SimpleMDE.togglePreview(editor);
                    },
                    className: "fa fa-eye no-disable",
                    title: localization.translate("TogglePreview", "ItJakubJs").value
                },
                "horizontal-rule", "|", {
                    name: "comment",
                    action: ((editor) => {
                        this.toggleCommentFromEditor(editor, true);
                    }),
                    className: "fa fa-comment",
                    title: "Add comment"
                }
                //{Temporarily hide while there is no markdown manual yet
                //    name: "help",
                //    action: (() => { this.openMarkdownHelp(); }),
                //    className: "fa fa-question-circle",
                //    title: "Markdown help"
                //},
               
            ]
        };

        this.simplemde = new SimpleMDE(simpleMdeOptions);
       
        const commentIdRegex = new RegExp(`${this.commentIdPattern}`);
        const commentBeginRegex = new RegExp(`(\\$${this.commentIdPattern}\\%)`);
        const commentEndRegex = new RegExp(`(\\%${this.commentIdPattern}\\$)`);
        this.simplemde.defineMode("comment",
            () => ({
                token(stream: CodeMirror.StringStream) {
                    if (stream.match(commentBeginRegex)) {
                        return "comment-start";
                    }
                    if (stream.match(commentEndRegex)) {
                        return "comment-end";
                    }
                    while (stream.next() != null &&
                        !stream.match(commentIdRegex, false)) {
                        return null;
                    }

                }
            }));

        this.simplemde.codemirror.addOverlay("comment");
        this.simplemde.codemirror.focus();
        this.commentArea.updateCommentAreaHeight(jEl);
        this.commentArea.toggleAreaSizeIconHide(jEl.children(".comment-area"));

       
    }

    private previewRemoteRender(text: string, previewElement: HTMLElement) {
        if (this.isPreviewRendering) {
            return;
        }

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Text/RenderPreview",
            data: JSON.stringify({
                text: text,
                inputTextFormat: "markdown"
            }),
            dataType: "json",
            contentType: "application/json",
            success: (generatedHtml) => {
                previewElement.innerHTML = generatedHtml;
                this.isPreviewRendering = false;
            },
            error: () => {
                previewElement.innerHTML = "<div>" + localization.translate("RenderError", "ItJakubJs").value + "</div>";
                this.isPreviewRendering = false;
            }
        });
    }

    toggleDivAndTextarea = () => {
        this.togglePageRows($(".page-row"));
    }

    togglePageRows = (pageRow: JQuery) => {
        const lazyloadedCompositionEl = pageRow.children(".composition-area");
        if (pageRow.hasClass("lazyloaded") && !lazyloadedCompositionEl.hasClass("lazyloaded")) {
            lazyloadedCompositionEl.addClass("lazyload");
        }
        if (lazyloadedCompositionEl.hasClass("lazyloaded")) {
            lazyloadedCompositionEl.removeClass("lazyloaded");
            lazyloadedCompositionEl.addClass("lazyload");
        }
        if (this.editingMode) { // changing div to textarea here
            pageRow.each((index, child) => {
                const pageEl = $(child as Node as HTMLElement);
                const compositionAreaEl = pageEl.children(".composition-area");
                this.commentArea.updateCommentAreaHeight(pageEl);
                const placeholderSpinner = pageEl.find(".loading");
                placeholderSpinner.show();
                this.createEditorAreaBody(compositionAreaEl);
            });
        } else { // changing textarea to div here
            pageRow.each((index, child) => {
                const pageEl = $(child as Node as HTMLElement);
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