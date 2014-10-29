/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />

class ReaderModule {

    readerContainer: string;

    constructor(readerContainer: string) {
        this.readerContainer = readerContainer;
    }

    public makeReader(book: IBookInfo) {
        $(this.readerContainer).empty();
        var readerDiv: HTMLDivElement = document.createElement('div');
        $(readerDiv).addClass('reader');
        
        var controls = this.makeControls(book);
        readerDiv.appendChild(controls);

        $(this.readerContainer).append(readerDiv);
    }

    private makeControls(book: IBookInfo): HTMLDivElement {
        var contorlsDiv: HTMLDivElement = document.createElement('div');
        $(contorlsDiv).addClass('reader-controls');

        var slider: HTMLDivElement = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: 100,
            value: 0,
            change: (event: Event, ui) => {
                /* Update as desired. */
            }
        });

        contorlsDiv.appendChild(slider);

        return contorlsDiv;
    }
}