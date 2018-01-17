class IndefinitePagination {
    private readonly options: IndefinitePagination.Options;
    private readonly paginationContainer: JQuery;
    private buttonClass = "btn-default";
    private goToPageInput: JQuery;
    private totalNumberOfPages: number;
    private firstPage = 1;
    private currentPage = this.firstPage;

    constructor(options: IndefinitePagination.Options) {
        this.options = options;
        this.paginationContainer = $(options.container);
        if (options.buttonClass) {
            this.buttonClass = options.buttonClass;
        }
    }

    make() { //TODO logic
        const container = this.paginationContainer;
        container.addClass("indefinite-pagination");
        container.empty();
        const innerContainer = $("<div></div>");

        innerContainer.append(this.createElements());

        this.paginationContainer.append(innerContainer);

        this.trackClicks();
    }

    previousPageIsAvailable(): boolean | null {
        if (this.totalNumberOfPages) {
            if (this.currentPage > this.firstPage) {
                return true;
            } else {
                return false;
            }
        }
        return null;
    }

    nextPageIsAvailable(): boolean | null {
        if (this.totalNumberOfPages) {
            if (this.currentPage < this.totalNumberOfPages) {
                return true;
            } else {
                return false;
            }
        }
        return null;
    }

    goToPage(pageNumber: number) {
        if (this.totalNumberOfPages) {
            this.options.loadPageCallBack(pageNumber);
        }
    }

    /**
     * Function to call after load all pages callback has completed successfully
     * @param numberOfPages Total amount of pages
     */
    setTotalNumberOfPages(numberOfPages: number) {
        this.totalNumberOfPages = numberOfPages;
        this.makePageIndicator(numberOfPages);
    }

    getCurrentPage(): number {
        return this.currentPage;
    }

    private makeSlider(currentPage: number, totalNumberOfPages: number): JQuery {
        const slider = $(`<div classs="indefinite-pagination-navigation-slider"></div>`);
        slider.slider({
            range: true,
            min: this.firstPage,
            max: totalNumberOfPages,
            value: currentPage,
            change: this.onSliderChange
        });
        return slider;
    }

    private onSliderChange(event: Event, ui: JQueryUI.SliderUIParams) {
        if (ui.value !== this.currentPage) {
            const pageGoingTo = ui.value;
            this.goToPage(pageGoingTo);
        }
    }

    private createElements() {
        const controlElements = $(`<div class="indefinite-pagination-button-group-elements"></div>`);
        const buttonGroupContainerEl = $(`<div class="btn-group"></div>`);
        const previousButtonEl = $(`<button class="btn indefinite-pagination-prev-page ${this.buttonClass
            }"><span class="glyphicon glyphicon-chevron-left"></span></button>`);
        if (!this.options.previousPageCallback) {
            previousButtonEl.prop("disabled", true);
        }
        buttonGroupContainerEl.append(previousButtonEl);
        if (this.options.loadAllPagesButton) {
            buttonGroupContainerEl.append(
                `<button class="btn indefinite-pagination-load-all ${this.buttonClass
                }"><span class="glyphicon glyphicon-arrow-down"></span></button>`);
        }
        buttonGroupContainerEl.append(
            `<button class="btn indefinite-pagination-next-page ${this.buttonClass
            }"><span class="glyphicon glyphicon-chevron-right"></span></button>`);

        controlElements.append(buttonGroupContainerEl);
        return controlElements;
    }

    private createPageInput() {
        const inputGroup = $(`<div class="input-group"></div>`);
        const placeholderText = ""; //TODO
        const inputEl = $(`<input type="text" class="form-control" placeholder=${placeholderText}>`);
        inputGroup.append(inputEl);
        const buttonContainer = $(`<div class="input-group-btn"></div>`);
        const buttonEl =
            $(`<button class="btn indefinite-pagination-go-to-page ${this.buttonClass
                }"><i class="glyphicon glyphicon-arrow-right"></i></button>`);
        buttonContainer.append(buttonEl);
        inputGroup.append(buttonContainer);
        inputEl.on("keypress",
            (event: JQuery.Event) => {
                this.onGoToPageKey(event);
            });
        buttonEl.on("click",
            () => {
                this.onGoToPageButton();
            });
        this.goToPageInput = inputEl;
        return inputGroup;
    }

    private onGoToPageKey(event: JQuery.Event) {
        if (event.keyCode === 13) {
            this.onGoToPageButton();
        }
    }

    private onGoToPageButton() {
        const pageNumberString = this.goToPageInput.val();
        const pageNumber = Number(pageNumberString);
        this.goToPage(pageNumber);
    }

    private updatePage(pageNumber: number) {
        const indicatorEl = this.paginationContainer.find(".indefinite-pagination-load-all-indicator");
        const text = `${pageNumber} / ${this.totalNumberOfPages}`;
        indicatorEl.text(text);
    }

    private trackClicks() {
        this.paginationContainer.on("click",
            ".indefinite-pagination-next-page",
            () => {
                this.currentPage++;
                if (this.totalNumberOfPages) {
                    if (this.currentPage > this.totalNumberOfPages) {
                        this.currentPage = this.totalNumberOfPages;
                    }else{
                        this.updatePage(this.currentPage);
                    }
                }
                this.options.nextPageCallback();
            });
        this.paginationContainer.on("click",
            ".indefinite-pagination-prev-page",
            () => {
                this.currentPage--;
                if (this.totalNumberOfPages) {
                    if (this.currentPage < this.firstPage) {
                        this.currentPage = this.firstPage;
                    } else {
                        this.updatePage(this.currentPage);
                    }
                }
                this.options.previousPageCallback();
            });
        if (this.options.loadAllPagesButton) {
            this.paginationContainer.on("click",
                ".indefinite-pagination-load-all",
                () => {
                    this.showLoadingSpinner();
                    const deferredObject = this.options.loadAllPagesCallback();
                    deferredObject.done((totalNumberOfPages: number) => {
                        this.setTotalNumberOfPages(totalNumberOfPages);
                    });
                    deferredObject.fail(() => {
                        this.showFullPageListLoadingError();
                    });
                });
        }
    }

    private showFullPageListLoadingError() {
        const loadAllPagesButtonEl = this.paginationContainer.find(".indefinite-pagination-load-all");
        loadAllPagesButtonEl.empty().html(
            `<button class="btn indefinite-pagination-load-all-error ${this.buttonClass
            }"><span class="glyphicon glyphicon-warning-sign"></span></button>`);
    }

    private makePageIndicator(totalNumberOfPages: number) {
        const loadAllPagesButtonEl = this.paginationContainer.find(".indefinite-pagination-load-all");
        const text = `${this.currentPage} / ${this.totalNumberOfPages}`;
        loadAllPagesButtonEl.empty().html(
            `<button class="btn indefinite-pagination-load-all-indicator ${this.buttonClass}">${text}</button>`);
        const innerContainer =
            this.paginationContainer.find(".indefinite-pagination-button-group-elements");
        if (this.options.showSlider) {
            innerContainer.before(this.makeSlider(this.currentPage, totalNumberOfPages));
        }
        if (this.options.showInput) {
            innerContainer.after(this.createPageInput());
        }
    }

    private showLoadingSpinner() {
        const loadAllPagesButtonEl = this.paginationContainer.find(".indefinite-pagination-load-all");
        loadAllPagesButtonEl.empty().html(
            `<button class="btn indefinite-pagination-load-all-loading ${this.buttonClass
            }"><span class="glyphicon glyphicon-spin glyphicon-refresh"></span></button>`);
    }
}