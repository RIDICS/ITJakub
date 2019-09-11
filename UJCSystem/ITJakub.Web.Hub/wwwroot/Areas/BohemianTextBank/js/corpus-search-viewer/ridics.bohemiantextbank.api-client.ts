class TextBankApiClient extends WebHubApiClient {
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