class PageListEditorMain {
    private editDialog: BootstrapDialogWrapper;
    private readonly gui: EditorsGui;
    private readonly util: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly fsPageName = "FS";
    private readonly fcPageName = "FC";

    constructor() {
        this.gui = new EditorsGui();
        this.errorHandler = new ErrorHandler();
        this.util = new EditorsApiClient();

        jQuery.extend(jQuery.validator.messages,
            {
                required: localization.translate("EnterValidNumbers", "RidicsProject").value
            });

        $.validator.addMethod(
            "regex",
            (value, element, regex) => {
                if (value === "")
                    return false;
                return PageListGeneratorFactory.createPageListGenerator(this.getSelectedFormat()).checkInputValue(value);
            },
            localization.translate("EnterValidNumbers", "RidicsProject").value
        );

        $("#rangeForm").validate({
            rules: {
                "project-pages-generate-from": {
                    required: true,
                    regex: true,
                },
                "project-pages-generate-to": {
                    required: true,
                    regex: true,
                }
            },
            highlight: (element) => {
                $(element).parents(".form-group").removeClass("has-success").addClass("has-error");
            },
            unhighlight: (element) => {
                $(element).parents(".form-group").removeClass("has-error").addClass("has-success");
            }
        });

        $("#project-pages-format").change(() => {
            $("#rangeForm").validate().element(":input[name=\"project-pages-generate-from\"]");
            $("#rangeForm").validate().element(":input[name=\"project-pages-generate-to\"]");
            this.setAlerts();
        });
      
        const doublePageRadiobuttonEl = $(".doublepage-radiobutton");
        doublePageRadiobuttonEl.change(() => {
            this.setAlerts();
        });

        $("#project-pages-generate-to, #project-pages-generate-from").on("input", () => {
            this.setAlerts();
        });
    }

    init(projectId: number) {
        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-pages-dialog"),
            autoClearInputs: false
        });

        $(".panel-bottom-buttons .move-page-down").click(() => {
            this.moveList(true, $(".pages"));
        });

        $(".panel-bottom-buttons .move-page-up").click(() => {
            this.moveList(false, $(".pages"));
        });

        $(".save-pages-button").on("click",
            () => {
                const listing = $(".page-listing");
                const pages = $(".page-row").toArray();
                const pageListArray: IUpdatePage[] = [];
                for (let i = 0; i < pages.length; i++) {
                    pageListArray.push({
                        id: $(pages[i]).data("page-id"),
                        position: i + 1,
                        name: $(pages[i]).find(".name").text().trim()
                    });
                }
                listing.empty().append(`<div class="loader"></div>`);
                this.util.savePageList(projectId, pageListArray).done(() => {
                    $("#unsavedChanges").addClass("hide");
                    const alert = new AlertComponentBuilder(AlertType.Success).addContent(localization.translate("PageListSaveSuccess","RidicsProject").value).buildElement();
                    const alertHolder = $(".save-alert-holder");
                    alertHolder.append(alert);
                    alertHolder.delay(3000).fadeOut(2000);
                    
                    this.util.getPageListView(projectId).done((data) => {
                        listing.html(data);
                        this.initPageRowClicks();
                    }).fail((error) => {
                        const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                        listing.empty().append(alert);
                    });
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                    listing.empty().append(alert);
                });
            });

        this.initPageRowClicks();

        $("#project-pages-edit-button").click(() => {
            this.editDialog.show();
            this.enableCheckboxes();
            const pages = $(".page-row").toArray();
            for (let page of pages) {
                const pageName = $(page).find(".name").text();
                if (pageName.toLocaleLowerCase().trim() === this.fcPageName.toLocaleLowerCase()) {
                    this.checkmarkFC();
                }
                if (pageName.toLocaleLowerCase().trim() === this.fsPageName.toLocaleLowerCase()) {
                    this.checkmarkFS();
                }
            }

            this.trackSpecialPagesCheckboxesState();
        });

        $(".page-list-editor-content").on("click",
            ".generate-page-list",
            () => {
                this.startGeneration();
            });

        $(".page-list-editor-content").on("click",
            ".cancel-page-list",
            (event) => {
                event.stopPropagation();
                this.editDialog.hide();
            }
        );
    }

    private getSelectedFormat(): number {
        const formatString = $("#project-pages-format").find(":selected").data("format-value") as string;
        return PageListFormat[formatString] as number;
    }

    private isSetDoublePageGeneration(): boolean {
        const doublePageRadiobuttonEl = $(".doublepage-radiobutton");
        return doublePageRadiobuttonEl.prop("checked") as boolean;
    }

    private setAlerts(): void {
        const from = String($("#project-pages-generate-from").val());
        const to = String($("#project-pages-generate-to").val());

        const doubleGenerationWarning = $("#doubleGenerationWarning");
        const swapNumbersError = $("#swapNumbersError");
        const listGenerator = PageListGeneratorFactory.createPageListGenerator(this.getSelectedFormat());

        if (!listGenerator.checkInputValue(from) || !listGenerator.checkInputValue(to)) {
            doubleGenerationWarning.addClass("hide");
            swapNumbersError.addClass("hide");
        }
        else {
            if (listGenerator.checkValidPagesLength(from, to)) {
                swapNumbersError.addClass("hide");
                this.setDoubleGenerationWarning();
            } else {
                swapNumbersError.removeClass("hide");
                doubleGenerationWarning.addClass("hide");
            }
        }
    }

    private setDoubleGenerationWarning(): void {
        const doubleGenerationWarning = $("#doubleGenerationWarning");
        if (this.isSetDoublePageGeneration()) {
            const to = String($("#project-pages-generate-from").val());
            const from = String($("#project-pages-generate-to").val());
            const listGenerator = PageListGeneratorFactory.createPageListGenerator(this.getSelectedFormat());

            if (listGenerator.checkValidDoublePageRange(to, from)) {
                doubleGenerationWarning.addClass("hide");
            } else {
                doubleGenerationWarning.removeClass("hide");
            }
        } else {
            doubleGenerationWarning.addClass("hide");
        }
    }

    private initPageRowClicks() {
        $(".page-row .ridics-checkbox label").off();
        $(".page-row .ridics-checkbox label").click((event) => {
            event.stopPropagation(); //stop propagation to prevent loading detail, while is clicked on the checkbox
        });


        $(".page-row .remove-page").off();
        $(".page-row .remove-page").click((event) => {
            event.stopPropagation();
            const pageRow = $(event.currentTarget).parents(".page-row");
            pageRow.remove();
            this.showUnsavedChangesAlert();
        });

        $(".page-row .edit-page").off();
        $(".page-row .edit-page").on("click",
            (event) => {
                event.stopPropagation();
                this.editPage($(event.currentTarget));
            });
        
        $(".page-row .discard-changes").off();
        $(".page-row .discard-changes").on("click",
            (event) => {
                event.stopPropagation();
                this.discardChanges($(event.currentTarget));
            });

        $(".page-row").off();
        $(".page-row").on("click",
            (event) => {
                const checkbox = $(event.currentTarget).find(".selection-checkbox");
                checkbox.prop("checked", !checkbox.is(":checked"));
                this.selectPage($(event.currentTarget));
            });
    }

    private editPage(element: JQuery) {
        const editButton = element.find("i.fa");
        const pageRow = editButton.parents(".page-row");
        const nameElement = pageRow.find(".name");
        const nameInput = pageRow.find("input[name=\"page-name\"]");
        const discardButton = pageRow.find(".discard-changes");

        if (editButton.hasClass("fa-pencil")) {
            editButton.switchClass("fa-pencil", "fa-check");
            nameElement.addClass("hide");
            nameInput.removeClass("hide");
            discardButton.removeClass("hide");
        } else {
            nameInput.addClass("hide");
            discardButton.addClass("hide");
            editButton.switchClass("fa-check", "fa-pencil");
            const newName = String(nameInput.val());
            if (newName !== "" && String(nameElement.text().trim()) !== newName) {
                nameElement.text(newName);
                this.showUnsavedChangesAlert();
            }
            nameElement.removeClass("hide");
        }
    }

    private discardChanges(discardButton: JQuery) {
        const pageRow = discardButton.parents(".page-row");
        const editButton = pageRow.find(".edit-page i.fa");
        const nameElement = pageRow.find(".name");
        const nameInput = pageRow.find("input[name=\"page-name\"]");
        
        nameInput.addClass("hide");
        discardButton.addClass("hide");
        editButton.switchClass("fa-check", "fa-pencil");
        nameInput.val(nameElement.text().trim());
        nameElement.removeClass("hide");
    }

    private selectPage(pageRow: JQuery) {
        pageRow.siblings().removeClass("active");
        pageRow.addClass("active");

        const pageId = pageRow.data("page-id");
        const pageDetail = $("#page-detail");
        const alertHolder = pageDetail.find(".alert-holder");
        const content = pageDetail.find(".body-content");
        const textIcon = pageDetail.find(".fa-file-text-o");
        const imageIcon = pageDetail.find(".fa-image");
        alertHolder.empty();

        if (typeof pageId == "undefined") {
            textIcon.addClass("hide");
            imageIcon.addClass("hide");
            const alert = new AlertComponentBuilder(AlertType.Info)
                .addContent(localization.translate("EmptyPage", "RidicsProject").value).buildElement();
            alertHolder.empty().append(alert);
            content.empty();
            pageDetail.removeClass("hide");
            return;
        }

        content.empty().html("<div class=\"sub-content\"></div>");
        const subcontent = content.find(".sub-content");
        subcontent.html("<div class=\"loader\"></div>");
        pageDetail.removeClass("hide");

        this.util.getPageDetail(pageId).done((response) => {
            subcontent.html(response);

            if (content.find(".page-text").length > 0) {
                textIcon.removeClass("hide");
            } else {
                textIcon.addClass("hide");
            }

            if (content.find(".image-preview").length > 0) {
                imageIcon.removeClass("hide");
            } else {
                imageIcon.addClass("hide");
            }
        }).fail((error) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
            alertHolder.empty().append(alert);
            subcontent.empty();
        });
    }

    private enableCheckboxes() {
        const fcCheckbox = $(".book-cover-checkbox");
        const fsCheckbox = $(".book-startpage-checkbox");
        fsCheckbox.prop("disabled", false);
        fcCheckbox.prop("disabled", false);
    }

    private checkmarkFC() {
        const fcCheckbox = $(".book-cover-checkbox");
        fcCheckbox.prop("checked", true);
    }

    private checkmarkFS() {
        const fsCheckbox = $(".book-startpage-checkbox");
        fsCheckbox.prop("checked", true);
    }

    private removePage(name: string) {
        const pageListEl = $(".page-listing tbody");
        const listItems = pageListEl.children(".page-row").toArray();
        for (let element of listItems) {
            const listItemEl = $(element as Node as Element);
            if (listItemEl.find(".name").text().toLocaleLowerCase().trim() === name.toLocaleLowerCase()) {
                listItemEl.remove();
            }
        }
    }

    private addFSPage() {
        const pageListEl = $(".page-listing tbody");
        const firstPageRow = pageListEl.children(".page-row").first();
        if (firstPageRow.length) {
            const fsPageRow = this.createPageRow(this.fsPageName);
            const pageName = firstPageRow.find(".name").text().toLocaleLowerCase().trim();
            if (pageName === this.fcPageName.toLocaleLowerCase()) {
                firstPageRow.after(fsPageRow);
            } else {
                firstPageRow.before(fsPageRow);
            }
        }
    }

    private addFCPage() {
        const pageListEl = $(".page-listing tbody");
        const firstPageRow = pageListEl.children(".page-row").first();
        if (firstPageRow.length) {
            const fcPageRow = this.createPageRow(this.fcPageName);
            firstPageRow.before(fcPageRow);
        }
    }

    private trackSpecialPagesCheckboxesState() {
        const specialPagesControlsEl = $(".special-pages-controls");
        const startpageCheckboxEl = $(".book-startpage-checkbox");
        const bookcoverCheckboxEl = $(".book-cover-checkbox");
        specialPagesControlsEl.off();
        var fsCheckboxPrevState = startpageCheckboxEl.prop("checked") as boolean;
        var fcCheckboxPrevState = bookcoverCheckboxEl.prop("checked") as boolean;
        specialPagesControlsEl.on("click",
            ".book-cover-checkbox",
            () => {
                const fcCheckboxCurrentState = bookcoverCheckboxEl.prop("checked") as boolean;
                if (fcCheckboxPrevState && !fcCheckboxCurrentState) {
                    this.removePage(this.fcPageName);
                }
                if (!fcCheckboxPrevState && fcCheckboxCurrentState) {
                    this.addFCPage();
                }
                fcCheckboxPrevState = fcCheckboxCurrentState;
                this.showUnsavedChangesAlert();
            });
        specialPagesControlsEl.on("click",
            ".book-startpage-checkbox",
            () => {
                const fsCheckboxCurrentState = startpageCheckboxEl.prop("checked") as boolean;
                if (fsCheckboxPrevState && !fsCheckboxCurrentState) {
                    this.removePage(this.fsPageName);
                }
                if (!fsCheckboxPrevState && fsCheckboxCurrentState) {
                    this.addFSPage();
                }
                fsCheckboxPrevState = fsCheckboxCurrentState;
                this.showUnsavedChangesAlert();
            });
    }

    private startGeneration() {
        const fromFieldValue = ($("#project-pages-generate-from").val() as string).replace(" ", "");
        const toFieldValue = ($("#project-pages-generate-to").val() as string).replace(" ", "");
        const listGenerator = PageListGeneratorFactory.createPageListGenerator(this.getSelectedFormat());

        if (!listGenerator.checkInputValue(fromFieldValue) || !listGenerator.checkInputValue(toFieldValue)) {
            this.gui.showInfoDialog(localization.translate("Warning").value,
                localization.translate("EnterValidNumbers", "RidicsProject").value);
        } else if (listGenerator.checkValidPagesLength(String($("#project-pages-generate-from").val()),
            String($("#project-pages-generate-to").val())))
        {
            const pageList = listGenerator.generatePageList(fromFieldValue, toFieldValue, this.isSetDoublePageGeneration());
            $("#project-pages-dialog").modal("hide");
            this.populateList(pageList);
            this.enableCheckboxes();
            this.trackSpecialPagesCheckboxesState();
        } else {
            this.gui.showInfoDialog(localization.translate("Warning").value,
                localization.translate("SwapNumbers", "RidicsProject").value);
        }
    }

    private populateList(pageList: string[]) {
        const listContainerEl = $(".page-listing tbody");
        if (listContainerEl.children().length) {
            this.gui.showInfoDialog(localization.translate("Info").value,
                localization.translate("AddingGeneratedNames", "RidicsProject").value);
        } else {
            $("#noPagesAlert").remove();
        }

        for (let page of pageList) {
            const html = this.createPageRow(page);
            listContainerEl.append(html);
        }

        this.showUnsavedChangesAlert();
        this.initPageRowClicks();
    }

    private moveList(down: boolean, context: JQuery) {
        const distance = Number(context.find(".page-move-distance").val());
        const pages = context.find(".page-row").toArray();
        const newPages = new Array<HTMLElement>(pages.length);

        if (context.find(".selection-checkbox:checked").length == 0) {
            return;
        }

        for (let i = 0; i < pages.length; i++) {
            const page = pages[i];
            const isSelected = $(page).find(".selection-checkbox").is(":checked");
            if (isSelected) {
                let newPosition = i;
                if (down) {
                    newPosition += distance;
                } else {
                    newPosition -= distance;
                }

                if (newPosition >= pages.length || newPosition < 0) {
                    newPosition = newPosition % pages.length;
                }
                if (newPosition < 0) {
                    newPosition = pages.length + newPosition;
                }

                newPages[newPosition] = page;
            }
        }

        let j = 0;
        for (const page of pages) {
            const isSelected = $(page).find(".selection-checkbox").is(":checked");
            if (!isSelected) {
                while (true) {
                    if (typeof newPages[j] == "undefined") {
                        newPages[j] = page;
                        break;
                    } else {
                        j++;
                    }
                }
            }
        }

        const listing = context.find(".page-listing tbody");
        listing.empty();
        for (const page of newPages) {
            listing.append(page);
        }

        this.showUnsavedChangesAlert();
        this.initPageRowClicks();
    }

    private showUnsavedChangesAlert() {
        $("#unsavedChanges").removeClass("hide");
    }

    // page-row has duplicate definition in _PageTable.cshtml
    private createPageRow(name: string): string {
        return `<tr class="page-row">
                    <td class="ridics-checkbox">
                        <label>
                            <input type="checkbox" class="selection-checkbox">
                            <span class="cr cr-black">
                                <i class="cr-icon glyphicon glyphicon-ok"></i>
                            </span>
                        </label>
                    </td>
                    <td>
                        <div>
                            <input type="text" name="page-name" class="form-control hide" value="${name}" />
                            <div class="name">
                                ${name}
                            </div>
                        </div>
                        <div class="alert alert-danger"></div>
                    </td>
                    <td class="buttons">
                        <button type="button"  class="edit-page btn btn-sm btn-default">
                            <i class="fa fa-pencil"></i>
                        </button>
                        <button type="button" class="discard-changes btn btn-sm btn-default hide">
                            <i class="fa fa-times"></i>
                        </button>
                        <button type="button" class="remove-page btn btn-sm btn-default">
                            <i class="fa fa-trash"></i>
                        </button>
                    </td>
                </tr>`;
    }
}