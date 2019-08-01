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
    private simplemde: SimpleMDE;
    private options: SimpleMDE.Options;
    private dialogInsertImage: BootstrapDialogWrapper;
    private dialogInsertLink: BootstrapDialogWrapper;
    private isPreviewRendering = false;
    private originalPreviewRender: (plaintext: string, preview?: HTMLElement) => string;

    constructor(textArea: HTMLElement) {
        this.options = {
            element: textArea,
            autoDownloadFontAwesome: false,
            mode: "gfm",
            promptURLs: false,
            spellChecker: false,
            toolbar: [
                this.toolUndo,
                this.toolRedo,
                this.toolSeparator,
                this.toolBold,
                this.toolItalic,
                //this.toolStrikethrough, // not supported by the most markdown parsers
                this.toolSeparator,
                this.toolHeading1,
                this.toolHeading2,
                this.toolHeading3,
                this.toolHeadingSmaller,
                this.toolHeadingBigger,
                this.toolSeparator,
                this.toolUnorderedList,
                this.toolOrderedList,
                this.toolCodeBlock,
                this.toolQuote,
                this.toolSeparator,
                this.toolLink,
                this.toolImage,
                this.toolTable,
                this.toolHorizontalRule,
                this.toolSeparator,
                this.toolPreview,
                this.toolSideBySide,
                this.toolFullScreen,
                this.toolSeparator,
                this.toolGuide
            ]
        };
    }

    public create(initValue?: string) {
        this.setCustomImageTool();
        this.setCustomLinkTool();
        this.setCustomPreviewRender();
        
        this.options.initialValue = initValue;
        this.simplemde = new SimpleMDE(this.options);

        this.originalPreviewRender = this.options.previewRender;
    }

    public getValue(): string {
        return this.simplemde.value();
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
        this.toolSideBySide.action = (editor: SimpleMDE) => {
            this.options.previewRender = this.originalPreviewRender;
            SimpleMDE.toggleSideBySide(editor);
        };

        // for Preview mode use server-side markdown parser
        this.toolPreview.action = (editor: SimpleMDE) => {
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

        this.toolImage.action = (editor: SimpleMDE) => {
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

        this.toolLink.action = (editor: SimpleMDE) => {
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

        var cm = this.simplemde.codemirror;
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

        var cm = this.simplemde.codemirror;
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

    public toolSeparator = "|";

    public toolBold: SimpleMDE.ToolbarIcon = {
        name: "bold",
        action: SimpleMDE.toggleBold,
        className: "fa fa-bold",
        title: localization.translate("Bold", "ItJakubJs").value
    }

    public toolItalic: SimpleMDE.ToolbarIcon = {
        name: "italic",
        action: SimpleMDE.toggleItalic,
        className: "fa fa-italic",
        title: localization.translate("Italic", "ItJakubJs").value
    }

    public toolStrikethrough: SimpleMDE.ToolbarIcon = {
        name: "strikethrough",
        action: SimpleMDE.toggleStrikethrough,
        className: "fa fa-strikethrough",
        title: localization.translate("StrikeThrough", "ItJakubJs").value
    }

    public toolHeadingSmaller: SimpleMDE.ToolbarIcon = {
        name: "heading-smaller",
        action: SimpleMDE.toggleHeadingSmaller,
        className: "fa fa-header fa-header-x fa-header-smaller",
        title: localization.translate("HeaderSmaller", "ItJakubJs").value
    }

    public toolHeadingBigger: SimpleMDE.ToolbarIcon = {
        name: "heading-bigger",
        action: SimpleMDE.toggleHeadingBigger,
        className: "fa fa-header fa-header-x fa-header-bigger",
        title: localization.translate("HeaderBigger", "ItJakubJs").value
    }

    public toolHeading1: SimpleMDE.ToolbarIcon = {
        name: "heading-1",
        action: SimpleMDE.toggleHeading1,
        className: "fa fa-header fa-header-x fa-header-1",
        title: localization.translate("H1", "ItJakubJs").value
    }

    public toolHeading2: SimpleMDE.ToolbarIcon = {
        name: "heading-2",
        action: SimpleMDE.toggleHeading2,
        className: "fa fa-header fa-header-x fa-header-2",
        title: localization.translate("H2", "ItJakubJs").value
    }

    public toolHeading3: SimpleMDE.ToolbarIcon = {
        name: "heading-3",
        action: SimpleMDE.toggleHeading3,
        className: "fa fa-header fa-header-x fa-header-3",
        title: localization.translate("H3", "ItJakubJs").value
    }

    public toolCodeBlock: SimpleMDE.ToolbarIcon = {
        name: "code",
        action: SimpleMDE.toggleCodeBlock,
        className: "fa fa-code",
        title: localization.translate("Code", "ItJakubJs").value
    }

    public toolQuote: SimpleMDE.ToolbarIcon = {
        name: "quote",
        action: SimpleMDE.toggleBlockquote,
        className: "fa fa-quote-left",
        title: localization.translate("Quote", "ItJakubJs").value
    }

    public toolUnorderedList: SimpleMDE.ToolbarIcon = {
        name: "unordered-list",
        action: SimpleMDE.toggleUnorderedList,
        className: "fa fa-list-ul",
        title: localization.translate("UnorderedList", "ItJakubJs").value
    }

    public toolOrderedList: SimpleMDE.ToolbarIcon = {
        name: "ordered-list",
        action: SimpleMDE.toggleOrderedList,
        className: "fa fa-list-ol",
        title: localization.translate("OrderedList", "ItJakubJs").value
    }

    public toolLink: SimpleMDE.ToolbarIcon = {
        name: "link",
        action: SimpleMDE.drawLink,
        className: "fa fa-link",
        title: localization.translate("CreateLink", "ItJakubJs").value
    }

    public toolImage: SimpleMDE.ToolbarIcon = {
        name: "image",
        action: SimpleMDE.drawImage,
        className: "fa fa-picture-o",
        title: localization.translate("InsertImage", "ItJakubJs").value
    }

    public toolTable: SimpleMDE.ToolbarIcon = {
        name: "table",
        action: SimpleMDE.drawTable,
        className: "fa fa-table",
        title: localization.translate("InsertTable", "ItJakubJs").value
    }

    public toolHorizontalRule: SimpleMDE.ToolbarIcon = {
        name: "horizontal-rule",
        action: SimpleMDE.drawHorizontalRule,
        className: "fa fa-minus",
        title: localization.translate("InsertHorizontalRule", "ItJakubJs").value
    }

    public toolPreview: SimpleMDE.ToolbarIcon = {
        name: "preview",
        action: SimpleMDE.togglePreview,
        className: "fa fa-eye no-disable",
        title: localization.translate("TogglePreview", "ItJakubJs").value
    }

    public toolSideBySide: SimpleMDE.ToolbarIcon = {
        name: "side-by-side",
        action: SimpleMDE.toggleSideBySide,
        className: "fa fa-columns no-disable no-mobile",
        title: localization.translate("ToggleSideBySide", "ItJakubJs").value
    }

    public toolFullScreen: SimpleMDE.ToolbarIcon = {
        name: "fullscreen",
        action: SimpleMDE.toggleFullScreen,
        className: "fa fa-arrows-alt no-disable no-mobile",
        title: localization.translate("ToggleFullscreen", "ItJakubJs").value
    }

    public toolGuide: SimpleMDE.ToolbarIcon = {
        name: "guide",
        action: "https://simplemde.com/markdown-guide",
        className: "fa fa-question-circle",
        title: localization.translate("MarkdownGuide", "ItJakubJs").value
    }

    public toolUndo: SimpleMDE.ToolbarIcon = {
        name: "undo",
        action: SimpleMDE.undo,
        className: "fa fa-undo no-disable",
        title: localization.translate("Undo", "ItJakubJs").value
    }

    public toolRedo: SimpleMDE.ToolbarIcon = {
        name: "redo",
        action: SimpleMDE.redo,
        className: "fa fa-repeat no-disable",
        title: localization.translate("Redo", "ItJakubJs").value
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