var SearchBox = (function () {
    function SearchBox(inputFieldElement, controllerPath) {
        this.inputField = inputFieldElement;
        this.urlWithController = getBaseUrl() + controllerPath;
        this.datasets = [];
        this.bloodhounds = [];
        this.options = {
            hint: true,
            highlight: true,
            minLength: 1
        };
    }
    SearchBox.prototype.value = function (value) {
        $(this.inputField).typeahead('val', value);
    };
    SearchBox.prototype.create = function () {
        $(this.inputField).typeahead(this.options, this.datasets);
    };
    SearchBox.prototype.destroy = function () {
        $(this.inputField).typeahead("destroy");
    };
    SearchBox.prototype.clearAndDestroy = function () {
        for (var i = 0; i < this.bloodhounds.length; i++) {
            var bloodhound = this.bloodhounds[i];
            bloodhound.clear();
            bloodhound.clearPrefetchCache();
            bloodhound.clearRemoteCache();
        }
        this.datasets = [];
        this.bloodhounds = [];
        this.destroy();
    };
    SearchBox.prototype.addDataSet = function (name, groupHeader, parameterUrlString) {
        if (parameterUrlString === void 0) { parameterUrlString = null; }
        var prefetchUrl = this.urlWithController + "/GetTypeahead" + name;
        var remoteUrl = this.urlWithController + "/GetTypeahead" + name + "?query=%QUERY";
        if (parameterUrlString != null) {
            prefetchUrl += "?" + parameterUrlString;
            remoteUrl += "&" + parameterUrlString;
        }
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
        var dataset = {
            name: name,
            limit: 5,
            source: bloodhound,
            templates: {
                header: "<div class=\"tt-suggestions-header\">" + groupHeader + "</div>"
            }
        };
        this.bloodhounds.push(bloodhound);
        this.datasets.push(dataset);
    };
    return SearchBox;
})();
//# sourceMappingURL=itjakub.plugins.searchbox.js.map