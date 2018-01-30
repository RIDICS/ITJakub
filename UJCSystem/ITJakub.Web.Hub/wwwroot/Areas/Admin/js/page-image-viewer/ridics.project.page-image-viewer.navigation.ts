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
                create: function() {
                    thisClass.loadPage(0, pages);
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
            } else {
                this.gui.showInfoDialog("Warning", "No more pages on the right.");
            }
        }
    }

    private loadPreviousPage() {
        let index = parseInt($(".text-editor-page-slider").slider("option", "value"));
        if (!isNaN(index)) {
            index--;
            if (index > -1 && index < this.pages.length) {
                $(".text-editor-page-slider").slider("value", index);
            } else {
                this.gui.showInfoDialog("Warning", "No more pages on the left.");
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
        $(document).keypress(
            (event) => {
                var keycode = (event.which);
                switch (keycode) {
                case 106: //j
                    this.loadPreviousPage();
                    break;
                case 107: //k
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
            this.gui.showInfoDialog("Warning", "You haven't entered anything. Please enter a page name.");
        } else {
            const namesStringArray: string[] = $.map(pages, (x) => { return x.name });
            let index = this.getPageIdByPageName(inputFieldValue,
                namesStringArray); //TODO precise page names are needed. Implement partial page name search?
            if (index === -1) {
                const minusToDashInputValue = inputFieldValue.replace("-", "–");
                index = this.getPageIdByPageName(minusToDashInputValue, namesStringArray);
            }
            if (index === -1) {
                this.gui.showInfoDialog("Warning", "No such page.");
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
        tooltipText.text(`Page: ${pageName}`);
    }

    private loadPage(index: number, pages: IPage[]) {
        const pageName = pages[index].name;
        this.updateSliderTooltipText(index, pageName);
        this.updatePageIndicator(pageName);
        this.contentAddition.formImageContent(pages[index].id);
    }
}