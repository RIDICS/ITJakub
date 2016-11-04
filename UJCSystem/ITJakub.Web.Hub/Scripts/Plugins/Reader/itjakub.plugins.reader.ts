/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />

class ReaderModule {

    private favoriteManager: FavoriteManager;
    private newFavoriteDialog: NewFavoriteDialog;
    readerContainer: HTMLDivElement;
    sliderOnPage: number;
    actualPageIndex: number;
    pages: Array<BookPage>;
    pagerDisplayPages: number;
    preloadPagesBefore: number;
    preloadPagesAfter: number;
    bookId: string;
    versionId: string;
    loadedBookContent: boolean;

    clickedMoveToPage: boolean;

    leftSidePanels: Array<SidePanel>;
    rightSidePanels: Array<SidePanel>;


    imagePanelIdentificator: string = "ImagePanel";
    textPanelIdentificator: string = "TextPanel";
    searchPanelIdentificator: string = "SearchPanel";
    termsPanelIdentificator: string = "TermsPanel";
    bookmarksPanelIdentificator: string = "BookmarksPanel";
    settingsPanelIdentificator: string = "SettingsPanel";
    contentPanelIdentificator: string = "ContentPanel";


    imagePanel: ImagePanel;
    textPanel: TextPanel;
    searchPanel: SearchResultPanel;
    bookmarksPanel: BookmarksPanel;
    settingsPanel: SettingsPanel;
    contentPanel: ContentPanel;
    termsPanel: TermsPanel;

    showPanelList: Array<ReaderPanelEnum>;
    showLeftSidePanelsButtonList: Array<PanelButtonEnum>;
    showMainPanelsButtonList: Array<PanelButtonEnum>;

    pageChangedCallback: (pageXmlId: string) => void;


    constructor(readerContainer: HTMLDivElement, pageChangedCallback: (pageXmlId: string) => void, showPanelList: Array<ReaderPanelEnum>, showLeftSidePanelsButtonList: Array<PanelButtonEnum>, showMainPanelsButtonList: Array<PanelButtonEnum>) {
        this.readerContainer = readerContainer;
        this.pageChangedCallback = pageChangedCallback;
        this.pagerDisplayPages = 5;
        this.showPanelList = showPanelList;
        this.showLeftSidePanelsButtonList = showLeftSidePanelsButtonList;
        this.showMainPanelsButtonList = showMainPanelsButtonList;
        this.favoriteManager = new FavoriteManager();
        this.newFavoriteDialog = new NewFavoriteDialog(this.favoriteManager, false);
    }

    public makeReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList) {
        this.bookId = bookXmlId;
        this.versionId = versionXmlId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<BookPage>();
        this.leftSidePanels = new Array<SidePanel>();
        this.rightSidePanels = new Array<SidePanel>();

        $(window).on("beforeunload", (event: Event) => {
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
            this.pages.push(new BookPage(page["XmlId"], page["Text"], page["Position"]));
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
                type: "POST",
                traditional: true,
                data: JSON.stringify({ bookId: bookId, versionId: bookVersionId }),
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
                type: "POST",
                traditional: true,
                data: JSON.stringify({ bookId: bookId, versionId: bookVersionId }),
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

    private makeTitle(bookTitle: string): HTMLDivElement {
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
                $(event.target).find(".ui-slider-handle").find(".slider-tip").show();
            },
            stop: (event, ui) => {
                $(event.target).find(".ui-slider-handle").find(".slider-tip").fadeOut(1000);
            },
            slide: (event, ui) => {
                $(event.target).find(".ui-slider-handle").find(".slider-tip").stop(true, true);
                $(event.target).find(".ui-slider-handle").find(".slider-tip").show();
                if (this.pages[ui.value] !== undefined) {
                    $(event.target).find(".ui-slider-handle").find(".tooltip-inner").html("Strana: " + this.pages[ui.value].text);
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
            $(innerTooltip).html("Strana: " + this.pages[0].text);
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
        pageInputText.setAttribute("placeholder", "Přejít na stranu...");
        pageInputText.classList.add("page-input-text");
        pageInputDiv.appendChild(pageInputText);

        var pageInputButton = document.createElement("button");
        $(pageInputButton).addClass("btn btn-default page-input-button");

        var pageInputButtonSpan = document.createElement("span");
        $(pageInputButtonSpan).addClass("glyphicon glyphicon-arrow-right");
        $(pageInputButton).append(pageInputButtonSpan);

        $(pageInputButton).click((event: Event) => {
            var pageName = $("#pageInputText").val();
            var pageIndex: number = -1;
            for (var i = 0; i < this.pages.length; i++) {
                if (this.pages[i].text === pageName) {
                    pageIndex = i;
                    break;
                }
            }

            if (this.pages[pageIndex] !== undefined) {
                var page: BookPage = this.pages[pageIndex];
                this.moveToPage(page.xmlId, true);
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

        var paginationUl: HTMLUListElement = document.createElement("ul");
        paginationUl.classList.add("pagination", "pagination-sm");

        var toLeft = document.createElement("ul");
        toLeft.classList.add("page-navigation-container","page-navigation-container-left");

        var liElement: HTMLLIElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        var anchor: HTMLAnchorElement = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = "|<";
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(0, true);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = "<<";
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex - 5, true);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = "<";
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex - 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        var toRight = document.createElement("ul");
        toRight.classList.add("page-navigation-container","page-navigation-container-right");

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = ">";
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex + 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = ">>";
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex + 5, true);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = ">|";
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.pages.length - 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-left");
        liElement.innerHTML = "...";
        paginationUl.appendChild(liElement);

        $.each(this.pages, (index, page) => {
            liElement = document.createElement("li");
            $(liElement).addClass("page");
            $(liElement).data("page-index", index);
            anchor = document.createElement("a");
            anchor.href = "#";
            anchor.innerHTML = page.text;
            $(anchor).click((event: Event) => {
                event.stopPropagation();
                this.moveToPage(page.xmlId, true);
                return false;
            });
            liElement.appendChild(anchor);
            paginationUl.appendChild(liElement);
        });

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-right");
        liElement.innerHTML = "...";
        paginationUl.appendChild(liElement);

        var listingContainer = document.createElement("div");
        listingContainer.classList.add("page-navigation-container-helper");
        listingContainer.appendChild(toLeft);
        listingContainer.appendChild(paginationUl);
        listingContainer.appendChild(toRight);

        var buttonsDiv: HTMLDivElement = document.createElement("div");
        $(buttonsDiv).addClass("buttons");

        var addBookmarkButton = document.createElement("button");
        $(addBookmarkButton).addClass("bookmark-button");

        var addBookmarkSpan = document.createElement("span");
        $(addBookmarkSpan).addClass("glyphicon glyphicon-bookmark");
        $(addBookmarkButton).append(addBookmarkSpan);

        var addBookmarkSpanText = document.createElement("span");
        $(addBookmarkSpanText).addClass("button-text");
        $(addBookmarkSpanText).append("Přidat/odebrat záložku");
        $(addBookmarkButton).append(addBookmarkSpanText);

        $(addBookmarkButton).click((event: Event) => {
            if (!this.removeBookmark()) {
                this.newFavoriteDialog.show("");
            }
        });

        var bookmarkButton = document.createElement("button");
        $(bookmarkButton).addClass("bookmark-button");

        var bookmarkSpan = document.createElement("span");
        $(bookmarkSpan).addClass("glyphicon glyphicon-bookmark");
        $(bookmarkButton).append(bookmarkSpan);

        var bookmarkSpanText = document.createElement("span");
        $(bookmarkSpanText).addClass("button-text");
        $(bookmarkSpanText).append("Záložky");
        $(bookmarkButton).append(bookmarkSpanText);

        $(bookmarkButton).click((event: Event) => {
            var panelId = this.bookmarksPanelIdentificator;
            if (!this.existSidePanel(panelId)) {
                var bookmarksPanel: BookmarksPanel = new BookmarksPanel(panelId, this, this.showLeftSidePanelsButtonList);
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
            $(settingsSpanText).append("Zobrazení");
            $(settingsButton).append(settingsSpanText);

            $(settingsButton).click((event: Event) => {
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
            $(searchSpanText).append("Vyhledávání");
            $(searchResultButton).append(searchSpanText);

            $(searchResultButton).click((event: Event) => {
                var panelId = this.searchPanelIdentificator;
                if (!this.existSidePanel(panelId)) {
                    var searchPanel = new SearchResultPanel(panelId, this, this.showLeftSidePanelsButtonList);
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
            $(termsSpanText).append("Témata");
            $(termsButton).append(termsSpanText);

            $(termsButton).click((event: Event) => {
                var panelId = this.termsPanelIdentificator;
                if (!this.existSidePanel(panelId)) {
                    var termsPanel = new TermsPanel(panelId, this, this.showLeftSidePanelsButtonList);
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
                $(contentSpanText).append("Obsah");
                $(contentButton).append(contentSpanText);

                $(contentButton).click((event: Event) => {
                    var panelId = this.contentPanelIdentificator;
                    if (!this.existSidePanel(panelId)) {
                        var contentPanel: ContentPanel = new ContentPanel(panelId, this, this.showLeftSidePanelsButtonList);
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

    private activateTypeahead(input: HTMLInputElement) {

        var pagesTexts = new Array<string>();
        $.each(this.pages, (index, page: BookPage) => {
            pagesTexts.push(page.text);
        });

        var pages = new Bloodhound({ datumTokenizer: Bloodhound.tokenizers.whitespace, queryTokenizer: Bloodhound.tokenizers.whitespace, local: (): string[] => { return pagesTexts; } });

        $(input).typeahead({ hint: true, highlight: true, minLength: 1 },{ name: "pages", source: pages });
    }

    private loadBookmarks() {
        this.favoriteManager.getPageBookmarks(this.bookId, (bookmarks) => {
            for (var i = 0; i < bookmarks.length; i++) {
                var bookmark = bookmarks[i];
                this.loadBookmark(bookmark);
            }
        });
    }

    private loadBookmark(actualBookmark: IBookPageBookmark) {
        for (var pageIndex = 0; pageIndex < this.pages.length; pageIndex++) {
            var actualPage = this.pages[pageIndex];
            if (actualBookmark.PageXmlId === actualPage.xmlId) {
                var bookmarkSpan: HTMLSpanElement = this.createBookmarkSpan(pageIndex, actualPage.text, actualPage.xmlId, actualBookmark.Title, actualBookmark.FavoriteLabel);
                $(bookmarkSpan).data("favorite-id", actualBookmark.Id);
                
                this.showBookmark(bookmarkSpan);
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

    findPanelInstanceById(panelIdentificator: string): SidePanel {
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

        var textPanel: TextPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
            textPanel = this.appendTextPanel(bodyContainerDiv);
        }

        if (this.showPanelList.indexOf(ReaderPanelEnum.ImagePanel) >= 0) {
            var imagePanel: ImagePanel =this.appendImagePanel(bodyContainerDiv);

            if (textPanel !== null && this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
                this.hasBookPage(this.bookId, this.versionId, null, () => {
                    $(textPanel.panelHtml).hide();
                    $(imagePanel.panelHtml).show();
                });
            }
        }

        return bodyContainerDiv;
    }

    protected appendTextPanel(bodyContainerDiv: HTMLDivElement): TextPanel {
        var textPanel: TextPanel = new TextPanel(this.textPanelIdentificator, this, this.showMainPanelsButtonList);
        this.rightSidePanels.push(textPanel);
        this.textPanel = textPanel;

        bodyContainerDiv.appendChild(textPanel.panelHtml);

        return textPanel;
    }

    protected appendImagePanel(bodyContainerDiv: HTMLDivElement): ImagePanel {
        var imagePanel: ImagePanel = new ImagePanel(this.imagePanelIdentificator, this, this.showMainPanelsButtonList);
        this.rightSidePanels.push(imagePanel);
        this.imagePanel = imagePanel;

        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {       //Text panel is higher priority
            $(imagePanel.panelHtml).hide();
        }

        bodyContainerDiv.appendChild(imagePanel.panelHtml);

        return imagePanel;
    }

    moveToPageNumber(pageIndex: number, scrollTo: boolean) {
        if (pageIndex < 0) {
            pageIndex = 0;
        } else if (pageIndex >= this.pages.length) {
            pageIndex = this.pages.length - 1;
        }

        if (!scrollTo) {
            this.clickedMoveToPage = true;
        }

        this.actualPageIndex = pageIndex;
        this.actualizeSlider(pageIndex);
        this.actualizePagination(pageIndex);
        this.notifyPanelsMovePage(pageIndex, scrollTo);

        var pageXmlId = this.pages[pageIndex].xmlId;
        this.pageChangedCallback(pageXmlId);
    }

    notifyPanelsMovePage(pageIndex: number, scrollTo: boolean) {
        for (var k = 0; k < this.leftSidePanels.length; k++) {
            this.leftSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }

        for (var k = 0; k < this.rightSidePanels.length; k++) {
            this.rightSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }
    }

    moveToPage(pageXmlId: string, scrollTo: boolean) {
        var pageIndex: number = -1;
        for (var i = 0; i < this.pages.length; i++) {
            if (this.pages[i].xmlId === pageXmlId) {
                pageIndex = i;
                break;
            }
        }
        if (pageIndex >= 0 && pageIndex < this.pages.length) {
            this.moveToPageNumber(pageIndex, scrollTo);

        } else {
            console.log("Page with id '" + pageXmlId + "' does not exist");
            //TODO tell user page not exist  
        }
    }

    actualizeSlider(pageIndex: number) {
        var slider = $(this.readerContainer).find(".slider");
        $(slider).slider().slider("value", pageIndex);
        $(slider).find(".ui-slider-handle").find(".tooltip-inner").html("Strana: " + this.pages[pageIndex].text);
    }

    actualizePagination(pageIndex: number) {
        var pager = $(this.readerContainer).find("ul.pagination");
        pager.find("li.page-navigation").css("visibility", "visible");
        pager.find("li.more-pages").css("visibility", "visible");
        if (pageIndex === 0) {
            pager.find("li.page-navigation-left").css("visibility", "hidden");
            pager.find("li.more-pages-left").css("visibility", "hidden");
        } else if (pageIndex === this.pages.length - 1) {
            pager.find("li.page-navigation-right").css("visibility", "hidden");
            pager.find("li.more-pages-right").css("visibility", "hidden");
        }

        var pages = $(pager).find(".page");
        $(pages).css("display", "none");
        $(pages).removeClass("page-active");
        var actualPage = $(pages).filter(function (index) {
            return $(this).data("page-index") === pageIndex;
        });

        var displayPagesOnEachSide = (this.pagerDisplayPages - 1) / 2;
        var displayOnRight = displayPagesOnEachSide;
        var displayOnLeft = displayPagesOnEachSide;
        var pagesOnLeft = pageIndex;
        var pagesOnRight = this.pages.length - (pageIndex + 1);
        if (pagesOnLeft <= displayOnLeft) {
            displayOnRight += displayOnLeft - pagesOnLeft;
            pager.find("li.more-pages-left").css("visibility", "hidden");
        } else if (pagesOnRight <= displayOnRight) {
            displayOnLeft += displayOnRight - pagesOnRight;
            pager.find("li.more-pages-right").css("visibility", "hidden");
        }

        var displayedPages = $(pages).filter(function (index) {
            var itemPageIndex = $(this).data("page-index");
            return (itemPageIndex >= pageIndex - displayOnLeft && itemPageIndex <= pageIndex + displayOnRight);
        });
        $(displayedPages).css("display", "inline-block");
        $(actualPage).addClass("page-active");

    }

    createBookmarkSpan(pageIndex: number, pageName: string, pageXmlId: string, title: string, favoriteLabel: IFavoriteLabel): HTMLSpanElement {
        var positionStep = 100 / (this.pages.length - 1);
        var bookmarkSpan = document.createElement("span");
        var $bookmarkSpan = $(bookmarkSpan);

        $bookmarkSpan.addClass("glyphicon glyphicon-bookmark bookmark");
        $bookmarkSpan.data("page-index", pageIndex);
        $bookmarkSpan.data("page-name", pageName);
        $bookmarkSpan.data("page-xmlId", pageXmlId);
        $bookmarkSpan.data("title", title);
        
        $bookmarkSpan.click(() => {
            this.moveToPage(pageXmlId, true);
        });

        if (favoriteLabel) {
            $bookmarkSpan.css("color", favoriteLabel.Color);
            $bookmarkSpan.data("label-id", favoriteLabel.Id);
            $bookmarkSpan.data("label-name", favoriteLabel.Name);
            $bookmarkSpan.data("label-color", favoriteLabel.Color);
        }

        var tooltipOptions: TooltipOptions = {
            placement: "bottom",
            title: function() {
                var bookmarkTitle = $(this).data("title");
                return favoriteLabel
                    ? bookmarkTitle + " (Štítek: " + favoriteLabel.Name + ")"
                    : bookmarkTitle;
            }
        };
        $bookmarkSpan.tooltip(tooltipOptions);

        var computedPosition = (positionStep * pageIndex);
        $bookmarkSpan.css("left", computedPosition + "%");

        return bookmarkSpan;
    }
    
    showBookmark(bookmarkHtml: HTMLSpanElement) {
        $(this.readerContainer).find(".slider").append(bookmarkHtml).promise();
    }

    getBookmarks():JQuery {
        const slider = $(this.readerContainer).find(".slider");
        const bookmarks = $(slider).find(".bookmark");

        const bookMap = {};

        for (let i = 0; i < bookmarks.length; i++) {
            const bookmark = bookmarks[i];
            const $bookmark = $(bookmark);
            bookMap[$bookmark.data("pageXmlId") + ($bookmark.hasClass("bookmark-local")?"_local":"_online")] = bookmark;
        }

        const outputBooks = [];
        for (let j = 0; j < this.pages.length; j++) {
            let page = this.pages[j];
            if (bookMap[page.xmlId + "_local"] !== undefined) {
                outputBooks.push(bookMap[page.xmlId + "_local"]);
            }
            if (bookMap[page.xmlId + "_online"] !== undefined) {
                outputBooks.push(bookMap[page.xmlId + "_online"]);
            }
        }

        return $(outputBooks);
    }

    createBookmarks(data: INewFavoriteItemData) {
        if (data.labels.length === 0) {
            // TODO possible create default label
            return;
        }

        var pageIndex: number = this.actualPageIndex;
        var page: BookPage = this.pages[pageIndex];

        var firstLabel = data.labels[0];
        var favoriteLabel: IFavoriteLabel = {
            Id: firstLabel.labelId,
            Name: firstLabel.labelName,
            Color: firstLabel.labelColor,
            IsDefault: false,
            LastUseTime: null
        }
        var bookmarkSpan: HTMLSpanElement = this.createBookmarkSpan(pageIndex, page.text, page.xmlId, data.itemName, favoriteLabel);

        const postShowAction = () => {
            const $bookmarksContainer = $(".reader-bookmarks-container");
            if (this.bookmarksPanel !== undefined && $bookmarksContainer.length > 0) {
                this.bookmarksPanel.createBookmarkList(
                    $bookmarksContainer.parent().get(0),
                    this.bookmarksPanel
                );
            }
        };

        this.favoriteManager.createPageBookmark(this.bookId, page.xmlId, data.itemName, firstLabel.labelId, (id, error) => {
            if (error) {
                this.newFavoriteDialog.showError("Chyba při vytváření záložky");
                return;
            }

            $(bookmarkSpan).data("favorite-id", id);
            this.newFavoriteDialog.hide();
            this.showBookmark(bookmarkSpan);
            postShowAction();
        });
    }
    
    setBookmarkTitle(targetBookmark: HTMLSpanElement, title: string) {
        var $targetBookmark = $(targetBookmark);
        var originalTitle = $targetBookmark.data("title");
        if (originalTitle === title) {
            return;
        }

        $targetBookmark.data("title", title);

        var favoriteId = Number($targetBookmark.data("favorite-id"));
        this.favoriteManager.updateFavoriteItem(favoriteId, title, () => {});
    }

    removeBookmark(): boolean {
        const bookmarks = this.getBookmarks();

        if (typeof bookmarks === "undefined" || bookmarks == null || bookmarks.length === 0) {
            return false;
        }

        var actualPage: BookPage = this.pages[this.actualPageIndex];
        var targetBookmark:JQuery = $(bookmarks).filter(function (index) {
            return $(this).data("page-xmlId") === actualPage.xmlId;
        });

        if (targetBookmark === undefined || targetBookmark == null || targetBookmark.length === 0) {
            return false;
        }

        this.persistRemoveBookmark(targetBookmark);
        
        return true;
    }

    public persistRemoveBookmark(targetBookmark: JQuery) {
        const postRemoveAction = () => {
            const $bookmarksContainer = $(".reader-bookmarks-container");
            if (this.bookmarksPanel !== undefined && $bookmarksContainer.length > 0) {
                this.bookmarksPanel.createBookmarkList(
                    $bookmarksContainer.parent().get(0),
                    this.bookmarksPanel
                );
            }
        };

        var favoriteItemId = targetBookmark.data("favorite-id");
        this.favoriteManager.deleteFavoriteItem(favoriteItemId, () => {
            targetBookmark.remove();

            postRemoveAction();
        });
    }

    repaint() {
        for (var i = 0; i < this.leftSidePanels.length; i++) {
            if ($(this.leftSidePanels[i]).is(":visible")) {
                $(this.leftSidePanels[i]).hide();
                $(this.leftSidePanels[i]).show();
            }
        }

        for (var i = 0; i < this.rightSidePanels.length; i++) {
            if ($(this.rightSidePanels[i]).is(":visible")) {
                $(this.rightSidePanels[i]).hide();
                $(this.rightSidePanels[i]).show();
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

    populatePanelOnTop(panel: SidePanel) {
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
    
    showSearchInPanel(searchResults: Array<SearchResult>) {
        this.getSearchPanel().showResults(searchResults);
    }

    setResultsPaging(itemsCount: number, pageChangedCallback: (pageNumner: number) => void) {
        this.getSearchPanel().createPagination(pageChangedCallback, itemsCount);
    }

    getSearchResultsCountOnPage(): number {
        return this.getSearchPanel().getResultsCountOnPage();
    }

    private getSearchPanel(): SearchResultPanel {
        var panelId = this.searchPanelIdentificator;
        if (!this.existSidePanel(panelId)) {
            var searchPanel = new SearchResultPanel(panelId, this, this.showLeftSidePanelsButtonList);
            this.loadSidePanel(searchPanel.panelHtml);
            this.leftSidePanels.push(<any>searchPanel);
            this.searchPanel = searchPanel;
        }

        return this.searchPanel;
    }

    showSearchResultInPages(searchQuery: string, isQueryJson: boolean, pages: Array<PageDescription>) {
        this.textPanel.setSearchedQuery(searchQuery, isQueryJson);
        $(".search-unloaded").removeClass(".search-unloaded");
        var previousSearchPages = $(".search-loaded");
        $(previousSearchPages).removeClass(".search-loaded");
        $(previousSearchPages).addClass("unloaded");
        for (var i = 0; i < pages.length; i++) {
            var page = pages[i];
            var pageDiv = document.getElementById(page.PageXmlId);
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

    private getTermsPanel(): TermsPanel {
        var panelId = this.termsPanelIdentificator;
        if (!this.existSidePanel(panelId)) {
            var termPanel = new TermsPanel(panelId, this, this.showLeftSidePanelsButtonList);
            this.loadSidePanel(termPanel.panelHtml);
            this.leftSidePanels.push(<any>termPanel);
            this.termsPanel = termPanel;
        }

        return this.termsPanel;
    }

    setTermPanelCallback(callback: (xmlId:string, text: string) => void) {
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


class SidePanel {
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
            $(sidePanelCloseButton).click((event: Event) => {
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
            $(panelPinButton).click((event: Event) => {
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
            $(newWindowButton).click((event: Event) => {
                this.onNewWindowButtonClick(sidePanelDiv);
            });

            var windowSpan = document.createElement("span");
            $(windowSpan).addClass("glyphicon glyphicon-new-window");
            newWindowButton.appendChild(windowSpan);

            this.newWindowButton = newWindowButton;

            this.panelHeaderDiv.appendChild(newWindowButton);
        }

        sidePanelDiv.appendChild(this.panelHeaderDiv);

        this.innerContent = this.makeBody(this, window);
        var panelBodyDiv = this.makePanelBody(this.innerContent, this, window);
        
        sidePanelDiv.appendChild(panelBodyDiv);

        $(sidePanelDiv).mousedown((event: Event) => {
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

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
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

        $(newWindow).on("beforeunload", (event: Event) => {
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
        $(this.childwindow).val("");
    }

    onPinButtonClick(sidePanelDiv: HTMLDivElement) { throw new Error("Not implemented"); }

    onCloseButtonClick(sidePanelDiv: HTMLDivElement) { throw new Error("Not implemented"); }
}


class LeftSidePanel extends SidePanel {
    decorateSidePanel(sidePanelDiv: HTMLDivElement) {
        $(sidePanelDiv).addClass("reader-left-panel");
        $(sidePanelDiv).resizable({
            handles: "e",
            maxWidth: 250,
            minWidth: 100
        });
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
        super(identificator, "Zobrazení", readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var textButtonSpan = window.document.createElement("span");
        $(textButtonSpan).addClass("glyphicon glyphicon-text-size");
        var textButton = window.document.createElement("button");
        var $textButton = $(textButton);
        $textButton.addClass("reader-settings-button");
        $textButton.append(textButtonSpan);

        $(textButton).click((event: Event) => {
            rootReference.parentReader.changeSidePanelVisibility(rootReference.parentReader.textPanelIdentificator, "");
            rootReference.parentReader.setRightPanelsLayout();
        });

        var imageButtonSpan = window.document.createElement("span");
        $(imageButtonSpan).addClass("glyphicon glyphicon-picture");
        var imageButton = window.document.createElement("button");
        var $imageButton = $(imageButton);
        $imageButton.addClass("reader-settings-button");
        $imageButton.append(imageButtonSpan);

        $(imageButton).click((event: Event) => {
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

        $(showPageNameCheckbox).change((eventData: Event) => {
            var readerText:JQuery = $("#" + this.parentReader.textPanelIdentificator).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            if (currentTarget.checked) {
                readerText.addClass("reader-text-show-page-names");
            } else {
                readerText.removeClass("reader-text-show-page-names");
            }
        });

        var showPageNameLabel: HTMLLabelElement = window.document.createElement("label");
        showPageNameLabel.innerHTML = "Zobrazit číslování stránek";
        showPageCheckboxDiv.appendChild(showPageNameCheckbox);
        showPageCheckboxDiv.appendChild(showPageNameLabel);
        showPageNameCheckbox.id = "checkbox-show-page-numbers";
        showPageNameLabel.setAttribute("for", showPageNameCheckbox.id);

        var showPageOnNewLineDiv: HTMLDivElement = window.document.createElement("div");
        var showPageOnNewLineCheckbox: HTMLInputElement = window.document.createElement("input");
        showPageOnNewLineCheckbox.type = "checkbox";

        $(showPageOnNewLineCheckbox).change((eventData: Event) => {
            var readerText = $("#" + this.parentReader.textPanelIdentificator).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            if (currentTarget.checked) {
                $(readerText).addClass("reader-text-page-new-line");
            } else {
                $(readerText).removeClass("reader-text-page-new-line");
            }
        });

        var showPageOnNewLineLabel: HTMLLabelElement = window.document.createElement("label");
        showPageOnNewLineLabel.innerHTML = "Zalamovat stránky";
        showPageOnNewLineDiv.appendChild(showPageOnNewLineCheckbox);
        showPageOnNewLineDiv.appendChild(showPageOnNewLineLabel);
        showPageOnNewLineCheckbox.id = "checkbox-page-breaks";
        showPageOnNewLineLabel.setAttribute("for", showPageOnNewLineCheckbox.id);

        var showCommentCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        var showCommentCheckbox: HTMLInputElement = window.document.createElement("input");
        showCommentCheckbox.type = "checkbox";

        $(showCommentCheckbox).change((eventData: Event) => {
            var readerText = $("#" + this.parentReader.textPanelIdentificator).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            if (currentTarget.checked) {
                $(readerText).addClass("show-notes");
            } else {
                $(readerText).removeClass("show-notes");
            }
        });

        var showCommentLabel: HTMLLabelElement = window.document.createElement("label");
        showCommentLabel.innerHTML = "Zobrazit komentáře";
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
        displaySettingsHead.innerHTML = "Možnosti zobrazení";
        displaySettingsHead.classList.add("reader-view-head");
        innerContent.appendChild(displaySettingsHead);

        innerContent.appendChild(buttonsDiv);
        innerContent.appendChild(checkboxesDiv);

        return innerContent;
    }
}

class BookmarksPanel extends LeftSidePanel {
    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, "Záložky", readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");
        this.createBookmarkList(innerContent, rootReference);
        return innerContent;
    }

    public createBookmarkList(
        innerContent: HTMLElement,
        rootReference: SidePanel
    ) {
        const bookmarksPerPage = 10;
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
            bookmarksContainer = <HTMLDivElement>$bookmarksContainer.get(0);
        }

        var bookmarksHead = document.createElement("h2");
        bookmarksHead.innerHTML = "Všechny záložky";
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

        for (let i = 0; i < Math.ceil(bookmarks.length / bookmarksPerPage); i++) {
            pageInContainer[i] = document.createElement("ul");
            pageInContainer[i].classList.add("reader-bookmarks-content-list");
            pageInContainer[i].setAttribute("data-page-index", (i + 1).toString());
            if (i != actualBookmarkPage) {
                pageInContainer[i].classList.add("hide");
            }

            pagesContainer.appendChild(pageInContainer[i]);
        }

        let j = 0;

        for (let i = 0; i < bookmarks.length; i++) {
            pageInContainer[Math.floor(j / bookmarksPerPage)].appendChild(
                this.createBookmark(
                    bookmarks[i],
                    rootReference,
                    $(bookmarks[i]).hasClass("bookmark-local")
                )
            );

            j++;
        }

        const paginator = new Pagination(<any>paginationContainer, 3);
        paginator.createPagination(bookmarks.length, bookmarksPerPage, (pageNumber: number) => {
            this.showBookmarkPage(
                pagesContainer,
                pageNumber
            );
        }, actualBookmarkPage);
    }

    protected showBookmarkPage(pagesContainer: HTMLDivElement, page: number) {
        $(pagesContainer).children().addClass("hide");
        $(pagesContainer).children(`[data-page-index="${page}"]`).removeClass("hide");
    }

    protected createBookmark(bookmark: HTMLElement, rootReference: SidePanel, local: boolean) {
        const $bookmark = $(bookmark);
        const bookmarkItem = document.createElement("li");
        bookmarkItem.classList.add("reader-bookmarks-content-item", local ? "reader-bookmarks-content-item-local" : "reader-bookmarks-content-item-online");

        const bookmarkRemoveIco = document.createElement("a");
        bookmarkRemoveIco.href = "#";
        bookmarkRemoveIco.classList.add("glyphicon", "glyphicon-trash", "bookmark-remote-ico");
        bookmarkItem.appendChild(bookmarkRemoveIco);

        bookmarkRemoveIco.addEventListener("click", (e) => {
            e.preventDefault();

            rootReference.parentReader.persistRemoveBookmark($(bookmark));
        });

        const bookmarkIco = document.createElement("span");
        bookmarkIco.classList.add("glyphicon", "glyphicon-bookmark", "bookmark-ico");
        $(bookmarkIco).css("color", $bookmark.data("label-color"));
        $(bookmarkIco).attr("title", $bookmark.data("label-name"));
        bookmarkItem.appendChild(bookmarkIco);

        const page = document.createElement("a");
        page.href = "#";
        page.innerHTML = $bookmark.data("pageName");
        page.classList.add("reader-bookmarks-content-item-page");

        const actionHook = () => {
            rootReference.parentReader.moveToPage($bookmark.data("page-xmlId"), true);
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
        titleInput.value = $bookmark.data("title");
        $(titleInput).attr("maxlength", FavoriteManager.maxTitleLength);
        bookmarkItem.appendChild(titleInput);

        const title = document.createElement("span");
        this.setBookmarkTitle(title, bookmark, rootReference, $bookmark.data("title"));
        title.classList.add("reader-bookmarks-content-item-title");

        titleContainer.addEventListener("click", () => {
            titleContainer.classList.add("hide");
            titleInput.classList.remove("hide");

            titleInput.focus();
        });
        const updateHook = () => {
            this.setBookmarkTitle(title, bookmark, rootReference, titleInput.value);

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

    protected setBookmarkTitle(titleItem: HTMLElement, bookmark: HTMLElement, rootReference: SidePanel, title: string) {
        rootReference.parentReader.setBookmarkTitle(bookmark, title);

        if (!title) {
            title = "&lt;bez názvu&gt;";
        }

        titleItem.innerHTML = title;
    }
}

class SearchResultPanel extends LeftSidePanel {
    private searchResultItemsDiv: HTMLDivElement;
    private searchPagingDiv: HTMLDivElement;

    private paginator: Pagination;
    private resultsOnPage;
    private maxPaginatorVisibleElements;

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, "Vyhledávání", readerModule, showPanelButtonList);
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


    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");

        var searchResultItemsDiv = window.document.createElement("div");
        $(searchResultItemsDiv).addClass("reader-search-result-items-div");
        this.searchResultItemsDiv = searchResultItemsDiv;

        var pagingDiv = window.document.createElement("div");
        $(pagingDiv).addClass("reader-search-result-paging");
        this.searchPagingDiv = pagingDiv;

        this.resultsOnPage = 8;
        this.maxPaginatorVisibleElements = 5;
        this.paginator = new Pagination(<any>this.searchPagingDiv, this.maxPaginatorVisibleElements);

        innerContent.appendChild(this.searchPagingDiv);
        innerContent.appendChild(searchResultItemsDiv);

        return innerContent;
    }

    createPagination(pageChangedCallback: (pageNumber: number) => void, itemsCount: number) {
        this.paginator.createPagination(itemsCount, this.resultsOnPage, pageChangedCallback);
    }

    getResultsCountOnPage(): number {
        return this.resultsOnPage;
    }

    showResults(searchResults: SearchResult[]) {
        $(this.searchResultItemsDiv).empty();
        for (var i = 0; i < searchResults.length; i++) {
            var result = searchResults[i];
            var resultItem = this.createResultItem(result);
            this.searchResultItemsDiv.appendChild(resultItem);
        }
    }

    private createResultItem(result: SearchResult): HTMLDivElement {
        var resultItemDiv = document.createElement("div");
        $(resultItemDiv).addClass("reader-search-result-item");
        $(resultItemDiv).click(() => {
            this.parentReader.moveToPage(result.pageXmlId, true);
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

class ContentPanel extends LeftSidePanel {

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, "Obsah", readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
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
            data: { bookId: this.parentReader.bookId },
            url: getBaseUrl() + "Reader/GetBookContent",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var rootContentItems: JSON[] = response["content"];
                var ulElement = document.createElement("ul");
                $(ulElement).addClass("content-item-root-list");
                for (var i = 0; i < rootContentItems.length; i++) {
                    var jsonItem: JSON = rootContentItems[i];
                    $(ulElement).append(this.makeContentItem(this.parseJsonItemToContentItem(jsonItem)));
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
                $(this.panelBodyHtml).append("Chyba při načítání obsahu");
            }
        });
    }

    private parseJsonItemToContentItem(jsonItem: JSON): ContentItem {
        return new ContentItem(jsonItem["Text"], jsonItem["ReferredPageXmlId"],
            jsonItem["ReferredPageName"], jsonItem["ChildBookContentItems"]);
    }

    private makeContentItemChilds(contentItem: ContentItem): HTMLUListElement {
        var childItems: JSON[] = contentItem.childBookContentItems;
        if (childItems.length === 0) return null;
        var ulElement = document.createElement("ul");
        $(ulElement).addClass("content-item-list");
        for (var i = 0; i < childItems.length; i++) {
            var jsonItem: JSON = childItems[i];
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
            this.parentReader.moveToPage(contentItem.referredPageXmlId, true);
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


class RightSidePanel extends SidePanel {
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

class ImagePanel extends RightSidePanel {

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, "Obrázky", readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var imageContainerDiv: HTMLDivElement = window.document.createElement("div");
        imageContainerDiv.classList.add("reader-image-container");
        return imageContainerDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        var pagePosition = pageIndex + 1;
        $(this.innerContent).empty();
      
        
        var image: HTMLImageElement = document.createElement("img");
        image.classList.add("reader-image");
        image.src = getBaseUrl() + "Editions/Editions/GetBookImage?bookId=" + this.parentReader.bookId + "&position=" + pagePosition;

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
                $(window).resize(() => {
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
        img.src = getBaseUrl() + "Editions/Editions/GetBookImage?bookId=" + this.parentReader.bookId + "&position=" + pagePosition;

        if (typeof this.windowBody !== "undefined") {
            $(this.windowBody).empty();
            this.windowBody.appendChild(image);
        }
    }
}

class TextPanel extends RightSidePanel {
    preloadPagesBefore: number;
    preloadPagesAfter: number;

    private query: string; //search for text search
    private queryIsJson: boolean;

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, "Text", readerModule, showPanelButtonList);
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

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var textContainerDiv: HTMLDivElement = window.document.createElement("div");
        $(textContainerDiv).addClass("reader-text-container");

        $(textContainerDiv).scroll((event: Event) => {
            this.parentReader.clickedMoveToPage = false;

            var pages = $(event.target).find(".page");
            var minOffset = Number.MAX_VALUE;
            var pageWithMinOffset;
            $.each(pages, (index, page) => {
                var pageOfsset = Math.abs($(page).offset().top - $(event.target).offset().top);
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
            $(pageTextDiv).data("page-xmlId", page.xmlId);
            pageTextDiv.id = page.xmlId; // each page has own id

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
        var pageDiv = document.getElementById(page.xmlId);
        var pageLoaded: boolean = !($(pageDiv).hasClass("unloaded"));
        var pageSearchUnloaded: boolean = $(pageDiv).hasClass("search-unloaded");
        var pageLoading: boolean = $(pageDiv).hasClass("loading");
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
                var pageToScrollOffset = $("#" + page.xmlId, this.childwindow.document).offset().top;
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
        $(this.childwindow.document).ready(() => {
            this.parentReader.moveToPageNumber(pageIndex, true);
        });
    }

    onUnloadWindowMode() {
        super.onUnloadWindowMode();
        var pageIndex = this.parentReader.actualPageIndex;
        this.parentReader.moveToPageNumber(pageIndex, true);
    }

    private downloadPageByXmlId(page: BookPage, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageContainer = document.getElementById(page.xmlId);
        $(pageContainer).addClass("loading");
        if (typeof this.windowBody !== "undefined") {
            $(this.windowBody).find("#" + page.xmlId).addClass("loading");
        }
        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.parentReader.bookId, pageXmlId: page.xmlId },
            url: getBaseUrl() + "Reader/GetBookPageByXmlId",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                $(pageContainer).empty();
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
                $(pageContainer).removeClass("unloaded");

                if (typeof this.windowBody !== "undefined") {
                    $(this.windowBody).find("#" + page.xmlId).removeClass("loading");
                    $(this.windowBody).find("#" + page.xmlId).append(response["pageText"]);
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
                $(pageContainer).removeClass("loading");
                $(pageContainer).append("Chyba při načítání stránky '" + page.text + "'");

                if (onFailed != null) {
                    onFailed();
                }
            }
        });
    }

    private downloadSearchPageByXmlId(query: string, queryIsJson: boolean, page: BookPage, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageContainer = document.getElementById(page.xmlId);
        $(pageContainer).addClass("loading");
        if (typeof this.windowBody !== "undefined") {
            $(this.windowBody).find("#" + page.xmlId).addClass("loading");
        }
        $.ajax({
            type: "GET",
            traditional: true,
            data: { query: query, isQueryJson: queryIsJson, bookId: this.parentReader.bookId, pageXmlId: page.xmlId },
            url: getBaseUrl() + "Reader/GetBookSearchPageByXmlId",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                $(pageContainer).empty();
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
                $(pageContainer).removeClass("unloaded");
                $(pageContainer).removeClass("search-unloaded");
                $(pageContainer).addClass("search-loaded");

                if (typeof this.windowBody !== "undefined") {
                    $(this.windowBody).find("#" + page.xmlId).removeClass("loading");
                    $(this.windowBody).find("#" + page.xmlId).append(response["pageText"]);
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
                $(pageContainer).removeClass("loading");
                $(pageContainer).append("Chyba při načítání stránky '" + page.text + "' s výsledky vyhledávání");

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
        const progress = new Progress("print-progress-bar", "Probíhá příprava díla pro tisk",
        {
            body: {
                showLoading: true,
                afterLoadingText: "Připravuji",
                afterLoadingTextPosition: ProgressTextPosition.Center
            },
            update: {
                field: ProgressUpdateField.BodyAfterLoading,
                valueCallback: (value:number, max:number) => {
                    return `Zpracováno ${value} z ${max}`;
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

class TermsPanel extends LeftSidePanel {
    private searchResultItemsDiv: HTMLDivElement;
    private termsResultItemsDiv: HTMLDivElement;
    private searchResultOrderedList: HTMLOListElement;
    private termsOrderedList: HTMLOListElement;

    private termsResultItemsLoadDiv: HTMLDivElement;
    private searchResultItemsLoadDiv: HTMLDivElement;

    private termClickedCallback: (xmlId: string, text: string) => void;

    constructor(identificator: string, readerModule: ReaderModule, showPanelButtonList: Array<PanelButtonEnum>) {
        super(identificator, "Témata", readerModule, showPanelButtonList);
    }

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");
        $(innerContent).addClass("reader-terms-div");

        var searchResultDiv = window.document.createElement("div");
        $(searchResultDiv).addClass("reader-search-result-div");

        var searchResultDivHeading = window.document.createElement("h2");
        searchResultDivHeading.innerHTML = "Výskyty na stránce";
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
        termsResultDivHeading.innerHTML = "Témata na stránce";
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
        $(this.searchResultOrderedList).append("Pro zobrazení výskytů použijte vyhledávání.");
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
            $(this.searchResultOrderedList).append("Žádné výskyty na stránce.");
        }
    }

    setTermClickedCallback(callback: (xmlId:string, text: string)=>void) {
        this.termClickedCallback = callback;
    }

    private createResultItem(page: PageDescription): HTMLLIElement {
        var resultItemListElement = document.createElement("li");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            this.parentReader.moveToPage(page.PageXmlId, true);
        });

        var textSpanElement = document.createElement("span");
        textSpanElement.innerHTML = `[${page.PageName}]`;
        
        $(hrefElement).append(textSpanElement);

        $(resultItemListElement).append(hrefElement);

        return resultItemListElement;
    }

    private createTermItem(xmlId: string, text: string): HTMLLIElement {
        var termItemListElement = document.createElement("li");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            if (typeof this.termClickedCallback !== "undefined" && this.termClickedCallback !== null) {
                this.termClickedCallback(xmlId, text);   
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
            data: { bookId: this.parentReader.bookId, pageXmlId: page.xmlId },
            url: getBaseUrl() + "Reader/GetTermsOnPage",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {

                if (page.xmlId === this.parentReader.getActualPage().xmlId) {

                    $(this.termsResultItemsLoadDiv).hide();
                    $(this.termsResultItemsDiv).show();

                    var terms = response["terms"];
                    for (var i = 0; i < terms.length; i++) {
                        var term = terms[i];
                        this.termsOrderedList.appendChild(this.createTermItem(term["XmlId"], term["Text"]));
                    }

                    if (terms.length === 0 && this.termsOrderedList.innerHTML == "") {
                        $(this.termsOrderedList).addClass("no-items");
                        $(this.termsOrderedList).append("Na této stránce se nenachází žádné téma");
                    }
                }
            },
            error: (response) => {
                if (page.xmlId === this.parentReader.getActualPage().xmlId) {
                    $(this.termsResultItemsLoadDiv).hide();
                    $(this.termsResultItemsDiv).show();
                    $(this.termsOrderedList).addClass("no-items");
                    $(this.termsOrderedList).append("Chyba při načítání témat na stránce '" + page.text + "'");
                }
            }
        });
    }
}



class BookPage {
    private _xmlId: string;
    private _text: string;
    private _position: number;

    constructor(xmlId: string, text: string, position: number) {
        this._xmlId = xmlId;
        this._text = text;
        this._position = position;
    }

    get xmlId(): string {
        return this._xmlId;
    }

    get text(): string {
        return this._text;
    }

    get position(): number {
        return this._position;
    }
}

class ContentItem {
    private _referredPageXmlId: string;
    private _referredPageName: string;
    private _text: string;
    private _childBookContentItems: JSON[];

    constructor(text: string, referredPageXmlId: string, referredPageName: string, childBookContentItems: JSON[]) {
        this._referredPageXmlId = referredPageXmlId;
        this._referredPageName = referredPageName;
        this._text = text;
        this._childBookContentItems = childBookContentItems;
    }

    get referredPageXmlId(): string {
        return this._referredPageXmlId;
    }

    get referredPageName(): string {
        return this._referredPageName;
    }

    get text(): string {
        return this._text;
    }

    get childBookContentItems(): JSON[] {
        return this._childBookContentItems;
    }
}


class SearchResult {
    pageXmlId: string;
    pageName: string;
    before: string;
    after: string;
    match; string;
}

class PageDescription {
    PageXmlId: string;
    PageName: string;
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