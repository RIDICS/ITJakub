class ReaderPagination {
    protected readonly pagerDisplayPages: number;
    protected clickedMoveToPage: boolean;
    public readerContainer: HTMLElement;
    public pages: Array<BookPage>;
    public actualPageIndex = 0;
    private pageChangedCallback: (pageId: number, pageIndex: number, scrollTo: boolean) => void;

    constructor(readerContainer: HTMLElement) {
        this.pagerDisplayPages = 5;
        this.readerContainer = readerContainer;
    }
    
    init(pageChangedCallback: (pageId: number, pageIndex: number, scrollTo: boolean) => void) {
        this.pageChangedCallback = pageChangedCallback;
    }
    
    createPagination(stepByOneArrows = false): HTMLElement {
        const paginationUl: HTMLUListElement = document.createElement("ul");
        paginationUl.classList.add("pagination", "pagination-sm");

        const toLeft = document.createElement("ul");
        toLeft.classList.add("page-navigation-container", "page-navigation-container-left");

        let liElement: HTMLLIElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        let anchor: HTMLAnchorElement = document.createElement("a");
        anchor.href = "#";
        $(anchor).text("|<");
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.moveToPageNumber(0, true);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        anchor = document.createElement("a");
        anchor.href = "#";
        $(anchor).text("<<");
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex - 5, true);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);


        let toRight;
        
        if(stepByOneArrows) {
            liElement = document.createElement("li");
            $(liElement).addClass("page-navigation page-navigation-left");
            anchor = document.createElement("a");
            anchor.href = "#";
            $(anchor).text("<");
            $(anchor).click((event: JQuery.Event) => {
                event.stopPropagation();
                this.moveToPageNumber(this.actualPageIndex - 1, true);
                return false;
            });
            liElement.appendChild(anchor);
            toLeft.appendChild(liElement);

            toRight = document.createElement("ul");
            toRight.classList.add("page-navigation-container", "page-navigation-container-right");

            liElement = document.createElement("li");
            $(liElement).addClass("page-navigation page-navigation-right");
            anchor = document.createElement("a");
            anchor.href = "#";
            $(anchor).text(">");
            $(anchor).click((event: JQuery.Event) => {
                event.stopPropagation();
                this.moveToPageNumber(this.actualPageIndex + 1, true);
                return false;
            });
            liElement.appendChild(anchor);
            toRight.appendChild(liElement);
        }
        
        toRight = document.createElement("ul");
        toRight.classList.add("page-navigation-container", "page-navigation-container-right");

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        $(anchor).text(">>");
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex + 5, true);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        $(anchor).text(">|");
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.pages.length - 1, true);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-left");
        $(liElement).text("...");
        paginationUl.appendChild(liElement);

        $.each(this.pages, (index, page) => {
            liElement = document.createElement("li");
            $(liElement).addClass("page");
            $(liElement).data("page-index", index);
            anchor = document.createElement("a");
            anchor.href = "#";
            $(anchor).text(page.text);
            $(anchor).click((event: JQuery.Event) => {
                event.stopPropagation();
                this.moveToPage(page.pageId);
                return false;
            });
            liElement.appendChild(anchor);
            paginationUl.appendChild(liElement);
        });

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-right");
        $(liElement).text("...");
        paginationUl.appendChild(liElement);

        var listingContainer = document.createElement("div");
        listingContainer.classList.add("page-navigation-container-helper");
        listingContainer.appendChild(toLeft);
        listingContainer.appendChild(paginationUl);
        listingContainer.appendChild(toRight);

        return listingContainer;
    }

    moveToPageNumber(pageIndex: number, scrollTo: boolean) {
        if (pageIndex < 0) {
            pageIndex = 0;
        } else if (pageIndex >= this.pages.length) {
            pageIndex = this.pages.length - 1;
        }

        if (!scrollTo) {
            this.clickedMoveToPage = true;
        }
        
        this.actualPageIndex = pageIndex;
        this.actualizePagination(pageIndex);

        const pageId = this.pages[pageIndex].pageId;
        this.pageChangedCallback.call(null, pageId, pageIndex, scrollTo);
    }

    actualizePagination(pageIndex: number) {
        const pager = $(this.readerContainer).find("ul.pagination");
        pager.find("li.page-navigation").css("visibility", "visible");
        pager.find("li.more-pages").css("visibility", "visible");
        if (pageIndex === 0) {
            pager.find("li.page-navigation-left").css("visibility", "hidden");
            pager.find("li.more-pages-left").css("visibility", "hidden");
        } else if (pageIndex === this.pages.length - 1) {
            pager.find("li.page-navigation-right").css("visibility", "hidden");
            pager.find("li.more-pages-right").css("visibility", "hidden");
        }

        const pages = $(pager).find(".page");
        $(pages).css("display", "none");
        $(pages).removeClass("page-active");
        const actualPage = $(pages).filter(function (index) {
            return $(this).data("page-index") === pageIndex;
        });

        const displayPagesOnEachSide = (this.pagerDisplayPages - 1) / 2;
        let displayOnRight = displayPagesOnEachSide;
        let displayOnLeft = displayPagesOnEachSide;
        const pagesOnLeft = pageIndex;
        const pagesOnRight = this.pages.length - (pageIndex + 1);
        if (pagesOnLeft <= displayOnLeft) {
            displayOnRight += displayOnLeft - pagesOnLeft;
            pager.find("li.more-pages-left").css("visibility", "hidden");
        } else if (pagesOnRight <= displayOnRight) {
            displayOnLeft += displayOnRight - pagesOnRight;
            pager.find("li.more-pages-right").css("visibility", "hidden");
        }

        const displayedPages = $(pages).filter(function (index) {
            const itemPageIndex = $(this).data("page-index");
            return (itemPageIndex >= pageIndex - displayOnLeft && itemPageIndex <= pageIndex + displayOnRight);
        });
        $(displayedPages).css("display", "inline-block");
        $(actualPage).addClass("page-active");
    }

    moveToPage(pageId: number, scrollTo = false) {
        let pageIndex = -1;
        for (let i = 0; i < this.pages.length; i++) {
            if (this.pages[i].pageId === pageId) {
                pageIndex = i;
                break;
            }
        }
        if (pageIndex >= 0 && pageIndex < this.pages.length) {
            this.moveToPageNumber(pageIndex, scrollTo);
        }
    }
}