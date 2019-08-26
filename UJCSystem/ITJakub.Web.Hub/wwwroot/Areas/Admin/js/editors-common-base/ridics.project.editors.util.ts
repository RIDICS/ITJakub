class EditorsUtil {
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

    savePageList(projectId: number, pageList: IPage[]): JQueryXHR {
        const pageAjax = $.post(`${this.serverPath}Admin/ContentEditor/SavePageList`,
            {
                projectId: projectId,
                pageList: pageList
            } as JQuery.PlainObject);
        return pageAjax;
    }

    getServerAddress(): string {
        return this.serverPath;
    }

    /**
 * Generates guid on the server
 * @returns {JQueryXHR} Ajax conraining GUID
 */
    createTextRefereceId(): string {
        const guid = Guid.generate();
        return guid;
    }

    getProjectContent(projectId: number): JQuery.jqXHR<ITextWithPage[]> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/GetProjectContent`,
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
* @param {Number} textId - Number of page for which to load plain text
* @returns {JQueryXHR} Ajax containing page plain text
*/
    loadPlainText(textId: number): JQuery.jqXHR<ITextWithContent> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/GetTextResource`,
            { textId: textId, format: TextFormatEnumContract.Raw } as JQuery.PlainObject);
        return ajax;
    }

    /**
* Loads markdown rendered to html from the server.
* @param {Number} textId  - Id of page for which to load rendered text
* @returns {JQueryXHR} Ajax query of rendered text
*/
    loadRenderedText(textId: number): JQuery.jqXHR<ITextWithContent> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/GetTextResource`,
            { textId: textId, format: TextFormatEnumContract.Html } as JQuery.PlainObject);
        return ajax;
    }

    savePlainText(textId: number, request: ICreateTextVersion): JQueryXHR {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/SetTextResource`,
            {
                textId: textId,
                request: request
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

    loadEditionNote(projectId: number): JQuery.jqXHR<string> {//TODO
        const format: TextFormatEnumContract = TextFormatEnumContract.Raw;
        const ajax = $.get(`${this.serverPath}Admin/ContentEditor/GetEditionNote`,
            {
                projectId: projectId,
                format: format
            } as JQuery.PlainObject);
        return ajax;
    }

    saveEditionNote(noteRequest: IEditionNote): JQuery.jqXHR {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/SetEditionNote`,
            noteRequest as JQuery.PlainObject);
        return ajax;
    }
}