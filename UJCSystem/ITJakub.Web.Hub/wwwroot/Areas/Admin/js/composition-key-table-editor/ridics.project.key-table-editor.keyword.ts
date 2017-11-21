class KeyTableKeywordEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private keywordItemList: IKeywordContract[];
    private readonly gui: EditorsGui;

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new keyword");
        $(".rename-key-table-entry").text("Rename keyword");
        $(".delete-key-table-entry").text("Delete keyword");
        this.util.getKeywordList().done((data: IKeywordContract[]) => {
            this.keywordItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.keywordRename();
            this.keywordDelete();
            this.keywordCreation();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    };

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const splitArray = this.splitArray(this.keywordItemList, pageNumber);
        listEl.empty();
        const generatedListStructure = this.generateKeywordList(splitArray, listEl);
        listEl.append(generatedListStructure);
        this.makeSelectable(listEl);
    }

    private updateContentAfterChange() {
        this.util.getKeywordList().done((data: IKeywordContract[]) => {
            this.keywordItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog("Warning", "Connection to server lost.\nAutomatic page reload is not possible.");
        });
    }

    protected generateKeywordList(keywordItemList: IKeywordContract[], jEl: JQuery): JQuery {
        const nameArray = keywordItemList.map(a => a.name);
        const idArray = keywordItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private keywordCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showSingleInputDialog("Name input", "Please input new keyword:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const keywordString = textareaEl.val();
                        const newKeywordAjax = this.util.createNewKeyword(keywordString);
                        newKeywordAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog("Success", "New keyword has been created");
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newKeywordAjax.fail(() => {
                            this.gui.showInfoDialog("Error", "New keyword has not been created");
                            $(".info-dialog-ok-button").off();
                        });
                    });
            });
    }

    private keywordRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").children(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showSingleInputDialog("Name input", "Please input new keyword name:");
                    const textareaEl = $(".input-dialog-textarea");
                    const originalText = selectedPageEl.text();
                    textareaEl.val(originalText);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const keywordName = textareaEl.val();
                            const keywordId = selectedPageEl.data("key-id") as number;
                            const renameAjax = this.util.renameKeyword(keywordId, keywordName);
                            renameAjax.done(() => {
                                textareaEl.val("");
                                this.gui.showInfoDialog("Success", "Keyword has been renamed");
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog("Error", "Keyword has not been renamed");
                                $(".info-dialog-ok-button").off();
                            });
                    });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a keyword");
                }
            });
    }

    private keywordDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this keyword?");
                $(".confirmation-ok-button").on("click",
                    () => {
                            const id = selectedPageEl.data("key-id") as number;
                            const deleteAjax = this.util.deleteKeyword(id);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Success", "Keyword deletion was successful");
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Error", "Keyword deletion was not successful");
                            });
                    });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a keyword");
                }
            });
    }
}