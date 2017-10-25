class TextEditorGui {
    createConfirmationDialog(onClose: Function, onCancel: Function, onSave: Function) {
        $("#save-confirmation-dialog").dialog({
            resizable: false,
            height: "auto",
            width: 400,
            modal: true,
            dialogClass: "save-confirmation-dialog",
            close: () => { onCancel(); },
            title: "Do you want to leave without saving?",
            buttons: [
                {
                    text: "Close without saving",
                    click: function() {
                        onClose();
                        $(this).dialog("close");
                    },
                    class: "btn btn-default save-confirmation-dialog-button"
                },
                {
                    text: "Cancel",
                    click: function() {
                        $(this).dialog("close");
                        onCancel();
                    },
                    class: "btn btn-default save-confirmation-dialog-button",
                    id: "dialog-cancel-button"
                }, {
                    text: "Save",
                    click: function() {
                        onSave();
                        $(this).dialog("close");
                    },
                    class: "btn btn-default save-confirmation-dialog-button"
                }
            ],
            open: (event) => {
                $("#dialog-cancel-button").focus();
                const targetElement = $(event.target);
                targetElement.closest(".ui-dialog")
                    .find(".ui-dialog-titlebar-close")
                    .removeClass("ui-dialog-titlebar-close")
                    .addClass("save-confirmation-dialog-close-button")
                    .html(
                        "<i class=\"fa fa-times\" aria-hidden=\"true\"></i>"); //hack, because bootstrap breaks close button icon
            }
        });
    }

    showMessageDialog(title:string, message:string) {
        const dialogEl = $("#status-dialog");
        dialogEl.dialog({
            resizable: false,
            height: "auto",
            width: 400,
            modal: true,
            dialogClass: "status-dialog",
            title: title,
            buttons: [
                {
                    text: "OK",
                    click: function () {
                        $(this).dialog("close");
                    },
                    class: "btn btn-default status-dialog-button"
                }
            ],
            open: (event) => {
                const targetElement = $(event.target);
                targetElement.closest(".ui-dialog")
                    .find(".ui-dialog-titlebar-close")
                    .removeClass("ui-dialog-titlebar-close")
                    .addClass("status-dialog-close-button")
                    .html(
                        "<i class=\"fa fa-times\" aria-hidden=\"true\"></i>"); //hack, because bootstrap breaks close button icon
            }
        });
        dialogEl.text(message);
    }

    showCommentInputDialog() {
        const dialogEl = $("#comment-input-dialog");
        dialogEl.dialog({
            resizable: false,
            height: "auto",
            width: 400,
            modal: true,
            dialogClass: "comment-input-dialog",
            title: "Your comment",
            buttons: [
                {
                    text: "Send",
                    class: "btn btn-default send-comment-button comment-input-dialog-button"
                }
            ],
            open: (event) => {
                const targetElement = $(event.target);
                targetElement.closest(".ui-dialog")
                    .find(".ui-dialog-titlebar-close")
                    .removeClass("ui-dialog-titlebar-close")
                    .addClass("comment-input-dialog-close-button")
                    .html(
                        "<i class=\"fa fa-times\" aria-hidden=\"true\"></i>"); //hack, because bootstrap breaks close button icon
            }
        });
    }
}