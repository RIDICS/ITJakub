class KeyTableEditorBase {
    protected currentPage: number;
    protected numberOfItemsPerPage = 25;

    protected initPagination(itemsCount: number, itemsOnPage: number, callback : Function) {
        const pagination = new Pagination({
            container: $(".key-table-pagination"),
            pageClickCallback: (pageNumber) => {
                callback(pageNumber);
                this.currentPage = pageNumber;
            }
        });
        pagination.make(itemsCount, itemsOnPage);
    }

    protected makeSelectable(jEl: JQuery) {
        jEl.children(".page-list").on("click", ".page-list-item", (event) => {
            event.stopImmediatePropagation();
            const targetEl = $(event.target);
            if (targetEl.hasClass("collapse-category-button") || targetEl.parent().hasClass("collapse-category-button")) {
                return;
            }
            targetEl.toggleClass("page-list-item-selected");
            $(".page-list-item").not(targetEl).removeClass("page-list-item-selected");
        });
    }
}