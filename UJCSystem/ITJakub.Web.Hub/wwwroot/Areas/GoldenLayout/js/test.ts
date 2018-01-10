function testFunction(bookXmlId: string,
    versionXmlId: string,
    bookTitle: string,
    pageList: any,
    searchedText?: string,
    initPageXmlId?: string) {


    function readerPageChangedCallback(pageId: number) {
        updateQueryStringParameter("page", pageId);
    }

    var readerPanels = [
        ReaderPanelEnum.TextPanel, ReaderPanelEnum.ImagePanel, ReaderPanelEnum.ContentPanel,
        ReaderPanelEnum.SearchPanel, ReaderPanelEnum.SettingsPanel
    ];
    var panelButtons = [PanelButtonEnum.Close, PanelButtonEnum.Pin, PanelButtonEnum.ToNewWindow];
    var readerPlugin = new TestReaderModule(<any>$("#ReaderDiv")[0],
        readerPageChangedCallback,
        readerPanels,
        panelButtons,
        panelButtons);
    readerPlugin.test(bookXmlId, versionXmlId, bookTitle, pageList);
}

class TestReaderModule extends ReaderModule {
    test(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList) {
        this.bookId = bookXmlId;
        this.versionId = versionXmlId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<BookPage>();
        this.pagesById = {};
        this.bookmarks = new Array<IBookmarkPosition>(pageList.length);
        this.leftSidePanels = new Array<SidePanel>();
        this.rightSidePanels = new Array<SidePanel>();

        for (var i = 0; i < pageList.length; i++) { //load pageList
            var page = pageList[i];
            var bookPageItem = new BookPage(page.id, page.name, page.position);

            this.pages.push(bookPageItem);
            this.pagesById[bookPageItem.pageId] = bookPageItem;
        }

        var readerText = this.getText();
        this.readerContainer.appendChild(readerText);
    }

    getText() : HTMLDivElement{
        var returnDiv: HTMLDivElement = document.createElement("div");
        var textPanel: TextPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
            textPanel = this.appendTextPanel(returnDiv);
        }
        return returnDiv;
    }
}