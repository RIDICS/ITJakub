class ImageViewerPageGui {

    showInfoDialog(title: string, message: string) {
        const dialogEl = $(".info-modal-dialog");
        const titleEl = dialogEl.find(".info-dialog-title");
        const messageEl = dialogEl.find(".info-dialog-message");
        titleEl.text(title);
        messageEl.text(message);
        dialogEl.modal("show");
    }
}