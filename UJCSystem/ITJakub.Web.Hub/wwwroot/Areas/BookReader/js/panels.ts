﻿abstract class Panel {
    identificator: string;
    innerContent: HTMLElement;
    sc: ServerCommunication;
    parentReader: ReaderLayout;

    constructor(identificator: string, readerLayout: ReaderLayout, sc: ServerCommunication) {
        this.identificator = identificator;
        this.sc = sc;
        this.parentReader = readerLayout;

        this.innerContent = this.makeBody(this, window);
    }

    protected makeBody(rootReference: Panel, window: Window): HTMLElement {
        throw new Error("Not implemented");
    }

    public getPanelHtml(): HTMLDivElement {
        var panelDiv: HTMLDivElement = document.createElement("div");
        panelDiv.id = this.identificator;
        this.addPanelClass(panelDiv);
        panelDiv.appendChild(this.innerContent);
        return panelDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {

    }

    protected addPanelClass(sidePanelDiv: HTMLDivElement) {
        throw new Error("Not implemented");
    }
}

abstract class ToolPanel extends Panel {
    addPanelClass(sidePanelDiv: HTMLDivElement): void {
        $(sidePanelDiv).addClass("reader-tool-panel");
    }
}

class ContentPanel extends ToolPanel {
    makeBody(rootReference: Panel, window: Window): HTMLElement {
        var bodyDiv: HTMLDivElement = window.document.createElement("div");
        $(bodyDiv).addClass("content-panel-container");
        this.downloadBookContent();
        return bodyDiv;
    }

    private downloadBookContent() {

        $(this.innerContent).empty();
        $(this.innerContent).addClass("loader");
        var bookContent: JQueryXHR = this.sc.getBookContent(this.parentReader.bookId);
        bookContent.done((response: { content: IChapterHieararchyContract[] }) => {
            var ulElement = document.createElement("ul");
            $(ulElement).addClass("content-item-root-list");
            for (var i = 0; i < response.content.length; i++) {
                var chapterItem: IChapterHieararchyContract = response.content[i];
                $(ulElement).append(this.makeContentItem(this.parseJsonItemToContentItem(chapterItem)));
            }

            $(this.innerContent).removeClass("loader");
            $(this.innerContent).empty();
            $(this.innerContent).append(ulElement);

        });
        bookContent.fail(() => {
            $(this.innerContent).empty();
            $(this.innerContent).append(localization.translate("LoadingContentError", "BookReader").value);
        });
    }

    private parseJsonItemToContentItem(chapterItem: IChapterHieararchyContract): ContentItem {
        var pageItem = chapterItem.beginningPageId != null
            ? this.parentReader.pagesById[chapterItem.beginningPageId]
            : null;
        var pageItemText = pageItem != null ? pageItem.text : "";
        return new ContentItem(chapterItem.name, chapterItem.beginningPageId,
            pageItemText, chapterItem.subChapters);
    }

    private makeContentItemChilds(contentItem: ContentItem): HTMLUListElement {
        var childItems: IChapterHieararchyContract[] = contentItem.childBookContentItems;
        if (childItems.length === 0) return null;
        var ulElement = document.createElement("ul");
        $(ulElement).addClass("content-item-list");
        for (var i = 0; i < childItems.length; i++) {
            var jsonItem: IChapterHieararchyContract = childItems[i];
            $(ulElement).append(this.makeContentItem(this.parseJsonItemToContentItem(jsonItem)));
        }
        return ulElement;
    }

    private makeContentItem(contentItem: ContentItem): HTMLLIElement {
        var liElement = document.createElement("li");
        $(liElement).addClass("content-item");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            if (this.parentReader.deviceType === Device.Mobile && !$(".lm_popin").is("div")) {
                $($(".view-control button")[1] as Node as HTMLElement).click();
            }
            this.parentReader.readerLayout.eventHub.emit("navigationClicked", contentItem.referredPageId);
        });
        this.parentReader.readerLayout.on("itemCreated", () => {
            $(hrefElement).click(() => {
                if (this.parentReader.deviceType === Device.Mobile && !$(".lm_popin").is("div")) {
                    $($(".view-control button")[1] as Node as HTMLElement).click();
                }
                this.parentReader.readerLayout.eventHub.emit("navigationClicked", contentItem.referredPageId);
            });
        });

        var textSpanElement = document.createElement("span");
        $(textSpanElement).addClass("content-item-text");
        $(textSpanElement).text(contentItem.text);

        var pageNameSpanElement = document.createElement("span");
        $(pageNameSpanElement).addClass("content-item-page-name");
        if (contentItem.referredPageName !== "") {
            $(pageNameSpanElement).text("[" + contentItem.referredPageName + "]");
        }
        $(hrefElement).append(pageNameSpanElement);
        $(hrefElement).append(textSpanElement);

        $(liElement).append(hrefElement);
        $(liElement).append(this.makeContentItemChilds(contentItem));
        return liElement;
    }
}

class SearchResultPanel extends ToolPanel {
    private searchResultItemsDiv: HTMLDivElement;
    private searchPagingDiv: HTMLDivElement;

    private itemsCount: number;
    private paginator: Pagination;
    private paginatorOptions: Pagination.Options;
    private resultsOnPage;
    private maxPaginatorVisibleElements;

    public getSearchResultDiv(): HTMLDivElement {
        return this.searchResultItemsDiv;
    }

    public getPaginator(): Pagination {
        return this.paginator;
    }

    protected makeBody(rootReference: Panel, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");

        var searchResultItemsDiv = window.document.createElement("div");
        $(searchResultItemsDiv).addClass("reader-search-result-items-div");
        this.searchResultItemsDiv = searchResultItemsDiv;

        var pagingDiv = window.document.createElement("div");
        $(pagingDiv).addClass("reader-search-result-paging pagination-extra-small");
        this.searchPagingDiv = pagingDiv;
        if (this.parentReader.deviceType === Device.Mobile && !$(".lm_popin").is("div")) {
            this.resultsOnPage = Number.POSITIVE_INFINITY;
        } else {
            this.resultsOnPage = 8;
        }
        this.maxPaginatorVisibleElements = 11;

        this.paginatorOptions = {
            container: this.searchPagingDiv,
            maxVisibleElements: this.maxPaginatorVisibleElements,
            callPageClickCallbackOnInit: true
        };
        this.paginator = new Pagination(this.paginatorOptions);


        innerContent.appendChild(this.searchPagingDiv);
        innerContent.appendChild(searchResultItemsDiv);

        return innerContent;
    }

    createPagination(pageChangedCallback: (pageNumber: number) => void, itemsCount: number, currentPage: number) {
        if (this.parentReader.deviceType === Device.Mobile && !$(".lm_popin").is("div")) {
            currentPage = 1;
        }
        this.paginatorOptions.pageClickCallback = pageChangedCallback;
        this.itemsCount = itemsCount;
        this.paginator.make(itemsCount, this.resultsOnPage, currentPage);
    }

    getResultsCountOnPage(): number {
        return this.resultsOnPage;
    }

    showResults(searchResults: SearchHitResult[]) {
        this.clearResults();
        for (var i = 0; i < searchResults.length; i++) {
            var result = searchResults[i];
            var resultItem = this.createResultItem(result);
            this.searchResultItemsDiv.appendChild(resultItem);
        }
    }

    private createResultItem(result: SearchHitResult): HTMLDivElement {
        var resultItemDiv = document.createElement("div");
        $(resultItemDiv).addClass("reader-search-result-item");
        $(resultItemDiv).click(() => {
            if (this.parentReader.deviceType === Device.Mobile && !$(".lm_popin").is("div")) {
                $($(".view-control button")[1] as Node as HTMLElement).click();
            }
            var pageId = Number(result.pageId);
            this.parentReader.readerLayout.eventHub.emit("navigationClicked", pageId);
        });
        this.parentReader.readerLayout.on("itemCreated", () => {
            $(resultItemDiv).click(() => {
                if (this.parentReader.deviceType === Device.Mobile && !$(".lm_popin").is("div")) {
                    $($(".view-control button")[1] as Node as HTMLElement).click();
                }
                var pageId = Number(result.pageId);
                this.parentReader.readerLayout.eventHub.emit("navigationClicked", pageId);
            });
        });

        var pageNameSpan = document.createElement("span");
        $(pageNameSpan).addClass("reader-search-result-name");
        $(pageNameSpan).text(result.pageName);

        var resultBeforeSpan = document.createElement("span");
        $(resultBeforeSpan).addClass("reader-search-result-before");
        $(resultBeforeSpan).text(result.before);

        var resultMatchSpan = document.createElement("span");
        $(resultMatchSpan).addClass("reader-search-result-match");
        $(resultMatchSpan).text(result.match);

        var resultAfterSpan = document.createElement("span");
        $(resultAfterSpan).addClass("reader-search-result-after");
        $(resultAfterSpan).text(result.after);

        resultItemDiv.appendChild(pageNameSpan);
        resultItemDiv.appendChild(resultBeforeSpan);
        resultItemDiv.appendChild(resultMatchSpan);
        resultItemDiv.appendChild(resultAfterSpan);

        return resultItemDiv;
    }

    showLoading() {
        $(this.searchResultItemsDiv).addClass("loader");

    }

    clearLoading() {
        $(this.searchResultItemsDiv).removeClass("loader");
    }

    clearResults() {
        $(this.searchResultItemsDiv).empty();
    }
}

class BookmarksPanel extends ToolPanel {
    actualBookmarkPage: number;
    paginator: Pagination;

    constructor(identificator: string, readerLayout: ReaderLayout, sc: ServerCommunication) {
        super(identificator, readerLayout, sc);
        this.actualBookmarkPage = 1;
    }

    public getPanelHtml(): HTMLDivElement {
        var panelDiv: HTMLDivElement = document.createElement("div");
        panelDiv.id = this.identificator;
        this.addPanelClass(panelDiv);
        panelDiv.appendChild(this.makeBody(this, window));
        return panelDiv;
    }

    protected makeBody(rootReference: Panel, window: Window): HTMLElement {
        var innerContent: HTMLDivElement = window.document.createElement("div");
        this.createBookmarkList(innerContent, rootReference);
        return innerContent;
    }

    public createBookmarkList(innerContent: HTMLElement, rootReference: Panel) {
        const bookmarksPerPage = 20;

        const $bookmarksContainer = $(innerContent).children(".reader-bookmarks-container");
        let bookmarksContainer: HTMLDivElement;

        if ($(innerContent).children(".reader-bookmarks-container").length === 0) {
            bookmarksContainer = document.createElement("div");
            bookmarksContainer.classList.add("reader-bookmarks-container");
            innerContent.appendChild(bookmarksContainer);
        } else {
            $bookmarksContainer.empty();
            bookmarksContainer = <HTMLDivElement>($bookmarksContainer.get(0) as Node);
        }

        var bookmarksHead = document.createElement("h2");
        bookmarksHead.innerHTML = localization.translate("AllBookmarks", "BookReader").value;
        bookmarksHead.classList.add("reader-bookmarks-head");
        bookmarksContainer.appendChild(bookmarksHead);

        var bookmarksContent = document.createElement("div");
        bookmarksContent.classList.add("reader-bookmarks-content");
        bookmarksContainer.appendChild(bookmarksContent);

        var bookmarks = rootReference.parentReader.getBookmarks();

        var pageInContainer: Array<HTMLUListElement> = [];
        var pagesContainer: HTMLDivElement = document.createElement("div");
        bookmarksContent.appendChild(pagesContainer);

        var paginationContainer: HTMLDivElement = document.createElement("div");
        paginationContainer.classList.add("reader-bookmarks-pagination");
        bookmarksContent.appendChild(paginationContainer);

        for (let i = 0; i < Math.ceil(bookmarks.totalCount / bookmarksPerPage); i++) {
            pageInContainer[i] = document.createElement("ul");
            pageInContainer[i].classList.add("reader-bookmarks-content-list");
            pageInContainer[i].setAttribute("data-page-index", (i + 1).toString());
            if (i !== this.actualBookmarkPage) {
                pageInContainer[i].classList.add("hide");
            }

            pagesContainer.appendChild(pageInContainer[i]);
        }

        var currentIndex = 0;
        for (let i = 0; i < bookmarks.positions.length; i++) {
            var bookmarkPosition = bookmarks.positions[i];
            for (let j = 0; j < bookmarkPosition.bookmarks.length; j++) {
                var bookmark = bookmarkPosition.bookmarks[j];
                var bookmarkElement = this.createBookmark(bookmark, rootReference, bookmarkPosition.pageIndex);
                pageInContainer[Math.floor(currentIndex / bookmarksPerPage)].appendChild(bookmarkElement);
                currentIndex++;
            }
        }
        if (this.paginator != null) {
            this.actualBookmarkPage = this.paginator.getCurrentPage();
        }

        this.paginator = new Pagination({
            container: paginationContainer,
            pageClickCallback: (pageNumber: number) => this.showBookmarkPage(pagesContainer, pageNumber),
            callPageClickCallbackOnInit: true
        });
        this.paginator.make(bookmarks.totalCount, bookmarksPerPage, this.actualBookmarkPage);

        $(".pagination", paginationContainer).addClass("pagination-extra-small");
    }

    protected showBookmarkPage(pagesContainer: HTMLDivElement, page: number) {
        $(pagesContainer).children().addClass("hide");
        $(pagesContainer).children(`[data-page-index="${page}"]`).removeClass("hide");
    }

    protected createBookmark(bookmark: IBookPageBookmark, rootReference: Panel, pageIndex: number) {
        const bookmarkItem = document.createElement("li");
        bookmarkItem.classList.add("reader-bookmarks-content-item");

        const bookmarkRemoveIco = document.createElement("a");
        bookmarkRemoveIco.href = "#";
        bookmarkRemoveIco.classList.add("glyphicon", "glyphicon-trash", "bookmark-remote-ico");
        bookmarkItem.appendChild(bookmarkRemoveIco);

        bookmarkRemoveIco.addEventListener("click", (e) => {
            e.preventDefault();

            rootReference.parentReader.persistRemoveBookmark(pageIndex, bookmark.id);
        });

        const bookmarkIco = document.createElement("span");
        bookmarkIco.classList.add("glyphicon", "glyphicon-bookmark", "bookmark-ico");
        if (bookmark.favoriteLabel) {
            $(bookmarkIco).css("color", bookmark.favoriteLabel.color);
            $(bookmarkIco).attr("title", bookmark.favoriteLabel.name);
        }
        bookmarkItem.appendChild(bookmarkIco);

        const pageInfo = rootReference.parentReader.getPageByIndex(pageIndex);
        const page = document.createElement("a");
        page.href = "#";
        $(page).text(pageInfo.text);
        page.classList.add("reader-bookmarks-content-item-page");

        const actionHook = () => {
            var pageId = bookmark.pageId;
            this.parentReader.readerLayout.eventHub.emit("navigationClicked", pageId);
        };
        bookmarkIco.addEventListener("click", actionHook);
        page.addEventListener("click", actionHook);

        bookmarkItem.appendChild(page);
        bookmarkItem.appendChild(document.createTextNode(" "));

        const titleContainer = document.createElement("span");
        titleContainer.classList.add("reader-bookmarks-content-item-title-container");
        bookmarkItem.appendChild(titleContainer);

        const titleInput = document.createElement("input");
        titleInput.classList.add("reader-bookmarks-content-item-title-input", "hide");
        titleInput.value = bookmark.title;
        $(titleInput).attr("maxlength", FavoriteManager.maxTitleLength);
        bookmarkItem.appendChild(titleInput);

        const title = document.createElement("span");
        this.setBookmarkTitle(title, rootReference, bookmark.id, pageIndex, bookmark.title);
        title.classList.add("reader-bookmarks-content-item-title");

        titleContainer.addEventListener("click", () => {
            titleContainer.classList.add("hide");
            titleInput.classList.remove("hide");

            titleInput.focus();
        });

        const updateHook = () => {
            this.setBookmarkTitle(title, rootReference, bookmark.id, pageIndex, titleInput.value);

            titleInput.classList.add("hide");
            titleContainer.classList.remove("hide");
        };
        titleInput.addEventListener("blur", updateHook);
        titleInput.addEventListener("keyup", (e: KeyboardEvent) => {
            if (e.keyCode === 13) {
                updateHook();
            }
        });

        titleContainer.appendChild(title);
        titleContainer.appendChild(document.createTextNode(" "));

        const titleEdit = document.createElement("span");
        titleEdit.classList.add("glyphicon", "glyphicon-pencil", "edit-button");
        titleContainer.appendChild(titleEdit);

        return bookmarkItem;
    }

    protected setBookmarkTitle(titleItem: HTMLElement, rootReference: Panel, bookmarkId: number, pageIndex: number, title: string) {
        rootReference.parentReader.setBookmarkTitle(bookmarkId, pageIndex, title);

        if (!title) {
            title = localization.translate("NoName", "BookReader").value;
        }

        $(titleItem).text(title);
    }

}

abstract class TermsPanel extends ToolPanel {
    protected termClickedCallback: (termId: number, text: string) => void;

    setTermClickedCallback(callback: (termId: number, text: string) => void) {
        this.termClickedCallback = callback;
    }


}

class TermsSearchPanel extends TermsPanel {
    private searchResultItemsDiv: HTMLDivElement;
    private searchResultOrderedList: HTMLOListElement;

    private searchResultItemsLoadDiv: HTMLDivElement;

    makeBody(rootReference: Panel, window: Window): HTMLElement {
        var searchResultDiv = window.document.createElement("div");
        $(searchResultDiv).addClass("reader-search-result-div");

        var searchResultItemsLoadDiv = window.document.createElement("div");
        $(searchResultItemsLoadDiv).addClass("reader-terms-search-result-items-div-load loader");
        this.searchResultItemsLoadDiv = searchResultItemsLoadDiv;
        $(searchResultItemsLoadDiv).hide();
        searchResultDiv.appendChild(searchResultItemsLoadDiv);

        var searchResultItemsDiv = window.document.createElement("div");
        $(searchResultItemsDiv).addClass("reader-terms-search-result-items-div");
        this.searchResultItemsDiv = searchResultItemsDiv;
        searchResultDiv.appendChild(searchResultItemsDiv);

        this.searchResultOrderedList = window.document.createElement("ol");

        this.searchResultItemsDiv.appendChild(this.searchResultOrderedList);

        return searchResultDiv;
    }

    showLoading() {
        $(this.searchResultItemsDiv).hide();
        $(this.searchResultItemsLoadDiv).show();

    }

    clearLoading() {
        $(this.searchResultItemsLoadDiv).hide();
        $(this.searchResultItemsDiv).show();
    }

    clearResults() {
        $(this.searchResultOrderedList).empty();
        $(this.searchResultOrderedList).append(localization.translate("UseSearch", "BookReader").value);
        $(this.searchResultOrderedList).addClass("no-items");
    }

    showResults(searchResults: PageDescription[]) {

        $(this.searchResultOrderedList).empty();
        $(this.searchResultOrderedList).removeClass("no-items");

        for (var i = 0; i < searchResults.length; i++) {
            var result = searchResults[i];
            var resultItem = this.createResultItem(result);
            this.searchResultOrderedList.appendChild(resultItem);
        }

        if (searchResults.length === 0) {
            $(this.searchResultOrderedList).addClass("no-items");
            $(this.searchResultOrderedList).append(localization.translate("NoOccurancesOnPage", "BookReader").value);
        }
    }

    private createResultItem(page: PageDescription): HTMLLIElement {
        var resultItemListElement = document.createElement("li");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            if (this.parentReader.deviceType === Device.Mobile && !$(".lm_popin").is("div")) {
                $($(".view-control button")[1] as Node as HTMLElement).click();
            }
            this.parentReader.readerLayout.eventHub.emit("navigationClicked", page.pageId);
        });
        this.parentReader.readerLayout.on("itemCreated", () => {
            $(hrefElement).click(() => {
                if (this.parentReader.deviceType === Device.Mobile && !$(".lm_popin").is("div")) {
                    $($(".view-control button")[1] as Node as HTMLElement).click();
                }
                this.parentReader.readerLayout.eventHub.emit("navigationClicked", page.pageId);
            });
        });

        var textSpanElement = document.createElement("span");
        $(textSpanElement).text(`[${page.pageName}]`);

        $(hrefElement).append(textSpanElement);

        $(resultItemListElement).append(hrefElement);

        return resultItemListElement;
    }
}

class TermsResultPanel extends TermsPanel {

    private termsResultItemsDiv: HTMLDivElement;
    private termsOrderedList: HTMLOListElement;

    private termsResultItemsLoadDiv: HTMLDivElement;

    makeBody(rootReference: Panel, window: Window): HTMLElement {
        var termsResultDiv = window.document.createElement("div");
        $(termsResultDiv).addClass("reader-terms-result-div");

        var termsResultItemsLoadDiv = window.document.createElement("div");
        $(termsResultItemsLoadDiv).addClass("reader-terms-result-items-div-load loader");
        this.termsResultItemsLoadDiv = termsResultItemsLoadDiv;
        $(termsResultItemsLoadDiv).hide();
        termsResultDiv.appendChild(termsResultItemsLoadDiv);

        var termsResultItemsDiv = window.document.createElement("div");
        $(termsResultItemsDiv).addClass("reader-terms-result-items-div");
        this.termsResultItemsDiv = termsResultItemsDiv;
        termsResultDiv.appendChild(termsResultItemsDiv);

        this.termsOrderedList = window.document.createElement("ol");

        this.termsResultItemsDiv.appendChild(this.termsOrderedList);

        var actualPage = this.parentReader.bookHeader.pages[this.parentReader.actualPageIndex];
        this.loadTermsOnPage(actualPage);
        if (typeof this.termClickedCallback === "undefined") {
            this.termClickedCallback = (termId: number, text: string) => {
                window.location.href = getBaseUrl() + "OldGrammar/OldGrammar/Search?search=" + text;
            };
        }
        return termsResultItemsDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        var page = this.parentReader.getPageByIndex(pageIndex);
        this.loadTermsOnPage(page);
    }

    private loadTermsOnPage(page: BookPage) {

        $(this.termsOrderedList).empty();
        $(this.termsOrderedList).removeClass("no-items");
        $(this.termsResultItemsLoadDiv).show();
        $(this.termsResultItemsDiv).hide();
        var terms: JQueryXHR = this.sc.getTerms(this.parentReader.bookId, page.pageId);
        terms.done((response: { terms: Array<ITermContract> }) => {

            $(this.termsOrderedList).empty();
            $(this.termsOrderedList).removeClass("no-items");
            $(this.termsResultItemsLoadDiv).hide();
            $(this.termsResultItemsDiv).show();

            for (var i = 0; i < response.terms.length; i++) {
                var term = response.terms[i];
                this.termsOrderedList.appendChild(this.createTermItem(term.id, term.name));
            }

            if (response.terms.length === 0 && this.termsOrderedList.innerHTML === "") {
                $(this.termsOrderedList).addClass("no-items");
                $(this.termsOrderedList).append(localization.translate("NoTopicOnPage", "BookReader").value);
            }

        });
        terms.fail(() => {
            if (page.pageId === this.parentReader.getActualPage().pageId) {
                $(this.termsResultItemsLoadDiv).hide();
                $(this.termsResultItemsDiv).show();
                $(this.termsOrderedList).addClass("no-items");
                $(this.termsOrderedList).append(localization.translateFormat("LoadingTopicsError", new Array<string>(page.text), "BookReader").value);
            }
        });
    }

    private createTermItem(termId: number, text: string): HTMLLIElement {
        var termItemListElement = document.createElement("li");

        var hrefElement = document.createElement("a");
        hrefElement.href = "#";
        $(hrefElement).click(() => {
            if (typeof this.termClickedCallback !== "undefined" && this.termClickedCallback !== null) {
                this.termClickedCallback(termId, text);
            }
        });
        this.parentReader.readerLayout.on("itemCreated", () => {
            $(hrefElement).click(() => {
                if (typeof this.termClickedCallback !== "undefined" && this.termClickedCallback !== null) {
                    this.termClickedCallback(termId, text);
                }
            });
        });

        var textSpanElement = document.createElement("span");
        $(textSpanElement).text(`[${text}]`);

        $(hrefElement).append(textSpanElement);

        $(termItemListElement).append(hrefElement);

        return termItemListElement;
    }
}

//end of tool panels

abstract class ContentViewPanel extends Panel {
    addPanelClass(sidePanelDiv: HTMLDivElement): void {
        $(sidePanelDiv).addClass("reader-view-panel");
    }
}

class TextPanel extends ContentViewPanel {
    preloadPagesBefore: number;
    preloadPagesAfter: number;

    private query: string; //search for text search
    private queryIsJson: boolean;

    constructor(identificator: string, readerLayout: ReaderLayout, sc: ServerCommunication) {
        super(identificator, readerLayout, sc);
        this.preloadPagesBefore = 5;
        this.preloadPagesAfter = 10;
    }

    protected makeBody(rootReference: Panel, window: Window): HTMLElement {
        var textContainerDiv: HTMLDivElement = window.document.createElement("div");
        $(textContainerDiv).addClass("reader-text-container");

        document.addEventListener("scroll", (event) => {
            if (!$(event.target).hasClass("reader-text-container")) {
                return
            }
            this.parentReader.clickedMoveToPage = false;

            var pages = $(event.target as Node as HTMLElement).find(".page");
            var minOffset = Number.MAX_VALUE;
            var pageWithMinOffset;
            $.each(pages, (index, page: HTMLElement) => {
                var pageOfsset = Math.abs($(page).offset().top - $(event.target as Node as HTMLElement).offset().top);
                if (minOffset > pageOfsset) {
                    minOffset = pageOfsset;
                    pageWithMinOffset = page;
                }
            });

            var pageId: number = parseInt($(pageWithMinOffset).attr("data-page-xmlId"));
            this.parentReader.readerLayout.eventHub.emit("scrollPage", pageId);
            this.parentReader.readerLayout.eventHub.on("toggleComments", (isChecked: boolean, className: string) => {
                if (isChecked) {
                    $(this.innerContent).addClass(className);
                } else {
                    $(this.innerContent).removeClass(className);
                }
            });
        }, true);

        var textAreaDiv: HTMLDivElement = window.document.createElement("div");
        $(textAreaDiv).addClass("reader-text");

        for (var i = 0; i < rootReference.parentReader.bookHeader.pages.length; i++) {
            let page: BookPage = rootReference.parentReader.bookHeader.pages[i];

            var pageTextDiv: HTMLDivElement = window.document.createElement("div");
            $(pageTextDiv).addClass("page");
            $(pageTextDiv).addClass("unloaded");
            $(pageTextDiv).data("page-name", page.text);
            $(pageTextDiv).attr("data-page-xmlId", page.pageId);
            pageTextDiv.id = page.pageId.toString(); // each page has own id

            var pageNameDiv: HTMLDivElement = window.document.createElement("div");
            $(pageNameDiv).addClass("page-name");
            $(pageNameDiv).html("[" + page.text + "]");

            var pageDiv: HTMLDivElement = window.document.createElement("div");
            $(pageDiv).addClass("page-wrapper");
            $(pageDiv).append(pageTextDiv);
            $(pageDiv).append(pageNameDiv);
            textAreaDiv.appendChild(pageDiv);
        }

        var dummyPage: HTMLDivElement = window.document.createElement("div");
        $(dummyPage).addClass("dummy-page");
        textAreaDiv.appendChild(dummyPage);

        textContainerDiv.appendChild(textAreaDiv);
        return textContainerDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        //fetch page only if exist
        this.parentReader.hasBookPage(this.parentReader.bookId, this.parentReader.versionId, () => {
            for (var j = 1; pageIndex - j >= 0 && j <= this.preloadPagesBefore; j++) {
                this.displayPage(this.parentReader.bookHeader.pages[pageIndex - j], false);
            }
            for (var i = 1; pageIndex + i < this.parentReader.bookHeader.pages.length && i <= this.preloadPagesAfter; i++) {
                this.displayPage(this.parentReader.bookHeader.pages[pageIndex + i], false);
            }
            this.displayPage(this.parentReader.bookHeader.pages[pageIndex], scrollTo);
        });
    }

    displayPage(page: BookPage, scrollTo: boolean, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageDiv = document.getElementById(page.pageId.toString());
        var pageLoaded: boolean = !($(pageDiv).hasClass("unloaded"));
        var pageSearchUnloaded: boolean = $(pageDiv).hasClass("search-unloaded");
        var pageLoading: boolean = $(pageDiv).hasClass("loading");
        if (!pageLoading) {
            if (pageSearchUnloaded) {
                this.downloadSearchPageById(this.query, this.queryIsJson, page, onSuccess, onFailed);
            } else if (!pageLoaded) {
                this.downloadPageById(page, onSuccess, onFailed);
            } else if (onSuccess !== null) {
                onSuccess();
            }
        } else if (onSuccess !== null) {
            onSuccess();
        }

        if (scrollTo) {
            this.scrollTextToPositionFromTop(0);
            var offset = $(pageDiv).offset();
            if (offset != undefined) {
                this.scrollTextToPositionFromTop(offset.top);
            }

        }
    }

    scrollTextToPositionFromTop(topOffset: number) {
        var scrollableContainer = $(this.innerContent);
        var containerTopOffset = $(scrollableContainer).offset().top;
        $(scrollableContainer).scrollTop(topOffset - containerTopOffset);
    }

    private downloadPageById(page: BookPage, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageContainer = document.getElementById(page.pageId.toString());
        $(pageContainer).addClass("loading");
        var bookPage: JQueryXHR = this.sc.getBookPage(this.parentReader.versionId, page.pageId);
        bookPage.done((response: { pageText: string }) => {
            $(pageContainer).empty();
            $(pageContainer).append(response.pageText);
            $(pageContainer).removeClass("loading");
            $(pageContainer).removeClass("unloaded");

            if (this.parentReader.clickedMoveToPage) {
                this.parentReader.bookHeader.moveToPageNumber(this.parentReader.actualPageIndex, true);
            }

            if (onSuccess != null) {
                onSuccess();
            }
        });
        bookPage.fail(() => {
            $(pageContainer).empty();
            $(pageContainer).removeClass("loading");
            $(pageContainer).append(localization.translateFormat("PageLoadingError", new Array<string>(page.text), "BookReader").value);

            if (onFailed != null) {
                onFailed();
            }
        });
    }

    private downloadSearchPageById(query: string, queryIsJson: boolean, page: BookPage, onSuccess: () => any = null, onFailed: () => any = null) {
        var pageContainer = document.getElementById(page.pageId.toString());
        $(pageContainer).addClass("loading");
        var bookPage: JQueryXHR = this.sc.getBookPageSearch(this.parentReader.versionId, page.pageId, queryIsJson, query);
        bookPage.done((response: { pageText: string }) => {
            $(pageContainer).empty();
            $(pageContainer).append(response.pageText);
            $(pageContainer).removeClass("loading");
            $(pageContainer).removeClass("unloaded");
            $(pageContainer).removeClass("search-unloaded");
            $(pageContainer).addClass("search-loaded");

            if (this.parentReader.clickedMoveToPage) {
                this.parentReader.bookHeader.moveToPageNumber(this.parentReader.actualPageIndex, true);
            }

            if (onSuccess != null) {
                onSuccess();
            }
        });

        bookPage.fail(() => {
            $(pageContainer).empty();
            $(pageContainer).removeClass("loading");
            $(pageContainer).append(localization.translateFormat("PageLoadingErrorWithSearchResults", new Array<string>(page.text), "BookReader").value);

            if (onFailed != null) {
                onFailed();
            }
        });
    }

    public setSearchedQuery(query: string, isJson: boolean) {
        this.query = query;
        this.queryIsJson = isJson;
    }
}

class ImagePanel extends ContentViewPanel {
    protected makeBody(rootReference: Panel, window: Window): HTMLElement {
        var imageContainerDiv: HTMLDivElement = window.document.createElement("div");
        imageContainerDiv.classList.add("reader-image-container");
        return imageContainerDiv;
    }

    public onMoveToPage(pageIndex: number, scrollTo: boolean) {
        if(pageIndex === this.parentReader.actualPageIndex && !$(this.innerContent).is(":empty")) {
            return;
        }
        var pageInfo = this.parentReader.bookHeader.pages[pageIndex];
        $(this.innerContent).empty();
        
        var image: HTMLImageElement = document.createElement("img");
        image.classList.add("reader-image");
        image.src = getBaseUrl() + "Reader/GetBookImage?snapshotId=" + this.parentReader.versionId + "&pageId=" + pageInfo.pageId;

        var imageLink: HTMLAnchorElement = document.createElement("a");
        imageLink.classList.add("no-click-href");
        imageLink.href = image.src;
        imageLink.onclick = (event: MouseEvent) => {
            return event.ctrlKey;
        };

        imageLink.appendChild(image);
        this.innerContent.appendChild(imageLink);

        var zoomOnClick = false;

        var img = new Image();
        img.onload = () => {
            var $innerContent = $(this.innerContent);

            if (zoomOnClick) {
                $innerContent.zoom({on: "click"});
            } else {
                image.setAttribute("data-image-src", image.src);
                new ImageZoom(image, $(this.innerContent));
                //wheelzoom(image);

                var lastWidth = $innerContent.width();
                var lastHeight = $innerContent.height();
                $(window as any).resize(() => {
                    var newWidth = $innerContent.width();
                    var newHeight = $innerContent.height();

                    if (lastWidth !== newWidth || lastHeight !== newHeight) {
                        image.src = image.getAttribute("data-image-src");

                        new ImageZoom(image, $(this.innerContent));


                        lastWidth = newWidth;
                        lastHeight = newHeight;
                    }
                });

            }
        };
        img.src = getBaseUrl() + "Reader/GetBookImage?snapshotId=" + this.parentReader.versionId + "&pageId=" + pageInfo.pageId;

    }
}

class AudioPanel extends ContentViewPanel {
    private trackId: number;
    private numberOfTracks: number;
    private currentTrack: HTMLAudioElement;
    private currentTrackDuration: number;

    protected makeBody(rootReference: Panel, window: Window): HTMLElement {
        var audioContainerDiv: HTMLDivElement = document.createElement("div");
        $(audioContainerDiv).addClass("reader-audio-container");
        $(audioContainerDiv).addClass("loading");

        var trackName = document.createElement("h3");
        $(trackName).addClass("track-name");
        audioContainerDiv.appendChild(trackName);
        var trackSelect = document.createElement("select");
        trackSelect.id = "track-select";
        var book: JQueryXHR = this.sc.getAudioBook(this.parentReader.bookId);
        book.done((response: { audioBook: IAudioBookSearchResultContract }) => {
            this.numberOfTracks = response.audioBook.Tracks.length;
            for (var track of response.audioBook.Tracks) {
                var trackOption = document.createElement("option");
                $(trackOption).prop("value", track.Position - 1);
                $(trackOption).text(track.Name);
                $(trackSelect).append(trackOption);
            }
            for (var recording of response.audioBook.FullBookRecordings) {
                var download = document.createElement("a");
                $(download).addClass("audio-download-href");
                download.href = this.sc.getTrackDownloadUrl(recording.Id, recording.AudioType);
                $(download).html(recording.AudioType);
                $(".full-book").append(download);
            }

            this.trackId = 0;
            this.reloadTrack();
            $(".reader-audio-container").removeClass("loading");
        });
        book.fail(() => {
            $(".reader-audio-container").empty();
            $(".reader-audio-container").append(localization.translate("FailedToLoadAudioBook", "BookReader").value);
            $(".reader-audio-container").removeClass("loading");
        });

        trackSelect.addEventListener("change", () => {
            this.trackId = Number($(trackSelect).val());
            this.reloadTrack();
        });

        audioContainerDiv.appendChild(trackSelect);

        var audioTextDiv = document.createElement("div");
        $(audioTextDiv).addClass("audio-text");
        audioContainerDiv.appendChild(audioTextDiv);

        var audioPlayerContainer = document.createElement("div");
        $(audioPlayerContainer).addClass("audio-player-container");
        audioContainerDiv.appendChild(audioPlayerContainer);

        var audioDownloadDiv = document.createElement("div");
        $(audioDownloadDiv).addClass("audio-download");
        var downloadBookDiv = document.createElement("div");
        $(downloadBookDiv).addClass("full-book");
        $(downloadBookDiv).append(localization.translate("DownloadAudiobook", "BookReader").value);
        audioDownloadDiv.appendChild(downloadBookDiv);
        var downloadTrackDiv = document.createElement("div");
        $(downloadTrackDiv).addClass("track");
        audioDownloadDiv.appendChild(downloadTrackDiv);
        audioContainerDiv.appendChild(audioDownloadDiv);

        return audioContainerDiv;
    }

    private reloadTrack(autoplay: boolean = false) {
        if (this.currentTrack) {
            this.currentTrack.pause();
        }
        var getTrack: JQueryXHR = this.sc.getTrack(this.parentReader.bookId, this.trackId);
        getTrack.done((response: { track: ITrackWithRecordingContract }) => {
            $(".track-name").html(response.track.Name);
            $("#track-select").val(this.trackId);
            $(".audio-text").html(response.track.Text);
            $(".track").html(`${localization.translate("DownloadChapter", "BookReader").value}:`);
            for (var recording of response.track.Recordings) {
                var download = document.createElement("a");
                $(download).addClass("audio-download-href");
                download.href = this.sc.getTrackDownloadUrl(recording.Id, recording.AudioType);
                $(download).html(recording.AudioType);
                $(".track").append(download);
            }
            this.buildAudioPlayer(response.track, autoplay);

        });
        getTrack.fail(() => {
            $(".reader-audio-container").empty();
            $(".reader-audio-container").append(localization.translate("FailedToLoadTrack", "BookReader").value);
        });
    }

    private buildAudioPlayer(track: ITrackWithRecordingContract, autoplay: boolean) {
        this.currentTrack = new Audio();
        if (autoplay) {
            $(this.currentTrack).on("canplay", () => {
                this.currentTrack.play();
            })

        }
        this.currentTrackDuration = this.parseStringTimeToSeconds(track.Recordings[0].Duration);
        for (var recording of track.Recordings) {
            
            var source = document.createElement("source");
            source.src = this.sc.getTrackDownloadUrl(recording.Id, recording.AudioType);
            source.type = recording.MimeType;
            this.currentTrack.appendChild(source);
        }

        var audioContainer = $(".audio-player-container");

        var audioControl = document.createElement("div");
        $(audioControl).addClass("buttons btn-group control-buttons");
        var buttonBack = document.createElement("button");
        $(buttonBack).addClass("glyphicon btn glyphicon-step-backward");
        buttonBack.addEventListener("click", () => {
            if (this.trackId > 0) {
                this.trackId--;
                this.reloadTrack(true);
            }
        });
        audioControl.appendChild(buttonBack);

        var buttonPlay = document.createElement("button");
        $(buttonPlay).addClass(`glyphicon btn ${autoplay ? "glyphicon-pause" : "glyphicon-play"}`);
        buttonPlay.addEventListener("click", () => {
            if (this.currentTrack.paused) {
                $(buttonPlay)
                    .removeClass("glyphicon-play")
                    .addClass("glyphicon-pause");
                this.currentTrack.play();
            } else {
                $(buttonPlay)
                    .removeClass("glyphicon-pause")
                    .addClass("glyphicon-play");
                this.currentTrack.pause();
            }
        });
        audioControl.appendChild(buttonPlay);


        var buttonForward = document.createElement("button");
        $(buttonForward).addClass("glyphicon btn glyphicon-step-forward");
        buttonForward.addEventListener("click", () => {
            if (this.trackId < this.numberOfTracks - 1) {
                this.trackId++;
                this.reloadTrack(true);
            }
        });
        audioControl.appendChild(buttonForward);
        audioContainer.html(audioControl);

        var audioProgress = document.createElement("div");

        var timer = document.createElement("div");
        var currentTimeContainer = document.createElement("span");
        $(this.currentTrack).on("timeupdate", () => {
            currentTimeContainer.innerText = this.getFormattedTime(this.currentTrack.currentTime, this.currentTrackDuration);
        });

        timer.append(currentTimeContainer);
        $(this.currentTrack).on("loadedmetadata", () => {
            currentTimeContainer.innerText = this.getFormattedTime(this.currentTrack.currentTime, this.currentTrackDuration);
            timer.append(` / ${this.getFormattedTime(this.currentTrackDuration, this.currentTrackDuration)}`);
        });
        audioContainer.append(timer);

        var progressBar = document.createElement("div");
        $(progressBar).addClass("full-progressbar");

        var currentProgressEl = document.createElement("div");
        $(currentProgressEl)
            .addClass("current-progress");
        $(this.currentTrack).on("timeupdate loadedmetadata", () => {
            $(currentProgressEl).css("width", this.calculatePercentageProgress(this.currentTrack));
        });

        $(this.currentTrack).on("ended", () => {
            $(buttonPlay)
                .removeClass("glyphicon-pause")
                .addClass("glyphicon-play");
        });
        
        progressBar.append(currentProgressEl);
        audioContainer.append(progressBar);

    }

    private parseStringTimeToSeconds(time: string): number {
        var splittedTime = time.split(":");
        var seconds = 0;
        for (var i = splittedTime.length - 1; i >= 0; i--) {
            seconds += parseInt(splittedTime[i]) * Math.pow(60, (splittedTime.length - 1) - i);
        }
        return seconds;
    }

    private getFormattedTime(timeInSeconds: number, audioLength: number): string {

        var hours = Math.floor(timeInSeconds / 3600);
        timeInSeconds -= hours * 3600;
        var hoursString: string;
        if (Math.floor(audioLength / 3600) !== 0) {
            if (hours < 10) {
                hoursString = `0${hours}`;
            } else {
                hoursString = `${hours}`;
            }
        }
        var minutes = Math.floor(timeInSeconds / 60);
        timeInSeconds -= minutes * 60;
        var minutesString: string;
        if (Math.floor(audioLength / 60) !== 0) {
            if (minutes < 10) {
                minutesString = `0${minutes}`;
            } else {
                minutesString = `${minutes}`;
            }
        }

        var secondsString: string;
        if (timeInSeconds < 10) {
            secondsString = `0${Math.floor(timeInSeconds)}`;
        } else {
            secondsString = `${Math.floor(timeInSeconds)}`;
        }

        return `${(hoursString !== undefined ? `${hoursString}:` : "")}${(minutesString !== undefined ? `${minutesString}:` : "")}${secondsString}`
    }

    private calculatePercentageProgress(audio: HTMLAudioElement): string {
        return `${audio.currentTime / audio.duration * 100}%`;
    }
}