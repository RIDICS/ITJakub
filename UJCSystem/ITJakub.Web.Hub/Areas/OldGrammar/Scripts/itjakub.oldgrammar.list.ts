$(document).ready(() => {
    const modulInicializator = new ModulInicializator({
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
            ]
        },
        searchBox: {
            controllerPath: "OldGrammar/OldGrammar",

            searchUrl: {
                advanced: getBaseUrl() + "OldGrammar/OldGrammar/AdvancedSearchPaged",
                text: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchPaged",
                textCount: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchCount"
            }
        },
        dropDownSelect: {
            dataUrl: getBaseUrl() + "OldGrammar/OldGrammar/GetGrammarsWithCategories"
        }
    });
    modulInicializator.init();
});
