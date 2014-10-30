/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var ReaderModule = (function () {
    function ReaderModule(readerContainer) {
        this.readerContainer = readerContainer;
        this.actualPage = 0;
        this.sliderOnPage = 0;
        this.pages = new Array();
        for (var i = 0; i < 15; i++) {
            this.pages.push(i.toString() + "r");
        }
    }
    ReaderModule.prototype.makeReader = function (book) {
        $(this.readerContainer).empty();
        var readerDiv = document.createElement('div');
        $(readerDiv).addClass('reader');

        var controls = this.makeControls(book);
        readerDiv.appendChild(controls);

        var textArea = this.makeTextArea(book);
        readerDiv.appendChild(textArea);

        $(this.readerContainer).append(readerDiv);
    };

    ReaderModule.prototype.makeControls = function (book) {
        var _this = this;
        var controlsDiv = document.createElement('div');
        $(controlsDiv).addClass('reader-controls');
        var slider = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: this.pages.length - 1,
            value: 0,
            start: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
            },
            stop: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').hide();
            },
            slide: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html("Strana: " + _this.pages[ui.value]);
            },
            change: function (event, ui) {
                _this.moveToPage(ui.value);
            }
        });

        var sliderTooltip = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);

        var innerTooltip = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html("Strana: " + this.pages[0]);
        sliderTooltip.appendChild(innerTooltip);
        $(sliderTooltip).hide();

        var sliderHandle = $(slider).find('.ui-slider-handle');
        $(sliderHandle).append(sliderTooltip);
        $(sliderHandle).hover(function (event) {
            $(event.target).find('.slider-tip').show();
        });
        $(sliderHandle).mouseout(function (event) {
            $(event.target).find('.slider-tip').hide();
        });
        controlsDiv.appendChild(slider);

        return controlsDiv;
    };

    ReaderModule.prototype.makeTextArea = function (book) {
        var textAreaDiv = document.createElement('div');
        $(textAreaDiv).addClass('reader-text');
        return textAreaDiv;
    };

    ReaderModule.prototype.moveToPage = function (page) {
        this.actualPage = page;
        this.displayPage(this.pages[page]);
    };

    ReaderModule.prototype.displayPage = function (page) {
        $(this.readerContainer).find('div.reader-text').empty();
        $(this.readerContainer).find('div.reader-text').append(page);
    };
    return ReaderModule;
})();
//# sourceMappingURL=itjakub.plugins.reader.js.map
