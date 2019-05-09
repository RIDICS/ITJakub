$(document.documentElement).ready(
    () => {
        const modulInicializator = new SearchModulInicializator({
            bibliographyModule: {
                forcedBookType: BookTypeEnum.Edition,
                customConfigurationPath: "Editions/Editions/GetSearchConfiguration"
            },
            search: {
                enabledOptions: [
                    SearchTypeEnum.Title,
                    SearchTypeEnum.Author,
                    SearchTypeEnum.Editor,
                    SearchTypeEnum.Dating,
                    SearchTypeEnum.Fulltext,
                    SearchTypeEnum.TokenDistance,
                    SearchTypeEnum.Heading,
                    SearchTypeEnum.Sentence
                ],
                url: {
                    advanced: getBaseUrl() + "Editions/Editions/AdvancedSearchPaged",
                    text: getBaseUrl() + "Editions/Editions/TextSearchFulltextPaged",
                    textCount: getBaseUrl() + "Editions/Editions/TextSearchFulltextCount",
                    advancedCount: getBaseUrl() + "Editions/Editions/AdvancedSearchResultsCount"
                },
                favoriteQueries: {
                    bookType: BookTypeEnum.Edition,
                    queryType: QueryTypeEnum.Search
                }
            },
            dropDownSelect: {
                dataUrl: getBaseUrl() + "Editions/Editions/GetEditionsWithCategories"
            }
        });
        modulInicializator.init();
    }
);
