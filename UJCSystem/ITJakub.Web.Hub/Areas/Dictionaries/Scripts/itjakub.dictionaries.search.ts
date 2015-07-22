$(document).ready(() => {
    var search = new Search($("#dictionarySearchDiv"), processSearchJson, processSearchText );
    var disabledOptions = new Array<SearchTypeEnum>();
    disabledOptions.push(SearchTypeEnum.Fulltext);
    disabledOptions.push(SearchTypeEnum.TokenDistance);
    disabledOptions.push(SearchTypeEnum.Sentence);
    disabledOptions.push(SearchTypeEnum.Heading);
    search.makeSearch(disabledOptions);

    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state) => {
        
    };

    var dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
});

function processSearchResults(result: any) {
    alert("processed: " + result);
}

function processSearchJson(json: string) {
    $.ajax({
        type: "POST",
        traditional: true,
        data: JSON.stringify({
            "json": json
        }),
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaResultsCount",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        success: (response) => {
            processSearchResults(response);
        },
        error: (response: JQueryXHR) => {
        }
    });

}

function processSearchText(text: string) {
    $.ajax({
        type: "GET",
        traditional: true,
        data: {
            text: text
        },
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicResultsCount",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        success: (response) => {
            processSearchResults(response);
        },
        error: (response: JQueryXHR) => {
        }
    });

}

interface IHeadwordSearchResultContract {
    HeadwordCount: number;
    FulltextCount: number;
}