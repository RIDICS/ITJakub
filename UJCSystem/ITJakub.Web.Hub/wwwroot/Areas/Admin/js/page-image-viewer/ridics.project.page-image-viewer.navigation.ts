class ImageViewerPageNavigation {
    private readonly contentAddition: ImageViewerContentAddition;
    private readonly gui: EditorsGui;
    private pages: IPage[];

    constructor(contentAddition: ImageViewerContentAddition, gui: EditorsGui) {
        this.contentAddition = contentAddition;
        this.gui = gui;
    }

    init(pages: IPage[]) {
        this.createSlider(pages);
        this.showTooltipOnHover();
        this.pages = pages;
        this.pageButtonClickProcess();
        this.listenToPageEnteredConfirmation(pages);
    }

    private createSlider(pages: IPage[]) {
        $(() => {
            const tooltip = $(".slider-tooltip");
            const tooltipText = tooltip.children(".slider-tooltip-text");
            const thisClass = this;
            $(".text-editor-page-slider").slider({
                min: 0,
                max: pages.length - 1,
                step: 1,
                create: () => {
                    this.loadPage(0, pages);
                },
                slide: (event, ui) => {
                    const index = ui.value;
                    if (!isNaN(index)) {
                        tooltipText.text(localization.translateFormat("PageName", [pages[index].name], "RidicsProject").value);
                        tooltip.show();
                    }
                },
                change: (event, ui) => {
                    const index = ui.value;
                    if (!isNaN(index)) {
                        thisClass.loadPage(index, pages);
                    }
                }
            });
        });
    }

    private getPageIdByPageName(pageName: string, pages: string[]): number {
        const index = $.inArray(pageName, pages);
        return index;
    }

    private loadNextPage() {
        let index = parseInt($(".text-editor-page-slider").slider("option", "value"));
        if (!isNaN(index)) {
            index++;
            if (index > -1 && index < this.pages.length) {
                $(".text-editor-page-slider").slider("value", index);
            }
        }
    }

    private loadPreviousPage() {
        let index = parseInt($(".text-editor-page-slider").slider("option", "value"));
        if (!isNaN(index)) {
            index--;
            if (index > -1 && index < this.pages.length) {
                $(".text-editor-page-slider").slider("value", index);
            }
        }
    }

    private pageButtonClickProcess() {
        $(".page-navigator").on("click",
            ".previous-page",
            () => {
                this.loadPreviousPage();
            });
        $(".page-navigator").on("click",
            ".next-page",
            () => {
                this.loadNextPage();
            });
        $(document.documentElement).keyup(
            (event) => {
                var keycode = (event.which);
                switch (keycode) {
                case 37: //left arrow
                    this.loadPreviousPage();
                    break;
                case 39: //right arrow
                    this.loadNextPage();
                    break;
                default:
                }
            });
    }

    private listenToPageEnteredConfirmation(pages: IPage[]) {
        $(".page-menu-row").on("click",
            ".go-to-page-button",
            () => {
                this.processPageInputField(pages);
            });

        $(".page-menu-row").on("keypress",
            ".go-to-page-field",
            (event) => {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode === 13) { //Enter key
                    this.processPageInputField(pages);
                }
            });
    }

    private processPageInputField(pages: IPage[]) {
        const inputField = $(".go-to-page-field");
        const inputFieldValue = inputField.val() as string;
        if (inputFieldValue === "") {
            this.gui.showInfoDialog(localization.translate("Warning", "RidicsProject").value,
                localization.translate("EnterPageName", "RidicsProject").value);
        } else {
            const namesStringArray: string[] = $.map(pages, (x) => { return x.name });
            let index = this.getPageIdByPageName(inputFieldValue, namesStringArray);
            if (index === -1) {
                const minusToDashInputValue = inputFieldValue.replace("-", "–");
                index = this.getPageIdByPageName(minusToDashInputValue, namesStringArray);
            }
            if (index === -1) {
                this.gui.showInfoDialog(localization.translate("Warning", "RidicsProject").value,
                    localization.translateFormat("PageDoesNotExist", [inputFieldValue], "RidicsProject").value);
            } else {
                $(".text-editor-page-slider").slider("value", index);
                inputField.val("");
            }
        }
    }

    private showTooltipOnHover() {
        var tooltip = $(".slider-tooltip");
        $("#project-resource-images").on("mouseenter", ".page-slider-handle", () => { tooltip.show(); });
        $("#project-resource-images").on("mouseleave", ".page-slider-handle", () => { tooltip.hide(); });
    }

    private updatePageIndicator(pageName: string) {
        $(".page-indicator").text(pageName);
    }

    private updateSliderTooltipText(index: number, pageName: string) {
        const tooltip = $(".slider-tooltip");
        const tooltipText = tooltip.children(".slider-tooltip-text");
        tooltipText.text(localization.translateFormat("PageName", [pageName], "RidicsProject").value);
    }

    private loadPage(index: number, pages: IPage[]) {
        const pageName = pages[index].name;
        this.updateSliderTooltipText(index, pageName);
        this.updatePageIndicator(pageName);
        this.contentAddition.formImageContent(pages[index].id);
    }
}