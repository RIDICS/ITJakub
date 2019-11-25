$(document.documentElement).ready(() => {
    const modulInicializator = new ProfessionalLiteratureModulInicializator({
        bibliographyModule: {
            forcedBookType: BookTypeEnum.ProfessionalLiterature,
            customConfigurationPath: $("#bibliography-configuration-url").data("url")
        },
        search: {
            enabledOptions: [
                SearchTypeEnum.Title,
                SearchTypeEnum.Author,
                SearchTypeEnum.Editor,
                SearchTypeEnum.Dating
            ],
            enabledSearchInSecondPortal: true,
            url: {
                advanced: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchPaged",
                text: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchPaged",
                textCount: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchCount",
                advancedCount: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchResultsCount"
            },
            favoriteQueries: {
                bookType: BookTypeEnum.ProfessionalLiterature,
                queryType: QueryTypeEnum.List
            }
        },
        searchBox: {
            controllerPath: "ProfessionalLiterature/ProfessionalLiterature"
        },
        dropDownSelect: {
            dataUrl: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/GetProfessionalLiteratureWithCategories"
        }
    });
    modulInicializator.init();
});
