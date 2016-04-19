class ModulInicializator {
    
}

interface IModulInicializatorConfiguration {
    base?: {
        url?: {
            searchKey?: string;
            pageKey?: string;
            selectionKey?: string;
            sortAscKey?: string;
            sortCriteriaKey?: string;
        };
    };
    bibliographyModule: {
        resultsContainer?: string;
        sortBarContainer?: string;
        sortChangeCallback?: () => void;
        forcedBookType?: BookTypeEnum;
        customConfigurationPath?: string;
    };
    search: {
        container?: HTMLDivElement;
        processSearchJsonCallback?: (jsonData: string, pageNumber?:number) => void;
        processSearchTextCallback?: (text: string) => void;

        enabledOptions: Array<SearchTypeEnum>;
    };
    searchBox: IModulInicializatorConfigurationSearchBox;
    dropDownSelect: {
        dropDownSelectContainer?: string;
        dataUrl: string;
        showStar?: boolean;
    };
}

interface IModulInicializatorConfigurationSearchBox {
    inputFieldElement?: string;
    controllerPath: string;

    dataSet?: {
        name?: string;
        groupHeader?: string;
        parameterUrlString?: string;
    };
    searchBoxInputSelector?: string;
    searchUrl: IModulInicializatorConfigurationSearchBoxSearchUrl;
}

interface IModulInicializatorConfigurationSearchBoxSearchUrl {
    advanced: string;
    text: string;
    textCount: string;
}
