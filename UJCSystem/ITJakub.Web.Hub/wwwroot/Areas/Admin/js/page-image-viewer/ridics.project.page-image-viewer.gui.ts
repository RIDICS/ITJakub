class ImageViewerPageGui {

    init() {
        this.createModalDialogContainer();
    }

    private createModalDialogContainer() {
        $("#project-resource-images").append(`<div class="modals"></div>`);
    }

    private createModalDialogElement():JQuery {
        const dialogText = `<div class="modal fade info-modal-dialog" role="dialog">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal">&times;</button>
        <h4 class="modal-title info-dialog-title"></h4>
      </div>
      <div class="modal-body">
        <p class="info-dialog-message"></p>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>`;
        const html = $.parseHTML(dialogText);
        const htmlEl = $(html);
        htmlEl.hide();
        $(".modals").append(htmlEl);
        return htmlEl;
    }

    showInfoDialog(title: string, message: string) {
        var dialogEl = $(".info-modal-dialog");
        if (!dialogEl.length) {
            dialogEl = this.createModalDialogElement();
        }
        const titleEl = dialogEl.find(".info-dialog-title");
        const messageEl = dialogEl.find(".info-dialog-message");
        titleEl.text(title);
        messageEl.text(message);
        dialogEl.modal("show");
    }

    private dialogFillStrings() {}
}