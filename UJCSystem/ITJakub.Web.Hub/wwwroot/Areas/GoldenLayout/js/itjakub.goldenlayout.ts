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
        this.makeGoldenReader();
        document.getElementById("BookText").appendChild(this.getBookText());
        document.getElementById("BookContent").appendChild(this.getBookContent());
        $(document).ready(function() {
            $(".reader-text-container").scroll();
        });
    }

    private makeGoldenReader() {
        var config = this.createConfig();
        this.readerLayout = new GoldenLayout(config, $('#ReaderBodyDiv'));
        this.readerLayout.registerComponent('readerTab', function (container, state) {
           
            $(container.getElement()).html("<div id='Book"+state.label+ "'></div>");
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

    private getBookContent(): HTMLDivElement {
        var returnDiv: HTMLDivElement = document.createElement("div");
        var contentPanel: ContentPanel = null;
        if (this.showPanelList.indexOf(ReaderPanelEnum.ContentPanel) >= 0) {
            contentPanel = this.appendContentPanel(returnDiv);
        }
        return returnDiv;
    }

    private createConfig() {
        var layoutConfig = {
            settings: {
                selectionEnabled: true
            },
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
                        id: 'content',
                        componentState: { label: 'Content' },
                        componentName: 'readerTab',
                        title: 'Obsah'
                    }, {
                        type: 'component',
                        id: 'bookmarks',
                        componentState: { label: 'Bookmarks' },
                        componentName: 'readerTab',
                        title: 'Záložky'
                    }, {
                        type: 'component',
                        id: 'view',
                        componentState: { label: 'View' },
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
