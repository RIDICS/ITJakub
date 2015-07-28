/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />
/// <reference path="itjakub.plugins.bibliography.configuration.ts" />
/// <reference path="../itjakub.plugins.pagination.ts" />
var BibliographyModule = (function () {
    function BibliographyModule(resultsContainer, sortBarContainer, forcedBookType) {
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
            url: getBaseUrl() + "Bibliography/GetConfiguration",
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                configObj = response;
            }
        });
        this.configurationManager = new ConfigurationManager(configObj);
        this.bibliographyFactoryResolver = new BibliographyFactoryResolver(this.configurationManager.getBookTypeConfigurations());
        $(this.sortBarContainer).empty();
        this.sortBar = new SortBar();
        var sortBarHtml = this.sortBar.makeSortBar(this.booksContainer, this.sortBarContainer);
        $(this.sortBarContainer).append(sortBarHtml);
    }
    BibliographyModule.prototype.showBooks = function (books) {
        var _this = this;
        this.clearBooks();
        this.clearLoading();
        if (books.length > 0) {
            var rootElement = document.createElement('ul');
            $(rootElement).addClass('bib-listing');
            $.each(books, function (index, book) {
                var bibliographyHtml = _this.makeBibliography(book);
                rootElement.appendChild(bibliographyHtml);
            });
            $(this.booksContainer).append(rootElement);
        }
        else {
            var divElement = document.createElement('div');
            $(divElement).addClass('bib-listing-empty');
            divElement.innerHTML = "Žádné výsledky k zobrazení";
            $(this.booksContainer).append(divElement);
        }
    };
    BibliographyModule.prototype.clearBooks = function () {
        $(this.booksContainer).empty();
    };
    BibliographyModule.prototype.clearLoading = function () {
        $(this.booksContainer).removeClass("loader");
    };
    BibliographyModule.prototype.showLoading = function () {
        $(this.booksContainer).addClass("loader");
    };
    BibliographyModule.prototype.makeBibliography = function (bibItem) {
        var liElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookid", bibItem.BookXmlId);
        $(liElement).attr("data-booktype", bibItem.BookType);
        $(liElement).attr("data-name", bibItem.Title);
        $(liElement).attr("data-century", bibItem.Century);
        var visibleContent = document.createElement('div');
        $(visibleContent).addClass('visible-content');
        var bibFactory;
        if (typeof this.forcedBookType == 'undefined') {
            bibFactory = this.bibliographyFactoryResolver.getFactoryForType(bibItem.BookType);
        }
        else {
            bibFactory = this.bibliographyFactoryResolver.getFactoryForType(this.forcedBookType);
        }
        var panel = bibFactory.makeLeftPanel(bibItem);
        if (panel != null)
            visibleContent.appendChild(panel);
        panel = bibFactory.makeRightPanel(bibItem);
        if (panel != null)
            visibleContent.appendChild(panel);
        panel = bibFactory.makeMiddlePanel(bibItem);
        if (panel != null)
            visibleContent.appendChild(panel);
        $(liElement).append(visibleContent);
        var hiddenContent = document.createElement('div');
        $(hiddenContent).addClass('hidden-content');
        panel = bibFactory.makeBottomPanel(bibItem);
        if (panel != null)
            hiddenContent.appendChild(panel);
        $(liElement).append(hiddenContent);
        return liElement;
    };
    BibliographyModule.prototype.showPage = function (pageNumber) {
        this.paginator.goToPage(pageNumber);
    };
    BibliographyModule.prototype.createPagination = function (booksOnPage, pageClickCallback, booksCount) {
        this.booksCount = booksCount;
        this.booksOnPage = booksOnPage;
        this.paginator = new Pagination(this.paginatorContainer, booksOnPage);
        this.paginator.createPagination(booksCount, booksOnPage, pageClickCallback);
    };
    BibliographyModule.prototype.getPagesCount = function () {
        return this.paginator.getPageCount();
    };
    BibliographyModule.prototype.getBooksCount = function () {
        return this.booksCount;
    };
    BibliographyModule.prototype.getBooksCountOnPage = function () {
        return this.booksOnPage;
    };
    BibliographyModule.prototype.destroyPagination = function () {
        $(this.paginatorContainer).empty();
        this.paginator = null;
    };
    BibliographyModule.prototype.isSortedAsc = function () {
        return this.sortBar.isSortedAsc();
    };
    BibliographyModule.prototype.getSortCriteria = function () {
        return this.sortBar.getSortCriteria();
    };
    return BibliographyModule;
})();
////TODO remove or move to separated file
//class BookInfo implements IBookInfo {
//    BookXmlId = "{FA10177B-25E6-4BB6-B061-0DB988AD3840}";
//    //BookXmlId = "%7BFA10177B-25E6-4BB6-B061-0DB988AD3840%7D";
//    BookType: BookTypeEnum;
//    Title = "PasKal";
//    Editor: string;
//    Pattern: string;
//    SourceAbbreviation: string;
//    RelicAbbreviation: string;
//    LiteraryType: string;
//    LiteraryGenre: string;
//    LastEditation: string;
//    EditationNote: string; //anchor href?
//    Copyright: string;
//    Pages: IPage[];
//    Archive: IArchive;
//    Century: number;
//    Sign: string;
//    Authors: IAuthor[];
//    Description: string;
//    Year: number;
//}
//
//  [DataContract]
//  public enum BookTypeEnumContract : byte
//  {
//    [EnumMember] Edition = 0, //Edice
//    [EnumMember] Dictionary = 1, //Slovnik
//    [EnumMember] Grammar = 2, //Mluvnice
//    [EnumMember] ProfessionalLiterature = 3, //Odborna literatura
//    [EnumMember] TextBank = 4, //Textova banka
//    [EnumMember] BibliographicalItem = 5,
//    [EnumMember] CardFile = 6,
//    }
var BookTypeEnum;
(function (BookTypeEnum) {
    BookTypeEnum[BookTypeEnum["Edition"] = 0] = "Edition";
    BookTypeEnum[BookTypeEnum["Dictionary"] = 1] = "Dictionary";
    BookTypeEnum[BookTypeEnum["Grammar"] = 2] = "Grammar";
    BookTypeEnum[BookTypeEnum["ProfessionalLiterature"] = 3] = "ProfessionalLiterature";
    BookTypeEnum[BookTypeEnum["TextBank"] = 4] = "TextBank";
    BookTypeEnum[BookTypeEnum["BibliographicalItem"] = 5] = "BibliographicalItem";
    BookTypeEnum[BookTypeEnum["CardFile"] = 6] = "CardFile";
})(BookTypeEnum || (BookTypeEnum = {}));
//# sourceMappingURL=itjakub.plugins.bibliography.js.map