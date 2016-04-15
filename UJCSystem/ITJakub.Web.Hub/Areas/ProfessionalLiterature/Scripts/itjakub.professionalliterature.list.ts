$(document).ready(() => {
    const modulInicializator = new ProfessionalLiteratureModulInicializator({
        bibliographyModule: {
            forcedBookType: BookTypeEnum.ProfessionalLiterature
        },
        search: {
            enabledOptions: [
                SearchTypeEnum.Title,
                SearchTypeEnum.Author,
                SearchTypeEnum.Editor,
                SearchTypeEnum.Dating
            ]
        },
        searchBox: {
            controllerPath: "ProfessionalLiterature/ProfessionalLiterature",
            searchUrl: {
                advanced: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchPaged",
                text: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchPaged",
                textCount: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchCount",
                advancedCount: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchResultsCount"
            }
        },
        dropDownSelect: {
            dataUrl: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/GetProfessionalLiteratureWithCategories"
        }
    });
    modulInicializator.init();
});
