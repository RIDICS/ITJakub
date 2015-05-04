var RegExEditor = (function () {
    function RegExEditor(container, searchBox) {
        this.container = container;
        this.searchBox = searchBox;
    }
    RegExEditor.prototype.makeRegExEditor = function () {
        var testDiv = document.createElement("strong");
        testDiv.innerHTML = "TEST";
        $(this.container).empty();
        $(this.container).append(testDiv);
    };
    return RegExEditor;
})();
//# sourceMappingURL=itjakub.plugins.regexsearch.js.map