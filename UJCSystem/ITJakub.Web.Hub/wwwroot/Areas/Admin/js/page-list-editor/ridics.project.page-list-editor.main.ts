class PageListEditorMain {
    private editDialog: BootstrapDialogWrapper;
    private readonly gui: EditorsGui;
    private readonly util: EditorsUtil;
    private readonly errorHandler: ErrorHandler;

    constructor() {
        this.gui = new EditorsGui();
        this.errorHandler = new ErrorHandler();
        this.util = new EditorsUtil();
    }
    
    init(projectId: number) {
        const listGenerator = new PageListGenerator();
        const listStructure = new PageListStructure();
        const editor = new PageListEditorTextEditor();

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

       this.initPageRowClicks();

        $("#project-pages-edit-button").click(() => {
            this.editDialog.show();
            this.enableCheckboxes();
            const pages = $(".page-row").toArray();
            for (let page of pages) {
                const pageName = $(page).find(".name").text();
                if (pageName.toLocaleLowerCase() === "fc") {
                    this.checkmarkFC();
                }
                if (pageName.toLocaleLowerCase() === "fs") {
                    this.checkmarkFS();
                } 
            }
           
            this.trackSpecialPagesCheckboxesState();

            
            $(".page-list-editor-content").on("click",
                ".generate-page-list",
                () => {
                    const doublePageRadiobuttonEl = $(".doublepage-radiobutton");
                    const doublePageGeneration = doublePageRadiobuttonEl.prop("checked") as boolean;
                    this.startGeneration(listGenerator, listStructure, doublePageGeneration);
                });

            $(".page-list-editor-content").on("click",
                ".save-page-list",
                () => {
                    const textAreaEl = $(".page-list-edit-textarea");
                    const pageListDivEl = $(".page-list");
                    if (textAreaEl.length) {
                        const pageListString = textAreaEl.val() as string;
                        const pageListStringArray = pageListString.split("\n");
                        //util.savePageList(pageLingStringArray); TODO use after server functions are done
                    }
                    if (pageListDivEl.length) {
                        const pageItemsEl = pageListDivEl.children(".page-list-item");
                        var pageListStringArray: string[] = [];
                        pageItemsEl.each((index, element) => {
                            const pageEl = $(element as Node as Element);
                            pageListStringArray.push(pageEl.text());
                        });
                        //util.savePageList(pageLingStringArray); TODO use after server functions are done
                    }
                }
            );

            $(".page-list-editor-content").on("click",
                ".cancel-page-list",
                (event) => {
                    event.stopPropagation();
                    this.editDialog.hide();
                    $(".page-list-editor-content").off();
                }
            );
        });
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
        $(".page-row .edit-page").click((event) => {
            event.stopPropagation();
            const editButton = $(event.currentTarget).find("i.fa");
            const pageRow = editButton.parents(".page-row");
            const nameElement = pageRow.find(".name");
            if (editButton.hasClass("fa-pencil")) {
                editButton.switchClass("fa-pencil", "fa-check");
                const name = nameElement.text();
                nameElement.html(`<input type="text" data-old-name="${name}" class="form-control" value="${name}" />`);
            } else {
                editButton.switchClass("fa-check", "fa-pencil");
                const newNameInput = nameElement.find("input");
                const newName = String(newNameInput.val());
                nameElement.text(newName);

                if (String(newNameInput.data("old-name")) !== newName) {
                    this.showUnsavedChangesAlert();
                }
            }
        });

        $(".page-row").off();
        $(".page-row").click((event) => {
            const pageRow = $(event.currentTarget);
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
                alertHolder.append(alert);
                content.empty();
                pageDetail.removeClass("hide");
                return;
            }
            
            content.html("<div class=\"loader\"></div>");
            pageDetail.removeClass("hide");

            this.util.getPageDetail(pageId).done((response) => {
                content.html(response);
                
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
                alertHolder.append(alert);
            });
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

    private removeFSPage() {
        const pageListEl = $(".page-list");
        if (pageListEl.length) {
            const listItems = pageListEl.children(".page-list-item");
            listItems.each((index, element) => {
                const listItemEl = $(element as Node as Element);
                if (listItemEl.text().toLocaleLowerCase() === "fs") {
                    listItemEl.remove();
                }
            });
        }
    }

    private removeFCPage() {
        const pageListEl = $(".page-list");
        if (pageListEl.length) {
            const listItems = pageListEl.children(".page-list-item");
            listItems.each((index, element) => {
                const listItemEl = $(element as Node as Element);
                if (listItemEl.text().toLocaleLowerCase() === "fc") {
                    listItemEl.remove();
                }
            });
        }
    }

    private addFSPage() {
        const pageListEl = $(".page-list");
        if (pageListEl.length) {
            const listItem = pageListEl.children(".page-list-item").first();
            const fcPageListItem = listItem.clone().text("FS");
            if (listItem.text().toLocaleLowerCase() === "fc") {
                listItem.after(fcPageListItem);
            } else {
                listItem.before(fcPageListItem);
            }
        }
    }

    private addFCPage() {
        const pageListEl = $(".page-list");
        if (pageListEl.length) {
            const listItem = pageListEl.children(".page-list-item").first();
            listItem.before(listItem.clone().text("FC"));
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
                    this.removeFCPage();
                }
                if (!fcCheckboxPrevState && fcCheckboxCurrentState) {
                    this.addFCPage();
                }
                fcCheckboxPrevState = fcCheckboxCurrentState;
            });
        specialPagesControlsEl.on("click",
            ".book-startpage-checkbox",
            () => {
                const fsCheckboxCurrentState = startpageCheckboxEl.prop("checked") as boolean;
                if (fsCheckboxPrevState && !fsCheckboxCurrentState) {
                    this.removeFSPage();
                }
                if (!fsCheckboxPrevState && fsCheckboxCurrentState) {
                    this.addFSPage();
                }
                fsCheckboxPrevState = fsCheckboxCurrentState;
            });
    }

  private startGeneration(listGenerator: PageListGenerator,
      listStructure: PageListStructure, doublePage:boolean) {
        const fromFieldValue = $("#project-pages-generate-from").val() as string;
        const toFieldValue = $("#project-pages-generate-to").val() as string;
        if (/\d+/.test(fromFieldValue) && /\d+/.test(toFieldValue)) {
            const from = parseInt(fromFieldValue);
            const to = parseInt(toFieldValue);
            const formatString = $("#project-pages-format").find(":selected").data("format-value") as string;
            const format = PageListFormat[formatString] as number;
            if (!isNaN(from) && !isNaN(to)) {
                if (to > from) {
                    const pageList = listGenerator.generatePageList(from, to, format, doublePage);
                    this.populateList(pageList, listStructure);
                    this.enableCheckboxes();
                    this.trackSpecialPagesCheckboxesState();
                } else {
                    this.gui.showInfoDialog("Warning", "Please swap to and from numbers.");
                }
            }
        } else {
            this.gui.showInfoDialog("Warning", "Please enter a number.");
        }
    }

    private populateList(pageList: string[], listStructure: PageListStructure) {
        const listContainerEl = $(".page-listing tbody");
        let position = 0;
        if (listContainerEl.length) {
            this.gui.showInfoDialog("Info", "Page names already exist for this project. Appending generated names to the end of the list.");
            position = listContainerEl.length;
        }

        for (let page of pageList) {
            const html =
                `<tr class="page-row" data-position="${position}">
                    <td class="ridics-checkbox">
                        <label>
                            <input type="checkbox" class="selection-checkbox">
                            <span class="cr cr-black">
                                <i class="cr-icon glyphicon glyphicon-ok"></i>
                            </span>
                        </label>
                    </td>
                    <td>
                        <div class="name">${page}</div>
                        <div class="alert alert-danger"></div>
                    </td>
                    <td class="buttons">
                        <a class="edit-page btn btn-sm btn-default">
                            <i class="fa fa-pencil"></i>
                        </a>
                        <a class="remove-page btn btn-sm btn-default">
                            <i class="fa fa-trash"></i>
                        </a>
                    </td>
                </tr>`;

            listContainerEl.append(html);
            position++;
        }

        this.showUnsavedChangesAlert();
        this.initPageRowClicks();
    }

    private moveList(down: boolean, context: JQuery) {
        const distance = Number(context.find(".page-move-distance").val());
        const pages = context.find(".page-row").toArray();
        const newPages = new Array<HTMLElement>(pages.length);
        
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

                $(page).data("position", newPosition+1);
                newPages[newPosition] = page;
            }
        }

        let j = 0;
        for (const page of pages) {
            const isSelected = $(page).find(".selection-checkbox").is(":checked");
            if (!isSelected) {
                while (true) {
                    if (typeof newPages[j] == "undefined") {
                        $(page).data("position", j+1);
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
}