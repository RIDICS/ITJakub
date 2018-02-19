class IndefinitePagination {
    private readonly options: IndefinitePagination.Options;
    private readonly paginationContainer: JQuery;
    private buttonClass = "btn-default";
    private goToPageInput: JQuery;
    private totalNumberOfPages: number;
    private firstPage = 1;
    private currentPage = this.firstPage;
    private basicMode = true;
    private slider: JQuery;
    private wrapped = false;

    constructor(options: IndefinitePagination.Options) {
        this.options = options;
        this.paginationContainer = $(options.container);
        if (options.buttonClass) {
            this.buttonClass = options.buttonClass;
        }
    }

    make() {
        const container = this.paginationContainer;
        container.addClass("indefinite-pagination");
        container.empty();
        const innerContainer = $("<div></div>");

        innerContainer.append(this.createElements());

        this.paginationContainer.append(innerContainer);

        this.trackClicks();
    }

    previousPageIsAvailable(): boolean | null {
        if (!this.isBasicMode()) {
            if (this.currentPage > this.firstPage) {
                return true;
            } else {
                return false;
            }
        }
        return null;
    }

    nextPageIsAvailable(): boolean | null {
        if (!this.isBasicMode()) {
            if (this.currentPage < this.totalNumberOfPages) {
                return true;
            } else {
                return false;
            }
        }
        return null;
    }

    goToPage(pageNumber: number) {
        if (!this.isBasicMode()) {
            if (pageNumber <= this.totalNumberOfPages && pageNumber >= this.firstPage) {
                this.wrapped = false;
                this.options.loadPageCallBack(pageNumber);
            } else {
                this.wrapped = true;
                if (this.options.pageDoesntExistCallBack) {
                    this.options.pageDoesntExistCallBack(pageNumber);
                }
            }
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

    hasBeenWrapped(): boolean {
        return this.wrapped;
    }

    isBasicMode() {
        return this.basicMode;
    }

    reset() {
        this.paginationContainer.off();
        if (this.slider) {
            if (this.slider.slider("instance")){
                this.slider.slider("destroy");
            }
        }
        if (this.goToPageInput) {
            this.goToPageInput.off();
        }
        this.currentPage = this.firstPage;
        this.basicMode = true;
        this.make();
    }

    disable() {
        this.paginationContainer.find("button, input").prop("disabled", true);
        if (this.slider) {
            if (this.slider.slider("instance")) {
                this.slider.slider("disable");
            }
        }
    }

    enable() {
        this.paginationContainer.find("button, input").prop("disabled", false);
        if (this.slider) {
            if (this.slider.slider("instance")) {
                this.slider.slider("enable");
            }
        }
    }

    private makeSlider(currentPage: number, totalNumberOfPages: number): JQuery {
        const slider = $(`<div class="indefinite-pagination-navigation-slider"></div>`);
        const handle = $(`<div class="ui-slider-handle"></div>`);
        const tooltip = $(`<div class="tooltip top indefinite-pagination-tooltip"></div>`);
        const tooltipArrow = $(`<div class="tooltip-arrow"></div>`);
        const tooltipInner = $(`<div class="tooltip-inner"></div>`);
        tooltip.append(tooltipArrow).append(tooltipInner).hide();
        handle.append(tooltip);
        slider.append(handle);

        const showSliderTooltip = () => {
            tooltip.stop(true, true).show();
        };
        const hideSliderTooltip = () => {
            tooltip.fadeOut(600);
        };

        slider.slider({
            min: this.firstPage,
            max: totalNumberOfPages,
            value: currentPage,
            change: (event, ui) => {
                this.onSliderChange(event, ui);
            },
            create: () => {
                tooltipInner.text(slider.slider("value"));
            },
            start: showSliderTooltip,
            stop: hideSliderTooltip,
            slide: (event, ui) => {
                showSliderTooltip();
                tooltipInner.text(ui.value);
            }
        });

        handle.hover((event) => {
            showSliderTooltip();
        });
        handle.mouseout((event) => {
            hideSliderTooltip();
        });

        return slider;
    }

    private onSliderChange(event: Event, ui: JQueryUI.SliderUIParams) {
        if (ui.value !== this.currentPage) {
            const newPage = ui.value;
            const indicatorEl = $(event.target as Node as Element).closest(".indefinite-pagination-load-all-indicator");
            const text = `${newPage} / ${this.totalNumberOfPages}`;
            indicatorEl.text(text);
            this.goToPage(newPage);
        }
    }

    private createElements() {
        const controlElements = $(`<div class="indefinite-pagination-button-group-elements"></div>`);
        const buttonGroupContainerEl = $(`<div class="btn-group full-width-btn-group"></div>`);
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
        const inputGroup = $(`<div class="input-group indefinite-pagination-go-to-page"></div>`);
        const placeholderText = "Go to page..."; //TODO localisation
        const inputEl = $(`<input type="text" class="form-control" placeholder="${placeholderText}">`);
        inputGroup.append(inputEl);
        const buttonContainer = $(`<div class="input-group-btn"></div>`);
        const buttonEl =
            $(`<button class="btn indefinite-pagination-go-to-page-button ${this.buttonClass
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
        const pageNumberString = this.goToPageInput.val() as string;
        const pageNumber = parseInt(pageNumberString);
        if (!isNaN(pageNumber)) {
            this.goToPage(pageNumber);
            this.goToPageInput.val("");
        }
    }

    updatePage(pageNumber: number) {
        this.currentPage = pageNumber;
        if (this.totalNumberOfPages) {
            const indicatorEl = this.paginationContainer.find(".indefinite-pagination-load-all-indicator");
            const text = `${pageNumber} / ${this.totalNumberOfPages}`;
            indicatorEl.text(text);
            if (this.slider) {
                if (this.slider.slider("instance")) {
                    this.slider.slider("option", "value", pageNumber);
                    this.slider.find(".tooltip-inner").text(pageNumber);    
                }
            }
        }
    }

    private trackClicks() {
        this.paginationContainer.on("click",
            ".indefinite-pagination-next-page",
            () => {
                this.onNextPageButtonClick();
            });
        this.paginationContainer.on("click",
            ".indefinite-pagination-prev-page",
            () => {
                this.onPrevPageButtonClick();
            });
        if (this.options.loadAllPagesButton) {
            this.paginationContainer.on("click",
                ".indefinite-pagination-load-all",
                () => {
                    this.showLoadingSpinner();
                    const deferredObject = this.options.loadAllPagesCallback();
                    deferredObject.done((totalNumberOfPages: number) => {
                        this.setTotalNumberOfPages(totalNumberOfPages);
                        this.basicMode = false;
                    });
                    deferredObject.fail(() => {
                        this.showFullPageListLoadingError();
                    });
                });
        }
    }

    private onPrevPageButtonClick() {
        this.currentPage--;
        this.wrapped = false;
        if (this.currentPage < this.firstPage) {
            this.currentPage = this.firstPage;
            this.wrapped = true;
            $(".indefinite-pagination-prev-page").prop("disabled", true);
        }
        this.updatePage(this.currentPage);
        if (this.nextPageIsAvailable()) {
            $(".indefinite-pagination-next-page").prop("disabled", false);
        }
        this.options.previousPageCallback();
    }

    private onNextPageButtonClick() {
        this.currentPage++;
        this.wrapped = false;
        if (!this.isBasicMode()) {
            if (this.currentPage > this.totalNumberOfPages) {
                this.currentPage = this.totalNumberOfPages;
                this.wrapped = true;
                $(".indefinite-pagination-next-page").prop("disabled", true);
            }
            this.updatePage(this.currentPage);
        }
        if (this.previousPageIsAvailable()) {
            $(".indefinite-pagination-prev-page").prop("disabled", false);
        }
        this.options.nextPageCallback();
    }

    private showFullPageListLoadingError() {
        const loadAllPagesButtonEl = this.paginationContainer.find(".indefinite-pagination-load");
        loadAllPagesButtonEl.replaceWith(
            `<div class="btn indefinite-pagination-load-all-error ${this.buttonClass
            }"><span class="glyphicon glyphicon-warning-sign"></span></div>`);
    }

    private makePageIndicator(totalNumberOfPages: number) {
        const loadAllPagesButtonEl = this.paginationContainer.find(".indefinite-pagination-load");
        const text = `${this.currentPage} / ${this.totalNumberOfPages}`;
        loadAllPagesButtonEl.replaceWith(
            `<div class="btn indefinite-pagination-load-all-indicator ${this.buttonClass}">${text}</div>`);
        const innerContainer =
            this.paginationContainer.find(".indefinite-pagination-button-group-elements");
        if (this.options.showSlider) {
            const slider = this.makeSlider(this.currentPage, totalNumberOfPages);
            this.slider = slider;
            innerContainer.before(slider);
        }
        if (this.options.showInput) {
            innerContainer.after(this.createPageInput());
        }
    }

    private showLoadingSpinner() {
        const loadAllPagesButtonEl = this.paginationContainer.find(".indefinite-pagination-load-all");
        loadAllPagesButtonEl.replaceWith(
            `<div class="btn indefinite-pagination-load indefinite-pagination-load-all-loading ${this.buttonClass
            }"><span class="glyphicon glyphicon-spin glyphicon-refresh"></span></div>`);
    }
}