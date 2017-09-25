$(document).ready(() => {
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
        var textName = $("#name").val();
        var category = $("#scope").val();
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

                $("#modification-author").text(modificationUpdate.user ? modificationUpdate.user : "(anonym)");
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
            promptURLs: false,
            spellChecker: false,
            toolbar: [
                TextEditorWrapper.toolUndo,
                TextEditorWrapper.toolRedo,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolBold,
                TextEditorWrapper.toolItalic,
                //TextEditorWrapper.toolStrikethrough, // not supported by the most markdown parsers
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolHeading1,
                TextEditorWrapper.toolHeading2,
                TextEditorWrapper.toolHeading3,
                TextEditorWrapper.toolHeadingSmaller,
                TextEditorWrapper.toolHeadingBigger,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolUnorderedList,
                TextEditorWrapper.toolOrderedList,
                TextEditorWrapper.toolCodeBlock,
                TextEditorWrapper.toolQuote,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolLink,
                TextEditorWrapper.toolImage,
                TextEditorWrapper.toolTable,
                TextEditorWrapper.toolHorizontalRule,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolPreview,
                TextEditorWrapper.toolSideBySide,
                TextEditorWrapper.toolFullScreen,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolGuide
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
                previewElement.innerHTML = "<div>Chyba při vykreslování</div>";
                this.isPreviewRendering = false;
            }
        });
    }

    private setCustomPreviewRender() {
        // for SideBySide mode use faster inner markdown parser
        TextEditorWrapper.toolSideBySide.action = (editor: SimpleMDE) => {
            this.options.previewRender = this.originalPreviewRender;
            SimpleMDE.toggleSideBySide(editor);
        };

        // for Preview mode use server-side markdown parser
        TextEditorWrapper.toolPreview.action = (editor: SimpleMDE) => {
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

        TextEditorWrapper.toolImage.action = (editor: SimpleMDE) => {
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

        TextEditorWrapper.toolLink.action = (editor: SimpleMDE) => {
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
        var url = $("#editor-insert-image-url").val();
        var alt = $("#editor-insert-image-alt").val();
        var imageText = "![" + alt + "](" + url+ ")";

        var cm = this.simplemde.codemirror;
        this.replaceSelection(cm, imageText);
    }

    private customLinkAction() {
        var url = $("#editor-insert-link-url").val();
        var label = $("#editor-insert-link-label").val();

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

    static toolSeparator = "|";

    static toolBold: SimpleMDE.ToolbarIcon = {
        name: "bold",
        action: SimpleMDE.toggleBold,
        className: "fa fa-bold",
        title: "Tučné"
    }

    static toolItalic: SimpleMDE.ToolbarIcon = {
        name: "italic",
        action: SimpleMDE.toggleItalic,
        className: "fa fa-italic",
        title: "Kurzíva"
    }

    static toolStrikethrough: SimpleMDE.ToolbarIcon = {
        name: "strikethrough",
        action: SimpleMDE.toggleStrikethrough,
        className: "fa fa-strikethrough",
        title: "Přeškrtnuté"
    }

    static toolHeadingSmaller: SimpleMDE.ToolbarIcon = {
        name: "heading-smaller",
        action: SimpleMDE.toggleHeadingSmaller,
        className: "fa fa-header fa-header-x fa-header-smaller",
        title: "Zmenšit nadpis"
    }

    static toolHeadingBigger: SimpleMDE.ToolbarIcon = {
        name: "heading-bigger",
        action: SimpleMDE.toggleHeadingBigger,
        className: "fa fa-header fa-header-x fa-header-bigger",
        title: "Zvětšit nadpis"
    }

    static toolHeading1: SimpleMDE.ToolbarIcon = {
        name: "heading-1",
        action: SimpleMDE.toggleHeading1,
        className: "fa fa-header fa-header-x fa-header-1",
        title: "Velký nadpis"
    }

    static toolHeading2: SimpleMDE.ToolbarIcon = {
        name: "heading-2",
        action: SimpleMDE.toggleHeading2,
        className: "fa fa-header fa-header-x fa-header-2",
        title: "Střední nadpis"
    }

    static toolHeading3: SimpleMDE.ToolbarIcon = {
        name: "heading-3",
        action: SimpleMDE.toggleHeading3,
        className: "fa fa-header fa-header-x fa-header-3",
        title: "Malý nadpis"
    }

    static toolCodeBlock: SimpleMDE.ToolbarIcon = {
        name: "code",
        action: SimpleMDE.toggleCodeBlock,
        className: "fa fa-code",
        title: "Kód"
    }

    static toolQuote: SimpleMDE.ToolbarIcon = {
        name: "quote",
        action: SimpleMDE.toggleBlockquote,
        className: "fa fa-quote-left",
        title: "Citát"
    }

    static toolUnorderedList: SimpleMDE.ToolbarIcon = {
        name: "unordered-list",
        action: SimpleMDE.toggleUnorderedList,
        className: "fa fa-list-ul",
        title: "Seznam s odrážkami"
    }

    static toolOrderedList: SimpleMDE.ToolbarIcon = {
        name: "ordered-list",
        action: SimpleMDE.toggleOrderedList,
        className: "fa fa-list-ol",
        title: "Číslovaný seznam"
    }

    static toolLink: SimpleMDE.ToolbarIcon = {
        name: "link",
        action: SimpleMDE.drawLink,
        className: "fa fa-link",
        title: "Vytvořit odkaz"
    }

    static toolImage: SimpleMDE.ToolbarIcon = {
        name: "image",
        action: SimpleMDE.drawImage,
        className: "fa fa-picture-o",
        title: "Vložit obrázek"
    }

    static toolTable: SimpleMDE.ToolbarIcon = {
        name: "table",
        action: SimpleMDE.drawTable,
        className: "fa fa-table",
        title: "Vložit tabulku"
    }

    static toolHorizontalRule: SimpleMDE.ToolbarIcon = {
        name: "horizontal-rule",
        action: SimpleMDE.drawHorizontalRule,
        className: "fa fa-minus",
        title: "Vložit horizontální čáru"
    }

    static toolPreview: SimpleMDE.ToolbarIcon = {
        name: "preview",
        action: SimpleMDE.togglePreview,
        className: "fa fa-eye no-disable",
        title: "Přepnout na náhled"
    }

    static toolSideBySide: SimpleMDE.ToolbarIcon = {
        name: "side-by-side",
        action: SimpleMDE.toggleSideBySide,
        className: "fa fa-columns no-disable no-mobile",
        title: "Přepnout na zobrazení textu a náhledu vedle sebe"
    }

    static toolFullScreen: SimpleMDE.ToolbarIcon = {
        name: "fullscreen",
        action: SimpleMDE.toggleFullScreen,
        className: "fa fa-arrows-alt no-disable no-mobile",
        title: "Přepnout na režim celé obrazovky"
    }

    static toolGuide: SimpleMDE.ToolbarIcon = {
        name: "guide",
        action: "https://simplemde.com/markdown-guide",
        className: "fa fa-question-circle",
        title: "Nápověda k formátu Markdown"
    }

    static toolUndo: SimpleMDE.ToolbarIcon = {
        name: "undo",
        action: SimpleMDE.undo,
        className: "fa fa-undo no-disable",
        title: "Zpět"
    }

    static toolRedo: SimpleMDE.ToolbarIcon = {
        name: "redo",
        action: SimpleMDE.redo,
        className: "fa fa-repeat no-disable",
        title: "Znovu"
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