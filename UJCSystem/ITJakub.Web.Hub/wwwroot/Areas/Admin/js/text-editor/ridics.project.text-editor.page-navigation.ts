class TextEditorPageNavigation {

    private readonly main: TextEditorMain;

    constructor(main: TextEditorMain) {
        this.main = main;
    }

    private updateOnlySliderValue = false;
    private pageToSkipTo: number = 0;
    private neededPageLoaded = false;
    private skippingToPage = false;


    init(compositionPages: IPage[]) {
        var loadingPageIds: number[] = [];
        this.createSlider(loadingPageIds, compositionPages);
        $(".pages-start").on("scroll resize",
            () => {
                this.pageUserOn();
            });
        this.attachEventToGoToPageButton(loadingPageIds, compositionPages);
        this.attachEventInputFieldEnterKey(loadingPageIds, compositionPages);
        this.trackLoading(loadingPageIds);
        this.showTooltipOnHover();
    }

    private createSlider(loadingPageIds: number[], compositionPages: IPage[]) {
        $(() => {
            var tooltip = $(".slider-tooltip");
            var tooltipText = tooltip.children(".slider-tooltip-text");
            const thisClass = this;
            $("#page-slider").slider({
                min: 0,
                max: compositionPages.length - 1,
                step: 1,
                create: function() {
                    const pageName = compositionPages[$(this).slider("value")].name;
                    tooltipText.text(localization.translateFormat("PageName", [pageName], "RidicsProject").value);
                    thisClass.updatePageIndicator(pageName);
                },
                slide(event, ui) {
                    tooltipText.text(localization.translateFormat("PageName", [compositionPages[ui.value].name], "RidicsProject").value);
                    tooltip.show();
                },
                change: () => {
                    if (!this.updateOnlySliderValue) {
                        tooltip.hide();
                        this.refreshSwatch(loadingPageIds, compositionPages);
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

    private updatePageNames(pageId: number) {
        const pageEl = $(`*[data-page-id="${pageId}"]`);
        const pageName = pageEl.data("page-name") as string;
        const index = $(".page-row").index(pageEl);
        $("#page-slider").slider("option", "value", index);
        $(".slider-tooltip-text").text(localization.translateFormat("PageName", [pageName], "RidicsProject").value);
        this.updatePageIndicator(pageName);
    }

    private updatePageIndicator(pageName: string) {
        $(".page-indicator").text(pageName);
    }

    private refreshSwatch(loadingPageIds: number[], compositionPages: IPage[]) {
        const pageIdIndex = $("#page-slider").slider("value");
        const pageId = compositionPages[pageIdIndex].id;
        this.navigateToPage(pageId, loadingPageIds, compositionPages);
    }

    private pageUserOn() {
        const containerXPos = $(".pages-start").offset().left;
        const containerYPos = $(".pages-start").offset().top;
        const element = document.elementFromPoint(containerXPos, containerYPos);
        if (element !== null && typeof element !== "undefined" && $(element).parents(".page-row").length === 1) {
            const pageIdString = $(element).parents(".page-row").data("page-id");
            if (typeof pageIdString !== "undefined" && pageIdString !== null && !this.skippingToPage) {
                const pageId = parseInt(pageIdString);
                this.updateOnlySliderValue = true;
                this.updatePageNames(pageId);
                this.updateOnlySliderValue = false;
            }
        }
    }

    private attachEventToGoToPageButton(loadingPageIds: number[], compositionPages: IPage[]) {
        $("#project-resource-preview").on("click",
            ".go-to-page-button",
            () => {
                this.processPageInputField(loadingPageIds, compositionPages);
            });
    }

    private attachEventInputFieldEnterKey(loadingPageIds: number[], compositionPages: IPage[]) {
        $("#project-resource-preview").on("keypress",
            ".go-to-page-field",
            (event) => {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode === 13) { //Enter key
                    this.processPageInputField(loadingPageIds, compositionPages);
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

    private processPageInputField(loadingPageIds: number[], compositionPages: IPage[]) {
        const inputField = $(".go-to-page-field");
        const inputFieldValue = inputField.val() as string;
        if (inputFieldValue === "") {
            bootbox.alert({
                title: localization.translate("Warning", "RidicsProject").value,
                message: localization.translate("EnterPageName", "RidicsProject").value,
                buttons: {
                    ok: {
                        className: "btn-default"
                }
            }
            });
        } else {
            const pageEl = $(`*[data-page-name="${inputFieldValue}"]`);
            const pageId = pageEl.data("page-id");
            if (!pageEl.length) {
                bootbox.alert({
                    title: localization.translate("Warning", "RidicsProject").value,
                    message: localization.translateFormat("PageDoesNotExist", [inputFieldValue], "RidicsProject").value,
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
                inputField.val("");
            } else {
                this.navigateToPage(pageId, loadingPageIds, compositionPages);
                inputField.val("");
                inputField.blur();
            }
        }
    }

    private getPageById(pageId: number, compositionPages: IPage[]): IPage {
        for (let page of compositionPages) {
            if (page.id === pageId) {
                return page;
            }
        }
        return null;
    }

    private getPageByPosition(position: number, compositionPages: IPage[]): IPage {
        for (let page of compositionPages) {
            if (page.position === position) {
                return page;
            }
        }
        return null;
    }

    private navigateToPage(pageId: number, loadingPageIds: number[], compositionPages: IPage[]) {
        const firstPagePosition = compositionPages[0].position;
        const targetPage = this.getPageById(pageId, compositionPages);
        const numberOfPagesToPreload = 10;
        const preloadedPagePosition = targetPage.position - numberOfPagesToPreload;
        if (preloadedPagePosition > firstPagePosition) { // load 10 previous pages
            $(".preloading-pages-spinner").show();
            this.pageToSkipTo = pageId;
            this.skippingToPage = true;
            for (let i = preloadedPagePosition; i <= targetPage.position; i++) {
                const pageByPosition = this.getPageByPosition(i, compositionPages);
                const currentPageEl = $(`*[data-page-id="${pageByPosition.id}"]`);
                if (!currentPageEl.hasClass("lazyloaded")) {
                    loadingPageIds.push(pageByPosition.id);
                    lazySizes.loader.unveil(currentPageEl[0]);
                }
            }
            if ($(`*[data-page-id="${pageId}"]`).hasClass("lazyloaded")) {
                this.scrollToPage(pageId);
                this.skippingToPage = false;
                $(".preloading-pages-spinner").hide();
            }
        } else {
            this.scrollToPage(pageId);
        }

    }

    private trackLoading(loadingPageIds: number[]) {
        $(".pages-start").on("pageConstructed",
            (event, data: IPageConstructedEventData) => { //custom event
                var pageId = (data.pageId) as number;
                loadingPageIds = loadingPageIds.filter(e => e !== pageId);
                if (pageId === this.pageToSkipTo) {
                    this.neededPageLoaded = true;
                }
                if ((loadingPageIds.length === 0) && this.neededPageLoaded) {
                    this.scrollToPage(this.pageToSkipTo);
                    $(".preloading-pages-spinner").hide();
                    this.skippingToPage = false;
                    this.neededPageLoaded = false;
                }
            });
    }

    private scrollToPage(pageId: number) {
        const container = $(".pages-start");
        const pageEl = $(`*[data-page-id="${pageId}"]`);
        const editorPageContainer = ".pages-start";
        const compositionPagePosition = pageEl.offset().top;
        const compositionPageContainerPosition = container.offset().top;
        const scrollTo = compositionPagePosition - compositionPageContainerPosition + container.scrollTop();
        $(editorPageContainer).scrollTop(scrollTo);
        this.updateOnlySliderValue = true;
        this.updatePageNames(pageId);
        this.updateOnlySliderValue = false;
    }
}