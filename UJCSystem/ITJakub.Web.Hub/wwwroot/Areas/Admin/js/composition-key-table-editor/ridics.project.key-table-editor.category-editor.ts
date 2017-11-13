class KeyTableCategoryEditor {
    private readonly util: EditorsUtil;

    constructor() {
        this.util = new EditorsUtil();
    }

    init() {
        this.util.getCategoryList().done((data) => {
            this.initPagination();
            console.log(data);
        });
    };

    private initPagination() {
        const pagination = new Pagination({
            container: $(".key-table-pagination"),
            pageClickCallback: function (pageNumber) { }
        });
        const itemsOnPage = 1;
        const itemsCount = 100;//TODO debug
        pagination.make(itemsCount, itemsOnPage);
    }
}