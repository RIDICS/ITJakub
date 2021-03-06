﻿/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />
/// <reference path="itjakub.plugins.bibliography.configuration.ts" />
/// <reference path="../Sort/itjakub.plugins.sort.ts" />

class BibliographyModule {
    
    private readonly errorHandler: ErrorHandler;
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

    private defaultConfigurationUrl = $("#bibliography-configuration-default-url").data("url");

    private isConfigurationLoad=false;
    private onConfigurationLoad: Array<() => any>=[];

    constructor(resultsContainer: string, sortBarContainer: string, sortChangeCallback: () => void, forcedBookType?: BookTypeEnum, customConfigurationPath?: string, protected modulInicializator?: ModulInicializator) {
        this.errorHandler = new ErrorHandler();
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
            url: configDownloadPath,
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
            },
            error: (jqXHR) => {
                this.showAjaxError(jqXHR);
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
            var bookDataList = new Array<IBookRenderData>();
            var rootElement: HTMLUListElement = document.createElement('ul');
            rootElement.classList.add('bib-listing');
            this.booksContainer.appendChild(rootElement);
            $.each(books, (index, book: IBookInfo) => {
                var bibliographyHtml = this.makeBibliography(book);
                rootElement.appendChild(bibliographyHtml);

                bookDataList.push({
                    bookId: book.BookId,
                    bookType: book.BookType,
                    bookName: book.Title,
                    element: bibliographyHtml
                });
            });
            this.showFavoriteLabels(bookDataList);
        } else {
            var divElement: HTMLDivElement = document.createElement('div');
            $(divElement).addClass('bib-listing-empty');
            divElement.innerHTML = localization.translate("NoResultsToShow", "PluginsJs").value;
            this.booksContainer.appendChild(divElement);
        }

    }

    private showFavoriteLabels(bookDataList: IBookRenderData[]) {
        var favoriteManager = new FavoriteManager();
        var bookIds = new Array<number>();
        $.each(bookDataList, (index, bookData) => {
            bookData.$favoritesContainer = $(".favorites", bookData.element);
            bookData.$favoriteButton = $(".favorite-button", bookData.element);
            if (bookData.$favoritesContainer.length > 0 || bookData.$favoriteButton.length > 0) {
                bookIds.push(Number(bookData.bookId));
            }
        });
        if (bookIds.length === 0) return;

        var favoriteLabels: IFavoriteLabel[] = null;
        var favoriteBooksDictionary: DictionaryWrapper<IFavoriteBaseInfoWithLabel[]> = null;

        favoriteManager.getLatestFavoriteLabels(labels => {
            favoriteLabels = labels;

            if (favoriteBooksDictionary != null) {
                this.finishShowingFavoriteLabels(bookDataList, favoriteBooksDictionary, favoriteLabels);
            }
        });

        favoriteManager.getFavoritesForBooks(null, bookIds, favoriteBooks => {
            favoriteBooksDictionary = new DictionaryWrapper<IFavoriteBaseInfoWithLabel[]>();
            $.each(favoriteBooks, (index, favoriteLabeledBook) => {
                favoriteBooksDictionary.add(favoriteLabeledBook.id, favoriteLabeledBook.favoriteInfo); 
            });

            if (favoriteLabels != null) {
                this.finishShowingFavoriteLabels(bookDataList, favoriteBooksDictionary, favoriteLabels);
            }
        });
    }

    private finishShowingFavoriteLabels(bookDataList: IBookRenderData[], favoriteBooksDictionary: DictionaryWrapper<IFavoriteBaseInfoWithLabel[]>, favoriteLabels: IFavoriteLabel[]) {
        $.each(bookDataList, (index, bookData) => {
            var bookFavorites = favoriteBooksDictionary.get(bookData.bookId);
            if (bookFavorites) {
                var bibliographyFactory = this.getBibliographyFactory(bookData.bookType);
                bookData.$favoritesContainer.append(bibliographyFactory.makeFavoriteBookInfo(bookFavorites));
            }

            // create FavoriteStar
            if (bookData.$favoriteButton.length === 0) {
                return;
            }
            var newFavoriteDialog = NewFavoriteDialogProvider.getInstance(true);
            var favoriteStar = new FavoriteStar(bookData.$favoriteButton, FavoriteType.Project, bookData.bookId.toString(), bookData.bookName, newFavoriteDialog, new FavoriteManager(), () => {
                new NewFavoriteNotification().show();
            });
            if (bookFavorites) {
                favoriteStar.addFavoriteItems(bookFavorites);
            }
            favoriteStar.addFavoriteLabels(favoriteLabels);
            favoriteStar.make("left", true);

            var className = bookData.$favoriteButton.data("buttonClass");
            $("a", bookData.$favoriteButton).addClass(className);
        });
    }

    public clearBooks() {
        $(this.booksContainer).empty();
    }

    public clearLoading() {
        $("#loader").remove();
    }

    public showLoading() {
        if ($("#loader").length === 0) {
            var loader = lv.create(null, "lv-circles sm lv-mid lvt-5");
            loader.setId("loader");
            $(this.booksContainer).append(loader.getElement());
        }
    }

    public showSearchError() {
        this.showError(localization.translate("SearchError", "PluginsJs").value);
    }

    public showPageLoadError() {
        this.showError(localization.translate("LoadingBookListError", "PluginsJs").value);
    }

    public showAjaxError(jqXHR: JQueryXHR) {
        this.showError(this.errorHandler.getErrorMessage(jqXHR));
    }
    
    public showError(text: string) {
        var errorDiv = BibliographyFactory.makeError(text);
        $(this.booksContainer)
            .empty()
            .append(errorDiv);
    }

    private getBibliographyFactory(bookType: BookTypeEnum): BibliographyFactory {
        if (typeof this.forcedBookType == 'undefined') {
            return this.bibliographyFactoryResolver.getFactoryForType(bookType);
        } else {
            return this.bibliographyFactoryResolver.getFactoryForType(this.forcedBookType);
        }
    }

    private makeBibliography(bibItem: IBookInfo): HTMLLIElement {
        var liElement: HTMLLIElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-id", bibItem.BookId);
        $(liElement).attr("data-booktype", bibItem.BookType);
        $(liElement).attr("data-name", bibItem.Title);

        var visibleContent: HTMLDivElement = document.createElement('div');
        $(visibleContent).addClass('visible-content');
        var visibleWrapper: HTMLDivElement = document.createElement('div');
        $(visibleWrapper).addClass('visible-wrapper');

        this.runAsyncOnLoad(()=> {
            var bibFactory = this.getBibliographyFactory(bibItem.BookType);
            
            var leftPanel = bibFactory.makeLeftPanel(bibItem);
            if (leftPanel != null) visibleWrapper.appendChild(leftPanel);

            var middlePanel = bibFactory.makeMiddlePanel(bibItem);
            if (middlePanel != null) visibleWrapper.appendChild(middlePanel);

            var midRightPanel = bibFactory.makeMidRightPanel(bibItem);
            if (midRightPanel != null) visibleWrapper.appendChild(midRightPanel);

            var rightPanel = bibFactory.makeRightPanel(bibItem);
            if (rightPanel != null) visibleWrapper.appendChild(rightPanel);

            $(visibleContent).append(visibleWrapper);
            $(liElement).append(visibleContent);

            var hiddenContent: HTMLDivElement = document.createElement('div');
            $(hiddenContent).addClass('hidden-content');

            if (bibFactory.configuration.containsBottomPanel()) {
                $(hiddenContent).addClass("not-loaded");

                var loadingDiv = lv.create(null, "lv-circles lv-mid sm lvt-2 lvb-2");
                hiddenContent.appendChild(loadingDiv.getElement());
            }

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
        var paginationOptions: Pagination.Options = {
            container: this.paginatorContainer,
            pageClickCallback: pageClickCallback,
            callPageClickCallbackOnInit: true
        };
        this.paginator = new Pagination(paginationOptions);
        this.paginator.make(booksCount, booksOnPage, initPageNumber);
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

//var newFavoriteFromBibliographyDialog: NewFavoriteDialog;
//function addFavoriteFromBibliography(target) {
//    return context => {
//        if (!newFavoriteFromBibliographyDialog) {
//            var favoriteManager = new FavoriteManager();
//            newFavoriteFromBibliographyDialog = new NewFavoriteDialog(favoriteManager, true);
//            newFavoriteFromBibliographyDialog.make();
//        }

//        var $item = $(target).parents("li.list-item");
//        var bookId = $item.attr("data-id");
//        var bookName = $item.attr("data-name");
//        newFavoriteFromBibliographyDialog.setSaveCallback(data => {
//            var favoriteManager = new FavoriteManager();
//            var labelIds: Array<number> = [];
//            var labelElements: Array<HTMLSpanElement> = [];
//            for (var i = 0; i < data.labels.length; i++) {
//                var label = data.labels[i];
//                labelIds.push(label.labelId);
//                var labelSpan = BibliographyFactory.makeFavoriteLabel(data.itemName, label.labelName, label.labelColor);
//                labelElements.push(labelSpan);
//            }
//            favoriteManager.createFavoriteItem(FavoriteType.Book, bookId, data.itemName, labelIds, (ids, error) => {
//                if (error) {
//                    newFavoriteFromBibliographyDialog.showError("Chyba při vytváření oblíbené knihy");
//                    return;
//                }

//                newFavoriteFromBibliographyDialog.hide();
//                $(".favorites", $item).append(labelElements);
//            });
//        });
//        newFavoriteFromBibliographyDialog.show(bookName);
//    };
//}

//functions used in VariableInterpreter.interpretScript

function translateAudioType(audioType: AudioType): string {
    switch (audioType) {
        case AudioType.Mp3:
            return "Mp3";
        case AudioType.Ogg:
            return "Ogg";
        case AudioType.Wav:
            return "Wav";
        default:
            return localization.translate("Unknown", "PluginsJs").value;
    }
}

function fillLeadingZero(seconds: number): string {
    var secondsString = seconds.toString();
    if (secondsString.length === 1) {
        secondsString = `0${secondsString}`;
    }
    return secondsString;
}

function getAudioLengthString(value: string): string {
    return new TimeSpan(value).toShortString();
}

interface IBookRenderData {
    bookId: string;
    bookType: BookTypeEnum;
    bookName: string;
    element: HTMLLIElement;
    $favoritesContainer?: JQuery;
    $favoriteButton?: JQuery;
}

interface IBookInfo {
    BookId: string;
    ProjectType: ProjectType;
    TextType: ProjectTextType;
    BookType: BookTypeEnum;
    Title: string;
    RelicAbbreviation: string;
    SourceAbbreviation: string;
    // there are other properties but they are not required in TypeScript because they are used in configuration JSON file
}

enum BookTypeEnum {
    Edition = "Edition", //Edice
    Dictionary = "Dictionary", //Slovnik
    Grammar = "Grammar", //Mluvnice
    ProfessionalLiterature = "ProfessionalLiterature", //Odborna literatura
    TextBank = "TextBank", //Textova banka
    BibliographicalItem = "BibliographicalItem",
    CardFile = "CardFile",
    AudioBook = "AudioBook"
}

enum ProjectType {
    Research = "Research",
    Community = "Community",
    Bibliography = "Bibliography",
}

enum ProjectTextType {
    NoneOrOriginal = "NoneOrOriginal",
    Transliterated = "Transliterated",
    Transcribed = "Transcribed",
}

//interface IPage {
//    Start: number;
//    End: number;
//}

interface IAuthor {
    FirstName: string;
    LastName: string;
}