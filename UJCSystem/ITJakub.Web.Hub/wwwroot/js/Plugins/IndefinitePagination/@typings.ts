declare namespace IndefinitePagination {
    interface Options {
        container: HTMLDivElement | JQuery;
        loadAllPagesButton?: boolean;
        loadAllPagesCallback: Function;
        showSlider?: boolean;
        showInput?: boolean;
        previousPageCallback?: Function;
        nextPageCallback: Function;
        initialCallback?: Function;
        executeInitialCallbackOnInit?: boolean;
    }
}