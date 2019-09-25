class EditorsApiClient {
    private serverPath = getBaseUrl();

    getPagesList(projectId: number): JQuery.jqXHR<IPage[]> {
        const pageListAjax = $.get(`${this.serverPath}Admin/ContentEditor/GetPageList`,
            {
                projectId: projectId
            } as JQuery.PlainObject);
        return pageListAjax;
    }

    getImageUrlOnPage(pageId: number): string {
        return `${this.serverPath}Admin/ContentEditor/GetPageImage?pageId=${pageId}`;
    }


    getPageDetail(pageId: number): JQuery.jqXHR<string> {
        return  $.get(`${this.serverPath}Admin/ContentEditor/GetPageDetail?pageId=${pageId}`);
    }

    savePageList(projectId: number, pageList: IUpdatePage[]): JQuery.jqXHR {
        return $.post(`${this.serverPath}Admin/ContentEditor/SavePageList`,
            {
                projectId: projectId,
                pageList: pageList
            } as JQuery.PlainObject);
    }

    getServerAddress(): string {
        return this.serverPath;
    }
    
    getTextPages(projectId: number): JQuery.jqXHR<ITextWithPage[]> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/GetTextPages`,
            {
                projectId: projectId
            } as JQuery.PlainObject);
        return ajax;
    }

    deleteComment(commentId: number): JQueryXHR {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/DeleteComment`,
            {
                commentId: commentId
            } as JQuery.PlainObject);
        return ajax;
    }

    /**
* Loads plain text with markdown from the server.
* @param {Number} pageId - Id of page for which to load plain text
* @returns {JQueryXHR} Ajax containing page plain text
*/
    loadPlainText(pageId: number): JQuery.jqXHR<ITextWithContent> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/GetTextResourceByPageId`,
            { pageId: pageId, format: TextFormatEnumContract.Raw } as JQuery.PlainObject);
        return ajax;
    }

    /**
* Loads markdown rendered to html from the server.
* @param {Number} pageId  - Id of page for which to load rendered text
* @returns {JQueryXHR} Ajax query of rendered text
*/
    loadRenderedText(pageId: number): JQuery.jqXHR<ITextWithContent> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/GetTextResourceByPageId`,
            { pageId: pageId, format: TextFormatEnumContract.Html } as JQuery.PlainObject);
        return ajax;
    }

    savePlainText(textId: number, request: ICreateTextVersion, mode: SaveTextModeType): JQuery.jqXHR<ISaveTextResponse> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/SetTextResource`,
            {
                textId: textId,
                request: request,
                mode: mode,
            } as JQuery.PlainObject);
        return ajax;
    }

    createTextOnPage(pageId: number): JQuery.jqXHR<number> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/CreateTextResource`,
            {
                pageId: pageId,
            } as JQuery.PlainObject);
        return ajax;
    }

    /**
     * Receives array of comments sorted by id. Splits it into arrays with same id. Sorts every array and rejoins them into one array.
     * @param {ICommentSctucture[]} content - Array of comments.
     * @param {number[]} indexes - Array of indexes where to split comment array.
     * @returns {ICommentSctucture[]}
     */
    splitArrayToArrays(content: ICommentSctucture[], indexes: number[]): ICommentSctucture[] {
        let result: ICommentSctucture[] = [];
        let beginIndex = 0;
        let endIndex = 0;
        for (let i = 0; i <= indexes.length; i++) {
            if (i === indexes.length) {
                endIndex = content.length;
            } else {
                endIndex = indexes[i];
            }
            const tempArray = content.slice(beginIndex, endIndex);
            tempArray.sort((a, b) => { //sort by boolean nested, false comes first
                return (a.nested === b.nested) ? 0 : a.nested ? 1 : -1;
            });
            tempArray.sort((a, b) => { //sort by nested comment order, ascending, as integer
                return a.order - b.order;
            });
            if (result.length) {
                result = result.concat(tempArray);
            } else {
                result = tempArray;
            }

            beginIndex = endIndex;
        }
        return result;
    }

    loadEditionNote(projectId: number): JQuery.jqXHR<IEditionNoteContract> {
        const format: TextFormatEnumContract = TextFormatEnumContract.Raw;
        const ajax = $.get(`${this.serverPath}Admin/ContentEditor/GetEditionNote`,
            {
                projectId: projectId,
                format: format
            } as JQuery.PlainObject);
        return ajax;
    }

    saveEditionNote(noteRequest: ICreateEditionNote): JQuery.jqXHR<number> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/SetEditionNote`,
            noteRequest as JQuery.PlainObject);
        return ajax;
    }

    createTextReferenceId(textId: number): JQuery.jqXHR<string> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/GenerateCommentId`, { textId: textId } as JQuery.PlainObject);
        return ajax;
    }
}