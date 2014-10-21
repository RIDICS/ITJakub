var BibliographyModule = (function () {
    function BibliographyModule() {
        this.bibliographyModulControllerUrl = "";
        if (BibliographyModule._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        BibliographyModule._instance = this;
    }
    BibliographyModule.getInstance = function () {
        if (BibliographyModule._instance === null) {
            BibliographyModule._instance = new BibliographyModule();
        }
        return BibliographyModule._instance;
    };

    BibliographyModule.prototype.showBooks = function (books, container) {
        var _this = this;
        $(container).empty();
        var rootElement = document.createElement('ul');
        $(rootElement).addClass('listing');
        $.each(books, function (index, book) {
            var bibliographyHtml = _this.makeBibliography(book);
            rootElement.appendChild(bibliographyHtml);
        });
        $(container).append(rootElement);
    };

    BibliographyModule.prototype.makeBibliography = function (bibItem) {
        var liElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookId", bibItem.BookId);

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
    BibliographyModule._instance = null;
    return BibliographyModule;
})();

var OldCzechTextBankFactory = (function () {
    function OldCzechTextBankFactory() {
    }
    OldCzechTextBankFactory.prototype.makeLeftPanel = function (bookInfo) {
        var leftPanel = document.createElement('div');
        $(leftPanel).addClass('left-panel');
        return leftPanel;
    };

    OldCzechTextBankFactory.prototype.makeRightPanel = function (bookInfo) {
        var rightPanel = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var infoButton = document.createElement('button');
        infoButton.type = 'button';
        $(infoButton).addClass('btn btn-sm information-button');
        var spanInfo = document.createElement('span');
        $(spanInfo).addClass('glyphicon glyphicon-info-sign');
        infoButton.appendChild(spanInfo);
        $(infoButton).click(function (event) {
        }); //TODO fill click action
        rightPanel.appendChild(infoButton);

        var showContentButton = document.createElement('button');
        showContentButton.type = 'button';
        $(showContentButton).addClass('btn btn-sm show-button');
        var spanChevrDown = document.createElement('span');
        $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
        showContentButton.appendChild(spanChevrDown);
        $(showContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').show("slow");
            $(this).siblings('.hide-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(showContentButton);

        var hideContentButton = document.createElement('button');
        hideContentButton.type = 'button';
        $(hideContentButton).addClass('btn btn-sm hide-button');
        var spanChevrUp = document.createElement('span');
        $(spanChevrUp).addClass('glyphicon glyphicon-chevron-up');
        hideContentButton.appendChild(spanChevrUp);
        $(hideContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').hide("slow");
            $(this).siblings('.show-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(hideContentButton);

        return rightPanel;
    };

    OldCzechTextBankFactory.prototype.makeMiddlePanel = function (bookInfo) {
        var middlePanel = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        var middlePanelHeading = document.createElement('div');
        $(middlePanelHeading).addClass('heading');
        middlePanelHeading.innerHTML = bookInfo.Name;
        middlePanel.appendChild(middlePanelHeading);
        var middlePanelBody = document.createElement('div');
        $(middlePanelBody).addClass('body');
        middlePanelBody.innerHTML = bookInfo.Body;
        middlePanel.appendChild(middlePanelBody);
        return middlePanel;
    };

    OldCzechTextBankFactory.prototype.makeBottomPanel = function (bookInfo) {
        var tableBuilder = new TableBuilder();
        tableBuilder.makeTableRow("Editor", bookInfo.Editor);
        tableBuilder.makeTableRow("Předloha", bookInfo.Pattern);
        tableBuilder.makeTableRow("Zkratka památky", bookInfo.RelicAbbreviation);
        tableBuilder.makeTableRow("Zkratka pramene", bookInfo.SourceAbbreviation);
        tableBuilder.makeTableRow("Literární druh", bookInfo.LiteraryType);
        tableBuilder.makeTableRow("Literární žánr", bookInfo.LiteraryGenre);
        tableBuilder.makeTableRow("Poslední úprava edice", bookInfo.LastEditation);
        return tableBuilder.build();
    };
    return OldCzechTextBankFactory;
})();

var DictionaryFactory = (function () {
    function DictionaryFactory() {
    }
    DictionaryFactory.prototype.makeLeftPanel = function (bookInfo) {
        var leftPanel = document.createElement('div');
        $(leftPanel).addClass('left-panel');

        var inputCheckbox = document.createElement('input');
        inputCheckbox.type = "checkbox";
        $(inputCheckbox).addClass('checkbox');
        leftPanel.appendChild(inputCheckbox);

        var starEmptyButton = document.createElement('button');
        starEmptyButton.type = 'button';
        $(starEmptyButton).addClass('btn btn-xs star-empty-button');
        var spanEmptyStar = document.createElement('span');
        $(spanEmptyStar).addClass('glyphicon glyphicon-star-empty');
        starEmptyButton.appendChild(spanEmptyStar);
        $(starEmptyButton).click(function (event) {
            $(this).siblings('.star-button').show();
            $(this).hide();
        }); //TODO fill click action
        leftPanel.appendChild(starEmptyButton);

        var starButton = document.createElement('button');
        starButton.type = 'button';
        $(starButton).addClass('btn btn-xs star-button');
        $(starButton).css('display', 'none');
        var spanStar = document.createElement('span');
        $(spanStar).addClass('glyphicon glyphicon-star');
        starButton.appendChild(spanStar);
        $(starButton).click(function (event) {
            $(this).siblings('.star-empty-button').show();
            $(this).hide();
        }); //TODO fill click action
        leftPanel.appendChild(starButton);

        return leftPanel;
    };

    DictionaryFactory.prototype.makeRightPanel = function (bookInfo) {
        var rightPanel = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var bookButton = document.createElement('button');
        bookButton.type = 'button';
        $(bookButton).addClass('btn btn-sm book-button');
        var spanBook = document.createElement('span');
        $(spanBook).addClass('glyphicon glyphicon-book');
        bookButton.appendChild(spanBook);
        $(bookButton).click(function (event) {
        }); //TODO fill click action
        rightPanel.appendChild(bookButton);

        var infoButton = document.createElement('button');
        infoButton.type = 'button';
        $(infoButton).addClass('btn btn-sm information-button');
        var spanInfo = document.createElement('span');
        $(spanInfo).addClass('glyphicon glyphicon-info-sign');
        infoButton.appendChild(spanInfo);
        $(infoButton).click(function (event) {
        }); //TODO fill click action
        rightPanel.appendChild(infoButton);

        return rightPanel;
    };

    DictionaryFactory.prototype.makeMiddlePanel = function (bookInfo) {
        var middlePanel = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        var middlePanelHeading = document.createElement('div');
        $(middlePanelHeading).addClass('heading');
        middlePanelHeading.innerHTML = bookInfo.Name;
        middlePanel.appendChild(middlePanelHeading);
        var middlePanelBody = document.createElement('div');
        $(middlePanelBody).addClass('body');
        middlePanelBody.innerHTML = bookInfo.Body;
        middlePanel.appendChild(middlePanelBody);
        return middlePanel;
    };

    DictionaryFactory.prototype.makeBottomPanel = function (bookInfo) {
        return null;
    };
    return DictionaryFactory;
})();

var EditionFactory = (function () {
    function EditionFactory() {
    }
    EditionFactory.prototype.makeLeftPanel = function (bookInfo) {
        var leftPanel = document.createElement('div');
        $(leftPanel).addClass('left-panel');
        return leftPanel;
    };

    EditionFactory.prototype.makeRightPanel = function (bookInfo) {
        var rightPanel = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var bookButton = document.createElement('button');
        bookButton.type = 'button';
        $(bookButton).addClass('btn btn-sm book-button');
        var spanBook = document.createElement('span');
        $(spanBook).addClass('glyphicon glyphicon-book');
        bookButton.appendChild(spanBook);
        $(bookButton).click(function (event) {
        }); //TODO fill click action
        rightPanel.appendChild(bookButton);

        var infoButton = document.createElement('button');
        infoButton.type = 'button';
        $(infoButton).addClass('btn btn-sm information-button');
        var spanInfo = document.createElement('span');
        $(spanInfo).addClass('glyphicon glyphicon-info-sign');
        infoButton.appendChild(spanInfo);
        $(infoButton).click(function (event) {
        }); //TODO fill click action
        rightPanel.appendChild(infoButton);

        var showContentButton = document.createElement('button');
        showContentButton.type = 'button';
        $(showContentButton).addClass('btn btn-sm show-button');
        var spanChevrDown = document.createElement('span');
        $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
        showContentButton.appendChild(spanChevrDown);
        $(showContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').show("slow");
            $(this).siblings('.hide-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(showContentButton);

        var hideContentButton = document.createElement('button');
        hideContentButton.type = 'button';
        $(hideContentButton).addClass('btn btn-sm hide-button');
        var spanChevrUp = document.createElement('span');
        $(spanChevrUp).addClass('glyphicon glyphicon-chevron-up');
        hideContentButton.appendChild(spanChevrUp);
        $(hideContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').hide("slow");
            $(this).siblings('.show-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(hideContentButton);

        return rightPanel;
    };

    EditionFactory.prototype.makeMiddlePanel = function (bookInfo) {
        var middlePanel = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        var middlePanelHeading = document.createElement('div');
        $(middlePanelHeading).addClass('heading');
        middlePanelHeading.innerHTML = bookInfo.Name;
        middlePanel.appendChild(middlePanelHeading);
        var middlePanelBody = document.createElement('div');
        $(middlePanelBody).addClass('body');
        middlePanelBody.innerHTML = bookInfo.Body;
        middlePanel.appendChild(middlePanelBody);
        return middlePanel;
    };

    EditionFactory.prototype.makeBottomPanel = function (bookInfo) {
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
    };
    return EditionFactory;
})();

var BibliographyFactoryResolver = (function () {
    function BibliographyFactoryResolver() {
        if (BibliographyFactoryResolver._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        BibliographyFactoryResolver._instance = this;
        this.m_factories = new Array();
        this.m_factories['Edition'] = new EditionFactory(); //TODO make enum bookType
        this.m_factories['Dictionary'] = new DictionaryFactory();
        this.m_factories['OldCzechTextBank'] = new OldCzechTextBankFactory();
    }
    BibliographyFactoryResolver.getInstance = function () {
        if (BibliographyFactoryResolver._instance === null) {
            BibliographyFactoryResolver._instance = new BibliographyFactoryResolver();
        }
        return BibliographyFactoryResolver._instance;
    };

    BibliographyFactoryResolver.prototype.getFactoryForType = function (bookType) {
        return this.m_factories[bookType];
    };
    BibliographyFactoryResolver._instance = null;
    return BibliographyFactoryResolver;
})();

var TableBuilder = (function () {
    function TableBuilder() {
        this.m_tableDiv = document.createElement('div');
        $(this.m_tableDiv).addClass('table');
    }
    TableBuilder.prototype.makeTableRow = function (label, value) {
        var rowDiv = document.createElement('div');
        $(rowDiv).addClass('row');
        var labelDiv = document.createElement('div');
        $(labelDiv).addClass('cell label');
        labelDiv.innerHTML = label;
        rowDiv.appendChild(labelDiv);
        var valueDiv = document.createElement('div');
        $(valueDiv).addClass('cell');
        if (!value || value.length === 0) {
            valueDiv.innerHTML = "&lt;nezadáno&gt;";
        } else {
            valueDiv.innerHTML = value;
        }
        rowDiv.appendChild(valueDiv);
        this.m_tableDiv.appendChild(rowDiv);
    };

    TableBuilder.prototype.build = function () {
        return this.m_tableDiv;
    };
    return TableBuilder;
})();
//# sourceMappingURL=itjakub.plugins.bibliography.js.map
