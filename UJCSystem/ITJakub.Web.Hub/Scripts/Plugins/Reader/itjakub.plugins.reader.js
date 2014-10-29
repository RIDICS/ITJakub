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
            range: "min",
            min: 0,
            max: 500,
            values: 0
        });

        contorlsDiv.appendChild(slider);

        return contorlsDiv;
    };
    return ReaderModule;
})();
//# sourceMappingURL=itjakub.plugins.reader.js.map
