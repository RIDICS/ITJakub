class PageListEditorUtil {
private serverPath = getBaseUrl();

    getPagesList(projectId: number):JQueryXHR {
        const pageListAjax = $.post(`${this.serverPath}admin/project/GetPageList`,
            {
                projectId: projectId
            });
        return pageListAjax;
    }
}