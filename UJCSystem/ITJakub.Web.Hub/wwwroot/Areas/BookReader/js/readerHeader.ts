class BookHeader extends ReaderPagination{
    private parentReader: ReaderLayout;
    private bookId: string;
    private versionId: string;
    private sc: ServerCommunication;
    private bookTitle: string;

    constructor(parentReader: ReaderLayout, sc: ServerCommunication, headerDiv: HTMLDivElement, bookTitle: string) {
        super(headerDiv);
        this.parentReader = parentReader;
        this.sc = sc;
        this.bookId = parentReader.getBookId();
        this.versionId = parentReader.getVersionId();
        this.bookTitle = bookTitle;
        this.init((pageId, pageIndex, scrollTo) => {
            this.parentReader.notifyPanelsMovePage(pageIndex, scrollTo);
            this.parentReader.actualPageIndex = pageIndex;
            this.actualizeSlider(pageIndex);
            this.parentReader.pageChangedCallback(pageId);
        });

    }

    private actualizeSlider(pageIndex: number) {
        var slider = $(this.readerContainer).find(".slider");
        $(slider).slider().slider("value", pageIndex);
        $(slider).find(".ui-slider-handle").find(".tooltip-inner")
            .html(`${localization.translate("page", "BookReader").value}: ${this.pages[pageIndex].text}`);
    }


    public getInnerHtml(deviceType: Device): HTMLDivElement {
        var innerHtml: HTMLDivElement = null;
        if (deviceType === Device.Mobile) {
            innerHtml = this.createMobileHeaderDiv(this.bookTitle);
        } 
        if (deviceType === Device.Desktop) {
            innerHtml = this.createDesktopHeaderDiv(this.bookTitle);
        }
        return innerHtml;
    }

    private getEditionNote(addHeader: boolean): HTMLDivElement {
        var editionNoteDiv = document.createElement("div");
        $(editionNoteDiv).addClass("loading");
        $(editionNoteDiv).addClass("edition-note-wrapper");
        if (addHeader) {
            var editionNoteHeader = document.createElement("h3");
            $(editionNoteHeader).append(localization.translate("editionNote", "BookReader").value);
            $(editionNoteDiv).append(editionNoteHeader);
        }
        
        var editionNote: JQueryXHR = this.sc.getEditionNote(this.bookId);
        editionNote.done((response: { editionNote: string }) => {
            var editionNoteText = document.createElement("div");
            $(editionNoteText).addClass("edition-note-text");
            if (response.editionNote === "") {
                $(editionNoteText).append(localization.translate("noEditionNote", "BookReader").value);
            } else {
                $(editionNoteText).append(response.editionNote);
            }
            editionNoteDiv.appendChild(editionNoteText);
            $(editionNoteDiv).removeClass("loading");
        });
        editionNote.fail(() => {
            $(editionNoteDiv).append(localization.translate("failedToLoadNote", "BookReader").value);
            $(editionNoteDiv).removeClass("loading");
        });

        return editionNoteDiv;
    }

    private getBookDetail(): HTMLDivElement {
        var bookDetailDiv = document.createElement("div");
        $(bookDetailDiv).addClass("book-detail-wrapper");
        var bookDetailHeader = document.createElement("h3");
        $(bookDetailHeader).append(localization.translate("information", "BookReader").value);
        bookDetailDiv.appendChild(bookDetailHeader);

        var bookDetail: JQueryXHR = this.sc.getBookDetail(this.bookId);
        bookDetail.done((response) => {
            var detailData = response["detail"];
            var detailTable = new TableBuilder();
            var editors: string = "";
            for (var i = 0; i < detailData.Editors.length; i++) {
                var editor = detailData.Editors[i];
                editors += editor.FirstName + " " + editor.LastName;
                if (i + 1 !== detailData.Editors.length) {
                    editors += ", ";
                }
            }

            detailTable.makeTableRow(localization.translate("editor", "BookReader").value, editors);
            detailTable.makeTableRow(localization.translate("original", "BookReader").value, detailData.LiteraryOriginal);
            detailTable.makeTableRow(localization.translate("relicAbbr", "BookReader").value, detailData.RelicAbbreviation);
            detailTable.makeTableRow(localization.translate("sourceAbbr", "BookReader").value, detailData.SourceAbbreviation);
            detailTable.makeTableRow(localization.translate("literaryType", "BookReader").value, detailData.LiteraryKinds);
            detailTable.makeTableRow(localization.translate("literaryGenre", "BookReader").value, detailData.LiteraryGenre);
            detailTable.makeTableRow(localization.translate("EditionLastModification", "BookReader").value, detailData.CreateTimeString);

            $(detailTable.build()).find(".bib-table-cell").each((index, element) => {
                if (element.innerHTML === "" || element.innerHTML === "undefined") {
                    element.innerHTML = `&lt;${localization.translate("nullValue", "BookReader").value}&gt;`;
                }
            });

            $(bookDetailDiv).append(detailTable.build());
            
            
        });
        bookDetail.fail(() => {
            $(bookDetailDiv).append(localization.translate("FailedToLoadInfo", "BookReader").value);
        });

        return bookDetailDiv;
    }

    private getAuthors(target: JQuery) {
        var authors: string = "";
        var bookDetail: JQueryXHR = this.sc.getBookDetail(this.bookId);
        bookDetail.done((response) => {
            var detailData = response["detail"];
            if (detailData.Authors.length !== 0) {

                
                for (var i = 0; i < detailData.Authors.length; i++) {
                    var author = detailData.Authors[i];
                    authors += author.FirstName + " " + author.LastName;
                    if (i + 1 !== detailData.Authors.length) {
                        authors += ", ";
                    }
                }
                authors += ": ";
                target.prepend(authors);
            }
        });
        bookDetail.fail(() => {
            target.prepend(`&lt;${localization.translate("failedToLoadAuthors", "BookReader").value}&gt;`);
        });
    }

    private makeToolButtons(deviceType: Device): HTMLDivElement {
        var toolButtonsDiv = document.createElement("div");
        $(toolButtonsDiv).addClass("tool-panel");

        var button = new ButtonFactory(this.parentReader, deviceType);
        var toolButtons: HTMLDivElement = document.createElement("div");
        $(toolButtons).addClass("buttons left");

        var addBookmarksButton = button.createAddBookmarkButton("bookmark",
            localization.translate("addBoookmark", "BookReader").value,
            this.parentReader.bookmarksPanelId
        );
        toolButtons.appendChild(addBookmarksButton);


        var bookmarksButton =
            button.createToolButton("bookmark",
                localization.translate(this.parentReader.bookmarksPanelId, "BookReader").value,
                this.parentReader.bookmarksPanelId);
        toolButtons.appendChild(bookmarksButton);

        var contentButton = button.createToolButton("book",
            localization.translate(this.parentReader.contentPanelId, "BookReader").value,
            this.parentReader.contentPanelId);
        toolButtons.appendChild(contentButton);

        var searchResultButton = button.createToolButton("search",
            localization.translate(this.parentReader.searchPanelId, "BookReader").value,
            this.parentReader.searchPanelId);
        toolButtons.appendChild(searchResultButton);

        var termsButton = button.createToolButton("list-alt",
            localization.translate(this.parentReader.termsPanelId, "BookReader").value,
            this.parentReader.termsPanelId);
        toolButtons.appendChild(termsButton);
        toolButtonsDiv.appendChild(toolButtons);
        if (deviceType === Device.Mobile) {
            $(toolButtonsDiv).addClass("buttons");
            var showPanelButton = button.createButton("display-panel", "wrench");
            $(showPanelButton).click(() => {
                if ($(showPanelButton.firstChild as Element).hasClass("glyphicon-chevron-left")) {
                    $(showPanelButton.firstChild as Element)
                        .removeClass("glyphicon-chevron-left")
                        .addClass("glyphicon-wrench");
                    $(toolButtonsDiv).animate({
                        "left": "-116.5"
                    } as JQuery.PlainObject);
                } else if ($(showPanelButton.firstChild as Element).hasClass("glyphicon-wrench")) {
                    $(showPanelButton.firstChild as Element)
                        .removeClass("glyphicon-wrench")
                        .addClass("glyphicon-chevron-left");
                    $(toolButtonsDiv).animate({
                        "left": "0"
                    } as JQuery.PlainObject);
                }
            });
            
            toolButtonsDiv.appendChild(showPanelButton);
        }
        return toolButtonsDiv;

    }

    private makeViewButtons(deviceType: Device): HTMLDivElement {
        var viewControl: HTMLDivElement = document.createElement("div");
        $(viewControl).addClass("view-control");
        viewControl.id = "view";
        var buttonObject = new ButtonFactory(this.parentReader, deviceType);
        var viewButtons = document.createElement("div");
        $(viewButtons).addClass("buttons");
        var hasBookText: boolean = false;
        var hasBookImage: boolean = false;
        var hasBookPage: JQueryXHR = this.sc.hasBookPage(this.bookId, this.versionId);
        hasBookPage.done((response: { hasBookPage: boolean }) => {
            if (response.hasBookPage) {
                var textButton = buttonObject.createViewButton("font",
                    localization.translate(this.parentReader.textPanelId, "BookReader").value,
                    this.parentReader.textPanelId);
                hasBookText = true;
                if (!$(".lm_popin").is("div")) {
                    textButton.click();
                }
                $(".page-navigation-container-helper").removeClass("hidden");
                viewButtons.appendChild(textButton);

                var checkboxDiv = this.createCheckboxDiv();
                viewControl.appendChild(checkboxDiv);
            }
        });
        var hasBookImageAjax: JQueryXHR;
        $.when(hasBookPage).then(() => {
            hasBookImageAjax = this.sc.hasBookImage(this.bookId, this.versionId);

            hasBookImageAjax.done((response: { hasBookImage: boolean }) => {
                if (response.hasBookImage) {
                    var imageButton = buttonObject.createViewButton("picture",
                        localization.translate(this.parentReader.imagePanelId, "BookReader").value,
                        this.parentReader.imagePanelId);
                    hasBookImage = true;
                    if (!hasBookText && !$(".lm_popin").is("div")) {
                        imageButton.click();
                        $(".page-navigation-container-helper").removeClass("hidden");
                    }
                    viewButtons.appendChild(imageButton);
                }
            });    
        });

        $.when(hasBookPage, hasBookImageAjax).then(() => {
            var audioBook: JQueryXHR = this.sc.getAudioBook(this.bookId);
            audioBook.done((response: { audioBook: IAudioBookSearchResultContract }) => {
                if (response.audioBook.Tracks.length > 0) {
                    var audioButton = buttonObject.createViewButton("music",
                        localization.translate(this.parentReader.audioPanelId, "BookReader").value,
                        this.parentReader.audioPanelId);

                    if (!hasBookText && !hasBookImage && !$(".lm_popin").is("div")) {
                        audioButton.click();
                        $(".page-navigation-container-helper").removeClass("hidden");
                    }
                    viewButtons.appendChild(audioButton);
                }
            });    
        });

        if (deviceType === Device.Mobile) {
            var editionNoteButton = buttonObject.createButton("display-note", "comment");
            $(editionNoteButton)
                .attr("data-toggle", "modal")
                .attr("data-target", "#book-info-modal")
                .click(() => {
                    $("#modalHeader")
                        .empty()
                        .append(localization.translate("editionNote", "BookReader").value);

                    $("#modalBody")
                        .empty()
                        .append(this.getEditionNote(false));
                });
            var editionNoteLabel = document.createElement("span");
            $(editionNoteLabel).addClass("button-text");
            $(editionNoteLabel).append(localization.translate("editionNote", "BookReader").value);
            $(editionNoteButton).append(editionNoteLabel);
            viewButtons.appendChild(editionNoteButton);
        }
        

        viewControl.appendChild(viewButtons);
        return viewControl;
    }

    private createCheckboxDiv(): HTMLDivElement {
        var checkboxesDiv = window.document.createElement("div");
        $(checkboxesDiv).addClass("reader-settings-checkboxes-area");

        var showPageCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        var showPageNameCheckbox: HTMLInputElement = window.document.createElement("input");
        showPageNameCheckbox.type = "checkbox";

        $(showPageNameCheckbox).change((eventData) => {
            var readerText: JQuery = $("#" + this.parentReader.textPanelId).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget as Node);
            this.parentReader.readerLayout.eventHub.emit("toggleComments", currentTarget.checked, "reader-text-show-page-names");
        });

        var pageNameSlider = document.createElement("label");
        $(pageNameSlider).addClass("switch");

        var showPageNameLabel: HTMLLabelElement = window.document.createElement("label");
        showPageNameLabel.innerHTML = localization.translate("pageNames", "BookReader").value;
        showPageCheckboxDiv.appendChild(showPageNameCheckbox);
        showPageCheckboxDiv.appendChild(pageNameSlider);
        showPageCheckboxDiv.appendChild(showPageNameLabel);
        showPageNameCheckbox.id = "checkbox-show-page-numbers";
        showPageNameLabel.setAttribute("for", showPageNameCheckbox.id);
        pageNameSlider.setAttribute("for", showPageNameCheckbox.id);
        checkboxesDiv.appendChild(showPageCheckboxDiv);

        var showPageOnNewLineDiv: HTMLDivElement = window.document.createElement("div");
        var showPageOnNewLineCheckbox: HTMLInputElement = window.document.createElement("input");
        showPageOnNewLineCheckbox.type = "checkbox";

        $(showPageOnNewLineCheckbox).change((eventData) => {
            var readerText = $("#" + this.parentReader.textPanelId).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget as Node);
            this.parentReader.readerLayout.eventHub.emit("toggleComments", currentTarget.checked, "reader-text-page-new-line");
        });

        var pageOnNewLineSlider = document.createElement("label");
        $(pageOnNewLineSlider).addClass("switch");

        var showPageOnNewLineLabel: HTMLLabelElement = window.document.createElement("label");
        showPageOnNewLineLabel.innerHTML = localization.translate("wrapPages", "BookReader").value;
        showPageOnNewLineDiv.appendChild(showPageOnNewLineCheckbox);
        showPageOnNewLineDiv.appendChild(pageOnNewLineSlider);
        showPageOnNewLineDiv.appendChild(showPageOnNewLineLabel);
        showPageOnNewLineCheckbox.id = "checkbox-page-breaks";
        showPageOnNewLineLabel.setAttribute("for", showPageOnNewLineCheckbox.id);
        pageOnNewLineSlider.setAttribute("for", showPageOnNewLineCheckbox.id);
        checkboxesDiv.appendChild(showPageOnNewLineDiv);

        var showCommentCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        var showCommentCheckbox: HTMLInputElement = window.document.createElement("input");
        showCommentCheckbox.type = "checkbox";

        $(showCommentCheckbox).change((eventData) => {
            var readerText = $("#" + this.parentReader.textPanelId).find(".reader-text");
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget as Node);
            this.parentReader.readerLayout.eventHub.emit("toggleComments", currentTarget.checked, "show-notes");
            
        });

        var commentSlider = document.createElement("label");
        $(commentSlider).addClass("switch");

        var showCommentLabel: HTMLLabelElement = window.document.createElement("label");
        showCommentLabel.innerHTML = localization.translate("notes", "BookReader").value;
        showCommentCheckboxDiv.appendChild(showCommentCheckbox);
        showCommentCheckboxDiv.appendChild(commentSlider);
        showCommentCheckboxDiv.appendChild(showCommentLabel);
        showCommentCheckbox.id = "checkbox-show-comment";
        showCommentLabel.setAttribute("for", showCommentCheckbox.id);
        commentSlider.setAttribute("for", showCommentCheckbox.id);
        checkboxesDiv.appendChild(showCommentCheckboxDiv);

        return checkboxesDiv;
    }

    private createDesktopHeaderDiv(bookTitle: string): HTMLDivElement {
        var headerDiv = document.createElement("div");
        headerDiv.appendChild(this.informationDiv(bookTitle, Device.Desktop));

        var controlsDiv = document.createElement("div");
        $(controlsDiv).addClass("reader-controls content-container");
        controlsDiv.appendChild(this.makeSlider());
        controlsDiv.appendChild(this.makeViewButtons(Device.Desktop));
        controlsDiv.appendChild(this.makeToolButtons(Device.Desktop));
        controlsDiv.appendChild(this.createPagination(true, pageIndex => {
            this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", pageIndex);
        }, pageIndex => {
            this.parentReader.readerLayout.eventHub.emit("navigationClicked", pageIndex);
        }));
        headerDiv.appendChild(controlsDiv);

        return headerDiv;
    }

    private createMobileHeaderDiv(bookTitle: string): HTMLDivElement {
        var headerDiv = document.createElement("div");
        headerDiv.appendChild(this.informationDiv(bookTitle, Device.Mobile));

        var controlsDiv = document.createElement("div");
        $(controlsDiv).addClass("reader-controls content-container");
        controlsDiv.appendChild(this.makeViewButtons(Device.Mobile));
        controlsDiv.appendChild(this.createPagination(true, pageIndex => {
            this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", pageIndex);
        }, pageIndex => {
            this.parentReader.readerLayout.eventHub.emit("navigationClicked", pageIndex);
        }));
        controlsDiv.appendChild(this.makePageInput());
        controlsDiv.appendChild(this.makeToolButtons(Device.Mobile));
        headerDiv.appendChild(controlsDiv);

        return headerDiv;    
    }

    private makePageInput(): HTMLDivElement {
        var pageInputDiv: HTMLDivElement = document.createElement("div");
        pageInputDiv.classList.add("page-input");

        var pageInputText = document.createElement("input");
        pageInputText.setAttribute("type", "text");
        pageInputText.setAttribute("id", "pageInputText");
        pageInputText.setAttribute("placeholder", localization.translate("GoToPage", "BookReader").value);
        pageInputText.classList.add("page-input-text");
        pageInputDiv.appendChild(pageInputText);

        var pageInputButton = document.createElement("button");
        $(pageInputButton).addClass("btn btn-default page-input-button");

        var pageInputButtonSpan = document.createElement("span");
        $(pageInputButtonSpan).addClass("glyphicon glyphicon-arrow-right");
        $(pageInputButton).append(pageInputButtonSpan);

        $(pageInputButton).click(() => {
            var pageName = $("#pageInputText").val();
            var pageIndex: number = -1;
            for (var i = 0; i < this.pages.length; i++) {
                if (this.pages[i].text === pageName) {
                    pageIndex = i;
                    break;
                }
            }

            if (this.pages[pageIndex] !== undefined) {
                var page: BookPage = this.pages[pageIndex];
                this.parentReader.readerLayout.eventHub.emit("navigationClicked", page.pageId);
            }
            else {
                console.error("missing page " + pageIndex);
            }
        });

        pageInputDiv.appendChild(pageInputButton);

        $(pageInputText).keypress((event: any) => {
            var keyCode = event.which || event.keyCode;
            if (keyCode === 13) {     //13 = Enter
                $(pageInputButton).click();
                event.preventDefault();
                event.stopPropagation();
                return false;
            }
// ReSharper disable once NotAllPathsReturnValue
        });

        this.parentReader.activateTypeahead(pageInputText);

        return pageInputDiv;
    }


    private makeSlider(): HTMLDivElement {
        var slider: HTMLDivElement = document.createElement("div");
        $(slider).addClass("slider");
        $(slider).slider({
            min: 0,
            max: this.pages.length - 1,
            value: 0,
            start: (event) => {
                $(event.target as Node as HTMLElement).find(".ui-slider-handle").find(".slider-tip").show();
            },
            stop: (event) => {
                $(event.target as Node as HTMLElement).find(".ui-slider-handle").find(".slider-tip").fadeOut(1000);
            },
            slide: (event, ui) => {
                $(event.target as Node as HTMLElement).find(".ui-slider-handle").find(".slider-tip").stop(true, true);
                $(event.target as Node as HTMLElement).find(".ui-slider-handle").find(".slider-tip").show();
                if (this.pages[ui.value] !== undefined) {
                    $(event.target as Node as HTMLElement).find(".ui-slider-handle").find(".tooltip-inner").html("Strana: " + this.pages[ui.value].text);
                } else {
                    console.error("missing page " + ui.value);
                }
            },
            change: (event: Event, ui: JQueryUI.SliderUIParams) => {
                if (this.parentReader.actualPageIndex !== ui.value) {
                    this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", <any>ui.value);
                }
            }
        });

        var sliderTooltip: HTMLDivElement = document.createElement("div");
        sliderTooltip.classList.add("tooltip", "top", "slider-tip");
        var arrowTooltip: HTMLDivElement = document.createElement("div");
        arrowTooltip.classList.add("tooltip-arrow");
        sliderTooltip.appendChild(arrowTooltip);

        var innerTooltip: HTMLDivElement = document.createElement("div");
        $(innerTooltip).addClass("tooltip-inner");
        if (this.pages[0] !== undefined) {
            $(innerTooltip).html(`${localization.translate("page", "BookReader").value}: ${this.pages[0].text}`);
        }
        else {
            console.error("missing page " + 0);
        }
        sliderTooltip.appendChild(innerTooltip);
        $(sliderTooltip).hide();

        var sliderHandle: JQuery = $(slider).find(".ui-slider-handle");
        sliderHandle.append(sliderTooltip);
        sliderHandle.hover((event) => {
            $(event.target).find(".slider-tip").stop(true, true);
            $(event.target).find(".slider-tip").show();
        });
        sliderHandle.mouseout((event) => {
            $(event.target).find(".slider-tip").fadeOut(1000);
        });
        return slider;
    }

    private informationDiv(bookTitle: string, deviceType: Device): HTMLDivElement {
        var buttonObject = new ButtonFactory(this.parentReader, deviceType);
        var bookInfoDiv: HTMLDivElement = document.createElement("div");
        $(bookInfoDiv).addClass("book-details");
        
        var editionNoteDiv = this.getEditionNote(true);
        
        var title = document.createElement("span");
        $(title).addClass("title");
        
        var bookHeaderButtons = document.createElement("div");
        $(bookHeaderButtons).addClass("book-details-buttons");
                
        bookInfoDiv.appendChild(title);
        if (deviceType === Device.Desktop) {
            $(title).text(bookTitle);
            this.getAuthors($(title));
            
            var detailsButton = buttonObject.createButton("more", "info-sign");
            $(detailsButton).click((event) => {
                var target: JQuery = $(event.target as Node as HTMLInputElement);

                var title = target.parents(".book-details").find(".title");
                title.toggleClass("full");

                var details = target.parents(".book-details").find(".collapsible-div");
                if (!details.is(":visible")) {
                    $(target).removeClass("glyphicon-info-sign");
                    $(target).addClass("glyphicon-chevron-up");
                    details.collapse("show");
                } else {
                    $(target).removeClass("glyphicon-chevron-up");
                    $(target).addClass("glyphicon-info-sign");
                    details.collapse("hide");
                }
            });
            bookHeaderButtons.appendChild(detailsButton);  

        }

        if (deviceType === Device.Mobile) {
            var bookDetailDiv = this.getBookDetail();
            var bookDetailButton = buttonObject.createButton("display-details", "info-sign");
            $(bookDetailButton)
                .attr("data-toggle", "modal")
                .attr("data-target", "#book-info-modal")
                .click(() => {
                    $("#modalHeader")
                        .empty()
                        .append(bookTitle);
                    this.getAuthors($("#modalHeader"));

                    $("#modalBody")
                        .empty()
                        .append(bookDetailDiv);
                });
            title.appendChild(bookDetailButton);
            $(title).append(bookTitle);

            
        }

        var fullscreenButton = buttonObject.createButton("fullscreen", "fullscreen");
        $(fullscreenButton).click(() => {
            var $fullscreenButtonFirstChild = $(fullscreenButton.firstChild as Node as HTMLElement);
            if ($fullscreenButtonFirstChild.hasClass("glyphicon-fullscreen")) {
                $("#ReaderDiv").addClass("fullscreen");
                $("body").addClass("fullscreen-body");
                $fullscreenButtonFirstChild.removeClass("glyphicon-fullscreen");
                $fullscreenButtonFirstChild.addClass("glyphicon-remove");
            } else {
                $("#ReaderDiv").removeClass("fullscreen");
                $("body").removeClass("fullscreen-body");
                $fullscreenButtonFirstChild.removeClass("glyphicon-remove");
                $fullscreenButtonFirstChild.addClass("glyphicon-fullscreen");
            }
            this.parentReader.readerLayout.updateSize();
        });
        bookHeaderButtons.appendChild(fullscreenButton);
        bookInfoDiv.appendChild(bookHeaderButtons);

        var hiddenDiv = document.createElement("div");
        $(hiddenDiv).addClass("collapse collapsible-div");
        var bookFullInfoDiv = document.createElement("div");
        $(bookFullInfoDiv).addClass("book-full-info-container");
        bookFullInfoDiv.appendChild(editionNoteDiv);
        bookFullInfoDiv.appendChild(this.getBookDetail());
        hiddenDiv.appendChild(bookFullInfoDiv);

        bookInfoDiv.appendChild(hiddenDiv);
        return bookInfoDiv;
    }
}

class ButtonFactory {
    private readerLayout: ReaderLayout;
    private deviceType: Device;

    constructor(readerLayout: ReaderLayout, deviceType: Device) {
        this.readerLayout = readerLayout;
        this.deviceType = deviceType;
    }

    public createViewButton(iconName: string, label: string, buttonId: string): HTMLButtonElement {
        var button: HTMLButtonElement = document.createElement("button");
        $(button).addClass(buttonId + "-button");
        var span = document.createElement("span");
        $(span).addClass("glyphicon glyphicon-" + iconName);
        $(button).append(span);

        var spanText = document.createElement("span");
        $(spanText).addClass("button-text");
        $(spanText).append(label);
        $(button).append(spanText);

        $(button).click(() => {
            if (this.deviceType === Device.Desktop) {
                this.readerLayout.createDesktopViewPanel(buttonId, spanText.innerHTML);
            } else {
                this.readerLayout.createMobileViewPanel(buttonId, spanText.innerHTML);
            }
        });
        return button;
    }

    public createToolButton(iconName: string, label: string, buttonId: string): HTMLButtonElement {
        var button: HTMLButtonElement = document.createElement("button");
        $(button).addClass(buttonId + "-button");
        if (buttonId === this.readerLayout.searchPanelId && this.readerLayout.getSearchResultPanel() == null) {
            $(button).prop("disabled", true);
        }

        var span = document.createElement("span");
        $(span).addClass("glyphicon glyphicon-" + iconName);
        $(button).append(span);

        var spanText = document.createElement("span");
        $(spanText).addClass("button-text");
        $(spanText).append(label);
        $(button).append(spanText);
        
        $(button).click(() => {
            if (buttonId === this.readerLayout.searchPanelId) {
                $(".searchbar-button").first().click(); //WORKAROUND recreating of search result panel
            } else {
            if (this.deviceType === Device.Desktop) {
                this.readerLayout.createDesktopToolPanel(buttonId, spanText.innerHTML);
            } else {
                this.readerLayout.createMobileToolPanel(buttonId, spanText.innerHTML);
                }
            }
        });
        return button;
    }

    public createAddBookmarkButton(iconName: string, label: string, buttonId: string): HTMLButtonElement {
        var button: HTMLButtonElement = document.createElement("button");
        $(button).addClass(buttonId + "-button");
        var span = document.createElement("span");
        $(span).addClass("glyphicon glyphicon-" + iconName);
        $(button).append(span);

        var spanText = document.createElement("span");
        $(spanText).addClass("button-text");
        $(spanText).append(label);
        $(button).append(spanText);


        $(button).click(() => {
            var actualPageName = this.readerLayout.getActualPage().text;
            this.readerLayout.getNewFavoriteDialog().show(actualPageName);
        });

        return button;
    }

    public createButton(buttonId: string, iconName: string): HTMLButtonElement {
        var button = document.createElement("button");
        $(button).addClass(buttonId + "-button button");

        var span = document.createElement("span");
        $(span).addClass("glyphicon glyphicon-" + iconName);
        $(button).append(span);

        return button;
    }

    public createLink(buttonId: string, iconName: string): HTMLAnchorElement {
        var button = document.createElement("a");
        $(button).addClass(buttonId + "-button");

        var span = document.createElement("span");
        $(span).addClass("glyphicon glyphicon-" + iconName);
        $(button).append(span);

        return button;
    }
}

enum Device {
    Mobile,
    Desktop
}
