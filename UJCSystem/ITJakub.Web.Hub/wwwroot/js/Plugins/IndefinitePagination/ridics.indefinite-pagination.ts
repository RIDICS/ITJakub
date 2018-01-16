class IndefinitePagination {
    private readonly options: IndefinitePagination.Options;
    private readonly paginationContainer: JQuery;

    constructor(options: IndefinitePagination.Options) {
        this.options = options;
        this.paginationContainer = $(options.container);
    }

    make() {//TODO logic
        const container = this.paginationContainer;
        container.addClass("indefinite-pagination");
        container.empty();
        const innerContainer = $("<div></div>");
        if (this.options.showSlider) {
            innerContainer.append(this.makeSlider());
        }

        innerContainer.append(this.createElements());

        if (this.options.showInput) {
            innerContainer.append(this.createPageInput());
        }

        this.paginationContainer.append(innerContainer);
    }

    previousPageIsAvailable() {
        //TODO logic
    }

    nextPageIsAvailable() {
        //TODO logic
    }

    goToPage(pageNumber: number) {
    //TODO implement logic. Check number of pages
    }

    private makeSlider():JQuery {
        var slider = $("<div></div>");
        //TODO implement logic. Check number of pages
        return slider;
    }

    private createElements() {
        var controlElements = $("<div></div>");
        //TODO logic
        return controlElements;
    }

    private createPageInput() {
        var inputElement = $("<div></div>");
        //TODO logic
        return inputElement;
    }
}