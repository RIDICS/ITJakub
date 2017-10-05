class Util {
    private serverAddress = "localhost:2927"; //TODO debug

    getServerAddress(): string { //TODO debug
        return this.serverAddress;
    }

    /**
 * Generates guid on the server
 * @returns {JQueryXHR} Ajax conraining GUID
 */
    getGuid(): JQueryXHR {
        const ajax=$.post(`http://${this.serverAddress}/admin/project/GetGuid`,
            {});
        return ajax;
    }

    /**
     * Loads contents of files with comments in a page from the server.
     * @param {Number} pageNumber - Number of page for which to load comment file contents
     * @returns {string[][]} Array of threads containing comments in a page
     */
    private loadCommentFileString(pageNumber: number): string[] {
        let fileContent: string[];
        $.ajaxSetup({ async: false }); // make async
        $.post(`http://${this.serverAddress}/admin/project/LoadCommentFile`,
            { pageNumber: pageNumber },
            (data: string[]) => { fileContent = data; });
        if (fileContent[0] === "error-no-file") {
            return null;
        } else
            return fileContent;
    }

    private loadCommentFile(pageNumber: number): ICommentSctucture[] {
        const contentStringArray = this.loadCommentFileString(pageNumber);
        if (contentStringArray !== null && typeof contentStringArray !== "undefined") {
            let commentsParsed: ICommentSctucture[] = [];
            for (let i = 0; i < contentStringArray.length; i++) {
                commentsParsed[i] = this.fromJson(contentStringArray[i]);
            }
            return commentsParsed;
        } else return null;
    }

    /**
* Loads plain text with markdown from the server.
* @param {Number} pageNumber - Number of page for which to load plain text
* @returns {JQueryXHR} Ajax containing page plain text
*/
    loadPlainText(pageNumber: number): JQueryXHR {
        const ajax = $.post(`http://${this.serverAddress}/admin/project/LoadPlaintextCompositionPage`,
            { pageNumber: pageNumber });
            return ajax;
    }

    /**
* Gets number of pages in a composition from the server.
* @param {string} compositionId - Number of page for which to load plain text
* @returns {JQueryXHR} Ajax containing number of pages
*/
    getNumberOfPages(compositionId: string): JQueryXHR {
        const ajax = $.post(`http://${this.serverAddress}/admin/project/GetNumberOfPages`,
            { compositionId: compositionId });
        return ajax;
    }

    /**
 * Loads markdown rendered to html from the server.
 * @param {Number} pageNumber  - Number of page for which to load rendered text
 * @returns {JQueryXHR} Ajax query of rendered text
 */
    loadRenderedText(pageNumber: number): JQueryXHR {
        let pageContent: string;
        const ajax = $.post(`http://${this.serverAddress}/admin/project/LoadRenderedCompositionPage`,
            { pageNumber: pageNumber },
            (data: string) => { pageContent = data; });
        return ajax;
    }

    /**
* Loads comments from server, sorts them by id, executes split array function. Returns sorted array of comments.
* @param {Number} pageNumber  - Number of a page in composition
* @returns {ICommentSctucture[]} - Sorted comment contents
*/
    parseLoadedCommentFiles(pageNumber: number): ICommentSctucture[] {
        const content = this.loadCommentFile(pageNumber);
        if (content !== null && typeof content !== "undefined") {
            content.sort((a, b) => { //sort by id, ascending
                if (a.id < b.id) {
                    return -1;
                }
                if (a.id === b.id) {
                    return 0;
                }
                if (a.id > b.id) {
                    return 1;
                }
            });

            let id = content[0].id;
            const indexes: number[] = [];
            for (let i = 0; i < content.length; i++) {
                const currentId = content[i].id;
                if (currentId !== id) {
                    indexes.push(i);
                    id = currentId;
                }
            }
            const sortedContent = this.splitArrayToArrays(content, indexes);
            return sortedContent;
        } else return null;
    }

    /**
     * Receives array of comments sorted by id. Splits it into arrays with same id. Sorts every array and rejoins them into one array.
     * @param {ICommentSctucture[]} content - Array of comments.
     * @param {number[]} indexes - Array of indexes where to split comment array.
     * @returns {ICommentSctucture[]}
     */
    private splitArrayToArrays(content: ICommentSctucture[], indexes: number[]): ICommentSctucture[] {
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

    getCommentIds(content: ICommentSctucture[]): string[] {
        if (content.length > 0) {
            let id = content[0].id;
            const ids: string[] = [];
            ids.push(id);
            for (let i = 0; i < content.length; i++) {
                const currentId = content[i].id;
                if (id !== currentId) {
                    ids.push(currentId);
                    id = currentId;
                }
            }
            return ids;
        } else {
            console.log("no comments");//TODO debug
            return null;
        }
    }

    private fromJson(jsonString: string): ICommentSctucture {
        if (jsonString !== null) {
            const stringObject = JSON.parse(jsonString);
            const id: string = stringObject.id;
            const picture = stringObject.picture;
            const name: string = stringObject.name;
            const body: string = stringObject.body;
            const page: number = parseInt(stringObject.page);
            const order: number = parseInt(stringObject.order);
            const time: number = parseInt(stringObject.time);
            const nested: boolean = (stringObject.nested === "true");
            const result: ICommentSctucture = {
                id: id,
                picture : picture,
                name: name,
                body: body,
                page: page,
                order: order,
                time: time,
                nested: nested
            };
            return result;
        } else return null;
    }

}