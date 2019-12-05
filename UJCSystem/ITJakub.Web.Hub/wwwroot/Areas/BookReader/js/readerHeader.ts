﻿class BookHeader {
    private parentReader: ReaderLayout;
    private bookId: string;
    private versionId: string;
    private sc: ServerCommunication;
    private bookTitle: string;

    constructor(parentReader: ReaderLayout, sc: ServerCommunication, headerDiv: HTMLDivElement, bookTitle: string) {
        this.parentReader = parentReader;
        this.sc = sc;
        this.bookId = parentReader.getBookId();
        this.versionId = parentReader.getVersionId();
        this.bookTitle = bookTitle;

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
            $(editionNoteHeader).append("Ediční poznámka");
            $(editionNoteDiv).append(editionNoteHeader);
        }
        
        var editionNote: JQueryXHR = this.sc.getEditionNote(this.bookId);
        editionNote.done((response: { editionNote: string }) => {
            var editionNoteText = document.createElement("div");
            $(editionNoteText).addClass("edition-note-text");
            if (response.editionNote === "") {
                $(editionNoteText).append("Toto dílo nemá ediční poznámku");
            } else {
                $(editionNoteText).append(response.editionNote);
            }
            editionNoteDiv.appendChild(editionNoteText);
            $(editionNoteDiv).removeClass("loading");
        });
        editionNote.fail(() => {
            $(editionNoteDiv).append("Ediční poznámku se nepodařilo načíst");
            $(editionNoteDiv).removeClass("loading");
        });

        return editionNoteDiv;
    }

    private getBookDetail(): HTMLDivElement {
        var bookDetailDiv = document.createElement("div");
        $(bookDetailDiv).addClass("book-detail-wrapper");
        var bookDetailHeader = document.createElement("h3");
        $(bookDetailHeader).append("Informace o díle");
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

            detailTable.makeTableRow("Editor", editors);
            detailTable.makeTableRow("Předloha", detailData.LiteraryOriginal);
            detailTable.makeTableRow("Zkratka památky", detailData.RelicAbbreviation);
            detailTable.makeTableRow("Zkratka pramene", detailData.SourceAbbreviation);
            detailTable.makeTableRow("Literární druh", detailData.LiteraryKinds);
            detailTable.makeTableRow("Literární žánr", detailData.LiteraryGenre);
            detailTable.makeTableRow("Poslední úprava edice	", detailData.CreateTimeString);

            $(detailTable.build()).find(".bib-table-cell").each((index, element) => {
                if (element.innerHTML === "" || element.innerHTML === "undefined") {
                    element.innerHTML = "&lt;Nezadáno&gt;";
                }
            });

            $(bookDetailDiv).append(detailTable.build());
            
            
        });
        bookDetail.fail(() => {
            $(bookDetailDiv).append("Nepodařilo se načíst detaily o díle");
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
            target.prepend("&lt;Nenačtení autoři&gt;");
        });
    }

    private makeToolButtons(deviceType: Device): HTMLDivElement {
        var toolButtonsDiv = document.createElement("div");
        $(toolButtonsDiv).addClass("tool-panel");

        var button = new ButtonFactory(this.parentReader, deviceType);
        var toolButtons: HTMLDivElement = document.createElement("div");
        $(toolButtons).addClass("buttons left");

        var addBookmarksButton = button.createAddBookmarkButton("bookmark",
            "Přidat záložku",
            this.parentReader.bookmarksPanelId
        );
        toolButtons.appendChild(addBookmarksButton);


        var bookmarksButton =
            button.createToolButton("bookmark",
                this.parentReader.bookmarksPanelLabel,
                this.parentReader.bookmarksPanelId);
        toolButtons.appendChild(bookmarksButton);

        var contentButton = button.createToolButton("book",
            this.parentReader.contentPanelLabel,
            this.parentReader.contentPanelId);
        toolButtons.appendChild(contentButton);

        var searchResultButton = button.createToolButton("search",
            this.parentReader.searchPanelLabel,
            this.parentReader.searchPanelId);
        toolButtons.appendChild(searchResultButton);

        var termsButton = button.createToolButton("list-alt",
            this.parentReader.termsPanelLabel,
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
                    this.parentReader.textPanelLabel,
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
                        this.parentReader.imagePanelLabel,
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
                        this.parentReader.audioPanelLabel,
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
                        .append("Ediční poznámka");

                    $("#modalBody")
                        .empty()
                        .append(this.getEditionNote(false));
                });
            var editionNoteLabel = document.createElement("span");
            $(editionNoteLabel).addClass("button-text");
            $(editionNoteLabel).append("Ediční poznámka");
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
        showPageNameLabel.innerHTML = "Číslování stránek";
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
        showPageOnNewLineLabel.innerHTML = "Zalamovat stránky";
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
        showCommentLabel.innerHTML = "Poznámky";
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
        controlsDiv.appendChild(this.makePageNavigation());
        headerDiv.appendChild(controlsDiv);

        return headerDiv;
    }

    private createMobileHeaderDiv(bookTitle: string): HTMLDivElement {
        var headerDiv = document.createElement("div");
        headerDiv.appendChild(this.informationDiv(bookTitle, Device.Mobile));

        var controlsDiv = document.createElement("div");
        $(controlsDiv).addClass("reader-controls content-container");
        controlsDiv.appendChild(this.makeViewButtons(Device.Mobile));
        controlsDiv.appendChild(this.makePageNavigation());
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
        pageInputText.setAttribute("placeholder", "Přejít na stranu...");
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
            for (var i = 0; i < this.parentReader.pages.length; i++) {
                if (this.parentReader.pages[i].text === pageName) {
                    pageIndex = i;
                    break;
                }
            }

            if (this.parentReader.pages[pageIndex] !== undefined) {
                var page: BookPage = this.parentReader.pages[pageIndex];
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
            max: this.parentReader.pages.length - 1,
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
                if (this.parentReader.pages[ui.value] !== undefined) {
                    $(event.target as Node as HTMLElement).find(".ui-slider-handle").find(".tooltip-inner").html("Strana: " + this.parentReader.pages[ui.value].text);
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
        if (this.parentReader.pages[0] !== undefined) {
            $(innerTooltip).html("Strana: " + this.parentReader.pages[0].text);
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
        var fullscreenButton = buttonObject.createButton("fullscreen", "fullscreen");
        $(fullscreenButton).click(() => {
            var $fullscreenButtonFirstChild = $(fullscreenButton.firstChild as Node as HTMLElement);
            if ($fullscreenButtonFirstChild.hasClass("glyphicon-fullscreen")) {
                $("#ReaderDiv").addClass("fullscreen");
                $fullscreenButtonFirstChild.removeClass("glyphicon-fullscreen");
                $fullscreenButtonFirstChild.addClass("glyphicon-remove");
            } else {
                $("#ReaderDiv").removeClass("fullscreen");
                $fullscreenButtonFirstChild.removeClass("glyphicon-remove");
                $fullscreenButtonFirstChild.addClass("glyphicon-fullscreen");
            }
            this.parentReader.readerLayout.updateSize();


        });
        bookInfoDiv.appendChild(fullscreenButton);
        var title = document.createElement("span");
        $(title).addClass("title");
        bookInfoDiv.appendChild(title);
        if (deviceType === Device.Desktop) {
            title.innerHTML = bookTitle;
            this.getAuthors($(title));
            
            var detailsButton = buttonObject.createButton("more", "info-sign");
            $(detailsButton).click((event) => {
                var target: JQuery = $(event.target as Node as HTMLInputElement);

                var title = target.parents(".book-details").find(".title");
                title.toggleClass("full");

                var details = target.parents(".book-details").find(".hidden-content");
                if (!details.hasClass("visible")) {
                    $(target).removeClass("glyphicon-info-sign");
                    $(target).addClass("glyphicon-chevron-up");
                    details.addClass("visible");
                } else {
                    $(target).removeClass("glyphicon-chevron-up");
                    $(target).addClass("glyphicon-info-sign");
                    details.removeClass("visible");
                }
            });
            bookInfoDiv.appendChild(detailsButton);  

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

        

        var hiddenDiv = document.createElement("div");
        $(hiddenDiv).addClass("hidden-content");
        hiddenDiv.appendChild(editionNoteDiv);
        hiddenDiv.appendChild(this.getBookDetail());

        bookInfoDiv.appendChild(hiddenDiv);
        return bookInfoDiv;
    }

    private makePageNavigation(): HTMLDivElement {
        var paginationUl: HTMLUListElement = document.createElement("ul");
        paginationUl.classList.add("pagination", "pagination-sm");

        var toLeft = document.createElement("ul");
        toLeft.classList.add("page-navigation-container", "page-navigation-container-left");

        var liElement: HTMLLIElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        var anchor: HTMLAnchorElement = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = "|<";
        $(anchor).click((event) => {
            event.stopPropagation();
            this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", 0);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = "<<";
        $(anchor).click((event) => {
            event.stopPropagation();
            this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", this.parentReader.actualPageIndex - 5);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = "<";
        $(anchor).click((event) => {
            event.stopPropagation();
            this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", this.parentReader.actualPageIndex - 1);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        var toRight = document.createElement("ul");
        toRight.classList.add("page-navigation-container", "page-navigation-container-right");

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = ">";
        $(anchor).click((event) => {
            event.stopPropagation();
            this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", this.parentReader.actualPageIndex + 1);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = ">>";
        $(anchor).click((event) => {
            event.stopPropagation();
            this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", this.parentReader.actualPageIndex + 5);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.innerHTML = ">|";
        $(anchor).click((event) => {
            event.stopPropagation();
            this.parentReader.readerLayout.eventHub.emit("moveToPageNumber", this.parentReader.actualPageIndex - 1);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-left");
        liElement.innerHTML = "...";
        paginationUl.appendChild(liElement);

        $.each(this.parentReader.pages, (index, page) => {
            liElement = document.createElement("li");
            $(liElement).addClass("page");
            $(liElement).data("page-index", index);
            anchor = document.createElement("a");
            anchor.href = "#";
            anchor.innerHTML = page.text;
            $(anchor).click((event) => {
                event.stopPropagation();
                this.parentReader.readerLayout.eventHub.emit("navigationClicked", page.pageId);
                return false;
            });
            liElement.appendChild(anchor);
            paginationUl.appendChild(liElement);
        });

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-right");
        liElement.innerHTML = "...";
        paginationUl.appendChild(liElement);

        var listingContainer: HTMLDivElement = document.createElement("div");
        listingContainer.classList.add("page-navigation-container-helper");
        listingContainer.classList.add("hidden");
        if (this.parentReader.deviceType === Device.Mobile) {
            $(listingContainer).prepend("Str: ");
            listingContainer.appendChild(paginationUl);
        } else {
            listingContainer.appendChild(toLeft);
            listingContainer.appendChild(paginationUl);
            listingContainer.appendChild(toRight);
        }
        return listingContainer;
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
