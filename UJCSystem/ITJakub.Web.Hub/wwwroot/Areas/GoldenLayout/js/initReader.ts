function initGoldenReader(bookId: string,
    versionId: string,
    bookTitle: string,
    pageList: any,
    searchedText?: string,
    initPageId?: string) {


    function readerPageChangedCallback(pageId: number) {
        updateQueryStringParameter("page", pageId);
    }

    var readerPanels = [
        ReaderPanelEnum.TextPanel, ReaderPanelEnum.ImagePanel, ReaderPanelEnum.ContentPanel,
        ReaderPanelEnum.SearchPanel, ReaderPanelEnum.SettingsPanel
    ];
    var sc = new ServerCommunication();
    var readerPlugin = new DesktopReader(<any>$("#ReaderHeaderDiv")[0],
        readerPageChangedCallback,
        readerPanels,
        sc
    );
    readerPlugin.makeReader(bookId, versionId, bookTitle, pageList);

    var searchModule = new SearchModule(<any>$("#SearchDiv")[0], sc, readerPlugin, bookId, versionId);
    searchModule.initSearchModule(initPageId, searchedText);
}