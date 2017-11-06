class PageListEditorUtil {
private serverPath = getBaseUrl();

    getPagesList(projectId: number):JQueryXHR {
        const pageListAjax = $.get(`${this.serverPath}admin/project/GetPageList`,
            {
                projectId: projectId
            });
        return pageListAjax;
}

    savePageList(pageList: string[]): JQueryXHR {
        const pageAjax = $.post(`${this.serverPath}admin/project/SavePageList`,
            {
                pageList: pageList
            });
        return pageAjax;
    }
}