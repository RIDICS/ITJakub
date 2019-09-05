class SnapshotEditor {
    private readonly projectId: number;
    private readonly client: SnapshotApiClient;
    private readonly imageViewer: ImageViewerContentAddition;
    private readonly errorHandler: ErrorHandler;

    constructor(projectId: number) {
        this.projectId = projectId;
        this.client = new SnapshotApiClient();
        this.imageViewer = new ImageViewerContentAddition(new EditorsUtil());
        this.errorHandler = new ErrorHandler();
    }

    init() {
        $(".selectpicker").selectpicker();

        $(".include-all-checkbox").click((event) => {
            const checkbox = $(event.currentTarget);
            const table = checkbox.parents(".table");
            const isChecked = checkbox.is(":checked");
            table.find(".include-checkboxes input").prop("checked", isChecked);
        });

        $(".include-checkboxes input").click((event) => {
            const table = $(event.currentTarget).parents(".table");

            let isAllChecked = true;
            table.find(".include-checkboxes input").each((index, elem) => {
                if (!(elem as Node as HTMLInputElement).checked) {
                    isAllChecked = false;
                }
            });

            table.find(".include-all-checkbox").prop("checked", isAllChecked);
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