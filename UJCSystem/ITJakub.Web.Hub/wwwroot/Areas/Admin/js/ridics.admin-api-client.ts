class AdminApiClient extends WebHubApiClient {
    public getHtmlPageByUrl(url: string): JQuery.jqXHR<string> {
        return this.get(url);
    }

    public getOriginalAuthorTypeahead(query: string): JQuery.jqXHR<IOriginalAuthor[]> {
        return this.get(`${getBaseUrl()}Admin/Project/GetTypeaheadOriginalAuthor?query=${query}`);
    }

    public getResponsiblePersonTypeahead(query: string): JQuery.jqXHR<IResponsiblePerson[]> {
        return this.get(`${getBaseUrl()}Admin/Project/GetTypeaheadResponsiblePerson?query=${query}`);
    }

    public loadCommentFile(textId: number): JQuery.jqXHR<ICommentSctucture[]> {
        return this.post(`${getBaseUrl()}Admin/ContentEditor/LoadCommentFile`,
            JSON.stringify({
                 textId: textId
            }));
    }

    public createNewKeywordsByArray(names: string[]): JQuery.jqXHR<number[]> {
        const url = `${getBaseUrl()}Admin/Project/CreateKeywordsWithArray`;
        const id = 0; //keyword doesn't have an id yet
        const payload: IKeywordContract[] = [];
        for (let i = 0; i < names.length; i++) {
            payload.push(
                {
                    name: names[i],
                    id: id
                });
        };
        return $.post(url, { request: payload } as JQuery.PlainObject);
    }
}

class BasicApiClient extends WebHubApiClient {
    public basicSearchGetResultSnapshotListPageOfIdsWithoutResultNumbers(data: ICorpusListPageLookupBasicSearch): JQuery.jqXHR<ICoprusSearchSnapshotResult> {
        return this.post(
            `${getBaseUrl()}BohemianTextBank/BohemianTextBank/BasicSearchGetResultSnapshotListPageOfIdsWithoutResultNumbers`,
            JSON.stringify(data));
    }

    public advancedSearchGetResultSnapshotListPageOfIdsWithoutResultNumbers(data: ICorpusListPageLookupAdvancedSearch): JQuery.jqXHR<ICoprusSearchSnapshotResult> {
        return this.post(
            `${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetResultSnapshotListPageOfIdsWithoutResultNumbers`,
            JSON.stringify(data));
    }

    public advancedSearchGetResultSnapshotListPageOfIdsWithResultNumbers(data: ICorpusListPageLookupAdvancedSearch): JQuery.jqXHR<ICoprusSearchSnapshotResult> {
        return this.post(
            `${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetResultSnapshotListPageOfIdsWithResultNumbers`,
            JSON.stringify(data));
    }

    public basicSearchGetResultSnapshotListPageOfIdsWithResultNumbers(data: ICorpusListPageLookupBasicSearch): JQuery.jqXHR<ICoprusSearchSnapshotResult> {
        return this.post(
            `${getBaseUrl()}BohemianTextBank/BohemianTextBank/BasicSearchGetResultSnapshotListPageOfIdsWithResultNumbers`,
            JSON.stringify(data));
    }
}