﻿class EditionNote {
    private readonly projectId: number;
    private editionNoteVersionId?: number;
    private simpleMde: SimpleMDE;
    private simpleMdeIcons: SimpleMdeTools;
    private util: EditorsApiClient;
    private errorHandler: ErrorHandler;
    private alertHolder: JQuery;
    private originalNote: string;

    constructor(projectId: number) {
        this.projectId = projectId;
        this.errorHandler = new ErrorHandler();
        this.simpleMde = null;
    }

    isChangeMade(): boolean {
        if (this.simpleMde == null) {
            return false;
        }

        return this.simpleMde.value() != this.originalNote;
    }
    
    init() {
        this.util = new EditorsApiClient();
        this.simpleMdeIcons = new SimpleMdeTools();
        const noteTab = $("#project-layout-content");
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

        $("#saveNote").on("click", () => {
            this.saveNote(this.simpleMde.value());
        });
    }

    private initEditorOnTextarea(note: string) {
        if (note == null) {
            note = "";
        }

        const editPermission = new ProjectPermissionsProvider().hasEditPermission();

        this.originalNote = note;        
        const textAreaEl = $(".note-editor-textarea");
        $(".note-editor .bottom-buttons").removeClass("hide");
        textAreaEl.removeClass("hide");
        const simpleMdeOptions: SimpleMDE.Options = {
            element: textAreaEl[0] as Node as HTMLElement,
            autoDownloadFontAwesome: false,
            spellChecker: false,
            mode: "gfm",
            toolbar: [
                {
                    name: "save",
                    action: (editor) => { this.saveNote(editor.value()) },
                    className: "fa fa-floppy-o",
                    title: localization.translate("Save", "ItJakubJs").value
                },
                this.simpleMdeIcons.toolSeparator,
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
                this.simpleMdeIcons.toolSeparator
            ]
        };
        if (!editPermission) {
            simpleMdeOptions.toolbar = false;
        }

        this.simpleMde = new SimpleMDE(simpleMdeOptions);
        this.simpleMde.value(note);
        this.simpleMde.codemirror.focus();

        if (!editPermission) {
            this.simpleMde.codemirror.options.readOnly = true;
        }
    }

    private saveNote(noteValue: string) {
        const request: ICreateEditionNote = {
            projectId: this.projectId,
            content: noteValue,
            originalVersionId: this.editionNoteVersionId
        };
        this.alertHolder.empty();
        this.util.saveEditionNote(request).done((editionNoteVersionId) => {
            this.editionNoteVersionId = editionNoteVersionId;
            this.originalNote = noteValue;
            const successAlert = new AlertComponentBuilder(AlertType.Success).addContent(localization.translate("EditionNoteSaveSuccess", "RidicsProject").value);
            this.alertHolder.empty().append(successAlert.buildElement()).delay(3000).fadeOut(2000);
        }).fail((error) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error, localization.translate("EditionNoteSaveFailed", "RidicsProject").value));
            this.alertHolder.empty().append(alert.buildElement());          
        });
    }
}