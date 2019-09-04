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
                resourceRow.data("version-id", selectedVersion.val());
                resourceRow.find(".author").text(selectedVersion.data("author"));
                resourceRow.find(".comment").text(selectedVersion.data("comment"));
                resourceRow.find(".created").text(selectedVersion.data("created"));
            });

        $(".resource-preview").click((event) => {
            const resourceRow = $(event.currentTarget).parents(".resource-row");
            const resourceVersionId = resourceRow.data("version-id");
            const resourceName = resourceRow.find(".name").text();
            const resourceType = Number(resourceRow.parents(".publish-resource-panel").data("resource-type"));
            this.showResourcePreview(resourceVersionId, resourceName, resourceType);
        });

        $("#createSnapshot").click(() => {
            this.createSnapshot();
        });

        $("input[name=\"default-book-type\"]").click((event) => {
            const value = String($(event.currentTarget).val());
            this.selectDefaultBookType(value);
        });
    }

    private loadResourceVersions(selectBox: JQuery) {
        const dataLoaded = selectBox.data("loaded");
        if (!dataLoaded) {
            const resourceId = selectBox.parents(".resource-row").data("id");
            this.client.getVersionList(resourceId).done((data) => {
                selectBox.empty();
                for (let i = 0; i < data.length; i++) {
                    const resource = data[i];
                    const option = new Option(resource.versionNumber, String(resource.id));
                    $(option).html(resource.versionNumber);
                    $(option).data("author", resource.author);
                    $(option).data("comment", resource.comment);
                    $(option).data("created", resource.createDate);
                    selectBox.append(option);
                }
                selectBox.selectpicker("refresh");
                selectBox.data("loaded", true);
            }).fail((error) => {
                console.log(error);
                //TODO error, where to place it? Gui.something
            });
        }
    }

    private selectDefaultBookType(bookType: string) {
        const defaultBookType = $("#publishToModules").find(`input[name="book-types"][value="${bookType}"]`);
        defaultBookType.prop("checked", true);
        defaultBookType.attr("disabled", "disabled");

        const otherBookTypes = $("#publishToModules").find(`input[name="book-types"]:not([value="${bookType}"])`);
        otherBookTypes.removeAttr("disabled");
    }

    private createSnapshot() {
        const defaultBookType = String($("#publishToModules").find("input[name=\"default-book-type\"]:checked").val());
        const comment = String($("#publishToModules").find("input[name=\"comment\"]").val());
        
        const publishToModules: string[] = [];
        $("#publishToModules").find("input[name=\"book-types\"]:checked").toArray().forEach((module) => {
            publishToModules.push(String($(module).val()));
        });

        const selectedResources: number[] = [];
        $(".resource-row .include-checkboxes input:checked").parents(".resource-row").toArray().forEach((resource) => {
            selectedResources.push(Number($(resource).data("version-id")));
        });

        const alertHolder = $("#snapshotCreatingResult");
        this.client.createSnapshot(this.projectId, comment, defaultBookType, publishToModules, selectedResources).done(() => {
            const alert = new AlertComponentBuilder(AlertType.Success).addContent(localization.translate("SnapshotCreated", "Admin").value).buildElement();
            alertHolder.html(alert);
        }).fail((error) => {
            const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
            alertHolder.html(alert);
        });
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