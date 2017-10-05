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
        const ajax = $.post(`http://${this.serverAddress}/admin/project/GetGuid`,
            {});
        return ajax;
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

    fromJson(jsonString: string): ICommentSctucture {
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
                picture: picture,
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