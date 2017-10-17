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

    getProjectContent(projectId: number) {
        const ajax = $.post(`http://${this.serverAddress}/admin/project/GetProjectContent`,
            {
                projectId: projectId
            });
        return ajax;
    }


    /**
* Loads plain text with markdown from the server.
* @param {Number} pageNumber - Number of page for which to load plain text
* @returns {JQueryXHR} Ajax containing page plain text
*/
    loadPlainText(textId: number): JQueryXHR {
        const ajax = $.post(`http://${this.serverAddress}/admin/project/GetTextResource`,
            { textId: textId, format: "Raw" });
        return ajax;
    }

    /**
* Loads markdown rendered to html from the server.
* @param {Number} textId  - Id of page for which to load rendered text
* @returns {JQueryXHR} Ajax query of rendered text
*/
    loadRenderedText(textId: number): JQueryXHR {
        const ajax = $.post(`http://${this.serverAddress}/admin/project/GetTextResource`, { textId: textId, format: "Html"});
        return ajax;
    }

    savePlainText(body: string, textId: number){//TODO finish
        const plainText = this.loadPlainText(textId);
        plainText.done((data: IPageText) => {
            const id = data.id;
            const versionNumber = data.versionNumber;
            const request = {
                Id: id,
                Text: body,
                VersionNumber: versionNumber//TODO
        };
            const ajax = $.post(`http://${this.serverAddress}/admin/project/SetTextResource`,
                {
                    textId: textId,
                    request: request
                });
            ajax.done(() => { console.log("saved succesfully"); });

        });
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

    commentFromJson(jsonString: string): ICommentSctucture {
        if (jsonString !== null) {
            const stringObject = JSON.parse(jsonString);
            const id: string = stringObject.id;
            const picture = stringObject.picture;
            const name: string = stringObject.name;
            const surname: string = stringObject.surname;
            const body: string = stringObject.body;
            const page: number = parseInt(stringObject.page);
            const order: number = parseInt(stringObject.order);
            const time: number = parseInt(stringObject.time);
            const nested: boolean = (stringObject.nested === "true");
            const result: ICommentSctucture = {
                id: id,
                picture: picture,
                name: name,
                surname: surname,
                body: body,
                page: page,
                order: order,
                time: time,
                nested: nested
            };
            return result;
        } else return null;
    }

    compositionPageFromJson(jsonString: string): ITextProjectPage {
//TODO write logic
        return null;
    }

}