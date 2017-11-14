class EditorsGui {

    showInfoDialog(title: string, message: string) {
        const dialogEl = $(".info-modal-dialog");
        const titleEl = dialogEl.find(".info-dialog-title");
        const messageEl = dialogEl.find(".info-dialog-message");
        titleEl.text(title);
        messageEl.text(message);
        dialogEl.modal("show");
    }

    showInputDialog(title: string, message: string) {
        const dialogEl = $(".input-modal-dialog");
        const titleEl = dialogEl.find(".input-dialog-title");
        const messageEl = dialogEl.find(".input-dialog-message");
        titleEl.text(title);
        messageEl.text(message);
        dialogEl.modal("show");
    }

    showConfirmationDialog(title: string, message: string) {
        const dialogEl = $(".confirmation-modal-dialog");
        const titleEl = dialogEl.find(".confirmation-dialog-title");
        const messageEl = dialogEl.find(".confirmation-dialog-message");
        titleEl.text(title);
        messageEl.text(message);
        dialogEl.modal("show");
    }
}