class EditionNote {
    private readonly projectId: number;
    private editionNoteVersionId?: number;
    private simpleMde: SimpleMDE;
    private simpleMdeIcons: SimpleMdeTools;
    private util: EditorsUtil;
    private errorHandler: ErrorHandler;
    private alertHolder: JQuery;

    constructor(projectId: number) {
        this.projectId = projectId;
        this.errorHandler = new ErrorHandler();
    }

    init() {
        this.util = new EditorsUtil();
        this.simpleMdeIcons = new SimpleMdeTools();
        const noteTab = $("#project-work-note");
        this.alertHolder = noteTab.find(".alert-holder");
        const noteEditorLoader = noteTab.find(".loader");    

        this.util.loadEditionNote(this.projectId).done((result) => {
            if (result !== null) {
                this.initEditorOnTextarea(result.text);
                this.editionNoteVersionId = result.versionId;
            } else {
                this.initEditorOnTextarea("");
                this.editionNoteVersionId = null;
            }
        }).fail((error) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error, localization.translate("EditionNoteLoadFailed", "RidicsProject").value));
            this.alertHolder.empty().append(alert.buildElement());
        }).always(() => {
            noteEditorLoader.addClass("hide");
        });

        $("#saveNote").click(() => {
            this.saveNote(this.simpleMde.value());
        });
    }

    private initEditorOnTextarea(note: string) {
        if (note == null) {
            note = "";
        }
        const textAreaEl = $(".note-editor-textarea");
        $(".note-editor .bottom-buttons").removeClass("hide");
        textAreaEl.removeClass("hide");
        const simpleMdeOptions: SimpleMDE.Options = {
            element: textAreaEl[0] as Node as HTMLElement,
            autoDownloadFontAwesome: false,
            spellChecker: false,
            mode: "gfm",
            toolbar: [
                this.simpleMdeIcons.toolBold,
                this.simpleMdeIcons.toolItalic,
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolUnorderedList,
                this.simpleMdeIcons.toolOrderedList,
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolHeading1,
                this.simpleMdeIcons.toolHeading2,
                this.simpleMdeIcons.toolHeading3,
                this.simpleMdeIcons.toolSeparator,
                this.simpleMdeIcons.toolQuote,
                this.simpleMdeIcons.toolPreview,
                this.simpleMdeIcons.toolHorizontalRule,
                this.simpleMdeIcons.toolSeparator,
                {
                    name: "save",
                    action: (editor) => { this.saveNote(editor.value()) },
                    className: "fa fa-floppy-o",
                    title: localization.translate("Save", "ItJakubJs").value
                }
            ]
        };
        this.simpleMde = new SimpleMDE(simpleMdeOptions);
        this.simpleMde.value(note);
        this.simpleMde.codemirror.focus();
    }

    private saveNote(noteValue: string) {
        const request: ICreateEditionNote = {
            projectId: this.projectId,
            content: noteValue,
            originalVersionId: this.editionNoteVersionId
        };
        this.alertHolder.empty();
        this.util.saveEditionNote(request).done(() => {
            const error = new AlertComponentBuilder(AlertType.Success).addContent(localization.translate("EditionNoteSaveSuccess", "RidicsProject").value);
            this.alertHolder.empty().append(error.buildElement());
        }).fail((error) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error, localization.translate("EditionNoteSaveFailed", "RidicsProject").value));
            this.alertHolder.empty().append(alert.buildElement());          
        });
    }
}