class Editor {
    private currentTextId = 0; //default initialisation
    private originalContent = "";
    private simplemde: IExtendedSimpleMDE;
    private simpleMdeOptions: SimpleMDE.Options;
    private readonly commentInput: CommentInput;
    private readonly util: EditorsApiClient;
    private readonly commentArea: CommentArea;
    private readonly editModeSelector = "is-edited";
    private readonly client: TextApiClient;
    private readonly simpleMdeTools: SimpleMdeTools;
    private isPreviewRendering = true;
    private editorExistsInTab = false;

    constructor(commentInput: CommentInput, util: EditorsApiClient, commentArea: CommentArea) {
        this.commentInput = commentInput;
        this.util = util;
        this.commentArea = commentArea;
        this.client = new TextApiClient();
        this.simpleMdeTools = new SimpleMdeTools();

        bootbox.setLocale("cs");
    }

    getCurrentTextId() {
        return this.currentTextId;
    }

    getEditModeSelector() {
        return this.editModeSelector;
    }

    init() {
        this.processAreaSwitch();
    }

    private processAreaSwitch () {
        this.editorExistsInTab = false;

        $(".page-toolbar .edit-page").click((event) => {
            const pageRow = $(event.currentTarget).parents(".page-row");
            pageRow.addClass("init-editor");
            pageRow.data(this.editModeSelector, true);
            this.changeOrInitEditor(pageRow);
        });

        $("#project-resource-preview").on("click", ".editor", (e) => { //dynamically instantiating SimpleMDE editor on textarea
            const elSelected = e.target as HTMLElement;
            const selectedPageRow = $(elSelected).closest(".page-row");
            this.changeOrInitEditor(selectedPageRow);
        });
    }

    private onSendCommentButtonClick(text: string) {
        const textId = this.getCurrentTextId();
        const cm = this.simplemde.codemirror as CodeMirror.Doc;
        this.commentInput.toggleCommentSignsAndCreateComment(cm, true, text, textId, this.saveText, this);
    }

    private toggleCommentFromEditor = (editor: SimpleMDE, userIsEnteringText: boolean) => {
        const cm = editor.codemirror as CodeMirror.Doc;
        if (userIsEnteringText) {
            const commentText = cm.getSelection();
            const commentBeginRegex = new RegExp(`(\\$${this.commentInput.commentRegexExpr}\\%)`);
            const commentEndRegex = new RegExp(`(\\%${this.commentInput.commentRegexExpr}\\$)`);
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
            bootbox.dialog({
                title: localization.translate("EnterComment", "RidicsProject").value,
                message: `<div class="alert alert-warning">
                                <i class="fa fa-warning"></i> ${localization.translate("CreateCommentWarning", "RidicsProject").value}
                            </div>
                            <div class="bootbox-body">
                                <form class="bootbox-form">
                                    <textarea id="commentInput" class="bootbox-input bootbox-input-textarea form-control" spellcheck="false"></textarea>
                                </form>
                            </div>`, 
                buttons: {
                    cancel: {
                        label: localization.translate("Cancel", "RidicsProject").value,
                        className: "btn-default",
                        callback: () => {
                            this.toggleCommentFromEditor(editor, false);
                        }
                    },
                    confirm: {
                        label: localization.translate("AddComment", "RidicsProject").value,
                        className: "btn-default",
                        callback: () => {
                            const result = String($("#commentInput").val());
                            if (!result) {
                                this.toggleCommentFromEditor(editor, false);
                            } else {
                                this.onSendCommentButtonClick(result);
                            }
                        }
                    }
                }
            });
        } else {
            (this.commentInput).toggleCommentSignsAndCreateComment(cm, false);
        }
    }

    changeOrInitEditor(selectedPageRow: JQuery) {
        if (selectedPageRow.data(this.editModeSelector)) {
            let pageDiffers = false;
            const textId = selectedPageRow.data("text-id") as number;
            if (textId !== this.currentTextId) {
                pageDiffers = true;
            }
            const editorEl = $(".CodeMirror");
            this.editorExistsInTab = (editorEl.length !== 0);
            if (!this.editorExistsInTab) {
                if (typeof this.simplemde !== "undefined" && this.simplemde !== null) {
                    this.simplemde.toTextArea();
                    this.simplemde = null;
                }
                this.togglePageRows(selectedPageRow);
            }
            if (this.editorExistsInTab && pageDiffers) {
                const previousPageEl = $(`*[data-text-id="${this.currentTextId}"]`);
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
                                    this.editorChangePage(previousPageEl, selectedPageRow);
                                }
                            },
                            cancel: {
                                label: localization.translate("Cancel", "RidicsProject").value,
                                className: "btn-default",
                                callback: () => {
                                    const textareaEl = selectedPageRow.find(".editor")
                                        .children(".textarea-plain-text");
                                    textareaEl.trigger("blur");
                                    this.simplemde.codemirror.focus();
                                }
                            },
                            save: {
                                label: localization.translate("SaveAndClose", "RidicsProject").value,
                                className: "btn-default",
                                callback: () => {
                                    this.saveContents(this.currentTextId, contentBeforeClose, SaveTextModeType.FullValidateOrDeny, () => {
                                        this.editorChangePage(previousPageEl, selectedPageRow);
                                    });
                                }
                            }
                        }
                    });
                } else if (contentBeforeClose === this.originalContent) {
                    this.editorChangePage(previousPageEl, selectedPageRow);
                }
            }
        }
    }

    private closeEditor(pageRows: JQuery) {
        const thisClass = this;
        pageRows.data(this.editModeSelector, false);
        if (typeof this.simplemde !== "undefined" && this.simplemde !== null) {
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
                                this.togglePageRows(pageRows);
                            }
                        },
                        cancel: {
                            label: localization.translate("Cancel", "RidicsProject").value,
                            className: "btn-default",
                            callback: () => { //Switch back to editing mode on cancel
                                pageRows.data(this.editModeSelector, true);
                            }
                        },
                        save: {
                            label: localization.translate("SaveAndClose", "RidicsProject").value,
                            className: "btn-default",
                            callback: () => {
                                this.saveContents(this.currentTextId, contentBeforeClose, SaveTextModeType.FullValidateOrDeny, () => {
                                    thisClass.simplemde.toTextArea();
                                    thisClass.simplemde = null;
                                    this.togglePageRows(pageRows);
                                });
                            }
                        }
                    }
                });
            } else if (contentBeforeClose === this.originalContent) {
                thisClass.simplemde.toTextArea();
                thisClass.simplemde = null;
                this.togglePageRows(pageRows);
            }
        } else {
            this.togglePageRows(pageRows);
        }
    }

    private editorChangePage(previousPageEl: JQuery, currentPageEl: JQuery) {
        previousPageEl.data(this.editModeSelector, false);
        this.togglePageRows(previousPageEl);
        const previousPageCommentAreaEl = previousPageEl.children(".comment-area");
        this.commentArea.updateCommentAreaHeight(previousPageEl);
        this.commentArea.toggleAreaSizeIconHide(previousPageCommentAreaEl);
        currentPageEl.data(this.editModeSelector, true);
        this.togglePageRows(currentPageEl);
        this.originalContent = this.simplemde.value();
    }

    saveText(textId: number, contents: string, mode: SaveTextModeType): JQuery.jqXHR<ISaveTextResponse> {
        const pageEl = $(`*[data-text-id="${textId}"]`);
        const compositionArea = pageEl.children(".composition-area");
        const id = compositionArea.data("id");
        const versionId = compositionArea.data("version-id");
        const request: ICreateTextVersion = {
            id: id,
            text: contents,
            resourceVersionId: versionId
        };
        const ajax = this.util.savePlainText(textId, request, mode);
        ajax.done((response) => {
            if (response.isValidationSuccess) {
                this.originalContent = contents;
                compositionArea.data("version-id", response.resourceVersionId);
            }
        });
        return ajax;
    }

    saveContents(textId: number, contents: string, mode = SaveTextModeType.FullValidateOrDeny, doneCallback: () => void = null): void {
        const saveAjax = this.saveText(textId, contents, mode);
        saveAjax.done((response) => {
            if (!response.isValidationSuccess) {
                bootbox.dialog({
                    title: localization.translate("Fail", "RidicsProject").value,
                    message: localization.translate("CommentValidationError", "RidicsProject").value,
                    buttons: {
                        cancel: {
                            label: localization.translate("Cancel", "RidicsProject").value,
                            className: "btn-default",
                        },
                        autofix: {
                            label: localization.translate("AutomaticallyFixProblems", "RidicsProject").value,
                            className: "btn-default",
                            callback: () => {
                                this.saveContents(textId, contents, SaveTextModeType.FullValidateAndRepair, doneCallback);
                            }
                        },
                        overwrite: {
                            label: localization.translate("SaveWithErrors", "RidicsProject").value,
                            className: "btn-default",
                            callback: () => {
                                this.saveContents(textId, contents, SaveTextModeType.NoValidation, doneCallback);
                            }
                        }
                    }
                });
                return;
            }
            this.simplemde.value(response.newText);
            bootbox.alert({
                title: localization.translate("Success", "RidicsProject").value,
                message: localization.translate("ChangesSaveSuccess", "RidicsProject").value,
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
            if (doneCallback != null) {
                doneCallback();
            }
        }).fail((error) => {
            if (error.status === 409) {
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

    addEditor(pageRow: JQuery) {
        const editorEl = pageRow.find(".editor");
        const textAreaEl = editorEl.children("textarea");
        const textId = parseInt(pageRow.data("text-id") as string);
        this.currentTextId = textId;
        this.simpleMdeOptions = {
            element: textAreaEl[0],
            autoDownloadFontAwesome: false,
            spellChecker: false,
            mode: "gfm",
            toolbar: [ 
                {
                    name: "save",
                    action: (editor) => { this.saveContents(textId, editor.value()) },
                    className: "fa fa-floppy-o",
                    title: localization.translate("Save", "RidicsProject").value
                },{
                    name: "close",
                    action: () => { this.closeEditor(pageRow) },
                    className: "fa fa-times",
                    title: localization.translate("Close", "RidicsProject").value
                },
                this.simpleMdeTools.toolSeparator,
                this.simpleMdeTools.toolSeparator,
                this.simpleMdeTools.toolBold,
                this.simpleMdeTools.toolItalic,
                this.simpleMdeTools.toolSeparator,
                this.simpleMdeTools.toolUnorderedList,
                this.simpleMdeTools.toolOrderedList,
                this.simpleMdeTools.toolSeparator,
                this.simpleMdeTools.toolHeading1,
                this.simpleMdeTools.toolHeading2,
                this.simpleMdeTools.toolHeading3,
                this.simpleMdeTools.toolSeparator,
                this.simpleMdeTools.toolQuote,
                this.simpleMdeTools.toolPreview,
                this.simpleMdeTools.toolHorizontalRule,
                this.simpleMdeTools.toolSeparator,
                {
                    name: "comment",
                    action: ((editor) => {
                        this.toggleCommentFromEditor(editor, true);
                    }),
                    className: "fa fa-comment",
                    title: localization.translate("AddComment", "RidicsProject").value
                },
                this.simpleMdeTools.toolGuide
            ]
        };

        this.setCustomPreviewRender();
        this.simplemde = new SimpleMDE(this.simpleMdeOptions);
        
        const commentIdRegex = new RegExp(`${this.commentInput.commentRegexExpr}`);
        const commentBeginRegex = new RegExp(`(\\$${this.commentInput.commentRegexExpr}\\%)`);
        const commentEndRegex = new RegExp(`(\\%${this.commentInput.commentRegexExpr}\\$)`);
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
        this.commentArea.updateCommentAreaHeight(pageRow);
        this.commentArea.toggleAreaSizeIconHide(pageRow.children(".comment-area"));
        this.originalContent = this.simplemde.value();
    }


    private setCustomPreviewRender() {
        this.simpleMdeTools.toolPreview.action = (editor: SimpleMDE) => {
            this.simpleMdeOptions.previewRender = (plainText: string, preview: HTMLElement) => {
                this.previewRemoteRender(plainText, preview);
                return "<div class=\"loading\"></div>";
            };
            SimpleMDE.togglePreview(editor);
        };
    }

    private previewRemoteRender(text: string, previewElement: HTMLElement) {
        this.isPreviewRendering = !this.isPreviewRendering;
        if (this.isPreviewRendering) {
            return;
        }

        this.client.renderPreview(text, "markdown").done((generatedHtml) => {
            previewElement.innerHTML = generatedHtml;
        }).fail(() => {
            previewElement.innerHTML = `<div>${localization.translate("RenderError", "ItJakubJs").value}</div>`;
        });
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

        pageRow.each((index, child) => {
            const pageEl = $(child as Node as HTMLElement);
            const compositionAreaEl = pageEl.children(".composition-area");
            this.commentArea.updateCommentAreaHeight(pageEl);
            const placeholderSpinner = pageEl.find(".loading");
            placeholderSpinner.show();
            const editPageButton = pageEl.find(".edit-page");
            if (pageEl.data(this.editModeSelector)) { // changing div to textarea here
                editPageButton.addClass("hide");
                this.createEditorAreaBody(compositionAreaEl);
            } else { // changing textarea to div here
                editPageButton.removeClass("hide");
                this.createViewerAreaBody(compositionAreaEl);
            }
        });
    }

    private createEditorAreaBody(compositionAreaEl: JQuery) {
        const child = compositionAreaEl.children(".page");
        const editorElement = child.children(".editor");
        if (editorElement.length) {
            editorElement.remove();
        }
        const viewerElement = child.children(".viewer");
        viewerElement.remove();
       
        const elm = `<div class="editor"><textarea class="plain-text textarea-no-resize"></textarea></div>`;
        child.append(elm);
    }

    private createViewerAreaBody(compositionAreaEl: JQuery) {
        const child = compositionAreaEl.children(".page");
        const viewerElement = child.children(".viewer");
        if (viewerElement.length) {
            viewerElement.remove();
        }

        const editorElement = child.children(".editor");
        editorElement.remove();
        const elm = `<div class="viewer"><span class="rendered-text"></span></div>`;
        child.append(elm);
    }
}