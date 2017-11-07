class ImageViewerPageNavigation {

    init(pages: IParentPage[]) {
        this.createSlider(pages);
        this.showTooltipOnHover();
        this.pageButtonClickProcess(pages);
        this.listenToPageEnteredConfirmation(pages);
    }

    private createSlider(pages: IParentPage[]) {
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

    private pageButtonClickProcess(pages: IParentPage[]) {
        $(".page-navigator").on("click",
            ".previous-page",
            () => {
                //TODO check whether prev page exists
                var index = parseInt($(".text-editor-page-slider").slider("option", "value"));
                if (!isNaN(index)) {
                    index--;
                    $(".text-editor-page-slider").slider("value", index);
                    const sliderNeedsUpdate = true;
                    this.loadPage(index, pages, sliderNeedsUpdate);
                }

            });
        $(".page-navigator").on("click",
            ".next-page",
            () => {
                //TODO check whether next page exists
                var index = parseInt($(".text-editor-page-slider").slider("option", "value"));
                if (!isNaN(index)) {
                    index++;
                    $(".text-editor-page-slider").slider("value", index);
                    const sliderNeedsUpdate = true;
                    this.loadPage(index, pages, sliderNeedsUpdate);
                }
            });
    }

    private listenToPageEnteredConfirmation(pages: IParentPage[]) {
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

    private processPageInputField(pages: IParentPage[]) {
        const inputField = $(".go-to-page-field");
        const inputFieldValue = inputField.val() as string;
        if (inputFieldValue === "") {
            alert("Warning. You haven't entered anything. Please enter a page name."); //TODO implement dialog
        } else {
            const namesStringArray: string[] = $.map(pages, (x) => { return x.name });
            const index = this.getPageIdByPageName(inputFieldValue, namesStringArray);
            if (index === -1) {
                alert("No such page.");
            } else {
                const sliderNeedsUpdate = true;
                this.loadPage(index, pages, sliderNeedsUpdate);
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

    private loadPage(index: number, pages: IParentPage[], sliderNeedsUpdate: boolean) {
        const pageName = pages[index].name;
        if (sliderNeedsUpdate) {
            this.updateSlider(index, pageName);
        }
        this.updatePageIndicator(pageName);
        //TODO add logic
    }
}