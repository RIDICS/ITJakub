/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var ReaderModule = (function () {
    function ReaderModule(readerContainer) {
        this.readerContainer = readerContainer;
        this.actualPage = 0;
        this.pages = new Array();
        for (var i = 0; i < 15; i++) {
            this.pages.push("This is text of page " + i.toString());
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
        var contorlsDiv = document.createElement('div');
        $(contorlsDiv).addClass('reader-controls');

        var slider = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: this.pages.length - 1,
            value: 0,
            change: function (event, ui) {
                _this.moveToPage(ui.value);
            }
        });

        contorlsDiv.appendChild(slider);

        return contorlsDiv;
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
