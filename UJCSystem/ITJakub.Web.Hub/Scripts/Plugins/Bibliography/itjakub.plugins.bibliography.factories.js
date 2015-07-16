var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var BibliographyFactoryResolver = (function () {
    function BibliographyFactoryResolver(booksConfigurations) {
        this.factories = new Array();
        this.factories[0 /* Edition */] = new EditionFactory(new BookTypeConfiguration("Edition", booksConfigurations["Edition"])); //TODO make enum bookType, BookTypeConfiguration should make config manager
        this.factories[1 /* Dictionary */] = new DictionaryFactory(new BookTypeConfiguration("Dictionary", booksConfigurations["Dictionary"]));
        this.factories[4 /* TextBank */] = new TextBankFactory(new BookTypeConfiguration("TextBank", booksConfigurations["TextBank"]));
        this.factories[6 /* CardFile */] = new CardFileFactory(new BookTypeConfiguration("CardFile", booksConfigurations["CardFile"]));
        this.factories['Default'] = new BibliographyFactory(new BookTypeConfiguration("Default", booksConfigurations["Default"]));
    }
    BibliographyFactoryResolver.prototype.getFactoryForType = function (bookType) {
        if (typeof this.factories[bookType] !== 'undefined') {
            return this.factories[bookType];
        }
        return this.factories['Default'];
    };
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
        if (!this.configuration.containsRightPanel())
            return null;
        var config = this.configuration.getRightPanelConfig();
        var rightPanel = document.createElement('div');
        $(rightPanel).addClass('right-panel');
        if (config.containsReadButton()) {
            var bookButton = document.createElement('button');
            bookButton.type = 'button';
            $(bookButton).addClass('btn btn-sm book-button');
            var spanBook = document.createElement('span');
            $(spanBook).addClass('glyphicon glyphicon-book');
            bookButton.appendChild(spanBook);
            $(bookButton).click(function (event) {
                window.location.href = config.getReadButton(bookInfo);
            });
            rightPanel.appendChild(bookButton);
        }
        if (config.containsInfoButton()) {
            var infoButton = document.createElement('button');
            infoButton.type = 'button';
            $(infoButton).addClass('btn btn-sm information-button');
            var spanInfo = document.createElement('span');
            $(spanInfo).addClass('glyphicon glyphicon-info-sign');
            infoButton.appendChild(spanInfo);
            $(infoButton).click(function (event) {
                window.location.href = config.getInfoButton(bookInfo);
            });
            rightPanel.appendChild(infoButton);
        }
        if (this.configuration.containsBottomPanel()) {
            var contentButton = document.createElement('button');
            contentButton.type = 'button';
            $(contentButton).addClass('btn btn-sm content-button');
            var spanChevrDown = document.createElement('span');
            $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
            contentButton.appendChild(spanChevrDown);
            $(contentButton).click(function (event) {
                $(this).parents('li.list-item').first().find('.hidden-content').slideToggle("slow");
                $(this).children('span').toggleClass('glyphicon-chevron-down glyphicon-chevron-up');
            });
            rightPanel.appendChild(contentButton);
        }
        return rightPanel;
    };
    BibliographyFactory.prototype.makeMiddlePanel = function (bookInfo) {
        if (!this.configuration.containsMiddlePanel())
            return null;
        var config = this.configuration.getMidllePanelConfig();
        var middlePanel = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        if (config.containsShortInfo()) {
            var middlePanelShortInfo = document.createElement('div');
            $(middlePanelShortInfo).addClass('short-info');
            middlePanelShortInfo.innerHTML = config.getShortInfo(bookInfo);
            middlePanel.appendChild(middlePanelShortInfo);
        }
        if (config.containsTitle()) {
            var middlePanelHeading = document.createElement('div');
            $(middlePanelHeading).addClass('heading');
            middlePanelHeading.innerHTML = config.getTitle(bookInfo);
            middlePanel.appendChild(middlePanelHeading);
        }
        if (config.containsBody()) {
            var middlePanelBody = document.createElement('div');
            $(middlePanelBody).addClass('body');
            middlePanelBody.innerHTML = config.getBody(bookInfo);
            middlePanel.appendChild(middlePanelBody);
        }
        if (config.containsCustom()) {
            var customDiv = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = config.getCustom(bookInfo);
            middlePanel.appendChild(customDiv);
        }
        return middlePanel;
    };
    BibliographyFactory.prototype.makeBottomPanel = function (bookInfo) {
        if (!this.configuration.containsBottomPanel())
            return null;
        var config = this.configuration.getBottomPanelConfig();
        var bottomPanel = document.createElement('div');
        $(bottomPanel).addClass('bottom-panel');
        if (config.containsBody()) {
            var bottomPanelBody = document.createElement('div');
            $(bottomPanelBody).addClass('body');
            bottomPanelBody.innerHTML = config.getBody(bookInfo);
            bottomPanel.appendChild(bottomPanelBody);
        }
        if (config.containsCustom()) {
            var customDiv = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = config.getCustom(bookInfo);
            bottomPanel.appendChild(customDiv);
        }
        return bottomPanel;
    };
    return BibliographyFactory;
})();
var TextBankFactory = (function (_super) {
    __extends(TextBankFactory, _super);
    function TextBankFactory(configuration) {
        _super.call(this, configuration);
    }
    return TextBankFactory;
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