var DictionaryViewer = (function () {
    function DictionaryViewer(headwordListContainer, headwordDescriptionContainer) {
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.headwordListContainer = headwordListContainer;
    }
    DictionaryViewer.prototype.showHeadwords = function (headwords) {
        $(this.headwordListContainer).empty();
        $(this.headwordDescriptionContainer).empty();
        var listUl = document.createElement("ul");
        for (var i = 0; i < headwords.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords[i];
            var headwordSpan = document.createElement("span");
            headwordSpan.innerText = record.Headword;
            $(headwordSpan).addClass("dictionary-result-headword");
            var favoriteGlyphSpan = document.createElement("span");
            $(favoriteGlyphSpan).addClass("glyphicon").addClass("glyphicon-star-empty").addClass("dictionary-result-headword-favorite");
            var aLink = document.createElement("a");
            aLink.href = "?guid=" + record.BookInfo.Guid;
            aLink.innerText = record.BookInfo.Acronym;
            $(aLink).addClass("dictionary-result-headword-book");
            headwordLi.appendChild(headwordSpan);
            headwordLi.appendChild(favoriteGlyphSpan);
            headwordLi.appendChild(aLink);
            listUl.appendChild(headwordLi);
        }
        $(this.headwordListContainer).append(listUl);
    };
    DictionaryViewer.prototype.showHeadwordDescription = function (headword) {
        //TODO
    };
    return DictionaryViewer;
})();
//# sourceMappingURL=itjakub.dictionariesViewer.js.map