/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var ReaderModule = (function () {
    function ReaderModule(readerContainer) {
        this.readerContainer = readerContainer;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array();
        for (var i = 0; i < 15; i++) {
            this.pages.push(i.toString() + "r");
        }
    }
    ReaderModule.prototype.makeReader = function (book) {
        $(this.readerContainer).empty();
        var readerDiv = document.createElement('div');
        $(readerDiv).addClass('reader');

        var readerHeadDiv = document.createElement('div');
        $(readerHeadDiv).addClass('reader-head content-container');
        var title = this.makeTitle(book);
        readerHeadDiv.appendChild(title);

        var controls = this.makeControls(book);
        readerHeadDiv.appendChild(controls);
        readerDiv.appendChild(readerHeadDiv);

        var textArea = this.makeTextArea(book);
        readerDiv.appendChild(textArea);

        $(this.readerContainer).append(readerDiv);
    };

    ReaderModule.prototype.makeTitle = function (book) {
        var titleDiv = document.createElement('div');
        $(titleDiv).addClass('title');
        titleDiv.innerHTML = "Stitny ze stitneho, Tomas : [Stitensky sbornik klementinsky]";
        return titleDiv;
    };

    ReaderModule.prototype.makeControls = function (book) {
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
                _this.moveToPageNumber(ui.value);
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
            $(event.target).find('.slider-tip').show();
        });
        $(sliderHandle).mouseout(function (event) {
            $(event.target).find('.slider-tip').fadeOut(1000);
        });
        controlsDiv.appendChild(slider);

        var pagingDiv = document.createElement('div');
        $(pagingDiv).addClass('paging');

        var pageInputText = document.createElement("input");
        pageInputText.setAttribute("type", "text");
        pageInputText.setAttribute("id", "pageInputText");
        $(pageInputText).addClass('page-input-text');
        pagingDiv.appendChild(pageInputText);

        var pageInputButton = document.createElement("button");
        pageInputButton.innerHTML = "Přejít na stránku";
        $(pageInputButton).addClass('page-input-button');
        $(pageInputButton).click(function (event) {
            _this.moveToPage($('#pageInputText').val());
        });
        pagingDiv.appendChild(pageInputButton);

        var paginationUl = document.createElement('ul');
        $(paginationUl).addClass('pagination pagination-sm');

        var liElement = document.createElement('li');
        var anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '|<';
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<<';
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<';
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '1r';
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '2r';
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>';
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>>';
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>|';
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
        $(commentSpan).addClass('glyphicon glyphicon-comment');
        $(commentButton).append(commentSpan);

        var commentSpanText = document.createElement("span");
        $(commentSpanText).addClass('button-text');
        $(commentSpanText).append("Zobrazit komentář");
        $(commentButton).append(commentSpanText);

        $(commentButton).click(function (event) {
        });

        buttonsDiv.appendChild(commentButton);

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
        });

        buttonsDiv.appendChild(contentButton);

        pagingDiv.appendChild(buttonsDiv);

        controlsDiv.appendChild(pagingDiv);
        return controlsDiv;
    };

    ReaderModule.prototype.makeTextArea = function (book) {
        var textAreaDiv = document.createElement('div');
        $(textAreaDiv).addClass('reader-text');
        return textAreaDiv;
    };

    ReaderModule.prototype.moveToPageNumber = function (pageIndex) {
        this.actualPageIndex = pageIndex;
        this.displayPage(this.pages[pageIndex]);
    };

    ReaderModule.prototype.moveToPage = function (page) {
        var pageIndex = $.inArray(page, this.pages);
        if (pageIndex >= 0) {
            this.actualizeSlider(pageIndex);
            this.moveToPageNumber(pageIndex);
        } else {
            //TODO tell user page not exist
        }
    };

    ReaderModule.prototype.actualizeSlider = function (pageIndex) {
        var slider = $(this.readerContainer).find('.slider');
        $(slider).slider().slider('value', pageIndex);
        $(slider).find('.ui-slider-handle').find('.tooltip-inner').html("Strana: " + this.pages[pageIndex]);
    };

    ReaderModule.prototype.displayPage = function (page) {
        $(this.readerContainer).find('div.reader-text').empty();
        $(this.readerContainer).find('div.reader-text').append(page);
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
    return ReaderModule;
})();
//# sourceMappingURL=itjakub.plugins.reader.js.map
