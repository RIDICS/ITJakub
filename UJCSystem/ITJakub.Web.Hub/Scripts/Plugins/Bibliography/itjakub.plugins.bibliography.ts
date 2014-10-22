

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


    public showBooks(books: IBookInfo[], container: string) {
        $(container).empty();
        var rootElement: HTMLUListElement = document.createElement('ul');
        $(rootElement).addClass('listing');
        $.each(books, (index, book: IBookInfo) => {
            var bibliographyHtml = this.makeBibliography(book);
            rootElement.appendChild(bibliographyHtml);
        });
        $(container).append(rootElement);
    }

    private makeBibliography(bibItem: IBookInfo): HTMLLIElement {
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

class BibliographyFactory {
    configuration: ConfigurationManager;

    constructor(configuration) {
        this.configuration = configuration;
    }

    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');
        return leftPanel;
    }

    makeRightPanel(bookInfo: IBookInfo): HTMLDivElement {
        var rightPanel: HTMLDivElement = document.createElement('div');
        $(rightPanel).addClass('right-panel');
        return rightPanel;
    }

    makeMiddlePanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsMiddlePanel()) return null;

        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');


        if (this.configuration.containsTitle()) {
            var middlePanelHeading: HTMLDivElement = document.createElement('div');
            $(middlePanelHeading).addClass('heading');
            middlePanelHeading.innerHTML = this.configuration.getTitle(bookInfo);
            middlePanel.appendChild(middlePanelHeading);
        }
        if (this.configuration.containsBody()) {
            var middlePanelBody: HTMLDivElement = document.createElement('div');
            $(middlePanelBody).addClass('body');
            middlePanelBody.innerHTML = this.configuration.getBody(bookInfo);
            middlePanel.appendChild(middlePanelBody);
        }

        if (this.configuration.containsCustomInMiddlePanel()) {
            var customDiv: HTMLDivElement = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = this.configuration.getCustomInMiddlePanel(bookInfo);
            middlePanel.appendChild(customDiv);
        }

        return middlePanel;
    }

    makeBottomPanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsBottomPanel()) return null;

        var bottomPanel: HTMLDivElement = document.createElement('div');

        if (this.configuration.containsCustomInBottomPanel()) {
            var customDiv: HTMLDivElement = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = this.configuration.getCustomInMiddlePanel(bookInfo);
            bottomPanel.appendChild(customDiv);
        }

        return bottomPanel;
    }
}

class OldCzechTextBankFactory extends BibliographyFactory {

    constructor(configuration: ConfigurationManager) {
        super(configuration);
    }

    makeRightPanel(bookInfo: IBookInfo): HTMLDivElement {
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

    makeMiddlePanel(bookInfo: IBookInfo): HTMLDivElement {
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

    makeBottomPanel(bookInfo: IBookInfo): HTMLDivElement {
        var tableBuilder = new TableBuilder();
        tableBuilder.makeTableRow("Editor", bookInfo.Editor);
        tableBuilder.makeTableRow("Předloha", bookInfo.Pattern);
        tableBuilder.makeTableRow("Zkratka památky", bookInfo.RelicAbbreviation);
        tableBuilder.makeTableRow("Zkratka pramene", bookInfo.SourceAbbreviation);
        tableBuilder.makeTableRow("Literární druh", bookInfo.LiteraryType);
        tableBuilder.makeTableRow("Literární žánr", bookInfo.LiteraryGenre);
        tableBuilder.makeTableRow("Poslední úprava edice", bookInfo.LastEditation);
        return tableBuilder.build();
    }
}

class DictionaryFactory extends BibliographyFactory {

    constructor(configuration: ConfigurationManager) {
        super(configuration);
    }

    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');

        var inputCheckbox: HTMLInputElement = document.createElement('input');
        inputCheckbox.type = "checkbox";
        $(inputCheckbox).addClass('checkbox');
        leftPanel.appendChild(inputCheckbox);

        var starEmptyButton: HTMLButtonElement = document.createElement('button');
        starEmptyButton.type = 'button';
        $(starEmptyButton).addClass('btn btn-xs star-empty-button');
        var spanEmptyStar: HTMLSpanElement = document.createElement('span');
        $(spanEmptyStar).addClass('glyphicon glyphicon-star-empty');
        starEmptyButton.appendChild(spanEmptyStar);
        $(starEmptyButton).click(function(event) {
            $(this).siblings('.star-button').show();
            $(this).hide();
        }); //TODO fill click action
        leftPanel.appendChild(starEmptyButton);

        var starButton: HTMLButtonElement = document.createElement('button');
        starButton.type = 'button';
        $(starButton).addClass('btn btn-xs star-button');
        $(starButton).css('display', 'none');
        var spanStar: HTMLSpanElement = document.createElement('span');
        $(spanStar).addClass('glyphicon glyphicon-star');
        starButton.appendChild(spanStar);
        $(starButton).click(function(event) {
            $(this).siblings('.star-empty-button').show();
            $(this).hide();
        }); //TODO fill click action
        leftPanel.appendChild(starButton);

        return leftPanel;
    }

    makeRightPanel(bookInfo: IBookInfo): HTMLDivElement {
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

    makeMiddlePanel(bookInfo: IBookInfo): HTMLDivElement {
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
}

class EditionFactory extends BibliographyFactory {


    constructor(configuration: ConfigurationManager) {
        super(configuration);
    }

    makeRightPanel(bookInfo: IBookInfo): HTMLDivElement {
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


    //makeMiddlePanel(bookInfo: IBookInfo): HTMLDivElement {
    //    var middlePanel: HTMLDivElement = document.createElement('div');
    //    $(middlePanel).addClass('middle-panel');
    //    var middlePanelHeading: HTMLDivElement = document.createElement('div');
    //    $(middlePanelHeading).addClass('heading');
    //    middlePanelHeading.innerHTML = bookInfo.Name;
    //    middlePanel.appendChild(middlePanelHeading);
    //    var middlePanelBody: HTMLDivElement = document.createElement('div');
    //    $(middlePanelBody).addClass('body');
    //    middlePanelBody.innerHTML = bookInfo.Body;
    //    middlePanel.appendChild(middlePanelBody);
    //    return middlePanel;
    //}

    makeBottomPanel(bookInfo: IBookInfo): HTMLDivElement {
        var tableBuilder = new TableBuilder();
        tableBuilder.makeTableRow("Editor", bookInfo.Editor);
        tableBuilder.makeTableRow("Předloha", bookInfo.Pattern);
        tableBuilder.makeTableRow("Zkratka památky", bookInfo.RelicAbbreviation);
        tableBuilder.makeTableRow("Zkratka pramene", bookInfo.SourceAbbreviation);
        tableBuilder.makeTableRow("Literární druh", bookInfo.LiteraryType);
        tableBuilder.makeTableRow("Literární žánr", bookInfo.LiteraryGenre);
        tableBuilder.makeTableRow("Poslední úprava edice", bookInfo.LastEditation);

        //TODO add Edicni poznamka anchor and copyright to hiddenContent here

        return tableBuilder.build();
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
        var configObj;
        $.ajax({
            type: "GET",
            traditional: true,
            async: false,
            url: "/Bibliography/GetConfiguration",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                configObj = response;
            }
        });

        this.m_factories = new Array();
        this.m_factories['Edition'] = new EditionFactory(new ConfigurationManager(configObj["Edition"])); //TODO make enum bookType
        this.m_factories['Dictionary'] = new DictionaryFactory(new ConfigurationManager(configObj["Dictionary"]));
        this.m_factories['OldCzechTextBank'] = new OldCzechTextBankFactory(new ConfigurationManager(configObj["OldCzechTextBank"]));

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


class ConfigurationManager {
    config: Object;
    varInterpreter: VariableInterpreter;

    constructor(config: Object) {
        this.config = config;
        this.varInterpreter = new VariableInterpreter();
    }

    containsMiddlePanel() { return typeof this.config["middle-panel"] !== 'undefined'; }

    containsBottomPanel() { return typeof this.config["bottom-panel"] !== 'undefined'; }

    containsCustomInMiddlePanel() { return typeof this.config['middle-panel']['custom'] !== 'undefined'; }

    containsCustomInBottomPanel() { return typeof this.config['bottom-panel']['custom'] !== 'undefined'; }

    containsBody() { return typeof this.config["middle-panel"]['body'] !== 'undefined'; }

    containsTitle() { return typeof this.config["middle-panel"]['title'] !== 'undefined'; }

    getTitle(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['middle-panel']['title'], this.config['middle-panel']['variables'], bibItem); }

    getBody(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['middle-panel']['body'], this.config['middle-panel']['variables'], bibItem); }

    getCustomInMiddlePanel(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['middle-panel']['custom'], this.config['middle-panel']['variables'], bibItem); }

    getCustomInBottomPanel(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['bottom-panel']['custom'], this.config['bottom-panel']['variables'], bibItem); }
}

interface IBookInfo {
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
    Pages: Page[];

}

class Page {
    start: number;
    end: number;
}


class TableBuilder {
    private m_tableDiv: HTMLDivElement;

    constructor() {
        this.m_tableDiv = document.createElement('div');
        $(this.m_tableDiv).addClass('table');
    }

    public makeTableRow(label: string, value: string): void {
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
        this.m_tableDiv.appendChild(rowDiv);
    }

    public build(): HTMLDivElement {
        return this.m_tableDiv;
    }
}