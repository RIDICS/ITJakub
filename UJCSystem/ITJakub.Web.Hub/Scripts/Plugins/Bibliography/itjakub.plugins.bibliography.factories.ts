class BibliographyFactoryResolver {
    private factories: BibliographyFactory[];


    constructor(booksConfigurations: Object) {
        this.factories = new Array();
        this.factories[BookTypeEnum.Edition] = new EditionFactory(new BookTypeConfiguration(BookTypeEnum.Edition, booksConfigurations["Edition"])); //TODO make enum bookType, BookTypeConfiguration should make config manager
        this.factories[BookTypeEnum.Dictionary] = new DictionaryFactory(new BookTypeConfiguration(BookTypeEnum.Dictionary, booksConfigurations["Dictionary"]));
        this.factories[BookTypeEnum.TextBank] = new TextBankFactory(new BookTypeConfiguration(BookTypeEnum.TextBank, booksConfigurations["TextBank"]));
        this.factories[BookTypeEnum.CardFile] = new CardFileFactory(new BookTypeConfiguration(BookTypeEnum.CardFile, booksConfigurations["CardFile"]));
        //this.factories['Default'] = new BibliographyFactory(new BookTypeConfiguration("Default", booksConfigurations["Default"]));

    }

    public getFactoryForType(bookType: BookTypeEnum): BibliographyFactory {
        if (typeof this.factories[bookType]!== 'undefined'){
            return this.factories[bookType];
        }
        //return this.factories['Default'];
        return null;
    }


}

class BibliographyFactory {
    configuration: BookTypeConfiguration;

    constructor(configuration) {
        this.configuration = configuration;
    }

    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');
        return leftPanel;
    }

    makeRightPanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsRightPanel()) return null;
        var config = this.configuration.getRightPanelConfig();

        var rightPanel: HTMLDivElement = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        if (config.containsReadButton()) {
            var bookButton: HTMLButtonElement = document.createElement('button');
            bookButton.type = 'button';
            $(bookButton).addClass('btn btn-sm book-button');
            var spanBook: HTMLSpanElement = document.createElement('span');
            $(spanBook).addClass('glyphicon glyphicon-book');
            bookButton.appendChild(spanBook);
            $(bookButton).click((event) => {
                var buttonScript = config.getReadButtonOnClick(bookInfo);
                if (typeof buttonScript !== "undefined" && buttonScript != null && buttonScript !== "") {
                    eval(buttonScript);
                } else {
                    window.location.href = config.getReadButtonUrl(bookInfo);
                }
            });
            rightPanel.appendChild(bookButton);
        }

        if (config.containsInfoButton()) {
            var infoButton: HTMLButtonElement = document.createElement('button');
            infoButton.type = 'button';
            $(infoButton).addClass('btn btn-sm information-button');
            var spanInfo: HTMLSpanElement = document.createElement('span');
            $(spanInfo).addClass('glyphicon glyphicon-info-sign');
            infoButton.appendChild(spanInfo);
            $(infoButton).click((event) => {
                var buttonScript = config.getInfoButtonOnClick(bookInfo);
                if (typeof buttonScript !== "undefined" && buttonScript != null && buttonScript !== "") {
                    eval(buttonScript);
                } else {
                    window.location.href = config.getInfoButtonUrl(bookInfo);
                }
            });
            rightPanel.appendChild(infoButton);
        }

        if (this.configuration.containsBottomPanel()) {
            var contentButton: HTMLButtonElement = document.createElement('button');
            contentButton.type = 'button';
            $(contentButton).addClass('btn btn-sm content-button');
            var spanChevrDown: HTMLSpanElement = document.createElement('span');
            $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
            contentButton.appendChild(spanChevrDown);
            $(contentButton).click(function(event) {
                $(this).parents('li.list-item').first().find('.hidden-content').slideToggle("slow");
                $(this).children('span').toggleClass('glyphicon-chevron-down glyphicon-chevron-up');
            });
            rightPanel.appendChild(contentButton);
        }

        return rightPanel;
    }

    makeMiddlePanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsMiddlePanel()) return null;
        var config = this.configuration.getMidllePanelConfig();

        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');

        if (config.containsShortInfo()) {
            var middlePanelShortInfo: HTMLDivElement = document.createElement('div');
            $(middlePanelShortInfo).addClass('short-info');
            middlePanelShortInfo.innerHTML = config.getShortInfo(bookInfo);
            middlePanel.appendChild(middlePanelShortInfo);
        }

        if (config.containsTitle()) {
            var middlePanelHeading: HTMLDivElement = document.createElement('div');
            $(middlePanelHeading).addClass('heading');
            middlePanelHeading.innerHTML = config.getTitle(bookInfo);
            middlePanel.appendChild(middlePanelHeading);
        }
        if (config.containsBody()) {
            var middlePanelBody: HTMLDivElement = document.createElement('div');
            $(middlePanelBody).addClass('body');
            middlePanelBody.innerHTML = config.getBody(bookInfo);
            middlePanel.appendChild(middlePanelBody);
        }

        if (config.containsCustom()) {
            var customDiv: HTMLDivElement = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = config.getCustom(bookInfo);
            middlePanel.appendChild(customDiv);
        }

        return middlePanel;
    }

    makeBottomPanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsBottomPanel()) return null;
        var config = this.configuration.getBottomPanelConfig();

        var bottomPanel: HTMLDivElement = document.createElement('div');
        $(bottomPanel).addClass('bottom-panel');

        if (config.containsBody()) {
            var bottomPanelBody: HTMLDivElement = document.createElement('div');
            $(bottomPanelBody).addClass('body');
            bottomPanelBody.innerHTML = config.getBody(bookInfo);
            bottomPanel.appendChild(bottomPanelBody);
        }

        if (config.containsCustom()) {
            var customDiv: HTMLDivElement = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = config.getCustom(bookInfo);
            bottomPanel.appendChild(customDiv);
        }

        return bottomPanel;
    }
}

class TextBankFactory extends BibliographyFactory {

    constructor(configuration: BookTypeConfiguration) {
        super(configuration);
    }
}

class DictionaryFactory extends BibliographyFactory {

    constructor(configuration: BookTypeConfiguration) {
        super(configuration);
    }

    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');

        var inputCheckbox: HTMLInputElement = document.createElement('input');
        inputCheckbox.type = "checkbox";
        $(inputCheckbox).addClass('checkbox');
        leftPanel.appendChild(inputCheckbox);

        var starEmptyButton: HTMLButtonElement = document.createElement('button');
        starEmptyButton.type = 'button';
        $(starEmptyButton).addClass('btn btn-xs star-empty-button');
        var spanEmptyStar: HTMLSpanElement = document.createElement('span');
        $(spanEmptyStar).addClass('glyphicon glyphicon-star-empty');
        starEmptyButton.appendChild(spanEmptyStar);
        $(starEmptyButton).click(function(event) {
            $(this).siblings('.star-button').show();
            $(this).hide();
        }); //TODO fill click action
        leftPanel.appendChild(starEmptyButton);

        var starButton: HTMLButtonElement = document.createElement('button');
        starButton.type = 'button';
        $(starButton).addClass('btn btn-xs star-button');
        $(starButton).css('display', 'none');
        var spanStar: HTMLSpanElement = document.createElement('span');
        $(spanStar).addClass('glyphicon glyphicon-star');
        starButton.appendChild(spanStar);
        $(starButton).click(function(event) {
            $(this).siblings('.star-empty-button').show();
            $(this).hide();
        }); //TODO fill click action
        leftPanel.appendChild(starButton);

        return leftPanel;
    }
}

class CardFileFactory extends BibliographyFactory {

    constructor(configuration: BookTypeConfiguration) {
        super(configuration);
    }

    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');

        var inputCheckbox: HTMLInputElement = document.createElement('input');
        inputCheckbox.type = "checkbox";
        $(inputCheckbox).addClass('checkbox');
        leftPanel.appendChild(inputCheckbox);

        var starEmptyButton: HTMLButtonElement = document.createElement('button');
        starEmptyButton.type = 'button';
        $(starEmptyButton).addClass('btn btn-xs star-empty-button');
        var spanEmptyStar: HTMLSpanElement = document.createElement('span');
        $(spanEmptyStar).addClass('glyphicon glyphicon-star-empty');
        starEmptyButton.appendChild(spanEmptyStar);
        $(starEmptyButton).click(function(event) {
            $(this).siblings('.star-button').show();
            $(this).hide();
        }); //TODO fill click action
        leftPanel.appendChild(starEmptyButton);

        var starButton: HTMLButtonElement = document.createElement('button');
        starButton.type = 'button';
        $(starButton).addClass('btn btn-xs star-button');
        $(starButton).css('display', 'none');
        var spanStar: HTMLSpanElement = document.createElement('span');
        $(spanStar).addClass('glyphicon glyphicon-star');
        starButton.appendChild(spanStar);
        $(starButton).click(function(event) {
            $(this).siblings('.star-empty-button').show();
            $(this).hide();
        }); //TODO fill click action
        leftPanel.appendChild(starButton);

        return leftPanel;
    }
}

class EditionFactory extends BibliographyFactory {


    constructor(configuration: BookTypeConfiguration) {
        super(configuration);
    }

}