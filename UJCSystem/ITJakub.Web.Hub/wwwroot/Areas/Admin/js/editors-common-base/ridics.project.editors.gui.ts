class EditorsGui {

    showInfoDialog(title: string, message: string) {
        const dialogEl = $(".info-modal-dialog");
        const titleEl = dialogEl.find(".info-dialog-title");
        const messageEl = dialogEl.find(".info-dialog-message");
        titleEl.text(title);
        messageEl.text(message);
        dialogEl.modal("show");
    }

    showSingleInputDialog(title: string, message: string) {
        const dialogEl = $(".input-modal-dialog");
        const titleEl = dialogEl.find(".input-dialog-title");
        const messageEl = dialogEl.find(".input-dialog-message");
        titleEl.text(title);
        messageEl.text(message);
        dialogEl.modal("show");
    }

    showDoubleInputDialog(title: string, primaryMessage: string, secondaryMessage: string) {
        const dialogEl = $(".double-input-modal-dialog");
        const titleEl = dialogEl.find(".double-input-dialog-title");
        const primaryMessageEl = dialogEl.find(".primary-input-dialog-message");
        const secondaryMessageEl = dialogEl.find(".secondary-input-dialog-message");
        titleEl.text(title);
        primaryMessageEl.text(primaryMessage);
        secondaryMessageEl.text(secondaryMessage);
        dialogEl.modal("show");
    }

    showAuthorInputDialog(title: string, primaryMessage: string, secondaryMessage: string) {
        const dialogEl = $(".author-input-modal-dialog");
        const titleEl = dialogEl.find(".author-input-dialog-title");
        const primaryMessageEl = dialogEl.find(".primary-input-dialog-message");
        const secondaryMessageEl = dialogEl.find(".secondary-input-dialog-message");
        titleEl.text(title);
        primaryMessageEl.text(primaryMessage);
        secondaryMessageEl.text(secondaryMessage);
        dialogEl.modal("show");
    }

    showResponsibleTypeInputDialog(title: string, primaryMessage: string, secondaryMessage: string) {
        const dialogEl = $(".responsible-type-input-modal-dialog");
        const titleEl = dialogEl.find(".responsible-type-input-dialog-title");
        const primaryMessageEl = dialogEl.find(".primary-input-dialog-message");
        const secondaryMessageEl = dialogEl.find(".secondary-input-dialog-message");
        titleEl.text(title);
        primaryMessageEl.text(primaryMessage);
        secondaryMessageEl.text(secondaryMessage);
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