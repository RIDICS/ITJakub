/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />
/// <reference path="itjakub.plugins.bibliography.configuration.ts" />

class BibliographyModule {

    booksContainer: string;
    sortBarContainer: string
    bibliographyFactoryResolver: BibliographyFactoryResolver;
    configurationManager : ConfigurationManager;

    constructor(booksContainer: string, sortBarContainer: string) {
        this.booksContainer = booksContainer;
        this.sortBarContainer = sortBarContainer;

        //Download configuration
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
        this.configurationManager = new ConfigurationManager(configObj);
        this.bibliographyFactoryResolver = new BibliographyFactoryResolver(this.configurationManager.getBookTypeConfigurations());
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
        var sortBarHtml = new SortBar().makeSortBar(this.booksContainer, this.sortBarContainer);
        $(this.sortBarContainer).append(sortBarHtml);
    }

    private makeBibliography(bibItem: IBookInfo): HTMLLIElement {
        var liElement: HTMLLIElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookid", bibItem.BookId);
        $(liElement).attr("data-booktype", bibItem.BookType);
        $(liElement).attr("data-name", bibItem.Name);
        $(liElement).attr("data-century", bibItem.Century);
        //TODO toggle uncommented with commented code after testing
        //$(liElement).data('bookid', bibItem.BookId);
        //$(liElement).data('booktype', bibItem.BookType);
        //$(liElement).data('name', bibItem.Name);
        //$(liElement).data('century', bibItem.Century); //TODO add values for sorting


        var visibleContent: HTMLDivElement = document.createElement('div');
        $(visibleContent).addClass('visible-content');

        var bibFactory: BibliographyFactory = this.bibliographyFactoryResolver.getFactoryForType(bibItem.BookType);

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