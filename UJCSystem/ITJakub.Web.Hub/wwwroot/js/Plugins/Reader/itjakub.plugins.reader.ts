﻿class ReaderModule extends ReaderPagination {

    private favoriteManager: FavoriteManager;
    private newFavoriteDialog: NewFavoriteDialog;
    readerContainer: HTMLDivElement;
    sliderOnPage: number;
    pagesById: IDictionary<BookPage>;
    bookmarks: Array<IBookmarkPosition>;
    preloadPagesBefore: number;
    preloadPagesAfter: number;
    bookId: string;
    versionId: string;
    loadedBookContent: boolean;

    clickedMoveToPage: boolean;

    leftSidePanels: Array<SidePanelOld>;
    rightSidePanels: Array<SidePanelOld>;


    imagePanelIdentificator: string = "ImagePanel";
    textPanelIdentificator: string = "TextPanel";
    searchPanelIdentificator: string = "SearchPanel";
    termsPanelIdentificator: string = "TermsPanel";
    bookmarksPanelIdentificator: string = "BookmarksPanel";
    settingsPanelIdentificator: string = "SettingsPanel";
    contentPanelIdentificator: string = "ContentPanel";

    imagePanel: ImagePanelOld;
    textPanel: TextPanelOld;
    searchPanel: SearchResultPanelOld;
    bookmarksPanel: BookmarksPanelOld;
    settingsPanel: SettingsPanel;
    contentPanel: ContentPanelOld;
    termsPanel: TermsPanelOld;

    showPanelList: Array<ReaderPanelEnum>;
    showLeftSidePanelsButtonList: Array<PanelButtonEnum>;
    showMainPanelsButtonList: Array<PanelButtonEnum>;

    pageChangeCallback: (pageId: number) => void;


    constructor(readerContainer: HTMLDivElement, pageChangedCallback: (pageId: number) => void, showPanelList: Array<ReaderPanelEnum>, showLeftSidePanelsButtonList: Array<PanelButtonEnum>, showMainPanelsButtonList: Array<PanelButtonEnum>) {
        super(readerContainer);
        this.pageChangeCallback = pageChangedCallback;
        this.showPanelList = showPanelList;
        this.showLeftSidePanelsButtonList = showLeftSidePanelsButtonList;
        this.showMainPanelsButtonList = showMainPanelsButtonList;
        this.favoriteManager = new FavoriteManager();
        this.newFavoriteDialog = new NewFavoriteDialog(this.favoriteManager, true);
        this.init((pageId, pageIndex, scrollTo) => {
            this.actualizeSlider(pageIndex);
            this.notifyPanelsMovePage(pageIndex, scrollTo);
            this.pageChangeCallback(pageId);        
        });
    }
        
    public makeReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: IPage[]) {
        this.bookId = bookXmlId;
        this.versionId = versionXmlId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<BookPage>();
        this.pagesById = {};
        this.bookmarks = new Array<IBookmarkPosition>(pageList.length);
        this.leftSidePanels = new Array<SidePanelOld>();
        this.rightSidePanels = new Array<SidePanelOld>();

        $(window.document.documentElement).on("beforeunload", (event: JQuery.Event) => {
            for (var k = 0; k < this.leftSidePanels.length; k++) {
                if (this.leftSidePanels && this.leftSidePanels[k].childwindow) {
                    this.leftSidePanels[k].childwindow.close();    
                }
            }

            for (var k = 0; k < this.rightSidePanels.length; k++) {
                if (this.rightSidePanels && this.rightSidePanels[k].childwindow) {
                    this.rightSidePanels[k].childwindow.close();
                }
            }
        });

        for (var i = 0; i < pageList.length; i++) { //load pageList
            var page = pageList[i];
            var bookPageItem = new BookPage(page.id, page.name, page.position);

            this.pages.push(bookPageItem);
            this.pagesById[bookPageItem.pageId] = bookPageItem;
        }

        $(this.readerContainer).empty();
        var readerDiv: HTMLDivElement = document.createElement("div");
        $(readerDiv).addClass("reader");

        var readerHeadDiv: HTMLDivElement = document.createElement("div");
        $(readerHeadDiv).addClass("reader-head content-container");

        var fullscreenButton = document.createElement("button");
        $(fullscreenButton).addClass("fullscreen-button");

        var fullscreenSpan = document.createElement("span");
        $(fullscreenSpan).addClass("glyphicon glyphicon-fullscreen");
        $(fullscreenButton).append(fullscreenSpan);
        $(fullscreenButton).click((event) => {
            $(this.readerContainer).find(".reader").addClass("fullscreen");
        });
        readerHeadDiv.appendChild(fullscreenButton);

        var fullscreenCloseButton = document.createElement("button");
        $(fullscreenCloseButton).addClass("fullscreen-close-button");

        var closeSpan = document.createElement("span");
        $(closeSpan).addClass("glyphicon glyphicon-remove");
        $(fullscreenCloseButton).append(closeSpan);
        $(fullscreenCloseButton).click((event) => {
            $(this.readerContainer).find(".reader").removeClass("fullscreen");
        });
        readerHeadDiv.appendChild(fullscreenCloseButton);


        var title = this.makeTitle(bookTitle);
        readerHeadDiv.appendChild(title);


        var controls = this.makeControls();
        readerHeadDiv.appendChild(controls);
        readerDiv.appendChild(readerHeadDiv);

        var readerBodyDiv = this.makeReaderBody();
        readerDiv.appendChild(readerBodyDiv);

        $(this.readerContainer).append(readerDiv);

        this.loadBookmarks();
        this.newFavoriteDialog.make();
        this.newFavoriteDialog.setSaveCallback(this.createBookmarks.bind(this));

        this.moveToPageNumber(0, false); //load first page
    }

    protected hasBookPageCache: { [key: string]: { [key: string]: boolean; }; } = {};
    protected hasBookPageCallOnSuccess: { [key: string]: { [key: string]: Array<()=>any>; }; } = {};

    hasBookPage(bookId: string, bookVersionId: string, onTrue: () => any = null, onFalse: () => any = null) {
        if (this.hasBookPageCache[bookId] !== undefined && this.hasBookPageCache[bookId][bookVersionId + "_loading"]) {
            this.hasBookPageCallOnSuccess[bookId][bookVersionId].push(() => {
                this.hasBookPage(bookId, bookVersionId, onTrue, onFalse);
            });
        }
        else if (this.hasBookPageCache[bookId] === undefined || this.hasBookPageCache[bookId][bookVersionId] === undefined) {
            if (this.hasBookPageCache[bookId] === undefined) {
                this.hasBookPageCache[bookId] = {};
                this.hasBookPageCache[bookId][bookVersionId + "_loading"] = true;

                this.hasBookPageCallOnSuccess[bookId] = {};
                this.hasBookPageCallOnSuccess[bookId][bookVersionId] = [];
            }

            $.ajax({
                type: "GET",
                traditional: true,
                data: { bookId: bookId, snapshotId: bookVersionId } as JQuery.PlainObject,
                url: document.getElementsByTagName("body")[0].getAttribute("data-has-book-text-url"),
                dataType: "json",
                contentType: "application/json",
                success: (response: { HasBookPage: boolean }) => {
                    this.hasBookPageCache[bookId][bookVersionId] = response.HasBookPage;
                    this.hasBookPageCache[bookId][bookVersionId + "_loading"] = false;

                    if (response.HasBookPage && onTrue !== null) {
                        onTrue();
                    } else if (!response.HasBookPage && onFalse !== null) {
                        onFalse();
                    }

                    while (this.hasBookPageCallOnSuccess[bookId][bookVersionId].length) {
                        this.hasBookPageCallOnSuccess[bookId][bookVersionId].pop()();
                    }
                },
                error: (response) => {
                    console.error(response);

                    this.hasBookPageCache[bookId][bookVersionId + "_loading"] = false;
                    this.hasBookPageCallOnSuccess[bookId][bookVersionId].pop()();
                }
            });
        } else if (this.hasBookPageCache[bookId][bookVersionId] && onTrue !== null) {
            onTrue();
        } else if (!this.hasBookPageCache[bookId][bookVersionId] && onFalse !== null) {
            onFalse();
        }
    }

    protected hasBookImageCache: { [key: string]: { [key: string]: boolean; }; } = {};

    hasBookImage(bookId: string, bookVersionId: string, onTrue: () => any = null, onFalse: () => any = null) {
        if (this.hasBookImageCache[bookId] === undefined || this.hasBookImageCache[bookId][bookVersionId] === undefined) {
            $.ajax({
                type: "GET",
                traditional: true,
                data: { bookId: bookId, snapshotId: bookVersionId } as JQuery.PlainObject,
                url: this.readerContainer.getAttribute("data-has-image-url"),
                dataType: "json",
                contentType: "application/json",
                success: (response: { HasBookImage: boolean }) => {
                    if (this.hasBookImageCache[bookId] === undefined) {
                        this.hasBookImageCache[bookId] = {};
                    }
                    this.hasBookImageCache[bookId][bookVersionId] = response.HasBookImage;
                    if (response.HasBookImage && onTrue !== null) {
                        onTrue();
                    } else if (!response.HasBookImage && onFalse !== null) {
                        onFalse();
                    }
                },
                error: (response) => {
                    console.error(response);
                }
            });
        } else if (this.hasBookImageCache[bookId][bookVersionId] && onTrue !== null) {
            onTrue();
        } else if (!this.hasBookImageCache[bookId][bookVersionId] && onFalse !== null) {
            onFalse();
        }
    }

    getBookXmlId(): string {
        return this.bookId;
    }

    getVersionXmlId(): string {
        return this.versionId;
    }

    getActualPage(): BookPage {
        return this.pages[this.actualPageIndex];
    }

    getPageByIndex(pageIndex: number): BookPage {
        return this.pages[pageIndex];
    }

    public makeTitle(bookTitle: string): HTMLDivElement {
        var titleDiv: HTMLDivElement = document.createElement("div");
        $(titleDiv).addClass("title");
        titleDiv.innerHTML = bookTitle;
        return titleDiv;
    }

    private makeControls(): HTMLDivElement {

        var controlsDiv: HTMLDivElement = document.createElement("div");
        $(controlsDiv).addClass("reader-controls content-container");

        var slider: HTMLDivElement = document.createElement("div");
        $(slider).addClass("slider");
        $(slider).slider({
            min: 0,
            max: this.pages.length - 1,
            value: 0,
            start: (event, ui) => {
                $(event.target as Node as Element).find(".ui-slider-handle").find(".slider-tip").show();
            },
            stop: (event, ui) => {
                $(event.target as Node as Element).find(".ui-slider-handle").find(".slider-tip").fadeOut(1000);
            },
            slide: (event, ui) => {
                const targetEl = $(event.target as Node as Element);
                targetEl.find(".ui-slider-handle").find(".slider-tip").stop(true, true);
                targetEl.find(".ui-slider-handle").find(".slider-tip").show();
                if (this.pages[ui.value] !== undefined) {
                    targetEl.find(".ui-slider-handle").find(".tooltip-inner").html(localization.translate("Page:", "PluginsJs").value + this.pages[ui.value].text);
                } else {
                    console.error("missing page "+ui.value);
                }
            },
            change: (event: Event, ui: JQueryUI.SliderUIParams) => {
                if (this.actualPageIndex !== ui.value) {
                    this.moveToPageNumber(<any>ui.value, true);
                }
            }
        });

        var sliderTooltip: HTMLDivElement = document.createElement("div");
        sliderTooltip.classList.add("tooltip", "top", "slider-tip");
        var arrowTooltip: HTMLDivElement = document.createElement("div");
        arrowTooltip.classList.add("tooltip-arrow");
        sliderTooltip.appendChild(arrowTooltip);

        var innerTooltip: HTMLDivElement = document.createElement("div");
        $(innerTooltip).addClass("tooltip-inner");
        if (this.pages[0] !== undefined) {
            $(innerTooltip).html(localization.translate("Page:", "PluginsJs").value + this.pages[0].text);
        }
        else {
            console.error("missing page " + 0);
        }
        sliderTooltip.appendChild(innerTooltip);
        $(sliderTooltip).hide();

        var sliderHandle:JQuery = $(slider).find(".ui-slider-handle");
        sliderHandle.append(sliderTooltip);
        sliderHandle.hover((event) => {
            $(event.target).find(".slider-tip").stop(true, true);
            $(event.target).find(".slider-tip").show();
        });
        sliderHandle.mouseout((event) => {
            $(event.target).find(".slider-tip").fadeOut(1000);
        });
        controlsDiv.appendChild(slider);

        var pagingDiv: HTMLDivElement = document.createElement("div");
        pagingDiv.classList.add("paging");

        var pageInputDiv: HTMLDivElement = document.createElement("div");
        pageInputDiv.classList.add("page-input");

        var pageInputText = document.createElement("input");
        pageInputText.setAttribute("type", "text");
        pageInputText.setAttribute("id", "pageInputText");
        pageInputText.setAttribute("placeholder", localization.translate("GotoPage", "PluginsJs").value + "...");
        pageInputText.classList.add("page-input-text");
        pageInputDiv.appendChild(pageInputText);

        var pageInputButton = document.createElement("button");
        $(pageInputButton).addClass("btn btn-default page-input-button");

        var pageInputButtonSpan = document.createElement("span");
        $(pageInputButtonSpan).addClass("glyphicon glyphicon-arrow-right");
        $(pageInputButton).append(pageInputButtonSpan);

        $(pageInputButton).click((event: JQuery.Event) => {
            var pageName = $("#pageInputText").val() as string;
            var pageIndex: number = -1;
            for (var i = 0; i < this.pages.length; i++) {
                if (this.pages[i].text === pageName) {
                    pageIndex = i;
                    break;
                }
            }

            if (this.pages[pageIndex] !== undefined) {
                var page: BookPage = this.pages[pageIndex];
                this.moveToPage(page.pageId, true);
            }
            else {
                console.error("missing page " + pageIndex);
            }
        });

        pageInputDiv.appendChild(pageInputButton);

        $(pageInputText).keypress((event: any) => {
            var keyCode = event.which || event.keyCode;
            if (keyCode === 13) {     //13 = Enter
                $(pageInputButton).click();
                event.preventDefault();
                event.stopPropagation();
                return false;
            }
        });

        this.activateTypeahead(pageInputText);

       const listingContainer = this.createPagination(true);

        var buttonsDiv: HTMLDivElement = document.createElement("div");
        $(buttonsDiv).addClass("buttons");

        var addBookmarkButton = document.createElement("button");
        $(addBookmarkButton).addClass("bookmark-button");

        var addBookmarkSpan = document.createElement("span");
        $(addBookmarkSpan).addClass("glyphicon glyphicon-bookmark");
        $(addBookmarkButton).append(addBookmarkSpan);

        var addBookmarkSpanText = document.createElement("span");
        $(addBookmarkSpanText).addClass("button-text");
        $(addBookmarkSpanText).append(localization.translate("AddBookmark", "PluginsJs").value);
        $(addBookmarkButton).append(addBookmarkSpanText);

        $(addBookmarkButton).click((event: JQuery.Event) => {
            var actualPageName = this.getActualPage().text;
            this.newFavoriteDialog.show(actualPageName);
        });

        var bookmarkButton = document.createElement("button");
        $(bookmarkButton).addClass("bookmark-button");

        var bookmarkSpan = document.createElement("span");
        $(bookmarkSpan).addClass("glyphicon glyphicon-bookmark");
        $(bookmarkButton).append(bookmarkSpan);

        var bookmarkSpanText = document.createElement("span");
        $(bookmarkSpanText).addClass("button-text");
        $(bookmarkSpanText).append(localization.translate("Bookmarks", "PluginsJs").value);
        $(bookmarkButton).append(bookmarkSpanText);

        $(bookmarkButton).click((event: JQuery.Event) => {
            var panelId = this.bookmarksPanelIdentificator;
            if (!this.existSidePanel(panelId)) {
                var bookmarksPanel: BookmarksPanelOld = new BookmarksPanelOld(panelId, this, this.showLeftSidePanelsButtonList);
                this.loadSidePanel(bookmarksPanel.panelHtml);
                this.leftSidePanels.push(bookmarksPanel);
                this.bookmarksPanel = bookmarksPanel;
            }
            this.changeSidePanelVisibility(this.bookmarksPanelIdentificator, "left");
        });

        buttonsDiv.appendChild(addBookmarkButton);
        buttonsDiv.appendChild(bookmarkButton);

        if (this.showPanelList.indexOf(ReaderPanelEnum.SettingsPanel) >= 0) {

            var settingsButton = document.createElement("button");
            $(settingsButton).addClass("comment-button");

            var settingsSpan = document.createElement("span");
            $(settingsSpan).addClass("glyphicon glyphicon-cog");
            $(settingsButton).append(settingsSpan);

            var settingsSpanText = document.createElement("span");
            $(settingsSpanText).addClass("button-text");
            $(settingsSpanText).append(localization.translate("View", "PluginsJs").value);
            $(settingsButton).append(settingsSpanText);

            $(settingsButton).click((event: JQuery.Event) => {
                var panelId = this.settingsPanelIdentificator;
                if (!this.existSidePanel(panelId)) {
                    var settingsPanel: SettingsPanel = new SettingsPanel(panelId, this, this.showLeftSidePanelsButtonList);
                    this.loadSidePanel(settingsPanel.panelHtml);
                    this.leftSidePanels.push(settingsPanel);
                    this.settingsPanel = settingsPanel;
                }
                this.changeSidePanelVisibility(this.settingsPanelIdentificator, "left");
            });

            buttonsDiv.appendChild(settingsButton);
        }

        if (this.showPanelList.indexOf(ReaderPanelEnum.SearchPanel) >= 0) {

            var searchResultButton = document.createElement("button");
            $(searchResultButton).addClass("search-button");

            var searchSpan = document.createElement("span");
            $(searchSpan).addClass("glyphicon glyphicon-search");
            $(searchResultButton).append(searchSpan);

            var searchSpanText = document.createElement("span");
            $(searchSpanText).addClass("button-text");
            $(searchSpanText).append(localization.translate("Search", "PluginsJs").value);
            $(searchResultButton).append(searchSpanText);

            $(searchResultButton).click((event: JQuery.Event) => {
                var panelId = this.searchPanelIdentificator;
                if (!this.existSidePanel(panelId)) {
                    var searchPanel = new SearchResultPanelOld(panelId, this, this.showLeftSidePanelsButtonList);
                    this.loadSidePanel(searchPanel.panelHtml);
                    this.leftSidePanels.push(<any>searchPanel);
                    this.searchPanel = searchPanel;
                }
                this.changeSidePanelVisibility(this.searchPanelIdentificator, "left");
            });

            buttonsDiv.appendChild(searchResultButton);
        }

        if (this.showPanelList.indexOf(ReaderPanelEnum.TermsPanel) >= 0) {

            var termsButton = document.createElement("button");
            $(termsButton).addClass("terms-button");

            var termsSpan = document.createElement("span");
            $(termsSpan).addClass("glyphicon glyphicon-list-alt");
            $(termsButton).append(termsSpan);

            var termsSpanText = document.createElement("span");
            $(termsSpanText).addClass("button-text");
            $(termsSpanText).append(localization.translate("Terms", "PluginsJs").value);
            $(termsButton).append(termsSpanText);

            $(termsButton).click((event: JQuery.Event) => {
                var panelId = this.termsPanelIdentificator;
                if (!this.existSidePanel(panelId)) {
                    var termsPanel = new TermsPanelOld(panelId, this, this.showLeftSidePanelsButtonList);
                    this.loadSidePanel(termsPanel.panelHtml);
                    this.leftSidePanels.push(<any>termsPanel);
                    this.termsPanel = termsPanel;
                }
                this.changeSidePanelVisibility(this.termsPanelIdentificator, "left");
            });

            buttonsDiv.appendChild(termsButton);
        }

        if (this.showPanelList.indexOf(ReaderPanelEnum.ContentPanel) >= 0) {

            var contentButton = document.createElement("button");
            $(contentButton).addClass("content-button");

            var contentSpan = document.createElement("span");
            $(contentSpan).addClass("glyphicon glyphicon-book");
            $(contentButton).append(contentSpan);

        
                var contentSpanText = document.createElement("span");
                $(contentSpanText).addClass("button-text");
                $(contentSpanText).append(localization.translate("Content", "PluginsJs").value);
                $(contentButton).append(contentSpanText);

                $(contentButton).click((event: JQuery.Event) => {
                    var panelId = this.contentPanelIdentificator;
                    if (!this.existSidePanel(panelId)) {
                        var contentPanel: ContentPanelOld = new ContentPanelOld(panelId, this, this.showLeftSidePanelsButtonList);
                        this.loadSidePanel(contentPanel.panelHtml);
                        this.leftSidePanels.push(contentPanel);
                        this.contentPanel = contentPanel;
                    }
                    this.changeSidePanelVisibility(this.contentPanelIdentificator, "left");
                });

                buttonsDiv.appendChild(contentButton);
        }

        pagingDiv.appendChild(pageInputDiv);
        pagingDiv.appendChild(buttonsDiv);
        pagingDiv.appendChild(listingContainer);

        controlsDiv.appendChild(pagingDiv);

        var resetDiv = document.createElement("div");
        resetDiv.classList.add("reset");
        controlsDiv.appendChild(resetDiv);

        return controlsDiv;
    }

    public activateTypeahead(input: HTMLInputElement) {

        var pagesTexts = new Array<string>();
        $.each(this.pages, (index, page: BookPage) => {
            pagesTexts.push(page.text);
        });

        var pages = new Bloodhound({ datumTokenizer: Bloodhound.tokenizers.whitespace, queryTokenizer: Bloodhound.tokenizers.whitespace, local: (): string[] => { return pagesTexts; } });

        $(input as Node as Element).typeahead({ hint: true, highlight: true, minLength: 1 },{ name: "pages", source: pages });
    }

    private loadBookmarks() {
        this.favoriteManager.getPageBookmarks(Number(this.bookId), (bookmarks) => {
            for (var i = 0; i < bookmarks.length; i++) {
                var bookmark = bookmarks[i];
                this.loadBookmark(bookmark);
            }
        });
    }

    private loadBookmark(actualBookmark: IBookPageBookmark) {
        for (var pageIndex = 0; pageIndex < this.pages.length; pageIndex++) {
            var actualPage = this.pages[pageIndex];
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

    private existSidePanel(sidePanelIdentificator: string): boolean {
        var sidePanel = document.getElementById(sidePanelIdentificator);
        return ($(sidePanel).length > 0 && sidePanel != null);
    }

    private loadSidePanel(sidePanel) {
        var bodyContainerDiv = $(".reader-body-container");
        $(sidePanel).hide();
        $(bodyContainerDiv).prepend(sidePanel);
    }

    changeSidePanelVisibility(sidePanelIdentificator: string, slideDirection: string) {
        var sidePanel = document.getElementById(sidePanelIdentificator);
        if ($(sidePanel).is(":visible")) {
            if ($(sidePanel).hasClass("ui-draggable")) {
                $(sidePanel).hide();
            } else {
                if (slideDirection) {
                    $(sidePanel).hide("slide", { direction: slideDirection });
                } else {
                    $(sidePanel).hide();
                }
            }
        } else {
            if ($(sidePanel).hasClass("windowed")) {
                var panelInstance = this.findPanelInstanceById(sidePanelIdentificator);
                panelInstance.childwindow.focus();
            }
            else if ($(sidePanel).hasClass("ui-draggable")) {
                $(sidePanel).show();
            } else {
                if (slideDirection) {
                    $(sidePanel).show("slide", { direction: slideDirection });
                } else {
                    $(sidePanel).css("display", "");
                }
            }
        }
    }

    findPanelInstanceById(panelIdentificator: string): SidePanelOld {
        for (var k = 0; k < this.leftSidePanels.length; k++) {
            if (this.leftSidePanels[k].identificator === panelIdentificator) {
                return this.leftSidePanels[k];
            }
        }

        for (var k = 0; k < this.rightSidePanels.length; k++) {
            if (this.rightSidePanels[k].identificator === panelIdentificator) {
                return this.rightSidePanels[k];
            }
        }

        return null;
    }

    private makeReaderBody(): HTMLDivElement {
        var bodyContainerDiv: HTMLDivElement = document.createElement("div");
        $(bodyContainerDiv).addClass("reader-body-container content-container");

        var textPanel: TextPanelOld = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
            textPanel = this.appendTextPanel(bodyContainerDiv);
        }

        if (this.showPanelList.indexOf(ReaderPanelEnum.ImagePanel) >= 0) {
            var imagePanel: ImagePanelOld =this.appendImagePanel(bodyContainerDiv);

            if (textPanel !== null && this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
                this.hasBookPage(this.bookId, this.versionId, null, () => {
                    $(textPanel.panelHtml).hide();
                    $(imagePanel.panelHtml).show();
                });

                this.hasBookImage(this.bookId, this.versionId, null, () => {
                    imagePanel.isDisabled = true;
                });
            }
        }

        return bodyContainerDiv;
    }

    protected appendTextPanel(bodyContainerDiv: HTMLDivElement): TextPanelOld {
        var textPanel: TextPanelOld = new TextPanelOld(this.textPanelIdentificator, this, this.showMainPanelsButtonList);
        this.rightSidePanels.push(textPanel);
        this.textPanel = textPanel;

        bodyContainerDiv.appendChild(textPanel.panelHtml);

        return textPanel;
    }
    
    protected appendImagePanel(bodyContainerDiv: HTMLDivElement): ImagePanelOld {
        var imagePanel: ImagePanelOld = new ImagePanelOld(this.imagePanelIdentificator, this, this.showMainPanelsButtonList);
        this.rightSidePanels.push(imagePanel);
        this.imagePanel = imagePanel;

        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {       //Text panel is higher priority
            $(imagePanel.panelHtml).hide();
        }

        bodyContainerDiv.appendChild(imagePanel.panelHtml);

        return imagePanel;
    }

    notifyPanelsMovePage(pageIndex: number, scrollTo: boolean) {
        for (var k = 0; k < this.leftSidePanels.length; k++) {
            this.leftSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }

        for (var k = 0; k < this.rightSidePanels.length; k++) {
            this.rightSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }
    }
    
    actualizeSlider(pageIndex: number) {
        var slider = $(this.readerContainer).find(".slider");
        $(slider).slider().slider("value", pageIndex);
        $(slider).find(".ui-slider-handle").find(".tooltip-inner").html(localization.translate("Page:", "PluginsJs").value + this.pages[pageIndex].text);
    }
        
    private createBookmarkSpan(pageIndex: number, pageName: string, pageId: number, title: string, tooltipTitle: string|(()=>string), favoriteLabel: IFavoriteLabel) {
        var positionStep = 100 / (this.pages.length - 1);
        var bookmarkSpan = document.createElement("span");
        var $bookmarkSpan = $(bookmarkSpan);

        $bookmarkSpan.addClass("glyphicon glyphicon-bookmark bookmark");
        $bookmarkSpan.data("page-index", pageIndex);
        $bookmarkSpan.data("page-name", pageName);
        $bookmarkSpan.data("page-xmlId", pageId);
        $bookmarkSpan.data("title", title);

        $bookmarkSpan.click(() => {
            this.moveToPage(pageId, true);
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
        var tooltipTitle = function() {
                var bookmarkTitle = $(this).data("title");
                return favoriteLabel
                    ? localization.translateFormat("Label", new Array<string>(bookmarkTitle, favoriteLabel.name))
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
            tooltipTitle = labelCount + localization.translate("BookmarksGt4", "PluginsJs").value;
        } else if (labelCount > 1) {
            tooltipTitle = labelCount + localization.translate("BookmarksLt4", "PluginsJs").value;
        } else {
            tooltipTitle = "1" + localization.translate("Bookmark", "PluginsJs").value;
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
    
    showBookmark(bookmarkHtml: HTMLSpanElement) {
        $(this.readerContainer).find(".slider").append(bookmarkHtml).promise();
    }

    recreateAndShowBookmarkSpan(bookmarkPosition: IBookmarkPosition) {
        var pageIndex = bookmarkPosition.pageIndex;
        var actualPage = this.pages[pageIndex];

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
        var page: BookPage = this.pages[pageIndex];
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
                this.newFavoriteDialog.showError(localization.translate("CreatingBookmarkError", "PluginsJs").value);
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
        
        this.favoriteManager.updateFavoriteItem(bookmarkId, title, () => { });
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

    repaint() {//no usages found
        for (var i = 0; i < this.leftSidePanels.length; i++) {
            if ($(this.leftSidePanels[i].panelHtml).is(":visible")) {
                $(this.leftSidePanels[i].panelHtml).hide();
                $(this.leftSidePanels[i].panelHtml).show();
            }
        }

        for (var i = 0; i < this.rightSidePanels.length; i++) {
            if ($(this.rightSidePanels[i].panelHtml).is(":visible")) {
                $(this.rightSidePanels[i].panelHtml).hide();
                $(this.rightSidePanels[i].panelHtml).show();
            }
        }
    }

    setRightPanelsLayout() {
        var rightPanels = this.rightSidePanels;
        var pinnedPanelsCount = 0;
        for (var i = 0; i < rightPanels.length; i++) {
            var panel = rightPanels[i].panelHtml;
            if ($(panel).is(":visible") && !$(panel).hasClass("ui-draggable")) {
                ++pinnedPanelsCount;
            }
        }

        if (pinnedPanelsCount > 1) {
            $(".reader-body-container").addClass("both-pinned");
            var leftPanels = this.leftSidePanels;
            for (var i = 0; i < leftPanels.length; i++) {
                var leftPanel = leftPanels[i];
                if (!leftPanel.isDraggable) {
                    leftPanel.pinButton.click();
                }
            }

        } else {
            $(".reader-body-container").removeClass("both-pinned");
        }
    }

    populatePanelOnTop(panel: SidePanelOld) {
        if (!panel.isDraggable) {
            return;
        }

        var max: number = 0;
        var leftPanels = this.leftSidePanels;
        for (var i = 0; i < leftPanels.length; i++) {
            var leftPanel = leftPanels[i];
            var zIndex = parseInt($(leftPanel.panelHtml).css("z-index"));
            if (zIndex > max) {
                max = zIndex;
            }
        }

        var rightPanels = this.rightSidePanels;
        for (var i = 0; i < rightPanels.length; i++) {
            var rightPanel = rightPanels[i];
            var zIndex = parseInt($(rightPanel.panelHtml).css("z-index"));
            if (zIndex > max) {
                max = zIndex;
            }
        }

        $(panel.panelHtml).css("z-index", max + 1);
    }

    //******** Reader search panel start ************
    
    showSearchInPanel(searchResults: Array<SearchHitResult>) {
        this.getSearchPanel().showResults(searchResults);
    }

    setResultsPaging(itemsCount: number, pageChangedCallback: (pageNumner: number) => void) {
        this.getSearchPanel().createPagination(pageChangedCallback, itemsCount);
    }

    getSearchResultsCountOnPage(): number {
        return this.getSearchPanel().getResultsCountOnPage();
    }

    private getSearchPanel(): SearchResultPanelOld {
        var panelId = this.searchPanelIdentificator;
        if (!this.existSidePanel(panelId)) {
            var searchPanel = new SearchResultPanelOld(panelId, this, this.showLeftSidePanelsButtonList);
            this.loadSidePanel(searchPanel.panelHtml);
            this.leftSidePanels.push(<any>searchPanel);
            this.searchPanel = searchPanel;
        }

        return this.searchPanel;
    }

    showSearchResultInPages(searchQuery: string, isQueryJson: boolean, pages: Array<IPage>) {
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
        this.moveToPageNumber(this.actualPageIndex, true);
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

    //******** Reader search panel end ************


    //******** Reader terms search panel start ************

    showSearchInTermsPanel(searchResults: Array<PageDescription>) {
        this.getTermsPanel().showResults(searchResults);
    }

    private getTermsPanel(): TermsPanelOld {
        var panelId = this.termsPanelIdentificator;
        if (!this.existSidePanel(panelId)) {
            var termPanel = new TermsPanelOld(panelId, this, this.showLeftSidePanelsButtonList);
            this.loadSidePanel(termPanel.panelHtml);
            this.leftSidePanels.push(<any>termPanel);
            this.termsPanel = termPanel;
        }

        return this.termsPanel;
    }

    setTermPanelCallback(callback: (termId: number, text: string) => void) {
        this.getTermsPanel().setTermClickedCallback(callback);
    }

    termsPanelShowLoading() {
        this.getTermsPanel().showLoading();

    }

    termsPanelRemoveLoading() {
        this.getTermsPanel().clearLoading();
    }

    termsPanelClearResults() {
        this.getTermsPanel().clearResults();
    }

    //******** Reader terms search panel end ************
    
}


class SidePanelOld {
    panelHtml: HTMLDivElement;
    panelBodyHtml: HTMLDivElement;
    closeButton: HTMLButtonElement;
    pinButton: HTMLButtonElement;
    newWindowButton: HTMLButtonElement;
    identificator: string;
    headerName: string;
    innerContent: HTMLElement;
    parentReader: ReaderModule;
    windowBody: HTMLDivElement;
    childwindow: Window;
    isDraggable: boolean;
    documentWindow: Window;

    protected panelHeaderDiv: HTMLDivElement;

    public constructor(identificator: string, headerName: string, parentReader: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        this.parentReader = parentReader;
        this.identificator = identificator;
        this.headerName = headerName;
        this.isDraggable = false;
        var sidePanelDiv: HTMLDivElement = document.createElement("div");
        sidePanelDiv.id = identificator;
        this.decorateSidePanel(sidePanelDiv);

        this.panelHeaderDiv = document.createElement("div");
        this.panelHeaderDiv.classList.add("reader-left-panel-header");

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("panel-header-name");
        nameSpan.innerHTML=headerName;
        this.panelHeaderDiv.appendChild(nameSpan);

        if (showPanelButtonList.indexOf(PanelButtonEnum.Close) >= 0) {
            var sidePanelCloseButton = document.createElement("button");
            $(sidePanelCloseButton).addClass("close-button");
            $(sidePanelCloseButton).click((event: JQuery.Event) => {
                this.onCloseButtonClick(sidePanelDiv);
            });

            var closeSpan = document.createElement("span");
            $(closeSpan).addClass("glyphicon glyphicon-remove");
            $(sidePanelCloseButton).append(closeSpan);

            this.closeButton = sidePanelCloseButton;

            this.panelHeaderDiv.appendChild(sidePanelCloseButton);
        }

        if(showPanelButtonList.indexOf(PanelButtonEnum.Pin) >= 0)
        {
            var panelPinButton = document.createElement("button");
            $(panelPinButton).addClass("pin-button");
            $(panelPinButton).click((event: JQuery.Event) => {
                this.onPinButtonClick(sidePanelDiv);
            });

            var pinSpan = document.createElement("span");
            $(pinSpan).addClass("glyphicon glyphicon-pushpin");
            $(panelPinButton).append(pinSpan);

            this.pinButton = panelPinButton;

            this.panelHeaderDiv.appendChild(panelPinButton);   
        }

        if (showPanelButtonList.indexOf(PanelButtonEnum.ToNewWindow) >= 0) {
            var newWindowButton = document.createElement("button");
            $(newWindowButton).addClass("new-window-button");
            $(newWindowButton).click((event: JQuery.Event) => {
                this.onNewWindowButtonClick(sidePanelDiv);
            });

            var windowSpan = document.createElement("span");
            $(windowSpan).addClass("glyphicon glyphicon-new-window");
            newWindowButton.appendChild(windowSpan);

            this.newWindowButton = newWindowButton;

            this.panelHeaderDiv.appendChild(newWindowButton);
        }

        //sidePanelDiv.appendChild(this.panelHeaderDiv);

        this.innerContent = this.makeBody(this, window);
        var panelBodyDiv = this.makePanelBody(this.innerContent, this, window);
        
        sidePanelDiv.appendChild(panelBodyDiv);

        $(sidePanelDiv).mousedown((event: JQuery.Event) => {
            this.parentReader.populatePanelOnTop(this);
        });

        this.panelHtml = sidePanelDiv;
        this.panelBodyHtml = panelBodyDiv;


    }

    protected  makePanelBody(innerContent, rootReference, window: Window): HTMLDivElement {
        var panelBodyDiv: HTMLDivElement = window.document.createElement("div");
        $(panelBodyDiv).addClass("reader-left-panel-body");
        panelBodyDiv.appendChild(innerContent);
        return panelBodyDiv;
    }

    protected makeBody(rootReference: SidePanelOld, window: Window): HTMLElement {
        throw new Error("Not implemented");
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
    }

    protected placeOnDragStartPosition(sidePanelDiv: HTMLDivElement) {
        var dispersion = Math.floor((Math.random() * 15) + 1) * 3;
        $(sidePanelDiv).css("top", 135 + dispersion);  //TODO kick out magic number
        $(sidePanelDiv).css("left", dispersion);
    }

    protected setRightPanelsLayout(sidePanelDiv: HTMLDivElement) {
        this.parentReader.setRightPanelsLayout();
    }

    makePanelWindow(documentWindow: Window): HTMLDivElement {
        return this.makePanelBody($(this.innerContent).clone(true), this, window);
    }

    decorateSidePanel(htmlDivElement: HTMLDivElement) { throw new Error("Not implemented"); }

    onNewWindowButtonClick(sidePanelDiv: HTMLDivElement) {
        this.closeButton.click();
        var newWindow = window.open("//" + document.domain, "_blank", "width=400,height=600,resizable=yes");
        newWindow.document.open();
        newWindow.document.close();

        $(newWindow.document.documentElement).on("beforeunload", (event: JQuery.Event) => {
            this.onUnloadWindowMode();
        });

        $(newWindow.document.getElementsByTagName("head")[0]).append($("script").clone(true));
        $(newWindow.document.getElementsByTagName("head")[0]).append($("link").clone(true));

        var panelWindow = this.makePanelWindow(newWindow);

        $(newWindow.document.getElementsByTagName("body")[0]).append(panelWindow);
        $(newWindow.document.getElementsByTagName("body")[0]).css("padding", 0);
        $(newWindow.document.getElementsByTagName("body")[0]).css("background-color", "white");
        newWindow.document.title = this.headerName;
        $(document.getElementById(this.identificator)).addClass("windowed");
        this.windowBody = panelWindow;
        this.childwindow = newWindow;
    }

    onUnloadWindowMode() {
        $(document.getElementById(this.identificator)).removeClass("windowed");
        $(this.windowBody).val("");
        $(this.childwindow.document.documentElement).val("");
    }

    onPinButtonClick(sidePanelDiv: HTMLDivElement) { throw new Error("Not implemented"); }

    onCloseButtonClick(sidePanelDiv: HTMLDivElement) { throw new Error("Not implemented"); }
}


class LeftSidePanel extends SidePanelOld {
    decorateSidePanel(sidePanelDiv: HTMLDivElement) {
        $(sidePanelDiv).addClass("reader-left-panel");
        
    }

    onPinButtonClick(sidePanelDiv: HTMLDivElement) {
        if ($(sidePanelDiv).data("ui-draggable")) {
            $(sidePanelDiv).draggable("destroy");
            $(sidePanelDiv).css("top", "");
            $(sidePanelDiv).css("left", "");
            $(sidePanelDiv).css("width", "");
            $(sidePanelDiv).css("height", "");
            $(sidePanelDiv).resizable("destroy");
            $(sidePanelDiv).resizable({ handles: "e", maxWidth: 250, minWidth: 100 });
            this.isDraggable = false;
            $(sidePanelDiv).css("z-index", 9999);

        } else {
            $(sidePanelDiv).draggable({ containment: "body", appendTo: "body", cursor: "move" });
            $(sidePanelDiv).resizable("destroy");
            $(sidePanelDiv).resizable({ handles: "all", minWidth: 100 });
            this.placeOnDragStartPosition(sidePanelDiv);
            this.isDraggable = true;
            this.parentReader.populatePanelOnTop(this);
        }

        this.setRightPanelsLayout(sidePanelDiv);
    }

    onCloseButtonClick(sidePanelDiv: HTMLDivElement) {
        if ($(sidePanelDiv).data("ui-draggable")) {
            $(sidePanelDiv).hide();
        } else {
            $(sidePanelDiv).hide("slide", { direction: "left" });
        }
    }
}

class SettingsPanel extends LeftSidePanel {

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, localization.translate("View", "PluginsJs").value, readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanelOld, window: Window): HTMLElement {
        var textButtonSpan = window.document.createElement("span");
        $(textButtonSpan).addClass("glyphicon glyphicon-text-size");
        var textButton = window.document.createElement("button");
        var $textButton = $(textButton);
        $textButton.addClass("reader-settings-button");
        $textButton.append(textButtonSpan);

        $(textButton).click((event: JQuery.Event) => {
            rootReference.parentReader.changeSidePanelVisibility(rootReference.parentReader.textPanelIdentificator, "");
            rootReference.parentReader.setRightPanelsLayout();
        });

        var imageButtonSpan = window.document.createElement("span");
        $(imageButtonSpan).addClass("glyphicon glyphicon-picture");
        var imageButton = window.document.createElement("button");
        var $imageButton = $(imageButton);
        $imageButton.addClass("reader-settings-button");
        $imageButton.append(imageButtonSpan);

        $(imageButton).click((event: JQuery.Event) => {
            rootReference.parentReader.changeSidePanelVisibility(rootReference.parentReader.imagePanelIdentificator, "");
            rootReference.parentReader.setRightPanelsLayout();
        });

        var buttonsDiv = window.document.createElement("div");
        $(buttonsDiv).addClass("reader-settings-buttons-area");
        
        buttonsDiv.appendChild(textButton);
        buttonsDiv.appendChild(imageButton);
        
        var checkboxesDiv = window.document.createElement("div");
        $(checkboxesDiv).addClass("reader-settings-checkboxes-area");

        var showPageCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        var showPageNameCheckbox: HTMLInputElement = window.document.createElement("input");
        showPageNameCheckbox.type = "checkbox";

        $(showPageNameCheckbox).change((eventData) => {
            var readerText:JQuery = $("#" + this.parentReader.textPanelIdentificator).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget as Node as HTMLElement);
            if (currentTarget.checked) {
                readerText.addClass("reader-text-show-page-names");
            } else {
                readerText.removeClass("reader-text-show-page-names");
            }
        });

        var showPageNameLabel: HTMLLabelElement = window.document.createElement("label");
        showPageNameLabel.innerHTML = localization.translate("ViewPagination", "PluginsJs").value;
        showPageCheckboxDiv.appendChild(showPageNameCheckbox);
        showPageCheckboxDiv.appendChild(showPageNameLabel);
        showPageNameCheckbox.id = "checkbox-show-page-numbers";
        showPageNameLabel.setAttribute("for", showPageNameCheckbox.id);

        var showPageOnNewLineDiv: HTMLDivElement = window.document.createElement("div");
        var showPageOnNewLineCheckbox: HTMLInputElement = window.document.createElement("input");
        showPageOnNewLineCheckbox.type = "checkbox";

        $(showPageOnNewLineCheckbox).change((eventData) => {
            var readerText = $("#" + this.parentReader.textPanelIdentificator).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget as Node as HTMLElement);
            if (currentTarget.checked) {
                $(readerText).addClass("reader-text-page-new-line");
            } else {
                $(readerText).removeClass("reader-text-page-new-line");
            }
        });

        var showPageOnNewLineLabel: HTMLLabelElement = window.document.createElement("label");
        showPageOnNewLineLabel.innerHTML = localization.translate("WrapPages", "PluginsJs").value;
        showPageOnNewLineDiv.appendChild(showPageOnNewLineCheckbox);
        showPageOnNewLineDiv.appendChild(showPageOnNewLineLabel);
        showPageOnNewLineCheckbox.id = "checkbox-page-breaks";
        showPageOnNewLineLabel.setAttribute("for", showPageOnNewLineCheckbox.id);

        var showCommentCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        var showCommentCheckbox: HTMLInputElement = window.document.createElement("input");
        showCommentCheckbox.type = "checkbox";

        $(showCommentCheckbox).change((eventData) => {
            var readerText = $("#" + this.parentReader.textPanelIdentificator).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget as Node as HTMLElement);
            if (currentTarget.checked) {
                $(readerText).addClass("show-notes");
            } else {
                $(readerText).removeClass("show-notes");
            }
        });

        var showCommentLabel: HTMLLabelElement = window.document.createElement("label");
        showCommentLabel.innerHTML = localization.translate("ViewComments", "PluginsJs").value;
        showCommentCheckboxDiv.appendChild(showCommentCheckbox);
        showCommentCheckboxDiv.appendChild(showCommentLabel);
        showCommentCheckbox.id = "checkbox-show-comment";
        showCommentLabel.setAttribute("for",showCommentCheckbox.id);

        rootReference.parentReader.hasBookPage(rootReference.parentReader.bookId, rootReference.parentReader.versionId, null, () => {
            textButton.disabled = true;
            showPageNameCheckbox.disabled = true;
            showPageOnNewLineCheckbox.disabled = true;
            showCommentCheckbox.disabled = true;
        });
        rootReference.parentReader.hasBookImage(rootReference.parentReader.bookId, rootReference.parentReader.versionId, null, () => {
            imageButton.disabled = true;
        });

        checkboxesDiv.appendChild(showPageCheckboxDiv);
        checkboxesDiv.appendChild(showPageOnNewLineDiv);
        checkboxesDiv.appendChild(showCommentCheckboxDiv);
        var innerContent: HTMLDivElement = window.document.createElement("div");
        var displaySettingsHead = document.createElement("h2");
        displaySettingsHead.innerHTML = localization.translate("ViewOptions", "PluginsJs").value;
        displaySettingsHead.classList.add("reader-view-head");
        innerContent.appendChild(displaySettingsHead);

        innerContent.appendChild(buttonsDiv);
        innerContent.appendChild(checkboxesDiv);

        return innerContent;
    }
}

class BookmarksPanelOld extends LeftSidePanel {
    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, localization.translate("Bookmarks", "PluginsJs").value, readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanelOld, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");
        this.createBookmarkList(innerContent, rootReference);
        return innerContent;
    }

    public createBookmarkList(
        innerContent: HTMLElement,
        rootReference: SidePanelOld
    ) {
        const bookmarksPerPage = 20;
        const actualBookmarkPage = 1;

        const $bookmarksContainer = $(innerContent).children(".reader-bookmarks-container");
        let bookmarksContainer: HTMLDivElement;

        if ($(innerContent).children(".reader-bookmarks-container").length == 0) {
            bookmarksContainer = document.createElement("div");
            bookmarksContainer.classList.add("reader-bookmarks-container");
            innerContent.appendChild(bookmarksContainer);
        }
        else {
            $bookmarksContainer.empty();
            bookmarksContainer = $bookmarksContainer.get(0) as Node as HTMLDivElement;
        }

        var bookmarksHead = document.createElement("h2");
        bookmarksHead.innerHTML = localization.translate("AllBookmarks", "PluginsJs").value;
        bookmarksHead.classList.add("reader-bookmarks-head");
        bookmarksContainer.appendChild(bookmarksHead);

        var bookmarksContent = document.createElement("div");
        bookmarksContent.classList.add("reader-bookmarks-content");
        bookmarksContainer.appendChild(bookmarksContent);

        var bookmarks = rootReference.parentReader.getBookmarks();

        var pageInContainer: Array<HTMLUListElement> = [];
        var pagesContainer: HTMLDivElement = document.createElement("div");
        bookmarksContent.appendChild(pagesContainer);

        var paginationContainer: HTMLDivElement = document.createElement("div");
        paginationContainer.classList.add("reader-bookmarks-pagination");
        bookmarksContent.appendChild(paginationContainer);

        for (let i = 0; i < Math.ceil(bookmarks.totalCount / bookmarksPerPage); i++) {
            pageInContainer[i] = document.createElement("ul");
            pageInContainer[i].classList.add("reader-bookmarks-content-list");
            pageInContainer[i].setAttribute("data-page-index", (i + 1).toString());
            if (i != actualBookmarkPage) {
                pageInContainer[i].classList.add("hide");
            }

            pagesContainer.appendChild(pageInContainer[i]);
        }

        var currentIndex = 0;
        for (let i = 0; i < bookmarks.positions.length; i++) {
            var bookmarkPosition = bookmarks.positions[i];
            for (let j = 0; j < bookmarkPosition.bookmarks.length; j++) {
                var bookmark = bookmarkPosition.bookmarks[j];
                var bookmarkElement = this.createBookmark(bookmark, rootReference, bookmarkPosition.pageIndex);
                pageInContainer[Math.floor(currentIndex / bookmarksPerPage)].appendChild(bookmarkElement);
                currentIndex++;
        }
    }
        
        const paginator = new Pagination({
            container: paginationContainer,
            pageClickCallback: (pageNumber: number) => this.showBookmarkPage(pagesContainer, pageNumber),
            callPageClickCallbackOnInit: true
        });
        paginator.make(bookmarks.totalCount, bookmarksPerPage, actualBookmarkPage);
        
        $(".pagination", paginationContainer).addClass("pagination-extra-small");
    }

    protected showBookmarkPage(pagesContainer: HTMLDivElement, page: number) {
        $(pagesContainer).children().addClass("hide");
        $(pagesContainer).children(`[data-page-index="${page}"]`).removeClass("hide");
    }

    protected createBookmark(bookmark: IBookPageBookmark, rootReference: SidePanelOld, pageIndex: number) {
        const bookmarkItem = document.createElement("li");
        bookmarkItem.classList.add("reader-bookmarks-content-item");

        const bookmarkRemoveIco = document.createElement("a");
        bookmarkRemoveIco.href = "#";
        bookmarkRemoveIco.classList.add("glyphicon", "glyphicon-trash", "bookmark-remote-ico");
        bookmarkItem.appendChild(bookmarkRemoveIco);

        bookmarkRemoveIco.addEventListener("click", (e) => {
            e.preventDefault();

            rootReference.parentReader.persistRemoveBookmark(pageIndex, bookmark.id);
        });

        const bookmarkIco = document.createElement("span");
        bookmarkIco.classList.add("glyphicon", "glyphicon-bookmark", "bookmark-ico");
        if (bookmark.favoriteLabel) {
            $(bookmarkIco).css("color", bookmark.favoriteLabel.color);
            $(bookmarkIco).attr("title", bookmark.favoriteLabel.name);
        }
        bookmarkItem.appendChild(bookmarkIco);

        const pageInfo = rootReference.parentReader.getPageByIndex(pageIndex);
        const page = document.createElement("a");
        page.href = "#";
        page.innerHTML = pageInfo.text;
        page.classList.add("reader-bookmarks-content-item-page");

        const actionHook = () => {
            var pageId = bookmark.pageId;
            rootReference.parentReader.moveToPage(pageId, true);
        };
        bookmarkIco.addEventListener("click", actionHook);
        page.addEventListener("click", actionHook);

        bookmarkItem.appendChild(page);
        bookmarkItem.appendChild(document.createTextNode(" "));

        const titleContainer = document.createElement("span");
        titleContainer.classList.add("reader-bookmarks-content-item-title-container");
        bookmarkItem.appendChild(titleContainer);

        const titleInput = document.createElement("input");
        titleInput.classList.add("reader-bookmarks-content-item-title-input", "hide");
        titleInput.value = bookmark.title;
        $(titleInput).attr("maxlength", FavoriteManager.maxTitleLength);
        bookmarkItem.appendChild(titleInput);

        const title = document.createElement("span");
        this.setBookmarkTitle(title, rootReference, bookmark.id, pageIndex, bookmark.title);
        title.classList.add("reader-bookmarks-content-item-title");

        titleContainer.addEventListener("click", () => {
            titleContainer.classList.add("hide");
            titleInput.classList.remove("hide");

            titleInput.focus();
        });
        const updateHook = () => {
            this.setBookmarkTitle(title, rootReference, bookmark.id, pageIndex, titleInput.value);

            titleInput.classList.add("hide");
            titleContainer.classList.remove("hide");
        };
        titleInput.addEventListener("blur", updateHook);
        titleInput.addEventListener("keyup", (e: KeyboardEvent) => {
            if (e.keyCode == 13) {
                updateHook();
            }
        });

        titleContainer.appendChild(title);
        titleContainer.appendChild(document.createTextNode(" "));

        const titleEdit = document.createElement("span");
        titleEdit.classList.add("glyphicon", "glyphicon-pencil", "edit-button");
        titleContainer.appendChild(titleEdit);

        return bookmarkItem;
    }
    
    protected setBookmarkTitle(titleItem: HTMLElement, rootReference: SidePanelOld, bookmarkId: number, pageIndex: number, title: string) {
        rootReference.parentReader.setBookmarkTitle(bookmarkId, pageIndex, title);

        if (!title) {
            //title = "&lt;bez názvu&gt;";
            title = localization.translateFormat("NoTitle", new Array<string>("&lt;", "&gt;")).value;
        }

        titleItem.innerHTML = title;
    }
}

class SearchResultPanelOld extends LeftSidePanel {
    private searchResultItemsDiv: HTMLDivElement;
    private searchPagingDiv: HTMLDivElement;

    private paginator: Pagination;
    private paginatorOptions: Pagination.Options;
    private resultsOnPage;
    private maxPaginatorVisibleElements;

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, localization.translate("Search", "PluginsJs").value, readerModule, showPanelButtonList);
    }

    showLoading() {
        $(this.searchResultItemsDiv).addClass("loader");

    }

    clearLoading() {
        $(this.searchResultItemsDiv).removeClass("loader");
    }

    clearResults() {
        $(this.searchResultItemsDiv).empty();
    }


    protected makeBody(rootReference: SidePanelOld, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");

        var searchResultItemsDiv = window.document.createElement("div");
        $(searchResultItemsDiv).addClass("reader-search-result-items-div");
        this.searchResultItemsDiv = searchResultItemsDiv;

        var pagingDiv = window.document.createElement("div");
        $(pagingDiv).addClass("reader-search-result-paging pagination-extra-small");
        this.searchPagingDiv = pagingDiv;

        this.resultsOnPage = 8;
        this.maxPaginatorVisibleElements = 11;

        this.paginatorOptions = {
            container: this.searchPagingDiv,
            maxVisibleElements: this.maxPaginatorVisibleElements,
            callPageClickCallbackOnInit: true
        };
        this.paginator = new Pagination(this.paginatorOptions);

        innerContent.appendChild(this.searchPagingDiv);
        innerContent.appendChild(searchResultItemsDiv);

        return innerContent;
    }

    createPagination(pageChangedCallback: (pageNumber: number) => void, itemsCount: number) {
        this.paginatorOptions.pageClickCallback = pageChangedCallback;
        this.paginator.make(itemsCount, this.resultsOnPage);
    }

    getResultsCountOnPage(): number {
        return this.resultsOnPage;
    }

    showResults(searchResults: SearchHitResult[]) {
        $(this.searchResultItemsDiv).empty();
        for (var i = 0; i < searchResults.length; i++) {
            var result = searchResults[i];
            var resultItem = this.createResultItem(result);
            this.searchResultItemsDiv.appendChild(resultItem);
        }
    }

    private createResultItem(result: SearchHitResult): HTMLDivElement {
        var resultItemDiv = document.createElement("div");
        $(resultItemDiv).addClass("reader-search-result-item");
        $(resultItemDiv).click(() => {
            var pageId = Number(result.pageId);
            this.parentReader.moveToPage(pageId, true);
        });

        var pageNameSpan = document.createElement("span");
        $(pageNameSpan).addClass("reader-search-result-name");
        pageNameSpan.innerHTML = result.pageName;

        var resultBeforeSpan = document.createElement("span");
        $(resultBeforeSpan).addClass("reader-search-result-before");
        resultBeforeSpan.innerHTML = result.before;

        var resultMatchSpan = document.createElement("span");
        $(resultMatchSpan).addClass("reader-search-result-match");
        resultMatchSpan.innerHTML = result.match;

        var resultAfterSpan = document.createElement("span");
        $(resultAfterSpan).addClass("reader-search-result-after");
        resultAfterSpan.innerHTML = result.after;

        resultItemDiv.appendChild(pageNameSpan);
        resultItemDiv.appendChild(resultBeforeSpan);
        resultItemDiv.appendChild(resultMatchSpan);
        resultItemDiv.appendChild(resultAfterSpan);

        return resultItemDiv;
    }
}

class ContentPanelOld extends LeftSidePanel {

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, localization.translate("Content", "PluginsJs").value, readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanelOld, window: Window): HTMLElement {
        var bodyDiv: HTMLDivElement = window.document.createElement("div");
        $(bodyDiv).addClass("content-panel-container");
        this.downloadBookContent();
        return bodyDiv;
    }

    private downloadBookContent() {

        $(this.panelBodyHtml).empty();
        $(this.panelBodyHtml).addClass("loader");

        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.parentReader.bookId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetBookContent",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var rootContentItems: IChapterHieararchyContract[] = response["content"];
                var ulElement = document.createElement("ul");
                $(ulElement).addClass("content-item-root-list");
                for (var i = 0; i < rootContentItems.length; i++) {
                    var chapterItem: IChapterHieararchyContract = rootContentItems[i];
                    $(ulElement).append(this.makeContentItem(this.parseJsonItemToContentItem(chapterItem)));
                }

                $(this.panelBodyHtml).removeClass("loader");
                $(this.panelBodyHtml).empty();
                $(this.panelBodyHtml).append(ulElement);

                this.innerContent = this.panelBodyHtml;

                if (typeof this.windowBody !== "undefined") {
                    $(this.windowBody).empty();
                    $(this.windowBody).append(ulElement);
                }
            },
            error: (response) => {
                $(this.panelBodyHtml).empty();
                $(this.panelBodyHtml).append(localization.translate("LoadingContentError", "PluginsJs").value);
            }
        });
    }

    private parseJsonItemToContentItem(chapterItem: IChapterHieararchyContract): ContentItem {
        var pageItem = chapterItem.beginningPageId != null
            ? this.parentReader.pagesById[chapterItem.beginningPageId]
            : null;
        var pageItemText = pageItem != null ? pageItem.text : "";

        return new ContentItem(chapterItem.name, chapterItem.beginningPageId,
            pageItemText, chapterItem.subChapters);
    }

    private makeContentItemChilds(contentItem: ContentItem): HTMLUListElement {
        var childItems: IChapterHieararchyContract[] = contentItem.childBookContentItems;
        if (childItems.length === 0) return null;
        var ulElement = document.createElement("ul");
        $(ulElement).addClass("content-item-list");
        for (var i = 0; i < childItems.length; i++) {
            var jsonItem: IChapterHieararchyContract = childItems[i];
            $(ulElement).append(this.makeContentItem(this.parseJsonItemToContentItem(jsonItem)));
        }
        return ulElement;
    }

    private makeContentItem(contentItem: ContentItem): HTMLLIElement {
        var liElement = document.createElement("li");
        $(liElement).addClass("content-item");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            this.parentReader.moveToPage(contentItem.referredPageId, true);
        });


        var textSpanElement = document.createElement("span");
        $(textSpanElement).addClass("content-item-text");
        textSpanElement.innerHTML = contentItem.text;

        var pageNameSpanElement = document.createElement("span");
        $(pageNameSpanElement).addClass("content-item-page-name");
        pageNameSpanElement.innerHTML = "[" + contentItem.referredPageName + "]";

        $(hrefElement).append(pageNameSpanElement);
        $(hrefElement).append(textSpanElement);

        $(liElement).append(hrefElement);
        $(liElement).append(this.makeContentItemChilds(contentItem));
        return liElement;
    }
}


class RightSidePanel extends SidePanelOld {
    decorateSidePanel(sidePanelDiv: HTMLDivElement) {
        $(sidePanelDiv).addClass("reader-right-panel");
    }

    onPinButtonClick(sidePanelDiv: HTMLDivElement) {
        if ($(sidePanelDiv).data("ui-draggable")) {
            $(sidePanelDiv).draggable("destroy");
            $(sidePanelDiv).css("top", "");
            $(sidePanelDiv).css("left", "");
            $(sidePanelDiv).css("width", "");
            $(sidePanelDiv).css("position", "");
            $(sidePanelDiv).css("height", "");
            $(sidePanelDiv).resizable("destroy");
            this.isDraggable = false;
            $(sidePanelDiv).css("z-index", 9999);

        } else {
            var height = $(sidePanelDiv).css("height");
            var width = $(sidePanelDiv).css("width");
            $(sidePanelDiv).draggable({ containment: "body", appendTo: "body", cursor: "move" });
            $(sidePanelDiv).resizable({ handles: "all", minWidth: 100 });
            $(sidePanelDiv).css("width", width);
            $(sidePanelDiv).css("height", height);
            this.placeOnDragStartPosition(sidePanelDiv);
            this.isDraggable = true;
            this.parentReader.populatePanelOnTop(this);
        }

        this.setRightPanelsLayout(sidePanelDiv);
    }

    onCloseButtonClick(sidePanelDiv: HTMLDivElement) {
        $(sidePanelDiv).hide();
        this.setRightPanelsLayout(sidePanelDiv);
    }

    onNewWindowButtonClick(sidePanelDiv: HTMLDivElement) {
        super.onNewWindowButtonClick(sidePanelDiv);
        this.setRightPanelsLayout(sidePanelDiv);
    }

    protected  makePanelBody(innerContent, rootReference, window: Window): HTMLDivElement {
        var panelBodyDiv: HTMLDivElement = window.document.createElement("div");
        $(panelBodyDiv).addClass("reader-right-panel-body");
        $(panelBodyDiv).append(innerContent);
        return panelBodyDiv;
    }
}

class ImagePanelOld extends RightSidePanel {
    isDisabled: boolean;

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, localization.translate("Pictures", "PluginsJs").value, readerModule, showPanelButtonList);
        this.isDisabled = false;
    }

    protected makeBody(rootReference: SidePanelOld, window: Window): HTMLElement {
        var imageContainerDiv: HTMLDivElement = window.document.createElement("div");
        imageContainerDiv.classList.add("reader-image-container");
        return imageContainerDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        var pageInfo = this.parentReader.pages[pageIndex];
        $(this.innerContent).empty();

        if (this.isDisabled)
            return;
        
        var image: HTMLImageElement = document.createElement("img");
        image.classList.add("reader-image");
        image.src = getBaseUrl() + "Reader/GetBookImage?snapshotId=" + this.parentReader.versionId + "&pageId=" + pageInfo.pageId;

        var imageLink: HTMLAnchorElement = document.createElement("a");
        imageLink.classList.add("no-click-href");
        imageLink.href = image.src;
        imageLink.onclick = (event: MouseEvent) => { return event.ctrlKey;};

        imageLink.appendChild(image);
        this.innerContent.appendChild(imageLink);

        var zoomOnClick = false;

        var img = new Image();
        img.onload = () => {
            var $innerContent = $(this.innerContent);

            if (zoomOnClick) {
                $innerContent.zoom({ on: "click" });
            } else {
                image.setAttribute("data-image-src", image.src);
                wheelzoom(image);

                var lastWidth = $innerContent.width();
                var lastHeight = $innerContent.height();
                $(window.document.documentElement).resize(() => {
                    var newWidth = $innerContent.width();
                    var newHeight = $innerContent.height();

                    if (lastWidth != newWidth || lastHeight != newHeight) {
                        image.src=image.getAttribute("data-image-src");

                        console.log(image);
                        wheelzoom(image);

                        lastWidth = newWidth;
                        lastHeight = newHeight;
                    }
                });
                
            }
        };
        img.src = getBaseUrl() + "Reader/GetBookImage?snapshotId=" + this.parentReader.versionId + "&pageId=" + pageInfo.pageId;

        if (typeof this.windowBody !== "undefined") {
            $(this.windowBody).empty();
            this.windowBody.appendChild(image);
        }
    }
}

class TextPanelOld extends RightSidePanel {
    preloadPagesBefore: number;
    preloadPagesAfter: number;

    private query: string; //search for text search
    private queryIsJson: boolean;

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, localization.translate("Text", "PluginsJs").value, readerModule, showPanelButtonList);
        this.preloadPagesBefore = 5;
        this.preloadPagesAfter = 10;
        
        if (readerModule.readerContainer.getAttribute("data-can-print")==="True") {
            const sidePanelPrintButton = document.createElement("button");
            sidePanelPrintButton.classList.add("print-button");
            sidePanelPrintButton.addEventListener("click", (event: Event) => {
                this.onPrintButtonClick(sidePanelPrintButton);
            });

            const printSpan = document.createElement("span");
            printSpan.classList.add("glyphicon","glyphicon-print");
            sidePanelPrintButton.appendChild(printSpan);

            this.panelHeaderDiv.appendChild(sidePanelPrintButton);
        }
    }

    protected makeBody(rootReference: SidePanelOld, window: Window): HTMLElement {
        var textContainerDiv: HTMLDivElement = window.document.createElement("div");
        $(textContainerDiv).addClass("reader-text-container");

        $(textContainerDiv).scroll((event) => {
            this.parentReader.clickedMoveToPage = false;

            var pages = $(event.target as Node as HTMLElement).find(".page");
            var minOffset = Number.MAX_VALUE;
            var pageWithMinOffset;
            $.each(pages, (index, page) => {
                var pageOfsset = Math.abs($(page as Node as Element).offset().top - $(event.target as Node as Element).offset().top);
                if (minOffset > pageOfsset) {
                    minOffset = pageOfsset;
                    pageWithMinOffset = page;
                }
            });

            rootReference.parentReader.moveToPage($(pageWithMinOffset).data("page-xmlId"), false);
        });

        var textAreaDiv: HTMLDivElement = window.document.createElement("div");
        $(textAreaDiv).addClass("reader-text");
        for (var i = 0; i < rootReference.parentReader.pages.length; i++) {
            var page: BookPage = rootReference.parentReader.pages[i];

            var pageTextDiv: HTMLDivElement = window.document.createElement("div");
            $(pageTextDiv).addClass("page");
            $(pageTextDiv).addClass("unloaded");
            $(pageTextDiv).data("page-name", page.text);
            $(pageTextDiv).data("page-xmlId", page.pageId);
            pageTextDiv.id = page.pageId.toString(); // each page has own id

            var pageNameDiv: HTMLDivElement = window.document.createElement("div");
            $(pageNameDiv).addClass("page-name");
            $(pageNameDiv).html("[" + page.text + "]");

            var pageDiv: HTMLDivElement = window.document.createElement("div");
            $(pageDiv).addClass("page-wrapper");
            $(pageDiv).append(pageTextDiv);
            $(pageDiv).append(pageNameDiv);
            textAreaDiv.appendChild(pageDiv);
        }

        var dummyPage: HTMLDivElement = window.document.createElement("div");
        $(dummyPage).addClass("dummy-page");
        textAreaDiv.appendChild(dummyPage);

        textContainerDiv.appendChild(textAreaDiv);
        return textContainerDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        //fetch page only if exist
        this.parentReader.hasBookPage(this.parentReader.bookId, this.parentReader.versionId, () => {
            for (var j = 1; pageIndex - j >= 0 && j <= this.preloadPagesBefore; j++) {
                this.displayPage(this.parentReader.pages[pageIndex - j], false);
            }
            for (var i = 1; pageIndex + i < this.parentReader.pages.length && i <= this.preloadPagesAfter; i++) {
                this.displayPage(this.parentReader.pages[pageIndex + i], false);
            }
            this.displayPage(this.parentReader.pages[pageIndex], scrollTo);
        });
    }

    displayPage(page: BookPage, scrollTo: boolean, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageDiv = document.getElementById(page.pageId.toString());
        var pageLoaded: boolean = !($(pageDiv).hasClass("unloaded"));
        var pageSearchUnloaded: boolean = $(pageDiv).hasClass("search-unloaded");
        var pageLoading: boolean = $(pageDiv).hasClass("load");
        if (!pageLoading) {
            if (pageSearchUnloaded) {
                this.downloadSearchPageByXmlId(this.query, this.queryIsJson, page, onSuccess, onFailed);
            }
            else if (!pageLoaded) {
                this.downloadPageByXmlId(page, onSuccess, onFailed);
            }
            else if (onSuccess!==null) {
                onSuccess();
            }
        }
        else if (onSuccess !== null) {
            onSuccess();
        }

        if (scrollTo) {
            this.scrollTextToPositionFromTop(0);
            var topOffset = $(pageDiv).offset().top;
            this.scrollTextToPositionFromTop(topOffset);

            if (typeof this.childwindow !== "undefined") {
                $(".reader-text-container", this.childwindow.document).scrollTop(0);
                var pageToScrollOffset = $("#" + page.pageId, this.childwindow.document).offset().top;
                $(".reader-text-container", this.childwindow.document).scrollTop(pageToScrollOffset);
            }
        }
    }

    scrollTextToPositionFromTop(topOffset: number) {
        var scrollableContainer = $(this.innerContent);
        var containerTopOffset = $(scrollableContainer).offset().top;
        $(scrollableContainer).scrollTop(topOffset - containerTopOffset);
    }

    onNewWindowButtonClick(sidePanelDiv: HTMLDivElement) {
        super.onNewWindowButtonClick(sidePanelDiv);
        var pageIndex = this.parentReader.actualPageIndex;
        $(this.childwindow.document.documentElement).ready(() => {
            this.parentReader.moveToPageNumber(pageIndex, true);
        });
    }

    onUnloadWindowMode() {
        super.onUnloadWindowMode();
        var pageIndex = this.parentReader.actualPageIndex;
        this.parentReader.moveToPageNumber(pageIndex, true);
    }

    private downloadPageByXmlId(page: BookPage, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageContainer = document.getElementById(page.pageId.toString());
        var loader = '<div class="lv-circles lv-mid sm"></div>';
        $(pageContainer).addClass("load");
        $(pageContainer).html(loader);
        if (typeof this.windowBody !== "undefined") {
            $(this.windowBody).find("#" + page.pageId).html(loader);
        }
        $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: this.parentReader.versionId, pageId: page.pageId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetBookPage",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                $(pageContainer).empty();
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("load");
                $(pageContainer).css("display", "inline");
                $(pageContainer).removeClass("unloaded");

                if (typeof this.windowBody !== "undefined") {
                    $(this.windowBody).find("#" + page.pageId).removeClass("load");
                    $(pageContainer).css("display", "inline");
                    $(this.windowBody).find("#" + page.pageId).append(response["pageText"]);
                }

                if (this.parentReader.clickedMoveToPage) {
                    this.parentReader.moveToPageNumber(this.parentReader.actualPageIndex, true);
                }

                if (onSuccess != null) {
                    onSuccess();
                }
            },
            error: (response) => {
                $(pageContainer).empty();
                $(pageContainer).css("display", "inline");
                $(pageContainer).append(localization.translateFormat("PageLoadingError", new Array<string>(page.text), "PluginsJs").value);

                if (onFailed != null) {
                    onFailed();
                }
            }
        });
    }

    private downloadSearchPageByXmlId(query: string, queryIsJson: boolean, page: BookPage, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageContainer = document.getElementById(page.pageId.toString());
        var loader = '<div class="lv-circles lv-mid sm"></div>';
        $(pageContainer).addClass("load");
        $(pageContainer).html(loader);
        if (typeof this.windowBody !== "undefined") {
            $(this.windowBody).find("#" + page.pageId).html(loader);
        }
        $.ajax({
            type: "GET",
            traditional: true,
            data: { query: query, isQueryJson: queryIsJson, snapshotId: this.parentReader.versionId, pageId: page.pageId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetBookSearchPageByXmlId",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                $(pageContainer).empty();
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("load");
                $(pageContainer).css("display", "inline");
                $(pageContainer).removeClass("unloaded");
                $(pageContainer).removeClass("search-unloaded");
                $(pageContainer).addClass("search-loaded");

                if (typeof this.windowBody !== "undefined") {
                    $(this.windowBody).find("#" + page.pageId).removeClass("load");
                    $(pageContainer).css("display", "inline");
                    $(this.windowBody).find("#" + page.pageId).append(response["pageText"]);
                }

                if (this.parentReader.clickedMoveToPage) {
                    this.parentReader.moveToPageNumber(this.parentReader.actualPageIndex, true);
                }

                if (onSuccess != null) {
                    onSuccess();
                }
            },
            error: (response) => {
                $(pageContainer).empty();
                $(pageContainer).removeClass("load");
                $(pageContainer).css("display", "inline");
                $(pageContainer).append(localization.translateFormat("PageLoadingErrorWithSearchResults", new Array<string>(page.text), "PluginsJs").value);

                if (onFailed != null) {
                    onFailed();
                }
            }
        });
    }

    public setSearchedQuery(query: string, isJson: boolean) {
        this.query = query;
        this.queryIsJson = isJson;
    }

    private onPrintButtonClick(button: HTMLButtonElement) {
        const loadedPages: Array<boolean> = [];
        const progress = new Progress("print-progress-bar", localization.translate("PreparingPrint", "PluginsJs").value,
        {
            body: {
                showLoading: true,
                afterLoadingText: localization.translate("Preparing", "PluginsJs").value,
                afterLoadingTextPosition: ProgressTextPosition.Center
            },
            update: {
                field: ProgressUpdateField.BodyAfterLoading,
                valueCallback: (value:number, max:number) => {
                    //return `Zpracováno ${value} z ${max}`;
                    return localization
                        .translateFormat("CompletedIndicator", new Array<string>(value.toString(), max.toString()), "PluginsJs").value;
                }
            }
        });
        progress.show();

        for (let i = 1; i < this.parentReader.pages.length; i++) {
            const j = i;

            const onFailed = () => {
                loadedPages[j] = false;

                this.displayPage(this.parentReader.pages[j], false, () => {
                    loadedPages[j] = true;

                    this.onLoadPage(loadedPages, progress);
                }, onFailed);
            };

            onFailed();
        }
    }

    private onLoadPage(loadedPages: Array<boolean>, progress: Progress) {
        let success = 0;
        let failed = 0;

        for (let i = 1; i < loadedPages.length; i++) {
            if (loadedPages[i]) {
                success++;
            }
            else {
                failed++;
            }
        }
        
        progress.update(success, this.parentReader.pages.length - 1);

        if (success === this.parentReader.pages.length - 1) {
            progress.hide();

            window.print();
        }
    }
}

class TermsPanelOld extends LeftSidePanel {
    private searchResultItemsDiv: HTMLDivElement;
    private termsResultItemsDiv: HTMLDivElement;
    private searchResultOrderedList: HTMLOListElement;
    private termsOrderedList: HTMLOListElement;

    private termsResultItemsLoadDiv: HTMLDivElement;
    private searchResultItemsLoadDiv: HTMLDivElement;

    private termClickedCallback: (termId: number, text: string) => void;

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, localization.translate("Topics", "PluginsJs").value, readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanelOld, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");
        $(innerContent).addClass("reader-terms-div");

        var searchResultDiv = window.document.createElement("div");
        $(searchResultDiv).addClass("reader-search-result-div");

        var searchResultDivHeading = window.document.createElement("h2");
        searchResultDivHeading.innerHTML = localization.translate("OccurrencesOnPage", "PluginsJs").value;
        searchResultDiv.appendChild(searchResultDivHeading);

        var searchResultItemsLoadDiv = window.document.createElement("div");
        $(searchResultItemsLoadDiv).addClass("reader-terms-search-result-items-div-load loader");
        this.searchResultItemsLoadDiv = searchResultItemsLoadDiv;
        $(searchResultItemsLoadDiv).hide();
        searchResultDiv.appendChild(searchResultItemsLoadDiv);

        var searchResultItemsDiv = window.document.createElement("div");
        $(searchResultItemsDiv).addClass("reader-terms-search-result-items-div");
        this.searchResultItemsDiv = searchResultItemsDiv;
        searchResultDiv.appendChild(searchResultItemsDiv);

        this.searchResultOrderedList = window.document.createElement("ol");

        this.searchResultItemsDiv.appendChild(this.searchResultOrderedList);

        innerContent.appendChild(searchResultDiv);


        var termsResultDiv = window.document.createElement("div");
        $(termsResultDiv).addClass("reader-terms-result-div");

        var termsResultDivHeading = window.document.createElement("h2");
        termsResultDivHeading.innerHTML = localization.translate("TopicsOnPage", "PluginsJs").value;
        termsResultDiv.appendChild(termsResultDivHeading);

        var termsResultItemsLoadDiv = window.document.createElement("div");
        $(termsResultItemsLoadDiv).addClass("reader-terms-result-items-div-load loader");
        this.termsResultItemsLoadDiv = termsResultItemsLoadDiv;
        $(termsResultItemsLoadDiv).hide();
        termsResultDiv.appendChild(termsResultItemsLoadDiv);

        var termsResultItemsDiv = window.document.createElement("div");
        $(termsResultItemsDiv).addClass("reader-terms-result-items-div");
        this.termsResultItemsDiv = termsResultItemsDiv;
        termsResultDiv.appendChild(termsResultItemsDiv);

        this.termsOrderedList = window.document.createElement("ol");

        this.termsResultItemsDiv.appendChild(this.termsOrderedList);

        innerContent.appendChild(termsResultDiv);

        var actualPage = this.parentReader.pages[this.parentReader.actualPageIndex];

        this.loadTermsOnPage(actualPage);
        this.clearResults();

        return innerContent;
    }

    

    showLoading() {
        $(this.searchResultItemsDiv).hide();
        $(this.searchResultItemsLoadDiv).show();

    }

    clearLoading() {
        $(this.searchResultItemsLoadDiv).hide();
        $(this.searchResultItemsDiv).show();
    }

    clearResults() {
        $(this.searchResultOrderedList).empty();
        $(this.searchResultOrderedList).append(localization.translate("UseSearch", "PluginsJs").value);
        $(this.searchResultOrderedList).addClass("no-items");
    }

    showResults(searchResults: PageDescription[]) {

        $(this.searchResultOrderedList).empty();
        $(this.searchResultOrderedList).removeClass("no-items");

        for (var i = 0; i < searchResults.length; i++) {
            var result = searchResults[i];
            var resultItem = this.createResultItem(result);
            this.searchResultOrderedList.appendChild(resultItem);
        }

        if (searchResults.length === 0) {
            $(this.searchResultOrderedList).addClass("no-items");
            $(this.searchResultOrderedList).append(localization.translate("NoOccurrencesOnPage", "PluginsJs").value);
        }
    }

    setTermClickedCallback(callback: (termId: number, text: string)=>void) {
        this.termClickedCallback = callback;
    }

    private createResultItem(page: PageDescription): HTMLLIElement {
        var resultItemListElement = document.createElement("li");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            this.parentReader.moveToPage(page.id, true);
        });

        var textSpanElement = document.createElement("span");
        textSpanElement.innerHTML = `[${page.name}]`;
        
        $(hrefElement).append(textSpanElement);

        $(resultItemListElement).append(hrefElement);

        return resultItemListElement;
    }

    private createTermItem(termId: number, text: string): HTMLLIElement {
        var termItemListElement = document.createElement("li");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            if (typeof this.termClickedCallback !== "undefined" && this.termClickedCallback !== null) {
                this.termClickedCallback(termId, text);   
            }
        });

        var textSpanElement = document.createElement("span");
        textSpanElement.innerHTML = `[${text}]`;

        $(hrefElement).append(textSpanElement);

        $(termItemListElement).append(hrefElement);

        return termItemListElement;
    }


    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        var page = this.parentReader.getPageByIndex(pageIndex);
        this.loadTermsOnPage(page);
    }

    private loadTermsOnPage(page: BookPage) {

        $(this.termsOrderedList).empty();
        $(this.termsOrderedList).removeClass("no-items");
        $(this.termsResultItemsLoadDiv).show();
        $(this.termsResultItemsDiv).hide();

        $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: this.parentReader.bookId, pageId: page.pageId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetTermsOnPage",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {

                if (page.pageId === this.parentReader.getActualPage().pageId) {

                    $(this.termsResultItemsLoadDiv).hide();
                    $(this.termsResultItemsDiv).show();

                    var terms = response["terms"] as Array<ITermContract>;
                    for (var i = 0; i < terms.length; i++) {
                        var term = terms[i];
                        this.termsOrderedList.appendChild(this.createTermItem(term.id, term.name));
                    }

                    if (terms.length === 0 && this.termsOrderedList.innerHTML == "") {
                        $(this.termsOrderedList).addClass("no-items");
                        $(this.termsOrderedList).append(localization.translate("NoTopicOnPage", "PluginsJs").value);
                    }
                }
            },
            error: (response) => {
                if (page.pageId === this.parentReader.getActualPage().pageId) {
                    $(this.termsResultItemsLoadDiv).hide();
                    $(this.termsResultItemsDiv).show();
                    $(this.termsOrderedList).addClass("no-items");
                    $(this.termsOrderedList).append(localization.translateFormat("LoadingTopicsError", new Array<string>(page.text), "PluginsJs").value);
                }
            }
        });
    }
}



class BookPage {
    private _pageId: number;
    private _text: string;
    private _position: number;

    constructor(pageId: number, text: string, position: number) {
        this._pageId = pageId;
        this._text = text;
        this._position = position;
    }

    get pageId(): number {
        return this._pageId;
    }

    get text(): string {
        return this._text;
    }

    get position(): number {
        return this._position;
    }
}

class ContentItem {
    private _referredPageId: number;
    private _referredPageName: string;
    private _text: string;
    private _childBookContentItems: IChapterHieararchyContract[];

    constructor(text: string, referredPageId: number, referredPageName: string, childBookContentItems: IChapterHieararchyContract[]) {
        this._referredPageId = referredPageId;
        this._referredPageName = referredPageName;
        this._text = text;
        this._childBookContentItems = childBookContentItems;
    }

    get referredPageId(): number {
        return this._referredPageId;
    }

    get referredPageName(): string {
        return this._referredPageName;
    }

    get text(): string {
        return this._text;
    }

    get childBookContentItems(): IChapterHieararchyContract[] {
        return this._childBookContentItems;
    }
}


class SearchHitResult {
    pageId: string;
    pageName: string;
    before: string;
    after: string;
    match: string;
}

class PageDescription {
    id: number;
    name: string;
}

interface IBookmarkPosition {
    bookmarks: Array<IBookPageBookmark>;
    bookmarkSpan: HTMLSpanElement;
    pageIndex: number;
}

interface IBookmarksInfo {
    positions: Array<IBookmarkPosition>;
    totalCount: number;
}

enum ReaderPanelEnum {
    TextPanel,
    ImagePanel,
    SearchPanel,
    ContentPanel,
    TermsPanel,
    SettingsPanel
}

enum PanelButtonEnum {
    Close,
    Pin,
    ToNewWindow
}