$(document).ready(
    () => {
        const modulInicializator = new SearchModulInicializator({
            bibliographyModule: {
                forcedBookType: BookTypeEnum.ProfessionalLiterature,
                customConfigurationPath: "ProfessionalLiterature/ProfessionalLiterature/GetSearchConfiguration"
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
