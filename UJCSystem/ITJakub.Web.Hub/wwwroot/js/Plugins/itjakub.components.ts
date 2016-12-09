class DropzoneHelper {
    private static acceptedFiles = ".doc, .docx, .jpg, .jpeg, .png, .bmp, .gif, .xsl, .xslt, .xmd, .xml, .mp3, .ogg, .wav, .zip";

    public static getDefaultConfiguration(): DropzoneOptions {
        var options: DropzoneOptions = {
            maxFilesize: 10000, // size in MB
            uploadMultiple: true,
            autoProcessQueue: true,
            parallelUploads: 5,
            acceptedFiles: this.acceptedFiles,

            dictInvalidFileType: "Tento formát není podporovaný. Vyberte prosím jiný soubor s příponou " + this.acceptedFiles,
            dictDefaultMessage: "Pro nahrávání sem přesuňte soubory",
            dictFallbackMessage: "Váš prohlížeč nepodporuje nahrávání souborů pomocí drag'n'drop.",
            dictFallbackText: "Použijte prosím záložní formulář umíštěný níže.",
            dictFileTooBig: "Velikost souboru ({{filesize}}MB) překročila maximální povolenou velikost {{maxFilesize}}MB.",
            dictResponseError: "Chyba při nahrávání souboru (stavový kód {{statusCode}}).",
            dictCancelUpload: "Zrušit nahrávání",
            dictCancelUploadConfirmation: "Opravdu chcete zrušit nahrávání tohoto souboru?",
            dictRemoveFile: "Odstranit soubor",
            dictRemoveFileConfirmation: null,
            dictMaxFilesExceeded: "Nelze nahrát žádné další soubory."
        };
        return options;
    }

    public static getFullConfiguration(options: DropzoneOptions): DropzoneOptions {
        return $.extend({}, this.getDefaultConfiguration(), options);
    }
}

class BootstrapDialogWrapper {
    private options: IBootstrapDialogWrapperOptions;
    private $element: JQuery;
    private defaultOptions: IBootstrapDialogWrapperOptions = {
        element: null,
        autoClearInputs: true,
        errorElementSelector: ".dialog-error",
        progressElementSelector: ".saving-icon",
        submitElementSelector: ".save-button"
    }

    constructor(options: IBootstrapDialogWrapperOptions) {
        this.options = $.extend({}, this.defaultOptions, options);

        if (options.element instanceof jQuery) {
            this.$element = <JQuery>(options.element);
        } else {
            this.$element = $(options.element);
        }
        
        this.bindEvents();
    }

    private bindEvents() {
        this.$element.on("hidden.bs.modal", () => {
            this.clear();
        });

        $(this.options.submitElementSelector, this.$element).click(() => {
            var callback = this.options.submitCallback;
            if (typeof callback === "function") {
                callback();
            }
        });
    }

    public show() {
        $(this.options.errorElementSelector + ", " + this.options.progressElementSelector, this.$element).hide();

        this.$element.modal({
            show: true,
            backdrop: "static"
        });
    }

    public hide() {
        this.$element.modal("hide");
    }

    private clear() {
        if (this.options.autoClearInputs !== false) {
            $("input", this.$element).val("");
            $("textarea", this.$element).val("");
            $("select", this.$element).val("");
        }
    }

    public showSaving() {
        $(this.options.progressElementSelector, this.$element).show();
        $(this.options.errorElementSelector, this.$element).hide();
    }

    public showError(text: string = null) {
        var $error = $(this.options.errorElementSelector, this.$element);
        if (text != null) {
            $error.text(text);
        }

        $(this.options.progressElementSelector, this.$element).hide();
        $error.show();
    }
}

interface IBootstrapDialogWrapperOptions {
    element: JQuery | HTMLDivElement;
    autoClearInputs?: boolean;
    submitCallback?: () => void;
    //cancelCallback?: () => void;
    errorElementSelector?: string;
    progressElementSelector?: string;
    submitElementSelector?: string;
}

class AlertComponentBuilder {
    private alertType: AlertType;
    private heading: string = null;
    private content: string = null;

    constructor(alertType: AlertType) {
        this.alertType = alertType;
    }

    public addHeading(text: string): AlertComponentBuilder {
        this.heading = text;
        return this;
    }

    public addContent(text: string): AlertComponentBuilder {
        this.content = text;
        return this;
    }

    public buildElement(): HTMLDivElement {
        var alertDiv = document.createElement("div");
        var $alertDiv = $(alertDiv);
        $alertDiv.addClass("alert");

        switch (this.alertType) {
            case AlertType.Success:
                $alertDiv.addClass("alert-success");
            case AlertType.Info:
                $alertDiv.addClass("alert-info");
            case AlertType.Warning:
                $alertDiv.addClass("alert-warning");
            case AlertType.Error:
                $alertDiv.addClass("alert-danger");
        }

        if (this.heading != null) {
            var headingElement = document.createElement("h3");
            $(headingElement)
                .text(this.heading)
                .appendTo(alertDiv);
        }

        if (this.content != null) {
            var contentDiv = document.createElement("div");
            $(contentDiv)
                .text(this.content)
                .appendTo(alertDiv);
        }

        return alertDiv;
    }
}

enum AlertType {
    Success,
    Info,
    Warning,
    Error
}