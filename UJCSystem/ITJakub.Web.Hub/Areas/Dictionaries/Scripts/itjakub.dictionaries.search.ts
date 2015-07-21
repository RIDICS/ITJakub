$(document).ready(() => {
    var search = new Search($("#dictionarySearchDiv"), processSearchJson, processSearchText );
    var disabledOptions = new Array<SearchTypeEnum>();
    disabledOptions.push(SearchTypeEnum.Headwords);
    disabledOptions.push(SearchTypeEnum.TokenDistanceHeadwords);
    search.makeSearch(disabledOptions);
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