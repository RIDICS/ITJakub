class BohemianTextBankBase {
    showLoading() {
        $(".text-results-table").hide();
        $("#corpus-search-results-table-div-loader").empty();
        $("#corpus-search-results-table-div-loader").show();
        $("#corpus-search-results-table-div-loader").addClass("loader");
    }


    hideLoading() {
        $("#corpus-search-results-table-div-loader").removeClass("loader");
        $("#corpus-search-results-table-div-loader").hide();
        $(".text-results-table").show();
    }

    printErrorMessage(message: string) {
        this.hideLoading();
        const corpusErrorDiv = $("#corpus-search-results-table-div-loader");
        corpusErrorDiv.empty();
        corpusErrorDiv.text(message);
        corpusErrorDiv.show();
    }

    fillResultTable(results: ICorpusSearchResult[], query: string) {
        const tableSection = $(".corpus-search-results-div");
        const textColumn = tableSection.find(".result-text-col");
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
        this.hideLoading();//ensure table is visible before calculating offset
        //scroll from left to center match column in table
        const matchEl = textResultTableEl.children("tr").first().find(".text-center");
        const matchPosition = matchEl.position().left;
        const abbrColWidth = textResultTableEl.children("tr").first().find(".abbrev-col").width();
        var scrollOffset = matchPosition - ((textColumn.width() + abbrColWidth - matchEl.width()) / 2);
        textColumn.scrollLeft(scrollOffset);
    }

    emptyResultsTable() {
        const tableBody = $(".text-results-table-body");
        tableBody.empty();
    }

    printDetailInfo(tableRowEl: JQuery, query:string) {
        const undefinedReplaceString = "<Nezadáno>";

        $("#detail-author").text(tableRowEl.data("author") ? tableRowEl.data("author") : undefinedReplaceString);
        $("#detail-title").text(tableRowEl.data("title") ? tableRowEl.data("title") : undefinedReplaceString);
        $("#detail-dating").text(tableRowEl.data("dating") ? tableRowEl.data("dating") : undefinedReplaceString);
        $("#detail-dating-century").text(undefinedReplaceString); //TODO ask where is this info stored
        $("#detail-abbrev").text(tableRowEl.data("acronym") ? tableRowEl.data("acronym") : undefinedReplaceString);

        //Edition note
        const editionNoteAnchor = $("#detail-edition-note-href");
        const bookId = tableRowEl.data("bookid");
        editionNoteAnchor.prop("href", `/EditionNote/EditionNote?bookId=${bookId}`);

        const folioHref = $("<a></a>");
        const pageId = tableRowEl.data("pageid");
        folioHref.prop("href",
            `${getBaseUrl()}Editions/Editions/Listing?bookId=${bookId}&searchText=${
            query}&page=${pageId}`);
        folioHref.text(tableRowEl.data("pagename") ? tableRowEl.data("pagename") : undefinedReplaceString);

        $("#detail-folio").empty().append(folioHref);

        $("#detail-vers").text(tableRowEl.data("verseName") ? tableRowEl.data("verseName") : undefinedReplaceString);
        $("#detail-bible-vers-book")
            .text(tableRowEl.data("bibleBook") ? tableRowEl.data("bibleBook") : undefinedReplaceString);
        $("#detail-bible-vers-chapter")
            .text(tableRowEl.data("bibleChapter") ? tableRowEl.data("bibleChapter") : undefinedReplaceString);
        $("#detail-bible-vers-vers")
            .text(tableRowEl.data("bibleVerse") ? tableRowEl.data("bibleVerse") : undefinedReplaceString);
    }
}