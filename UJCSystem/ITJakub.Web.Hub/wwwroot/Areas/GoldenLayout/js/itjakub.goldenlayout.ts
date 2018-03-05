///<reference path="../../../lib/golden-layout/index.d.ts"/>
//import GoldenLayout = require("golden-layout");
//declare var GoldenLayout;

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
    sc: ServerCommunication;

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

        var bookInformationDiv = this.makeBookInformation(bookTitle);
        this.readerHeaderDiv.appendChild(bookInformationDiv);

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

    private makeBookInformation(bookTitle: string): HTMLDivElement {
        var bookInfoDiv: HTMLDivElement = document.createElement("div");
        $(bookInfoDiv).addClass("book-details");

        var title = document.createElement("span");
        $(title).addClass("title");
        title.innerHTML = bookTitle;
        bookInfoDiv.appendChild(title);

        var fullscreenButton = document.createElement("button");
        $(fullscreenButton).addClass("fullscreen-button");

        var fullscreenSpan = document.createElement("span");
        $(fullscreenSpan).addClass("glyphicon glyphicon-fullscreen");
        $(fullscreenButton).append(fullscreenSpan);
        $(fullscreenButton).click((event) => {
            if ($(fullscreenSpan).hasClass("glyphicon-fullscreen")) {
                $(this.readerHeaderDiv).parents("#ReaderDiv").addClass("fullscreen");
                $(fullscreenSpan).removeClass("glyphicon-fullscreen");
                $(fullscreenSpan).addClass("glyphicon-remove");
                this.readerLayout.updateSize();
            } else {
                $(this.readerHeaderDiv).parents("#ReaderDiv").removeClass("fullscreen");
                $(fullscreenSpan).removeClass("glyphicon-remove");
                $(fullscreenSpan).addClass("glyphicon-fullscreen");
                this.readerLayout.updateSize();
            }

        });
        bookInfoDiv.appendChild(fullscreenButton);

        var detailsButton = document.createElement("button");
        $(detailsButton).addClass("more-button");
        var detailsSpan = document.createElement("span");
        $(detailsSpan).addClass("glyphicon glyphicon-chevron-down");
        detailsButton.appendChild(detailsSpan);
        $(detailsButton).click((event) => {
            var target: JQuery = $(event.target);

            var title = target.parents(".book-details").find(".title");
            title.toggleClass("full")

            var details = target.parents(".book-details").find(".hidden-content");
            if (!details.hasClass("visible")) {
                $(target).removeClass("glyphicon-chevron-down");
                $(target).addClass("glyphicon-chevron-up");
                details.addClass("visible");
            } else {
                $(target).removeClass("glyphicon-chevron-up");
                $(target).addClass("glyphicon-chevron-down");
                details.removeClass("visible");
            }
        });
        bookInfoDiv.appendChild(detailsButton);

        var hiddenDiv = document.createElement("div");
        $(hiddenDiv).addClass("hidden-content");



        var editionNoteDiv = document.createElement("div");
        $(editionNoteDiv).addClass("loading");
        $(editionNoteDiv).addClass("edition-note-wrapper");
        var editionNoteHeader = document.createElement("h3");
        $(editionNoteHeader).append("Ediční poznámka");
        $(editionNoteDiv).append(editionNoteHeader);

        var editionNote: JQueryXHR = this.sc.getEditionNote(this.bookId);
        editionNote.done((response: {editionNote: string}) => {
            var editionNoteText = document.createElement("div");
                $(editionNoteText).addClass("edition-note-text");
                if (response.editionNote == "") {
                    $(editionNoteText).append("Toto dílo nemá ediční poznámku");
                } else {
                    $(editionNoteText).append(response.editionNote);
                }
                editionNoteDiv.appendChild(editionNoteText);
                $(editionNoteDiv).removeClass("loading");
        });
        editionNote.fail(() => {
            $(editionNoteDiv).append("Toto dílo nemá ediční poznámku");    
        });
        
        hiddenDiv.appendChild(editionNoteDiv);

        var bookDetailDiv = document.createElement("div");
        $(bookDetailDiv).addClass("book-detail-wrapper");
        var bookDetailHeader = document.createElement("h3");
        $(bookDetailHeader).append("Informace o díle");
        bookDetailDiv.appendChild(bookDetailHeader);

        var bookDetail: JQueryXHR = this.sc.getBookDetail(this.bookId);
        bookDetail.done((response) => {
            var detailData = response["detail"];
            var detailTable = new TableBuilder();
            var editors: string = "";
            for (var i = 0; i < detailData.Editors.length; i++) {
                var editor = detailData.Editors[i];
                editors += editor.FirstName + " " + editor.LastName;
                if (i + 1 != detailData.Editors.length) {
                    editors += ", ";
                }
            }

            detailTable.makeTableRow("Editor", editors);
            detailTable.makeTableRow("Předloha", detailData.LiteraryOriginal);
            detailTable.makeTableRow("Zkratka památky", detailData.RelicAbbreviation);
            detailTable.makeTableRow("Zkratka pramene", detailData.SourceAbbreviation);
            detailTable.makeTableRow("Literární druh", detailData.LiteraryKinds);
            detailTable.makeTableRow("Literární žánr", detailData.LiteraryGenre);
            detailTable.makeTableRow("Poslední úprava edice	", detailData.CreateTimeString);

            $(detailTable.build()).find(".bib-table-cell").each(function () {
                if (this.innerHTML === "" || this.innerHTML === "undefined") {
                    this.innerHTML = "&lt;Nezadáno&gt;";
                }
            });

            $(bookDetailDiv).append(detailTable.build());

            
            if (detailData.Authors.length != 0) {
                var authors: string = "";
                for (var i = 0; i < detailData.Authors.length; i++) {
                    var author = detailData.Authors[i];
                    authors += author.FirstName + " " + author.LastName;
                    if (i + 1 != detailData.Authors.length) {
                        authors += ", ";
                    }
                }
                $(".title").prepend(authors + ": ");    
            }
            
        });
        bookDetail.fail(() => {
            $(bookDetailDiv).append("Nepodařilo se načíst detaily o díle");
        });    
        hiddenDiv.appendChild(bookDetailDiv);

        bookInfoDiv.appendChild(hiddenDiv);
        
        return bookInfoDiv;
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

        var searchResultButton: HTMLButtonElement = document.createElement("button");
        $(searchResultButton).addClass("search-button");
        $(searchResultButton).prop("disabled", true);
        var searchResultSpan = document.createElement("span");
        $(searchResultSpan).addClass("glyphicon glyphicon-search");
        $(searchResultButton).append(searchResultSpan);

        var searchResultSpanText = document.createElement("span");
        $(searchResultSpanText).addClass("button-text");
        $(searchResultSpanText).append("Výsledky vyhledávání");
        $(searchResultButton).append(searchResultSpanText);

        $(searchResultButton).click((event: Event) => {
            readerLayout.createToolPanel(this.searchPanelId, searchResultSpanText.innerHTML);
        });
        toolButtons.appendChild(searchResultButton);

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

        return toolButtons;
    }

    private makeViewButtons(): HTMLDivElement {
        var readerLayout = this;
        var viewControl: HTMLDivElement = document.createElement("div");
        $(viewControl).addClass("view-control");
        //$(viewControl).append("Zobrazení: ");

        var viewButtons = document.createElement("div");
        $(viewButtons).addClass("buttons");
        var hasBookText: boolean = false;
        var hasBookImage: boolean = false;
        this.hasBookPage(this.bookId, this.versionId, () => {
            var textButton: HTMLButtonElement = document.createElement("button");
            $(textButton).addClass("text-button");
            var textSpan = document.createElement("span");
            $(textSpan).addClass("glyphicon glyphicon-font");
            $(textButton).append(textSpan);

            var textSpanText = document.createElement("span");
            $(textSpanText).addClass("button-text");
            $(textSpanText).append("Text");
            $(textButton).append(textSpanText);

            $(textButton).click((event: Event) => {
                readerLayout.createViewPanel(this.textPanelId, textSpanText.innerHTML);
            });
            hasBookText = true;
            textButton.click();
            viewButtons.appendChild(textButton); 

            var checkboxDiv = this.createCheckboxDiv();
            viewControl.appendChild(checkboxDiv);
        });
            

        this.hasBookImage(this.bookId, this.versionId,() => {
            var imageButton: HTMLButtonElement = document.createElement("button");
            $(imageButton).addClass("image-button");
            var imageSpan = document.createElement("span");
            $(imageSpan).addClass("glyphicon glyphicon-picture");
            $(imageButton).append(imageSpan);

            var imageSpanText = document.createElement("span");
            $(imageSpanText).addClass("button-text");
            $(imageSpanText).append("Obraz");
            $(imageButton).append(imageSpanText);

            $(imageButton).click((event: Event) => {
                readerLayout.createViewPanel(this.imagePanelId, imageSpanText.innerHTML);
            });
            hasBookImage = true;
            if (!hasBookText) {
                imageButton.click();
            }
            viewButtons.appendChild(imageButton);       
        });

        this.hasBookAudio(this.bookId, this.versionId, () => {
            var audioButton: HTMLButtonElement = document.createElement("button");
            $(audioButton).addClass("audio-button");
            var audioSpan = document.createElement("span");
            $(audioSpan).addClass("glyphicon glyphicon-music");
            $(audioButton).append(audioSpan);

            var audioSpanText = document.createElement("span");
            $(audioSpanText).addClass("button-text");
            $(audioSpanText).append("Zvuková stopa");
            $(audioButton).append(audioSpanText);

            $(audioButton).click((event: Event) => {
                readerLayout.createViewPanel(this.audioPanelId, audioSpanText.innerHTML);
            });

            if (!hasBookText && !hasBookImage) {
                audioButton.click();
            }
            viewButtons.appendChild(audioButton);
        });
        
        viewControl.appendChild(viewButtons);

        return viewControl;
    }

    private createCheckboxDiv(): HTMLDivElement {
        var checkboxesDiv = window.document.createElement("div");
        $(checkboxesDiv).addClass("reader-settings-checkboxes-area");
        
        var showPageCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        var showPageNameCheckbox: HTMLInputElement = window.document.createElement("input");
        showPageNameCheckbox.type = "checkbox";

        $(showPageNameCheckbox).change((eventData: Event) => {
            var readerText: JQuery = $("#" + this.textPanelId).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            if (currentTarget.checked) {
                readerText.addClass("reader-text-show-page-names");
            } else {
                readerText.removeClass("reader-text-show-page-names");
            }
        });

        var pageNameSlider = document.createElement("label");
        $(pageNameSlider).addClass("switch");

        var showPageNameLabel: HTMLLabelElement = window.document.createElement("label");
        showPageNameLabel.innerHTML = "Číslování stránek";
        showPageCheckboxDiv.appendChild(showPageNameCheckbox);
        showPageCheckboxDiv.appendChild(pageNameSlider);
        showPageCheckboxDiv.appendChild(showPageNameLabel);
        showPageNameCheckbox.id = "checkbox-show-page-numbers";
        showPageNameLabel.setAttribute("for", showPageNameCheckbox.id);
        pageNameSlider.setAttribute("for", showPageNameCheckbox.id);
        checkboxesDiv.appendChild(showPageCheckboxDiv);

        var showPageOnNewLineDiv: HTMLDivElement = window.document.createElement("div");
        var showPageOnNewLineCheckbox: HTMLInputElement = window.document.createElement("input");
        showPageOnNewLineCheckbox.type = "checkbox";

        $(showPageOnNewLineCheckbox).change((eventData: Event) => {
            var readerText = $("#" + this.textPanelId).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            if (currentTarget.checked) {
                $(readerText).addClass("reader-text-page-new-line");
            } else {
                $(readerText).removeClass("reader-text-page-new-line");
            }
        });

        var pageOnNewLineSlider = document.createElement("label");
        $(pageOnNewLineSlider).addClass("switch");

        var showPageOnNewLineLabel: HTMLLabelElement = window.document.createElement("label");
        showPageOnNewLineLabel.innerHTML = "Zalamovat stránky";
        showPageOnNewLineDiv.appendChild(showPageOnNewLineCheckbox);
        showPageOnNewLineDiv.appendChild(pageOnNewLineSlider);
        showPageOnNewLineDiv.appendChild(showPageOnNewLineLabel);
        showPageOnNewLineCheckbox.id = "checkbox-page-breaks";
        showPageOnNewLineLabel.setAttribute("for", showPageOnNewLineCheckbox.id);
        pageOnNewLineSlider.setAttribute("for", showPageOnNewLineCheckbox.id);
        checkboxesDiv.appendChild(showPageOnNewLineDiv);
        
        var showCommentCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        var showCommentCheckbox: HTMLInputElement = window.document.createElement("input");
        showCommentCheckbox.type = "checkbox";

        $(showCommentCheckbox).change((eventData: Event) => {
            var readerText = $("#" + this.textPanelId).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            if (currentTarget.checked) {
                $(readerText).addClass("show-notes");
            } else {
                $(readerText).removeClass("show-notes");
            }
        });

        var commentSlider = document.createElement("label");
        $(commentSlider).addClass("switch");

        var showCommentLabel: HTMLLabelElement = window.document.createElement("label");
        showCommentLabel.innerHTML = "Komentáře";
        showCommentCheckboxDiv.appendChild(showCommentCheckbox);
        showCommentCheckboxDiv.appendChild(commentSlider);
        showCommentCheckboxDiv.appendChild(showCommentLabel);
        showCommentCheckbox.id = "checkbox-show-comment";
        showCommentLabel.setAttribute("for", showCommentCheckbox.id);
        commentSlider.setAttribute("for", showCommentCheckbox.id);
        checkboxesDiv.appendChild(showCommentCheckboxDiv);

        return checkboxesDiv;
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
            if (panelId === this.termsPanelId) type = "column";
            else type = "component";
            var itemConfig = {
                type: type,
                id: panelId,
                componentState: { label: panelId },
                componentName: 'toolTab',
                title: panelTitle
            };
            this.readerLayout.root.getItemsById('tools')[0].addChild(itemConfig);
            if (panelId === this.termsPanelId) {
                this.createTermsPanel();
            }
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
        this.getSearchPanel().createPagination(pageChangedCallback, itemsCount);
    }

    getSearchResultsCountOnPage(): number {
        return this.getSearchPanel().getResultsCountOnPage();
    }

    private getSearchPanel(): SearchResultPanel {
        this.createToolPanel(this.searchPanelId, "Výsledky vyhledávání");
        var searchButton = $(document).find(".search-button");
        searchButton.prop("disabled", false);
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

    private initLayout(): GoldenLayout {
        var module = this;
        var config = this.createConfig(this.textPanelId, "Text");
        var readerLayout = new GoldenLayout(config, $('#ReaderBodyDiv'));
        readerLayout.registerComponent('toolTab', function (container, state) {
            switch (state.label) {
                case module.bookmarksPanelId:
                    container.getElement().append(module.createBookmarksPanel());
                    break;
                case module.termsResultId:
                    container.getElement().append(module.createTermsResultPanel());
                    break;
                case module.termsSearchId:
                    container.getElement().append(module.createTermsSearchPanel());
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
        readerLayout.registerComponent('viewTab', function (container, state) {
            switch (state.label) {
                case module.audioPanelId:
                    container.getElement().append(module.createAudioPanel());
                    break;
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
                headerHeight: 26,
                minItemWidth: 200
            },
            content: [{
                type: "row",
                isClosable: false,
                content: [{
                    type: "column",
                    id: "views",
                    isClosable: false,
                    content: [{
                        type: "row",
                        id: "viewsRow"
                        
                    }]
                }]
            }]
        }
        return layoutConfig;
    }

    private createBookmarksPanel(): HTMLDivElement {
        if (this.bookmarksPanel == null) {
            var bookmarksPanel: BookmarksPanel = new BookmarksPanel(this.bookmarksPanelId, this, this.sc);
            this.bookmarksPanel = bookmarksPanel;
            this.toolPanels.push(bookmarksPanel);
        }
        return this.bookmarksPanel.panelHtml;
    }

    private createContentPanel(): HTMLDivElement {
        if (this.contentPanel == null) {
            var contentPanel: ContentPanel = new ContentPanel(this.contentPanelId, this, this.sc);
            this.contentPanel = contentPanel;
            this.toolPanels.push(contentPanel);
        }
        return this.contentPanel.panelHtml;
    }

    private createSearchPanel(): HTMLDivElement {
        if (this.searchPanel == null) {
            var resultPanel: SearchResultPanel =  new SearchResultPanel(this.searchPanelId, this, this.sc);
            this.searchPanel = resultPanel;
            this.toolPanels.push(resultPanel);
        }
        return this.searchPanel.panelHtml;
    }

    private createTermsPanel() {
        var itemConfig = {
            type: 'component',
            id: this.termsSearchId,
            componentState: { label: this.termsSearchId },
            componentName: 'toolTab',
            title: "Výskyty na stránce"
        };
        this.readerLayout.root.getItemsById(this.termsPanelId)[0].addChild(itemConfig);
        itemConfig = {
            type: 'component',
            id: this.termsResultId,
            componentState: { label: this.termsResultId },
            componentName: 'toolTab',
            title: "Témata na stránce"
        };
        this.readerLayout.root.getItemsById(this.termsPanelId)[0].addChild(itemConfig);
    }

    private createTermsResultPanel(): HTMLDivElement {
        if (this.termsResultPanel == null) {
            var termsPanel: TermsResultPanel = new TermsResultPanel(this.termsResultId, this, this.sc);
            this.termsResultPanel = termsPanel;
            this.toolPanels.push(termsPanel);
        }
        return this.termsResultPanel.panelHtml;
    }

    private createTermsSearchPanel(): HTMLDivElement {
        if (this.termsSearchPanel == null) {
            var termsPanel: TermsSearchPanel = new TermsSearchPanel(this.termsSearchId, this, this.sc);
            this.termsSearchPanel = termsPanel;
            this.toolPanels.push(termsPanel);
        }
        return this.termsSearchPanel.panelHtml;
    }

    private createTextPanel(): HTMLDivElement {
        if (this.textPanel == null) {
            var textPanel: TextPanel = new TextPanel(this.textPanelId, this, this.sc);
            this.textPanel = textPanel;
            this.contentViewPanels.push(textPanel);    
        }
        return this.textPanel.panelHtml;
    }

    private createImagePanel(): HTMLDivElement {
        if (this.imagePanel == null) {
            var imagePanel: ImagePanel = new ImagePanel(this.imagePanelId, this, this.sc);
            this.imagePanel = imagePanel;
            this.contentViewPanels.push(imagePanel);    
        }
        return this.imagePanel.panelHtml;
    }

    private createAudioPanel(): HTMLDivElement {
        if (this.audioPanel == null) {
            var audioPanel: AudioPanel = new AudioPanel(this.audioPanelId, this, this.sc);
            this.audioPanel = audioPanel;
            this.contentViewPanels.push(audioPanel);
        }
        return this.audioPanel.panelHtml;
    }

    protected hasBookImageCache: { [key: string]: { [key: string]: boolean; }; } = {};

    hasBookImage(bookId: string, bookVersionId: string, onTrue: () => any = null, onFalse: () => any = null) {
        if (this.hasBookImageCache[bookId] === undefined || this.hasBookImageCache[bookId][bookVersionId] === undefined) {
            var hasBookImageAjax: JQueryXHR = this.sc.hasBookImage(bookId, bookVersionId);
            hasBookImageAjax.done((response: { HasBookImage: boolean }) => {
                if (this.hasBookImageCache[bookId] === undefined) {
                    this.hasBookImageCache[bookId] = {};
                }
                this.hasBookImageCache[bookId][bookVersionId] = response.HasBookImage;
                if (response.HasBookImage && onTrue !== null) {
                    onTrue();
                } else if (!response.HasBookImage && onFalse !== null) {
                    onFalse();
                }    
            });

            hasBookImageAjax.fail((response) => {
                console.error(response);    
            });
        } else if (this.hasBookImageCache[bookId][bookVersionId] && onTrue !== null) {
            onTrue();
        } else if (!this.hasBookImageCache[bookId][bookVersionId] && onFalse !== null) {
            onFalse();
        }
    }

    hasBookAudio(bookId: string, bookVersionId: string, onTrue: () => any = null, onFalse: () => any = null) {
        var audioBook: JQueryXHR = this.sc.getAudioBook(bookId);
        audioBook.done((response) => {
            if (response["audioBook"].Tracks.length == 0) {
                onFalse();
            } else {
                onTrue();
            }
        });
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
            var hasBookPage: JQueryXHR = this.sc.hasBookPage(bookId, bookVersionId)
            hasBookPage.done((response: { HasBookPage: boolean }) => {
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
}

