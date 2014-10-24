/// <reference path="itjakub.plugins.bibliography.variableInterpreter.ts" />
/// <reference path="itjakub.plugins.bibliography.factories.ts" />
var BibliographyModule = (function () {
    function BibliographyModule(booksContainer, sortBarContainer) {
        this.booksContainer = booksContainer;
        this.sortBarContainer = sortBarContainer;
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
            alert(value);
        });
        this.addOption(select, "Název", "Name");
        this.addOption(select, "Autor", "Author");
        this.addOption(select, "Datace", "Date"); //TODO add options to json config
        sortBarDiv.appendChild(select);
        return sortBarDiv;
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
        $(liElement).attr("data-bookId", bibItem.BookId);
        $(liElement).attr("data-bookType", bibItem.BookType);

        var visibleContent = document.createElement('div');
        $(visibleContent).addClass('visible-content');

        var bibFactory = BibliographyFactoryResolver.getInstance().getFactoryForType(bibItem.BookType);

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

var ConfigurationManager = (function () {
    function ConfigurationManager(config) {
        this.config = config;
        this.varInterpreter = new VariableInterpreter();
    }
    ConfigurationManager.prototype.containsMiddlePanel = function () {
        return typeof this.config["middle-panel"] !== 'undefined';
    };

    ConfigurationManager.prototype.containsBottomPanel = function () {
        return typeof this.config["bottom-panel"] !== 'undefined';
    };

    ConfigurationManager.prototype.containsCustomInMiddlePanel = function () {
        return typeof this.config['middle-panel']['custom'] !== 'undefined';
    };

    ConfigurationManager.prototype.containsCustomInBottomPanel = function () {
        return typeof this.config['bottom-panel']['custom'] !== 'undefined';
    };

    ConfigurationManager.prototype.containsMiddlePanelBody = function () {
        return typeof this.config["middle-panel"]['body'] !== 'undefined';
    };

    ConfigurationManager.prototype.containsBottomPanelBody = function () {
        return typeof this.config["bottom-panel"]['body'] !== 'undefined';
    };

    ConfigurationManager.prototype.containsMiddlePanelTitle = function () {
        return typeof this.config["middle-panel"]['title'] !== 'undefined';
    };

    ConfigurationManager.prototype.getTitle = function (bibItem) {
        return this.varInterpreter.interpret(this.config['middle-panel']['title'], this.config['middle-panel']['variables'], bibItem);
    };

    ConfigurationManager.prototype.getMiddlePanelBody = function (bibItem) {
        return this.varInterpreter.interpret(this.config['middle-panel']['body'], this.config['middle-panel']['variables'], bibItem);
    };

    ConfigurationManager.prototype.getBottomPanelBody = function (bibItem) {
        return this.varInterpreter.interpret(this.config['bottom-panel']['body'], this.config['bottom-panel']['variables'], bibItem);
    };

    ConfigurationManager.prototype.getCustomInMiddlePanel = function (bibItem) {
        return this.varInterpreter.interpret(this.config['middle-panel']['custom'], this.config['middle-panel']['variables'], bibItem);
    };

    ConfigurationManager.prototype.getCustomInBottomPanel = function (bibItem) {
        return this.varInterpreter.interpret(this.config['bottom-panel']['custom'], this.config['bottom-panel']['variables'], bibItem);
    };
    return ConfigurationManager;
})();
//# sourceMappingURL=itjakub.plugins.bibliography.js.map
