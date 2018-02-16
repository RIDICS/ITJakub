class BohemianTextBankBase {
    protected searchResultsOnPage = 10;//corresponds to amount of results per page that should be on screen
    protected contextLength = 50;

    protected minContextLength = 40;//search backend may crash if context is too short
    protected maxContextLength = 100;
    protected minResultsPerPage = 1;
    protected maxResultsPerPage = 50;

    protected hitBookIds:number[] = [];

    protected compositionResultListStart = -1;
    protected compositionsPerPage = 10;
    protected compositionPageIsLast = false;

    protected currentBookId = -1;
    protected currentBookIndex = 0;
    protected currentResultStart = -1;
    protected currentViewPage = 1;
    protected totalViewPages = 0;

    protected defaultErrorMessage =
        "Vyhledávání se nezdařilo. Ujistěte se, zda máte zadáno alespoň jedno kritérium na vyhledávání v textu.";

    protected urlSearchKey = "search";
    protected urlSelectionKey = "selected";
    protected urlSortAscKey = "sortAsc";
    protected urlSortCriteriaKey = "sortCriteria";
    protected urlContextSizeKey = "contextSize";
    protected urlResultPerPageKey = "resultsPerPage";

    protected readyForInit = false;
    protected notInitialized = true;

    protected bookIdsInQuery = new Array();
    protected categoryIdsInQuery = new Array();

    protected booksSelector: DropDownSelect2;
    protected sortBar: SortBar;

    protected enabledOptions = new Array<SearchTypeEnum>();

    protected search: Search;

    protected showLoading(tableEl: JQuery) {
        const loaderEl = tableEl.siblings(".corpus-search-results-table-div-loader");
        tableEl.hide();
        loaderEl.empty();
        loaderEl.show();
        loaderEl.addClass("loader");
    }


    protected hideLoading(tableEl: JQuery) {
        const loaderEl = tableEl.siblings(".corpus-search-results-table-div-loader");
        loaderEl.removeClass("loader");
        loaderEl.hide();
        tableEl.show();
    }

    protected printErrorMessage(message: string, loaderEl: JQuery<HTMLElement>) {
        const tableEl = loaderEl.siblings(".text-results-table");
        this.hideLoading(tableEl);
        const errorAlert = new AlertComponentBuilder(AlertType.Error);
        errorAlert.addContent(message);
        loaderEl.empty();
        loaderEl.append(errorAlert.buildElement());
        loaderEl.show();
    }

    protected initializeFromUrlParams() {
        if (this.readyForInit && this.notInitialized) {

            this.notInitialized = false;

            const contextSize = getQueryStringParameterByName(this.urlContextSizeKey);
            if (contextSize) {
                const contextLengthNumber = parseInt(contextSize);
                if (!isNaN(contextLengthNumber)) {
                    if (contextLengthNumber >= this.minContextLength && contextLengthNumber <= this.maxContextLength) {
                        this.contextLength = contextLengthNumber;
                        $("#contextPositionsSelect").val(contextLengthNumber);
                    }
                }
            }

            const resultPerPage = getQueryStringParameterByName(this.urlResultPerPageKey);
            if (resultPerPage) {
                const resultsPerPageNumber = parseInt(resultPerPage);
                if (!isNaN(resultsPerPageNumber)) {
                    if (resultsPerPageNumber >= this.minResultsPerPage && resultsPerPageNumber <= this.maxResultsPerPage) {
                        this.searchResultsOnPage = resultsPerPageNumber;
                        $("#number-of-results-per-viewing-page").val(resultPerPage);
                    }
                }
            }

            const sortedAsc = getQueryStringParameterByName(this.urlSortAscKey);
            const sortCriteria = parseInt(getQueryStringParameterByName(this.urlSortCriteriaKey));

            if (sortedAsc && sortCriteria) {
                this.sortBar.setSortedAsc(sortedAsc === "true");
                this.sortBar.setSortCriteria(sortCriteria as SortEnum);
            }

            const selected = getQueryStringParameterByName(this.urlSelectionKey);

            const searched = getQueryStringParameterByName(this.urlSearchKey);
            this.search.writeTextToTextField(searched);

            if (selected) {
                this.booksSelector.restoreFromSerializedState(selected);
            }

        } else if (!this.notInitialized) {
            this.search.processSearch();
        } else {
            this.readyForInit = true;
        }

    }

    protected fillResultTable(results: ICorpusSearchResult[], query: string, tableSectionEl: JQuery) {
        const textColumn = tableSectionEl.find(".result-text-col");
        const textResultTableEl = textColumn.find(".text-results-table-body");
        const undefinedReplaceString = "<Nezadáno>";
        for (let i = 0; i < results.length; i++) {
            const result = results[i];
            const pageContext = result.pageResultContext;
            const verseContext = result.verseResultContext;
            const bibleVerseContext = result.bibleVerseResultContext;
            const contextStructure = pageContext.contextStructure;
            const bookId = result.bookId;
            const pageId = pageContext.id;
            const acronym = result.sourceAbbreviation;
            const notes = result.notes;
            const textResult = $(`<tr class="row result-row"></tr>`);
            if (i % 2 === 1) {
                textResult.addClass("even");
            } else {
                textResult.addClass("odd");
            }
            textResult.attr("data-bookId", bookId);
            textResult.attr("data-author", result.author);
            textResult.attr("data-title", result.title);
            textResult.attr("data-dating", result.originDate);
            textResult.attr("data-pageId", pageId);
            textResult.attr("data-pageName", pageContext.name);
            textResult.attr("data-acronym", acronym);

            if (verseContext) {
                textResult.attr("data-verseXmlId", verseContext.verseXmlId);
                textResult.attr("data-verseName", verseContext.verseName);
            }

            if (bibleVerseContext) {
                textResult.attr("data-bibleBook", bibleVerseContext.bibleBook);
                textResult.attr("data-bibleChapter", bibleVerseContext.bibleChapter);
                textResult.attr("data-bibleVerse", bibleVerseContext.bibleVerse);
            }

            const contextBefore = $(`<td class="context-before"></td>`);
            contextBefore.text(contextStructure.before);

            const contextMatch = $(`<td class="text-center"></td>`);
            contextMatch.append(`<span class="match">${contextStructure.match}</span>`);

            const contextAfter = $(`<td class="context-after"></td>`);
            contextAfter.text(contextStructure.after);

            const abbrevHref = $("<a></a>");
            abbrevHref.prop("href",
                `${getBaseUrl()}Editions/Editions/Listing?bookId=${bookId}&searchText=${query
                }&page=${pageId}`);
            abbrevHref.text(acronym ? acronym : undefinedReplaceString);
            const abbrevTd = $(`<td class="abbrev-col"></td>`);
            abbrevTd.append(abbrevHref);

            textResult.append(abbrevTd);
            textResult.append(contextBefore);
            textResult.append(contextMatch);
            textResult.append(contextAfter);

            textResultTableEl.append(textResult);

            if (notes) {

                var notesTr = $("<tr></tr>");
                notesTr.addClass("notes");

                var tdNotes = $("<td></td>");
                tdNotes.attr("colSpan", 2);


                for (var j = 0; j < notes.length; j++) {
                    var noteSpan = $("<span></span>");
                    noteSpan.html(notes[j]);
                    noteSpan.addClass("note");
                    tdNotes.append(noteSpan);
                }


                notesTr.append(tdNotes);

                var beforeNotesTr = $("<tr></tr>");
                beforeNotesTr.addClass("notes spacer");

                var afterNotesTr = $("<tr></tr>");
                afterNotesTr.addClass("notes spacer");

                textResultTableEl.append(beforeNotesTr);
                textResultTableEl.append(notesTr);
                textResultTableEl.append(afterNotesTr);

            }

        }

        const tableEl = textColumn.find(".text-results-table");
        tableEl.tableHeadFixer({ "left": 1, "head": false });
        this.hideLoading(tableEl);//ensure table is visible before calculating offset
        //scroll from left to center match column in table
        const matchEl = textResultTableEl.children("tr").first().find(".text-center");
        const matchPosition = matchEl.position().left;
        const abbrColWidth = textResultTableEl.children("tr").first().find(".abbrev-col").width();
        var scrollOffset = matchPosition - ((textColumn.width() + abbrColWidth - matchEl.width()) / 2);
        textColumn.scrollLeft(scrollOffset);
    }

    protected printDetailInfo(tableRowEl: JQuery, detailSectionEl: JQuery, query:string) {
        const undefinedReplaceString = "<Nezadáno>";
        const detailAuthorEl = detailSectionEl.find(".detail-author");
        const detailTitleEl = detailSectionEl.find(".detail-title");
        const detailDatingEl = detailSectionEl.find(".detail-dating");
        const detailDatingCenturyEl = detailSectionEl.find(".detail-dating-century");
        const detailAbbrevEl = detailSectionEl.find(".detail-abbrev");
        const editionNoteEl = detailSectionEl.find(".detail-edition-note-href");
        const detailPholioEl = detailSectionEl.find(".detail-folio");
        const detailVerseEl = detailSectionEl.find(".detail-vers");
        const detailBibleVerseBookEl = detailSectionEl.find(".detail-bible-vers-book");
        const detailBibleVerseChapterEl = detailSectionEl.find(".detail-bible-vers-chapter");
        const detailBibleVerseVerseEl = detailSectionEl.find(".detail-bible-vers-vers");

        detailAuthorEl.text(tableRowEl.data("author") ? tableRowEl.data("author") : undefinedReplaceString);
        detailTitleEl.text(tableRowEl.data("title") ? tableRowEl.data("title") : undefinedReplaceString);
        detailDatingEl.text(tableRowEl.data("dating") ? tableRowEl.data("dating") : undefinedReplaceString);
        detailDatingCenturyEl.text(undefinedReplaceString); //TODO ask where is this info stored
        detailAbbrevEl.text(tableRowEl.data("acronym") ? tableRowEl.data("acronym") : undefinedReplaceString);

        //Edition note
        const editionNoteAnchor = editionNoteEl;
        const bookId = tableRowEl.data("bookid");
        editionNoteAnchor.prop("href", `/EditionNote/EditionNote?bookId=${bookId}`);

        const folioHref = $("<a></a>");
        const pageId = tableRowEl.data("pageid");
        folioHref.prop("href",
            `${getBaseUrl()}Editions/Editions/Listing?bookId=${bookId}&searchText=${
            query}&page=${pageId}`);
        folioHref.text(tableRowEl.data("pagename") ? tableRowEl.data("pagename") : undefinedReplaceString);

        detailPholioEl.empty().append(folioHref);

        detailVerseEl.text(tableRowEl.data("verseName") ? tableRowEl.data("verseName") : undefinedReplaceString);
        detailBibleVerseBookEl
            .text(tableRowEl.data("bibleBook") ? tableRowEl.data("bibleBook") : undefinedReplaceString);
        detailBibleVerseChapterEl
            .text(tableRowEl.data("bibleChapter") ? tableRowEl.data("bibleChapter") : undefinedReplaceString);
        detailBibleVerseVerseEl
            .text(tableRowEl.data("bibleVerse") ? tableRowEl.data("bibleVerse") : undefinedReplaceString);
    }
}