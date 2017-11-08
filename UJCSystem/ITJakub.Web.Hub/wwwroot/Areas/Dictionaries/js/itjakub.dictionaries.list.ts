function initDictionaryList() {
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
                ],
                url: {
                    advanced: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryAdvancedSearchPaged",
                    text: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryBasicSearchPaged",
                    advancedCount: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryAdvancedSearchResultsCount",
                    textCount: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryBasicSearchResultsCount"
                },
                favoriteQueries: {
                    bookType: BookTypeEnum.Dictionary,
                    queryType: QueryTypeEnum.List
                }
            },
            searchBox: {
                controllerPath: "Dictionaries/Dictionaries"
            },
            dropDownSelect: {
                dataUrl: getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories"
            }
        });
        modulInicializator.init();
    }
    );
}
