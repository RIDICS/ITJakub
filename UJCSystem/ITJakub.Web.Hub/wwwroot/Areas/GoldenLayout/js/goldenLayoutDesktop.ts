class DesktopReader extends ReaderLayout {

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

        var bookHeader = new DesktopHeader(this, this.sc, this.readerHeaderDiv, bookTitle);
        this.readerHeaderDiv.appendChild(bookHeader.getInnerHtml());

        this.readerLayout = this.initLayout();

        this.loadBookmarks();
        this.newFavoriteDialog.make();
        this.newFavoriteDialog.setSaveCallback(this.createBookmarks.bind(this));

    }  

    protected initLayout(): GoldenLayout {
        var config = new LayoutConfiguration();
        var readerLayout = new GoldenLayout(config.goldenLayoutDesktopConfig(), $('#ReaderBodyDiv'));
        readerLayout.registerComponent('toolTab', (container, state) => {
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
        readerLayout.registerComponent('viewTab', (container, state) => {
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
        readerLayout.on("stateChanged", () => {
            this.moveToPageNumber(this.actualPageIndex, true);
        });
        $(window).resize(() => {
            readerLayout.updateSize();
        });
        return readerLayout;
    }

    createToolPanel(panelId: string, panelTitle: string) {
        var configurationObject: LayoutConfiguration = new LayoutConfiguration();

        if (this.readerLayout.root.getItemsById(panelId).length === 0) {
            if (this.readerLayout.root.getItemsById('tools').length === 0) {
                var toolStackConfig = configurationObject.toolPanelConfig(PanelType.Stack, "tools", "");
                this.readerLayout.root.contentItems[0].addChild(toolStackConfig, 0);
            }
            var type: PanelType;
            if (panelId === this.termsPanelId) type = PanelType.Column;
            else type = PanelType.Component;
            var itemConfig = configurationObject.toolPanelConfig(type, panelId, panelTitle);
            this.readerLayout.root.getItemsById('tools')[0].addChild(itemConfig);
            this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
            this.readerLayout.updateSize();
            if (panelId === this.termsPanelId) {
                this.createTermsPanel(configurationObject);
            }
        }
    }

    createViewPanel(panelId: string, panelTitle: string) {
        var configurationObject: LayoutConfiguration = new LayoutConfiguration();
        if (this.readerLayout.root.getItemsById(panelId).length === 0) {
            if (this.readerLayout.root.getItemsById('views').length === 0) {
                var viewColumnConfig = configurationObject.viewPanelConfig(false, PanelType.Column, "views", "");
                this.readerLayout.root.contentItems[0].addChild(viewColumnConfig);
            }
            var itemConfig = configurationObject.viewPanelConfig(true, PanelType.Component, panelId, panelTitle);
            if (this.readerLayout.root.getItemsById('tools').length === 1) {
                this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
                this.readerLayout.updateSize();
            }
            if (panelId === "audio") {
                this.readerLayout.root.getItemsById('views')[0].addChild(itemConfig, 0);
                //TODO UpdateSize
            } else {
                if (this.readerLayout.root.getItemsById('viewsRow').length === 0) {
                    var viewRowConfig = configurationObject.viewPanelConfig(false, PanelType.Row, "viewsRow", "");
                    this.readerLayout.root.getItemsById('views')[0].addChild(viewRowConfig);
                }
                this.readerLayout.root.getItemsById('viewsRow')[0].addChild(itemConfig);
            }
        }
    }
}