class PageListStructure {

    private generateListStructure(pageList: string[], jEl: JQuery) {
        const listStart = `<ul class="page-list">`;
        const listItemStart = `<li class="ui-widget-content page-list-item">`;
        const listItemEnd = `</li>`;
        const listEnd = "</ul>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < pageList.length; i++) {
            elm += listItemStart;
            elm += pageList[i];
            elm += listItemEnd;
        }
        elm += listEnd;
        const html = $.parseHTML(elm);
        jEl.append(html);
    }

    private makeSelectable(jEl: JQuery) {
        jEl.children(".page-list").selectable();
    }

    createList(pageList: string[], jEl: JQuery) {
        this.generateListStructure(pageList, jEl);
        this.makeSelectable(jEl);
    }

    movePageUp(jEl: JQuery) {//TODO multiple page selection
        const prevPageEl = jEl.prev(".page-list-item");
        if (prevPageEl.length) {
            if (!prevPageEl.hasClass("ui-selected")) {
                jEl.detach();
                prevPageEl.before(jEl);
            }
        }
    }

    movePageDown(jEl: JQuery) {
        const nextPageEl = jEl.next(".page-list-item");
        if (nextPageEl.length) {
            if (!nextPageEl.hasClass("ui-selected")) {
                jEl.detach();
                nextPageEl.after(jEl);
            }
        }
    }
}