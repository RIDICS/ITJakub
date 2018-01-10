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

        $(this.readerContainer).empty();
        var readerDiv: HTMLDivElement = document.createElement("div");
        $(readerDiv).addClass("reader");

        var readerHeadDiv: HTMLDivElement = document.createElement("div");
        $(readerHeadDiv).addClass("reader-head content-container");

        var fullscreenButton = document.createElement("button");
        $(fullscreenButton).addClass("fullscreen-button");

        var fullscreenSpan = document.createElement("span");
        $(fullscreenSpan).addClass("glyphicon glyphicon-fullscreen");
        $(fullscreenButton).append(fullscreenSpan);
        $(fullscreenButton).click((event) => {
            $(this.readerContainer).find(".reader").addClass("fullscreen");
        });
        readerHeadDiv.appendChild(fullscreenButton);

        var fullscreenCloseButton = document.createElement("button");
        $(fullscreenCloseButton).addClass("fullscreen-close-button");

        var closeSpan = document.createElement("span");
        $(closeSpan).addClass("glyphicon glyphicon-remove");
        $(fullscreenCloseButton).append(closeSpan);
        $(fullscreenCloseButton).click((event) => {
            $(this.readerContainer).find(".reader").removeClass("fullscreen");
        });
        readerHeadDiv.appendChild(fullscreenCloseButton);


        //var title = this.makeTitle(bookTitle);
        //readerHeadDiv.appendChild(title);


        //var controls = this.makeControls();
        //readerHeadDiv.appendChild(controls);
        //readerDiv.appendChild(readerHeadDiv);
        
        this.makeGoldenReader();
    }

    private makeGoldenReader() {
        var config = this.createConfig();
        //var bookText = this.getBookText();
        this.readerLayout = new GoldenLayout(config, $('#ReaderBodyDiv'));
        this.readerLayout.registerComponent('readerTab', function (container, state) {
            if (state.label === 'text') {
                //$(container.getElement()).html(this.getBookText());
            }
        });
        this.readerLayout.init();
    }

    private getBookText(): HTMLDivElement {
        var returnDiv: HTMLDivElement = document.createElement("div");
        var textPanel: TextPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.TextPanel) >= 0) {
            textPanel = this.appendTextPanel(returnDiv);
        }
        return returnDiv;
    }

    private createConfig() {
        var layoutConfig = {
            dimensions: {
                headerHeight: 26
            },
            content: [{
                type: 'row',
                isClosable: false,
                content: [{
                    type: 'stack',
                    width: 18,
                    content: [{
                        type: 'component',
                        id: 'bookmarks',
                        componentState: { label: 'bookmarks' },
                        componentName: 'readerTab',
                        title: 'Záložky'
                    }, {
                        type: 'component',
                        id: 'view',
                        componentState: { label: 'view' },
                        componentName: 'readerTab',
                        title: 'Zobrazení'
                    }, {
                        type: 'component',
                        id: 'search',
                        componentState: { label: 'search' },
                        componentName: 'readerTab',
                        title: 'Vyhledávání'
                    }]
                }, {
                    type: 'component',
                    id: 'text',
                    componentState: { label: 'text' },
                    componentName: 'readerTab',
                    isClosable: false,
                    title: 'Text'
                }, {
                    type: 'component',
                    id: 'img',
                    componentState: { label: 'img' },
                    componentName: 'readerTab',
                    title: 'Náhled'
                }]
            }]
        };
        return layoutConfig;
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
    var readerPlugin = new GoldenLayoutReader(<any>$("#ControlDiv")[0],
        readerPageChangedCallback,
        readerPanels,
        panelButtons,
        panelButtons);
    readerPlugin.initReader(bookXmlId, versionXmlId, bookTitle, pageList);
}