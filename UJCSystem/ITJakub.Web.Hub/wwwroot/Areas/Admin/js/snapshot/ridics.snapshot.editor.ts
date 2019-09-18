﻿$(document.documentElement).ready(() => {
    var snapshotEditor = new SnapshotEditor();
    snapshotEditor.init();
});

class SnapshotEditor {
    private readonly client: SnapshotApiClient;
    private readonly imageViewer: ImageViewerContentAddition;
    private readonly errorHandler: ErrorHandler;

    constructor() {
        this.client = new SnapshotApiClient();
        this.imageViewer = new ImageViewerContentAddition(new EditorsApiClient());
        this.errorHandler = new ErrorHandler();
    }

    init() {
        $(".selectpicker").selectpicker();

        $(".include-all-checkbox").click((event) => {
            const checkbox = $(event.currentTarget);
            const table = checkbox.parents(".table");
            const isChecked = checkbox.is(":checked");
            table.find(".include-checkboxes input[type=\"checkbox\"]").prop("checked", isChecked);
            this.setResourcesCount();
        });

        $(".include-checkboxes input[type=\"checkbox\"]").click((event) => {
            const table = $(event.currentTarget).parents(".table");
            this.setIncludeAllCheckbox(table);
        });

        $(".select-version").click((event) => {
            const selectBox = $(event.currentTarget).find(".select-version");
            this.loadResourceVersions(selectBox);
        });

        $(".select-version").on("change",
            (event) => {
                const selectBox = $(event.currentTarget);
                const resourceRow = selectBox.parents(".resource-row");
                const selectedVersion = selectBox.find("option:selected");
                resourceRow.find(".author").text(selectedVersion.data("author"));
                resourceRow.find(".comment").text(selectedVersion.data("comment"));
                resourceRow.find(".created").text(selectedVersion.data("created"));
                resourceRow.find(".resource-version-id").val(selectedVersion.val());
            });

        $(".resource-preview").click((event) => {
            const resourceRow = $(event.currentTarget).parents(".resource-row");
            const resourceVersionId = Number(resourceRow.find(".resource-version-id").val());
            const resourceName = resourceRow.find(".name").text();
            const resourceType = Number(resourceRow.parents(".publish-resource-panel").data("resource-type"));
            this.showResourcePreview(resourceVersionId, resourceName, resourceType);
        });

        $(".default-book-type").click((event) => {
            const value = String($(event.currentTarget).val());
            this.selectDefaultBookType(value);
        });

        $(".book-types").change((event) => {
            const bookTypeState = $(event.currentTarget).next(".book-types-state");
            if ($(event.currentTarget).prop("checked")) {
                bookTypeState.val("true");
            } else {
                bookTypeState.val("false");
            }
        });

        const defaultBookType = String($(".default-book-type:checked").val());
        this.selectDefaultBookType(defaultBookType);

        for (let table of $(".publish-resource-panel table").toArray()) {
            this.setIncludeAllCheckbox($(table));
        }

        $("#createSnapshot button[type=\"submit\"]").on("click",
            (event) => {
                event.preventDefault();
                if ($("table .include-checkboxes input[type=\"checkbox\"]:checked").length === 0) {
                    bootbox.confirm({
                        message: localization.translate("CreateSnapshotWithoutResources", "RidicsProject").value,
                        callback: (result) => {
                            if (result) {
                                $("#createSnapshot").submit();
                            }
                        },
                        buttons: {
                            confirm: {
                                className: "btn btn-default",
                                label: localization.translate("Confirm", "Admin").value
                            },
                            cancel: {
                                className: "btn btn-default",
                                label: localization.translate("Cancel", "Admin").value
                            }
                        }
                    });
                } else {
                    $("#createSnapshot").submit();
                }
            });
    }

    private setIncludeAllCheckbox(table: JQuery) {
        const checkboxes = table.find(".include-checkboxes input[type=\"checkbox\"]").toArray();
        if (checkboxes.length === 0) {
            return;
        }

        let isAllChecked = true;
        for (let checkbox of checkboxes) {
            if (!(checkbox as HTMLInputElement).checked) {
                isAllChecked = false;
                break;
            }
        }

        table.find(".include-all-checkbox").prop("checked", isAllChecked);
        this.setResourcesCount();
    }

    
    
    
    private setResourcesCount(): void {
        let textResources = "0";
        let textSelectedResources = "0";
        let imageResources = "0";
        let imageSelectedResources = "0";
        let audioResources = "0";
        let audioSelectedResources = "0";
        for (let panel of $(".publish-resource-panel").toArray()) {
            const resourceType = Number($(panel).data("resource-type"));
            const resources = $(panel).find("table .include-checkboxes input[type=\"checkbox\"]").length;
            const selectedResources = $(panel).find("table .include-checkboxes input[type=\"checkbox\"]:checked").length;

            switch (resourceType) {
            case ResourceType.Audio:
                {
                    audioResources = String(resources);
                    audioSelectedResources = String(selectedResources);
                }
                break;
            case ResourceType.Image:
                {
                    imageResources = String(resources);
                    imageSelectedResources = String(selectedResources);
                }
            break;
            case ResourceType.Text:
                {
                    textResources = String(resources);
                    textSelectedResources = String(selectedResources);
                }
            break;
            }
        }

        $("#selectedResourcesCount").text(localization.translateFormat("SelectedResources", [textSelectedResources, textResources, imageSelectedResources, imageResources, audioSelectedResources, audioResources], "RidicsProject").value);
    }

    private loadResourceVersions(selectBox: JQuery) {
        const dataLoaded = selectBox.data("loaded");
        if (!dataLoaded) {
            selectBox.parent(".dropdown").find(".dropdown-menu.inner").append(`<li><a><i class="fa fa-refresh fa-spin"></i></a></li>`);
            const resourceId = selectBox.parents(".resource-row").data("id");
            this.client.getVersionList(resourceId).done((data) => {
                const selectedValue = Number(selectBox.val());
                selectBox.empty();
                for (let i = 0; i < data.length; i++) {
                    const resource = data[i];
                    const checked = selectedValue === resource.id;
                    const option = new Option(resource.versionNumber, String(resource.id), checked, checked);
                    $(option).html(resource.versionNumber);
                    $(option).data("author", resource.author);
                    $(option).data("comment", resource.comment);
                    $(option).data("created", new Date(resource.createDate).toLocaleString());
                    selectBox.append(option);
                }
                selectBox.selectpicker("refresh");
                selectBox.data("loaded", true);
            }).fail((error) => {
                console.log(this.errorHandler.getErrorMessage(error));
                //TODO error, where to place it? Gui.something
            });
        }
    }

    private selectDefaultBookType(bookType: string) {
        const defaultBookType = $("#publishToModules").find(`.book-types-value[value="${bookType}"] + .book-types`);
        defaultBookType.prop("checked", true);
        defaultBookType.attr("disabled", "disabled");
        defaultBookType.trigger("change");

        const otherBookTypes = $("#publishToModules").find(`.book-types-value:not([value="${bookType}"]) + .book-types`);
        otherBookTypes.removeAttr("disabled");
    }

    private showResourcePreview(resourceVersionId: number, resourceName: string, resourceType: ResourceType) {
        const resourcePreviewModal = $("#resourcePreviewModal");

        const modalBody = resourcePreviewModal.find(".modal-body");
        modalBody.html(`<div class="loader"></div>`);

        const modalTitle = resourcePreviewModal.find(".modal-title");
        modalTitle.text(resourceName);

        switch (resourceType) {
        case ResourceType.Audio:
            {
                this.client.getAudio(resourceVersionId).done((response) => {
                    modalBody.html(response);
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                    modalBody.html(alert);
                });
            }
            break;
        case ResourceType.Image:
            {
                const imageUrl = this.client.getImageUrl(resourceVersionId);
                this.imageViewer.addImageContent(modalBody, imageUrl);
            }
            break;
        case ResourceType.Text:
            {
                this.client.getText(resourceVersionId).done((response) => {
                    modalBody.html(response.text);
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                    modalBody.html(alert);
                });
            }
            break;
        default:
        {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate("UnsupportedResourceType", "Admin").value).buildElement();
            modalBody.html(alert);
        }
        }
        resourcePreviewModal.modal("show");
    }
}