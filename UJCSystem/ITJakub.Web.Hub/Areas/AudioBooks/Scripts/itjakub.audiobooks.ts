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
            ]
        },
        searchBox: {
            controllerPath: "AudioBooks/AudioBooks",
            searchUrl: {
                advanced: getBaseUrl() + "AudioBooks/AudioBooks/AdvancedSearchPaged",
                text: getBaseUrl() + "AudioBooks/AudioBooks/TextSearchPaged",
                textCount: getBaseUrl() + "AudioBooks/AudioBooks/TextSearchCount",
                advancedCount: getBaseUrl() + "AudioBooks/AudioBooks/AdvancedSearchResultsCount"
            }
        },
        dropDownSelect: {
            dataUrl: getBaseUrl() + "AudioBooks/AudioBooks/GetAudioWithCategories"
        }
    });
    modulInicializator.init();
});

//functions used in VariableInterpreter.interpretScript
var aduioTypeTranslation = [
    "Neznámý",
    "Mp3",
    "Ogg",
    "Wav"
];

function translateAudioType(audioType: number): string {
    return aduioTypeTranslation[audioType];
}

function fillLeadingZero(seconds: number): string {
    var secondsString = seconds.toString();
    if (secondsString.length === 1) {
        secondsString = `0${secondsString}`;
    }
    return secondsString;
}
