$(document).ready(() => {
    var textArea = document.getElementById("text");
    var textEditor = new TextEditorWrapper(textArea);
    textEditor.create();
});

class TextEditorWrapper {
    private simplemde: SimpleMDE;
    private options: SimpleMDE.Options;

    constructor(textArea: HTMLElement) {
        this.options = {
            element: textArea,
            promptURLs: true,
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
        this.options.initialValue = initValue;
        this.simplemde = new SimpleMDE(this.options);
    }

    public getValue(): string {
        return this.simplemde.value();
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
        title: "Italic"
    }

    static toolStrikethrough: SimpleMDE.ToolbarIcon = {
        name: "strikethrough",
        action: SimpleMDE.toggleStrikethrough,
        className: "fa fa-strikethrough",
        title: "Strikethrough"
    }

    static toolHeadingSmaller: SimpleMDE.ToolbarIcon = {
        name: "heading-smaller",
        action: SimpleMDE.toggleHeadingSmaller,
        className: "fa fa-header fa-header-x fa-header-smaller",
        title: "Smaller Heading"
    }

    static toolHeadingBigger: SimpleMDE.ToolbarIcon = {
        name: "heading-bigger",
        action: SimpleMDE.toggleHeadingBigger,
        className: "fa fa-header fa-header-x fa-header-bigger",
        title: "Bigger Heading"
    }

    static toolHeading1: SimpleMDE.ToolbarIcon = {
        name: "heading-1",
        action: SimpleMDE.toggleHeading1,
        className: "fa fa-header fa-header-x fa-header-1",
        title: "Big Heading"
    }

    static toolHeading2: SimpleMDE.ToolbarIcon = {
        name: "heading-2",
        action: SimpleMDE.toggleHeading2,
        className: "fa fa-header fa-header-x fa-header-2",
        title: "Medium Heading"
    }

    static toolHeading3: SimpleMDE.ToolbarIcon = {
        name: "heading-3",
        action: SimpleMDE.toggleHeading3,
        className: "fa fa-header fa-header-x fa-header-3",
        title: "Small Heading"
    }

    static toolCodeBlock: SimpleMDE.ToolbarIcon = {
        name: "code",
        action: SimpleMDE.toggleCodeBlock,
        className: "fa fa-code",
        title: "Code"
    }

    static toolQuote: SimpleMDE.ToolbarIcon = {
        name: "quote",
        action: SimpleMDE.toggleBlockquote,
        className: "fa fa-quote-left",
        title: "Quote"
    }

    static toolUnorderedList: SimpleMDE.ToolbarIcon = {
        name: "unordered-list",
        action: SimpleMDE.toggleUnorderedList,
        className: "fa fa-list-ul",
        title: "Generic List"
    }

    static toolOrderedList: SimpleMDE.ToolbarIcon = {
        name: "ordered-list",
        action: SimpleMDE.toggleOrderedList,
        className: "fa fa-list-ol",
        title: "Numbered List"
    }

    static toolLink: SimpleMDE.ToolbarIcon = {
        name: "link",
        action: SimpleMDE.drawLink,
        className: "fa fa-link",
        title: "Create Link"
    }

    static toolImage: SimpleMDE.ToolbarIcon = {
        name: "image",
        action: SimpleMDE.drawImage,
        className: "fa fa-picture-o",
        title: "Insert Image"
    }

    static toolTable: SimpleMDE.ToolbarIcon = {
        name: "table",
        action: SimpleMDE.drawTable,
        className: "fa fa-table",
        title: "Insert Table"
    }

    static toolHorizontalRule: SimpleMDE.ToolbarIcon = {
        name: "horizontal-rule",
        action: SimpleMDE.drawHorizontalRule,
        className: "fa fa-minus",
        title: "Insert Horizontal Line"
    }

    static toolPreview: SimpleMDE.ToolbarIcon = {
        name: "preview",
        action: SimpleMDE.togglePreview,
        className: "fa fa-eye no-disable",
        title: "Toggle Preview"
    }

    static toolSideBySide: SimpleMDE.ToolbarIcon = {
        name: "side-by-side",
        action: SimpleMDE.toggleSideBySide,
        className: "fa fa-columns no-disable no-mobile",
        title: "Toggle Side by Side"
    }

    static toolGuide: SimpleMDE.ToolbarIcon = {
        name: "guide",
        action: "https://simplemde.com/markdown-guide",
        className: "fa fa-question-circle",
        title: "Markdown Guide"
    }

    static toolUndo: SimpleMDE.ToolbarIcon = {
        name: "undo",
        action: SimpleMDE.undo,
        className: "fa fa-undo no-disable",
        title: "Undo"
    }

    static toolRedo: SimpleMDE.ToolbarIcon = {
        name: "redo",
        action: SimpleMDE.redo,
        className: "fa fa-repeat no-disable",
        title: "Redo"
    }
}