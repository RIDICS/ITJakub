$(document).ready(() => {
    const modulInicializator = new AudioBooksModulInicializator({
        bibliographyModule: {
            forcedBookType: BookTypeEnum.AudioBook
        },
        search: {
            enabledOptions: [
                SearchTypeEnum.Title,
                SearchTypeEnum.Author,
                SearchTypeEnum.Editor,
                SearchTypeEnum.Dating
            ],
            url: {
                advanced: getBaseUrl() + "AudioBooks/AudioBooks/AdvancedSearchPaged",
                text: getBaseUrl() + "AudioBooks/AudioBooks/TextSearchPaged",
                textCount: getBaseUrl() + "AudioBooks/AudioBooks/TextSearchCount",
                advancedCount: getBaseUrl() + "AudioBooks/AudioBooks/AdvancedSearchResultsCount"
            },
            favoriteQueries: {
                bookType: BookTypeEnum.AudioBook,
                queryType: QueryTypeEnum.List
            }
        },
        searchBox: {
            controllerPath: "AudioBooks/AudioBooks"
        },
        dropDownSelect: {
            dataUrl: getBaseUrl() + "AudioBooks/AudioBooks/GetAudioWithCategories"
        }
    });
    modulInicializator.init();
});
