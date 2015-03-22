var BibliographyModul = (function () {
    function BibliographyModul() {
        this.bibliographyModulControllerUrl = "";
        if (BibliographyModul._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        BibliographyModul._instance = this;
    }
    BibliographyModul.getInstance = function () {
        if (BibliographyModul._instance === null) {
            BibliographyModul._instance = new BibliographyModul();
        }
        return BibliographyModul._instance;
    };

    BibliographyModul.prototype.showBibliographies = function (bookIds, container) {
        var _this = this;
        $(container).empty();
        var rootElement = document.createElement('ul');
        $(rootElement).addClass('listing');
        $.ajax({
            type: "GET",
            url: "/Editions/Bibliographies",
            data: JSON.stringify({ 'bookIds': bookIds }),
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                $.each(response, function (index, item) {
                    var bibliographyHtml = _this.makeBibliography(item);
                    rootElement.appendChild(bibliographyHtml);
                });
                $(container).append(rootElement);
            }
        });
    };

    BibliographyModul.prototype.makeBibliography = function (bibItem) {
        var liElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookId", bibItem.BookId);

        var visibleContent = document.createElement('div');
        $(visibleContent).addClass('visible-content');

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
        visibleContent.appendChild(rightPanel);

        var middlePanel = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        var middlePanelHeading = document.createElement('div');
        $(middlePanelHeading).addClass('heading');
        middlePanelHeading.innerHTML = bibItem.Name;
        middlePanel.appendChild(middlePanelHeading);
        var middlePanelBody = document.createElement('div');
        $(middlePanelBody).addClass('body');
        middlePanelBody.innerHTML = bibItem.Body;
        middlePanel.appendChild(middlePanelBody);
        visibleContent.appendChild(middlePanel);

        $(liElement).append(visibleContent);

        var hiddenContent = document.createElement('div');
        $(hiddenContent).addClass('hidden-content');
        var tableDiv = document.createElement('div');
        $(tableDiv).addClass('table');

        this.appendTableRow("Editor", bibItem.Editor, tableDiv);
        this.appendTableRow("Předloha", bibItem.Pattern, tableDiv);
        this.appendTableRow("Zkratka památky", bibItem.RelicAbbreviation, tableDiv);
        this.appendTableRow("Zkratka pramene", bibItem.SourceAbbreviation, tableDiv);
        this.appendTableRow("Literární druh", bibItem.LiteraryType, tableDiv);
        this.appendTableRow("Literární žánr", bibItem.LiteraryGenre, tableDiv);
        this.appendTableRow("Poslední úprava edice", bibItem.LastEditation, tableDiv);

        //TODO add Edicni poznamka anchor and copyright to hiddenContent here
        hiddenContent.appendChild(tableDiv);

        $(liElement).append(hiddenContent);

        return liElement;
    };

    BibliographyModul.prototype.appendTableRow = function (label, value, tableDiv) {
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
        tableDiv.appendChild(rowDiv);
    };
    BibliographyModul._instance = null;
    return BibliographyModul;
})();
//# sourceMappingURL=itjakub.editions.bibliography.js.map
