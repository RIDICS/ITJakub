/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var ReaderModule = (function () {
    function ReaderModule(readerContainer) {
        this.readerContainer = readerContainer;
        this.pagerDisplayPages = 5;
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

        this.moveToPageNumber(0); //load first page
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

        var fullscreenButton = document.createElement("button");
        $(fullscreenButton).addClass('fullscreen-button');

        var fullscreenSpan = document.createElement("span");
        $(fullscreenSpan).addClass('glyphicon glyphicon-fullscreen');
        $(fullscreenButton).append(fullscreenSpan);
        $(fullscreenButton).click(function (event) {
            //TODO change class to reader for absolute or fixed positioning and overlay other elements
        });
        controlsDiv.appendChild(fullscreenButton);

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
                var pp = _this.actualPageIndex;
                if (_this.actualPageIndex !== ui.value) {
                    _this.moveToPageNumber(ui.value);
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
            _this.moveToPage($('#pageInputText').val());
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
            _this.moveToPageNumber(0);
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
            _this.moveToPageNumber(_this.actualPageIndex - 5);
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
            _this.moveToPageNumber(_this.actualPageIndex - 1);
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
                _this.moveToPage(page);
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
            _this.moveToPageNumber(_this.actualPageIndex + 1);
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
            _this.moveToPageNumber(_this.actualPageIndex + 5);
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
            _this.moveToPageNumber(_this.pages.length - 1);
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
        var textContainerDiv = document.createElement('div');
        $(textContainerDiv).addClass('reader-text-container content-container');

        var textAreaDiv = document.createElement('div');
        $(textAreaDiv).addClass('reader-text');

        textContainerDiv.appendChild(textAreaDiv);
        return textContainerDiv;
    };

    ReaderModule.prototype.moveToPageNumber = function (pageIndex) {
        if (pageIndex < 0) {
            pageIndex = 0;
        } else if (pageIndex >= this.pages.length) {
            pageIndex = this.pages.length - 1;
        }
        this.actualPageIndex = pageIndex;
        this.actualizeSlider(pageIndex);
        this.actualizePagination(pageIndex);
        this.displayPage(this.pages[pageIndex]);
    };

    ReaderModule.prototype.moveToPage = function (page) {
        var pageIndex = $.inArray(page, this.pages);
        if (pageIndex >= 0 && pageIndex < this.pages.length) {
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

    ReaderModule.prototype.actualizePagination = function (pageIndex) {
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
        } else if (pagesOnRight <= displayOnRight) {
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

    ReaderModule.prototype.displayPage = function (page) {
        $(this.readerContainer).find('div.reader-text').empty();

        //TODO load page content here
        $(this.readerContainer).find('div.reader-text').append(page + "<br>");
        $(this.readerContainer).find('div.reader-text').append("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce at varius felis. Praesent scelerisque elit ac felis faucibus, sit amet condimentum sem ullamcorper. Vestibulum in bibendum turpis. Aenean a tempor nisl, in auctor mi. Donec aliquam, ex vestibulum pulvinar imperdiet, turpis lectus placerat massa, ut finibus sapien lectus nec erat. Vestibulum nibh ante, congue molestie dolor at, sodales mollis ligula. Nullam tempus dictum iaculis. Nam quam enim, vehicula nec sapien eu, egestas fringilla purus. Nulla nec lectus nec mi eleifend mollis non et velit. Integer volutpat, ex eu imperdiet suscipit, mauris nunc convallis mi, nec aliquet urna massa ut ante. Mauris leo justo, convallis ut sagittis vel, sagittis et massa. Nam vitae erat at dolor mollis consequat. Vestibulum vel leo non diam consectetur aliquet at vel tellus. Etiam semper sapien nec accumsan vulputate.Mauris rutrum metus dignissim, eleifend risus vel, bibendum ante.Nulla fringilla odio ac vulputate eleifend.Sed dapibus accumsan nunc.Duis ullamcorper sapien eget urna blandit scelerisque sit amet in ligula.Interdum et malesuada fames ac ante ipsum primis in faucibus.Integer nec tempus sapien, ac fringilla orci.Donec lobortis massa sit amet orci imperdiet eleifend.Cras tristique mi id justo vulputate iaculis.Sed porttitor gravida diam, vitae pulvinar lorem scelerisque at.Pellentesque sit amet cursus lorem.Maecenas commodo ornare est, vel sollicitudin felis condimentum ac.Quisque ac luctus lacus, quis tempus libero.Morbi leo arcu, finibus sed sodales eu, mattis non nibh.Cras lobortis laoreet mauris sed gravida.Nullam pellentesque elementum vulputate.Integer condimentum eros id eleifend posuere.Nam id turpis non purus consequat interdum.Maecenas fermentum bibendum nisl, quis mollis nibh semper at.Aenean sed semper tellus.In a libero at magna suscipit luctus ut eget eros.Cras a gravida felis, ut tempor augue.Vivamus eget mauris a ex blandit consectetur.Donec lobortis augue felis, quis malesuada orci luctus a.Ut ac quam ac massa vehicula fermentum eu eget lectus.Ut ac quam gravida urna ornare fermentum eget sed augue.Mauris dictum justo a condimentum gravida.Cras ac nulla id erat fermentum sodales ut eu turpis.Nunc ullamcorper eros vitae odio efficitur rutrum.Curabitur fringilla ex id nunc sodales imperdiet.Cras porta arcu ut dolor euismod pretium.Nulla mattis justo ac feugiat mollis.Proin at tortor ut justo egestas ultricies quis nec risus.Nulla facilisi.Nulla eu enim ut lorem aliquam maximus.Fusce suscipit odio quis lorem ultricies faucibus.");
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
