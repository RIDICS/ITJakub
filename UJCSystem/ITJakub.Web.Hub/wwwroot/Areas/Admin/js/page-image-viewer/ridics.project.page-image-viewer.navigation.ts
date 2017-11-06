class ImageViewerPageNavigation {
    private currentPage = 0;

    init(pages: IParentPage[]) {
        this.createSlider(pages);
        this.showTooltipOnHover();
        this.pageButtonClickProcess();
        this.listenToPageEnteredConfirmation();
    }

    private createSlider(pages: IParentPage[]) {
        $(() => {
            var tooltip = $(".slider-tooltip");
            var tooltipText = tooltip.children(".slider-tooltip-text");
            const thisClass = this;
            $(".text-editor-page-slider").slider({
                min: 0,
                max: pages.length - 1,
                step: 1,
                create: function () {
                    const pageName = pages[0].name;
                    tooltipText.text(`Page: ${pageName}`);
                    thisClass.updatePageIndicator(pageName);
                },
                slide(event, ui) {
                    const value = parseInt($(".text-editor-page-slider").slider("option", "value"));
                    if (!isNaN(value)) {
                        tooltipText.text(`Page: ${pages[value].name}`);
                        tooltip.show();
                    }
                },
                change: () => {
                    const pageId = $(".text-editor-page-slider").slider("option", "value") as number;
                    this.loadPage(pageId, pages);
                }
            });
        });
    }

    private pageButtonClickProcess() {
        $(".page-navigator").on("click", ".previous-page", () => {
            //TODO check whether prev page exists
            var value = parseInt($(".text-editor-page-slider").slider("option", "value"));//TODO check NaN
            value--;
            $(".text-editor-page-slider").slider("value", value);
            //TODO add logic
        });
        $(".page-navigator").on("click", ".next-page", () => {
            //TODO check whether next page exists
            var value = parseInt($(".text-editor-page-slider").slider("option", "value"));//TODO check NaN
            value++;
            $(".text-editor-page-slider").slider("value", value);
            //TODO add logic
        });
    }

    private listenToPageEnteredConfirmation() {
            $(".page-menu-row").on("click",
                ".go-to-page-button",
                () => {
                    this.processPageInputField();
                });

            $(".page-menu-row").on("keypress",
                ".go-to-page-field",
                (event) => {
                    var keycode = (event.keyCode ? event.keyCode : event.which);
                    if (keycode === 13) { //Enter key
                        this.processPageInputField();
                    }
                });  
    }

    private processPageInputField() {
        const inputField = $(".go-to-page-field");
        const inputFieldValue = inputField.val() as string;
        if (inputFieldValue === "") {
            alert("Warning. You haven't entered anything. Please enter a page name.");//TODO implement dialog
        } else {
//TODO add logic
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

    private loadPage(pageId: number, pages: IParentPage[]) {
        this.updatePageIndicator("Page name");//TODO add logic
    }
}