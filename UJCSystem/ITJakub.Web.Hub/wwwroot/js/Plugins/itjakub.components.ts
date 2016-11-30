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