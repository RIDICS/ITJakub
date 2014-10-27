class BibliographyFactoryResolver {
    private m_factories: BibliographyFactory[];


    constructor(booksConfigurations: Object) {
        this.m_factories = new Array();
        this.m_factories['Edition'] = new EditionFactory(new BookTypeConfiguration("Edition", booksConfigurations["Edition"])); //TODO make enum bookType, BookTypeConfiguration should make config manager
        this.m_factories['Dictionary'] = new DictionaryFactory(new BookTypeConfiguration("Dictionary", booksConfigurations["Dictionary"]));
        this.m_factories['OldCzechTextBank'] = new OldCzechTextBankFactory(new BookTypeConfiguration("OldCzechTextBank", booksConfigurations["OldCzechTextBank"]));
        this.m_factories['CardFile'] = new CardFileFactory(new BookTypeConfiguration("CardFile", booksConfigurations["CardFile"]));

    }

    public getFactoryForType(bookType: string): BibliographyFactory {
        return this.m_factories[bookType];
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
                window.location.href = config.getReadButton(bookInfo);
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
                window.location.href = config.getInfoButton(bookInfo);
            });
            rightPanel.appendChild(infoButton);
        }

        if (this.configuration.containsBottomPanel()) {
            var showContentButton: HTMLButtonElement = document.createElement('button');
            showContentButton.type = 'button';
            $(showContentButton).addClass('btn btn-sm show-button');
            var spanChevrDown: HTMLSpanElement = document.createElement('span');
            $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
            showContentButton.appendChild(spanChevrDown);
            $(showContentButton).click(function(event) {
                $(this).parents('li.list-item').first().find('.hidden-content').show("slow");
                $(this).siblings('.hide-button').show();
                $(this).hide();
            });
            rightPanel.appendChild(showContentButton);

            var hideContentButton: HTMLButtonElement = document.createElement('button');
            hideContentButton.type = 'button';
            $(hideContentButton).addClass('btn btn-sm hide-button');
            var spanChevrUp: HTMLSpanElement = document.createElement('span');
            $(spanChevrUp).addClass('glyphicon glyphicon-chevron-up');
            hideContentButton.appendChild(spanChevrUp);
            $(hideContentButton).click(function(event) {
                $(this).parents('li.list-item').first().find('.hidden-content').hide("slow");
                $(this).siblings('.show-button').show();
                $(this).hide();
            });
            rightPanel.appendChild(hideContentButton);
        }

        return rightPanel;
    }

    makeMiddlePanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsMiddlePanel()) return null;
        var config = this.configuration.getMidllePanelConfig();

        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');


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

class OldCzechTextBankFactory extends BibliographyFactory {

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