declare namespace IndefinitePagination {
    interface Options {
        container: HTMLDivElement | JQuery;
        previousPageCallback?: Function;
        nextPageCallback: Function;

        initialCallback?: Function;
        executeInitialCallbackOnInit?: boolean;

        buttonClass?: string;

        loadAllPagesButton?: boolean;
        loadAllPagesButtonContent?: HTMLDivElement | JQuery;
        loadAllPagesCallback?: () => JQuery.Deferred<any>;
        loadPageCallBack?: (pageNumber: number) => void;
        showSlider?: boolean;
        showInput?: boolean;

        pageDoesntExistCallBack?: (pageNumber: number) => void;
    }
}