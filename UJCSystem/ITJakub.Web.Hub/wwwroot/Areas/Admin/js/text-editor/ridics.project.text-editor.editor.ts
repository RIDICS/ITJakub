﻿class Editor {
    private currentTextId = 0; //default initialisation
    private originalContent = "";
    private simplemde: IExtendedSimpleMDE;
    private simpleMdeOptions: SimpleMDE.Options;
    private readonly commentInput: CommentInput;
    private readonly util: EditorsUtil;
    private readonly commentArea: CommentArea;
    private readonly editModeSelector = "is-edited";
    private readonly client: TextApiClient;
    private readonly simpleMdeTools: SimpleMdeTools;
    private commentInputDialog: BootstrapDialogWrapper;
    private isPreviewRendering = true;
    private editorExistsInTab = false;

    constructor(commentInput: CommentInput, util: EditorsUtil, commentArea: CommentArea) {
        this.commentInput = commentInput;
        this.util = util;
        this.commentArea = commentArea;
        this.client = new TextApiClient();
        this.simpleMdeTools = new SimpleMdeTools();

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
            pageRow.data(this.editModeSelector, true);
            this.togglePageRows(pageRow);
            pageRow.addClass("init-editor");
        });

        $("#project-resource-preview").on("click", ".editor", (e) => { //dynamically instantiating SimpleMDE editor on textarea
            const elSelected = e.target as HTMLElement;
            const selectedPageRow = $(elSelected).closest(".page-row");
            this.changeOrInitEditor(selectedPageRow);
        });

        $(".turn-on-editing-mode").on("click", () => {
            $(".page-row").data(this.editModeSelector, true);
            this.togglePageRows($(".page-row"));
        });

        $(".turn-off-editing-mode").on("click", () => {
            this.closeEditor($(".page-row"));
        });
    }

    private onSendButtonClick(text: string) {
        const textId = this.getCurrentTextId();
        const cm = this.simplemde.codemirror as CodeMirror.Doc;
        (this.commentInput).toggleCommentSignsAndCreateComment(cm, true, text, textId);
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
            (this.commentInput).toggleCommentSignsAndCreateComment(cm, false);
        }
    }

    changeOrInitEditor(selectedPageRow: JQuery) {
        if (selectedPageRow.data(this.editModeSelector)) {
            let pageDiffers = false;
            const textId = selectedPageRow.data("page") as number;
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
                this.addEditor(selectedPageRow);
                this.originalContent = this.simplemde.value();
            }
            if (this.editorExistsInTab && pageDiffers) {
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
                                    this.saveContents(this.currentTextId, contentBeforeClose).done(() => {
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
                                this.changeEditButtons();
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
                                this.saveContents(this.currentTextId, contentBeforeClose).done(() => {
                                    thisClass.simplemde.toTextArea();
                                    thisClass.simplemde = null;
                                    this.togglePageRows(pageRows);
                                    this.changeEditButtons();
                                });
                            }
                        }
                    }
                });
            } else if (contentBeforeClose === this.originalContent) {
                thisClass.simplemde.toTextArea();
                thisClass.simplemde = null;
                this.changeEditButtons();
                this.togglePageRows(pageRows);
            }
        } else {
            this.changeEditButtons();
            this.togglePageRows(pageRows);
        }
    }

    private editorChangePage(previousPageEl: JQuery, currentPageEl: JQuery) {
        previousPageEl.data(this.editModeSelector, false);
        currentPageEl.data(this.editModeSelector, true);
        const previousPageCommentAreaEl = previousPageEl.children(".comment-area");
        this.simplemde.toTextArea();
        this.simplemde = null;
        this.commentArea.updateCommentAreaHeight(previousPageEl);
        this.commentArea.toggleAreaSizeIconHide(previousPageCommentAreaEl);
        this.addEditor(currentPageEl);
        this.originalContent = this.simplemde.value();
    }

    private saveContents(textId: number, contents: string): JQuery.jqXHR {
        const pageEl = $(`*[data-page="${textId}"]`);
        const compositionArea = pageEl.children(".composition-area");
        const id = compositionArea.data("id");
        const versionNumber = compositionArea.data("version-number");
        const request: ICreateTextVersion = {
            id: id,
            text: contents,
            versionNumber: versionNumber
        };
        const saveAjax = this.util.savePlainText(textId, request).done(() => {
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
        }).fail(() => {
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

        return saveAjax;
    }

    addEditor(pageRow: JQuery) {
        const editorEl = pageRow.find(".editor");
        const textAreaEl = editorEl.children("textarea");
        const textId = parseInt(pageRow.data("page") as string);
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
        this.changeEditButtons();
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

    private changeEditButtons() {
        if (!$(".turn-on-editing-mode").hasClass("hide")) {
            for (let pageRow of $(".page-row").toArray()) {
                if ($(pageRow).data(this.editModeSelector)) {
                    $(".turn-on-editing-mode").addClass("hide");
                    $(".turn-off-editing-mode").removeClass("hide");
                    break;
                }
            }
        } else if(!$(".turn-off-editing-mode").hasClass("hide")) {
            for (let pageRow of $(".page-row").toArray()) {
                if (Boolean($(pageRow).data(this.editModeSelector)) === false) {
                    $(".turn-off-editing-mode").addClass("hide");
                    $(".turn-on-editing-mode").removeClass("hide");
                    break;
                }
            }
        }
    }
}