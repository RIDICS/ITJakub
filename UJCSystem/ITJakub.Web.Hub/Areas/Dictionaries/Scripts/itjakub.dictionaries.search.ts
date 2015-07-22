$(document).ready(() => {
    var pageSize = 25;
    var dictionaryViewerAdvanced = new DictionaryViewer("#advanced", "#pagination-advanced", "#headword-description", true);
    var dictionaryWrapperAdvanced = new DictionaryViewerJsonWrapper(dictionaryViewerAdvanced, pageSize);

    var processSearchJson = (json: string) => {
        $("#headword-description").empty();
        dictionaryWrapperAdvanced.loadCount(json);
    };

    var processSearchText = (text: string) => {
        $("#headword-description").empty();
        //TODO
    };

    var search = new Search($("#dictionarySearchDiv"), processSearchJson, processSearchText);
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

class DictionaryViewerJsonWrapper {
    private pageSize: number;
    private dictionaryViewer: DictionaryViewer;
    private json: string;

    constructor(dictionaryViewer: DictionaryViewer, pageSize: number) {
        this.pageSize = pageSize;
        this.dictionaryViewer = dictionaryViewer;
    }

    loadCount(json: string) {
        this.json = json;
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaResultsCount",
            data: JSON.stringify({
                "json": json
            }),
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var resultCount = response;
                this.dictionaryViewer.createViewer(resultCount, this.loadHeadwords.bind(this), this.pageSize, json);
            }
        });
    }

    loadHeadwords(pageNumber: number) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria",
            data: JSON.stringify({
                "json": this.json,
                "start": this.pageSize * (pageNumber - 1),
                "count": this.pageSize
            }),
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.dictionaryViewer.showHeadwords(response);
            }
        });
    }
}

class DictionaryViewerTextWrapper {
    
}

interface IHeadwordSearchResultContract {
    HeadwordCount: number;
    FulltextCount: number;
}