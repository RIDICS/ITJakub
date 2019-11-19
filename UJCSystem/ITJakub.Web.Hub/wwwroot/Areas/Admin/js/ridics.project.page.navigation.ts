class PageNavigation {
    private readonly gui: EditorsGui;
    private readonly pageLoadCallback: (pageId: number) => void;
    private pages: IPage[];
    private index: number;

    constructor(gui: EditorsGui, pageLoadCallback: (pageId: number) => void = null) {
        this.gui = gui;
        this.pageLoadCallback = pageLoadCallback;
    }

    init() {
        this.index = 0;
        this.pageButtonClickProcess();
        this.listenToPageEnteredConfirmation();
        this.reinit();        
    }

    reinit() {
        this.pages = [];
        $(".page-row").toArray().forEach((element, index) => {
            this.pages[index] = {
                id: $(element).data("page-id"),
                name: String($(element).data("name")),
                versionId: $(element).data("version-id"),
                position: $(element).data("position"),
            }
        });

        if (this.hasPages()) {
            this.loadPage(this.index);
            $(`.page-row`).on("click", (event) => {
                const index = Number($(event.currentTarget).data("index"));
                this.loadPage(index);
            });
        }
    }
    
    public hasPages(): boolean {
        return this.pages.length > 0;
    }
    
    private getPageIdByPageName(pageName: string, pages: string[]): number {
        const index = $.inArray(pageName, pages);
        return index;
    }

    private loadNextPage() {
        let index = this.index;
        index++;
        if (index > -1 && index < this.pages.length) {
            this.loadPage(index);
        }        
    }

    private loadPreviousPage() {
        let index = this.index;
        index--;
        if (index > -1 && index < this.pages.length) {
            this.loadPage(index);
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
            this.gui.showInfoDialog(localization.translate("Warning", "RidicsProject").value,
                localization.translate("EnterPageName", "RidicsProject").value);
        } else {
            const namesStringArray: string[] = $.map(this.pages, (x) => { return x.name });
            let index = this.getPageIdByPageName(inputFieldValue, namesStringArray);
            if (index === -1) {
                const minusToDashInputValue = inputFieldValue.replace("-", "–");
                index = this.getPageIdByPageName(minusToDashInputValue, namesStringArray);
            }
            if (index === -1) {
                this.gui.showInfoDialog(localization.translate("Warning", "RidicsProject").value,
                    localization.translateFormat("PageDoesNotExist", [inputFieldValue], "RidicsProject").value);
            } else {
                this.loadPage(index);
                inputField.val("");
            }
        }
    }

    private updatePageIndicator(pageName: string) {
        $(".page-indicator").text(pageName);
    }

    private updateActiveItemInListing(index: number) {
        $(`.page-row[data-index="${index}"]`).addClass("active").siblings(".page-row").removeClass("active");
    }

    private loadPage(index: number) {
        this.index = index;
        const pageName = this.pages[index].name;
        this.updateActiveItemInListing(index);
        this.updatePageIndicator(pageName);
        
        if (this.pageLoadCallback !== null) {
            this.pageLoadCallback.call(null, this.pages[index].id);
        }
    }
}