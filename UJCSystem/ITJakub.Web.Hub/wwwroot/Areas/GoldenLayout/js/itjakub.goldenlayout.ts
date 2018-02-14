﻿///<reference path="../../../lib/golden-layout/index.d.ts"/>
//import GoldenLayout = require("golden-layout");
//declare var GoldenLayout;

function initGoldenReader(bookId: string,
    versionId: string,
    bookTitle: string,
    pageList: any,
    searchedText?: string,
    initPageId?: string) {


    function readerPageChangedCallback(pageId: number) {
        updateQueryStringParameter("page", pageId);
    }

    var readerPanels = [ReaderPanelEnum.TextPanel, ReaderPanelEnum.ImagePanel, ReaderPanelEnum.ContentPanel, ReaderPanelEnum.SearchPanel, ReaderPanelEnum.SettingsPanel];

    var readerPlugin = new ReaderLayout(<any>$("#ReaderHeaderDiv")[0],
        readerPageChangedCallback,
        readerPanels);
    readerPlugin.makeReader(bookId, versionId, bookTitle, pageList);
}

class ReaderLayout {
    private favoriteManager: FavoriteManager;
    private newFavoriteDialog: NewFavoriteDialog;
    readerLayout: GoldenLayout;
    readerHeaderDiv: HTMLDivElement;
    sliderOnPage: number;
    actualPageIndex: number;
    pages: Array<BookPage>;
    pagesById: IDictionary<BookPage>;
    bookmarks: Array<IBookmarkPosition>;
    pagerDisplayPages: number;
    preloadPagesBefore: number;
    preloadPagesAfter: number;
    bookId: string;
    versionId: string;
    loadedBookContent: boolean;

    clickedMoveToPage: boolean;

    private bookmarksPanel: BookmarksPanel;

    toolPanels: Array<ToolPanel>;
    contentViewPanels: Array<ContentViewPanel>;

    audioPanelId: string = "audio";
    textPanelId: string = "text";
    imagePanelId: string = "image";
    contentPanelId: string = "content";
    bookmarksPanelId: string = "bookmarks";
    searchPanelId: string = "search";
    termsPanelId: string = "terms";
    termsOnPageId: string = "termsOnPage";
    occurOnPageId: string = "occurance";
  
    showPanelList: Array<ReaderPanelEnum>;

    pageChangedCallback: (pageId: number) => void;

    constructor(readerHeaderDiv: HTMLDivElement, pageChangedCallback: (pageId: number) => void, showPanelList: Array<ReaderPanelEnum>) {
        this.readerHeaderDiv = readerHeaderDiv;
        $(this.readerHeaderDiv).addClass("content-container");
        this.pageChangedCallback = pageChangedCallback;
        this.pagerDisplayPages = 5;
        this.showPanelList = showPanelList;
        this.favoriteManager = new FavoriteManager();
        this.newFavoriteDialog = new NewFavoriteDialog(this.favoriteManager, true);
    }

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

        var bookDetails = this.makeBookDetails(bookTitle);
        this.readerHeaderDiv.appendChild(bookDetails);

        var searchBar = this.makeSearchBar();
       // this.readerHeaderDiv.appendChild(searchBar);

        var controlsDiv = this.makeControls();
        this.readerHeaderDiv.appendChild(controlsDiv);

        this.readerLayout = this.initLayout();

        this.loadBookmarks();
        this.newFavoriteDialog.make();
        this.newFavoriteDialog.setSaveCallback(this.createBookmarks.bind(this));

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
                    $bookmarksContainer.parent().get(0),
                    this.bookmarksPanel
                );
            }
        };

        this.favoriteManager.createPageBookmark(Number(this.bookId), page.pageId, data.itemName, labelIds, (ids, error) => {
            if (error) {
                this.newFavoriteDialog.showError("Chyba při vytváření záložky");
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
        var tooltipTitle = function () {
            var bookmarkTitle = $(this).data("title");
            return favoriteLabel
                ? bookmarkTitle + " (Štítek: " + favoriteLabel.name + ")"
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
            tooltipTitle = labelCount + " záložek";
        } else if (labelCount > 1) {
            tooltipTitle = labelCount + " záložky";
        } else {
            tooltipTitle = "1 záložka";
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
                    $bookmarksContainer.parent().get(0),
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

    private makeBookDetails(bookTitle: string) : HTMLDivElement {
        var bookDetailsDiv: HTMLDivElement = document.createElement("div");
        $(bookDetailsDiv).addClass("book-details");

        var title = document.createElement("span");
        $(title).addClass("title");
        title.innerHTML = bookTitle;
        bookDetailsDiv.appendChild(title);

        var detailsButton = document.createElement("button");
        $(detailsButton).addClass("more-button");
        var detailsSpan = document.createElement("span");
        $(detailsSpan).addClass("glyphicon glyphicon-chevron-down");
        detailsButton.appendChild(detailsSpan);
        $(detailsButton).click((event) => {
            var target: JQuery = $(event.target);
            var details = target.parents(".book-details").find(".hidden-content");
            if (details.is(":hidden")) {
                $(target).removeClass("glyphicon-chevron-down");
                $(target).addClass("glyphicon-chevron-up");
                details.css("display", "block");
            } else {
                $(target).removeClass("glyphicon-chevron-up");
                $(target).addClass("glyphicon-chevron-down");
                details.css("display", "none");
            }
        });
        bookDetailsDiv.appendChild(detailsButton);

        var detailsDiv = document.createElement("div");
        $(detailsDiv).addClass("hidden-content");
        var editionNote = document.createElement("div");

        bookDetailsDiv.appendChild(detailsDiv);

        return bookDetailsDiv;
    }

    private makeControls(): HTMLDivElement {
        var controlsDiv = document.createElement("div");
        $(controlsDiv).addClass("reader-controls content-container");

        var pageSlider = this.makeSlider();
        controlsDiv.appendChild(pageSlider);

        var viewButtons = this.makeViewButtons();
        controlsDiv.appendChild(viewButtons);

        var toolButtons = this.makeToolButtons();
        controlsDiv.appendChild(toolButtons);

        var pageNavigation = this.makePageNavigation();
        controlsDiv.appendChild(pageNavigation);
 

        return controlsDiv;
    }

    private makeSlider() {
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
                    console.error("missing page " + ui.value);
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

        var sliderHandle: JQuery = $(slider).find(".ui-slider-handle");
        sliderHandle.append(sliderTooltip);
        sliderHandle.hover((event) => {
            $(event.target).find(".slider-tip").stop(true, true);
            $(event.target).find(".slider-tip").show();
        });
        sliderHandle.mouseout((event) => {
            $(event.target).find(".slider-tip").fadeOut(1000);
        });
        return slider;

    }

    private makePageNavigation(): HTMLDivElement {
        var paginationUl: HTMLUListElement = document.createElement("ul");
        paginationUl.classList.add("pagination", "pagination-sm");

        var toLeft = document.createElement("ul");
        toLeft.classList.add("page-navigation-container", "page-navigation-container-left");

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
        toRight.classList.add("page-navigation-container", "page-navigation-container-right");

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
                this.moveToPage(page.pageId, true);
                return false;
            });
            liElement.appendChild(anchor);
            paginationUl.appendChild(liElement);
        });

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-right");
        liElement.innerHTML = "...";
        paginationUl.appendChild(liElement);

        var listingContainer: HTMLDivElement = document.createElement("div");
        listingContainer.classList.add("page-navigation-container-helper");
        listingContainer.appendChild(toLeft);
        listingContainer.appendChild(paginationUl);
        listingContainer.appendChild(toRight);
        return listingContainer;
    }  

    private makeSearchBar(): HTMLDivElement {
        //TODO create searchBar
        return;
    }

    private makeToolButtons(): HTMLDivElement {
        var readerLayout = this;
        var toolButtons: HTMLDivElement = document.createElement("div");
        $(toolButtons).addClass("buttons left");

        var addBookmarksButton: HTMLButtonElement = document.createElement("button");
        $(addBookmarksButton).addClass("bookmark-button");
        var addBookmarksSpan = document.createElement("span");
        $(addBookmarksSpan).addClass("glyphicon glyphicon-bookmark");
        $(addBookmarksButton).append(addBookmarksSpan);

        var addBookmarksSpanText = document.createElement("span");
        $(addBookmarksSpanText).addClass("button-text");
        $(addBookmarksSpanText).append("Přidat záložku");
        $(addBookmarksButton).append(addBookmarksSpanText);


        $(addBookmarksButton).click((event: Event) => {
            var actualPageName = this.getActualPage().text;
            this.newFavoriteDialog.show(actualPageName);
        });
        toolButtons.appendChild(addBookmarksButton);


        var bookmarksButton: HTMLButtonElement = document.createElement("button");
        $(bookmarksButton).addClass("bookmark-button");
        var bookmarksSpan = document.createElement("span");
        $(bookmarksSpan).addClass("glyphicon glyphicon-bookmark");
        $(bookmarksButton).append(bookmarksSpan);

        var bookmarksSpanText = document.createElement("span");
        $(bookmarksSpanText).addClass("button-text");
        $(bookmarksSpanText).append("Záložky");
        $(bookmarksButton).append(bookmarksSpanText);

        $(bookmarksButton).click((event: Event) => {
            readerLayout.createToolPanel(this.bookmarksPanelId, bookmarksSpanText.innerHTML);
        });
        toolButtons.appendChild(bookmarksButton);


        var termsButton: HTMLButtonElement = document.createElement("button");
        $(termsButton).addClass("terms-button");
        var termsSpan = document.createElement("span");
        $(termsSpan).addClass("glyphicon glyphicon-list-alt");
        $(termsButton).append(termsSpan);

        var termsSpanText = document.createElement("span");
        $(termsSpanText).addClass("button-text");
        $(termsSpanText).append("Témata");
        $(termsButton).append(termsSpanText);

        $(termsButton).click((event: Event) => {
            readerLayout.createToolPanel(this.termsPanelId, termsSpanText.innerHTML);
        });
        toolButtons.appendChild(termsButton);


        var contentButton: HTMLButtonElement = document.createElement("button");
        $(contentButton).addClass("content-button");
        var contentSpan = document.createElement("span");
        $(contentSpan).addClass("glyphicon glyphicon-book");
        $(contentButton).append(contentSpan);

        var contentSpanText = document.createElement("span");
        $(contentSpanText).addClass("button-text");
        $(contentSpanText).append("Obsah");
        $(contentButton).append(contentSpanText);

        $(contentButton).click((event: Event) => {
            readerLayout.createToolPanel(this.contentPanelId, contentSpanText.innerHTML);
        });
        toolButtons.appendChild(contentButton);
        return toolButtons;    
    }

    private makeViewButtons(): HTMLDivElement {
        var readerLayout = this;
        var viewButtons: HTMLDivElement = document.createElement("div");
        $(viewButtons).addClass("buttons");

        var textButton: HTMLButtonElement = document.createElement("button");
        $(textButton).addClass("bookmark-button");
        var textSpan = document.createElement("span");
        $(textSpan).addClass("glyphicon glyphicon-bookmark");
        $(textButton).append(textSpan);

        var textSpanText = document.createElement("span");
        $(textSpanText).addClass("button-text");
        $(textSpanText).append("Text");
        $(textButton).append(textSpanText);

        $(textButton).click((event: Event) => {
            readerLayout.createViewPanel(this.textPanelId, textSpanText.innerHTML);
        });
        viewButtons.appendChild(textButton);


        var imageButton: HTMLButtonElement = document.createElement("button");
        $(imageButton).addClass("image-button");
        var imageSpan = document.createElement("span");
        $(imageSpan).addClass("glyphicon glyphicon-list-alt");
        $(imageButton).append(imageSpan);

        var imageSpanText = document.createElement("span");
        $(imageSpanText).addClass("button-text");
        $(imageSpanText).append("Obraz");
        $(imageButton).append(imageSpanText);

        $(imageButton).click((event: Event) => {
            readerLayout.createViewPanel(this.imagePanelId, imageSpanText.innerHTML);
        });
        viewButtons.appendChild(imageButton);


        var audioButton: HTMLButtonElement = document.createElement("button");
        $(audioButton).addClass("audio-button");
        var audioSpan = document.createElement("span");
        $(audioSpan).addClass("glyphicon glyphicon-book");
        $(audioButton).append(audioSpan);

        var audioSpanText = document.createElement("span");
        $(audioSpanText).addClass("button-text");
        $(audioSpanText).append("Zvuková stopa");
        $(audioButton).append(audioSpanText);

        $(audioButton).click((event: Event) => {
            readerLayout.createViewPanel(this.audioPanelId, audioSpanText.innerHTML);
        });
        viewButtons.appendChild(audioButton);
        return viewButtons;       
    }

    private createToolPanel(panelId: string, panelTitle: string) {

        if (this.readerLayout.root.getItemsById('tools').length === 0) {
            var toolStackConfig = {
                type: 'stack',
                width: 18,
                id: 'tools',
                componentName: 'toolTab'
            };
            this.readerLayout.root.contentItems[0].addChild(toolStackConfig, 0);
            this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
            this.readerLayout.updateSize();
        }
        if (this.readerLayout.root.getItemsById(panelId).length === 0) {
            var type: string;
            if (panelId === "terms") type = "column";
            else type = "component";
            var itemConfig = {
                type: type,
                id: panelId,
                componentState: { label: panelId },
                componentName: 'toolTab',
                title: panelTitle
            };
            this.readerLayout.root.getItemsById('tools')[0].addChild(itemConfig);
        }
    }

    private createViewPanel(panelId: string, panelTitle: string) {
        if (this.readerLayout.root.getItemsById('views').length === 0) {
            var viewColumnConfig = {
                isClosable: false,
                type: 'column',
                id: 'views',
                componentName: 'viewTab'
            };
            this.readerLayout.root.contentItems[0].addChild(viewColumnConfig);
        }
        if (this.readerLayout.root.getItemsById(panelId).length === 0) {
            var itemConfig = {
                type: 'component',
                id: panelId,
                componentState: { label: panelId },
                componentName: 'viewTab',
                title: panelTitle,
            };
            if (this.readerLayout.root.getItemsById('tools').length === 1) {
                this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
                this.readerLayout.updateSize();
            }
            if (panelId === "audio") {
                this.readerLayout.root.getItemsById('views')[0].addChild(itemConfig, 0);
                //TODO UpdateSize
            } else {
                if (this.readerLayout.root.getItemsById('viewsRow').length === 0) {
                    var viewRowConfig = {
                        isClosable: false,
                        type: 'row',
                        id: 'viewsRow',
                        componentName: 'viewTab'
                    };
                    this.readerLayout.root.getItemsById('views')[0].addChild(viewRowConfig);
                }
                this.readerLayout.root.getItemsById('viewsRow')[0].addChild(itemConfig);
            }
        }
    }

    moveToPage(pageId: number, scrollTo: boolean) {
        var pageIndex: number = -1;
        for (var i = 0; i < this.pages.length; i++) {
            if (this.pages[i].pageId === pageId) {
                pageIndex = i;
                break;
            }
        }
        if (pageIndex >= 0 && pageIndex < this.pages.length) {
            this.moveToPageNumber(pageIndex, scrollTo);

        } else {
            console.log("Page with id '" + pageId + "' does not exist");
            //TODO tell user page not exist  
        }
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

        var pageId = this.pages[pageIndex].pageId;
        this.pageChangedCallback(pageId);
    }

    actualizePagination(pageIndex: number) {
        var pager = $(this.readerHeaderDiv).find("ul.pagination");
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

    actualizeSlider(pageIndex: number) {
        var slider = $(this.readerHeaderDiv).find(".slider");
        $(slider).slider().slider("value", pageIndex);
        $(slider).find(".ui-slider-handle").find(".tooltip-inner").html("Strana: " + this.pages[pageIndex].text);
    }

    notifyPanelsMovePage(pageIndex: number, scrollTo: boolean) {
        for (var k = 0; k < this.toolPanels.length; k++) {
            this.toolPanels[k].onMoveToPage(pageIndex, scrollTo);
        }

        for (var k = 0; k < this.contentViewPanels.length; k++) {
            this.contentViewPanels[k].onMoveToPage(pageIndex, scrollTo);
        }
    }

    getActualPage(): BookPage {
        return this.pages[this.actualPageIndex];
    }

    getPageByIndex(pageIndex: number): BookPage {
        return this.pages[pageIndex];
    }

    private initLayout(): GoldenLayout {
        var module = this;
        var config = this.createConfig(this.textPanelId, "Text");
        var readerLayout = new GoldenLayout(config, $('#ReaderBodyDiv'));
        readerLayout.registerComponent('toolTab', function (container, state) {
            switch (state.label) {
                case module.bookmarksPanelId:
                    container.getElement().append(module.createBookmarksPanel());
                    break;
                case module.termsPanelId:
                    module.createTermsPanel();
                    break;
                case module.termsOnPageId:
                    //TODO create terms on page panel 
                    break;
                case module.occurOnPageId:
                    //TODO create occurance on page panel
                    break;
                case module.contentPanelId:
                    container.getElement().append(module.createContentPanel());
                    break;
                case module.searchPanelId:
                    container.getElement().append(module.createSearchPanel());
                    break;
                default:
                    break;
            }    
        });
        readerLayout.registerComponent('viewTab', function(container, state) {
            switch (state.label) {
            //case module.audioPanelId:
            //    container.getElement().append(module.createAudioPanel());
            //    break;
            case module.imagePanelId:
                container.getElement().append(module.createImagePanel());
                break;
            case module.textPanelId:
                container.getElement().append(module.createTextPanel());
              break;
            default:
                break;
            }        
        });
        readerLayout.init();
        readerLayout.on("stateChanged", function () {
            $(".reader-text-container").scroll();
        });
        $(window).resize(function () {
            readerLayout.updateSize();
        });
        return readerLayout;
    }

    private createConfig(panelId: string, panelTitle: string) {
        var layoutConfig = {
            dimensions: {
                headerHeight: 26
            },
            content: [{
                type: "row",
                isClosable: false,
                content: [{
                    type: "column",
                    id: "views",
                    content: [{
                        type: "row",
                        id: "viewsRow",
                        content: [{
                            type: 'component',
                            id: panelId,
                            componentState: { label: panelId },
                            componentName: 'viewTab',
                            title: panelTitle       
                        }]
                    }]
                }]                
            }]
        }
        return layoutConfig;    
    }

    private createBookmarksPanel(): HTMLDivElement {
        var bookmarksPanel: BookmarksPanel = new BookmarksPanel(this.bookmarksPanelId, this);
        this.bookmarksPanel = bookmarksPanel;
        this.toolPanels.push(bookmarksPanel);
        return bookmarksPanel.panelHtml;
    }

    private createContentPanel(): HTMLDivElement {
        var contentPanel: ContentPanel = null;
        contentPanel = new ContentPanel(this.contentPanelId, this);
        this.toolPanels.push(contentPanel);
        return contentPanel.panelHtml;
    }

    private createSearchPanel(): HTMLDivElement {
        var resultPanel: SearchResultPanel = null;
            resultPanel = new SearchResultPanel(this.searchPanelId, this);
            this.toolPanels.push(resultPanel);
        return resultPanel.panelHtml;
    }

    private createTermsPanel() {
        var itemConfig = {
            type: 'component',
            id: this.occurOnPageId,
            componentState: { label: this.occurOnPageId },
            componentName: 'viewTab',
            title: "Výskyty na stránce",
            isClosable: false
        };
        this.readerLayout.root.getItemsById(this.termsPanelId)[0].addChild(itemConfig);
        itemConfig = {
            type: 'component',
            id: this.termsOnPageId,
            componentState: { label: this.termsOnPageId },
            componentName: 'viewTab',
            title: "Témata na stránce",
            isClosable: false
        };   
        this.readerLayout.root.getItemsById(this.termsPanelId)[0].addChild(itemConfig);
    }

    private createTextPanel(): HTMLDivElement {
        var textPanel: TextPanel = null;
        textPanel = new TextPanel(this.textPanelId, this);
        this.contentViewPanels.push(textPanel);
        return textPanel.panelHtml;
    }

    private createImagePanel(): HTMLDivElement {
        var imagePanel: ImagePanel = null;
        imagePanel = new ImagePanel(this.imagePanelId, this);
        this.contentViewPanels.push(imagePanel);
        return imagePanel.panelHtml;
    }

    private createAudioPanel(): HTMLDivElement {
        var audioPanel: AudioPanel = null;
        audioPanel = new AudioPanel(this.audioPanelId, this);
        this.contentViewPanels.push(audioPanel);
        return audioPanel.panelHtml;
    }

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
                data: { bookId: bookId, snapshotId: bookVersionId },
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

    protected hasBookPageCache: { [key: string]: { [key: string]: boolean; }; } = {};
    protected hasBookPageCallOnSuccess: { [key: string]: { [key: string]: Array<() => any>; }; } = {};
}

abstract class SidePanel {
    panelHtml: HTMLDivElement;
    identificator: string;
    innerContent: HTMLElement;
    parentReader: ReaderLayout;

    constructor(identificator: string, readerLayout: ReaderLayout) {
        this.identificator = identificator;
        this.parentReader = readerLayout;
        var sidePanelDiv: HTMLDivElement = document.createElement("div");
        sidePanelDiv.id = identificator;
        this.addPanelClass(sidePanelDiv);

        this.innerContent = this.makeBody(this, window);

        sidePanelDiv.appendChild(this.innerContent);

        this.panelHtml = sidePanelDiv;
    }

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        throw new Error("Not implemented");
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        
    }

    protected addPanelClass(sidePanelDiv: HTMLDivElement){
        throw new Error("Not implemented");
    }
}

abstract class ToolPanel extends SidePanel {
    addPanelClass(sidePanelDiv: HTMLDivElement): void {
        $(sidePanelDiv).addClass("reader-tool-panel");
    }
}

class ContentPanel extends ToolPanel {
    makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var bodyDiv: HTMLDivElement = window.document.createElement("div");
        $(bodyDiv).addClass("content-panel-container");
        this.downloadBookContent();
        return bodyDiv;
    }

    private downloadBookContent() {

        $(this.panelHtml).empty();
        $(this.panelHtml).addClass("loader");

        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.parentReader.bookId },
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

                $(this.panelHtml).removeClass("loader");
                $(this.panelHtml).empty();
                $(this.panelHtml).append(ulElement);

                this.innerContent = this.panelHtml;

            },
            error: (response) => {
                $(this.panelHtml).empty();
                $(this.panelHtml).append("Chyba při načítání obsahu");
            }
        });
    }

    private parseJsonItemToContentItem(chapterItem: IChapterHieararchyContract): ContentItem {
        var pageItem = this.parentReader.pagesById[chapterItem.beginningPageId];
        return new ContentItem(chapterItem.name, chapterItem.beginningPageId,
            pageItem.text, chapterItem.subChapters);
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

class SearchResultPanel extends ToolPanel {
    private searchResultItemsDiv: HTMLDivElement;
    private searchPagingDiv: HTMLDivElement;

    private paginator: Pagination;
    private paginatorOptions: Pagination.Options;
    private resultsOnPage;
    private maxPaginatorVisibleElements;


    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
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

class BookmarksPanel extends ToolPanel {

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");
        this.createBookmarkList(innerContent, rootReference);
        return innerContent;
    }

    public createBookmarkList(innerContent: HTMLElement, rootReference: SidePanel) {
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

    protected createBookmark(bookmark: IBookPageBookmark, rootReference: SidePanel, pageIndex: number) {
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

    protected setBookmarkTitle(titleItem: HTMLElement, rootReference: SidePanel, bookmarkId: number, pageIndex: number, title: string) {
        rootReference.parentReader.setBookmarkTitle(bookmarkId, pageIndex, title);

        if (!title) {
            title = "&lt;bez názvu&gt;";
        }

        titleItem.innerHTML = title;
    }

}

abstract class TermsPanel extends ToolPanel {
    protected termClickedCallback: (termId: number, text: string) => void;

    setTermClickedCallback(callback: (termId: number, text: string) => void) {
        this.termClickedCallback = callback;
    }
}

class TermsSearchPanel extends TermsPanel {
    private searchResultItemsDiv: HTMLDivElement;
    private searchResultOrderedList: HTMLOListElement; 

    private searchResultItemsLoadDiv: HTMLDivElement;

    makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var searchResultDiv = window.document.createElement("div");
        $(searchResultDiv).addClass("reader-search-result-div");

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

        return searchResultDiv;
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

    private createResultItem(page: PageDescription): HTMLLIElement {
        var resultItemListElement = document.createElement("li");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            this.parentReader.moveToPage(page.pageId, true);
        });

        var textSpanElement = document.createElement("span");
        textSpanElement.innerHTML = `[${page.pageName}]`;

        $(hrefElement).append(textSpanElement);

        $(resultItemListElement).append(hrefElement);

        return resultItemListElement;
    }
}

class TermsResultPanel extends TermsPanel {

    private termsResultItemsDiv: HTMLDivElement;
    private termsOrderedList: HTMLOListElement;

    private termsResultItemsLoadDiv: HTMLDivElement;
    makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var termsResultDiv = window.document.createElement("div");
        $(termsResultDiv).addClass("reader-terms-result-div");

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

        var actualPage = this.parentReader.pages[this.parentReader.actualPageIndex];
        this.loadTermsOnPage(actualPage);

        return termsResultItemsDiv;
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
            data: { snapshotId: this.parentReader.bookId, pageId: page.pageId },
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
                        $(this.termsOrderedList).append("Na této stránce se nenachází žádné téma");
                    }
                }
            },
            error: (response) => {
                if (page.pageId === this.parentReader.getActualPage().pageId) {
                    $(this.termsResultItemsLoadDiv).hide();
                    $(this.termsResultItemsDiv).show();
                    $(this.termsOrderedList).addClass("no-items");
                    $(this.termsOrderedList).append("Chyba při načítání témat na stránce '" + page.text + "'");
                }
            }
        });
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
}
//end of tool panels

abstract class ContentViewPanel extends SidePanel {
    addPanelClass(sidePanelDiv: HTMLDivElement): void {
        $(sidePanelDiv).addClass("reader-right-panel");
    }
}

class TextPanel extends ContentViewPanel {
    preloadPagesBefore: number;
    preloadPagesAfter: number;

    private query: string; //search for text search
    private queryIsJson: boolean;

    constructor(identificator: string, readerLayout: ReaderLayout) {
        super(identificator, readerLayout);
        this.preloadPagesBefore = 5;
        this.preloadPagesAfter = 10;
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
        var pageLoading: boolean = $(pageDiv).hasClass("loading");
        if (!pageLoading) {
            if (pageSearchUnloaded) {
                this.downloadSearchPageById(this.query, this.queryIsJson, page, onSuccess, onFailed);
            }
            else if (!pageLoaded) {
                this.downloadPageById(page, onSuccess, onFailed);
            }
            else if (onSuccess !== null) {
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
            
        }
    }

    scrollTextToPositionFromTop(topOffset: number) {
        var scrollableContainer = $(this.innerContent);
        var containerTopOffset = $(scrollableContainer).offset().top;
        $(scrollableContainer).scrollTop(topOffset - containerTopOffset);
    }

    private downloadPageById(page: BookPage, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageContainer = document.getElementById(page.pageId.toString());
        $(pageContainer).addClass("loading");
        $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: this.parentReader.versionId, pageId: page.pageId },
            url: getBaseUrl() + "Reader/GetBookPage",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                $(pageContainer).empty();
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
                $(pageContainer).removeClass("unloaded");

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

    private downloadSearchPageById(query: string, queryIsJson: boolean, page: BookPage, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageContainer = document.getElementById(page.pageId.toString());
        $(pageContainer).addClass("loading");
        $.ajax({
            type: "GET",
            traditional: true,
            data: { query: query, isQueryJson: queryIsJson, snapshotId: this.parentReader.versionId, pageId: page.pageId },
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
}

class ImagePanel extends ContentViewPanel {
    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var imageContainerDiv: HTMLDivElement = window.document.createElement("div");
        imageContainerDiv.classList.add("reader-image-container");
        return imageContainerDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        var pageInfo = this.parentReader.pages[pageIndex];
        $(this.innerContent).empty();

        var image: HTMLImageElement = document.createElement("img");
        image.classList.add("reader-image");
        image.src = getBaseUrl() + "Reader/GetBookImage?snapshotId=" + this.parentReader.versionId + "&pageId=" + pageInfo.pageId;

        var imageLink: HTMLAnchorElement = document.createElement("a");
        imageLink.classList.add("no-click-href");
        imageLink.href = image.src;
        imageLink.onclick = (event: MouseEvent) => { return event.ctrlKey; };

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
                        image.src = image.getAttribute("data-image-src");

                        console.log(image);
                        wheelzoom(image);

                        lastWidth = newWidth;
                        lastHeight = newHeight;
                    }
                });

            }
        };
        img.src = getBaseUrl() + "Reader/GetBookImage?snapshotId=" + this.parentReader.versionId + "&pageId=" + pageInfo.pageId;

    }
}

class AudioPanel extends ContentViewPanel {
    protected

    makeBody(rootReference: SidePanel, window: Window): HTMLElement {
         throw new Error("Not implemented");
    }
}