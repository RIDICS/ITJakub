$(document).ready(() => {
    var textArea = document.getElementById("text");
    var textEditor = new TextEditorWrapper(textArea);
    textEditor.create();
});

class TextEditorWrapper {
    private simplemde: SimpleMDE;
    private options: SimpleMDE.Options;
    private dialogInsertImage: BootstrapDialogWrapper;
    private dialogInsertLink: BootstrapDialogWrapper;

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
                TextEditorWrapper.toolStrikethrough,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolHeading1,
                TextEditorWrapper.toolHeading2,
                TextEditorWrapper.toolHeading3,
                TextEditorWrapper.toolHeadingSmaller,
                TextEditorWrapper.toolHeadingBigger,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolCodeBlock,
                TextEditorWrapper.toolQuote,
                TextEditorWrapper.toolUnorderedList,
                TextEditorWrapper.toolOrderedList,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolLink,
                TextEditorWrapper.toolImage,
                TextEditorWrapper.toolTable,
                TextEditorWrapper.toolHorizontalRule,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolPreview,
                TextEditorWrapper.toolSideBySide,
                TextEditorWrapper.toolSeparator,
                TextEditorWrapper.toolGuide
            ]
        };
    }

    public create(initValue?: string) {
        this.setCustomImageTool();
        this.setCustomLinkTool();
        this.options.initialValue = initValue;
        this.simplemde = new SimpleMDE(this.options);
    }

    public getValue(): string {
        return this.simplemde.value();
    }

    private setCustomImageTool() {
        this.dialogInsertImage = new BootstrapDialogWrapper($("#editor-insert-image-dialog"), true);

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
        this.dialogInsertLink = new BootstrapDialogWrapper($("#editor-insert-link-dialog"), true);

        TextEditorWrapper.toolLink.action = (editor: SimpleMDE) => {
            var selectedText = editor.codemirror.getSelection();
            $("#editor-insert-link-label").val(selectedText);
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

class BootstrapDialogWrapper {
    private clearInputElements: boolean;
    private element: JQuery;

    constructor(dialogElement: JQuery, clearInputElements: boolean) {
        this.clearInputElements = clearInputElements;
        this.element = dialogElement;
    }

    public show() {
        this.element.modal({
            show: true,
            backdrop: "static"
        });
    }

    public hide() {
        this.element.modal("hide");
        if (this.clearInputElements) {
            $("input", this.element).val("");
            $("textarea", this.element).val("");
            $("select", this.element).val("");
        }
    }
}