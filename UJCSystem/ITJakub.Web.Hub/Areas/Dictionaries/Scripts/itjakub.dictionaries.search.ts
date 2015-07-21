$(document).ready(() => {
    var search = new Search($("#dictionarySearchDiv"), processSearchJson, processSearchText );
    var disabledOptions = new Array<SearchTypeEnum>();
    disabledOptions.push(SearchTypeEnum.Author);
    disabledOptions.push(SearchTypeEnum.Title);
    disabledOptions.push(SearchTypeEnum.Editor);
    disabledOptions.push(SearchTypeEnum.Dating);
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
        data: JSON.stringify({ "json": json }),
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria",
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
        type: "POST",
        traditional: true,
        data: JSON.stringify({ "text": text }),
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaText",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        success: (response) => {
            processSearchResults(response);
        },
        error: (response: JQueryXHR) => {
        }
    });

}