var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
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

var BibliographyFactory = (function () {
    function BibliographyFactory(configuration) {
        this.configuration = configuration;
    }
    BibliographyFactory.prototype.makeLeftPanel = function (bookInfo) {
        var leftPanel = document.createElement('div');
        $(leftPanel).addClass('left-panel');
        return leftPanel;
    };

    BibliographyFactory.prototype.makeRightPanel = function (bookInfo) {
        var rightPanel = document.createElement('div');
        $(rightPanel).addClass('right-panel');
        return rightPanel;
    };

    BibliographyFactory.prototype.makeMiddlePanel = function (bookInfo) {
        if (!this.configuration.containsMiddlePanel())
            return null;

        var middlePanel = document.createElement('div');
        $(middlePanel).addClass('middle-panel');

        if (this.configuration.containsTitle()) {
            var middlePanelHeading = document.createElement('div');
            $(middlePanelHeading).addClass('heading');
            middlePanelHeading.innerHTML = this.configuration.getTitle(bookInfo);
            middlePanel.appendChild(middlePanelHeading);
        }
        if (this.configuration.containsBody()) {
            var middlePanelBody = document.createElement('div');
            $(middlePanelBody).addClass('body');
            middlePanelBody.innerHTML = this.configuration.getBody(bookInfo);
            middlePanel.appendChild(middlePanelBody);
        }

        if (this.configuration.containsCustomInMiddlePanel()) {
            var customDiv = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = this.configuration.getCustomInMiddlePanel(bookInfo);
            middlePanel.appendChild(customDiv);
        }

        return middlePanel;
    };

    BibliographyFactory.prototype.makeBottomPanel = function (bookInfo) {
        if (!this.configuration.containsBottomPanel())
            return null;

        var bottomPanel = document.createElement('div');

        if (this.configuration.containsCustomInBottomPanel()) {
            var customDiv = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = this.configuration.getCustomInMiddlePanel(bookInfo);
            bottomPanel.appendChild(customDiv);
        }

        return bottomPanel;
    };
    return BibliographyFactory;
})();

var OldCzechTextBankFactory = (function (_super) {
    __extends(OldCzechTextBankFactory, _super);
    function OldCzechTextBankFactory(configuration) {
        _super.call(this, configuration);
    }
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
})(BibliographyFactory);

var DictionaryFactory = (function (_super) {
    __extends(DictionaryFactory, _super);
    function DictionaryFactory(configuration) {
        _super.call(this, configuration);
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
    return DictionaryFactory;
})(BibliographyFactory);

var EditionFactory = (function (_super) {
    __extends(EditionFactory, _super);
    function EditionFactory(configuration) {
        _super.call(this, configuration);
    }
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

    //makeMiddlePanel(bookInfo: IBookInfo): HTMLDivElement {
    //    var middlePanel: HTMLDivElement = document.createElement('div');
    //    $(middlePanel).addClass('middle-panel');
    //    var middlePanelHeading: HTMLDivElement = document.createElement('div');
    //    $(middlePanelHeading).addClass('heading');
    //    middlePanelHeading.innerHTML = bookInfo.Name;
    //    middlePanel.appendChild(middlePanelHeading);
    //    var middlePanelBody: HTMLDivElement = document.createElement('div');
    //    $(middlePanelBody).addClass('body');
    //    middlePanelBody.innerHTML = bookInfo.Body;
    //    middlePanel.appendChild(middlePanelBody);
    //    return middlePanel;
    //}
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
})(BibliographyFactory);

var BibliographyFactoryResolver = (function () {
    function BibliographyFactoryResolver() {
        if (BibliographyFactoryResolver._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        BibliographyFactoryResolver._instance = this;
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

        this.m_factories = new Array();
        this.m_factories['Edition'] = new EditionFactory(new ConfigurationManager(configObj["Edition"])); //TODO make enum bookType
        this.m_factories['Dictionary'] = new DictionaryFactory(new ConfigurationManager(configObj["Dictionary"]));
        this.m_factories['OldCzechTextBank'] = new OldCzechTextBankFactory(new ConfigurationManager(configObj["OldCzechTextBank"]));
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

var ConfigurationManager = (function () {
    function ConfigurationManager(config) {
        this.config = config;
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

    ConfigurationManager.prototype.containsBody = function () {
        return typeof this.config["middle-panel"]['body'] !== 'undefined';
    };

    ConfigurationManager.prototype.containsTitle = function () {
        return typeof this.config["middle-panel"]['title'] !== 'undefined';
    };

    ConfigurationManager.prototype.getTitle = function (bibItem) {
        return this.replaceVarNamesByValues(this.config['middle-panel']['title'], bibItem);
    };

    ConfigurationManager.prototype.getBody = function (bibItem) {
        return this.replaceVarNamesByValues(this.config['middle-panel']['body'], bibItem);
    };

    ConfigurationManager.prototype.getCustomInMiddlePanel = function (bibItem) {
        return this.replaceVarNamesByValues(this.config['bottom-panel']['custom'], bibItem);
    };

    ConfigurationManager.prototype.replaceVarNamesByValues = function (valueString, bibItem) {
        return valueString.replace(/{(.+?)}/g, function (foundPattern, varName) {
            return bibItem[varName];
        });
    };
    return ConfigurationManager;
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
