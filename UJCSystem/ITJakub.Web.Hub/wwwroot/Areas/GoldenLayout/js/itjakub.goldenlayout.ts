///<reference path="../../../lib/golden-layout/index.d.ts"/>
//import GoldenLayout = require("golden-layout");
//declare var GoldenLayout;

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
                var bookmarksItemConfig = {
                    type: 'component',
                    id: 'bookmarks',
                    componentState: { label: 'Bookmarks' },
                    componentName: 'readerTab',
                    title: 'Záložky'
                };
                this.readerLayout.root.getItemsById('tools')[0].addChild(bookmarksItemConfig);    
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
        
        this.readerLayout.on("stateChanged", function() {
            $(".reader-text-container").scroll();
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

    var readerPanels = [
        ReaderPanelEnum.TextPanel, ReaderPanelEnum.ImagePanel, ReaderPanelEnum.ContentPanel,
        ReaderPanelEnum.SearchPanel, ReaderPanelEnum.SettingsPanel
    ];
    var panelButtons = [PanelButtonEnum.Close, PanelButtonEnum.Pin, PanelButtonEnum.ToNewWindow];
    var readerPlugin = new GoldenLayoutReader(<any>$("#ReaderHeaderDiv")[0],
        readerPageChangedCallback,
        readerPanels,
        panelButtons,
        panelButtons);
    readerPlugin.initReader(bookXmlId, versionXmlId, bookTitle, pageList);
}
