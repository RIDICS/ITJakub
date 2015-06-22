var SearchBox = (function () {
    function SearchBox(inputFieldElement, controllerName) {
        this.inputField = inputFieldElement;
        var prefetchUrl = getBaseUrl() + controllerName + "/GetTypeaheadData";
        var remoteUrl = getBaseUrl() + controllerName + "/GetTypeaheadDataForQuery?query=%QUERY";
        var remoteOptions = {
            url: remoteUrl,
            wildcard: "%QUERY"
        };
        var bloodhound = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.whitespace,
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            prefetch: prefetchUrl,
            remote: remoteOptions
        });
        this.options = {
            hint: true,
            highlight: true,
            minLength: 2
        };
        this.datasets = {
            name: controllerName,
            source: bloodhound
        };
    }
    SearchBox.prototype.create = function () {
        $(this.inputField).typeahead(this.options, this.datasets);
    };
    SearchBox.prototype.destroy = function () {
        $(this.inputField).typeahead("destroy");
    };
    return SearchBox;
})();
//# sourceMappingURL=itjakub.plugins.searchbox.js.map