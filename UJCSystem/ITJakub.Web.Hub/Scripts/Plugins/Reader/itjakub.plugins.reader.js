/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ReaderModule = (function () {
    function ReaderModule(readerContainer) {
        this.imagePanelIdentificator = "ImagePanel";
        this.textPanelIdentificator = "TextPanel";
        this.readerContainer = readerContainer;
        this.pagerDisplayPages = 5;
    }
    ReaderModule.prototype.makeReader = function (bookId, bookTitle, pageList) {
        var _this = this;
        this.bookId = bookId;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array();
        this.leftSidePanels = new Array();
        this.rightSidePanels = new Array();
        $(window).on("beforeunload", function (event) {
            for (var k = 0; k < _this.leftSidePanels.length; k++) {
                _this.leftSidePanels[k].window.close();
            }
            for (var k = 0; k < _this.rightSidePanels.length; k++) {
                _this.rightSidePanels[k].window.close();
            }
        });
        for (var i = 0; i < pageList.length; i++) {
            this.pages.push(pageList[i]["Text"]);
        }
        $(this.readerContainer).empty();
        var readerDiv = document.createElement('div');
        $(readerDiv).addClass('reader');
        var readerHeadDiv = document.createElement('div');
        $(readerHeadDiv).addClass('reader-head content-container');
        var fullscreenButton = document.createElement("button");
        $(fullscreenButton).addClass('fullscreen-button');
        var fullscreenSpan = document.createElement("span");
        $(fullscreenSpan).addClass('glyphicon glyphicon-fullscreen');
        $(fullscreenButton).append(fullscreenSpan);
        $(fullscreenButton).click(function (event) {
            $(_this.readerContainer).find('.reader').addClass('fullscreen');
        });
        readerHeadDiv.appendChild(fullscreenButton);
        var fullscreenCloseButton = document.createElement("button");
        $(fullscreenCloseButton).addClass('fullscreen-close-button');
        var closeSpan = document.createElement("span");
        $(closeSpan).addClass('glyphicon glyphicon-remove');
        $(fullscreenCloseButton).append(closeSpan);
        $(fullscreenCloseButton).click(function (event) {
            $(_this.readerContainer).find('.reader').removeClass('fullscreen');
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
    };
    ReaderModule.prototype.makeTitle = function (bookTitle) {
        var titleDiv = document.createElement('div');
        $(titleDiv).addClass('title');
        titleDiv.innerHTML = bookTitle;
        return titleDiv;
    };
    ReaderModule.prototype.makeControls = function () {
        var _this = this;
        var controlsDiv = document.createElement('div');
        $(controlsDiv).addClass('reader-controls content-container');
        var slider = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: this.pages.length - 1,
            value: 0,
            start: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
            },
            stop: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').fadeOut(1000);
            },
            slide: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').stop(true, true);
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html("Strana: " + _this.pages[ui.value]);
            },
            change: function (event, ui) {
                if (_this.actualPageIndex !== ui.value) {
                    _this.moveToPageNumber(ui.value, true);
                }
            }
        });
        var sliderTooltip = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);
        var innerTooltip = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html("Strana: " + this.pages[0]);
        sliderTooltip.appendChild(innerTooltip);
        $(sliderTooltip).hide();
        var sliderHandle = $(slider).find('.ui-slider-handle');
        $(sliderHandle).append(sliderTooltip);
        $(sliderHandle).hover(function (event) {
            $(event.target).find('.slider-tip').stop(true, true);
            $(event.target).find('.slider-tip').show();
        });
        $(sliderHandle).mouseout(function (event) {
            $(event.target).find('.slider-tip').fadeOut(1000);
        });
        controlsDiv.appendChild(slider);
        var pagingDiv = document.createElement('div');
        $(pagingDiv).addClass('paging');
        var pageInputDiv = document.createElement('div');
        $(pageInputDiv).addClass('page-input');
        var pageInputText = document.createElement("input");
        pageInputText.setAttribute("type", "text");
        pageInputText.setAttribute("id", "pageInputText");
        $(pageInputText).addClass('page-input-text');
        pageInputDiv.appendChild(pageInputText);
        var pageInputButton = document.createElement("button");
        pageInputButton.innerHTML = "Přejít na stránku";
        $(pageInputButton).addClass('page-input-button');
        $(pageInputButton).click(function (event) {
            _this.moveToPage($('#pageInputText').val(), true);
        });
        pageInputDiv.appendChild(pageInputButton);
        pagingDiv.appendChild(pageInputDiv);
        var paginationUl = document.createElement('ul');
        $(paginationUl).addClass('pagination pagination-sm');
        var liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-left');
        var anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '|<';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.moveToPageNumber(0, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-left');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<<';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.moveToPageNumber(_this.actualPageIndex - 5, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-left');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.moveToPageNumber(_this.actualPageIndex - 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        $(liElement).addClass('more-pages more-pages-left');
        liElement.innerHTML = '...';
        paginationUl.appendChild(liElement);
        $.each(this.pages, function (index, page) {
            liElement = document.createElement('li');
            $(liElement).addClass('page');
            $(liElement).data('page-index', index);
            anchor = document.createElement('a');
            anchor.href = '#';
            anchor.innerHTML = page;
            $(anchor).click(function (event) {
                event.stopPropagation();
                _this.moveToPage(page, true);
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
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.moveToPageNumber(_this.actualPageIndex + 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-right');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>>';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.moveToPageNumber(_this.actualPageIndex + 5, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        $(liElement).addClass('page-navigation page-navigation-right');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>|';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.moveToPageNumber(_this.pages.length - 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        pagingDiv.appendChild(paginationUl);
        var buttonsDiv = document.createElement("div");
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
        $(bookmarkButton).click(function (event) {
            if (!_this.removeBookmark()) {
                _this.addBookmark();
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
        $(commentButton).click(function (event) {
            var panelId = "EditacniPanel";
            if (!_this.existSidePanel(panelId)) {
                var editPanel = new SettingsPanel(panelId, _this);
                _this.loadSidePanel(editPanel.panelHtml);
                _this.leftSidePanels.push(editPanel);
            }
            _this.changeSidePanelVisibility("EditacniPanel", 'left');
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
        $(searchResultButton).click(function (event) {
            var panelId = "SearchPanel";
            if (!_this.existSidePanel(panelId)) {
                var searchPanel = new LeftSidePanel(panelId, "Vyhlédávání", _this);
                _this.loadSidePanel(searchPanel.panelHtml);
                _this.leftSidePanels.push(searchPanel);
            }
            _this.changeSidePanelVisibility("SearchPanel", 'left');
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
        $(contentButton).click(function (event) {
            var panelId = "ObsahPanel";
            if (!_this.existSidePanel(panelId)) {
                var contentPanel = new LeftSidePanel(panelId, "Obsah", _this);
                _this.loadSidePanel(contentPanel.panelHtml);
                _this.leftSidePanels.push(contentPanel);
            }
            _this.changeSidePanelVisibility("ObsahPanel", 'left');
        });
        buttonsDiv.appendChild(contentButton);
        pagingDiv.appendChild(buttonsDiv);
        controlsDiv.appendChild(pagingDiv);
        return controlsDiv;
    };
    ReaderModule.prototype.existSidePanel = function (sidePanelIdentificator) {
        var sidePanel = document.getElementById(sidePanelIdentificator);
        return ($(sidePanel).length > 0 && sidePanel != null);
    };
    ReaderModule.prototype.loadSidePanel = function (sidePanel) {
        var bodyContainerDiv = $('.reader-body-container');
        $(sidePanel).hide();
        $(bodyContainerDiv).prepend(sidePanel);
    };
    ReaderModule.prototype.changeSidePanelVisibility = function (sidePanelIdentificator, slideDirection) {
        var sidePanel = document.getElementById(sidePanelIdentificator);
        if ($(sidePanel).is(':visible')) {
            if ($(sidePanel).hasClass('ui-draggable')) {
                $(sidePanel).hide();
            }
            else {
                if (slideDirection) {
                    $(sidePanel).hide('slide', { direction: slideDirection });
                }
                else {
                    $(sidePanel).hide();
                }
            }
        }
        else {
            if ($(sidePanel).hasClass("windowed")) {
                var panelInstance = this.findPanelInstanceById(sidePanelIdentificator);
                panelInstance.window.focus();
            }
            else if ($(sidePanel).hasClass('ui-draggable')) {
                $(sidePanel).show();
            }
            else {
                if (slideDirection) {
                    $(sidePanel).show('slide', { direction: slideDirection });
                }
                else {
                    $(sidePanel).css('display', '');
                }
            }
        }
    };
    ReaderModule.prototype.findPanelInstanceById = function (panelIdentificator) {
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
    };
    ReaderModule.prototype.makeReaderBody = function () {
        var bodyContainerDiv = document.createElement('div');
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
    };
    ReaderModule.prototype.moveToPageNumber = function (pageIndex, scrollTo) {
        if (pageIndex < 0) {
            pageIndex = 0;
        }
        else if (pageIndex >= this.pages.length) {
            pageIndex = this.pages.length - 1;
        }
        this.actualPageIndex = pageIndex;
        this.actualizeSlider(pageIndex);
        this.actualizePagination(pageIndex);
        this.notifyPanelsMovePage(pageIndex, scrollTo);
    };
    ReaderModule.prototype.notifyPanelsMovePage = function (pageIndex, scrollTo) {
        for (var k = 0; k < this.leftSidePanels.length; k++) {
            this.leftSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }
        for (var k = 0; k < this.rightSidePanels.length; k++) {
            this.rightSidePanels[k].onMoveToPage(pageIndex, scrollTo);
        }
    };
    ReaderModule.prototype.moveToPage = function (page, scrollTo) {
        var pageIndex = $.inArray(page, this.pages);
        if (pageIndex >= 0 && pageIndex < this.pages.length) {
            this.moveToPageNumber(pageIndex, scrollTo);
        }
        else {
            console.log("Page '" + page + "' does not exist");
        }
    };
    ReaderModule.prototype.actualizeSlider = function (pageIndex) {
        var slider = $(this.readerContainer).find('.slider');
        $(slider).slider().slider('value', pageIndex);
        $(slider).find('.ui-slider-handle').find('.tooltip-inner').html("Strana: " + this.pages[pageIndex]);
    };
    ReaderModule.prototype.actualizePagination = function (pageIndex) {
        var pager = $(this.readerContainer).find('ul.pagination');
        pager.find('li.page-navigation').css('visibility', 'visible');
        pager.find('li.more-pages').css('visibility', 'visible');
        if (pageIndex == 0) {
            pager.find('li.page-navigation-left').css('visibility', 'hidden');
            pager.find('li.more-pages-left').css('visibility', 'hidden');
        }
        else if (pageIndex == this.pages.length - 1) {
            pager.find('li.page-navigation-right').css('visibility', 'hidden');
            pager.find('li.more-pages-right').css('visibility', 'hidden');
        }
        var pages = $(pager).find('.page');
        $(pages).css('display', 'none');
        $(pages).removeClass('page-active');
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
            pager.find('li.more-pages-left').css('visibility', 'hidden');
        }
        else if (pagesOnRight <= displayOnRight) {
            displayOnLeft += displayOnRight - pagesOnRight;
            pager.find('li.more-pages-right').css('visibility', 'hidden');
        }
        var displayedPages = $(pages).filter(function (index) {
            var itemPageIndex = $(this).data("page-index");
            return (itemPageIndex >= pageIndex - displayOnLeft && itemPageIndex <= pageIndex + displayOnRight);
        });
        $(displayedPages).css('display', 'inherit');
        $(actualPage).addClass('page-active');
    };
    ReaderModule.prototype.addBookmark = function () {
        var positionStep = 100 / (this.pages.length - 1);
        var bookmarkSpan = document.createElement("span");
        $(bookmarkSpan).addClass('glyphicon glyphicon-bookmark bookmark');
        $(bookmarkSpan).data('page-index', this.actualPageIndex);
        $(bookmarkSpan).data('page-name', this.pages[this.actualPageIndex]);
        var computedPosition = (positionStep * this.actualPageIndex);
        $(bookmarkSpan).css('left', computedPosition + '%');
        $(this.readerContainer).find('.slider').append(bookmarkSpan);
        //TODO populate request on service for adding bookmark to DB
    };
    ReaderModule.prototype.removeBookmark = function () {
        var slider = $(this.readerContainer).find('.slider');
        var bookmarks = $(slider).find('.bookmark');
        if (typeof bookmarks === 'undefined' || bookmarks.length == 0) {
            return false;
        }
        var actualPageName = this.pages[this.actualPageIndex];
        var targetBookmark = $(bookmarks).filter(function (index) {
            return $(this).data("page-name") === actualPageName;
        });
        if (typeof targetBookmark === 'undefined' || targetBookmark.length == 0) {
            return false;
        }
        $(targetBookmark).remove();
        //TODO populate request on service for removing bookmark from DB
        return true;
    };
    ReaderModule.prototype.repaint = function () {
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
    };
    ReaderModule.prototype.setRightPanelsLayout = function () {
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
        }
        else {
            $(".reader-body-container").removeClass("both-pinned");
        }
    };
    ReaderModule.prototype.populatePanelOnTop = function (panel) {
        if (!panel.isDraggable) {
            return;
        }
        var max = 0;
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
    };
    return ReaderModule;
})();
var SidePanel = (function () {
    function SidePanel(identificator, headerName, parentReader) {
        var _this = this;
        this.parentReader = parentReader;
        this.identificator = identificator;
        this.headerName = headerName;
        this.isDraggable = false;
        var sidePanelDiv = document.createElement('div');
        sidePanelDiv.id = identificator;
        this.decorateSidePanel(sidePanelDiv);
        var panelHeaderDiv = document.createElement('div');
        $(panelHeaderDiv).addClass('reader-left-panel-header');
        var nameSpan = document.createElement("span");
        $(nameSpan).addClass('panel-header-name');
        $(nameSpan).append(headerName);
        $(panelHeaderDiv).append(nameSpan);
        var sidePanelCloseButton = document.createElement("button");
        $(sidePanelCloseButton).addClass('close-button');
        $(sidePanelCloseButton).click(function (event) {
            _this.onCloseButtonClick(sidePanelDiv);
        });
        var closeSpan = document.createElement("span");
        $(closeSpan).addClass('glyphicon glyphicon-remove');
        $(sidePanelCloseButton).append(closeSpan);
        this.closeButton = sidePanelCloseButton;
        panelHeaderDiv.appendChild(sidePanelCloseButton);
        var panelPinButton = document.createElement("button");
        $(panelPinButton).addClass('pin-button');
        $(panelPinButton).click(function (event) {
            _this.onPinButtonClick(sidePanelDiv);
        });
        var pinSpan = document.createElement("span");
        $(pinSpan).addClass('glyphicon glyphicon-pushpin');
        $(panelPinButton).append(pinSpan);
        this.pinButton = panelPinButton;
        panelHeaderDiv.appendChild(panelPinButton);
        var newWindowButton = document.createElement("button");
        $(newWindowButton).addClass('new-window-button');
        $(newWindowButton).click(function (event) {
            _this.onNewWindowButtonClick(sidePanelDiv);
        });
        var windowSpan = document.createElement("span");
        $(windowSpan).addClass('glyphicon glyphicon-new-window');
        $(newWindowButton).append(windowSpan);
        this.newWindowButton = newWindowButton;
        panelHeaderDiv.appendChild(newWindowButton);
        sidePanelDiv.appendChild(panelHeaderDiv);
        this.innerContent = this.makeBody(this);
        var panelBodyDiv = this.makePanelBody(this.innerContent, this);
        $(sidePanelDiv).append(panelBodyDiv);
        $(sidePanelDiv).mousedown(function (event) {
            _this.parentReader.populatePanelOnTop(_this);
        });
        this.panelHtml = sidePanelDiv;
        this.panelBodyHtml = panelBodyDiv;
    }
    SidePanel.prototype.makePanelBody = function (innerContent, rootReference) {
        var panelBodyDiv = document.createElement('div');
        $(panelBodyDiv).addClass('reader-left-panel-body');
        $(panelBodyDiv).append(innerContent);
        return panelBodyDiv;
    };
    SidePanel.prototype.makeBody = function (rootReference) {
        throw new Error("Not implemented");
    };
    SidePanel.prototype.onMoveToPage = function (pageIndex, scrollTo) {
        $(this.panelBodyHtml).append(" pageIndex is " + pageIndex);
        if (typeof this.windowBody !== 'undefined') {
            $(this.windowBody).append(" pageIndex is " + pageIndex);
        }
    };
    SidePanel.prototype.placeOnDragStartPosition = function (sidePanelDiv) {
        var dispersion = Math.floor((Math.random() * 15) + 1) * 3;
        $(sidePanelDiv).css('top', 135 + dispersion); //TODO kick out magic number
        $(sidePanelDiv).css('left', dispersion);
    };
    SidePanel.prototype.setRightPanelsLayout = function (sidePanelDiv) {
        this.parentReader.setRightPanelsLayout();
    };
    SidePanel.prototype.onNewWindowButtonClick = function (sidePanelDiv) {
        var _this = this;
        this.closeButton.click();
        var newWindow = window.open("//" + document.domain, '_blank', 'width=400,height=600,resizable=yes');
        newWindow.document.open();
        newWindow.document.close();
        $(newWindow).on("beforeunload", function (event) {
            $(document.getElementById(_this.identificator)).removeClass("windowed");
            $(_this.windowBody).val('');
            $(_this.window).val('');
        });
        $(newWindow.document.getElementsByTagName('head')[0]).append($("script").clone());
        $(newWindow.document.getElementsByTagName('head')[0]).append($("link").clone());
        var panelWindow = this.makePanelWindow();
        $(newWindow.document.getElementsByTagName('body')[0]).append(panelWindow);
        $(newWindow.document.getElementsByTagName('body')[0]).css("padding", 0);
        $(newWindow.document.getElementsByTagName('body')[0]).css("background-color", "white");
        newWindow.document.title = this.headerName;
        $(document.getElementById(this.identificator)).addClass("windowed");
        this.windowBody = panelWindow;
        this.window = newWindow;
    };
    SidePanel.prototype.makePanelWindow = function () {
        return this.makePanelBody($(this.innerContent).clone(true), this);
    };
    SidePanel.prototype.decorateSidePanel = function (htmlDivElement) {
        throw new Error("Not implemented");
    };
    SidePanel.prototype.onPinButtonClick = function (sidePanelDiv) {
        throw new Error("Not implemented");
    };
    SidePanel.prototype.onCloseButtonClick = function (sidePanelDiv) {
        throw new Error("Not implemented");
    };
    return SidePanel;
})();
var LeftSidePanel = (function (_super) {
    __extends(LeftSidePanel, _super);
    function LeftSidePanel() {
        _super.apply(this, arguments);
    }
    LeftSidePanel.prototype.decorateSidePanel = function (sidePanelDiv) {
        $(sidePanelDiv).addClass('reader-left-panel');
        $(sidePanelDiv).resizable({
            handles: "e",
            maxWidth: 250,
            minWidth: 100
        });
    };
    LeftSidePanel.prototype.onPinButtonClick = function (sidePanelDiv) {
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
        }
        else {
            $(sidePanelDiv).draggable({ containment: "body", appendTo: "body", cursor: "move" });
            $(sidePanelDiv).resizable("destroy");
            $(sidePanelDiv).resizable({ handles: "all", minWidth: 100 });
            this.placeOnDragStartPosition(sidePanelDiv);
            this.isDraggable = true;
            this.parentReader.populatePanelOnTop(this);
        }
        this.setRightPanelsLayout(sidePanelDiv);
    };
    LeftSidePanel.prototype.onCloseButtonClick = function (sidePanelDiv) {
        if ($(sidePanelDiv).data('ui-draggable')) {
            $(sidePanelDiv).hide();
        }
        else {
            $(sidePanelDiv).hide('slide', { direction: 'left' });
        }
    };
    LeftSidePanel.prototype.makeBody = function (rootReference) {
        var movePageButton = document.createElement('button');
        movePageButton.textContent = "Move to page 15";
        $(movePageButton).click(function (event) {
            rootReference.parentReader.moveToPageNumber(15, true);
        });
        return movePageButton;
    };
    return LeftSidePanel;
})(SidePanel);
var SettingsPanel = (function (_super) {
    __extends(SettingsPanel, _super);
    function SettingsPanel(identificator, readerModule) {
        _super.call(this, identificator, "Zobrazení", readerModule);
    }
    SettingsPanel.prototype.makeBody = function (rootReference) {
        var textButton = document.createElement("button");
        textButton.textContent = "Zobrazit/skrýt text";
        $(textButton).click(function (event) {
            rootReference.parentReader.changeSidePanelVisibility(rootReference.parentReader.textPanelIdentificator, "");
            rootReference.parentReader.setRightPanelsLayout();
        });
        var imagesButton = document.createElement("button");
        imagesButton.textContent = "Zobrazit/skrýt obrázky";
        $(imagesButton).click(function (event) {
            rootReference.parentReader.changeSidePanelVisibility(rootReference.parentReader.imagePanelIdentificator, "");
            rootReference.parentReader.setRightPanelsLayout();
        });
        var innerContent = document.createElement("div");
        innerContent.appendChild(textButton);
        innerContent.appendChild(imagesButton);
        return innerContent;
    };
    return SettingsPanel;
})(LeftSidePanel);
var RightSidePanel = (function (_super) {
    __extends(RightSidePanel, _super);
    function RightSidePanel() {
        _super.apply(this, arguments);
    }
    RightSidePanel.prototype.decorateSidePanel = function (sidePanelDiv) {
        $(sidePanelDiv).addClass('reader-right-panel');
    };
    RightSidePanel.prototype.onPinButtonClick = function (sidePanelDiv) {
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
        }
        else {
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
    };
    RightSidePanel.prototype.onCloseButtonClick = function (sidePanelDiv) {
        $(sidePanelDiv).hide();
        this.setRightPanelsLayout(sidePanelDiv);
    };
    RightSidePanel.prototype.onNewWindowButtonClick = function (sidePanelDiv) {
        _super.prototype.onNewWindowButtonClick.call(this, sidePanelDiv);
        this.setRightPanelsLayout(sidePanelDiv);
    };
    RightSidePanel.prototype.makePanelBody = function (innerContent, rootReference) {
        var panelBodyDiv = document.createElement('div');
        $(panelBodyDiv).addClass('reader-right-panel-body');
        $(panelBodyDiv).append(innerContent);
        return panelBodyDiv;
    };
    return RightSidePanel;
})(SidePanel);
var ImagePanel = (function (_super) {
    __extends(ImagePanel, _super);
    function ImagePanel(identificator, readerModule) {
        _super.call(this, identificator, "Obrázky", readerModule);
    }
    ImagePanel.prototype.makeBody = function (rootReference) {
        var imageContainerDiv = document.createElement('div');
        $(imageContainerDiv).addClass('reader-image-container');
        return imageContainerDiv;
    };
    ImagePanel.prototype.onMoveToPage = function (pageIndex, scrollTo) {
        var pagePosition = pageIndex + 1;
        $(this.innerContent).empty();
        var image = document.createElement("img");
        image.src = "/Editions/Editions/GetBookImage?bookId=" + this.parentReader.bookId + "&position=" + pagePosition;
        $(this.innerContent).append(image);
        if (typeof this.windowBody !== 'undefined') {
            $(this.windowBody).empty();
            $(this.windowBody).append(image);
        }
    };
    return ImagePanel;
})(RightSidePanel);
var TextPanel = (function (_super) {
    __extends(TextPanel, _super);
    function TextPanel(identificator, readerModule) {
        _super.call(this, identificator, "Text", readerModule);
        this.preloadPagesBefore = 5;
        this.preloadPagesAfter = 10;
    }
    TextPanel.prototype.makeBody = function (rootReference) {
        var textContainerDiv = document.createElement('div');
        $(textContainerDiv).addClass('reader-text-container');
        $(textContainerDiv).scroll(function (event) {
            var _this = this;
            var pages = $(this).find('.page');
            var minOffset = Number.MAX_VALUE;
            var pageWithMinOffset;
            $.each(pages, function (index, page) {
                var pageOfsset = Math.abs($(page).offset().top - $(_this).offset().top);
                if (minOffset > pageOfsset) {
                    minOffset = pageOfsset;
                    pageWithMinOffset = page;
                }
            });
            rootReference.parentReader.moveToPage($(pageWithMinOffset).data('page-name'), false);
        });
        var textAreaDiv = document.createElement('div');
        $(textAreaDiv).addClass('reader-text');
        for (var i = 0; i < rootReference.parentReader.pages.length; i++) {
            var pageDiv = document.createElement('div');
            $(pageDiv).addClass('page');
            $(pageDiv).data('page-name', rootReference.parentReader.pages[i]);
            pageDiv.id = 'page_' + rootReference.parentReader.pages[i];
            textAreaDiv.appendChild(pageDiv);
        }
        var dummyPage = document.createElement('div');
        $(dummyPage).addClass('dummy-page');
        textAreaDiv.appendChild(dummyPage);
        textContainerDiv.appendChild(textAreaDiv);
        return textContainerDiv;
    };
    TextPanel.prototype.onMoveToPage = function (pageIndex, scrollTo) {
        for (var j = 1; pageIndex - j >= 0 && j <= this.preloadPagesBefore; j++) {
            this.displayPage(this.parentReader.pages[pageIndex - j], false);
        }
        this.displayPage(this.parentReader.pages[pageIndex], scrollTo);
        for (var i = 1; pageIndex + i < this.parentReader.pages.length && i <= this.preloadPagesAfter; i++) {
            this.displayPage(this.parentReader.pages[pageIndex + i], false);
        }
    };
    TextPanel.prototype.displayPage = function (pageName, scrollTo) {
        var pageDiv = $(this.parentReader.readerContainer).find('div.reader-text').find('#page_' + pageName);
        var pageLoaded = $(pageDiv).data('loaded');
        var pageLoading = $(pageDiv).hasClass('loading');
        if ((typeof pageLoaded === 'undefined' || !pageLoaded) && !pageLoading) {
            this.downloadPageByName(pageName);
        }
        if (scrollTo) {
            this.scrollTextToPositionFromTop(0);
            var topOffset = $(pageDiv).offset().top;
            this.scrollTextToPositionFromTop(topOffset);
            if (typeof this.windowBody !== 'undefined') {
                $(this.windowBody).find(".reader-text-container").scrollTop(0);
                var pageToScrollOffset = $(this.windowBody).find('#page_' + pageName).offset().top;
                $(this.windowBody).find(".reader-text-container").scrollTop(pageToScrollOffset);
            }
        }
    };
    TextPanel.prototype.scrollTextToPositionFromTop = function (topOffset) {
        var scrollableContainer = $(this.innerContent);
        var containerTopOffset = $(scrollableContainer).offset().top;
        $(scrollableContainer).scrollTop(topOffset - containerTopOffset);
    };
    TextPanel.prototype.downloadPageByName = function (pageName) {
        var _this = this;
        var pageContainer = $(this.parentReader.readerContainer).find('div.reader-text').find('#page_' + pageName);
        $(pageContainer).addClass("loading");
        if (typeof this.windowBody !== 'undefined') {
            $(this.windowBody).find('#page_' + pageName).addClass("loading");
        }
        $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: this.parentReader.bookId, pageName: pageName },
            url: "/Reader/GetBookPageByName",
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                $(pageContainer).append(response["pageText"]);
                $(pageContainer).removeClass("loading");
                $(pageContainer).data('loaded', true);
                if (typeof _this.windowBody !== 'undefined') {
                    $(_this.windowBody).find('#page_' + pageName).removeClass("loading");
                    $(_this.windowBody).find('#page_' + pageName).append(response["pageText"]);
                }
            },
            error: function (response) {
                $(pageContainer).removeClass("loading");
            }
        });
    };
    return TextPanel;
})(RightSidePanel);
//# sourceMappingURL=itjakub.plugins.reader.js.map