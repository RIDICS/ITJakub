$(document).ready(function () {
    var dictionarySearch = new DictionarySearch();
    dictionarySearch.create();
});
var DictionarySearch = (function () {
    function DictionarySearch() {
        var pageSize = 25;
        this.tabs = new DictionarySearchTabs();
        this.callbackDelegate = new DropDownSelectCallbackDelegate();
        this.dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, this.callbackDelegate);
        var dictionaryViewerHeadword = new DictionaryViewer("#headwords-list", "#headwords-pagination", "#description-headwords", true);
        var dictionaryViewerFulltext = new DictionaryViewer("#headwords-list-fulltext", "#headwords-pagination-fulltext", "#description-fulltext", true);
        var dictionaryViewerAdvanced = new DictionaryViewer("#headwords-list-advanced", "#headwords-pagination-advanced", "#description-advanced", true);
        this.dictionaryWrapperBasic = new DictionaryViewerTextWrapper(dictionaryViewerHeadword, dictionaryViewerFulltext, pageSize, this.tabs, this.dictionarySelector);
        this.dictionaryWrapperAdvanced = new DictionaryViewerJsonWrapper(dictionaryViewerAdvanced, pageSize, this.tabs, this.dictionarySelector);
        this.search = new Search($("#dictionarySearchDiv")[0], this.processSearchJson.bind(this), this.processSearchText.bind(this));
        this.disabledShowOptions = [
            0 /* Author */,
            1 /* Title */,
            2 /* Editor */,
            3 /* Dating */
        ];
    }
    DictionarySearch.prototype.create = function () {
        this.callbackDelegate.selectedChangedCallback = function (state) {
            //TODO notify typeahead
        };
        var disabledOptions = new Array();
        disabledOptions.push(4 /* Fulltext */);
        disabledOptions.push(9 /* TokenDistance */);
        disabledOptions.push(6 /* Sentence */);
        disabledOptions.push(5 /* Heading */);
        this.dictionarySelector.makeDropdown();
        this.search.makeSearch(disabledOptions);
    };
    DictionarySearch.prototype.processSearchJson = function (json) {
        var filteredJsonForShowing = this.search.getFilteredQuery(json, this.disabledShowOptions);
        this.dictionaryWrapperAdvanced.loadCount(json, filteredJsonForShowing);
    };
    DictionarySearch.prototype.processSearchText = function (text) {
        this.dictionaryWrapperBasic.loadCount(text);
    };
    return DictionarySearch;
})();
var DictionarySearchTabs = (function () {
    function DictionarySearchTabs() {
        var _this = this;
        this.searchTabs = [
            new SearchTab("#tab-headwords", "#list-headwords", "#description-headwords"),
            new SearchTab("#tab-fulltext", "#list-fulltext", "#description-fulltext"),
            new SearchTab("#tab-advanced", "#list-advanced", "#description-advanced")
        ];
        $("#search-tabs li").addClass("hidden");
        $("#search-tabs a").click(function (e) {
            e.preventDefault();
            $(e.target).tab("show");
            _this.show(e.target.getAttribute("href"));
        });
    }
    DictionarySearchTabs.prototype.show = function (id) {
        var index = 0 /* Headwords */;
        switch (id) {
            case "#headwords":
                index = 0 /* Headwords */;
                break;
            case "#fulltext":
                index = 1 /* Fulltext */;
                break;
            case "#advanced":
                index = 2 /* Advanced */;
                break;
        }
        var searchTab = this.searchTabs[index];
        $("#headword-description > div").removeClass("active");
        $(".tab-content > div").removeClass("active");
        $(searchTab.descriptionDiv).addClass("active");
        $(searchTab.listDiv).addClass("active");
    };
    DictionarySearchTabs.prototype.showAdvanced = function () {
        var advancedSearchTab = this.searchTabs[2 /* Advanced */];
        $("#search-tabs li").addClass("hidden");
        $(advancedSearchTab.tabLi).removeClass("hidden");
        $(advancedSearchTab.tabLi).children().trigger("click");
    };
    DictionarySearchTabs.prototype.showBasic = function () {
        var advancedSearchTab = this.searchTabs[2 /* Advanced */];
        var headwordSearchTab = this.searchTabs[0 /* Headwords */];
        $("#search-tabs li").removeClass("hidden");
        $(advancedSearchTab.tabLi).addClass("hidden");
        $(headwordSearchTab.tabLi).children().trigger("click");
    };
    return DictionarySearchTabs;
})();
var DictionaryTabsEnum;
(function (DictionaryTabsEnum) {
    DictionaryTabsEnum[DictionaryTabsEnum["Headwords"] = 0] = "Headwords";
    DictionaryTabsEnum[DictionaryTabsEnum["Fulltext"] = 1] = "Fulltext";
    DictionaryTabsEnum[DictionaryTabsEnum["Advanced"] = 2] = "Advanced";
})(DictionaryTabsEnum || (DictionaryTabsEnum = {}));
var SearchTab = (function () {
    function SearchTab(tabLi, listDiv, descriptionDiv) {
        this.descriptionDiv = descriptionDiv;
        this.listDiv = listDiv;
        this.tabLi = tabLi;
    }
    return SearchTab;
})();
var DictionaryViewerJsonWrapper = (function () {
    function DictionaryViewerJsonWrapper(dictionaryViewer, pageSize, tabs, categoryDropDown) {
        this.categoryDropDown = categoryDropDown;
        this.tabs = tabs;
        this.pageSize = pageSize;
        this.dictionaryViewer = dictionaryViewer;
    }
    DictionaryViewerJsonWrapper.prototype.loadCount = function (json, filteredJsonForShowing) {
        var _this = this;
        this.json = json;
        this.selectedIds = this.categoryDropDown.getSelectedIds();
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaResultsCount",
            data: JSON.stringify({
                "json": json,
                "selectedBookIds": this.selectedIds.selectedBookIds,
                "selectedCategoryIds": this.selectedIds.selectedCategoryIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: function (resultCount) {
                $("#search-advanced-count").text(resultCount);
                _this.tabs.showAdvanced();
                _this.dictionaryViewer.createViewer(resultCount, _this.loadHeadwords.bind(_this), _this.pageSize, filteredJsonForShowing, true);
            }
        });
    };
    DictionaryViewerJsonWrapper.prototype.loadHeadwords = function (pageNumber) {
        var _this = this;
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria",
            data: JSON.stringify({
                "json": this.json,
                "start": this.pageSize * (pageNumber - 1),
                "count": this.pageSize,
                "selectedBookIds": this.selectedIds.selectedBookIds,
                "selectedCategoryIds": this.selectedIds.selectedCategoryIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.dictionaryViewer.showHeadwords(response);
            }
        });
    };
    return DictionaryViewerJsonWrapper;
})();
var DictionaryViewerTextWrapper = (function () {
    function DictionaryViewerTextWrapper(headwordViewer, fulltextViewer, pageSize, tabs, categoryDropDown) {
        this.categoryDropDown = categoryDropDown;
        this.tabs = tabs;
        this.pageSize = pageSize;
        this.headwordViewer = headwordViewer;
        this.fulltextViewer = fulltextViewer;
    }
    DictionaryViewerTextWrapper.prototype.loadCount = function (text) {
        var _this = this;
        this.text = text;
        this.selectedIds = this.categoryDropDown.getSelectedIds();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicResultsCount",
            data: {
                text: text,
                selectedBookIds: this.selectedIds.selectedBookIds,
                selectedCategoryIds: this.selectedIds.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                $("#search-headword-count").text(response.HeadwordCount);
                $("#search-fulltext-count").text(response.FulltextCount);
                _this.tabs.showBasic();
                _this.headwordViewer.createViewer(response.HeadwordCount, _this.loadHeadwords.bind(_this), _this.pageSize, text);
                _this.fulltextViewer.createViewer(response.FulltextCount, _this.loadFulltextHeadwords.bind(_this), _this.pageSize, text);
            }
        });
    };
    DictionaryViewerTextWrapper.prototype.loadHeadwords = function (pageNumber) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicHeadword",
            data: {
                text: this.text,
                start: this.pageSize * (pageNumber - 1),
                count: this.pageSize,
                selectedBookIds: this.selectedIds.selectedBookIds,
                selectedCategoryIds: this.selectedIds.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.headwordViewer.showHeadwords(response);
            }
        });
    };
    DictionaryViewerTextWrapper.prototype.loadFulltextHeadwords = function (pageNumber) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicFulltext",
            data: {
                text: this.text,
                start: this.pageSize * (pageNumber - 1),
                count: this.pageSize,
                selectedBookIds: this.selectedIds.selectedBookIds,
                selectedCategoryIds: this.selectedIds.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.fulltextViewer.showHeadwords(response);
            }
        });
    };
    return DictionaryViewerTextWrapper;
})();
//# sourceMappingURL=itjakub.dictionaries.search.js.map