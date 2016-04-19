function initDictionaryList(bookXmlId: string) {
    $(document).ready(() => {
        const modulInicializator = new ListModulInicializator({
            bibliographyModule: {
                forcedBookType: BookTypeEnum.Dictionary
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
                controllerPath: "Dictionaries/Dictionaries",

                searchUrl: {
                    advanced: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryAdvancedSearchPaged",
                    text: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryBasicSearchPaged",
                    textCount: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryBasicSearchResultsCount"
                }
            },
            dropDownSelect: {
                dataUrl: getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories"
            }
        });
        modulInicializator.init();
    }
    );
}
