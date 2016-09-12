$(document).ready(() => {
    const modulInicializator = new ListModulInicializator({
            bibliographyModule: {
                forcedBookType: BookTypeEnum.Edition,
                customConfigurationPath: "Editions/Editions/GetListConfiguration"
            },
            search: {
                enabledOptions: [
                    SearchTypeEnum.Title,
                    SearchTypeEnum.Author,
                    SearchTypeEnum.Editor,
                    SearchTypeEnum.Dating
                ],
                url: {
                    advanced: getBaseUrl() + "Editions/Editions/AdvancedSearchPaged",
                    text: getBaseUrl() + "Editions/Editions/TextSearchPaged",
                    textCount: getBaseUrl() + "Editions/Editions/TextSearchCount"
                }
            },
            searchBox: {
                controllerPath: "Editions/Editions"
            },
            dropDownSelect: {
                dataUrl: getBaseUrl() + "Editions/Editions/GetEditionsWithCategories"
            }
        });
        modulInicializator.init();
    }
);
