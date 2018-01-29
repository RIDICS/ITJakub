class PageListEditorTextEditor {
    convertDivListToTextarea(jEl: JQuery) {
        const childrenEl = jEl.children(".page-list-item");
        const pageListContainerEl = jEl.parent(".page-list-container");
        jEl.removeClass("ui-selectable");
        var pageListString: string[] = [];
        childrenEl.each((index, element) => {
            const childEl = $(element);
            pageListString.push(`${childEl.text()}`);
        });
        const textAreaEl = `<textarea class="page-list-edit-textarea textarea-no-resize">${pageListString.join("\n")}</textarea>`;
        jEl.remove();
        pageListContainerEl.append(textAreaEl);
    }
}