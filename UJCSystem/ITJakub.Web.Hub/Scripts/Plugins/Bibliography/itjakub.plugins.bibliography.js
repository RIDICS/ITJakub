/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />
/// <reference path="itjakub.plugins.bibliography.configuration.ts" />
var BibliographyModule = (function () {
    function BibliographyModule(booksContainer, sortBarContainer) {
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
            success: function (response) {
                configObj = response;
            }
        });
        this.configurationManager = new ConfigurationManager(configObj);
        this.bibliographyFactoryResolver = new BibliographyFactoryResolver(this.configurationManager.getBookTypeConfigurations());
    }
    BibliographyModule.prototype.showBooks = function (books) {
        var _this = this;
        $(this.booksContainer).empty();
        var rootElement = document.createElement('ul');
        $(rootElement).addClass('bib-listing');
        $.each(books, function (index, book) {
            var bibliographyHtml = _this.makeBibliography(book);
            rootElement.appendChild(bibliographyHtml);
        });
        $(this.booksContainer).append(rootElement);
        $(this.sortBarContainer).empty();
        var sortBarHtml = new SortBar().makeSortBar(this.booksContainer, this.sortBarContainer);
        $(this.sortBarContainer).append(sortBarHtml);
    };

    BibliographyModule.prototype.makeBibliography = function (bibItem) {
        var liElement = document.createElement('li');
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
        var visibleContent = document.createElement('div');
        $(visibleContent).addClass('visible-content');

        var bibFactory = this.bibliographyFactoryResolver.getFactoryForType(bibItem.BookType);

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
    return BibliographyModule;
})();
//# sourceMappingURL=itjakub.plugins.bibliography.js.map
