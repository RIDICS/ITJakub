

class BibliographyModul {
    private static _instance: BibliographyModul = null;
    bibliographyModulControllerUrl: string = "";

    constructor() {
        if (BibliographyModul._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        BibliographyModul._instance = this;
    }

    public static getInstance(): BibliographyModul {
        if (BibliographyModul._instance === null) {
            BibliographyModul._instance = new BibliographyModul();
        }
        return BibliographyModul._instance;
    }

    public showBibliographies(bookIds: string[], container: string) {
        $(container).empty();
        var rootElement: HTMLUListElement = document.createElement('ul');
        $(rootElement).addClass('listing');

        var items = bookIds; //TODO here will be download of bibItems by ajax

        $.each(items, (index, item : BibliographyInfo) => {
            var bibliographyHtml = this.makeBibliography(item);
            rootElement.appendChild(bibliographyHtml);
        });
        $(container).append(rootElement);
    }

    private makeBibliography(bibItem: BibliographyInfo): HTMLLIElement {
        var liElement: HTMLLIElement = document.createElement('li');
        $(liElement).addClass('list-item');
        $(liElement).attr("data-bookId",bibItem.BookId);

        var visibleContent: HTMLDivElement = document.createElement('div');
        $(visibleContent).addClass('visible-content');

        var rightPanel: HTMLDivElement = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var bookButton: HTMLButtonElement = document.createElement('button');
        bookButton.type = 'button';
        $(bookButton).addClass('btn btn-sm book-button');
        var spanBook: HTMLSpanElement = document.createElement('span');
        $(spanBook).addClass('glyphicon glyphicon-book');
        bookButton.appendChild(spanBook);
        $(bookButton).click((event) => {
            
        }); //TODO fill click action
        rightPanel.appendChild(bookButton);

        var infoButton: HTMLButtonElement = document.createElement('button');
        infoButton.type = 'button';
        $(infoButton).addClass('btn btn-sm information-button');
        var spanInfo: HTMLSpanElement = document.createElement('span');
        $(spanInfo).addClass('glyphicon glyphicon-info-sign');
        infoButton.appendChild(spanInfo);
        $(infoButton).click((event) => {
            
        }); //TODO fill click action
        rightPanel.appendChild(infoButton);

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
        $(hideContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').hide("slow");
            $(this).siblings('.show-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(hideContentButton);
        visibleContent.appendChild(rightPanel);

        var middlePanel: HTMLDivElement = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        var middlePanelHeading: HTMLDivElement = document.createElement('div');
        $(middlePanelHeading).addClass('heading');
        middlePanelHeading.innerHTML = bibItem.Name;
        middlePanel.appendChild(middlePanelHeading);
        var middlePanelBody: HTMLDivElement = document.createElement('div');
        $(middlePanelBody).addClass('body');
        middlePanelBody.innerHTML = bibItem.Body;
        middlePanel.appendChild(middlePanelBody);
        visibleContent.appendChild(middlePanel);

        $(liElement).append(visibleContent);

        var hiddenContent: HTMLDivElement = document.createElement('div');
        $(hiddenContent).addClass('hidden-content');
        var tableDiv: HTMLDivElement = document.createElement('div');
        $(tableDiv).addClass('table');

        this.appendTableRow("Editor",bibItem.Editor, tableDiv);
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
       
    }

    private appendTableRow(label:string, value:string, tableDiv:HTMLDivElement) {
        var rowDiv: HTMLDivElement = document.createElement('div');
        $(rowDiv).addClass('row');
        var labelDiv: HTMLDivElement = document.createElement('div');
        $(labelDiv).addClass('cell label');
        labelDiv.innerHTML = label;
        rowDiv.appendChild(labelDiv);
        var valueDiv: HTMLDivElement = document.createElement('div');
        $(valueDiv).addClass('cell');
        if (!value || value.length === 0) {
            valueDiv.innerHTML = "&lt;nezadáno&gt;";
        } else {
            valueDiv.innerHTML = value; 
        }
        rowDiv.appendChild(valueDiv);
        tableDiv.appendChild(rowDiv);
    }
}

interface BibliographyInfo {
    BookId: string;
    Name: string;
    Body: string;
    Editor: string;
    Pattern: string;
    SourceAbbreviation : string;
    RelicAbbreviation: string;
    LiteraryType: string;
    LiteraryGenre: string;
    LastEditation: string;
    //EditationNote: string; //anchor href?
    Copyright: string;

}