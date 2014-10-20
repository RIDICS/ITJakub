
class BibliographyModule {
    private static _instance: BibliographyModule = null;
    bibliographyModulControllerUrl: string = "";

    constructor() {
        if (BibliographyModule._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        BibliographyModule._instance = this;
    }

    public static getInstance(): BibliographyModule {
        if (BibliographyModule._instance === null) {
            BibliographyModule._instance = new BibliographyModule();
        }
        return BibliographyModule._instance;
    }


    public showBooks(books: BookInfo[], container: string) {
        $(container).empty();
        var rootElement: HTMLUListElement = document.createElement('ul');
        $(rootElement).addClass('listing');
        $.each(books, (index, book: BookInfo) => {
            var bibliographyHtml = this.makeBibliography(book);
            rootElement.appendChild(bibliographyHtml);
        });
        $(container).append(rootElement);
    }

    private makeBibliography(bibItem: BookInfo): HTMLLIElement {
        var liElement: HTMLLIElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookId", bibItem.BookId);

        var visibleContent: HTMLDivElement = document.createElement('div');
        $(visibleContent).addClass('visible-content');

        var bibFactory: BibliographyFactory = BibliographyFactoryResolver.getInstance().getFactoryForType(bibItem.BookType);

        var panel = bibFactory.makeLeftPanel(bibItem);
        if (panel != null) visibleContent.appendChild(panel);

        panel = bibFactory.makeRightPanel(bibItem);
        if (panel != null) visibleContent.appendChild(panel);

        panel = bibFactory.makeMiddlePanel(bibItem);
        if (panel != null) visibleContent.appendChild(panel);

        $(liElement).append(visibleContent);

        var hiddenContent: HTMLDivElement = document.createElement('div');
        $(hiddenContent).addClass('hidden-content');

        panel = bibFactory.makeBottomPanel(bibItem);
        if (panel != null) hiddenContent.appendChild(panel);

        $(liElement).append(hiddenContent);

        return liElement;

    }
}

class OldCzechTextBankFactory implements BibliographyFactory {
    makeLeftPanel(bookInfo: BookInfo): HTMLDivElement {
        return null;
    }

    makeRightPanel(bookInfo: BookInfo): HTMLDivElement {
        var rightPanel: HTMLDivElement = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var infoButton: HTMLButtonElement = document.createElement('button');
        infoButton.type = 'button';
        $(infoButton).addClass('btn btn-sm information-button');
        var spanInfo: HTMLSpanElement = document.createElement('span');
        $(spanInfo).addClass('glyphicon glyphicon-info-sign');
        infoButton.appendChild(spanInfo);
        $(infoButton).click((event) => {

        }); //TODO fill click action
        rightPanel.appendChild(infoButton);

        var showContentButton: HTMLButtonElement = document.createElement('button');
        showContentButton.type = 'button';
        $(showContentButton).addClass('btn btn-sm show-button');
        var spanChevrDown: HTMLSpanElement = document.createElement('span');
        $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
        showContentButton.appendChild(spanChevrDown);
        $(showContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').show("slow");
            $(this).siblings('.hide-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(showContentButton);

        var hideContentButton: HTMLButtonElement = document.createElement('button');
        hideContentButton.type = 'button';
        $(hideContentButton).addClass('btn btn-sm hide-button');
        var spanChevrUp: HTMLSpanElement = document.createElement('span');
        $(spanChevrUp).addClass('glyphicon glyphicon-chevron-up');
        hideContentButton.appendChild(spanChevrUp);
        $(hideContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').hide("slow");
            $(this).siblings('.show-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(hideContentButton);

        return rightPanel;
    }

    makeMiddlePanel(bookInfo: BookInfo): HTMLDivElement {
        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        var middlePanelHeading: HTMLDivElement = document.createElement('div');
        $(middlePanelHeading).addClass('heading');
        middlePanelHeading.innerHTML = bookInfo.Name;
        middlePanel.appendChild(middlePanelHeading);
        var middlePanelBody: HTMLDivElement = document.createElement('div');
        $(middlePanelBody).addClass('body');
        middlePanelBody.innerHTML = bookInfo.Body;
        middlePanel.appendChild(middlePanelBody);
        return middlePanel;
    }

    makeBottomPanel(bookInfo: BookInfo): HTMLDivElement {
        var tableDiv: HTMLDivElement = document.createElement('div');
        $(tableDiv).addClass('table');
        this.appendTableRow("Editor", bookInfo.Editor, tableDiv);
        this.appendTableRow("Předloha", bookInfo.Pattern, tableDiv);
        this.appendTableRow("Zkratka památky", bookInfo.RelicAbbreviation, tableDiv);
        this.appendTableRow("Zkratka pramene", bookInfo.SourceAbbreviation, tableDiv);
        this.appendTableRow("Literární druh", bookInfo.LiteraryType, tableDiv);
        this.appendTableRow("Literární žánr", bookInfo.LiteraryGenre, tableDiv);
        this.appendTableRow("Poslední úprava edice", bookInfo.LastEditation, tableDiv);

        //TODO add Edicni poznamka anchor and copyright to hiddenContent here

        return tableDiv;
    }

    //TODO make makeTable Helper or table builder
    private appendTableRow(label: string, value: string, tableDiv: HTMLDivElement) {
        var rowDiv: HTMLDivElement = document.createElement('div');
        $(rowDiv).addClass('row');
        var labelDiv: HTMLDivElement = document.createElement('div');
        $(labelDiv).addClass('cell label');
        labelDiv.innerHTML = label;
        rowDiv.appendChild(labelDiv);
        var valueDiv: HTMLDivElement = document.createElement('div');
        $(valueDiv).addClass('cell');
        if (!value || value.length === 0) {
            valueDiv.innerHTML = "&lt;nezadáno&gt;";
        } else {
            valueDiv.innerHTML = value;
        }
        rowDiv.appendChild(valueDiv);
        tableDiv.appendChild(rowDiv);
    }
}

class DictionaryFactory implements BibliographyFactory {
    makeLeftPanel(bookInfo: BookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');
        leftPanel.innerHTML = "checkbox and star"; //TODO
        return leftPanel;
    }

    makeRightPanel(bookInfo: BookInfo): HTMLDivElement {
        var rightPanel: HTMLDivElement = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var bookButton: HTMLButtonElement = document.createElement('button');
        bookButton.type = 'button';
        $(bookButton).addClass('btn btn-sm book-button');
        var spanBook: HTMLSpanElement = document.createElement('span');
        $(spanBook).addClass('glyphicon glyphicon-book');
        bookButton.appendChild(spanBook);
        $(bookButton).click((event) => {

        }); //TODO fill click action
        rightPanel.appendChild(bookButton);

        var infoButton: HTMLButtonElement = document.createElement('button');
        infoButton.type = 'button';
        $(infoButton).addClass('btn btn-sm information-button');
        var spanInfo: HTMLSpanElement = document.createElement('span');
        $(spanInfo).addClass('glyphicon glyphicon-info-sign');
        infoButton.appendChild(spanInfo);
        $(infoButton).click((event) => {

        }); //TODO fill click action
        rightPanel.appendChild(infoButton);

        return rightPanel;
    }

    makeMiddlePanel(bookInfo: BookInfo): HTMLDivElement {
        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        var middlePanelHeading: HTMLDivElement = document.createElement('div');
        $(middlePanelHeading).addClass('heading');
        middlePanelHeading.innerHTML = bookInfo.Name;
        middlePanel.appendChild(middlePanelHeading);
        var middlePanelBody: HTMLDivElement = document.createElement('div');
        $(middlePanelBody).addClass('body');
        middlePanelBody.innerHTML = bookInfo.Body;
        middlePanel.appendChild(middlePanelBody);
        return middlePanel;
    }

    makeBottomPanel(bookInfo: BookInfo): HTMLDivElement {
        return null;
    }
}

class EditionFactory implements BibliographyFactory {

    makeLeftPanel(bookInfo: BookInfo): HTMLDivElement {
        return null;
    }

    makeRightPanel(bookInfo: BookInfo): HTMLDivElement {
        var rightPanel: HTMLDivElement = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var bookButton: HTMLButtonElement = document.createElement('button');
        bookButton.type = 'button';
        $(bookButton).addClass('btn btn-sm book-button');
        var spanBook: HTMLSpanElement = document.createElement('span');
        $(spanBook).addClass('glyphicon glyphicon-book');
        bookButton.appendChild(spanBook);
        $(bookButton).click((event) => {

        }); //TODO fill click action
        rightPanel.appendChild(bookButton);

        var infoButton: HTMLButtonElement = document.createElement('button');
        infoButton.type = 'button';
        $(infoButton).addClass('btn btn-sm information-button');
        var spanInfo: HTMLSpanElement = document.createElement('span');
        $(spanInfo).addClass('glyphicon glyphicon-info-sign');
        infoButton.appendChild(spanInfo);
        $(infoButton).click((event) => {

        }); //TODO fill click action
        rightPanel.appendChild(infoButton);

        var showContentButton: HTMLButtonElement = document.createElement('button');
        showContentButton.type = 'button';
        $(showContentButton).addClass('btn btn-sm show-button');
        var spanChevrDown: HTMLSpanElement = document.createElement('span');
        $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
        showContentButton.appendChild(spanChevrDown);
        $(showContentButton).click(function(event) {
            $(this).parents('li.list-item').first().find('.hidden-content').show("slow");
            $(this).siblings('.hide-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(showContentButton);

        var hideContentButton: HTMLButtonElement = document.createElement('button');
        hideContentButton.type = 'button';
        $(hideContentButton).addClass('btn btn-sm hide-button');
        var spanChevrUp: HTMLSpanElement = document.createElement('span');
        $(spanChevrUp).addClass('glyphicon glyphicon-chevron-up');
        hideContentButton.appendChild(spanChevrUp);
        $(hideContentButton).click(function(event) {
            $(this).parents('li.list-item').first().find('.hidden-content').hide("slow");
            $(this).siblings('.show-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(hideContentButton);

        return rightPanel;
    }


    makeMiddlePanel(bookInfo: BookInfo): HTMLDivElement {
        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        var middlePanelHeading: HTMLDivElement = document.createElement('div');
        $(middlePanelHeading).addClass('heading');
        middlePanelHeading.innerHTML = bookInfo.Name;
        middlePanel.appendChild(middlePanelHeading);
        var middlePanelBody: HTMLDivElement = document.createElement('div');
        $(middlePanelBody).addClass('body');
        middlePanelBody.innerHTML = bookInfo.Body;
        middlePanel.appendChild(middlePanelBody);
        return middlePanel;
    }

    makeBottomPanel(bookInfo: BookInfo): HTMLDivElement {
        var tableDiv: HTMLDivElement = document.createElement('div');
        $(tableDiv).addClass('table');
        this.appendTableRow("Editor", bookInfo.Editor, tableDiv);
        this.appendTableRow("Předloha", bookInfo.Pattern, tableDiv);
        this.appendTableRow("Zkratka památky", bookInfo.RelicAbbreviation, tableDiv);
        this.appendTableRow("Zkratka pramene", bookInfo.SourceAbbreviation, tableDiv);
        this.appendTableRow("Literární druh", bookInfo.LiteraryType, tableDiv);
        this.appendTableRow("Literární žánr", bookInfo.LiteraryGenre, tableDiv);
        this.appendTableRow("Poslední úprava edice", bookInfo.LastEditation, tableDiv);

        //TODO add Edicni poznamka anchor and copyright to hiddenContent here

        return tableDiv;
    }

    //TODO make makeTable Helper
    private appendTableRow(label: string, value: string, tableDiv: HTMLDivElement) {
        var rowDiv: HTMLDivElement = document.createElement('div');
        $(rowDiv).addClass('row');
        var labelDiv: HTMLDivElement = document.createElement('div');
        $(labelDiv).addClass('cell label');
        labelDiv.innerHTML = label;
        rowDiv.appendChild(labelDiv);
        var valueDiv: HTMLDivElement = document.createElement('div');
        $(valueDiv).addClass('cell');
        if (!value || value.length === 0) {
            valueDiv.innerHTML = "&lt;nezadáno&gt;";
        } else {
            valueDiv.innerHTML = value;
        }
        rowDiv.appendChild(valueDiv);
        tableDiv.appendChild(rowDiv);
    }
}

class BibliographyFactoryResolver {
    private static _instance: BibliographyFactoryResolver = null;
    private m_factories: BibliographyFactory[];


    constructor() {
        if (BibliographyFactoryResolver._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        BibliographyFactoryResolver._instance = this;
        this.m_factories = new Array();
        this.m_factories['Edition'] = new EditionFactory(); //TODO make enum bookType
        this.m_factories['Dictionary'] = new DictionaryFactory();
        this.m_factories['OldCzechTextBank'] = new OldCzechTextBankFactory();

    }

    public static getInstance(): BibliographyFactoryResolver {
        if (BibliographyFactoryResolver._instance === null) {
            BibliographyFactoryResolver._instance = new BibliographyFactoryResolver();
        }
        return BibliographyFactoryResolver._instance;
    }

    public getFactoryForType(bookType: string): BibliographyFactory {
        return this.m_factories[bookType];
    }


}

interface BibliographyFactory {

    makeLeftPanel(bookInfo: BookInfo): HTMLDivElement;
    makeRightPanel(bookInfo: BookInfo): HTMLDivElement;
    makeMiddlePanel(bookInfo: BookInfo): HTMLDivElement;
    makeBottomPanel(bookInfo: BookInfo): HTMLDivElement;
}

interface BookInfo {
    BookId: string;
    BookType: string;
    Name: string;
    Body: string;
    Editor: string;
    Pattern: string;
    SourceAbbreviation: string;
    RelicAbbreviation: string;
    LiteraryType: string;
    LiteraryGenre: string;
    LastEditation: string;
    EditationNote: string; //anchor href?
    Copyright: string;

}