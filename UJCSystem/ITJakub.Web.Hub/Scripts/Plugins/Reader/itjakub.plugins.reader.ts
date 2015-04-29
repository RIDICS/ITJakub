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
        this.preloadPagesBefore = 5;
        this.preloadPagesAfter = 10;
    }

    private downloadPageByPosition(pagePosition: number, pageContainer: JQuery) {
        $(pageContainer).addClass("loading");
        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.bookId, pagePosition: pagePosition },
            url: "/Reader/GetBookPageByPosition",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
            },
            error: (response) => {
                $(pageContainer).removeClass("loading");
            }
        });
    }

    private downloadPageByName(pageName: string, pageContainer: JQuery) {
        $(pageContainer).addClass("loading");
        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.bookId, pageName: pageName },
            url: "/Reader/GetBookPageByName",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
            },
            error: (response) => {
                $(pageContainer).removeClass("loading");
            }
        });
    }

    public makeReader(bookId : string, bookTitle : string, pageList) {
        this.bookId = bookId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<string>();
        this.leftSidePanels = new Array<SidePanel>();
        this.rightSidePanels = new Array<SidePanel>();
        
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
        this.scrollTextToPositionFromTop(0);
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

            var textButton = document.createElement("button");
            textButton.textContent = "Zobrazit/skrýt text";
            $(textButton).click((event: Event) => {
                this.changeSidePanelVisibility(this.textPanelIdentificator, "");
                this.setRightPanelsLayout();
            });

            var imagesButton = document.createElement("button");
            imagesButton.textContent = "Zobrazit/skrýt obrázky";
            $(imagesButton).click((event: Event) => {
                this.changeSidePanelVisibility(this.imagePanelIdentificator, "");
                this.setRightPanelsLayout();
            });

            var innerContent: HTMLDivElement = document.createElement("div");
            innerContent.appendChild(textButton);
            innerContent.appendChild(imagesButton);

            var panelId = "EditacniPanel";
            if (!this.existSidePanel(panelId)) {
                var editPanel = new LeftSidePanel(innerContent, panelId, "Zobrazení" , this);
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
            var innerContent = "Obsah vyhledavaciho panelu";
            var panelId = "SearchPanel";
            if (!this.existSidePanel(panelId)) {
                var searchPanel = new LeftSidePanel(innerContent, panelId,"Vyhlédávání", this);
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
            var innerContent = "Obsah";
            var panelId = "ObsahPanel";
            if (!this.existSidePanel(panelId)) {
                var contentPanel = new LeftSidePanel(innerContent, panelId,"Obsah", this);
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

    private changeSidePanelVisibility(sidePanelIdentificator: string, slideDirection: string) {
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
            if ($(sidePanel).hasClass('ui-draggable')) {
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

    private makeReaderBody(): HTMLDivElement {
        var bodyContainerDiv: HTMLDivElement = document.createElement('div');
        $(bodyContainerDiv).addClass('reader-body-container content-container');

        var textPanel = new TextPanel(this.textPanelIdentificator, this);
        this.rightSidePanels.push(textPanel);

        bodyContainerDiv.appendChild(textPanel.panelHtml);

        // Image Panel
        var imagePanel = new ImagePanel(this.imagePanelIdentificator, this);
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
        this.notifyPanelsMovePage(pageIndex);

        for (var j = 1; pageIndex - j >= 0 && j <= this.preloadPagesBefore; j++) {
            this.displayPage(this.pages[pageIndex - j], false);
        }
        this.displayPage(this.pages[pageIndex], scrollTo);
        for (var i = 1; pageIndex + i < this.pages.length && i <= this.preloadPagesAfter; i++) {
            this.displayPage(this.pages[pageIndex + i], false);
        }
    }

    notifyPanelsMovePage(pageIndex : number) {
        for (var k = 0; k < this.leftSidePanels.length; k++) {
            this.leftSidePanels[k].onMoveToPage(pageIndex);
        }

        for (var k = 0; k < this.rightSidePanels.length; k++) {
            this.rightSidePanels[k].onMoveToPage(pageIndex);
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

    displayPage(page: string, scrollTo: boolean) {
        var pageDiv = $(this.readerContainer).find('div.reader-text').find('#page_' + page);
        var pageLoaded: boolean = $(pageDiv).data('loaded');
        if (typeof pageLoaded === 'undefined' || !pageLoaded) {
            this.downloadPageByName(page, $(pageDiv));
            $(pageDiv).data('loaded', true);
        }
        if (scrollTo) {
            this.scrollTextToPositionFromTop(0);
            var topOffset = $(pageDiv).offset().top;
            this.scrollTextToPositionFromTop(topOffset);
        }
    }

    scrollTextToPositionFromTop(topOffset: number) {
        var scrollableContainer = $(this.readerContainer).find('div.reader-text-container');
        var containerTopOffset = $(scrollableContainer).offset().top;
        $(scrollableContainer).scrollTop(topOffset-containerTopOffset);
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
    innerContent: string;
    parentReader: ReaderModule;
    windows: Array<HTMLDivElement>;
    isDraggable: boolean;

    public constructor(innerContent, identificator: string, headerName: string, parentReader: ReaderModule) {
        this.parentReader = parentReader;
        this.identificator = identificator;
        this.innerContent = innerContent;
        this.isDraggable = false;
        this.windows = new Array();
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

        var panelBodyDiv = this.makeBody(innerContent, this);

        $(sidePanelDiv).append(panelBodyDiv);

        $(sidePanelDiv).mousedown((event:Event)=> {
            this.parentReader.populatePanelOnTop(this);
        });

        this.panelHtml = sidePanelDiv;
        this.panelBodyHtml = panelBodyDiv;

        
    }

    protected  makeBody(innerContent, rootReference) : HTMLDivElement {
        var panelBodyDiv: HTMLDivElement = document.createElement('div');
        $(panelBodyDiv).addClass('reader-left-panel-body');

        var movePageButton: HTMLButtonElement = document.createElement('button');
        movePageButton.textContent = "Move to page 15";
        $(movePageButton).click((event: Event) => {
            rootReference.parentReader.moveToPageNumber(15, true);
        });

        $(panelBodyDiv).append(movePageButton);

        $(panelBodyDiv).append(innerContent);

        return panelBodyDiv;
    }

    public onMoveToPage(pageIndex: number) {
        $(this.panelBodyHtml).append(" pageIndex is " + pageIndex);
        for (var i = 0; i < this.windows.length;i++) {
            var windowBody = this.windows[i];
            $(windowBody).append(" pageIndex is " + pageIndex);
        }
    }

    protected placeOnDragStartPosition(sidePanelDiv: HTMLDivElement) {
        var dispersion = Math.floor((Math.random() * 15) + 1) * 3;
        $(sidePanelDiv).css('top', 135 + dispersion);
        $(sidePanelDiv).css('left', dispersion);
    }

    protected setRightPanelsLayout(sidePanelDiv: HTMLDivElement) {
        this.parentReader.setRightPanelsLayout();
    }

    decorateSidePanel(htmlDivElement: HTMLDivElement) { throw new Error("Not implemented"); }

    onPinButtonClick(sidePanelDiv: HTMLDivElement) { throw new Error("Not implemented"); }

    onCloseButtonClick(sidePanelDiv: HTMLDivElement) { throw new Error("Not implemented"); }

    onNewWindowButtonClick(sidePanelDiv: HTMLDivElement) {
        this.closeButton.click();
        var newWindow = window.open("//" + document.domain, '_blank', 'width=400,height=600,resizable=yes');
        newWindow.document.open();
        newWindow.document.close();

        $(newWindow.document.getElementsByTagName('head')[0]).append($("script").clone());
        $(newWindow.document.getElementsByTagName('head')[0]).append($("link").clone());

        var panelBody = this.makeBody(this.innerContent, this);
        $(newWindow.document.getElementsByTagName('body')[0]).append(panelBody);
        $(newWindow.document.getElementsByTagName('body')[0]).css("padding", 0);
        $(newWindow.document.getElementsByTagName('body')[0]).css("background-color", "white");

        this.windows.push(panelBody);
    }
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

    protected  makeBody(innerContent, rootReference): HTMLDivElement {
        var panelBodyDiv: HTMLDivElement = document.createElement('div');
        $(panelBodyDiv).addClass('reader-right-panel-body');
        $(panelBodyDiv).append(innerContent);
        return panelBodyDiv;
    }
}

class ImagePanel extends RightSidePanel {
    imageContaier : HTMLDivElement;

    constructor(identificator: string, readerModule: ReaderModule) {
        var imageContainerDiv: HTMLDivElement = document.createElement('div');
        $(imageContainerDiv).addClass('reader-image-container');
        this.imageContaier = imageContainerDiv;
        super(imageContainerDiv, identificator, "Obrázky", readerModule);
    }

    public onMoveToPage(pageIndex: number) {
        var pagePosition = pageIndex + 1;
        $(this.imageContaier).empty();
        var image : HTMLImageElement = document.createElement("img");
        image.src = "/Editions/Editions/GetBookImage?bookId=" + this.parentReader.bookId + "&position=" + pagePosition;
        $(this.imageContaier).append(image);
        for (var i = 0; i < this.windows.length; i++) {
            var windowBody = this.windows[i];
            $(windowBody).empty();
            $(windowBody).append(image);
        }
    }
}

class TextPanel extends RightSidePanel {
    constructor(identificator: string, readerModule: ReaderModule) {
        var textContainerDiv: HTMLDivElement = document.createElement('div');
        $(textContainerDiv).addClass('reader-text-container');

        $(textContainerDiv).scroll((event: Event) => { //TODO make better scroll event
            //var textContainer = $(readerModule.readerContainer).find('.reader-text-container');
            var pages = $(textContainerDiv).find('.page');
            var minOffset = Number.MAX_VALUE;
            var pageWithMinOffset;
            $.each(pages,(index, page) => {
                var pageOfsset = Math.abs($(page).offset().top - $(textContainerDiv).offset().top);
                if (minOffset > pageOfsset) {
                    minOffset = pageOfsset;
                    pageWithMinOffset = page;
                }
            });

            readerModule.moveToPage($(pageWithMinOffset).data('page-name'), false);
        });

        var textAreaDiv: HTMLDivElement = document.createElement('div');
        $(textAreaDiv).addClass('reader-text');
        for (var i = 0; i < readerModule.pages.length; i++) {
            var pageDiv: HTMLDivElement = document.createElement('div');
            $(pageDiv).addClass('page');
            $(pageDiv).data('page-name', readerModule.pages[i]);
            pageDiv.id = 'page_' + readerModule.pages[i];
            textAreaDiv.appendChild(pageDiv);
        }

        var dummyPage: HTMLDivElement = document.createElement('div');
        $(dummyPage).addClass('dummy-page');
        textAreaDiv.appendChild(dummyPage);

        textContainerDiv.appendChild(textAreaDiv);
        super(textContainerDiv, identificator, "Text", readerModule);
    }

    public onMoveToPage(pageIndex: number) {
    }
}