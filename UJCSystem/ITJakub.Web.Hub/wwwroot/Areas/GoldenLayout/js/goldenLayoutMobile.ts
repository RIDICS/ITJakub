class MobileReader extends ReaderLayout {
    public makeReader(bookId: string, versionId: string, bookTitle: string, pageList: IPage[]) {
        this.bookId = bookId;
        this.versionId = versionId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<BookPage>();
        this.pagesById = {};
        this.bookmarks = new Array<IBookmarkPosition>(pageList.length);
        this.toolPanels = new Array<ToolPanel>();
        this.contentViewPanels = new Array<ContentViewPanel>();

        for (var i = 0; i < pageList.length; i++) { //load pageList
            var page = pageList[i];
            var bookPageItem = new BookPage(page.id, page.name, page.position);

            this.pages.push(bookPageItem);
            this.pagesById[bookPageItem.pageId] = bookPageItem;
        }

        var bookHeader = new MobileHeader(this, this.sc, this.readerHeaderDiv, bookTitle);
        this.readerHeaderDiv.appendChild(bookHeader.getInnerHtml());

        this.readerLayout = this.initLayout();

        this.loadBookmarks();
        this.newFavoriteDialog.make();
        this.newFavoriteDialog.setSaveCallback(this.createBookmarks.bind(this));
    }

    initLayout(): GoldenLayout {
        var config = new LayoutConfiguration();
        var readerLayout = new GoldenLayout(config.goldenLayoutMobileConfig(), $('#ReaderBodyDiv'));
        readerLayout.registerComponent('toolTab',
            (container, state) => {
                switch (state.label) {
                case this.bookmarksPanelId:
                    container.getElement().append(this.createBookmarksPanel());
                    break;
                case this.termsResultId:
                    container.getElement().append(this.createTermsResultPanel());
                    break;
                case this.termsSearchId:
                    container.getElement().append(this.createTermsSearchPanel());
                    break;
                case this.contentPanelId:
                    container.getElement().append(this.createContentPanel());
                    break;
                case this.searchPanelId:
                    container.getElement().append(this.createSearchPanel());
                    break;
                default:
                    break;
                }
            });
        readerLayout.registerComponent('viewTab',
            (container, state) => {
                switch (state.label) {
                case this.audioPanelId:
                    container.getElement().append(this.createAudioPanel());
                    break;
                case this.imagePanelId:
                    container.getElement().append(this.createImagePanel());
                    break;
                case this.textPanelId:
                    container.getElement().append(this.createTextPanel());
                    break;
                default:
                    break;
                }
            });
        readerLayout.init();
        readerLayout.on("stateChanged",
            () => {
                this.moveToPageNumber(this.actualPageIndex, true);
            });
        $(window).resize(() => {
            readerLayout.updateSize();
        });
        return readerLayout;
    }
}