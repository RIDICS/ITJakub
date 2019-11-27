$(document.documentElement).ready(
    () => {
        const modulInicializator = new SearchModulInicializator({
            bibliographyModule: {
                forcedBookType: BookTypeEnum.ProfessionalLiterature,
                customConfigurationPath: $("#bibliography-configuration-url").data("url")
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
                enabledSearchInSecondPortal: false,
                url: {
                    advanced: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchPaged",
                    text: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchFulltextPaged",
                    textCount: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchFulltextCount",
                    advancedCount: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchResultsCount"
                },
                favoriteQueries: {
                    bookType: BookTypeEnum.ProfessionalLiterature,
                    queryType: QueryTypeEnum.Search
                }
            },
            dropDownSelect: {
                dataUrl: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/GetProfessionalLiteratureWithCategories"
            }
        });
        modulInicializator.init();
    }
);
