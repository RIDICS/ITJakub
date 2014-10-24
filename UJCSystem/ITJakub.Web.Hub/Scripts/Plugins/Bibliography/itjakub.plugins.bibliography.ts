/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />

class BibliographyModule {

    booksContainer: string;
    sortBarContainer: string

    constructor(booksContainer: string, sortBarContainer: string) {
        this.booksContainer = booksContainer;
        this.sortBarContainer = sortBarContainer;
    }

    public showBooks(books: IBookInfo[]) {
        $(this.booksContainer).empty();
        var rootElement: HTMLUListElement = document.createElement('ul');
        $(rootElement).addClass('bib-listing');
        $.each(books, (index, book: IBookInfo) => {
            var bibliographyHtml = this.makeBibliography(book);
            rootElement.appendChild(bibliographyHtml);
        });
        $(this.booksContainer).append(rootElement);
        $(this.sortBarContainer).empty();
        var sortBarHtml = this.makeSortBar();
        $(this.sortBarContainer).append(sortBarHtml);
    }

    private makeSortBar(): HTMLDivElement {
        var sortBarDiv: HTMLDivElement = document.createElement('div');
        $(sortBarDiv).addClass('bib-sortbar');
        var select: HTMLSelectElement = document.createElement('select');
        $(select).change(() => {
            var selectedOption = $(this.sortBarContainer).find('div.bib-sortbar').find('select').find("option:selected");
            var value = $(selectedOption).val();
            alert(value);
        });
        this.addOption(select, "Název", "Name");
        this.addOption(select, "Autor", "Author");
        this.addOption(select, "Datace", "Date"); //TODO add options to json config
        sortBarDiv.appendChild(select);
        return sortBarDiv;
    }

    private addOption(selectbox: HTMLSelectElement, text: string, value: string) {
        var option: HTMLOptionElement = document.createElement('option');
        option.text = text;
        option.value = value;
        selectbox.appendChild(option);
    }

    private makeBibliography(bibItem: IBookInfo): HTMLLIElement {
        var liElement: HTMLLIElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookId", bibItem.BookId);
        $(liElement).attr("data-bookType", bibItem.BookType);

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

    containsMiddlePanelBody() { return typeof this.config["middle-panel"]['body'] !== 'undefined'; }

    containsBottomPanelBody() { return typeof this.config["bottom-panel"]['body'] !== 'undefined'; }

    containsMiddlePanelTitle() { return typeof this.config["middle-panel"]['title'] !== 'undefined'; }

    getTitle(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['middle-panel']['title'], this.config['middle-panel']['variables'], bibItem); }

    getMiddlePanelBody(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['middle-panel']['body'], this.config['middle-panel']['variables'], bibItem); }

    getBottomPanelBody(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['bottom-panel']['body'], this.config['bottom-panel']['variables'], bibItem); }

    getCustomInMiddlePanel(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['middle-panel']['custom'], this.config['middle-panel']['variables'], bibItem); }

    getCustomInBottomPanel(bibItem: IBookInfo): string { return this.varInterpreter.interpret(this.config['bottom-panel']['custom'], this.config['bottom-panel']['variables'], bibItem); }

}

interface IBookInfo {
    BookId: string;
    BookType: string;
    Name: string;
    Editor: string;
    Pattern: string;
    SourceAbbreviation: string;
    RelicAbbreviation: string;
    LiteraryType: string;
    LiteraryGenre: string;
    LastEditation: string;
    EditationNote: string; //anchor href?
    Copyright: string;
    Pages: IPage[];
    Archive: IArchive;
    Century: number;
    Sign: string;
    Authors: IAuthor[];
    Description: string;
    Year: number;
}

interface IPage {
    Start: number;
    End: number;
}

interface IArchive {
    Name: string;
    City: string;
    State: string;
}

interface IAuthor {
    FirstName: string;
    LastName: string;
}