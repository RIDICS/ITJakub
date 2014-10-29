/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var ReaderModule = (function () {
    function ReaderModule(readerContainer) {
        this.readerContainer = readerContainer;
    }
    ReaderModule.prototype.makeReader = function (book) {
        $(this.readerContainer).empty();
        var readerDiv = document.createElement('div');
        $(readerDiv).addClass('reader');

        var controls = this.makeControls(book);
        readerDiv.appendChild(controls);

        $(this.readerContainer).append(readerDiv);
    };

    ReaderModule.prototype.makeControls = function (book) {
        var contorlsDiv = document.createElement('div');
        $(contorlsDiv).addClass('reader-controls');

        var slider = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: 100,
            value: 0,
            change: function (event, ui) {
                /* Update as desired. */
            }
        });

        contorlsDiv.appendChild(slider);

        return contorlsDiv;
    };
    return ReaderModule;
})();
//# sourceMappingURL=itjakub.plugins.reader.js.map
