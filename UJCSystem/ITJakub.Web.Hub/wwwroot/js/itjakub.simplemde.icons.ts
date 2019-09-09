class SimpleMdeIcons {
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