﻿$(document.documentElement).ready(() => {
        const modulInicializator = new ListModulInicializator({
            bibliographyModule: {
                forcedBookType: BookTypeEnum.Edition,
                customConfigurationPath: $("#bibliography-configuration-url").data("url")
            },
            search: {
                enabledOptions: [
                    SearchTypeEnum.Title,
                    SearchTypeEnum.Author,
                    SearchTypeEnum.Editor,
                    SearchTypeEnum.Dating
                ],
                enabledSearchInSecondPortal: true,
                url: {
                    advanced: getBaseUrl() + "Editions/Editions/AdvancedSearchPaged",
                    text: getBaseUrl() + "Editions/Editions/TextSearchPaged",
                    advancedCount: getBaseUrl() + "Editions/Editions/AdvancedSearchResultsCount",
                    textCount: getBaseUrl() + "Editions/Editions/TextSearchCount"
                },
                favoriteQueries: {
                    bookType: BookTypeEnum.Edition,
                    queryType: QueryTypeEnum.List
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
