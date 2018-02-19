$(document.documentElement).ready(
    () => {
        const modulInicializator = new SearchModulInicializator({
            bibliographyModule: {
                forcedBookType: BookTypeEnum.Grammar,
                customConfigurationPath: "OldGrammar/OldGrammar/GetSearchConfiguration"
            },
            search: {
                enabledOptions: [
                    SearchTypeEnum.Title,
                    SearchTypeEnum.Author,
                    SearchTypeEnum.Editor,
                    SearchTypeEnum.Dating,
                    SearchTypeEnum.Term
                ],
                url: {
                    advanced: getBaseUrl() + "OldGrammar/OldGrammar/AdvancedSearchPaged",
                    text: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchFulltextPaged",
                    textCount: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchFulltextCount",
                    advancedCount: getBaseUrl() + "OldGrammar/OldGrammar/AdvancedSearchResultsCount"
                },
                favoriteQueries: {
                    bookType: BookTypeEnum.Grammar,
                    queryType: QueryTypeEnum.Search
                }
            },
            dropDownSelect: {
                dataUrl: getBaseUrl() + "OldGrammar/OldGrammar/GetGrammarsWithCategories"
            }
        });
        modulInicializator.init();
    }
);
