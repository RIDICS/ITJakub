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
                forcedBookType: BookTypeEnum.Edition,
                customConfigurationPath: "Editions/Editions/GetListConfiguration"
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
                controllerPath: "Editions/Editions",

                dataSet: {
                    name: "Title",
                    groupHeader: "Název"
                },
                searchBoxInputSelector: ".searchbar-input.tt-input",
                searchUrl: {
                    advanced: getBaseUrl() + "Editions/Editions/AdvancedSearchPaged",
                    text: getBaseUrl() + "Editions/Editions/TextSearchPaged",
                    textCount: getBaseUrl() + "Editions/Editions/TextSearchCount"
                }
            },
            dropDownSelect: {
                dropDownSelectContainer: "#dropdownSelectDiv",
                dataUrl: getBaseUrl() + "Editions/Editions/GetEditionsWithCategories",
                showStar: true
            }
        });
        modulInicializator.init();
    }
);
