class DropzoneHelper {
    private static acceptedFiles = ".doc, .docx, .jpg, .jpeg, .png, .bmp, .gif, .xsl, .xslt, .xmd, .xml, .mp3, .ogg, .wav, .zip";

    public static getDefaultConfiguration(): Dropzone.DropzoneOptions {
        var options: Dropzone.DropzoneOptions = {
            maxFilesize: 10000, // size in MB
            uploadMultiple: true,
            autoProcessQueue: true,
            parallelUploads: 5,
            acceptedFiles: this.acceptedFiles,

            dictInvalidFileType: localization.translate("FormatNotSupported").value + this.acceptedFiles,
            dictDefaultMessage: localization.translate("DropFilesForUpload").value,
            dictFallbackMessage: localization.translate("BrowserDoesntSupportDragNDrop").value,
            dictFallbackText: localization.translate("UseBackupForm").value,
            dictFileTooBig: localization.translate("FileTooBig").value,
            dictResponseError: localization.translate("UploadFileError").value,
            dictCancelUpload: localization.translate("CancelUpload").value,
            dictCancelUploadConfirmation: localization.translate("CancelUploadModalConfirm").value,
            dictRemoveFile: localization.translate("DeleteFile").value,
            dictRemoveFileConfirmation: null,
            dictMaxFilesExceeded: localization.translate("CannotUploadMoreFiles").value
        };
        return options;
    }

    public static getErrorFunction(): (file: Dropzone.DropzoneFile, message: string|Error, xhr: XMLHttpRequest) => void {
        var resultFunction = function (file, message, xhr) {
            var errorMessage = xhr
                ? this.options.dictResponseError.replace("{{statusCode}}", xhr.status.toString())
                : message;
            this.defaultOptions.error(file, errorMessage, xhr);
        }
        return (resultFunction) as any; //HACK ohterwise ReSharper shows type error
    }

    public static getFullConfiguration(options: Dropzone.DropzoneOptions): Dropzone.DropzoneOptions {
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
            this.showSaving();
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

    private showSaving() {
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

class UiHelper {
    public static addSelectOptionAndSetDefault($selectElement: JQuery, optionName: string, optionValue: string | number) {
        var newOption = document.createElement("option");
        $(newOption)
            .text(optionName)
            .attr("value", optionValue)
            .appendTo($selectElement);

        var selectElement = <HTMLSelectElement>$selectElement.get(0);
        selectElement.selectedIndex = $selectElement.children().length - 1;
    }

    public static addCheckboxAndSetChecked($container: JQuery, title: string, value: string|number) {
        var div = document.createElement("div");
        var label = document.createElement("label");
        var checkbox = document.createElement("input");
        var span = document.createElement("span");

        $(div)
            .addClass("checkbox")
            .append(label);
        $(label)
            .append(checkbox)
            .append(span);
        $(checkbox)
            .attr("type", "checkbox")
            .attr("value", value);
        $(span).text(title);

        $container.append(div);
    }
}