/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />

class ReaderModule {

    readerContainer: string;
    actualPage: number;
    pages: Array<string>;

    constructor(readerContainer: string) {
        this.readerContainer = readerContainer;
        this.actualPage = 0;
        this.pages = new Array<string>();
        for (var i = 0; i < 15; i++) {  //TODO pages should be loaded by ajax
            this.pages.push("This is text of page " + i.toString());
        }
    }

    public makeReader(book: IBookInfo) {
        $(this.readerContainer).empty();
        var readerDiv: HTMLDivElement = document.createElement('div');
        $(readerDiv).addClass('reader');

        var controls = this.makeControls(book);
        readerDiv.appendChild(controls);

        var textArea = this.makeTextArea(book);
        readerDiv.appendChild(textArea);

        $(this.readerContainer).append(readerDiv);
    }

    private makeControls(book: IBookInfo): HTMLDivElement {
        var contorlsDiv: HTMLDivElement = document.createElement('div');
        $(contorlsDiv).addClass('reader-controls');

        var slider: HTMLDivElement = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: this.pages.length-1,
            value: 0,
            change: (event: Event, ui: JQueryUI.SliderUIParams) => {
                this.moveToPage(ui.value);
            }
        });

        contorlsDiv.appendChild(slider);

        return contorlsDiv;
    }

    private makeTextArea(book: IBookInfo): HTMLDivElement {
        var textAreaDiv: HTMLDivElement = document.createElement('div');
        $(textAreaDiv).addClass('reader-text');
        return textAreaDiv;
    }

    moveToPage(page: number) {
        this.actualPage = page;
        this.displayPage(this.pages[page]);
    }

    displayPage(page: string) {
        $(this.readerContainer).find('div.reader-text').empty();
        $(this.readerContainer).find('div.reader-text').append(page);

    }
}