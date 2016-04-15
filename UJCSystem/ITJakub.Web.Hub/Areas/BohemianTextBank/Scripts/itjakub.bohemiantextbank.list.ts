function initBohemiaTextBankList() {
    const modulInicializator = new BohemiaTextBankModulInicializator({
        bibliographyModule: {
            forcedBookType: BookTypeEnum.TextBank
        },
        search: {
            enabledOptions: [
                SearchTypeEnum.Title,
                SearchTypeEnum.Author,
                SearchTypeEnum.Editor,
                SearchTypeEnum.Dating,
                SearchTypeEnum.Fulltext,
                SearchTypeEnum.Heading,
                SearchTypeEnum.Sentence,
                SearchTypeEnum.Term,
                SearchTypeEnum.TokenDistance
            ]
        },
        searchBox: {
            controllerPath: "BohemianTextBank/BohemianTextBank",

            searchUrl: {
                advanced: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchPaged",
                text: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchPaged",
                textCount: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchCount",
                advancedCount: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchResultsCount"
            }
        },
        dropDownSelect: {
            dataUrl: getBaseUrl() + "BohemianTextBank/BohemianTextBank/GetCorpusWithCategories"
        }
    });
    modulInicializator.init();
}
