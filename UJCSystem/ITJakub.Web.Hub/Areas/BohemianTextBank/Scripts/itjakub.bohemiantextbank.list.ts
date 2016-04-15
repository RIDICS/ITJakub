function initList() {
    const modulInicializator = new BohemiaTextBankModulInicializator({
        base: {
            url: {
                searchKey: "search",
                pageKey: "page",
                selectionKey: "selected",
                sortAscKey: "sortAsc",
                sortCriteriaKey: "sortCriteria"
            }
        },
        bibliographyModule: {
            resultsContainer: "#listResults",
            sortBarContainer: "#listResultsHeader",
            forcedBookType: BookTypeEnum.TextBank
        },
        search: {
            container: document.getElementById("listSearchDiv") as HTMLDivElement,
            enabledOptions: [
                SearchTypeEnum.Title,
                SearchTypeEnum.Author,
                SearchTypeEnum.Editor,
                SearchTypeEnum.Dating,
                SearchTypeEnum.Fulltext,
                SearchTypeEnum.Heading,
                SearchTypeEnum.Sentence,
                SearchTypeEnum.Term,
                SearchTypeEnum.TokenDistance
            ]
        },
        searchBox: {
            inputFieldElement: ".searchbar-input",
            controllerPath: "BohemianTextBank/BohemianTextBank",

            dataSet: {
                name: "Title",
                groupHeader: "Název"
            },
            searchBoxInputSelector: ".searchbar-input.tt-input",
            searchUrl: {
                advanced: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchPaged",
                text: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchPaged",
                textCount: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchCount",
                advancedCount: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchResultsCount"
            }
        },
        dropDownSelect: {
            dropDownSelectContainer: "#dropdownSelectDiv",
            dataUrl: getBaseUrl() + "BohemianTextBank/BohemianTextBank/GetCorpusWithCategories",
            showStar: true
        }
    });
    modulInicializator.init();
}
