﻿class KeyTableEditorBase {
    protected currentPage: number;
    protected numberOfItemsPerPage = 14;
    protected createEntryButtonEl = $(".create-key-table-entry-description");
    protected changeEntryButtonEl = $(".rename-key-table-entry-description");
    protected deleteEntryButtonEl = $(".delete-key-table-entry-description");
    protected titleEl = $(".table-of-keys-title");
    protected listElement = $(".selectable-list-div");
    protected loaderElement = lv.create(null, "lv-circles sm lv-mid lvt-3");

    protected showLoading() {
        this.listElement.empty();
        this.listElement.append(this.loaderElement.getElement());
    }

    protected initPagination(itemsCount: number, itemsOnPage: number, callback : Function) {
        const pagination = new Pagination({
            container: document.getElementById("key-table-pagination") as HTMLDivElement,
            pageClickCallback: (pageNumber) => {
                this.showLoading();
                callback(pageNumber);
                this.currentPage = pageNumber;
            }
        });
        pagination.make(itemsCount, itemsOnPage);
    }

    protected makeSelectable(jEl: JQuery) {
        jEl.children(".list-group").on("click", ".page-list-item", (event) => {
            event.stopPropagation();
            var targetEl = $(event.target);
            if (targetEl.hasClass("collapse-category-button") || targetEl.parent().hasClass("collapse-category-button")) {
                return;
            }
            if (!targetEl.hasClass("page-list-item")) {
                targetEl = targetEl.closest(".page-list-item");
            }
            targetEl.toggleClass("page-list-item-selected active");
            $(".page-list-item").not(targetEl).removeClass("page-list-item-selected active");
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
        const listStart = `<div class="list-group">`;
        const listItemEnd = "</div>";
        const listEnd = "</div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < ids.length; i++) {
            const listItemStart =
                `<div class="page-list-item list-group-item" data-key-id="${ids[i]}">`;
            elm += listItemStart;
            elm += names[i];
            elm += listItemEnd;
        }
        elm += listEnd;
        return $(elm);
    }

    protected unbindEventsDialog() {
        $("#project-layout-content").on("hidden.bs.modal", event => {
            const targetDialogEl = $(event.target as Node as Element);
            targetDialogEl.find("*").off();
        });
    }
}