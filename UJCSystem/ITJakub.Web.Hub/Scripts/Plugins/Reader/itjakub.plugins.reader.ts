/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />


class ReaderModule {

    readerContainer: string;
    sliderOnPage: number;
    actualPageIndex: number;
    pages: Array<string>;
    pagerDisplayPages: number;
    preloadPagesBefore: number;
    preloadPagesAfter: number;
    bookId: string;
    loadedBookContent: boolean;

    leftSidePanels: Array<SidePanel>;
    rightSidePanels: Array<SidePanel>;


    imagePanelIdentificator: string = "ImagePanel";
    textPanelIdentificator: string = "TextPanel";

    constructor(readerContainer: string) {
        this.readerContainer = readerContainer;
        this.pagerDisplayPages = 5;
    }

    public makeReader(bookId : string, bookTitle : string, pageList) {
        this.bookId = bookId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<string>();
        this.leftSidePanels = new Array<SidePanel>();
        this.rightSidePanels = new Array<SidePanel>();

        $(window).on("beforeunload",(event: Event) => {
            for (var k = 0; k < this.leftSidePanels.length; k++) {
                this.leftSidePanels[k].childwindow.close();
            }

            for (var k = 0; k < this.rightSidePanels.length; k++) {
                this.rightSidePanels[k].childwindow.close();
            }
        });
        
        for (var i = 0; i < pageList.length; i++) { //load pageList
            this.pages.push(pageList[i]["Text"]);
        }

        $(this.readerContainer).empty();
        var readerDiv: HTMLDivElement = document.createElement('div');
        $(readerDiv).addClass('reader');

        var readerHeadDiv: HTMLDivElement = document.createElement('div');
        $(readerHeadDiv).addClass('reader-head content-container');

        var fullscreenButton = document.createElement("button");
        $(fullscreenButton).addClass('fullscreen-button');

        var fullscreenSpan = document.createElement("span");
        $(fullscreenSpan).addClass('glyphicon glyphicon-fullscreen');
        $(fullscreenButton).append(fullscreenSpan);
        $(fullscreenButton).click((event) => {
            $(this.readerContainer).find('.reader').addClass('fullscreen');
        });
        readerHeadDiv.appendChild(fullscreenButton);

        var fullscreenCloseButton = document.createElement("button");
        $(fullscreenCloseButton).addClass('fullscreen-close-button');

        var closeSpan = document.createElement("span");
        $(closeSpan).addClass('glyphicon glyphicon-remove');
        $(fullscreenCloseButton).append(closeSpan);
        $(fullscreenCloseButton).click((event) => {
            $(this.readerContainer).find('.reader').removeClass('fullscreen');
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

        this.moveToPageNumber(0, false); //load first page
    }

    private makeTitle(bookTitle : string): HTMLDivElement {
        var titleDiv: HTMLDivElement = document.createElement('div');
        $(titleDiv).addClass('title');
        titleDiv.innerHTML = bookTitle;
        return titleDiv;
    }

    private makeControls(): HTMLDivElement {

        var controlsDiv: HTMLDivElement = document.createElement('div');
        $(controlsDiv).addClass('reader-controls content-container');

        var slider: HTMLDivElement = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: this.pages.length - 1,
            value: 0,
            start: (event, ui) => {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
            },
            stop: (event, ui) => {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').fadeOut(1000);
            },
            slide: (event, ui) => {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').stop(true, true);
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html("Strana: " + this.pages[ui.value]);

            },
            change: (event: Event, ui: JQueryUI.SliderUIParams) => {
                if (this.actualPageIndex !== ui.value) {
                    this.moveToPageNumber(<any>ui.value, true);
                }
            }
        });

        var sliderTooltip: HTMLDivElement = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip: HTMLDivElement = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);

        var innerTooltip: HTMLDivElement = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html("Strana: " + this.pages[0]);
        sliderTooltip.appendChild(innerTooltip);
        $(sliderTooltip).hide();

        var sliderHandle = $(slider).find('.ui-slider-handle');
        $(sliderHandle).append(sliderTooltip);
        $(sliderHandle).hover((event) => {
            $(event.target).find('.slider-tip').stop(true, true);
            $(event.target).find('.slider-tip').show();
        });
        $(sliderHandle).mouseout((event) => {
            $(event.target).find('.slider-tip').fadeOut(1000);
        });
        controlsDiv.appendChild(slider);

        var pagingDiv: HTMLDivElement = document.createElement('div');
        $(pagingDiv).addClass('paging');

        var pageInputDiv: HTMLDivElement = document.createElement('div');
        $(pageInputDiv).addClass('page-input');

        var pageInputText = document.createElement("input");
        pageInputText.setAttribute("type", "text");
        pageInputText.setAttribute("id", "pageInputText");
        $(pageInputText).addClass('page-input-text');
        pageInputDiv.appendChild(pageInputText);

        var pageInputButton = document.createElement("button");
        pageInputButton.innerHTML = "Přejít na stránku";
        $(pageInputButton).addClass('page-input-button');
        $(pageInputButton).click((event: Event) => {
            this.moveToPage($('#pageInputText').val(), true);
        });
        pageInputDiv.appendChild(pageInputButton);

        pagingDiv.appendChild(pageInputDiv);

        var paginationUl: HTMLUListElement = document.createElement('ul');
        $(paginationUl).addClass('pagination pagination-sm');

        var liElement: HTMLLIElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-left');
        var anchor: HTMLAnchorElement = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '|<';
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(0, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-left');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<<';
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex - 5, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-left');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<';
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex - 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        $(liElement).addClass('more-pages more-pages-left');
        liElement.innerHTML = '...';
        paginationUl.appendChild(liElement);

        $.each(this.pages, (index, page) => {
            liElement = document.createElement('li');
            $(liElement).addClass('page');
            $(liElement).data('page-index', index);
            anchor = document.createElement('a');
            anchor.href = '#';
            anchor.innerHTML = page;
            $(anchor).click((event: Event) => {
                event.stopPropagation();
                this.moveToPage(page, true);
                return false;
            });
            liElement.appendChild(anchor);
            paginationUl.appendChild(liElement);
        });

        liElement = document.createElement('li');
        $(liElement).addClass('more-pages more-pages-right');
        liElement.innerHTML = '...';
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-right');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>';
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex + 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-right');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>>';
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex + 5, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-right');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>|';
        $(anchor).click((event: Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.pages.length - 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        pagingDiv.appendChild(paginationUl);

        var buttonsDiv: HTMLDivElement = document.createElement("div");
        $(buttonsDiv).addClass('buttons');

        var bookmarkButton = document.createElement("button");
        $(bookmarkButton).addClass('bookmark-button');

        var bookmarkSpan = document.createElement("span");
        $(bookmarkSpan).addClass('glyphicon glyphicon-bookmark');
        $(bookmarkButton).append(bookmarkSpan);

        var bookmarkSpanText = document.createElement("span");
        $(bookmarkSpanText).addClass('button-text');
        $(bookmarkSpanText).append("Záložky");
        $(bookmarkButton).append(bookmarkSpanText);

        $(bookmarkButton).click((event: Event) => {
            if (!this.removeBookmark()) {
                this.addBookmark();
            }
        });

        buttonsDiv.appendChild(bookmarkButton);

        var commentButton = document.createElement("button");
        $(commentButton).addClass('comment-button');

        var commentSpan = document.createElement("span");
        $(commentSpan).addClass('glyphicon glyphicon-cog');
        $(commentButton).append(commentSpan);

        var commentSpanText = document.createElement("span");
        $(commentSpanText).addClass('button-text');
        $(commentSpanText).append("Možnosti zobrazeni");
        $(commentButton).append(commentSpanText);

        $(commentButton).click((event: Event) => {
            var panelId = "EditacniPanel";
            if (!this.existSidePanel(panelId)) {
                var editPanel: SettingsPanel = new SettingsPanel(panelId , this);
                this.loadSidePanel(editPanel.panelHtml);
                this.leftSidePanels.push(editPanel);
            }
            this.changeSidePanelVisibility("EditacniPanel", 'left');
        });

        buttonsDiv.appendChild(commentButton);

        var searchResultButton = document.createElement("button");
        $(searchResultButton).addClass('search-button');

        var searchSpan = document.createElement("span");
        $(searchSpan).addClass('glyphicon glyphicon-search');
        $(searchResultButton).append(searchSpan);

        var searchSpanText = document.createElement("span");
        $(searchSpanText).addClass('button-text');
        $(searchSpanText).append("Výsledky vyhledávání");
        $(searchResultButton).append(searchSpanText);

        $(searchResultButton).click((event: Event) => {
            var panelId = "SearchPanel";
            if (!this.existSidePanel(panelId)) {
                var searchPanel = new LeftSidePanel(panelId,"Vyhlédávání", this);
                this.loadSidePanel(searchPanel.panelHtml);
                this.leftSidePanels.push(searchPanel);
            }
            this.changeSidePanelVisibility("SearchPanel", 'left');
        });

        buttonsDiv.appendChild(searchResultButton);

        var contentButton = document.createElement("button");
        $(contentButton).addClass('content-button');

        var contentSpan = document.createElement("span");
        $(contentSpan).addClass('glyphicon glyphicon-book');
        $(contentButton).append(contentSpan);

        var contentSpanText = document.createElement("span");
        $(contentSpanText).addClass('button-text');
        $(contentSpanText).append("Obsah");
        $(contentButton).append(contentSpanText);

        $(contentButton).click((event: Event) => {
            var panelId = "ObsahPanel";
            if (!this.existSidePanel(panelId)) {
                var contentPanel: ContentPanel = new ContentPanel(panelId, this);
                this.loadSidePanel(contentPanel.panelHtml);
                this.leftSidePanels.push(contentPanel);
            }
            this.changeSidePanelVisibility("ObsahPanel",'left');
        });

        buttonsDiv.appendChild(contentButton);

        pagingDiv.appendChild(buttonsDiv);

        controlsDiv.appendChild(pagingDiv);
        return controlsDiv;
    }

    private existSidePanel(sidePanelIdentificator: string): boolean {
        var sidePanel = document.getElementById(sidePanelIdentificator);
        return ($(sidePanel).length > 0 && sidePanel != null);
    }

    private loadSidePanel(sidePanel) {
        var bodyContainerDiv = $('.reader-body-container');
        $(sidePanel).hide();
        $(bodyContainerDiv).prepend(sidePanel);
    }

    changeSidePanelVisibility(sidePanelIdentificator: string, slideDirection: string) {
        var sidePanel = document.getElementById(sidePanelIdentificator);
        if ($(sidePanel).is(':visible')) {
            if ($(sidePanel).hasClass('ui-draggable')) {
                $(sidePanel).hide();
            } else {
                if (slideDirection) {
                    $(sidePanel).hide('slide', { direction: slideDirection });
                } else {
                    $(sidePanel).hide();
                }
            }
        } else {
            if ($(sidePanel).hasClass("windowed")) {
                var panelInstance = this.findPanelInstanceById(sidePanelIdentificator);
                panelInstance.childwindow.focus();
            }
            else if ($(sidePanel).hasClass('ui-draggable')) {
                $(sidePanel).show();
            } else {
                if (slideDirection) {
                    $(sidePanel).show('slide', { direction: slideDirection });
                } else {
                    $(sidePanel).css('display', '');
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
        var bodyContainerDiv: HTMLDivElement = document.createElement('div');
        $(bodyContainerDiv).addClass('reader-body-container content-container');

        var textPanel: TextPanel = new TextPanel(this.textPanelIdentificator, this);
        this.rightSidePanels.push(textPanel);

        bodyContainerDiv.appendChild(textPanel.panelHtml);

        // Image Panel
        var imagePanel: ImagePanel = new ImagePanel(this.imagePanelIdentificator, this);
        this.rightSidePanels.push(imagePanel);

        $(imagePanel.panelHtml).hide();
        bodyContainerDiv.appendChild(imagePanel.panelHtml);

        return bodyContainerDiv;
    }

    moveToPageNumber(pageIndex: number, scrollTo: boolean) {
        if (pageIndex < 0) {
            pageIndex = 0;
        } else if (pageIndex >= this.pages.length) {
            pageIndex = this.pages.length - 1;
        }
        this.actualPageIndex = pageIndex;
        this.actualizeSlider(pageIndex);
        this.actualizePagination(pageIndex);
        this.notifyPanelsMovePage(pageIndex, scrollTo);
    }

    notifyPanelsMovePage(pageIndex : number, scrollTo: boolean) {
        for (var k = 0; k < this.leftSidePanels.length; k++) {
            this.leftSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }

        for (var k = 0; k < this.rightSidePanels.length; k++) {
            this.rightSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }
    }

    moveToPage(page: string, scrollTo: boolean) {
        var pageIndex: number = $.inArray(page, this.pages);
        if (pageIndex >= 0 && pageIndex < this.pages.length) {
            this.moveToPageNumber(pageIndex, scrollTo);
            
        } else {
            console.log("Page '" + page + "' does not exist");
            //TODO tell user page not exist  
        }
    }

    actualizeSlider(pageIndex: number) {
        var slider = $(this.readerContainer).find('.slider');
        $(slider).slider().slider('value', pageIndex);
        $(slider).find('.ui-slider-handle').find('.tooltip-inner').html("Strana: " + this.pages[pageIndex]);
    }

    actualizePagination(pageIndex: number) {
        var pager = $(this.readerContainer).find('ul.pagination');
        pager.find('li.page-navigation').css('visibility', 'visible');
        pager.find('li.more-pages').css('visibility', 'visible');
        if (pageIndex == 0) {
            pager.find('li.page-navigation-left').css('visibility', 'hidden');
            pager.find('li.more-pages-left').css('visibility', 'hidden');
        } else if (pageIndex == this.pages.length - 1) {
            pager.find('li.page-navigation-right').css('visibility', 'hidden');
            pager.find('li.more-pages-right').css('visibility', 'hidden');
        }

        var pages = $(pager).find('.page');
        $(pages).css('display', 'none');
        $(pages).removeClass('page-active');
        var actualPage = $(pages).filter(function(index) {
            return $(this).data("page-index") === pageIndex;
        });

        var displayPagesOnEachSide = (this.pagerDisplayPages - 1) / 2;
        var displayOnRight = displayPagesOnEachSide;
        var displayOnLeft = displayPagesOnEachSide;
        var pagesOnLeft = pageIndex;
        var pagesOnRight = this.pages.length - (pageIndex + 1);
        if (pagesOnLeft <= displayOnLeft) {
            displayOnRight += displayOnLeft - pagesOnLeft;
            pager.find('li.more-pages-left').css('visibility', 'hidden');
        } else if (pagesOnRight <= displayOnRight) {
            displayOnLeft += displayOnRight - pagesOnRight;
            pager.find('li.more-pages-right').css('visibility', 'hidden');
        }

        var displayedPages = $(pages).filter(function(index) {
            var itemPageIndex = $(this).data("page-index");
            return (itemPageIndex >= pageIndex - displayOnLeft && itemPageIndex <= pageIndex + displayOnRight);
        });
        $(displayedPages).css('display', 'inherit');
        $(actualPage).addClass('page-active');

    }



    addBookmark() {
        var positionStep = 100 / (this.pages.length - 1);
        var bookmarkSpan = document.createElement("span");
        $(bookmarkSpan).addClass('glyphicon glyphicon-bookmark bookmark');
        $(bookmarkSpan).data('page-index', this.actualPageIndex);
        $(bookmarkSpan).data('page-name', this.pages[this.actualPageIndex]);

        var computedPosition = (positionStep * this.actualPageIndex);
        $(bookmarkSpan).css('left', computedPosition + '%');

        $(this.readerContainer).find('.slider').append(bookmarkSpan);
        //TODO populate request on service for adding bookmark to DB

    }

    removeBookmark(): boolean {
        var slider = $(this.readerContainer).find('.slider');
        var bookmarks = $(slider).find('.bookmark');

        if (typeof bookmarks === 'undefined' || bookmarks.length == 0) {
            return false;
        }

        var actualPageName = this.pages[this.actualPageIndex];
        var targetBookmark = $(bookmarks).filter(function(index) {
            return $(this).data("page-name") === actualPageName;
        });

        if (typeof targetBookmark === 'undefined' || targetBookmark.length == 0) {
            return false;
        }

        $(targetBookmark).remove();
        //TODO populate request on service for removing bookmark from DB
        return true;


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
        var allPinned = true;
        for (var i = 0; i < rightPanels.length; i++) {
            var panel = rightPanels[i].panelHtml;
            if (!$(panel).is(':visible') || $(panel).hasClass('ui-draggable')) {
                allPinned = false;
            }
        }

        if (allPinned) {
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
            var zIndex = parseInt($(leftPanel.panelHtml).css('z-index'));
            if (zIndex > max) {
                max = zIndex;
            }
        }

        var rightPanels = this.rightSidePanels;
        for (var i = 0; i < rightPanels.length; i++) {
            var rightPanel = rightPanels[i];
            var zIndex = parseInt($(rightPanel.panelHtml).css('z-index'));
            if (zIndex > max) {
                max = zIndex;
            }
        }

        $(panel.panelHtml).css('z-index', max + 1);
    }
}


class SidePanel {
    panelHtml : HTMLDivElement;
    panelBodyHtml: HTMLDivElement;
    closeButton : HTMLButtonElement;
    pinButton : HTMLButtonElement;
    newWindowButton: HTMLButtonElement;
    identificator: string;
    headerName: string;
    innerContent: HTMLElement;
    parentReader: ReaderModule;
    windowBody: HTMLDivElement;
    childwindow: Window;
    isDraggable: boolean;
    documentWindow: Window;

    public constructor(identificator: string, headerName: string, parentReader: ReaderModule) {
        this.parentReader = parentReader;
        this.identificator = identificator;
        this.headerName = headerName;
        this.isDraggable = false;
        var sidePanelDiv: HTMLDivElement = document.createElement('div');
        sidePanelDiv.id = identificator;
        this.decorateSidePanel(sidePanelDiv);

        var panelHeaderDiv: HTMLDivElement = document.createElement('div');
        $(panelHeaderDiv).addClass('reader-left-panel-header');

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass('panel-header-name');
        $(nameSpan).append(headerName);
        $(panelHeaderDiv).append(nameSpan);

        var sidePanelCloseButton = document.createElement("button");
        $(sidePanelCloseButton).addClass('close-button');
        $(sidePanelCloseButton).click((event: Event) => {
            this.onCloseButtonClick(sidePanelDiv);
        });

        var closeSpan = document.createElement("span");
        $(closeSpan).addClass('glyphicon glyphicon-remove');
        $(sidePanelCloseButton).append(closeSpan);

        this.closeButton = sidePanelCloseButton;

        panelHeaderDiv.appendChild(sidePanelCloseButton);

        var panelPinButton = document.createElement("button");
        $(panelPinButton).addClass('pin-button');
        $(panelPinButton).click((event: Event) => {
            this.onPinButtonClick(sidePanelDiv);
        });

        var pinSpan = document.createElement("span");
        $(pinSpan).addClass('glyphicon glyphicon-pushpin');
        $(panelPinButton).append(pinSpan);

        this.pinButton = panelPinButton;

        panelHeaderDiv.appendChild(panelPinButton);

        var newWindowButton = document.createElement("button");
        $(newWindowButton).addClass('new-window-button');
        $(newWindowButton).click((event: Event) => {
            this.onNewWindowButtonClick(sidePanelDiv);
        });

        var windowSpan = document.createElement("span");
        $(windowSpan).addClass('glyphicon glyphicon-new-window');
        $(newWindowButton).append(windowSpan);

        this.newWindowButton = newWindowButton;

        panelHeaderDiv.appendChild(newWindowButton);

        sidePanelDiv.appendChild(panelHeaderDiv);

        this.innerContent = this.makeBody(this,window);
        var panelBodyDiv = this.makePanelBody(this.innerContent, this, window);

        $(sidePanelDiv).append(panelBodyDiv);

        $(sidePanelDiv).mousedown((event:Event)=> {
            this.parentReader.populatePanelOnTop(this);
        });

        this.panelHtml = sidePanelDiv;
        this.panelBodyHtml = panelBodyDiv;

        
    }

    protected  makePanelBody(innerContent, rootReference, window: Window) : HTMLDivElement {
        var panelBodyDiv: HTMLDivElement = window.document.createElement('div');
        $(panelBodyDiv).addClass('reader-left-panel-body');
        $(panelBodyDiv).append(innerContent);
        return panelBodyDiv;
    }

    protected makeBody(rootReference: SidePanel, window:Window): HTMLElement {
        throw new Error("Not implemented");
    }

    public onMoveToPage(pageIndex: number, scrollTo:boolean) {
        //$(this.panelBodyHtml).append(" pageIndex is " + pageIndex);
        //if (typeof this.windowBody !== 'undefined') {
        //    $(this.windowBody).append(" pageIndex is " + pageIndex);
        //}
    }

    protected placeOnDragStartPosition(sidePanelDiv: HTMLDivElement) {
        var dispersion = Math.floor((Math.random() * 15) + 1) * 3;
        $(sidePanelDiv).css('top', 135 + dispersion);  //TODO kick out magic number
        $(sidePanelDiv).css('left', dispersion);
    }

    protected setRightPanelsLayout(sidePanelDiv: HTMLDivElement) {
        this.parentReader.setRightPanelsLayout();
    }

    makePanelWindow(documentWindow: Window): HTMLDivElement {
        return this.makePanelBody($(this.innerContent).clone(true), this, window);
        //var innerContent = this.makeBody(this, documentWindow);
        //return this.makePanelBody(innerContent, this, documentWindow);
    }

    decorateSidePanel(htmlDivElement: HTMLDivElement) { throw new Error("Not implemented"); }

    onNewWindowButtonClick(sidePanelDiv: HTMLDivElement) {

        //var scripts = document.getElementsByTagName('script');
        //var links = document.getElementsByTagName('link');
        this.closeButton.click();
        var newWindow = window.open("//" + document.domain, '_blank', 'width=400,height=600,resizable=yes');
        newWindow.document.open();

        //newWindow.document.write("<head>");

        //for (var i = 0; i < scripts.length; i++) {
        //    newWindow.document.write(scripts[i].outerHTML);
        //}

        //for (var i = 0; i < links.length; i++) {
        //    newWindow.document.write(links[i].outerHTML);
        //}

        //newWindow.document.write("</head>");
        newWindow.document.close();

        $(newWindow).on("beforeunload",(event: Event) => {
            this.onUnloadWindowMode();
        });

        $(newWindow.document.getElementsByTagName('head')[0]).append($("script").clone(true));
        $(newWindow.document.getElementsByTagName('head')[0]).append($("link").clone(true));

        var panelWindow = this.makePanelWindow(newWindow);

        $(newWindow.document.getElementsByTagName('body')[0]).append(panelWindow);
        $(newWindow.document.getElementsByTagName('body')[0]).css("padding", 0);
        $(newWindow.document.getElementsByTagName('body')[0]).css("background-color", "white");
        newWindow.document.title = this.headerName;
        $(document.getElementById(this.identificator)).addClass("windowed");
        this.windowBody = panelWindow;
        this.childwindow = newWindow;
    }

    onUnloadWindowMode() {
        $(document.getElementById(this.identificator)).removeClass("windowed");
        $(this.windowBody).val('');
        $(this.childwindow).val('');
    }

    onPinButtonClick(sidePanelDiv: HTMLDivElement) { throw new Error("Not implemented"); }

    onCloseButtonClick(sidePanelDiv: HTMLDivElement) { throw new Error("Not implemented"); }


}


class LeftSidePanel extends SidePanel {
    decorateSidePanel(sidePanelDiv: HTMLDivElement) {
        $(sidePanelDiv).addClass('reader-left-panel');
        $(sidePanelDiv).resizable({
            handles: "e",
            maxWidth: 250,
            minWidth: 100
        });
    }

    onPinButtonClick(sidePanelDiv: HTMLDivElement) {
        if ($(sidePanelDiv).data('ui-draggable')) {
            $(sidePanelDiv).draggable("destroy");
            $(sidePanelDiv).css('top', '');
            $(sidePanelDiv).css('left', '');
            $(sidePanelDiv).css('width', "");
            $(sidePanelDiv).css('height', "");
            $(sidePanelDiv).resizable("destroy");
            $(sidePanelDiv).resizable({ handles: "e", maxWidth: 250, minWidth: 100 });
            this.isDraggable = false;
            $(sidePanelDiv).css('z-index', 9999);

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
        if ($(sidePanelDiv).data('ui-draggable')) {
            $(sidePanelDiv).hide();
        } else {
            $(sidePanelDiv).hide('slide', { direction: 'left' });
        }
    }

    //protected makeBody(rootReference: SidePanel, window : Window): HTMLElement {
    //    var movePageButton: HTMLButtonElement = window.document.createElement('button');
    //    movePageButton.textContent = "Move to page 15";
    //    $(movePageButton).click((event: Event) => {
    //        rootReference.parentReader.moveToPageNumber(15, true);
    //    });

    //    return movePageButton;
    //}
}

class SettingsPanel extends LeftSidePanel {

    constructor(identificator: string, readerModule: ReaderModule) {
        super(identificator, "Zobrazení", readerModule);
    }
    
    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var textButtonSpan = window.document.createElement("span");
        $(textButtonSpan).addClass("glyphicon glyphicon-text-size");
        var textButton = window.document.createElement("button");
        $(textButton).addClass("reader-settings-button");
        $(textButton).append(textButtonSpan);

        $(textButton).click((event: Event) => {
            rootReference.parentReader.changeSidePanelVisibility(rootReference.parentReader.textPanelIdentificator, "");
            rootReference.parentReader.setRightPanelsLayout();
        });

        var imageButtonSpan = window.document.createElement("span");
        $(imageButtonSpan).addClass("glyphicon glyphicon-picture");
        var imageButton = window.document.createElement("button");
        $(imageButton).addClass("reader-settings-button");
        $(imageButton).append(imageButtonSpan);

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
        var showPageNameSpan: HTMLSpanElement = window.document.createElement("span");
        showPageNameSpan.innerHTML = "Zobrazit číslování stránek";
        showPageCheckboxDiv.appendChild(showPageNameCheckbox);
        showPageCheckboxDiv.appendChild(showPageNameSpan);

        var showCommentCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        var showCommentCheckbox: HTMLInputElement = window.document.createElement("input");
        showCommentCheckbox.type = "checkbox";
        var showCommentSpan: HTMLSpanElement = window.document.createElement("span");
        showCommentSpan.innerHTML = "Zobrazit komentáře";
        showCommentCheckboxDiv.appendChild(showCommentCheckbox);
        showCommentCheckboxDiv.appendChild(showCommentSpan);

        checkboxesDiv.appendChild(showPageCheckboxDiv);
        checkboxesDiv.appendChild(showCommentCheckboxDiv);
        var innerContent: HTMLDivElement = window.document.createElement("div");
        innerContent.appendChild(buttonsDiv);
        innerContent.appendChild(checkboxesDiv);
        return innerContent;
    }
}

class ContentPanel extends LeftSidePanel {

    constructor(identificator: string, readerModule: ReaderModule) {
        super(identificator, "Obsah", readerModule);
    }

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var bodyDiv: HTMLDivElement = window.document.createElement('div');
        $(bodyDiv).addClass('content-panel-container');
        this.downloadBookContent();
        return bodyDiv;
    }

    private downloadBookContent() {
        
        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.parentReader.bookId},
            url: getBaseUrl() + "Reader/GetBookContent",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                var rootContentItems = response["content"];
                var ulElement = document.createElement("ul");
                $(ulElement).addClass("content-item-root-list");
                for (var i = 0; i < rootContentItems.length; i++) {
                    $(ulElement).append(this.makeContentItem(rootContentItems[i]));
                }

                
                $(this.panelBodyHtml).empty();
                $(this.panelBodyHtml).append(ulElement);

                this.innerContent = this.panelBodyHtml;

                if (typeof this.windowBody !== 'undefined') {
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

    private makeContentItemChilds(contentItem): HTMLUListElement {
        var childItems = contentItem["ChildBookContentItems"];
        if (childItems.length === 0 ) return null;
        var ulElement = document.createElement("ul");
        $(ulElement).addClass("content-item-list");
        for (var i = 0; i < childItems.length; i++) {
            $(ulElement).append(this.makeContentItem(childItems[i]));
        }
        return ulElement;
    }

    private makeContentItem(contentItem): HTMLLIElement {
        var liElement = document.createElement("li");
        $(liElement).addClass("content-item");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            this.parentReader.moveToPage(contentItem["ReferredPageName"], true);
        });
        

        var textSpanElement = document.createElement("span");
        $(textSpanElement).addClass("content-item-text");
        textSpanElement.innerHTML = contentItem["Text"];

        var pageNameSpanElement = document.createElement("span");
        $(pageNameSpanElement).addClass("content-item-page-name");
        pageNameSpanElement.innerHTML = "["+contentItem["ReferredPageName"]+"]";
        
        $(hrefElement).append(pageNameSpanElement);
        $(hrefElement).append(textSpanElement);

        $(liElement).append(hrefElement);
        $(liElement).append(this.makeContentItemChilds(contentItem));
        return liElement;
    }
}


class RightSidePanel extends SidePanel {
    decorateSidePanel(sidePanelDiv: HTMLDivElement) {
        $(sidePanelDiv).addClass('reader-right-panel');
    }

    onPinButtonClick(sidePanelDiv: HTMLDivElement) {
        if ($(sidePanelDiv).data('ui-draggable')) {
            $(sidePanelDiv).draggable("destroy");
            $(sidePanelDiv).css('top', '');
            $(sidePanelDiv).css('left', '');
            $(sidePanelDiv).css('width', "");
            $(sidePanelDiv).css('position', "");
            $(sidePanelDiv).css('height', "");
            $(sidePanelDiv).resizable('destroy');
            this.isDraggable = false;
            $(sidePanelDiv).css('z-index', 9999);

        } else {
            var height = $(sidePanelDiv).css("height");
            var width = $(sidePanelDiv).css("width");
            $(sidePanelDiv).draggable({ containment: "body", appendTo: "body", cursor: "move" });
            $(sidePanelDiv).resizable({ handles: "all", minWidth: 100 });
            $(sidePanelDiv).css("width",width);
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
        var panelBodyDiv: HTMLDivElement = window.document.createElement('div');
        $(panelBodyDiv).addClass('reader-right-panel-body');
        $(panelBodyDiv).append(innerContent);
        return panelBodyDiv;
    }
}

class ImagePanel extends RightSidePanel {

    constructor(identificator: string, readerModule: ReaderModule) {
        super(identificator, "Obrázky", readerModule);
    }

    protected makeBody(rootReference: SidePanel, window: Window):HTMLElement {
        var imageContainerDiv: HTMLDivElement = window.document.createElement('div');
        $(imageContainerDiv).addClass('reader-image-container');
        return imageContainerDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) { 
        var pagePosition = pageIndex + 1;
        $(this.innerContent).empty();
        var image : HTMLImageElement = document.createElement("img");
        image.src = getBaseUrl()+"Editions/Editions/GetBookImage?bookId=" + this.parentReader.bookId + "&position=" + pagePosition;
        $(this.innerContent).append(image);
        if (typeof this.windowBody !== 'undefined') {
            $(this.windowBody).empty();
            $(this.windowBody).append(image);
        }
    }
}

class TextPanel extends RightSidePanel {
    preloadPagesBefore:number;
    preloadPagesAfter:number;

    constructor(identificator: string, readerModule: ReaderModule) {
        super(identificator, "Text", readerModule);
        this.preloadPagesBefore = 5;
        this.preloadPagesAfter = 10;
    }

    protected makeBody(rootReference: SidePanel, window: Window): HTMLElement {
        var textContainerDiv: HTMLDivElement = window.document.createElement('div');
        $(textContainerDiv).addClass('reader-text-container');

        $(textContainerDiv).scroll(function(event: Event) {
            var pages = $(this).find('.page');
            var minOffset = Number.MAX_VALUE;
            var pageWithMinOffset;
            $.each(pages,(index, page) => {
                var pageOfsset = Math.abs($(page).offset().top - $(this).offset().top);
                if (minOffset > pageOfsset) {
                    minOffset = pageOfsset;
                    pageWithMinOffset = page;
                }
            });

            rootReference.parentReader.moveToPage($(pageWithMinOffset).data('page-name'), false);
        });

        var textAreaDiv: HTMLDivElement = window.document.createElement('div');
        $(textAreaDiv).addClass('reader-text');
        for (var i = 0; i < rootReference.parentReader.pages.length; i++) {
            var pageDiv: HTMLDivElement = window.document.createElement('div');
            $(pageDiv).addClass('page');
            $(pageDiv).data('page-name', rootReference.parentReader.pages[i]);
            pageDiv.id = 'page_' + rootReference.parentReader.pages[i];
            textAreaDiv.appendChild(pageDiv);
        }

        var dummyPage: HTMLDivElement = window.document.createElement('div');
        $(dummyPage).addClass('dummy-page');
        textAreaDiv.appendChild(dummyPage);

        textContainerDiv.appendChild(textAreaDiv);
        return textContainerDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        for (var j = 1; pageIndex - j >= 0 && j <= this.preloadPagesBefore; j++) {
            this.displayPage(this.parentReader.pages[pageIndex - j], false);
        }
        for (var i = 1; pageIndex + i < this.parentReader.pages.length && i <= this.preloadPagesAfter; i++) {
            this.displayPage(this.parentReader.pages[pageIndex + i], false);
        }
        this.displayPage(this.parentReader.pages[pageIndex], scrollTo);
    }

    displayPage(pageName: string, scrollTo: boolean) {
        var pageDiv = $(this.parentReader.readerContainer).find('div.reader-text').find('#page_' + pageName);
        var pageLoaded: boolean = $(pageDiv).data('loaded');
        var pageLoading: boolean = $(pageDiv).hasClass('loading');
        if ( (typeof pageLoaded === 'undefined' || !pageLoaded) && !pageLoading ) {
            this.downloadPageByName(pageName);
        }
        if (scrollTo) {
            this.scrollTextToPositionFromTop(0);
            var topOffset = $(pageDiv).offset().top;
            this.scrollTextToPositionFromTop(topOffset);

            if (typeof this.childwindow !== 'undefined') {
                $(".reader-text-container", this.childwindow.document).scrollTop(0);
                var pageToScrollOffset = $('#page_' + pageName, this.childwindow.document).offset().top;
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

    private downloadPageByName(pageName: string) {
        var pageContainer = $(this.parentReader.readerContainer).find('div.reader-text').find('#page_' + pageName);
        $(pageContainer).addClass("loading");
        if (typeof this.windowBody !== 'undefined') {
            $(this.windowBody).find('#page_' + pageName).addClass("loading");
        }
        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.parentReader.bookId, pageName: pageName },
            url: getBaseUrl()+"Reader/GetBookPageByName",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                $(pageContainer).empty();
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
                $(pageContainer).data('loaded', true);

                if (typeof this.windowBody !== 'undefined') {
                    $(this.windowBody).find('#page_' + pageName).removeClass("loading");
                    $(this.windowBody).find('#page_' + pageName).append(response["pageText"]);
                }
            },
            error: (response) => {
                $(pageContainer).empty();
                $(pageContainer).removeClass("loading");
                $(pageContainer).append("Chyba při načítání stránky '"+pageName+"'");
            }
        });
    }
}