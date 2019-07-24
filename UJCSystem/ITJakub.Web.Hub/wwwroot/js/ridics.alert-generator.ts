class AlertGenerator {
    public addAlert(container: JQuery, message: string) {
        container.html(`<div class="alert alert-danger alert-dismissible" role="alert">
                          <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                          ${message}
                        </div>`);
    }

    public dismissAlert(container: JQuery) {
        container.find(".alert").alert("close");
    }
}
