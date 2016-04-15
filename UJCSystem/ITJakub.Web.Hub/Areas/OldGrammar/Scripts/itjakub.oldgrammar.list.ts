$(document).ready(() => {
    const modulInicializator = new ModulInicializator({
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
            forcedBookType: BookTypeEnum.Grammar,
            customConfigurationPath: "OldGrammar/OldGrammar/GetListConfiguration"
        },
        search: {
            container: document.getElementById("listSearchDiv") as HTMLDivElement,
            enabledOptions: [
                SearchTypeEnum.Title,
                SearchTypeEnum.Author,
                SearchTypeEnum.Editor,
                SearchTypeEnum.Dating
            ]
        },
        searchBox: {
            inputFieldElement: ".searchbar-input",
            controllerPath: "OldGrammar/OldGrammar",

            dataSet: {
                name: "Title",
                groupHeader: "Název"
            },
            searchBoxInputSelector: ".searchbar-input.tt-input",
            searchUrl: {
                advanced: getBaseUrl() + "OldGrammar/OldGrammar/AdvancedSearchPaged",
                text: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchPaged",
                textCount: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchCount"
            }
        },
        dropDownSelect: {
            dropDownSelectContainer: "#dropdownSelectDiv",
            dataUrl: getBaseUrl() + "OldGrammar/OldGrammar/GetGrammarsWithCategories",
            showStar: true
        }
    });
    modulInicializator.init();
});
