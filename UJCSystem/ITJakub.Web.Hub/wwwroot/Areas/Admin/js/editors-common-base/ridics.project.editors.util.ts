class EditorsUtil {
    private serverPath = getBaseUrl();

    getPagesList(projectId: number): JQueryXHR {
        const pageListAjax = $.get(`${this.serverPath}admin/project/GetPageList`,
            {
                projectId: projectId
            });
        return pageListAjax;
    }

    getImageOnPage(pageId: number, callbackSuccess: Function, callbackFail: Function) {
        const params = "pageId="+pageId;
        const xhr = new XMLHttpRequest();
        xhr.open("POST", `${this.serverPath}admin/project/GetPageImage`);
        xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        xhr.responseType = "blob";
        xhr.send(params); 
        xhr.onreadystatechange = function (this) {
            if (this.readyState === 4 && this.status === 200) {
                console.log(this.response, typeof this.response);
                callbackSuccess(this.response);
            }
            if (this.readyState === 4 && this.status !== 200) {
                console.log(this.response, typeof this.response);
                callbackFail(this.response);
            }
        }
    }

    savePageList(pageList: string[]): JQueryXHR {
        const pageAjax = $.post(`${this.serverPath}admin/project/SavePageList`,
            {
                pageList: pageList
            });
        return pageAjax;
    }

    getServerAddress(): string {
        return this.serverPath;
    }

    /**
 * Generates guid on the server
 * @returns {JQueryXHR} Ajax conraining GUID
 */
    createTextRefereceId(): JQueryXHR {
        const ajax = $.post(`${this.serverPath}admin/project/GetGuid`);
        return ajax;
    }

    getProjectContent(projectId: number): JQueryXHR {
        const ajax = $.post(`${this.serverPath}admin/project/GetProjectContent`,
            {
                projectId: projectId
            });
        return ajax;
    }

    deleteComment(commentId: number): JQueryXHR {
        const ajax = $.post(`${this.serverPath}admin/project/DeleteComment`,
            {
                commentId: commentId
            });
        return ajax;
    }

    /**
* Loads plain text with markdown from the server.
* @param {Number} textId - Number of page for which to load plain text
* @returns {JQueryXHR} Ajax containing page plain text
*/
    loadPlainText(textId: number): JQueryXHR {
        const ajax = $.post(`${this.serverPath}admin/project/GetTextResource`,
            { textId: textId, format: "Raw" });
        return ajax;
    }

    /**
* Loads markdown rendered to html from the server.
* @param {Number} textId  - Id of page for which to load rendered text
* @returns {JQueryXHR} Ajax query of rendered text
*/
    loadRenderedText(textId: number): JQueryXHR {
        const ajax = $.post(`${this.serverPath}admin/project/GetTextResource`, { textId: textId, format: "Html" });
        return ajax;
    }

    savePlainText(textId: number, request: IPageTextBase): JQueryXHR {
        const ajax = $.post(`${this.serverPath}admin/project/SetTextResource`,
            {
                textId: textId,
                request: request
            });
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
}