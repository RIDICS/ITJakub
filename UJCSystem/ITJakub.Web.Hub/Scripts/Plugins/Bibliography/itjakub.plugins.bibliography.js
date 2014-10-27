/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />
/// <reference path="itjakub.plugins.bibliography.configuration.ts" />
var BibliographyModule = (function () {
    function BibliographyModule(booksContainer, sortBarContainer) {
        var _this = this;
        this.booksContainer = booksContainer;
        this.sortBarContainer = sortBarContainer;

        //Download configuration
        $.ajax({
            type: "GET",
            traditional: true,
            async: false,
            url: "/Bibliography/GetConfiguration",
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                _this.configurationManager = new ConfigurationManager(response);
            }
        });

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
        var sortBarHtml = this.makeSortBar();
        $(this.sortBarContainer).append(sortBarHtml);
    };

    BibliographyModule.prototype.makeSortBar = function () {
        var _this = this;
        var sortBarDiv = document.createElement('div');
        $(sortBarDiv).addClass('bib-sortbar');
        var select = document.createElement('select');
        $(select).change(function () {
            var selectedOption = $(_this.sortBarContainer).find('div.bib-sortbar').find('select').find("option:selected");
            var value = $(selectedOption).val();
            var order = -1;
            var comparator = function (a, b) {
                var aa = $(a).data(value);
                var bb = $(b).data(value);
                return aa > bb ? 1 : -1;
            };
            _this.sort(comparator, order);
        });
        this.addOption(select, "Název", "name");
        this.addOption(select, "Id", "bookid");
        this.addOption(select, "Datace", "century"); //TODO add options to json config
        this.addOption(select, "Typ", "booktype");
        sortBarDiv.appendChild(select);
        return sortBarDiv;
    };

    BibliographyModule.prototype.sort = function (comparator, order) {
        var elems = $(this.booksContainer).children('ul.bib-listing').children('li').detach();
        var sortFunction = function (a, b) {
            return order * comparator(a, b);
        };
        elems.sort(sortFunction);
        $(this.booksContainer).children('ul.bib-listing').append(elems);
    };

    BibliographyModule.prototype.addOption = function (selectbox, text, value) {
        var option = document.createElement('option');
        option.text = text;
        option.value = value;
        selectbox.appendChild(option);
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
