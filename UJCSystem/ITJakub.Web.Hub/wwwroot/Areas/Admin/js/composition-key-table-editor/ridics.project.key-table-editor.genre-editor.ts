class KeyTableGenreEditor {
    private readonly commonUtil: EditorsUtil;
    private readonly util: KeyTableViewManager;
    private readonly gui: EditorsGui;
    private genreItemList: IGenreResponseContract[];
    private numberOfItemsPerPage = 28;

    constructor() {
        this.commonUtil = new EditorsUtil();//TODO move everything related to own util
        this.util = new KeyTableViewManager();
        this.gui = new EditorsGui();
    }

    init() {
        this.commonUtil.getLitararyGenreList().done((data: IGenreResponseContract[]) => {
            this.genreItemList = data;
            console.log(data);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage);
            this.loadPage(1);
            this.genreRename();
            this.genreDelete();
        });
        this.genreCreation();
    }

    private initPagination(itemsCount: number, itemsOnPage: number) {
        const pagination = new Pagination({
            container: $(".key-table-pagination"),
            pageClickCallback: (pageNumber) => {
                this.loadPage(pageNumber);
            }
        });
        pagination.make(itemsCount, itemsOnPage);
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const splitArray = this.splitGenreArray(this.genreItemList, pageNumber);
        this.generateListStructure(splitArray, listEl);
    }

    private splitGenreArray(genreItemList: IGenreResponseContract[], page: number): IGenreResponseContract[] {
        const numberOfListItemsPerPage = this.numberOfItemsPerPage;
        const startIndex = (page - 1) * numberOfListItemsPerPage;
        const endIndex = page * numberOfListItemsPerPage;
        const splitArray = genreItemList.slice(startIndex, endIndex);
        return splitArray;
    }

    private generateListStructure(genreItemList: IGenreResponseContract[], jEl: JQuery) {
        jEl.empty();
        const listStart = `<ul class="page-list">`;
        const listItemEnd = `</li>`;
        const listEnd = "</ul>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < genreItemList.length; i++) {
            const listItemStart = `<li class="ui-widget-content page-list-item" data-genre-id="${genreItemList[i].id}">`;
            elm += listItemStart;
            elm += genreItemList[i].name;
            elm += listItemEnd;
        }
        elm += listEnd;
        const html = $.parseHTML(elm);
        jEl.append(html);
        this.makeSelectable(jEl);
    }

    private makeSelectable(jEl: JQuery) {
        jEl.children(".page-list").selectable();
    }

    private genreCreation() {
        $(".crud-buttons-div").on("click", ".create-new-genre", () => {
            this.gui.showInputDialog("Name input", "Please input new genre name:");
            $(".info-dialog-ok-button").on("click", () => {
                const textareaEl = $(".input-dialog-textarea");
                const genreString = textareaEl.val();
                const newGenreAjax = this.util.createNewGenre(genreString);
                newGenreAjax.done(() => {
                    textareaEl.val("");
                    this.gui.showInfoDialog("Success", "New genre has been created");
                    $(".info-dialog-ok-button").off();
                });
                newGenreAjax.fail(() => {
                    this.gui.showInfoDialog("Error", "New genre has not been created");
                    $(".info-dialog-ok-button").off();
                });
            });
        });
    }

    private genreRename() {
        $(".crud-buttons-div").on("click", ".rename-genre", () => {
            this.gui.showInputDialog("Name input", "Please input genre name after rename:");
            $(".info-dialog-ok-button").on("click", () => {
                const textareaEl = $(".input-dialog-textarea");
                const genreString = textareaEl.val();
                const selectedPageEl = $(".page-list").children(".ui-selected");
                if (selectedPageEl.length) {
                    const id = selectedPageEl.data("genre-id") as number;
                    console.log(id);
                    const renameAjax = this.util.renameGenre(genreString, id);
                    renameAjax.done(() => {
                        textareaEl.val("");
                        this.gui.showInfoDialog("Success", "Genre has been renamed");
                        $(".info-dialog-ok-button").off();
                    });
                    renameAjax.fail(() => {
                        this.gui.showInfoDialog("Error", "Genre has not been renamed");
                        $(".info-dialog-ok-button").off();
                    });
                }
            });
        });
    }
    private genreDelete() {
        $(".crud-buttons-div").on("click", ".delete-genre", () => {
            this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this genre?");
            //TODO add logic on ok button
        });
    }
}