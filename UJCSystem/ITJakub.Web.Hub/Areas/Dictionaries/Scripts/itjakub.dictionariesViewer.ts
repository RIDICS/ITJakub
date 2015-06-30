class DictionaryViewer {
    private headwordDescriptionContainer: string;
    private headwordListContainer: string;

    constructor(headwordListContainer: string, headwordDescriptionContainer: string) {
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.headwordListContainer = headwordListContainer;
    }

    public showHeadwords(headwords: IHeadword[]) {
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
            $(favoriteGlyphSpan).addClass("glyphicon")
                .addClass("glyphicon-star-empty")
                .addClass("dictionary-result-headword-favorite");

            headwordLi.appendChild(headwordSpan);
            headwordLi.appendChild(favoriteGlyphSpan);

            for (var j = 0; j < record.Dictionaries.length; j++) {
                var dictionary = record.Dictionaries[j];

                var aLink = document.createElement("a");
                aLink.href = "?guid=" + dictionary.BookGuid + "&xmlEntryId=" + dictionary.XmlEntryId;
                aLink.innerText = dictionary.BookAcronym;
                $(aLink).addClass("dictionary-result-headword-book");
                
                headwordLi.appendChild(aLink);
            }
            
            listUl.appendChild(headwordLi);
        }

        $(this.headwordListContainer).append(listUl);
    }

    private showHeadwordDescription(headword: IHeadword) {
        //TODO
    }
} 

interface IHeadwordBookInfo {
    BookGuid: string;
    BookAcronym: string;
    BookVersionId: string;
    XmlEntryId: string;
}

interface IHeadword {
    Headword: string;
    Dictionaries: Array<IHeadwordBookInfo>;
}