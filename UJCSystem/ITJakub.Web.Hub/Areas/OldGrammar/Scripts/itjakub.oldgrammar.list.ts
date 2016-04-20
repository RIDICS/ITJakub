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
                textCount: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchCount"
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
