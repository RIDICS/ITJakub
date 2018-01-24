$(document.documentElement).ready(() => {
    const modulInicializator = new ProfessionalLiteratureModulInicializator({
        bibliographyModule: {
            forcedBookType: BookTypeEnum.ProfessionalLiterature,
            customConfigurationPath: "ProfessionalLiterature/ProfessionalLiterature/GetListConfiguration"
        },
        search: {
            enabledOptions: [
                SearchTypeEnum.Title,
                SearchTypeEnum.Author,
                SearchTypeEnum.Editor,
                SearchTypeEnum.Dating
            ],
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
