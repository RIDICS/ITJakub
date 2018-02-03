///<reference path="../../../lib/golden-layout/index.d.ts"/>
//import GoldenLayout = require("golden-layout");
//declare var GoldenLayout;

class ReaderLayout {
    private favoriteManager: FavoriteManager;
    private newFavoriteDialog: NewFavoriteDialog;
    readerLayout: any;
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

    leftSidePanels: Array<SidePanel>;
    rightSidePanels: Array<SidePanel>;

    audioPanelId: string = "audio";
    textPanelId: string = "text";
    imagePanelId: string = "image";
    contentPanelId: string = "content";
    bookmarksPanelId: string = "bookmarks";
    searchPanelId: string = "search";
    termsPanelId: string = "terms";
  
    showPanelList: Array<ReaderPanelEnum>;

    pageChangedCallback: (pageId: number) => void;

    constructor(readerHeaderDiv: HTMLDivElement, pageChangedCallback: (pageId: number) => void) {
        this.readerHeaderDiv = readerHeaderDiv;
        $(this.readerHeaderDiv).addClass("content-container");
        this.pageChangedCallback = pageChangedCallback;
        this.pagerDisplayPages = 5;
        this.favoriteManager = new FavoriteManager();
        this.newFavoriteDialog = new NewFavoriteDialog(this.favoriteManager, true);
    }

    public makeReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: IPage[]) {
        this.bookId = bookXmlId;
        this.versionId = bookXmlId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<BookPage>();
        this.pagesById = {};
        this.bookmarks = new Array<IBookmarkPosition>(pageList.length);

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
        $(toolButtons).addClass("buttons");

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
            throw new Error("Not implemented");
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
        $(viewButtons).addClass("buttons left");

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
                state: { label: panelId },
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
                state: { label: panelId },
                componentName: 'viewTab',
                title: panelTitle,
                
            };
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

    private moveToPageNumber(pageIndex: number, scrollTo: boolean) {
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
        for (var k = 0; k < this.leftSidePanels.length; k++) {
            this.leftSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }

        for (var k = 0; k < this.rightSidePanels.length; k++) {
            this.rightSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }
    }

    private initLayout(): any {
        var module = this;
        var config = this.createConfig();
        this.readerLayout = new GoldenLayout(config, $('#ReaderBodyDiv'));
        this.readerLayout.registerComponent('toolTab', function (container, state) {
            switch (state.label) {
                //case module.bookmarksPanelId:
                //    container.getElement().append(module.createBookmarkPanel());
                //    break;
                //case module.termsPanelId:
                //    //module.createTermsPanel(); TODO different panel type
                //    break;
                //case module.contentPanelId:
                //    container.getElement().append(module.createContentPanel());
                //    break;
                //case module.searchPanelId:
                //    container.getElement().append(module.searchContentPanel());
                //    break;
                default:
                    break;
            }    
        });
        this.readerLayout.registerComponent('viewTab', function(container, state) {
            switch (state.label) {
            //case module.audioPanelId:
            //    container.getElement().append(module.createAudioPanel());
            //    break;
            //case module.imagePanelId:
            //    container.getElement().append(module.createImagePanel());
            //    break;
            //case module.textPanelId:
            //    container.getElement().append(module.createTextPanel());
            //    break;
            default:
                break;
            }        
        });
        this.readerLayout.init();
        var readerLayout = this.readerLayout;
        this.readerLayout.on("stateChanged", function () {
            $(".reader-text-container").scroll();
        });
        $(window).resize(function () {
            readerLayout.updateSize();
        });
        return readerLayout;
    }

    private createConfig() {
        var layoutConfig = {
            dimensions: {
                headerHeight: 26
            },
            content: [{
                type: 'row',
                isClosable: false,
            }]
        };
        return layoutConfig;    
    }
}

class GoldenLayoutReader extends ReaderModule {
    readerLayout: any;
    public initReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: IPage[]) {
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
        this.makeHeader(bookTitle);
        this.makeGoldenReader();
        
    }

    private makeHeader(bookTitle: string) {
        var readerHeadDiv: HTMLDivElement = document.createElement("div");
        $(readerHeadDiv).addClass("reader-head content-container");
        var title = this.makeTitle(bookTitle);
        readerHeadDiv.appendChild(title);
        var paging = this.makeControl();
        readerHeadDiv.appendChild(paging);
        this.readerContainer.appendChild(readerHeadDiv);
    }

    private makeControl(): HTMLDivElement {
        var controlsDiv: HTMLDivElement = document.createElement("div");
        $(controlsDiv).addClass("reader-controls content-container");
        var slider: HTMLDivElement = this.makeSlider();
        controlsDiv.appendChild(slider);
        var paging: HTMLDivElement = document.createElement("div");
        $(paging).addClass("paging");
        var pageInput = this.makePageInput();
        paging.appendChild(pageInput);
        var buttonsDiv: HTMLDivElement = this.makeButtons();
        paging.appendChild(buttonsDiv);
        var pageNav = this.makePageNavigation();
        paging.appendChild(pageNav);
        controlsDiv.appendChild(paging);
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

    private makePageInput(): HTMLDivElement {
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
        return pageInputDiv;
    }

    private makeButtons(): HTMLDivElement {
        var toolButtons: HTMLDivElement = document.createElement("div");
        $(toolButtons).addClass("buttons");

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
            throw new Error("Not implemented");    
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
            if (this.readerLayout.root.getItemsById('tools').length === 0) {
                var toolStackConfig = {
                    type: 'stack',
                    width: 18,
                    id: 'tools',
                    componentName: 'readerTab'
                };
                this.readerLayout.root.contentItems[0].addChild(toolStackConfig, 0);
                this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
                this.readerLayout.updateSize();
            }
            if(this.readerLayout.root.getItemsById('bookmarks').length === 0) {
                var itemConfig = {
                    type: 'component',
                    id: 'bookmarks',
                    componentState: { label: 'Bookmarks' },
                    componentName: 'readerTab',
                    title: 'Záložky'
                };
                this.readerLayout.root.getItemsById('tools')[0].addChild(itemConfig);    
            }
        });
        toolButtons.appendChild(bookmarksButton);


        var settingsButton: HTMLButtonElement = document.createElement("button");
        $(settingsButton).addClass("settings-button");
        var settingsSpan = document.createElement("span");
        $(settingsSpan).addClass("glyphicon glyphicon-cog");
        $(settingsButton).append(settingsSpan);

        var settingsSpanText = document.createElement("span");
        $(settingsSpanText).addClass("button-text");
        $(settingsSpanText).append("Zobrazení");
        $(settingsButton).append(settingsSpanText);

        $(settingsButton).click((event: Event) => {
            if (this.readerLayout.root.getItemsById('tools').length === 0) {
                var toolStackConfig = {
                    type: 'stack',
                    width: 18,
                    id: 'tools',
                    componentName: 'readerTab'
                };
                this.readerLayout.root.contentItems[0].addChild(toolStackConfig, 0);
                this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
                this.readerLayout.updateSize();
            }
            if (this.readerLayout.root.getItemsById('settings').length === 0) {
                var settingsItemConfig = {
                    type: 'component',
                    id: 'settings',
                    componentState: { label: 'Settings' },
                    componentName: 'readerTab',
                    title: 'Zobrazení'
                };
                this.readerLayout.root.getItemsById('tools')[0].addChild(settingsItemConfig);
            }
        });
        toolButtons.appendChild(settingsButton);


        var searchButton: HTMLButtonElement = document.createElement("button");
        $(searchButton).addClass("search-button");
        var searchSpan = document.createElement("span");
        $(searchSpan).addClass("glyphicon glyphicon-search");
        $(searchButton).append(searchSpan);

        var searchSpanText = document.createElement("span");
        $(searchSpanText).addClass("button-text");
        $(searchSpanText).append("Vyhledávání");
        $(searchButton).append(searchSpanText);

        $(searchButton).click((event: Event) => {
            if (this.readerLayout.root.getItemsById('tools').length === 0) {
                var toolStackConfig = {
                    type: 'stack',
                    width: 18,
                    id: 'tools',
                    componentName: 'readerTab'
                };
                this.readerLayout.root.contentItems[0].addChild(toolStackConfig, 0);
                this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
                this.readerLayout.updateSize();
            }
            if (this.readerLayout.root.getItemsById('search').length === 0) {
                var searchItemConfig = {
                    type: 'component',
                    id: 'search',
                    componentState: { label: 'Search' },
                    componentName: 'readerTab',
                    title: 'Vyhledávání'
                };
                this.readerLayout.root.getItemsById('tools')[0].addChild(searchItemConfig);
            }
        });


        toolButtons.appendChild(searchButton);

        
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
            if (this.readerLayout.root.getItemsById('tools').length === 0) {
                var toolStackConfig = {
                    type: 'stack',
                    width: 18,
                    id: 'tools',
                    componentName: 'readerTab'
                };
                this.readerLayout.root.contentItems[0].addChild(toolStackConfig, 0);
                this.readerLayout.root.getItemsById('tools')[0].config.width = 15;
                this.readerLayout.updateSize();
            }
            if (this.readerLayout.root.getItemsById('content').length === 0) {
                var contentItemConfig = {
                    type: 'component',
                    id: 'content',
                    componentState: { label: 'Content' },
                    componentName: 'readerTab',
                    title: 'Obsah'
                };
                this.readerLayout.root.getItemsById('tools')[0].addChild(contentItemConfig);
            }
        });
        toolButtons.appendChild(contentButton);
        return toolButtons;
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

    private makeGoldenReader() {

        var config = this.createConfig();
        var module = this;
        this.readerLayout = new GoldenLayout(config, $('#ReaderBodyDiv'));
        this.readerLayout.registerComponent('readerTab', function (container, state) {
            if (state.label === "Text") {
                $(container.getElement()).append(module.getBookText());
            }
            if (state.label === "Content") {
                $(container.getElement()).append(module.getBookContent());
            }
            if (state.label === "Settings") {
                $(container.getElement()).append(module.getBookSettings());
            }
            if (state.label === "Bookmarks") {
                $(container.getElement()).append(module.getBookBookmarks());
            }
            if (state.label === "Img") {
                $(container.getElement()).append(module.getBookImage());
            }

            if (state.label === "Search") {
                $(container.getElement()).append(module.getBookSearch());
            }
        });
        this.readerLayout.init();
        var readerLayout = this.readerLayout;
        this.readerLayout.on("stateChanged", function() {
            $(".reader-text-container").scroll();
        });
        $(window).resize(function() {
            readerLayout.updateSize();
        });
    }

    private setSize() {}

    private createConfig() {
        var layoutConfig = {
            dimensions: {
                headerHeight: 26
            },
            content: [{
                type: 'row',
                isClosable: false,
                content: [{
                    type: 'component',
                    id: 'text',
                    componentState: { label: 'Text' },
                    componentName: 'readerTab',
                    isClosable: false,
                    title: 'Text'
                }, {
                    type: 'component',
                    id: 'img',
                    componentState: { label: 'Img' },
                    componentName: 'readerTab',
                    title: 'Náhled'
                }]
            }]
        };
        return layoutConfig;
    }

    private getBookText(): HTMLDivElement {
        var textPanel: TextPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
            textPanel = new TextPanel(this.textPanelIdentificator, this, this.showMainPanelsButtonList);
            this.rightSidePanels.push(textPanel);
            this.textPanel = textPanel;
        }
        return textPanel.panelHtml;
    }

    private getBookImage(): HTMLDivElement {
        var imagePanel: ImagePanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
            imagePanel = new ImagePanel(this.imagePanelIdentificator, this, this.showMainPanelsButtonList);
            this.rightSidePanels.push(imagePanel);
            this.imagePanel = imagePanel;
        }
        return imagePanel.panelHtml;
    }

    private getBookBookmarks(): HTMLDivElement {
        var bookmarksPanel: BookmarksPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
            bookmarksPanel = new BookmarksPanel(this.bookmarksPanelIdentificator, this, this.showMainPanelsButtonList);
            this.leftSidePanels.push(bookmarksPanel);
            this.bookmarksPanel = bookmarksPanel;
        }
        return bookmarksPanel.panelHtml;
    }

    private getBookSettings(): HTMLDivElement {
        var settingsPanel: SettingsPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
            settingsPanel = new SettingsPanel(this.settingsPanelIdentificator, this, this.showMainPanelsButtonList);
            this.leftSidePanels.push(settingsPanel);
            this.settingsPanel = settingsPanel;
        }
        return settingsPanel.panelHtml;
    }

    private getBookContent(): HTMLDivElement {
        var contentPanel: ContentPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.ContentPanel) >= 0) {
            contentPanel = new ContentPanel(this.contentPanelIdentificator, this, this.showMainPanelsButtonList);
            this.leftSidePanels.push(contentPanel);
            this.contentPanel = contentPanel;
        }
        return contentPanel.panelHtml;
    }   

    private getBookSearch(): HTMLDivElement {
        var searchPanel: SearchResultPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.SearchPanel) >= 0) {
            searchPanel = new SearchResultPanel(this.searchPanelIdentificator, this, this.showMainPanelsButtonList);
            this.leftSidePanels.push(searchPanel);
            this.searchPanel = searchPanel;
        }
        return searchPanel.panelHtml;
    } 
}

function initGoldenReader(bookXmlId: string,
    versionXmlId: string,
    bookTitle: string,
    pageList: any,
    searchedText?: string,
    initPageXmlId?: string) {


    function readerPageChangedCallback(pageId: number) {
        updateQueryStringParameter("page", pageId);
    }

    var readerPlugin = new ReaderLayout(<any>$("#ReaderHeaderDiv")[0],
        readerPageChangedCallback);
    readerPlugin.makeReader(bookXmlId, versionXmlId, bookTitle, pageList);
}
