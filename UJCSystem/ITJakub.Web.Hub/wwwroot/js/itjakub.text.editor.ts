$(document.documentElement).ready(() => {
    var staticTextEditor = new StaticTextEditor();
    staticTextEditor.init();
});

class StaticTextEditor {
    private textEditor: TextEditorWrapper;

    public init() {
        var textArea = document.getElementById("text");
        this.textEditor = new TextEditorWrapper(textArea);
        this.textEditor.create();

        $("#save-button").click(() => {
            this.saveText();
        });
    }

    private saveText() {
        var textName = $("#name").val() as string;
        var category = $("#scope").val() as string;
        var markdownText = this.textEditor.getValue();

        var data: IStaticTextViewModel = {
            name: textName,
            scope: category,
            text: markdownText,
            format: "markdown"
        };

        $("#save-error").addClass("hidden");
        $("#save-success").addClass("hidden");
        $("#save-progress").removeClass("hidden");
        $("#save-button").prop("disabled", true);

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Text/SaveText",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json",
            success: (modificationUpdate: IModificationUpdateViewModel) => {
                $("#save-success")
                    .removeClass("hidden")
                    .show();
                $("#save-progress").addClass("hidden");
                $("#save-button").prop("disabled", false);
                $("#save-success").delay(3000).fadeOut(2000);

                $("#modification-author").text(modificationUpdate.user ? modificationUpdate.user : localization.translate("Anonymous", "ItJakubJs").value);
                $("#modification-time").text(modificationUpdate.modificationTime);
            },
            error: () => {
                $("#save-error").removeClass("hidden");
                $("#save-progress").addClass("hidden");
                $("#save-button").prop("disabled", false);
            }
        });
    }
}

interface IStaticTextViewModel {
    name?: string;
    text?: string;
    scope?: string;
    format?: string|number;
}

interface IModificationUpdateViewModel {
    user: string;
    modificationTime: string;
}

class TextEditorWrapper {
    private simpleMde: SimpleMDE;
    private simpleMdeIcons: SimpleMdeTools;
    private options: SimpleMDE.Options;
    private dialogInsertImage: BootstrapDialogWrapper;
    private dialogInsertLink: BootstrapDialogWrapper;
    private isPreviewRendering = false;
    private originalPreviewRender: (plaintext: string, preview?: HTMLElement) => string;

    constructor(textArea: HTMLElement) {
        this.simpleMdeIcons = new SimpleMdeTools();
        this.options = {
            element: textArea,
            autoDownloadFontAwesome: false,
            mode: "gfm",
            promptURLs: false,
            spellChecker: false,
            toolbar: [
                this.simpleMdeIcons.toolUndo,
                this.simpleMdeIcons.toolRedo,
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolBold,
                this.simpleMdeIcons.toolItalic,
                //this.simplemdeIcons.toolStrikethrough, // not supported by the most markdown parsers
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolHeading1,
                this.simpleMdeIcons.toolHeading2,
                this.simpleMdeIcons.toolHeading3,
                this.simpleMdeIcons.toolHeadingSmaller,
                this.simpleMdeIcons.toolHeadingBigger,
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolUnorderedList,
                this.simpleMdeIcons.toolOrderedList,
                this.simpleMdeIcons.toolCodeBlock,
                this.simpleMdeIcons.toolQuote,
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolLink,
                this.simpleMdeIcons.toolImage,
                this.simpleMdeIcons.toolTable,
                this.simpleMdeIcons.toolHorizontalRule,
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolPreview,
                this.simpleMdeIcons.toolSideBySide,
                this.simpleMdeIcons.toolFullScreen,
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolGuide
            ]
        };
    }

    public create(initValue?: string) {
        this.setCustomImageTool();
        this.setCustomLinkTool();
        this.setCustomPreviewRender();
        
        this.options.initialValue = initValue;
        this.simpleMde = new SimpleMDE(this.options);

        this.originalPreviewRender = this.options.previewRender;
    }

    public getValue(): string {
        return this.simpleMde.value();
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

    private setCustomPreviewRender() {
        // for SideBySide mode use faster inner markdown parser
        this.simpleMdeIcons.toolSideBySide.action = (editor: SimpleMDE) => {
            this.options.previewRender = this.originalPreviewRender;
            SimpleMDE.toggleSideBySide(editor);
        };

        // for Preview mode use server-side markdown parser
        this.simpleMdeIcons.toolPreview.action = (editor: SimpleMDE) => {
            this.options.previewRender = (plainText: string, preview: HTMLElement) => {
                this.previewRemoteRender(plainText, preview);
                return "<div class=\"loading\"></div>";
            };
            SimpleMDE.togglePreview(editor);
        };
    }

    private setCustomImageTool() {
        this.dialogInsertImage = new BootstrapDialogWrapper({
            element: $("#editor-insert-image-dialog"),
            autoClearInputs: true
        });

        this.simpleMdeIcons.toolImage.action = (editor: SimpleMDE) => {
            var selectedText = editor.codemirror.getSelection();
            $("#editor-insert-image-alt").val(selectedText);
            this.dialogInsertImage.show();
        };

        $("#editor-insert-image-button").click(() => {
            this.customImageAction();
            this.dialogInsertImage.hide();
        });
    }

    private setCustomLinkTool() {
        this.dialogInsertLink = new BootstrapDialogWrapper({
            element: $("#editor-insert-link-dialog"),
            autoClearInputs: true
        });

        this.simpleMdeIcons.toolLink.action = (editor: SimpleMDE) => {
            var selectedText = editor.codemirror.getSelection();
            $("#editor-insert-link-label").val(selectedText);

            if (selectedText.startsWith("http")) {
                $("#editor-insert-link-url").val(selectedText);
            }
            else if (/^.+@.+\..+/.test(selectedText)) { // test if string is e-mail
                $("#editor-insert-link-url").val("mailto:" + selectedText);
            }

            this.dialogInsertLink.show();
        };

        $("#editor-insert-link-button").click(() => {
            this.customLinkAction();
            this.dialogInsertLink.hide();
        });
    }

    private customImageAction() {
        var url = $("#editor-insert-image-url").val() as string;
        var alt = $("#editor-insert-image-alt").val() as string;
        var imageText = "![" + alt + "](" + url+ ")";

        var cm = this.simpleMde.codemirror;
        this.replaceSelection(cm, imageText);
    }

    private customLinkAction() {
        var url = $("#editor-insert-link-url").val() as string;
        var label = $("#editor-insert-link-label").val() as string;

        if (!url) {
            url = "#";
        }
        if (!label) {
            label = url;
        }

        var linkText = "[" + label + "](" + url + ")";

        var cm = this.simpleMde.codemirror;
        this.replaceSelection(cm, linkText);
    }

    private replaceSelection(codeMirror: any, newText: string) {
        var startPoint = codeMirror.getCursor("start");
        var endPoint = codeMirror.getCursor("end");

        codeMirror.replaceSelection(newText);

        if (startPoint !== endPoint) {
            endPoint.ch = startPoint.ch + newText.length;
        }

        codeMirror.setSelection(startPoint, endPoint);
        codeMirror.focus();
    }
}

//class BootstrapDialogWrapper {
//    private clearInputElements: boolean;
//    private element: JQuery;

//    constructor(dialogElement: JQuery, clearInputElements: boolean) {
//        this.clearInputElements = clearInputElements;
//        this.element = dialogElement;

//        this.element.on("hidden.bs.modal", () => {
//            this.clear();
//        });
//    }

//    public show() {
//        this.element.modal({
//            show: true,
//            backdrop: "static"
//        });
//    }

//    public hide() {
//        this.element.modal("hide");
//    }

//    private clear() {
//        if (this.clearInputElements) {
//            $("input", this.element).val("");
//            $("textarea", this.element).val("");
//            $("select", this.element).val("");
//        }
//    }
//}