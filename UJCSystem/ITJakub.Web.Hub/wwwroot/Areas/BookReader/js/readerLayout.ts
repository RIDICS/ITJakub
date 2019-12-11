///<reference path="../../../lib/golden-layout/index.d.ts"/>
//import GoldenLayout = require("golden-layout");
//declare var GoldenLayout;

class ReaderLayout {
    private favoriteManager: FavoriteManager;
    protected newFavoriteDialog: NewFavoriteDialog;
    readerLayout: GoldenLayout;
    readerHeaderDiv: HTMLDivElement;
    sliderOnPage: number;
    actualPageIndex: number;
    pagesById: IDictionary<BookPage>;
    bookmarks: Array<IBookmarkPosition>;
    pagerDisplayPages: number;
    preloadPagesBefore: number;
    preloadPagesAfter: number;
    bookId: string;
    versionId: string;
    loadedBookContent: boolean;
    sc: ServerCommunication;
    deviceType: Device;
    bookHeader: BookHeader;
    
    clickedMoveToPage: boolean;

    private bookmarksPanel: BookmarksPanel;
    private searchPanel: SearchResultPanel;
    private contentPanel: ContentPanel;
    private termsResultPanel: TermsResultPanel;
    private termsSearchPanel: TermsSearchPanel;

    private textPanel: TextPanel;
    private imagePanel: ImagePanel;
    private audioPanel: AudioPanel;

    toolPanels: Array<ToolPanel>;
    contentViewPanels: Array<ContentViewPanel>;

    audioPanelId: string = "audio";
    textPanelId: string = "text";
    imagePanelId: string = "image";
    contentPanelId: string = "content";
    bookmarksPanelId: string = "bookmarks";
    searchPanelId: string = "search";
    termsPanelId: string = "terms";
    termsResultId: string = "termsResult";
    termsSearchId: string = "termsSearch";

    showPanelList: Array<ReaderPanelEnum>;

    pageChangedCallback: (pageId: number) => void;

    constructor(readerHeaderDiv: HTMLDivElement, pageChangedCallback: (pageId: number) => void, showPanelList: Array<ReaderPanelEnum>, sc: ServerCommunication) {
        this.readerHeaderDiv = readerHeaderDiv;
        $(this.readerHeaderDiv).addClass("content-container");
        this.pageChangedCallback = pageChangedCallback;
        this.pagerDisplayPages = 5;
        this.showPanelList = showPanelList;
        this.favoriteManager = new FavoriteManager();
        this.sc = sc;
        this.newFavoriteDialog = new NewFavoriteDialog(this.favoriteManager, true);
    }

    public getSearchResultPanel(): SearchResultPanel {
        return this.searchPanel;
    }

    public makeReader(bookId: string, versionId: string, bookTitle: string, pageList: IPage[]) {

        this.bookId = bookId;
        this.versionId = versionId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.bookHeader = new BookHeader(this, this.sc, this.readerHeaderDiv, bookTitle);
        this.bookHeader.pages = new Array<BookPage>();
        this.pagesById = {};
        this.bookmarks = new Array<IBookmarkPosition>(pageList.length);
        this.toolPanels = new Array<ToolPanel>();
        this.contentViewPanels = new Array<ContentViewPanel>();

        for (var i = 0; i < pageList.length; i++) { //load pageList
            var page = pageList[i];
            var bookPageItem = new BookPage(page.id, page.name, page.position);

            this.bookHeader.pages.push(bookPageItem);
            this.pagesById[bookPageItem.pageId] = bookPageItem;
        }


        var layoutConfig = new LayoutConfiguration().getLayoutConfig();
        for (var index = 0; index < layoutConfig.length; index++) {
            if (layoutConfig[index].maxWidth > window.innerWidth && layoutConfig[index].minWidth < window.innerWidth) {
                this.deviceType = layoutConfig[index].deviceType;
                this.readerLayout = this.initLayout(layoutConfig[index].goldenLayoutConfig);
                break;
            }
        }
        this.addListeners();
        this.readerHeaderDiv.appendChild(this.bookHeader.getInnerHtml(this.deviceType));

        this.loadBookmarks();
        this.newFavoriteDialog.make();
        this.newFavoriteDialog.setSaveCallback(this.createBookmarks.bind(this));

        $(window as any).resize(() => {
            this.addResponsiveBehavior(this.bookHeader);
        });
    }

    private initLayout(layoutConfiguration: GoldenLayout.Config): GoldenLayout {
        var readerLayout = new GoldenLayout(layoutConfiguration, $('#ReaderBodyDiv'));
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
                    return;
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
                    return;
            }
        });
        readerLayout.init();


        this.attachReaderLayoutEvents(readerLayout);

        $(window as any).resize(() => {
            readerLayout.updateSize();
        });
        return readerLayout;
    }

    private attachReaderLayoutEvents(readerLayout: GoldenLayout) {
        readerLayout.on("stateChanged", () => {
            this.bookHeader.moveToPageNumber(this.actualPageIndex, true);
        });

        // Prevent dragging tab if there is only one in layout
        readerLayout.on('tabCreated', (tab) => {
            var dragListener = tab._dragListener;
            if (dragListener) {
                tab._dragListener._eElement.on("mousedown touchstart", () => {
                    var tabs = $(".lm_tab");
                    if (tabs.length <= 1) {
                        dragListener.onMouseUp();
                    }
                });
            }
        });
    }

    createDesktopToolPanel(panelId: string, panelTitle: string) {
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
        } else {
            var contentItem = this.readerLayout.root.getItemsById(panelId)[0] as GoldenLayout.ContentItem;
            if (contentItem.parent.isStack) {
                contentItem.parent.setActiveContentItem(contentItem);
            } 
        }
    }

    createDesktopViewPanel(panelId: string, panelTitle: string) {
        var configurationObject: LayoutConfiguration = new LayoutConfiguration();
        if (this.readerLayout.root.getItemsById(panelId).length === 0) {
            if (this.readerLayout.root.getItemsById('views').length === 0) {
                var viewColumnConfig = configurationObject.viewPanelConfig(PanelType.Column, "views", localization.translate("BookSources", "BookReader").value);
                this.readerLayout.root.contentItems[0].addChild(viewColumnConfig);
            }
            var itemConfig = configurationObject.viewPanelConfig(PanelType.Component, panelId, panelTitle);
            if (this.readerLayout.root.getItemsById('tools').length === 1) {
                this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
                this.readerLayout.updateSize();
            }
            if (panelId === "audio") {
                this.readerLayout.root.getItemsById('views')[0].addChild(itemConfig, 0);
                //TODO UpdateSize
            } else {
                if (this.readerLayout.root.getItemsById('viewsRow').length === 0) {
                    var viewRowConfig = configurationObject.viewPanelConfig(PanelType.Row, "viewsRow", "");
                    this.readerLayout.root.getItemsById('views')[0].addChild(viewRowConfig);
                }
                this.readerLayout.root.getItemsById('viewsRow')[0].addChild(itemConfig);
            }
        }
    }

    createMobileToolPanel(panelId: string, panelTitle: string) {
        var type;
        if (panelId === this.termsPanelId) type = PanelType.Column;
        else type = PanelType.Component;
        var configurationObject: LayoutConfiguration = new LayoutConfiguration();
        var itemConfig = configurationObject.toolPanelConfig(type, panelId, panelTitle);
        if (this.readerLayout.root.contentItems.length > 0) {
            this.readerLayout.root.contentItems[0].remove();
        }

        this.readerLayout.root.addChild(itemConfig);
        if (panelId === this.termsPanelId) {
            this.createTermsPanel(configurationObject);
        }
    }

    createMobileViewPanel(panelId: string, panelTitle: string) {
        var configurationObject: LayoutConfiguration = new LayoutConfiguration();

        var itemConfig = configurationObject.viewPanelConfig(PanelType.Component, panelId, panelTitle);
        if (this.readerLayout.root.contentItems.length > 0) {
            this.readerLayout.root.contentItems[0].remove();
        }
        this.readerLayout.root.addChild(itemConfig);
    }

    public getNewFavoriteDialog(): NewFavoriteDialog {
        return this.newFavoriteDialog;
    }

    public activateTypeahead(input: HTMLInputElement) {

        var pagesTexts = new Array<ISearchResult>();
        $.each(this.bookHeader.pages, (index, page: BookPage) => {
            pagesTexts.push({index: index, pageText: page.text});
        });

        var pages = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace(['index', 'pageText']),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            local: (): ISearchResult[] => {
                return pagesTexts;
            }
        });

        $(input).typeahead(
            {
                hint: true,
                highlight: true,
                minLength: 1
            },
            {
                name: "pages",
                source: pages,
                display: data => data.pageText,
                templates: {
                    suggestion: data => '<div><span style = "text-align: left;display: inline-block;width: 50%;">Str: ' + data.pageText + '</span> <span style="width: 50%;float: right;text-align: right;display: inline-block;"> Pořadí: ' + data.index + '</span></div>'
                }
            });
    }

    getBookmarks(): IBookmarksInfo {
        var result: IBookmarksInfo = {
            positions: new Array<IBookmarkPosition>(),
            totalCount: 0
        };

        for (var i = 0; i < this.bookmarks.length; i++) {
            var bookmarkPosition = this.bookmarks[i];
            if (bookmarkPosition && bookmarkPosition.bookmarks.length > 0) {
                result.positions.push(bookmarkPosition);
                result.totalCount += bookmarkPosition.bookmarks.length;
            }
        }

        return result;
    }

    createBookmarks(data: INewFavoriteItemData) {
        if (data.labels.length === 0) {
            // TODO possible create default label
            return;
        }

        var pageIndex: number = this.actualPageIndex;
        var page: BookPage = this.bookHeader.pages[pageIndex];
        var bookmarkPosition: IBookmarkPosition = this.bookmarks[pageIndex];

        if (!bookmarkPosition) {
            bookmarkPosition = {
                bookmarks: new Array<IBookPageBookmark>(),
                bookmarkSpan: null,
                pageIndex: pageIndex
            };
            this.bookmarks[pageIndex] = bookmarkPosition;
        }

        var newBookmarks = new Array<IBookPageBookmark>();
        var labelIds = new Array<number>();
        for (let i = 0; i < data.labels.length; i++) {
            var labelItem = data.labels[i];
            var favoriteLabel: IFavoriteLabel = {
                id: labelItem.labelId,
                name: labelItem.labelName,
                color: labelItem.labelColor,
                isDefault: false,
                lastUseTime: null
            }
            var bookPageBookmark: IBookPageBookmark = {
                id: 0,
                favoriteLabel: favoriteLabel,
                pageId: page.pageId,
                title: data.itemName
            };

            newBookmarks.push(bookPageBookmark);
            bookmarkPosition.bookmarks.push(bookPageBookmark);
            labelIds.push(labelItem.labelId);
        }

        $(bookmarkPosition.bookmarkSpan).remove();

        if (bookmarkPosition.bookmarks.length > 1) {
            bookmarkPosition.bookmarkSpan = this.createMultipleBookmarkSpan(pageIndex, page.text, page.pageId, bookmarkPosition.bookmarks.length);
        } else {
            bookmarkPosition.bookmarkSpan = this.createSingleBookmarkSpan(pageIndex, page.text, page.pageId, data.itemName, bookmarkPosition.bookmarks[0].favoriteLabel);
        }

        const postShowAction = () => {
            const $bookmarksContainer = $(".reader-bookmarks-container");
            if (this.bookmarksPanel !== undefined && $bookmarksContainer.length > 0) {
                this.bookmarksPanel.createBookmarkList(
                    $bookmarksContainer.parent().get(0) as Node as HTMLElement,
                    this.bookmarksPanel
                );
            }
        };

        this.favoriteManager.createPageBookmark(Number(this.bookId), page.pageId, data.itemName, labelIds, (ids, error) => {
            if (error) {
                this.newFavoriteDialog.showError(localization.translate("CreatingBookmarkError", "BookReader").value);
                return;
            }

            for (var i = 0; i < ids.length; i++) {
                var id = ids[i];
                var bookmark = newBookmarks[i];
                bookmark.id = id;

            }
            this.newFavoriteDialog.hide();
            this.showBookmark(bookmarkPosition.bookmarkSpan);
            postShowAction();
        });
    }

    showBookmark(bookmarkHtml: HTMLSpanElement) {
        $(this.readerHeaderDiv).find(".slider").append(bookmarkHtml).promise();
    }

    private createBookmarkSpan(pageIndex: number, pageName: string, pageId: number, title: string, tooltipTitle: string | (() => string), favoriteLabel: IFavoriteLabel) {
        var positionStep = 100 / (this.bookHeader.pages.length - 1);
        var bookmarkSpan = document.createElement("span");
        var $bookmarkSpan = $(bookmarkSpan);

        $bookmarkSpan.addClass("glyphicon glyphicon-bookmark bookmark");
        $bookmarkSpan.data("page-index", pageIndex);
        $bookmarkSpan.data("page-name", pageName);
        $bookmarkSpan.attr("data-page-xmlId", pageId);
        $bookmarkSpan.data("title", title);

        $bookmarkSpan.click(() => {
            this.bookHeader.moveToPage(pageId, true);
        });

        if (favoriteLabel) {
            $bookmarkSpan.css("color", favoriteLabel.color);
            $bookmarkSpan.data("label-id", favoriteLabel.id);
            $bookmarkSpan.data("label-name", favoriteLabel.name);
            $bookmarkSpan.data("label-color", favoriteLabel.color);
        }

        var tooltipOptions: TooltipOptions = {
            placement: "bottom",
            title: tooltipTitle
        };
        $bookmarkSpan.tooltip(tooltipOptions);

        var computedPosition = (positionStep * pageIndex);
        $bookmarkSpan.css("left", computedPosition + "%");

        return bookmarkSpan;
    }

    createSingleBookmarkSpan(pageIndex: number, pageName: string, pageId: number, title: string, favoriteLabel: IFavoriteLabel): HTMLSpanElement {
        var tooltipTitle = function () {
            var bookmarkTitle = $(this).data("title");
            return favoriteLabel
                ? localization.translateFormat("Label", new Array<string>(bookmarkTitle, favoriteLabel.name), "BookReader").value
                : bookmarkTitle;
        };

        var bookmarkSpan = this.createBookmarkSpan(pageIndex, pageName, pageId, title, tooltipTitle, favoriteLabel);
        return bookmarkSpan;
    }

    createMultipleBookmarkSpan(pageIndex: number, pageName: string, pageId: number, labelCount: number): HTMLSpanElement {
        var favoriteLabel: IFavoriteLabel = {
            id: 0,
            name: "",
            isDefault: false,
            color: "#000000",
            lastUseTime: null
        };

        var tooltipTitle: string;
        if (labelCount > 4 || labelCount < 1) {
            tooltipTitle = `${labelCount} ${localization.translate("BookmarksGt4", "BookReader").value}`;
        } else if (labelCount > 1) {
            tooltipTitle = `${labelCount} ${localization.translate("BookmarksLt4", "BookReader").value}`;
        } else {
            tooltipTitle = `${labelCount} ${localization.translate("Bookmark", "BookReader").value}`;
        }

        var bookmarkSpan = this.createBookmarkSpan(pageIndex, pageName, pageId, "", tooltipTitle, favoriteLabel);
        var $bookmarkSpan = $(bookmarkSpan);
        $bookmarkSpan.addClass("bookmark-multiple");

        if (labelCount >= 2 && labelCount < 10) {
            $bookmarkSpan.addClass("bookmark-content-" + labelCount);
        } else if (labelCount >= 10) {
            $bookmarkSpan.addClass("bookmark-content-9plus");
        }

        return bookmarkSpan;
    }

    protected loadBookmarks() {
        this.favoriteManager.getPageBookmarks(Number(this.bookId), (bookmarks) => {
            for (var i = 0; i < bookmarks.length; i++) {
                var bookmark = bookmarks[i];
                this.loadBookmark(bookmark);
            }
            if ($(".lm_popin").is("div") && $("#bookmarks").is("div")) { //WORKAROUND - reload bookmarks panel after ajax request is done (if bookmarksPanel is poppedOut)
                $("#bookmarks").empty();
                $("#bookmarks").append(this.bookmarksPanel.getPanelHtml());
            }
        });
    }

    private loadBookmark(actualBookmark: IBookPageBookmark) {
        for (var pageIndex = 0; pageIndex < this.bookHeader.pages.length; pageIndex++) {
            var actualPage = this.bookHeader.pages[pageIndex];
            if (actualBookmark.pageId === actualPage.pageId) {
                var bookmarkPosition = this.bookmarks[pageIndex];
                if (!bookmarkPosition) {
                    bookmarkPosition = {
                        bookmarks: new Array<IBookPageBookmark>(),
                        bookmarkSpan: null,
                        pageIndex: pageIndex
                    };
                    this.bookmarks[pageIndex] = bookmarkPosition;
                }

                bookmarkPosition.bookmarks.push(actualBookmark);

                this.recreateAndShowBookmarkSpan(bookmarkPosition);
                break;
            }
        }
    }

    recreateAndShowBookmarkSpan(bookmarkPosition: IBookmarkPosition) {
        var pageIndex = bookmarkPosition.pageIndex;
        var actualPage = this.bookHeader.pages[pageIndex];

        $(bookmarkPosition.bookmarkSpan).remove();

        if (bookmarkPosition.bookmarks.length > 1) {
            bookmarkPosition.bookmarkSpan = this.createMultipleBookmarkSpan(pageIndex, actualPage.text, actualPage.pageId, bookmarkPosition.bookmarks.length);
            $(bookmarkPosition.bookmarkSpan).data("favorite-id", 0);
        } else if (bookmarkPosition.bookmarks.length === 1) {
            let actualBookmark = bookmarkPosition.bookmarks[0];
            bookmarkPosition.bookmarkSpan = this.createSingleBookmarkSpan(pageIndex, actualPage.text, actualPage.pageId, actualBookmark.title, actualBookmark.favoriteLabel);
            $(bookmarkPosition.bookmarkSpan).data("favorite-id", actualBookmark.id);
        } else {
            bookmarkPosition.bookmarkSpan = null;
        }

        if (bookmarkPosition.bookmarkSpan) {
            this.showBookmark(bookmarkPosition.bookmarkSpan);
        }
    }

    setBookmarkTitle(bookmarkId: number, pageIndex: number, title: string) {
        var bookmarkPosition = this.bookmarks[pageIndex];
        var bookmark: IBookPageBookmark = null;
        for (let i = 0; i < bookmarkPosition.bookmarks.length; i++) {
            var bookmarkItem = bookmarkPosition.bookmarks[i];
            if (bookmarkItem.id === bookmarkId) {
                bookmark = bookmarkItem;
                break;
            }
        }

        if (!bookmark || bookmark.title === title) {
            return;
        }

        bookmark.title = title;
        if (bookmarkPosition.bookmarks.length === 1) {
            $(bookmarkPosition.bookmarkSpan).data("title", title);
        }
        
        this.favoriteManager.updateFavoriteItem(bookmarkId, title, () => {
        });
    }

    public persistRemoveBookmark(pageIndex: number, bookmarkId: number) {
        const postRemoveAction = () => {
            const $bookmarksContainer = $(".reader-bookmarks-container");
            if (this.bookmarksPanel !== undefined && $bookmarksContainer.length > 0) {
                this.bookmarksPanel.createBookmarkList(
                    $bookmarksContainer.parent().get(0) as Node as HTMLElement,
                    this.bookmarksPanel
                );
            }
        };

        this.favoriteManager.deleteFavoriteItem(bookmarkId, () => {
            var bookmarkPosition = this.bookmarks[pageIndex];
            var indexToRemove: number;
            for (indexToRemove = 0; indexToRemove < bookmarkPosition.bookmarks.length; indexToRemove++) {
                if (bookmarkPosition.bookmarks[indexToRemove].id === bookmarkId) {
                    bookmarkPosition.bookmarks.splice(indexToRemove, 1);
                    break;
                }
            }

            this.recreateAndShowBookmarkSpan(bookmarkPosition);
            postRemoveAction();
        });
    }
    
    notifyPanelsMovePage(pageIndex: number, scrollTo: boolean) {
        var k: 0;
        for (k = 0; k < this.toolPanels.length; k++) {
            this.toolPanels[k].onMoveToPage(pageIndex, scrollTo);
        }

        for (k = 0; k < this.contentViewPanels.length; k++) {
            this.contentViewPanels[k].onMoveToPage(pageIndex, scrollTo);
        }
    }

    getActualPage(): BookPage {
        return this.bookHeader.pages[this.actualPageIndex];
    }

    getPageByIndex(pageIndex: number): BookPage {
        return this.bookHeader.pages[pageIndex];
    }

    getBookId(): string {
        return this.bookId;
    }

    getVersionId(): string {
        return this.versionId;
    }

    showSearchInPanel(searchResults: Array<SearchHitResult>) {
        this.getSearchPanel().showResults(searchResults);
    }

    setResultsPaging(itemsCount: number, pageChangedCallback: (pageNumner: number) => void) {
        this.getSearchPanel().createPagination(pageChangedCallback, itemsCount, this.getSearchPanel().getPaginator().getCurrentPage());
    }

    getSearchResultsCountOnPage(): number {
        return this.getSearchPanel().getResultsCountOnPage();
    }

    private getSearchPanel(): SearchResultPanel {
        if (this.deviceType === Device.Mobile) {
            this.createMobileToolPanel(this.searchPanelId, localization.translate(this.searchPanelId, "BookReader").value);
        } else {
            this.createDesktopToolPanel(this.searchPanelId, localization.translate(this.searchPanelId, "BookReader").value);
        }

        var searchButton = $(document as any).find(".search-button");
        searchButton.prop("disabled", false);
        return this.searchPanel;
    }

    private getTermsResultPanel(): TermsResultPanel {
        if (this.termsResultPanel === null) {
            if (this.deviceType === Device.Mobile) {
                this.createMobileToolPanel(this.termsResultId, localization.translate(this.termsResultId, "BookReader").value);
            } else {
                this.createDesktopToolPanel(this.termsResultId, localization.translate(this.termsResultId, "BookReader").value);
            }
        }

        return this.termsResultPanel;
    }

    private getTermsSearchPanel(): TermsSearchPanel {
        if (this.termsSearchPanel === null) {
            if (this.deviceType === Device.Mobile) {
                this.createMobileToolPanel(this.termsSearchId, localization.translate(this.termsSearchId, "BookReader").value);
            } else {
                this.createDesktopToolPanel(this.termsSearchId, localization.translate(this.termsSearchId, "BookReader").value);
            }
        }

        return this.termsSearchPanel;
    }

    private showSearchResultInPages(searchQuery: string, isQueryJson: boolean, pages: Array<IPage>) {
        this.textPanel.setSearchedQuery(searchQuery, isQueryJson);
        $(".search-unloaded").removeClass(".search-unloaded");
        var previousSearchPages = $(".search-loaded");
        $(previousSearchPages).removeClass(".search-loaded");
        $(previousSearchPages).addClass("unloaded");
        for (var i = 0; i < pages.length; i++) {
            var page = pages[i];
            var pageDiv = document.getElementById(page.id.toString());
            $(pageDiv).addClass("search-unloaded");
        }
        this.bookHeader.moveToPageNumber(this.actualPageIndex, true);
    }

    searchPanelShowLoading() {
        this.getSearchPanel().showLoading();

    }

    searchPanelRemoveLoading() {
        this.getSearchPanel().clearLoading();
    }

    searchPanelClearResults() {
        this.getSearchPanel().clearResults();
    }

    setTermPanelCallback(callback: (termId: number, text: string) => void) {
        this.getTermsResultPanel().setTermClickedCallback(callback);
    }

    termsPanelShowLoading() {
        this.getTermsSearchPanel().showLoading();

    }

    termsPanelRemoveLoading() {
        this.getTermsSearchPanel().clearLoading();
    }

    termsPanelClearResults() {
        this.getTermsSearchPanel().clearResults();
    }

    protected createBookmarksPanel(): HTMLDivElement {
        if (this.bookmarksPanel == null || this.deviceType === Device.Mobile) {
            var bookmarksPanel: BookmarksPanel = new BookmarksPanel(this.bookmarksPanelId, this, this.sc);
            this.bookmarksPanel = bookmarksPanel;
            this.toolPanels.push(bookmarksPanel);
        }
        return this.bookmarksPanel.getPanelHtml();
    }

    protected createContentPanel(): HTMLDivElement {
        if (this.contentPanel == null || this.deviceType === Device.Mobile) {
            var contentPanel: ContentPanel = new ContentPanel(this.contentPanelId, this, this.sc);
            this.contentPanel = contentPanel;
            this.toolPanels.push(contentPanel);
        }
        return this.contentPanel.getPanelHtml();
    }

    protected createSearchPanel(): HTMLDivElement {
        if (this.searchPanel == null) {
            var resultPanel: SearchResultPanel = new SearchResultPanel(this.searchPanelId, this, this.sc);
            this.searchPanel = resultPanel;
            this.toolPanels.push(resultPanel);
        }
        return this.searchPanel.getPanelHtml();
    }

    protected createTermsPanel(configObject: LayoutConfiguration) {
        var itemConfig = configObject.toolPanelConfig(PanelType.Component, this.termsSearchId, localization.translate(this.termsSearchId, "BookReader").value);
        this.readerLayout.root.getItemsById(this.termsPanelId)[0].addChild(itemConfig);
        itemConfig = configObject.toolPanelConfig(PanelType.Component, this.termsResultId, localization.translate(this.termsSearchId, "BookReader").value);
        this.readerLayout.root.getItemsById(this.termsPanelId)[0].addChild(itemConfig);
    }

    protected createTermsResultPanel(): HTMLDivElement {
        if (this.termsResultPanel == null) {
            var termsPanel: TermsResultPanel = new TermsResultPanel(this.termsResultId, this, this.sc);
            this.termsResultPanel = termsPanel;
            this.toolPanels.push(termsPanel);
        }
        return this.termsResultPanel.getPanelHtml();
    }

    protected createTermsSearchPanel(): HTMLDivElement {
        if (this.termsSearchPanel == null) {
            var termsPanel: TermsSearchPanel = new TermsSearchPanel(this.termsSearchId, this, this.sc);
            this.termsSearchPanel = termsPanel;
            this.toolPanels.push(termsPanel);
        }
        return this.termsSearchPanel.getPanelHtml();
    }

    protected createTextPanel(): HTMLDivElement {
        if (this.textPanel == null) {
            var textPanel: TextPanel = new TextPanel(this.textPanelId, this, this.sc);
            this.textPanel = textPanel;
            this.contentViewPanels.push(textPanel);
        }
        return this.textPanel.getPanelHtml();
    }

    protected createImagePanel(): HTMLDivElement {
        if (this.imagePanel == null) {
            var imagePanel: ImagePanel = new ImagePanel(this.imagePanelId, this, this.sc);
            this.imagePanel = imagePanel;
            this.contentViewPanels.push(imagePanel);
        }
        return this.imagePanel.getPanelHtml();
    }

    protected createAudioPanel(): HTMLDivElement {
        if (this.audioPanel == null) {
            var audioPanel: AudioPanel = new AudioPanel(this.audioPanelId, this, this.sc);
            this.audioPanel = audioPanel;
            this.contentViewPanels.push(audioPanel);
        }
        return this.audioPanel.getPanelHtml();
    }

    protected hasBookImageCache: { [key: string]: { [key: string]: boolean; }; } = {};

    hasBookPage(bookId: string, bookVersionId: string, onTrue: () => any = null, onFalse: () => any = null) {
        if (this.hasBookPageCache[bookId] !== undefined && this.hasBookPageCache[bookId][bookVersionId + "_loading"]) {
            this.hasBookPageCallOnSuccess[bookId][bookVersionId].push(() => {
                this.hasBookPage(bookId, bookVersionId, onTrue, onFalse);
            });
        } else if (this.hasBookPageCache[bookId] === undefined || this.hasBookPageCache[bookId][bookVersionId] === undefined) {
            if (this.hasBookPageCache[bookId] === undefined) {
                this.hasBookPageCache[bookId] = {};
                this.hasBookPageCache[bookId][bookVersionId + "_loading"] = true;

                this.hasBookPageCallOnSuccess[bookId] = {};
                this.hasBookPageCallOnSuccess[bookId][bookVersionId] = [];
            }
            var hasBookPage: JQueryXHR = this.sc.hasBookPage(bookId, bookVersionId);
            hasBookPage.done((response: { hasBookPage: boolean }) => {
                this.hasBookPageCache[bookId][bookVersionId] = response.hasBookPage;
                this.hasBookPageCache[bookId][bookVersionId + "_loading"] = false;

                if (response.hasBookPage && onTrue !== null) {
                    onTrue();
                } else if (!response.hasBookPage && onFalse !== null) {
                    onFalse();
                }

                while (this.hasBookPageCallOnSuccess[bookId][bookVersionId].length) {
                    this.hasBookPageCallOnSuccess[bookId][bookVersionId].pop()();
                }
            });
            hasBookPage.fail((response) => {
                console.error(response);

                this.hasBookPageCache[bookId][bookVersionId + "_loading"] = false;
                this.hasBookPageCallOnSuccess[bookId][bookVersionId].pop()();
            });
        } else if (this.hasBookPageCache[bookId][bookVersionId] && onTrue !== null) {
            onTrue();
        } else if (!this.hasBookPageCache[bookId][bookVersionId] && onFalse !== null) {
            onFalse();
        }
    }

    protected hasBookPageCache: { [key: string]: { [key: string]: boolean; }; } = {};
    protected hasBookPageCallOnSuccess: { [key: string]: { [key: string]: Array<() => any>; }; } = {};

    private addResponsiveBehavior(bookHeader: BookHeader) {
        var configArray = new LayoutConfiguration().getLayoutConfig();
        for (var index = 0; index < configArray.length; index++) {
            if (configArray[index].minWidth <= window.innerWidth && configArray[index].maxWidth >= window.innerWidth && this.deviceType !== configArray[index].deviceType) {
                this.createSearchPanel();
                this.deviceType = configArray[index].deviceType;
                this.readerLayout.destroy();
                this.readerLayout = this.initLayout(configArray[index].goldenLayoutConfig);
                $(this.readerHeaderDiv).empty();
                $(this.readerHeaderDiv).append(bookHeader.getInnerHtml(this.deviceType));
                return;
            }
        }

    }

    private addListeners() {
        this.readerLayout.eventHub.on("navigationClicked", (pageNumber: number) => {
            this.clickedMoveToPage = true;
            this.bookHeader.moveToPage(pageNumber, true);
        });

        this.readerLayout.on("windowOpened", () => {
            this.readerLayout.eventHub.emit("moveToPageNumber", this.getActualPage().position - 1);
        });

        this.readerLayout.eventHub.on("moveToPageNumber", (pageNumber: number) => {
            this.clickedMoveToPage = true;
            this.bookHeader.moveToPageNumber(pageNumber, true);
        });

        this.readerLayout.eventHub.on("scrollPage", (pageWithMinOffsetId) => {
            this.bookHeader.moveToPage(pageWithMinOffsetId, false);
        });
        
        this.readerLayout.eventHub.on("showTextSearchMatch", (searchQuery: string, isQueryJson: boolean, pages: Array<IPage>) => {
            this.showSearchResultInPages(searchQuery, isQueryJson, pages);
        })
    }
}

interface ISearchResult {
    index: number,
    pageText: string;
}
