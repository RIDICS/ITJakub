﻿/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />
/// <reference path="itjakub.plugins.bibliography.configuration.ts" />
/// <reference path="../Sort/itjakub.plugins.sort.ts" />
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
    private booksCount : number = 0;
    private booksOnPage: number = 10;

    private sortBar: SortBar;

    private sortChangeCallback: () => void;

    private defaultConfigurationUrl = "Bibliography/GetConfiguration";

    private isConfigurationLoad=false;
    private onConfigurationLoad: Array<() => any>=[];

    constructor(resultsContainer: string, sortBarContainer: string, sortChangeCallback: () => void, forcedBookType?: BookTypeEnum, customConfigurationPath?: string, protected modulInicializator?: ModulInicializator) {
        this.resultsContainer = $(resultsContainer);
        this.sortChangeCallback = sortChangeCallback;

        this.booksContainer = document.createElement("div");
        $(this.booksContainer).addClass("bib-listing-books-div");
        this.paginatorContainer = document.createElement("div");
        $(this.paginatorContainer).addClass("bib-listing-pagination-div");

        $(this.resultsContainer).append(this.booksContainer);
        $(this.resultsContainer).append(this.paginatorContainer);

        this.sortBarContainer = sortBarContainer;
        this.forcedBookType = forcedBookType;

        //Download configuration
        let configDownloadPath = this.defaultConfigurationUrl;

        if (typeof customConfigurationPath !== "undefined" && customConfigurationPath !== null && customConfigurationPath !== "") {
            configDownloadPath = customConfigurationPath;
        }

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + configDownloadPath,
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                this.configurationManager = new ConfigurationManager(response);
                this.bibliographyFactoryResolver = new BibliographyFactoryResolver(this.configurationManager.getBookTypeConfigurations(), this.modulInicializator);
                $(this.sortBarContainer).empty();
                this.sortBar = new SortBar(this.sortChangeCallback);
                var sortBarHtml = this.sortBar.makeSortBar(this.sortBarContainer);
                $(this.sortBarContainer).append(sortBarHtml);

                this.isConfigurationLoad = true;
                for (let i = 0; i < this.onConfigurationLoad.length; i++) {
                    this.onConfigurationLoad[i]();
                }
            }
        });
    }

    public runAsyncOnLoad(callback:()=>any) {
        if (this.isConfigurationLoad) {
            callback();
        } else {
            this.onConfigurationLoad.push(callback);
        }

        return this.isConfigurationLoad;
    }

    public showBooks(books: IBookInfo[]) {
        this.clearBooks();
        this.clearLoading();
        if (typeof books !== "undefined" && books !== null && books.length > 0) {
            var rootElement: HTMLUListElement = document.createElement('ul');
            rootElement.classList.add('bib-listing');
            this.booksContainer.appendChild(rootElement);
            $.each(books, (index, book: IBookInfo) => {
                var bibliographyHtml = this.makeBibliography(book);
                rootElement.appendChild(bibliographyHtml);
            });
        } else {
            var divElement: HTMLDivElement = document.createElement('div');
            $(divElement).addClass('bib-listing-empty');
            divElement.innerHTML = "Žádné výsledky k zobrazení";
            this.booksContainer.appendChild(divElement);
        }

    }

    public clearBooks() {
        $(this.booksContainer).empty();
    }

    public clearLoading() {
        $(this.booksContainer).removeClass("loader");
    }

    public showLoading() {
        $(this.booksContainer).addClass("loader");
    }

    private makeBibliography(bibItem: IBookInfo): HTMLLIElement {
        var liElement: HTMLLIElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookid", bibItem.BookXmlId);
        $(liElement).attr("data-booktype", bibItem.BookType);
        $(liElement).attr("data-name", bibItem.Title);
        $(liElement).attr("data-century", bibItem.Century);

        var visibleContent: HTMLDivElement = document.createElement('div');
        $(visibleContent).addClass('visible-content');

        this.runAsyncOnLoad(()=> {
            var bibFactory: BibliographyFactory;
            if (typeof this.forcedBookType == 'undefined') {
                bibFactory = this.bibliographyFactoryResolver.getFactoryForType(bibItem.BookType);
            } else {
                bibFactory = this.bibliographyFactoryResolver.getFactoryForType(this.forcedBookType);
            }

            var leftPanel = bibFactory.makeLeftPanel(bibItem);
            if (leftPanel != null) visibleContent.appendChild(leftPanel);

            var rightPanel = bibFactory.makeRightPanel(bibItem);
            if (rightPanel != null) visibleContent.appendChild(rightPanel);

            var middlePanel = bibFactory.makeMiddlePanel(bibItem);
            if (middlePanel != null) visibleContent.appendChild(middlePanel);

            $(liElement).append(visibleContent);

            var hiddenContent: HTMLDivElement = document.createElement('div');
            $(hiddenContent).addClass('hidden-content');

            var bottomPanel = bibFactory.makeBottomPanel(bibItem);
            if (bottomPanel != null) hiddenContent.appendChild(bottomPanel);

            $(liElement).append(hiddenContent);
        });

        return liElement;

    }

    public showPage(pageNumber: number) {
        this.paginator.goToPage(pageNumber);
    }

    public createPagination(booksOnPage: number, pageClickCallback: (pageNumber: number) => void, booksCount: number, initPageNumber: number = 1) {
        this.booksCount = booksCount;
        this.booksOnPage = booksOnPage;
        this.paginator = new Pagination(<any>this.paginatorContainer, booksOnPage);
        this.paginator.createPagination(booksCount, booksOnPage, pageClickCallback, initPageNumber);

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

    public setSortedAsc(isSortedAsc: boolean) {
        this.sortBar.setSortedAsc(isSortedAsc);
    }

    public getSortCriteria(): SortEnum {
        return this.sortBar.getSortCriteria();
    }

    public setSortCriteria(sortCriteria: SortEnum) {
        return this.sortBar.setSortCriteria(sortCriteria);
    }
}

interface IBookInfo {
    BookId: number;
    BookXmlId: string;
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

enum BookTypeEnum {
    Edition = 0, //Edice
    Dictionary = 1, //Slovnik
    Grammar = 2, //Mluvnice
    ProfessionalLiterature = 3, //Odborna literatura
    TextBank = 4, //Textova banka
    BibliographicalItem = 5,
    CardFile = 6,
    AudioBook = 7
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