class Util {
    private serverAddress = "localhost:2927"; //TODO debug

    getServerAddress(): string {//TODO debug
        return this.serverAddress;
    }

    /**
     * Generates guid on the server
     * @returns {string} GUID
     */
    getGuid(): string {
        let guid = "";
        $.ajaxSetup({ async: false }); // make async
        $.post(`http://${this.serverAddress}/admin/project/GetGuid`,
            {},
            (data: string) => { guid = data; });
        return guid;
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

    loadCommentFile(pageNumber: number): ICommentSctucture[] {
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
     * @returns {string} Plain text
    */
    loadPlainText(pageNumber: number): string { //TODO add logic
        if (pageNumber === 2) {
            return "$3844fc54-83cb-49f5-a6c9-63c55138bfb4%HIIIIIIIIIIII\nHHHHHHHHHHHHHHHHH\n%3844fc54-83cb-49f5-a6c9-63c55138bfb4$";
        } else
            return "Plain text\nAAAAAAAAAAAAAAAAAAAAAAA\nBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB\nCCCCCCCCCCCCCCCCCCCCCCCCCCC";
    }

    /**
     * Loads markdown rendered to html from the server.
     * @param pageNumber {Number} - Number of page for which to load rendered text
     * @returns {string} Rendered text
     */
    loadRenderedText(pageNumber: number): string { //TODO add logic
        if (pageNumber === 1) {
            return `<span id="60241999-6c28-4d9b-85df-c5901fc6e983-text">Comment to text</span><span id="4b877225-9456-403d-86a5-c7f8901572a5-text">Cras sit amet nibh libero,</span><span> in gravida nulla. <span id="1504620630028-text">Nulla</span> vel metus scelerisque ante sollicitudin commodo. Cras purus odio, vestibulum in vulputate at, tempus viverra turpis.</span>`;
        } else if (pageNumber === 2) {
            return "Lorem ipsum dolor sit amet, consectetur adipiscing elit.<br>Nullam vitae <span id=\"3844fc54-83cb-49f5-a6c9-63c55138bfb4-text\">posuere lectus</span>.<br>Vivamus vitae tincidunt eros, sit amet euismod lectus.<br>Donec in lorem venenatis, faucibus ligula faucibus, condimentum purus.";
        } else if (pageNumber === 3) {
            return "Lorem ipsum dolor sit amet, consectetur adipiscing elit.<br>Nullam vitae <span id=\"1f9ce475-5a28-433a-b07b-1547b5df25da-text\">posuere lectus</span>.<br>Vivamus vitae tincidunt eros, sit amet euismod lectus.<br>Donec in lorem venenatis, faucibus ligula faucibus, condimentum purus.";
        } else return "<div>hi</div>";
    }

    getNumberOfCommentsOnPage(pageNumber: number) {
        var numberOfComments = 0;
        $.post(`http://${this.serverAddress}/admin/project/GetCommentSectionNumberOfFiles`, //check what does async affect
            {
                pageNumber: pageNumber
            }).done((data: number) => {
                numberOfComments = data;
            }
            );
        return numberOfComments;
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
            console.log("no comments");
            return null;
        }
    }

    getNestedCommentsNumber = (pageNumber: number): number[] => {
        const content = this.parseLoadedCommentFiles(pageNumber);
        var nestedComments: number[] = [];
        var thread = 0;
        if (content !== null && typeof content !== "undefined") {
            let id = content[0].id;
            nestedComments[0] = 0;
            for (let i = 0; i < content.length; i++) {
                const currentId = content[i].id;
                if (currentId !== id) {
                    thread++;
                    id = currentId;
                    nestedComments[thread] = 0;
                }
                if (content[i].nested) {

                    nestedComments[thread]++;
                }
            }
            return nestedComments;
        } else {
            console.log(`No comments on page ${pageNumber}`);
            return null;
        }
    }

    fromJson(jsonString: string): ICommentSctucture {
        if (jsonString !== null) {
            const stringObject = JSON.parse(jsonString);
            const id: string = stringObject.id;
            const name: string = stringObject.name;
            const body: string = stringObject.body;
            const page: number = parseInt(stringObject.page);
            const order: number = parseInt(stringObject.order);
            const time: number = parseInt(stringObject.time);
            const nested: boolean = (stringObject.nested === "true");
            const result: ICommentSctucture = {
                id: id,
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