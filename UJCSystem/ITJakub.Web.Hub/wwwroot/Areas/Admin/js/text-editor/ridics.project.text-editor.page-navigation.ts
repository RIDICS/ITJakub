class TextEditorPageNavigation {

    private readonly main: TextEditorMain;
    private readonly gui: TextEditorGui;

    constructor(main: TextEditorMain, gui: TextEditorGui) {
        this.main = main;
        this.gui = gui;
    }

    private updateOnlySliderValue = false;
    private pageToSkipTo: number = 0;
    private neededPageLoaded = false;
    private skippingToPage = false;


    init(compositionPages: ITextProjectPage[]) {
        var loadingPages: number[] = [];
        this.createSlider(loadingPages, compositionPages);
        $(".pages-start").on("scroll resize",
            () => {
                this.pageUserOn();
            });
        this.attachEventToGoToPageButton(loadingPages, compositionPages);
        this.attachEventInputFieldEnterKey(loadingPages, compositionPages);
        this.trackLoading(loadingPages);
        this.showTooltipOnHover();
    }

    private createSlider(loadingPages: number[], compositionPages: ITextProjectPage[]) {
        $(() => {
            var tooltip = $(".slider-tooltip");
            var tooltipText = tooltip.children(".slider-tooltip-text");
            const thisClass = this;
            $("#page-slider").slider({
                min: 0,
                max: compositionPages.length - 1,
                step: 1,
                create: function() {
                    const pageName = compositionPages[$(this).slider("value")].parentPage.name;
                    tooltipText.text(`Page: ${pageName}`);
                    thisClass.updatePageIndicator(pageName);
                },
                slide(event, ui) {
                    tooltipText.text(`Page: ${compositionPages[ui.value].parentPage.name}`);
                    tooltip.show();
                },
                change: () => {
                    if (!this.updateOnlySliderValue) {
                        tooltip.hide();
                        this.refreshSwatch(loadingPages, compositionPages);
                    }
                }
            });
        });
    }

    private showTooltipOnHover() {
        var tooltip = $(".slider-tooltip");
        $("#project-resource-preview").on("mouseenter", ".page-slider-handle", () => { tooltip.show(); });
        $("#project-resource-preview").on("mouseleave", ".page-slider-handle", () => { tooltip.hide(); });
    }

    private updatePageNames(textId: number) {
        const pageEl = $(`*[data-page="${textId}"]`);
        const pageName = pageEl.data("page-name") as string;
        const index = $(".page-row").index(pageEl);
        $("#page-slider").slider("option", "value", index);
        $(".slider-tooltip-text").text(`Page: ${pageName}`);
        this.updatePageIndicator(pageName);
    }

    private updatePageIndicator(pageName: string) {
        $(".page-indicator").text(pageName);
    }

    private refreshSwatch(loadingPages: number[], compositionPages: ITextProjectPage[]) {
        const pageIdIndex = $("#page-slider").slider("value");
        const pageId = compositionPages[pageIdIndex].id;
        this.navigateToPage(pageId, loadingPages, compositionPages);
    }

    private pageUserOn() {
        const containerXPos = $(".pages-start").offset().left;
        const containerYPos = $(".pages-start").offset().top;
        const element = document.elementFromPoint(containerXPos, containerYPos);
        if (element !== null && typeof element !== "undefined") {
            const pageNumberString = element.getAttribute("data-page");
            if (typeof pageNumberString !== "undefined" && pageNumberString !== null && !this.skippingToPage) {
                const pageNumber = parseInt(pageNumberString);
                this.updateOnlySliderValue = true;
                this.updatePageNames(pageNumber);
                this.updateOnlySliderValue = false;
            }
        }
    }

    private attachEventToGoToPageButton(loadingPages: number[], compositionPages: ITextProjectPage[]) {
        $("#project-resource-preview").on("click",
            ".go-to-page-button",
            () => {
                this.processPageInputField(loadingPages, compositionPages);
            });
    }

    private attachEventInputFieldEnterKey(loadingPages: number[], compositionPages: ITextProjectPage[]) {
        $("#project-resource-preview").on("keypress",
            ".go-to-page-field",
            (event) => {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode === 13) { //Enter key
                    this.processPageInputField(loadingPages, compositionPages);
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

    private processPageInputField(loadingPages: number[], compositionPages: ITextProjectPage[]) {
        const inputField = $(".go-to-page-field");
        const inputFieldValue = inputField.val() as string;
        if (inputFieldValue === "") {
            this.gui.showMessageDialog("Warning", "You haven't entered anything. Please enter a page name.");
        } else {
            const pageEl = $(`*[data-page-name="${inputFieldValue}"]`);
            const pageId = pageEl.data("page");
            if (!pageEl.length) {
                this.gui.showMessageDialog("Warning", `Page ${inputFieldValue} does not exist.`);
                inputField.val("");
            } else {
                this.navigateToPage(pageId, loadingPages, compositionPages);
                inputField.val("");
                inputField.blur();
            }
        }
    }

    private navigateToPage(textId: number, loadingPages: number[], compositionPages: ITextProjectPage[]) {
        const firstId = compositionPages[0].id;
        const numberOfPagesToPreload = 10;
        const preloadedPage = textId - numberOfPagesToPreload;
        if (preloadedPage > firstId) {
            $(".preloading-pages-spinner").show();
            this.pageToSkipTo = textId;
            this.skippingToPage = true;
            for (let i = preloadedPage; i <= textId; i++) {
                const currentPageEl = $(`*[data-page="${i}"]`);
                if (!currentPageEl.hasClass("lazyloaded")) {
                    loadingPages.push(i);
                    lazySizes.loader.unveil(currentPageEl[0]);
                }
            }
            if ($(`*[data-page="${textId}"]`).hasClass("lazyloaded")) {
                this.scrollToPage(textId);
                this.skippingToPage = false;
                $(".preloading-pages-spinner").hide();
            }
        } else {
            this.scrollToPage(textId);
        }

    }

    private trackLoading(loadingPages: number[]) {
        $(".pages-start").on("pageConstructed",
            (event: any) => { //custom event
                var page = (event.page) as number;
                loadingPages = loadingPages.filter(e => e !== page);
                if (page === this.pageToSkipTo) {
                    this.neededPageLoaded = true;
                }
                if ((loadingPages.length === 0) && this.neededPageLoaded) {
                    this.scrollToPage(this.pageToSkipTo);
                    $(".preloading-pages-spinner").hide();
                    this.skippingToPage = false;
                    this.neededPageLoaded = false;
                }
            });
    }

    private scrollToPage(textId: number) {
        const container = $(".pages-start");
        const pageEl = $(`*[data-page="${textId}"]`);
        const editorPageContainer = ".pages-start";
        const compositionPagePosition = pageEl.offset().top;
        const compositionPageContainerPosition = container.offset().top;
        const scrollTo = compositionPagePosition - compositionPageContainerPosition + container.scrollTop();
        $(editorPageContainer).scrollTop(scrollTo);
        this.updateOnlySliderValue = true;
        this.updatePageNames(textId);
        this.updateOnlySliderValue = false;
    }

}