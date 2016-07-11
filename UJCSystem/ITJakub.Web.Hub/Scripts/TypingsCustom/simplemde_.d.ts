// Type definitions for SimpleMDE v1.11.2
// Project: https://github.com/NextStepWebs/simplemde-markdown-editor
// Definitions by: Vladimír Pokorný <https://github.com/vladapokorny>
// Definitions: TODO

declare namespace SimpleMDE {
    interface AutoSaveOptions {
        enabled?: boolean;
        delay?: number;
        uniqueId: string;
    }

    interface BlockStyleOptions {
        bold?: string;
        code?: string;
        italic?: string;
    }

    interface InsertTextOptions {
        horizontalRule?: string[];
        image?: string[];
        link?: string[];
        table?: string[];
    }

    interface ParsingOptions {
        allowAtxHeaderWithoutSpace?: boolean;
        strikethrough?: boolean;
        underscoresBreakWords?: boolean;
    }

    interface RenderingOptions {
        singleLineBreaks?: boolean;
        codeSyntaxHighlighting: boolean;
    }

    interface ShortcutsArray {
        [action: string]: string;
        toggleBlockquote?: string;
        toggleBold?: string;
        cleanBlock?: string;
        toggleHeadingSmaller?: string;
        toggleItalic?: string;
        drawLink?: string;
        toggleUnorderedList?: string;
        togglePreview?: string;
        toggleCodeBlock?: string;
        drawImage?: string;
        toggleOrderedList?: string;
        toggleHeadingBigger?: string;
        toggleSideBySide?: string;
        toggleFullScreen?: string;
    }

    interface StatusBarItem {
        className: string;
        defaultValue: (element: HTMLElement) => void;
        onUpdate: (element: HTMLElement) => void;
    }

    interface ToolbarIcon {
        name: string;
        action: string|((editor: any) => void); // TODO specific type
        className: string;
        title: string;
    }

    interface Options {
        autoDownloadFontAwesome?: boolean;
        autofocus?: boolean;
        autosave?: AutoSaveOptions;
        blockStyles?: BlockStyleOptions;
        element?: HTMLElement;
        forceSync?: boolean;
        hideIcons?: string[];
        indentWithTabs?: boolean;
        initialValue?: string;
        insertTexts?: InsertTextOptions;
        lineWrapping?: boolean;
        parsingConfig?: ParsingOptions;
        placeholder?: string;
        previewRender?: (markdownPlaintext: string, previewElement?: HTMLElement) => string;
        promptURLs?: boolean;
        renderingConfig?: RenderingOptions;
        shortcuts?: ShortcutsArray;
        showIcons?: string[];
        spellChecker?: boolean;
        status?: boolean|Array<string|StatusBarItem>;
        styleSelectedText?: boolean;
        tabSize?: number;
        toolbar?: boolean|Array<string|ToolbarIcon>;
        toolbarTips?: boolean;
    }
}

declare class SimpleMDE {
    constructor();
    constructor(options: SimpleMDE.Options);
    value(): string;
    value(val: string): void;
    codemirror: any; // TODO specific type
    toTextArea(): void;
    isPreviewActive(): boolean;
    isSideBySideActive(): boolean;
    isFullscreenActive(): boolean;
    clearAutosavedValue(): void;

    // todo add editor spicific type
    static toggleBold: (editor: any) => void;
    static toggleItalic: (editor: any) => void;
    static toggleStrikethrough: (editor: any) => void;
    static toggleHeadingSmaller: (editor: any) => void;
    static toggleHeadingBigger: (editor: any) => void;
    static toggleHeading1: (editor: any) => void;
    static toggleHeading2: (editor: any) => void;
    static toggleHeading3: (editor: any) => void;
    static toggleCodeBlock: (editor: any) => void;
    static toggleBlockquote: (editor: any) => void;
    static toggleUnorderedList: (editor: any) => void;
    static toggleOrderedList: (editor: any) => void;
    static cleanBlock: (editor: any) => void;
    static drawLink: (editor: any) => void;
    static drawImage: (editor: any) => void;
    static drawTable: (editor: any) => void;
    static drawHorizontalRule: (editor: any) => void;
    static togglePreview: (editor: any) => void;
    static toggleSideBySide: (editor: any) => void;
    static toggleFullScreen: (editor: any) => void;
    static undo: (editor: any) => void;
    static redo: (editor: any) => void;
}