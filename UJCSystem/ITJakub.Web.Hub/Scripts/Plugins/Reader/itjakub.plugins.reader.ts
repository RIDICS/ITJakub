/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />


class ReaderModule {

    readerContainer: string;
    sliderOnPage: number;
    actualPageIndex: number;
    pages: Array<string>;
    pagerDisplayPages: number;

    constructor(readerContainer: string) {
        this.readerContainer = readerContainer;
        this.pagerDisplayPages = 5;
        this.actualPageIndex = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<string>();
        for (var i = 0; i < 15; i++) { //TODO pages should be loaded by ajax
            this.pages.push(i.toString() + "r");
        }
    }

    public makeReader(book: IBookInfo) {
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

        var textArea = this.makeTextArea(book);
        readerDiv.appendChild(textArea);

        $(this.readerContainer).append(readerDiv);

        this.moveToPageNumber(0); //load first page
    }

    private makeTitle(book: IBookInfo): HTMLDivElement {
        var titleDiv: HTMLDivElement = document.createElement('div');
        $(titleDiv).addClass('title');
        titleDiv.innerHTML = "Stitny ze stitneho, Tomas : [Stitensky sbornik klementinsky]";
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
                var pp = this.actualPageIndex;
                if (this.actualPageIndex !== ui.value) {
                    this.moveToPageNumber(ui.value);
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
            this.moveToPage($('#pageInputText').val());
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
            this.moveToPageNumber(0);
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
            this.moveToPageNumber(this.actualPageIndex - 5);
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
            this.moveToPageNumber(this.actualPageIndex - 1);
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
                this.moveToPage(page);
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
            this.moveToPageNumber(this.actualPageIndex + 1);
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
            this.moveToPageNumber(this.actualPageIndex + 5);
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
            this.moveToPageNumber(this.pages.length - 1);
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
        $(commentSpan).addClass('glyphicon glyphicon-comment');
        $(commentButton).append(commentSpan);

        var commentSpanText = document.createElement("span");
        $(commentSpanText).addClass('button-text');
        $(commentSpanText).append("Zobrazit komentář");
        $(commentButton).append(commentSpanText);

        $(commentButton).click((event: Event) => {
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

        $(contentButton).click((event: Event) => {
        });

        buttonsDiv.appendChild(contentButton);

        pagingDiv.appendChild(buttonsDiv);

        controlsDiv.appendChild(pagingDiv);
        return controlsDiv;
    }

    private makeTextArea(book: IBookInfo): HTMLDivElement {
        var textContainerDiv: HTMLDivElement = document.createElement('div');
        $(textContainerDiv).addClass('reader-text-container content-container');


        var textAreaDiv: HTMLDivElement = document.createElement('div');
        $(textAreaDiv).addClass('reader-text');

        textContainerDiv.appendChild(textAreaDiv);
        return textContainerDiv;
    }

    moveToPageNumber(pageIndex: number) {
        if (pageIndex < 0) {
            pageIndex = 0;
        } else if (pageIndex >= this.pages.length) {
            pageIndex = this.pages.length - 1;
        }
        this.actualPageIndex = pageIndex;
        this.actualizeSlider(pageIndex);
        this.actualizePagination(pageIndex);
        this.displayPage(this.pages[pageIndex]);
    }

    moveToPage(page: string) {
        var pageIndex: number = $.inArray(page, this.pages);
        if (pageIndex >= 0 && pageIndex < this.pages.length) {
            this.moveToPageNumber(pageIndex);
        } else {
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

    displayPage(page: string) {
        $(this.readerContainer).find('div.reader-text').empty();
        //TODO load page content here
        $(this.readerContainer).find('div.reader-text').append(page + "<br>");
        $(this.readerContainer).find('div.reader-text').append("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce at varius felis. Praesent scelerisque elit ac felis faucibus, sit amet condimentum sem ullamcorper. Vestibulum in bibendum turpis. Aenean a tempor nisl, in auctor mi. Donec aliquam, ex vestibulum pulvinar imperdiet, turpis lectus placerat massa, ut finibus sapien lectus nec erat. Vestibulum nibh ante, congue molestie dolor at, sodales mollis ligula. Nullam tempus dictum iaculis. Nam quam enim, vehicula nec sapien eu, egestas fringilla purus. Nulla nec lectus nec mi eleifend mollis non et velit. Integer volutpat, ex eu imperdiet suscipit, mauris nunc convallis mi, nec aliquet urna massa ut ante. Mauris leo justo, convallis ut sagittis vel, sagittis et massa. Nam vitae erat at dolor mollis consequat. Vestibulum vel leo non diam consectetur aliquet at vel tellus. Etiam semper sapien nec accumsan vulputate.Mauris rutrum metus dignissim, eleifend risus vel, bibendum ante.Nulla fringilla odio ac vulputate eleifend.Sed dapibus accumsan nunc.Duis ullamcorper sapien eget urna blandit scelerisque sit amet in ligula.Interdum et malesuada fames ac ante ipsum primis in faucibus.Integer nec tempus sapien, ac fringilla orci.Donec lobortis massa sit amet orci imperdiet eleifend.Cras tristique mi id justo vulputate iaculis.Sed porttitor gravida diam, vitae pulvinar lorem scelerisque at.Pellentesque sit amet cursus lorem.Maecenas commodo ornare est, vel sollicitudin felis condimentum ac.Quisque ac luctus lacus, quis tempus libero.Morbi leo arcu, finibus sed sodales eu, mattis non nibh.Cras lobortis laoreet mauris sed gravida.Nullam pellentesque elementum vulputate.Integer condimentum eros id eleifend posuere.Nam id turpis non purus consequat interdum.Maecenas fermentum bibendum nisl, quis mollis nibh semper at.Aenean sed semper tellus.In a libero at magna suscipit luctus ut eget eros.Cras a gravida felis, ut tempor augue.Vivamus eget mauris a ex blandit consectetur.Donec lobortis augue felis, quis malesuada orci luctus a.Ut ac quam ac massa vehicula fermentum eu eget lectus.Ut ac quam gravida urna ornare fermentum eget sed augue.Mauris dictum justo a condimentum gravida.Cras ac nulla id erat fermentum sodales ut eu turpis.Nunc ullamcorper eros vitae odio efficitur rutrum.Curabitur fringilla ex id nunc sodales imperdiet.Cras porta arcu ut dolor euismod pretium.Nulla mattis justo ac feugiat mollis.Proin at tortor ut justo egestas ultricies quis nec risus.Nulla facilisi.Nulla eu enim ut lorem aliquam maximus.Fusce suscipit odio quis lorem ultricies faucibus.");
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