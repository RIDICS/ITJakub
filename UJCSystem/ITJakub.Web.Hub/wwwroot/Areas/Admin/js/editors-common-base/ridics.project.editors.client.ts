class EditorsApiClient {
    private serverPath = getBaseUrl();

    getPagesList(projectId: number): JQuery.jqXHR<IPage[]> {
        const pageListAjax = $.get(`${this.serverPath}Admin/ContentEditor/GetPageList`,
            {
                projectId: projectId
            } as JQuery.PlainObject);
        return pageListAjax;
    }

    getImagesPageListView(projectId: number): JQuery.jqXHR<string> {
        return $.get(`${this.serverPath}Admin/Project/ImagesPageList?projectId=${projectId}`);
    }    

    getImageResourceByPageId(pageId: number): JQuery.jqXHR<IImageContract> {
        return $.get(`${this.serverPath}Admin/ContentEditor/GetImageResourceByPageId?pageId=${pageId}`);
    }

    getTermsByPageId(pageId: number): JQuery.jqXHR<string> {
        return $.get(`${this.serverPath}Admin/ContentEditor/GetPageTermList?pageId=${pageId}`);
    }

    setTerms(pageId: number, termIds: number[]): JQuery.jqXHR<string> {
        return $.post(`${this.serverPath}Admin/ContentEditor/SetTerms`,
            {
                pageId: pageId,
                termIds: termIds
            } as JQuery.PlainObject);
    }
    
    getPageDetail(pageId: number): JQuery.jqXHR<string> {
        return $.get(`${this.serverPath}Admin/ContentEditor/GetPageDetail?pageId=${pageId}`);
    }

    getPageListView(projectId: number): JQuery.jqXHR<string> {
        return $.get(`${this.serverPath}Admin/Project/PageList?projectId=${projectId}`);
    }
    
    createPage(projectId: number, name: string, position: number): JQueryXHR {
        return $.post(`${this.serverPath}Admin/ContentEditor/CreatePage`,
            {
                projectId: projectId,
                name: name,
                position: position
            } as JQuery.PlainObject
        );
    }
    
    savePageList(projectId: number, pageList: IUpdatePage[]): JQuery.jqXHR {
        return $.post(`${this.serverPath}Admin/ContentEditor/SavePageList`,
            {
                projectId: projectId,
                pageList: pageList
            } as JQuery.PlainObject);
    }

    getChapterListView(projectId: number): JQuery.jqXHR<string> {
        return $.get(`${this.serverPath}Admin/Project/ChapterList?projectId=${projectId}`);
    }

    generateChapterList(projectId: number): JQuery.jqXHR<ITextWithPage[]> {
        return  $.post(`${this.serverPath}Admin/ContentEditor/GenerateChapters`,
            {
                projectId: projectId
            } as JQuery.PlainObject);
    }
    
    saveChapterList(projectId: number, chapterList: IUpdateChapter[]): JQuery.jqXHR {
        return $.ajax({
            url: `${this.serverPath}Admin/ContentEditor/UpdateChapterList`,
            type: "POST",
            data: JSON.stringify({
                projectId: projectId,
                chapterList: chapterList
            }),
            contentType: "application/json; charset=utf-8",
            cache: false,
            async: true,
            dataType: "json"
        });
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

    createComment(textId: number, comment: ICommentStructureReply): JQueryXHR {
        return $.post(`${this.serverPath}Admin/ContentEditor/SaveComment`,
            {
                comment: comment,
                textId: textId
            } as JQuery.PlainObject
        );
    }

    editComment(commentId: number, comment: ICommentStructureReply): JQueryXHR {
        return $.post(`${this.serverPath}Admin/ContentEditor/UpdateComment`,
            {
                comment: comment,
                commentId: commentId
            } as JQuery.PlainObject
        );
    }

    deleteComment(commentId: number): JQuery.jqXHR {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/DeleteComment`,
            {
                commentId: commentId
            } as JQuery.PlainObject);
        return ajax;
    }

    deleteRootComment(commentId: number): JQuery.jqXHR<IDeleteRootCommentResponse> {
        const ajax = $.post(`${this.serverPath}Admin/ContentEditor/DeleteRootComment`,
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