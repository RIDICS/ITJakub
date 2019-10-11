class AdminApiClient extends WebHubApiClient {
    public getOriginalAuthorTypeahead(query: string): JQuery.jqXHR<IOriginalAuthor[]> {
        return this.get(`${getBaseUrl()}Admin/Project/GetTypeaheadOriginalAuthor?query=${query}`);
    }

    public getResponsiblePersonTypeahead(query: string): JQuery.jqXHR<IResponsiblePerson[]> {
        return this.get(`${getBaseUrl()}Admin/Project/GetTypeaheadResponsiblePerson?query=${query}`);
    }

    public loadCommentFile(textId: number): JQuery.jqXHR<ICommentSctucture[]> {
        return this.get(`${getBaseUrl()}Admin/ContentEditor/LoadCommentFile?textId=${textId}`);
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
