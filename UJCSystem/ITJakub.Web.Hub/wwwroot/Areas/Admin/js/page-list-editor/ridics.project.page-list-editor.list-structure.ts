class PageListStructure {

    private generateListStructure(pageList: string[], jEl: JQuery, addHeaders: boolean) {
        const listStart = `<ul class="page-list">`;
        const listItemStart = `<li class="ui-widget-content page-list-item">`;
        const listItemEnd = `</li>`;
        const listEnd = "</ul>";
        var elm = "";
        if(addHeaders){elm += listStart;}
        for (let i = 0; i < pageList.length; i++) {
            elm += listItemStart;
            elm += pageList[i];
            elm += listItemEnd;
        }
        if(addHeaders){elm += listEnd;}
        const html = $.parseHTML(elm);
        jEl.append(html);
    }

    private makeSelectable(jEl: JQuery) {
        jEl.children(".page-list").selectable();
    }

    createList(pageList: string[], jEl: JQuery) {
        const addHeaders = true;
        this.generateListStructure(pageList, jEl, addHeaders);
        this.makeSelectable(jEl);
    }

    appendList(pageList: string[], jEl: JQuery) {
        const addHeaders = false;
        this.generateListStructure(pageList, jEl, addHeaders);
    }

    movePageUp(jEl: JQuery) {
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