var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
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
        this.m_factories['CardFile'] = new CardFileFactory(new ConfigurationManager(configObj["CardFile"]));
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
        var _this = this;
        var rightPanel = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        if (this.configuration.containsReadButtonInRightPanel()) {
            var bookButton = document.createElement('button');
            bookButton.type = 'button';
            $(bookButton).addClass('btn btn-sm book-button');
            var spanBook = document.createElement('span');
            $(spanBook).addClass('glyphicon glyphicon-book');
            bookButton.appendChild(spanBook);
            $(bookButton).click(function (event) {
                window.location.href = _this.configuration.getRightPanelBookButton(bookInfo);
            });
            rightPanel.appendChild(bookButton);
        }

        if (this.configuration.containsInfoButtonInRightPanel()) {
            var infoButton = document.createElement('button');
            infoButton.type = 'button';
            $(infoButton).addClass('btn btn-sm information-button');
            var spanInfo = document.createElement('span');
            $(spanInfo).addClass('glyphicon glyphicon-info-sign');
            infoButton.appendChild(spanInfo);
            $(infoButton).click(function (event) {
                window.location.href = _this.configuration.getRightPanelInfoButton(bookInfo);
            });
            rightPanel.appendChild(infoButton);
        }

        if (this.configuration.containsBottomPanel()) {
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
        }

        return rightPanel;
    };

    BibliographyFactory.prototype.makeMiddlePanel = function (bookInfo) {
        if (!this.configuration.containsMiddlePanel())
            return null;

        var middlePanel = document.createElement('div');
        $(middlePanel).addClass('middle-panel');

        if (this.configuration.containsMiddlePanelTitle()) {
            var middlePanelHeading = document.createElement('div');
            $(middlePanelHeading).addClass('heading');
            middlePanelHeading.innerHTML = this.configuration.getTitle(bookInfo);
            middlePanel.appendChild(middlePanelHeading);
        }
        if (this.configuration.containsMiddlePanelBody()) {
            var middlePanelBody = document.createElement('div');
            $(middlePanelBody).addClass('body');
            middlePanelBody.innerHTML = this.configuration.getMiddlePanelBody(bookInfo);
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
        $(bottomPanel).addClass('bottom-panel');

        if (this.configuration.containsBottomPanelBody()) {
            var bottomPanelBody = document.createElement('div');
            $(bottomPanelBody).addClass('body');
            bottomPanelBody.innerHTML = this.configuration.getBottomPanelBody(bookInfo);
            bottomPanel.appendChild(bottomPanelBody);
        }

        if (this.configuration.containsCustomInBottomPanel()) {
            var customDiv = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = this.configuration.getCustomInBottomPanel(bookInfo);
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
    return DictionaryFactory;
})(BibliographyFactory);

var CardFileFactory = (function (_super) {
    __extends(CardFileFactory, _super);
    function CardFileFactory(configuration) {
        _super.call(this, configuration);
    }
    CardFileFactory.prototype.makeLeftPanel = function (bookInfo) {
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
    return CardFileFactory;
})(BibliographyFactory);

var EditionFactory = (function (_super) {
    __extends(EditionFactory, _super);
    function EditionFactory(configuration) {
        _super.call(this, configuration);
    }
    return EditionFactory;
})(BibliographyFactory);
//# sourceMappingURL=itjakub.plugins.bibliography.factories.js.map
