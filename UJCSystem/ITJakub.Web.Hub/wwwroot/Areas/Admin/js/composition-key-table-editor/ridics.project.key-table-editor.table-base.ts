class KeyTableEditorBase {
    protected currentPage: number;
    protected numberOfItemsPerPage = Number($("#key-table-config").data("page-size"));
    protected createEntryButtonEl = $(".create-key-table-entry-description");
    protected deleteEntryButton = ".delete-key-table-entry-description";
    protected deleteEntryButtonEl = $(".delete-key-table-entry-description");
    protected changeEntryButton = ".rename-key-table-entry-description";
    protected changeEntryButtonEl = $(".rename-key-table-entry-description");
    protected titleEl = $(".table-of-keys-title");
    protected listElement = $(".key-table-div");
    protected loaderElement = lv.create(null, "lv-circles sm lv-mid lvt-1");

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

    protected translateButtons() {
        $(this.changeEntryButton).text(localization.translate("Change", "KeyTable").value);
        $(this.deleteEntryButton).text(localization.translate("Delete", "KeyTable").value);
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
        const listStart = `<div class="table-repsonsive"><table class="table table-hover"><tbody>`;
        const listItemEnd = "</tr>";
        const listEnd = "</tbody></table></div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < ids.length; i++) {
            const listItemStart =
                `<tr class="page-list-item" data-key-id="${ids[i]}">`;
            elm += listItemStart;
            elm += '<td><div class="empty-glyphicon"></div></td>';
            elm += '<td>' + names[i] + '</td>';
            const changeButton =
                `<td class="key-table-button-cell"><button type="button" class="btn btn-default rename-key-table-entry" data-target="${ids[i]}">
                <i class="fa fa-pencil" aria - hidden="true"> </i>
                <span class="rename-key-table-entry-description"> Rename table of keys entry </span>
                </button>`;
            elm += changeButton;
            const removeButton =
                `<button type="button" class="btn btn-default delete-key-table-entry separate-button" data-target="${ids[i]}">
                <i class="fa fa-trash-o" aria-hidden="true"></i>
                <span class="delete-key-table-entry-description">Delete table of keys entry</span>
                </button></td>`;
            elm += removeButton;
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