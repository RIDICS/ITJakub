class TextEditorGui {
    createConfirmationDialog(onClose: Function, onCancel: Function, onSave: Function) {
        $("#save-confirmation-dialog").dialog({
            resizable: false,
            height: "auto",
            width: 400,
            modal: true,
            dialogClass: "save-confirmation-dialogue",
            close: () => { onCancel(); },
            title: "Do you want to leave without saving?",
            buttons: [
                {
                    text: "Close without saving",
                    click: function () {
                        onClose();
                        $(this).dialog("close");
                    },
                    class: "btn btn-default save-confirmation-dialogue-button"
                },
                {
                    text: "Cancel",
                    click: function () {
                        $(this).dialog("close");
                        onCancel();
                    },
                    class: "btn btn-default save-confirmation-dialogue-button",
                    id: "dialog-cancel-button"
                }, {
                    text: "Save",
                    click: function () {
                        onSave();
                        $(this).dialog("close");
                    },
                    class: "btn btn-default save-confirmation-dialogue-button"
                }
            ],
            open: (event, ui) => {
                $("#dialog-cancel-button").focus();
                const targetElement = $(event.target);
                targetElement.closest(".ui-dialog")
                    .find(".ui-dialog-titlebar-close")
                    .removeClass("ui-dialog-titlebar-close")
                    .addClass("save-confirmation-dialogue-close-button")
                    .html(
                    "<i class=\"fa fa-times\" aria-hidden=\"true\"></i>"); //hack, because bootstrap breaks close button icon
            }
        });
    }
}