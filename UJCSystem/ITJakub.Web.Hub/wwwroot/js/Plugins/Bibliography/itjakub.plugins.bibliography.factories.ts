﻿class BibliographyFactoryResolver {
    private factories: BibliographyFactory[]=[];
    
    constructor(protected booksConfigurations: Object, protected modulInicializator?: ModulInicializator) {}

    public getFactoryForType(bookType: BookTypeEnum): BibliographyFactory {
        if (this.factories[bookType] === undefined) {
            this.factories[bookType] = this.createFactory(bookType);
        }

        if (this.factories[bookType] !== undefined) {
            return this.factories[bookType];
        }

        return null;
    }

    protected createFactory(bookType: BookTypeEnum): BibliographyFactory {
        switch (bookType) {
            case BookTypeEnum.Edition:
                return new BasicFactory(new BookTypeConfiguration(BookTypeEnum.Edition, this.booksConfigurations["Edition"]), this.modulInicializator); //TODO make enum bookType, BookTypeConfiguration should make config manager
            case BookTypeEnum.Dictionary:
                return new DictionaryFactory(new BookTypeConfiguration(BookTypeEnum.Dictionary, this.booksConfigurations["Dictionary"]), this.modulInicializator);
            case BookTypeEnum.TextBank:
                return new BasicFactory(new BookTypeConfiguration(BookTypeEnum.TextBank, this.booksConfigurations["TextBank"]), this.modulInicializator);
            case BookTypeEnum.CardFile:
                return new CardFileFactory(new BookTypeConfiguration(BookTypeEnum.CardFile, this.booksConfigurations["CardFile"]), this.modulInicializator);
            case BookTypeEnum.Grammar:
                return new BasicFactory(new BookTypeConfiguration(BookTypeEnum.Grammar, this.booksConfigurations["Grammar"]), this.modulInicializator);
            case BookTypeEnum.BibliographicalItem:
                return new BasicFactory(new BookTypeConfiguration(BookTypeEnum.BibliographicalItem, this.booksConfigurations["BibliographicalItem"]), this.modulInicializator);
            case BookTypeEnum.ProfessionalLiterature:
                return new BasicFactory(new BookTypeConfiguration(BookTypeEnum.ProfessionalLiterature, this.booksConfigurations["ProfessionalLiterature"]), this.modulInicializator);
            case BookTypeEnum.AudioBook:
                return new BasicFactory(new BookTypeConfiguration(BookTypeEnum.AudioBook, this.booksConfigurations["AudioBook"]), this.modulInicializator);

            default:
                console.error(`Unknown book type: ${bookType}`);
                return null;
        }
    }
}

class BibliographyFactory {

    constructor(public configuration: BookTypeConfiguration, public modulInicializator?: ModulInicializator) {}

    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');
        return leftPanel;
    }

    protected runEvalResponse(event: JQuery.Event, callback: (context:Object)=>any) {
        callback({ search: this.modulInicializator.getSearch(), event: event });
    }
    
    makeRightPanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsRightPanel()) return null;
        var config = this.configuration.getRightPanelConfig();

        var rightPanel: HTMLDivElement = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var self = this;

        if (config.containsReadButton()) {
            var bookButton: HTMLAnchorElement = document.createElement('a');
            var $bookButton = $(bookButton);
            $bookButton.addClass('btn btn-sm book-button button');
            var spanBook: HTMLSpanElement = document.createElement('span');
            $(spanBook).addClass('glyphicon glyphicon-book');
            bookButton.appendChild(spanBook);
            $bookButton.attr("href", "#");
            $bookButton.click((event: JQuery.Event) => {
                var buttonScript = config.getReadButtonOnClick(bookInfo);
                var buttonScriptCallable = config.getReadButtonOnClickCallable(bookInfo);
                if (typeof buttonScript !== "undefined" && buttonScript != null && buttonScript !== "") {
                    eval(buttonScript);
                }
                else if (typeof buttonScriptCallable !== "undefined" && buttonScriptCallable != null && buttonScriptCallable !== "") {
                    this.runEvalResponse(event, eval(buttonScriptCallable));
                }
                else {
                    onClickHref(event, config.getReadButtonUrl(bookInfo));
                }
            });
            rightPanel.appendChild(bookButton);
        }

        if (config.containsInfoButton()) {
            var infoButton: HTMLAnchorElement = document.createElement('a');
            var $infoButton = $(infoButton);
            infoButton.type = 'button';
            $infoButton.addClass('btn btn-sm information-button button');
            var spanInfo: HTMLSpanElement = document.createElement('span');
            $(spanInfo).addClass('glyphicon glyphicon-info-sign');
            infoButton.appendChild(spanInfo);
            $infoButton.attr("href", "#");
            $infoButton.click((event: JQuery.Event) => {
                var buttonScript = config.getInfoButtonOnClick(bookInfo);
                var buttonScriptCallable = config.getInfoButtonOnClickCallable(bookInfo);
                if (typeof buttonScript !== "undefined" && buttonScript != null && buttonScript !== "") {
                    eval(buttonScript);
                }
                else if (typeof buttonScriptCallable !== "undefined" && buttonScriptCallable != null && buttonScriptCallable !== "") {
                    this.runEvalResponse(event, eval(buttonScriptCallable));
                }
                else {
                    onClickHref(event, config.getReadButtonUrl(bookInfo));
                }
            });
            rightPanel.appendChild(infoButton);
        }

        if (config.containsFavoriteButton()) {
            var favoriteButton = document.createElement("span");
            var $favoriteButton = $(favoriteButton);
            $favoriteButton.addClass("favorite-button");
            $favoriteButton.data("buttonClass", "btn btn-sm button");
            rightPanel.appendChild(favoriteButton);
        }

        if (this.configuration.containsBottomPanel()) {
            var contentButton: HTMLButtonElement = document.createElement('button');
            contentButton.type = 'button';
            $(contentButton).addClass('btn btn-sm content-button');
            var spanChevrDown: HTMLSpanElement = document.createElement('span');
            $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
            contentButton.appendChild(spanChevrDown);
            $(contentButton).click(function (event) {
                var $hiddenContent = $(this).parents('li.list-item').first().find('.hidden-content');
                $hiddenContent.slideToggle("slow");
                $(this).children('span').toggleClass('glyphicon-chevron-down glyphicon-chevron-up');

                if ($hiddenContent.hasClass("not-loaded")) {
                    self.makeBottomPanelAsync(bookInfo, config, $hiddenContent);
                }
            });
            rightPanel.appendChild(contentButton);
        }

        return rightPanel;
    }

    makeMiddlePanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsMiddlePanel()) return null;
        var config = this.configuration.getMiddlePanelConfig();

        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');

        if (config.containsFavorites()) {
            var middlePanelFavorites: HTMLDivElement = document.createElement('div');
            $(middlePanelFavorites).addClass('favorites');
            middlePanel.appendChild(middlePanelFavorites);
        }

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

    makeMidRightPanel(bookInfo: IBookInfo): HTMLDivElement {
        if (!this.configuration.containsMiddleRightPanel()) return null;
        var config = this.configuration.getMiddleRightPanelConfig();

        var midRightPanel: HTMLDivElement = document.createElement('div');
        $(midRightPanel).addClass('middle-right-panel');

        if (config.containsBody()) {
            var middlePanelBody: HTMLDivElement = document.createElement('div');
            $(middlePanelBody).addClass('body');
            middlePanelBody.innerHTML = config.getBody(bookInfo);
            midRightPanel.appendChild(middlePanelBody);
        }

        if (config.containsCustom()) {
            var customDiv: HTMLDivElement = document.createElement('div');
            $(customDiv).addClass('custom');
            customDiv.innerHTML = config.getCustom(bookInfo);
            midRightPanel.appendChild(customDiv);
        }

        return midRightPanel;
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

    makeFavoriteBookInfo(bookFavorites: IFavoriteBaseInfoWithLabel[]): HTMLElement[] {
        var resultList = new Array<HTMLElement>();
        var config = this.configuration.getMiddlePanelConfig();
        var maxFavLabels = bookFavorites.length;
        if (config.containsFavorites() && config.containsFavoritesMaxCount()) {
            maxFavLabels = config.getFavoritesMaxCount();
        }

        var max = maxFavLabels < bookFavorites.length ? maxFavLabels : bookFavorites.length;
        for (var i = 0; i < bookFavorites.length; i++) {
            var favoriteInfo = bookFavorites[i];
            var label = BibliographyFactory.makeFavoriteLabel(favoriteInfo.title, favoriteInfo.favoriteLabel.name, favoriteInfo.favoriteLabel.color);
            resultList.push(label);

            if (i >= max) {
                $(label).hide();
            }
        }

        if (bookFavorites.length > maxFavLabels) {
            var showAllLink = document.createElement("a");
            var showAllSpan = document.createElement("span");
            $(showAllLink)
                .attr("href", "#")
                .append(showAllSpan)
                .click(event => {
                    var $item = $(event.currentTarget as Node as Element);
                    $item.siblings().show();
                    $item.hide();
                });
            $(showAllSpan)
                .css("color", "black")
                .css("font-weight", "bold")
                .css("margin-left", "3px")
                .text("...")
                .attr("title", localization.translate("ShowAllAssignedLabels", "PluginsJs").value)
                .attr("data-toggle", "tooltip")
                .tooltip();
            resultList.push(showAllLink);
        }

        return resultList;
    }

    private makeBottomPanelAsync(bookInfo: IBookInfo, rightPanelConfig: RightPanelConfiguration, $container: JQuery) {
        if (!rightPanelConfig.containsLoadDetailUrl()) {
            var bottomPanel = this.makeBottomPanel(bookInfo);
            this.appendBottomPanel($container, bottomPanel);
            return;
        }

        var url = rightPanelConfig.getLoadDetailUrl(bookInfo);
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + url,
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                var bottomPanel = this.makeBottomPanel(response);
                this.appendBottomPanel($container, bottomPanel);
            }
        });
    }

    private appendBottomPanel($container: JQuery, bottomPanel: HTMLElement) {
        var height = $container.height();
        $container
            .finish()
            .empty()
            .removeClass("not-loaded")
            .append(bottomPanel);

        var newHeight = $container.height();
        $container
            .height(height)
            .animate({
                height: newHeight
            } as JQuery.PlainObject, null, null, () => {
                $container.css("height", "");
            });
    }

    static makeFavoriteLabel(favoriteTitle: string, labelName: string, labelColor: string): HTMLSpanElement {
        var colorData = FavoriteHelper.getDefaultLabelColorData(labelColor);
        var label = document.createElement("span");
        $(label)
            .addClass("label")
            .css("color", colorData.fontColor)
            .css("background-color", colorData.backgroundColor)
            .css("border-color", colorData.borderColor)
            .text(labelName)
            .attr("data-toggle", "tooltip")
            .attr("title", localization.translateFormat("SavedAs:", new Array<string>(favoriteTitle), "PluginsJs").value)
            .tooltip();
        return label;
    }

    static makeError(text: string): HTMLDivElement {
        var errorContainer = document.createElement("div");
        $(errorContainer).addClass("bib-alert");

        var errorAlert = document.createElement("div");
        $(errorAlert)
            .addClass("alert alert-danger")
            .text(text)
            .appendTo(errorContainer);

        return errorContainer;
    }
}

class BasicFactory extends BibliographyFactory {}

class DictionaryFactory extends BibliographyFactory {
    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');

        //var inputCheckbox: HTMLInputElement = document.createElement('input');
        //inputCheckbox.type = "checkbox";
        //$(inputCheckbox).addClass('checkbox');
        //leftPanel.appendChild(inputCheckbox);

        //var starEmptyButton: HTMLButtonElement = document.createElement('button');
        //starEmptyButton.type = 'button';
        //$(starEmptyButton).addClass('btn btn-xs star-empty-button');
        //var spanEmptyStar: HTMLSpanElement = document.createElement('span');
        //$(spanEmptyStar).addClass('glyphicon glyphicon-star-empty');
        //starEmptyButton.appendChild(spanEmptyStar);
        //$(starEmptyButton).click(function(event) {
        //    $(this).siblings('.star-button').show();
        //    $(this).hide();
        //}); //TODO fill click action
        //leftPanel.appendChild(starEmptyButton);

        //var starButton: HTMLButtonElement = document.createElement('button');
        //starButton.type = 'button';
        //$(starButton).addClass('btn btn-xs star-button');
        //$(starButton).css('display', 'none');
        //var spanStar: HTMLSpanElement = document.createElement('span');
        //$(spanStar).addClass('glyphicon glyphicon-star');
        //starButton.appendChild(spanStar);
        //$(starButton).click(function(event) {
        //    $(this).siblings('.star-empty-button').show();
        //    $(this).hide();
        //}); //TODO fill click action
        //leftPanel.appendChild(starButton);

        return leftPanel;
    }
}

class CardFileFactory extends BibliographyFactory {
    makeLeftPanel(bookInfo: IBookInfo): HTMLDivElement {
        var leftPanel: HTMLDivElement = document.createElement('div');
        $(leftPanel).addClass('left-panel');

        //var inputCheckbox: HTMLInputElement = document.createElement('input');
        //inputCheckbox.type = "checkbox";
        //$(inputCheckbox).addClass('checkbox');
        //leftPanel.appendChild(inputCheckbox);

        //var starEmptyButton: HTMLButtonElement = document.createElement('button');
        //starEmptyButton.type = 'button';
        //$(starEmptyButton).addClass('btn btn-xs star-empty-button');
        //var spanEmptyStar: HTMLSpanElement = document.createElement('span');
        //$(spanEmptyStar).addClass('glyphicon glyphicon-star-empty');
        //starEmptyButton.appendChild(spanEmptyStar);
        //$(starEmptyButton).click(function(event) {
        //    $(this).siblings('.star-button').show();
        //    $(this).hide();
        //}); //TODO fill click action
        //leftPanel.appendChild(starEmptyButton);

        //var starButton: HTMLButtonElement = document.createElement('button');
        //starButton.type = 'button';
        //$(starButton).addClass('btn btn-xs star-button');
        //$(starButton).css('display', 'none');
        //var spanStar: HTMLSpanElement = document.createElement('span');
        //$(spanStar).addClass('glyphicon glyphicon-star');
        //starButton.appendChild(spanStar);
        //$(starButton).click(function(event) {
        //    $(this).siblings('.star-empty-button').show();
        //    $(this).hide();
        //}); //TODO fill click action
        //leftPanel.appendChild(starButton);

        return leftPanel;
    }
}