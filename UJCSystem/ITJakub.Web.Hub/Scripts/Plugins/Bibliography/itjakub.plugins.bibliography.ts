/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />
/// <reference path="itjakub.plugins.bibliography.configuration.ts" />
/// <reference path="../itjakub.plugins.pagination.ts" />

class BibliographyModule {

    private resultsContainer;
    private booksContainer: HTMLDivElement;
    private sortBarContainer: string;
    private forcedBookType: BookTypeEnum;
    private bibliographyFactoryResolver: BibliographyFactoryResolver;
    private configurationManager: ConfigurationManager;

    private paginator: Pagination;
    private paginatorContainer: HTMLDivElement;
    private booksCount : number;
    private booksOnPage: number;

    private sortBar: SortBar;

    constructor(resultsContainer: string, sortBarContainer: string, forcedBookType?: BookTypeEnum) {
        this.resultsContainer = $(resultsContainer);

        this.booksContainer = document.createElement("div");
        $(this.booksContainer).addClass("bib-listing-books-div");
        this.paginatorContainer = document.createElement("div");
        $(this.paginatorContainer).addClass("bib-listing-pagination-div");

        $(this.resultsContainer).append(this.booksContainer);
        $(this.resultsContainer).append(this.paginatorContainer);

        this.sortBarContainer = sortBarContainer;
        this.forcedBookType = forcedBookType;

        //Download configuration
        var configObj;
        $.ajax({
            type: "GET",
            traditional: true,
            async: false,
            url: getBaseUrl()+"Bibliography/GetConfiguration",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                configObj = response;
            }
        });
        this.configurationManager = new ConfigurationManager(configObj);
        this.bibliographyFactoryResolver = new BibliographyFactoryResolver(this.configurationManager.getBookTypeConfigurations());
        $(this.sortBarContainer).empty();
        this.sortBar = new SortBar();
        var sortBarHtml = this.sortBar.makeSortBar(<any>this.booksContainer, this.sortBarContainer);
        $(this.sortBarContainer).append(sortBarHtml);
    }

    public showBooks(books: IBookInfo[]) {
        $(this.booksContainer).empty();
        if (books.length > 0) {
            var rootElement: HTMLUListElement = document.createElement('ul');
            $(rootElement).addClass('bib-listing');
            $.each(books, (index, book: IBookInfo) => {
                var bibliographyHtml = this.makeBibliography(book);
                rootElement.appendChild(bibliographyHtml);
            });
            $(this.booksContainer).append(rootElement);
        } else {
            var divElement: HTMLDivElement = document.createElement('div');
            $(divElement).addClass('bib-listing-empty');
            divElement.innerHTML = "Žádné výsledky k zobrazení";
            $(this.booksContainer).append(divElement);
        }

    }

    private makeBibliography(bibItem: IBookInfo): HTMLLIElement {
        var liElement: HTMLLIElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookid", bibItem.BookId);
        $(liElement).attr("data-booktype", bibItem.BookType);
        $(liElement).attr("data-name", bibItem.Title);
        $(liElement).attr("data-century", bibItem.Century);
        //TODO toggle uncommented with commented code after testing
        //$(liElement).data('bookid', bibItem.BookXmlId);
        //$(liElement).data('booktype', bibItem.BookType);
        //$(liElement).data('name', bibItem.Title);
        //$(liElement).data('century', bibItem.Century); //TODO add values for sorting


        var visibleContent: HTMLDivElement = document.createElement('div');
        $(visibleContent).addClass('visible-content');

        var bibFactory: BibliographyFactory;
        if (typeof this.forcedBookType == 'undefined') {
            bibFactory = this.bibliographyFactoryResolver.getFactoryForType(bibItem.BookType);
        } else {
            bibFactory = this.bibliographyFactoryResolver.getFactoryForType(this.forcedBookType);
        }

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

    public showPage(pageNumber: number) {
        this.paginator.goToPage(pageNumber);
    }

    public createPagination(booksOnPage: number, pageClickCallback: (pageNumber: number) => void, booksCount: number) {
        this.booksCount = booksCount;
        this.booksOnPage = booksOnPage;
        this.paginator = new Pagination(<any>this.paginatorContainer, booksOnPage);
        this.paginator.createPagination(booksCount, booksOnPage, pageClickCallback);

    }

    public getPagesCount(): number {
        return this.paginator.getPageCount();
    }

    public getBooksCount(): number {
        return this.booksCount;
    }

    public getBooksCountOnPage(): number {
        return this.booksOnPage;
    }

    public destroyPagination() {
        $(this.paginatorContainer).empty();
        this.paginator = null;
    }

    public isSortedAsc(): boolean {
        return this.sortBar.isSortedAsc();
    }

    public getSortCriteria(): SortEnum {
        return this.sortBar.getSortCriteria();
    }
}

interface IBookInfo {
    BookId: string;
    BookType: BookTypeEnum;
    Title: string;
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


//TODO remove or move to separated file
class BookInfo implements IBookInfo {
    BookId = "{FA10177B-25E6-4BB6-B061-0DB988AD3840}";
    //BookXmlId = "%7BFA10177B-25E6-4BB6-B061-0DB988AD3840%7D";
    BookType: BookTypeEnum;
    Title = "PasKal";
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

/*
 *      [EnumMember] Edition = 0, //Edice
        [EnumMember] Dictionary = 1, //Slovnik
        [EnumMember] Grammar = 2, //Mluvnice
        [EnumMember] ProfessionalLiterature = 3, //Odborna literatura
        [EnumMember] TextBank = 4, //Textova banka
        [EnumMember] BibliographicalItem = 5,
        [EnumMember] CardFile = 6,
 * 
 */
enum BookTypeEnum {
    Edition = 0, //Edice
    Dictionary = 1, //Slovnik
    Grammar = 2, //Mluvnice
    ProfessionalLiterature = 3, //Odborna literatura
    TextBank = 4, //Textova banka
    BibliographicalItem = 5,
    CardFile = 6, 
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