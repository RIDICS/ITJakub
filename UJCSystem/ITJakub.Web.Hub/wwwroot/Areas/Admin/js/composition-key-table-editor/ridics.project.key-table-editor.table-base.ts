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
            event.stopPropagation();
            var targetEl = $(event.target);
            if (targetEl.hasClass("collapse-category-button") || targetEl.parent().hasClass("collapse-category-button")) {
                return;
            }
            if (!targetEl.hasClass("page-list-item")) {
                targetEl = targetEl.closest(".page-list-item");
            }
            targetEl.toggleClass("page-list-item-selected");
            $(".page-list-item").not(targetEl).removeClass("page-list-item-selected");
        });
    }

    protected splitArray(genreItemList: any[], page: number): any[] {
        const numberOfListItemsPerPage = this.numberOfItemsPerPage;
        const startIndex = (page - 1) * numberOfListItemsPerPage;
        const endIndex = page * numberOfListItemsPerPage;
        const splitArray = genreItemList.slice(startIndex, endIndex);
        return splitArray;
    }

    protected generateSimpleList(ids: number[], names:string[], jEl: JQuery): JQuery {
        const listStart = `<div class="page-list">`;
        const listItemEnd = "</div>";
        const listEnd = "</div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < ids.length; i++) {
            const listItemStart =
                `<div class="page-list-item" data-key-id="${ids[i]}">`;
            elm += listItemStart;
            elm += names[i];
            elm += listItemEnd;
        }
        elm += listEnd;
        const html = $.parseHTML(elm);
        return $(html);
    }
}