class EditionNote {
    private readonly projectId;
    private simplemde: SimpleMDE;
    private util: EditorsUtil;
    private gui: EditorsGui;

    constructor(projectId: number) {
        this.projectId = projectId;
    }

    init() {
        const util = new EditorsUtil();
        const gui = new EditorsGui();
        this.util = util;
        this.gui = gui;
        const noteContentAjax = util.loadEditionNote(this.projectId);
        noteContentAjax.done((note: string) => {
            this.initEditorOnTextarea(note);
        });
        noteContentAjax.fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load edition note");
            $("#project-work-note").empty().append(error.buildElement());
        });
    }

    private initEditorOnTextarea(note: string) {
        const textAreaEl = $(".note-editor-textarea");
        const simpleMdeOptions: SimpleMDE.Options = {
            element: textAreaEl[0] as Node as HTMLElement,
            autoDownloadFontAwesome: false,
            spellChecker: false,
            mode: "gfm",
            toolbar: [
                "bold", "italic", "|", "unordered-list", "ordered-list", "|", "heading-1", "heading-2", "heading-3",
                "|", "quote", "preview", "horizontal-rule", "|", {
                    name: "save",
                    action: (editor) => { this.saveNote(editor.value()) },
                    className: "fa fa-floppy-o",
                    title: "Save"
                }
            ]
        };
        this.simplemde = new SimpleMDE(simpleMdeOptions);
        this.simplemde.value(note);
        this.simplemde.codemirror.focus();
    }

    private saveNote(noteValue: string) {
        const request: IEditionNote = {
            projectId: this.projectId,
            content: noteValue
        };
        const saveNoteAjax = this.util.saveEditionNote(request);
        saveNoteAjax.done(() => {
            this.gui.showInfoDialog("Success", "Note has been saved successfully");
        });
        saveNoteAjax.fail(() => {
            this.gui.showInfoDialog("Fail", "Note has not been saved");
        });
    }
}