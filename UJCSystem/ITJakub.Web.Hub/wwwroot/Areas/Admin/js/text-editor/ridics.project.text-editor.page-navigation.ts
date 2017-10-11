class PageNavigation {

    private readonly main: TextEditorMain;

    constructor(main: TextEditorMain) {
        this.main = main;
    }

    private updateOnlySliderValue = false;
    private pageToSkipTo: number = 0;
    private loadingPages: number[] = [];
    private neededPageLoaded = false;


    init() {
        this.createSlider();
        $(".pages-start").on("scroll resize", () => { this.pageUserOn(); });
        this.attachEventToGoToPageButton();
        this.attachEventInputFieldEnterKey();

        this.trackLoading();
    }

    private createSlider() {
        $(() => {
            var tooltip = $(".slider-tooltip");
            var tooltipText = tooltip.children(".slider-tooltip-text");
            $("#page-slider").slider({
                min: 1,
                max: this.main.getNumberOfPages(),
                step: 1,
                create: function() {
                    tooltipText.text(`Page: ${$(this).slider("value")}`);
                },
                slide(event, ui) {
                    tooltipText.text(`Page: ${ui.value}`);
                    tooltip.show();
                },
                change: () => {
                    if (!this.updateOnlySliderValue) {
                        tooltip.hide();
                        this.refreshSwatch();
                    }
                }
            });
        });
    }

    private updateSlider(pageNumber: number) {
        $("#page-slider").slider("option", "value", pageNumber);
        $(".slider-tooltip-text").text(`Page: ${pageNumber}`);
    }

    private refreshSwatch() {
        const page = $("#page-slider").slider("value");
        this.navigateToPage(page);
    }

    private pageUserOn() {
        const containerXPos = $(".pages-start").offset().left;
        const containerYPos = $(".pages-start").offset().top;
        const element = document.elementFromPoint(containerXPos, containerYPos);
        const jqEl = $(element);
        const page = jqEl.parents(".page-row");
        if (page !== null && typeof page !== "undefined") {
            const pageNumber: number = $(page).data("page");
            if (typeof pageNumber !== "undefined" && pageNumber !== null) {
                this.updateOnlySliderValue = true;
                this.updateSlider(pageNumber);
                this.updateOnlySliderValue = false;
            }
        }
    }

    private attachEventToGoToPageButton() {
        $("#project-resource-preview").on("click",
            ".go-to-page-button",
            () => {
                this.processPageInputField();
            });
    }

    private attachEventInputFieldEnterKey() {
        $("#project-resource-preview").on("keypress",
            ".go-to-page-field",
            (event) => {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode === 13) { //Enter key
                    this.processPageInputField();
                }
            });
    }

    togglePageNumbers(show: boolean) {
        const pageNumberEl = $(".page-number");
        if (show) {
            pageNumberEl.removeClass("invisible");
        } else {
            pageNumberEl.addClass("invisible");
        }
    }

    private processPageInputField() {
        const inputField = $(".go-to-page-field");
        const inputFieldValue = inputField.val() as string;
        if (inputFieldValue === "") {
            alert("You haven't entered anything");
        } else {
            const page = parseInt(inputFieldValue);
            if (page > this.main.getNumberOfPages() || page < 1) {
                alert(`Page ${page} does not exist`);
                inputField.val("");
            } else {
                this.navigateToPage(page);
                inputField.val("");
                inputField.blur();
            }
        }
    }

    private navigateToPage(pageNumber: number) {
        const numberOfPagesToPreload = 10;
        const preloadedPage = pageNumber - numberOfPagesToPreload;
        if (preloadedPage > 1) {
            $(".preloading-pages-spinner").show();
            this.pageToSkipTo = pageNumber;
            for (let i = preloadedPage; i <= pageNumber; i++) {
                const currentPageEl = $(`*[data-page="${i}"]`);
                if (!currentPageEl.hasClass("lazyloaded")) {
                    this.loadingPages.push(i);
                    lazySizes.loader.unveil(currentPageEl[0]);
                }
            }
            if ($(`*[data-page="${pageNumber}"]`).hasClass("lazyloaded")) {
                this.scrollToPage(pageNumber);
                $(".preloading-pages-spinner").hide();
            }
        } else {
            this.scrollToPage(pageNumber);
        }

    }

    private trackLoading() {
        $(".pages-start").on("pageConstructed",
            (event: any) => { //custom event
                var page = (event.page) as number;
                this.loadingPages = this.loadingPages.filter(e => e !== page);
                if (page === this.pageToSkipTo) {
                    this.neededPageLoaded = true;
                }
                if ((this.loadingPages.length === 0) && this.neededPageLoaded) {
                    this.scrollToPage(this.pageToSkipTo);
                    $(".preloading-pages-spinner").hide();
                    this.neededPageLoaded = false;
                }
            });
    }

    private scrollToPage(pageNumber: number) {
        const container = $(".pages-start");
        const pageEl = $(`*[data-page="${pageNumber}"]`);
        const editorPageContainer = ".pages-start";
        const compositionPagePosition = pageEl.offset().top;
        const compositionPageContainerPosition = container.offset().top;
        const scrollTo = compositionPagePosition - compositionPageContainerPosition + container.scrollTop();
        $(`${editorPageContainer}`).scrollTop(scrollTo);
        this.updateOnlySliderValue = true;
        this.updateSlider(pageNumber);
        this.updateOnlySliderValue = false;
    }

}