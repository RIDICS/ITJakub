﻿/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />


class ReaderModule {

    readerContainer: string;
    sliderOnPage: number;
    actualPageIndex: number;
    pages: Array<string>;
    pagerDisplayPages: number;
    preloadPagesBefore: number;
    preloadPagesAfter: number;
    book: IBookInfo
    loadedBookContent : boolean;

    constructor(readerContainer: string) {
        this.readerContainer = readerContainer;
        this.pagerDisplayPages = 5;
        this.preloadPagesBefore = 2;
        this.preloadPagesAfter = 2;
    }

    private downloadPageList() {
        $.ajax({
            type: "GET",
            traditional: true,
            async: false,
            data: { bookId: this.book.BookId },
            url: "/Reader/GetBookPageList",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                var pages = response["pageList"];
                for (var i = 0; i < pages.length; i++) {
                    this.pages.push(pages[i]["Text"]);
                }
                   
            }
        });
    }

    private downloadPageByPosition(pagePosition: number, pageContainer: JQuery) {
        $(pageContainer).addClass("loading");
        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.book.BookId, pagePosition : pagePosition },
            url: "/Reader/GetBookPageByPosition",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
            }
        });
    }

    private downloadPageByName(pageName: string, pageContainer: JQuery) {
        $(pageContainer).addClass("loading");
        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.book.BookId, pageName: pageName },
            url: "/Reader/GetBookPageByName",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
            }
        });
    }

    public makeReader(book: IBookInfo) {
        this.book = book;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<string>();
        this.downloadPageList();

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


        var title = this.makeTitle(book);
        readerHeadDiv.appendChild(title);


        var controls = this.makeControls(book);
        readerHeadDiv.appendChild(controls);
        readerDiv.appendChild(readerHeadDiv);

        var readerBodyDiv = this.makeReaderBody(book);
        readerDiv.appendChild(readerBodyDiv);

        $(this.readerContainer).append(readerDiv);

        this.moveToPageNumber(0, false); //load first page
        this.scrollTextToPositionFromTop(0);
    }

    private makeTitle(book: IBookInfo): HTMLDivElement {
        var titleDiv: HTMLDivElement = document.createElement('div');
        $(titleDiv).addClass('title');
        titleDiv.innerHTML = book.Name;
        return titleDiv;
    }

    private makeControls(book: IBookInfo): HTMLDivElement {
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
                    this.moveToPageNumber(ui.value, true);
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
        $(commentSpanText).append("Nastavení prohlížení");
        $(commentButton).append(commentSpanText);

        $(commentButton).click((event: Event) => {
            var innerContent = "Obsah editacniho panelu";
            var panelId = "EditacniPanel";
            if (!this.existSidePanel(panelId)) {
                this.loadSidePanel(this.makeSidePanel(innerContent, panelId));
            }
            this.changeSidePanelVisibility("EditacniPanel");
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
                this.loadSidePanel(this.makeSidePanel(innerContent, panelId));
            }
            this.changeSidePanelVisibility("SearchPanel");
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
            if(!this.existSidePanel(panelId)){
                this.loadSidePanel(this.makeSidePanel(innerContent, panelId));
            }
            this.changeSidePanelVisibility("ObsahPanel");
        });

        buttonsDiv.appendChild(contentButton);

        pagingDiv.appendChild(buttonsDiv);

        controlsDiv.appendChild(pagingDiv);
        return controlsDiv;
    }

    private makeSidePanel(innerContent, identificator: string): HTMLDivElement {

        var leftPanelDiv: HTMLDivElement = document.createElement('div');
        leftPanelDiv.id = identificator;
        $(leftPanelDiv).addClass('reader-left-panel');
        $(leftPanelDiv).resizable({
            handles: "e",
            maxWidth: 250,
            minWidth: 100
        });

        var leftPanelHeaderDiv: HTMLDivElement = document.createElement('div');
        $(leftPanelHeaderDiv).addClass('reader-left-panel-header');

        var sidePanelCloseButton = document.createElement("button");
        $(sidePanelCloseButton).addClass('close-button');
        $(sidePanelCloseButton).click((event: Event) => {
            $(leftPanelDiv).hide();
        });

        var closeSpan = document.createElement("span");
        $(closeSpan).addClass('glyphicon glyphicon-remove');
        $(sidePanelCloseButton).append(closeSpan);

        leftPanelHeaderDiv.appendChild(sidePanelCloseButton);

        var leftPanelPinButton = document.createElement("button");
        $(leftPanelPinButton).addClass('pin-button');
        $(leftPanelPinButton).click((event: Event) => {
            if ($(leftPanelDiv).data('ui-draggable')) {
                $(leftPanelDiv).draggable("destroy");
                $(leftPanelDiv).css('top', '');
                $(leftPanelDiv).css('left', '');
                $(leftPanelDiv).css('width', "");
                $(leftPanelDiv).css('height', "");
                $(leftPanelDiv).resizable("destroy");
                $(leftPanelDiv).resizable({ handles: "e", maxWidth: 250, minWidth: 100 });

            }
            else {
                $(leftPanelDiv).draggable({ containment: "body", appendTo: "body" });
                $(leftPanelDiv).resizable("destroy");
                $(leftPanelDiv).resizable({ handles: "all", minWidth: 100 });
            }
        });

        var pinSpan = document.createElement("span");
        $(pinSpan).addClass('glyphicon glyphicon-pushpin');
        $(leftPanelPinButton).append(pinSpan);

        leftPanelHeaderDiv.appendChild(leftPanelPinButton);

        leftPanelDiv.appendChild(leftPanelHeaderDiv);

        $(leftPanelDiv).append(innerContent);

        return leftPanelDiv;
    }

    private existSidePanel(sidePanelIdentificator: string) : boolean {
        var sidePanel = document.getElementById(sidePanelIdentificator);
        return ($(sidePanel).length > 0 && sidePanel !=null);
    }

    private loadSidePanel(sidePanel) {
        var bodyContainerDiv = $('.reader-body-container');
        $(sidePanel).hide();
        $(bodyContainerDiv).prepend(sidePanel);
    }

    private changeSidePanelVisibility(sidePanelIdentificator: string) {
        var sidePanel = document.getElementById(sidePanelIdentificator);
        if ($(sidePanel).is(':visible')) {
            if ($(sidePanel).data('ui-draggable')) {
                $(sidePanel).hide();
            } else {
                $(sidePanel).hide('slide', { direction: 'left' });
            }
        } else {
            if ($(sidePanel).data('ui-draggable')) {
                $(sidePanel).show();
            } else {
                $(sidePanel).show('slide', { direction: 'left' });
            }
        }
      
    }

    private makeReaderBody(book: IBookInfo): HTMLDivElement {
        var bodyContainerDiv: HTMLDivElement = document.createElement('div');
        $(bodyContainerDiv).addClass('reader-body-container content-container');


        var textContainerDiv: HTMLDivElement = document.createElement('div');
        $(textContainerDiv).addClass('reader-text-container');

        $(textContainerDiv).scroll((event: Event) => { //TODO make better scroll event
            var pages = $(this.readerContainer).find('.reader-text-container').find('.page');
            var minOffset = Number.MAX_VALUE;
            var pageWithMinOffset;
            $.each(pages, (index, page) => {
                var pageOfsset = Math.abs($(page).offset().top);
                if (minOffset > pageOfsset) {
                    minOffset = pageOfsset;
                    pageWithMinOffset = page;
                }
            });

            this.moveToPage($(pageWithMinOffset).data('page-name'), false);
        });

        var textAreaDiv: HTMLDivElement = document.createElement('div');
        $(textAreaDiv).addClass('reader-text');
        for (var i = 0; i < this.pages.length; i++) {
            var pageDiv: HTMLDivElement = document.createElement('div');
            $(pageDiv).addClass('page');
            $(pageDiv).data('page-name', this.pages[i]);
            pageDiv.id = 'page_' + this.pages[i];
            textAreaDiv.appendChild(pageDiv);
        }

        textContainerDiv.appendChild(textAreaDiv);

        bodyContainerDiv.appendChild(textContainerDiv);
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
        for (var j = 1; pageIndex - j >= 0 && j <= this.preloadPagesBefore; j++) {
            this.displayPage(this.pages[pageIndex - j], false);
        }
        this.displayPage(this.pages[pageIndex], scrollTo);
        for (var i = 1; pageIndex + i < this.pages.length && i <= this.preloadPagesAfter; i++) {
            this.displayPage(this.pages[pageIndex + i], false);
        }
    }

    moveToPage(page: string, scrollTo: boolean) {
        var pageIndex: number = $.inArray(page, this.pages);
        if (pageIndex >= 0 && pageIndex < this.pages.length) {
            this.moveToPageNumber(pageIndex, scrollTo);
        } else {
            console.log("Page '"+page+"' does not exist");
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
        $(scrollableContainer).scrollTop(topOffset);
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
}