$(document).ready(() => {
    const modulInicializator = new ListModulInicializator({
        bibliographyModule: {
            forcedBookType: BookTypeEnum.Grammar,
            customConfigurationPath: "OldGrammar/OldGrammar/GetListConfiguration"
        },
        search: {
            enabledOptions: [
                SearchTypeEnum.Title,
                SearchTypeEnum.Author,
                SearchTypeEnum.Editor,
                SearchTypeEnum.Dating
            ],
            url: {
                advanced: getBaseUrl() + "OldGrammar/OldGrammar/AdvancedSearchPaged",
                text: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchPaged",
                advancedCount: getBaseUrl() + "OldGrammar/OldGrammar/AdvancedSearchResultsCount",
                textCount: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchCount"
            },
            favoriteQueries: {
                bookType: BookTypeEnum.Grammar,
                queryType: QueryTypeEnum.List
            }
        },
        searchBox: {
            controllerPath: "OldGrammar/OldGrammar"
        },
        dropDownSelect: {
            dataUrl: getBaseUrl() + "OldGrammar/OldGrammar/GetGrammarsWithCategories"
        }
    });
    modulInicializator.init();
});
