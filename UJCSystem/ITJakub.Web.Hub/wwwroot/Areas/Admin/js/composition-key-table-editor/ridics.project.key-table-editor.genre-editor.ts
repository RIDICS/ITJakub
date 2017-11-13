class KeyTableGenreEditor {
    private readonly commonUtil: EditorsUtil;
    private readonly util: KeyTableViewManager;

    constructor() {
        this.commonUtil = new EditorsUtil();//TODO move everything related to own util
        this.util = new KeyTableViewManager();
    }

    init() {
        this.commonUtil.getLitararyGenreList().done((data: IGenreResponseContract[]) => {
            console.log(data);
            const itemsOnPage = 2;//TODO debug
            this.initPagination(data.length, itemsOnPage);
            const listEl = $(".selectable-list-div");
            this.generateListStructure(data, listEl);
            this.makeSelectable(listEl);
            this.genreCreation();
        });
    }

    private initPagination(itemsCount: number, itemsOnPage: number) {
        const pagination = new Pagination({
            container: $(".key-table-pagination"),
            pageClickCallback: function (pageNumber) {//TODO
            }
        });
        pagination.make(itemsCount, itemsOnPage);
    }

    private generateListStructure(genreItemList: IGenreResponseContract[], jEl: JQuery) {
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
    }

    private makeSelectable(jEl: JQuery) {
        jEl.children(".page-list").selectable();
    }

    private genreCreation() {
        $(".crud-buttons-div").on("click", ".create-new-genre", () => {
            this.util.createNewGenre("New genre");//TODO add dialog
        });
    }
}