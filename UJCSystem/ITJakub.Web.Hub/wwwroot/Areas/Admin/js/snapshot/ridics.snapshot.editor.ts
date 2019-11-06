$(document.documentElement).ready(() => {
    var snapshotEditor = new SnapshotEditor();
    snapshotEditor.init();
});

class SnapshotEditor {
    private readonly pageSize = 50; // must be greater than data-size on Bootstrap Select
    private readonly client: SnapshotApiClient;
    private readonly imageViewer: ImageViewerContentAddition;
    private readonly errorHandler: ErrorHandler;

    constructor() {
        this.client = new SnapshotApiClient();
        this.imageViewer = new ImageViewerContentAddition(new EditorsApiClient());
        this.errorHandler = new ErrorHandler();
    }

    init() {
        $(".selectpicker")
            .attr("data-size", 15)
            .selectpicker();

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

        $(".select-version")
            .data("loading", false)
            .data("loaded", false);

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

        $(".select-version .dropdown-menu.inner").parent().on("scroll", event => {
            const container = $(event.currentTarget);
            var lasto = container.find("li:last");
            var s = container.position().top + container.height();
            var o = lasto.height() + lasto.position().top - 4;
            if (o < s) {
                const selectElement = container.parents(".select-version").find("select");
                this.loadResourceVersionsNextPage(selectElement);
            }
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

        const createSnapshotButton = $("#createSnapshotButton");
        $("#createSnapshotButton").on("click",
            (event) => {
                event.preventDefault();
                if ($("table .include-checkboxes input[type=\"checkbox\"]:checked").length === 0) {
                    bootbox.confirm({
                        message: localization.translate("CreateSnapshotWithoutResources", "RidicsProject").value,
                        callback: (result) => {
                            if (result) {
                                createSnapshotButton.children(".saving-icon").removeClass("hide");
                                createSnapshotButton.attr("disabled", "disabled");
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
                    createSnapshotButton.children(".saving-icon").removeClass("hide");
                    createSnapshotButton.attr("disabled", "disabled");
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
        const resourcesCount = $("#selectedResourcesCount");
        resourcesCount.empty();
        for (let panel of $(".publish-resource-panel").toArray()) {
            const resourceType = Number($(panel).data("resource-type"));
            const resources = $(panel).find("table .include-checkboxes input[type=\"checkbox\"]").length;
            const selectedResources =
                $(panel).find("table .include-checkboxes input[type=\"checkbox\"]:checked").length;

            switch (resourceType) {
            case ResourceType.Audio:
                const audioResources = String(resources);
                const audioSelectedResources = String(selectedResources);
                resourcesCount.append(`<p>${localization.translateFormat("SelectedAudioResources",
                    [audioSelectedResources, audioResources],
                    "Admin").value}</p>`);
                break;
            case ResourceType.Image:
                const imageResources = String(resources);
                const imageSelectedResources = String(selectedResources);
                resourcesCount.append(`<p>${localization.translateFormat("SelectedImageResources",
                    [imageSelectedResources, imageResources],
                    "Admin").value}</p>`);
                break;
            case ResourceType.Text:
                const textResources = String(resources);
                const textSelectedResources = String(selectedResources);
                resourcesCount.append(`<p>${localization.translateFormat("SelectedTextResources",
                    [textSelectedResources, textResources],
                    "Admin").value}</p>`);
                break;
            }
        }
    }

    private loadResourceVersions(selectBox: JQuery) {
        const dataLoaded = selectBox.data("loaded");
        if (dataLoaded)
            return;

        const selectedVersion = Number(selectBox.find("option[selected]").text());

        const dropdownContainer = this.getDropdownContainerFromSelect(selectBox);
        dropdownContainer.append(`<li><a><i class="fa fa-refresh fa-spin"></i></a></li>`);
        const resourceId = this.getResourceIdFromSelect(selectBox);

        this.client.getVersionList(resourceId, null, selectedVersion).done((data) => {
            const selectedValue = Number(selectBox.val());
            selectBox.empty();
            this.appendNewResourceVersions(selectBox, data, selectedValue);
            selectBox.data("loaded", true);

            const minLoadedItems = this.pageSize;
            if (data.length < minLoadedItems) {
                this.loadResourceVersionsNextPage(selectBox);
            }

        }).fail((error) => {
            dropdownContainer.empty()
                .append(`<li><a>${this.errorHandler.getErrorMessage(error)}</a></li>`);
        });
    }

    private loadResourceVersionsNextPage(selectBox: JQuery) {
        const dataLoaded = selectBox.data("loaded");
        const isLoading = selectBox.data("loading");

        if (!dataLoaded || isLoading) {
            return;
        }

        const lastLi = selectBox.find("option:last");
        const lastLoadedVersion = Number(lastLi.text());

        const dropdownContainer = this.getDropdownContainerFromSelect(selectBox);
        dropdownContainer.append(`<li><a><i class="fa fa-refresh fa-spin"></i></a></li>`);
        const resourceId = this.getResourceIdFromSelect(selectBox);

        selectBox.data("loading", true);
        this.client.getVersionList(resourceId, lastLoadedVersion, lastLoadedVersion - this.pageSize).done((data) => {
            if (data.length === 0) {
                dropdownContainer.parent().off("scroll");
            }

            this.appendNewResourceVersions(selectBox, data, null);
            selectBox.data("loading", false);

        }).fail((error) => {
            dropdownContainer.empty()
                .append(`<li><a>${this.errorHandler.getErrorMessage(error)}</a></li>`);
        });
    }

    private getDropdownContainerFromSelect(selectBox: JQuery): JQuery {
        return selectBox.parent(".dropdown").find(".dropdown-menu.inner");
    }

    private getResourceIdFromSelect(selectBox: JQuery): number {
        return Number(selectBox.parents(".resource-row").data("id"));
    }

    private appendNewResourceVersions(selectBox: JQuery, data: IResourceVersion[], selectedValue: number) {
        for (let i = 0; i < data.length; i++) {
            const resource = data[i];
            const checked = selectedValue === resource.id;
            const option = new Option(resource.versionNumber, String(resource.id), checked, checked);
            $(option).text(resource.versionNumber);
            $(option).attr("data-subtext", ` (${resource.createDateString})`);
            $(option).data("author", resource.author);
            $(option).data("comment", resource.comment);
            $(option).data("created", resource.createDateString);
            selectBox.append(option);
        }
        selectBox.selectpicker("refresh");
    }

    private selectDefaultBookType(bookType: string) {
        const defaultBookType = $("#publishToModules").find(`.book-types-value[value="${bookType}"] + .book-types`);
        defaultBookType.prop("checked", true);
        defaultBookType.attr("disabled", "disabled");
        defaultBookType.trigger("change");

        const otherBookTypes =
            $("#publishToModules").find(`.book-types-value:not([value="${bookType}"]) + .book-types`);
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
            this.client.getAudio(resourceVersionId).done((response) => {
                modalBody.html(response);
            }).fail((error) => {
                const alert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                modalBody.html(alert);
            });
            break;
        case ResourceType.Image:
            const imageUrl = this.client.getImageUrl(resourceVersionId);
            this.imageViewer.addImageContent(modalBody, imageUrl);
            break;
        case ResourceType.Text:
            this.client.getText(resourceVersionId).done((response) => {
                modalBody.html(response.text);
            }).fail((error) => {
                const alert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                modalBody.html(alert);
            });
            break;
        default:
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate("UnsupportedResourceType", "Admin").value).buildElement();
            modalBody.html(alert);
        }
        resourcePreviewModal.modal("show");
    }
}