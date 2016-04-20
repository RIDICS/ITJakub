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
            ],
            url: {
                advanced: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchPaged",
                text: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchPaged",
                textCount: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchCount",
                advancedCount: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchResultsCount"
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
