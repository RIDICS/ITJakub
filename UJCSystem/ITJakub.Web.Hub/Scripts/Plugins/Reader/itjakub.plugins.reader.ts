/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />




class ReaderModule {

    readerContainer: string;
    sliderOnPage: number;
    actualPage: number;
    pages: Array<string>;

    constructor(readerContainer: string) {
        this.readerContainer = readerContainer;
        this.actualPage = 0;
        this.sliderOnPage = 0;
        this.pages = new Array<string>();
        for (var i = 0; i < 15; i++) {  //TODO pages should be loaded by ajax
            this.pages.push(i.toString()+"r");
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
        var controlsDiv: HTMLDivElement = document.createElement('div');
        $(controlsDiv).addClass('reader-controls');
        var slider: HTMLDivElement = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: this.pages.length - 1,
            value: 0,
            start: (event, ui) => {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
            },
            stop: (event, ui) =>  {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').hide();
            },
            slide: (event, ui) => {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html("Strana: " + this.pages[ui.value]);
               
            },
            change: (event: Event, ui: JQueryUI.SliderUIParams) => {
                this.moveToPage(ui.value);
            }
        });

        var sliderTooltip: HTMLDivElement = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip: HTMLDivElement = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);

        var innerTooltip: HTMLDivElement = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html("Strana: " + this.pages[0]);
        sliderTooltip.appendChild(innerTooltip);
        $(sliderTooltip).hide();

        var sliderHandle = $(slider).find('.ui-slider-handle');
        $(sliderHandle).append(sliderTooltip);
        $(sliderHandle).hover((event) => {
            $(event.target).find('.slider-tip').show();
        });
        $(sliderHandle).mouseout((event) => {
            $(event.target).find('.slider-tip').hide();
        });
        controlsDiv.appendChild(slider);

        return controlsDiv;
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