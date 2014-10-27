class BibliographyFactoryResolver {
    private static _instance: BibliographyFactoryResolver = null;
    private m_factories: BibliographyFactory[];


    constructor() {
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
            success: (response) => {
                configObj = response;
            }
        });

        this.m_factories = new Array();
        this.m_factories['Edition'] = new EditionFactory(new ConfigurationManager(configObj["Edition"])); //TODO make enum bookType
        this.m_factories['Dictionary'] = new DictionaryFactory(new ConfigurationManager(configObj["Dictionary"]));
        this.m_factories['OldCzechTextBank'] = new OldCzechTextBankFactory(new ConfigurationManager(configObj["OldCzechTextBank"]));
        this.m_factories['CardFile'] = new CardFileFactory(new ConfigurationManager(configObj["CardFile"]));

    }

    public static getInstance(): BibliographyFactoryResolver {
        if (BibliographyFactoryResolver._instance === null) {
            BibliographyFactoryResolver._instance = new BibliographyFactoryResolver();
        }
        return BibliographyFactoryResolver._instance;
    }

    public getFactoryForType(bookType: string): BibliographyFactory {
        return this.m_factories[bookType];
    }


}

class BibliographyFactory {
    configuration: ConfigurationManager;

    constructor(configuration) {
        this.configuration = configuration;
    }

    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');
        return leftPanel;
    }

    makeRightPanel(bookInfo: IBookInfo): HTMLDivElement {
        var rightPanel: HTMLDivElement = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        if (this.configuration.containsReadButtonInRightPanel()) {
            var bookButton: HTMLButtonElement = document.createElement('button');
            bookButton.type = 'button';
            $(bookButton).addClass('btn btn-sm book-button');
            var spanBook: HTMLSpanElement = document.createElement('span');
            $(spanBook).addClass('glyphicon glyphicon-book');
            bookButton.appendChild(spanBook);
            $(bookButton).click((event) => {
                window.location.href = this.configuration.getRightPanelBookButton(bookInfo);
            });
            rightPanel.appendChild(bookButton);
        }

        if (this.configuration.containsInfoButtonInRightPanel()) {
            var infoButton: HTMLButtonElement = document.createElement('button');
            infoButton.type = 'button';
            $(infoButton).addClass('btn btn-sm information-button');
            var spanInfo: HTMLSpanElement = document.createElement('span');
            $(spanInfo).addClass('glyphicon glyphicon-info-sign');
            infoButton.appendChild(spanInfo);
            $(infoButton).click((event) => {
                window.location.href = this.configuration.getRightPanelInfoButton(bookInfo);
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

        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');


        if (this.configuration.containsMiddlePanelTitle()) {
            var middlePanelHeading: HTMLDivElement = document.createElement('div');
            $(middlePanelHeading).addClass('heading');
            middlePanelHeading.innerHTML = this.configuration.getTitle(bookInfo);
            middlePanel.appendChild(middlePanelHeading);
        }
        if (this.configuration.containsMiddlePanelBody()) {
            var middlePanelBody: HTMLDivElement = document.createElement('div');
            $(middlePanelBody).addClass('body');
            middlePanelBody.innerHTML = this.configuration.getMiddlePanelBody(bookInfo);
            middlePanel.appendChild(middlePanelBody);
        }

        if (this.configuration.containsCustomInMiddlePanel()) {
            var customDiv: HTMLDivElement = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = this.configuration.getCustomInMiddlePanel(bookInfo);
            middlePanel.appendChild(customDiv);
        }

        return middlePanel;
    }

    makeBottomPanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsBottomPanel()) return null;

        var bottomPanel: HTMLDivElement = document.createElement('div');
        $(bottomPanel).addClass('bottom-panel');

        if (this.configuration.containsBottomPanelBody()) {
            var bottomPanelBody: HTMLDivElement = document.createElement('div');
            $(bottomPanelBody).addClass('body');
            bottomPanelBody.innerHTML = this.configuration.getBottomPanelBody(bookInfo);
            bottomPanel.appendChild(bottomPanelBody);
        }

        if (this.configuration.containsCustomInBottomPanel()) {
            var customDiv: HTMLDivElement = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = this.configuration.getCustomInBottomPanel(bookInfo);
            bottomPanel.appendChild(customDiv);
        }

        return bottomPanel;
    }
}

class OldCzechTextBankFactory extends BibliographyFactory {

    constructor(configuration: ConfigurationManager) {
        super(configuration);
    }
}

class DictionaryFactory extends BibliographyFactory {

    constructor(configuration: ConfigurationManager) {
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

    constructor(configuration: ConfigurationManager) {
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


    constructor(configuration: ConfigurationManager) {
        super(configuration);
    }

}