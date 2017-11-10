class ImageViewerPageNavigation {
    private readonly contentAddition: ImageViewerContentAddition;
    private readonly gui: ImageViewerPageGui;

    constructor(contentAddition: ImageViewerContentAddition, gui: ImageViewerPageGui) {
        this.contentAddition = contentAddition;
        this.gui = gui;
    }

    init(pages: IPage[]) {
        this.createSlider(pages);
        this.showTooltipOnHover();
        this.pageButtonClickProcess(pages);
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
                create: function() {
                    const sliderNeedsUpdate = true;
                    thisClass.loadPage(0, pages, sliderNeedsUpdate);
                },
                slide(event, ui) {
                    const index = ui.value;
                    if (!isNaN(index)) {
                        tooltipText.text(`Page: ${pages[index].name}`);
                        tooltip.show();
                    }
                },
                change: (event, ui) => {
                    const index = ui.value;
                    if (!isNaN(index)) {
                        const sliderNeedsUpdate = false;
                        thisClass.loadPage(index, pages, sliderNeedsUpdate);
                    }
                }
            });
        });
    }

    private getPageIdByPageName(pageName: string, pages: string[]): number {
        const index = $.inArray(pageName, pages);
        return index;
    }

    private pageButtonClickProcess(pages: IPage[]) {
        $(".page-navigator").on("click",
            ".previous-page",
            () => {
                var index = parseInt($(".text-editor-page-slider").slider("option", "value"));
                if (!isNaN(index)) {
                    index--;
                    if (index > -1 && index < pages.length) {
                        $(".text-editor-page-slider").slider("value", index);
                    } else {
                        this.gui.showInfoDialog("Warning", "No more pages on the left.");
                    }
                }

            });
        $(".page-navigator").on("click",
            ".next-page",
            () => {
                var index = parseInt($(".text-editor-page-slider").slider("option", "value"));
                if (!isNaN(index)) {
                    index++;
                    if (index > -1 && index < pages.length) {
                        $(".text-editor-page-slider").slider("value", index);
                    } else {
                        this.gui.showInfoDialog("Warning", "No more pages on the right.");
                    }
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
        console.log(inputFieldValue);
        if (inputFieldValue === "") {
            this.gui.showInfoDialog("Warning", "You haven't entered anything. Please enter a page name.");
        } else {
            const namesStringArray: string[] = $.map(pages, (x) => { return x.name });
            const index = this.getPageIdByPageName(inputFieldValue, namesStringArray);
            if (index === -1) {
                this.gui.showInfoDialog("Warning", "No such page.");
            } else {
                const sliderNeedsUpdate = true;
                this.loadPage(index, pages, sliderNeedsUpdate);
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

    private updateSlider(index: number, pageName: string) {
        const tooltip = $(".slider-tooltip");
        const tooltipText = tooltip.children(".slider-tooltip-text");
        tooltipText.text(`Page: ${pageName}`);
        $(".text-editor-page-slider").slider("value", index);
    }

    private loadPage(index: number, pages: IPage[], sliderNeedsUpdate: boolean) {
        const pageName = pages[index].name;
        if (sliderNeedsUpdate) {
            this.updateSlider(index, pageName);
        }
        this.updatePageIndicator(pageName);
        this.contentAddition.formImageContent(pages[index].id);
    }
}