class DictionaryFavoriteHeadwords {
    private expandButton: string;
    private listContainer: string;
    private mainContainer: string;
    private headwordClickCallback: (bookId: string, entryXmlId: string) => void;

    constructor(mainContainer: string, listContainer: string, expandButton: string) {
        this.expandButton = expandButton;
        this.listContainer = listContainer;
        this.mainContainer = mainContainer;
    }

    public create(headwordClickCallback: (bookId: string, entryXmlId: string) => void) {
        this.headwordClickCallback = headwordClickCallback;
        var self = this;
        $(this.expandButton).click(function () {
            var area = $(self.mainContainer);
            if (!area.hasClass("uncollapsed")) {
                $(this).children().removeClass("glyphicon-collapse-down");
                $(this).children().addClass("glyphicon-collapse-up");
                area.addClass("uncollapsed");
                var actualHeight = area.height();
                var targetHeight = area.css("height", 'auto').height();
                area.height(actualHeight);
                area.animate({
                    height: targetHeight
                });

            } else {
                $(this).children().removeClass("glyphicon-collapse-up");
                $(this).children().addClass("glyphicon-collapse-down");
                area.removeClass("uncollapsed");
                area.animate({
                    height: "100%"
                });
            }
        });

        this.getAllHeadwords();
    }

    public getAllHeadwords() {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordBookmarks",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.showHeadwordList(response);
            }
        });
    }

    public addNewHeadword(bookId: string, entryXmlId: string) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/AddHeadwordBookmark",
            data: {
                bookId: bookId,
                entryXmlId: entryXmlId
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.getAllHeadwords();
            }
        });
    }

    private showHeadwordList(list: Array<IDictionaryFavoriteHeadword>) {
        $(this.listContainer).empty();
        var listDiv = document.createElement("div");

        for (var i = 0; i < list.length; i++) {
            var favoriteHeadword = list[i];

            var wordSpan = document.createElement("span");
            var removeWordSpan = document.createElement("span");
            var textWordSpan = document.createElement("span");

            $(wordSpan).addClass("saved-word");
            
            $(removeWordSpan).addClass("saved-word-remove")
                .addClass("glyphicon")
                .addClass("glyphicon-remove-circle");
            $(removeWordSpan).data("entryXmlId", favoriteHeadword.EntryXmlId);
            $(removeWordSpan).data("bookId", favoriteHeadword.BookId);
            $(removeWordSpan).click(event => {
                var element = event.target;
                $(element).parent(".saved-word").fadeOut(function () {
                    $(element).remove();
                });
                this.removeHeadword(element);
            });

            $(textWordSpan).addClass("saved-word-text");
            $(textWordSpan).text(favoriteHeadword.Headword);
            $(textWordSpan).data("entryXmlId", favoriteHeadword.EntryXmlId);
            $(textWordSpan).data("bookId", favoriteHeadword.BookId);
            $(textWordSpan).click(event => {
                this.goToPageWithSelectedHeadword(event.target);
            });

            wordSpan.appendChild(removeWordSpan);
            wordSpan.appendChild(textWordSpan);
            listDiv.appendChild(wordSpan);
        }

        $(this.listContainer).append(listDiv);
    }

    private goToPageWithSelectedHeadword(element: Element) {
        var entryXmlId = $(element).data("entryXmlId");
        var bookId = $(element).data("bookId");

        this.headwordClickCallback(bookId, entryXmlId);
    }

    private removeHeadword(element: Element) {
        var entryXmlId = $(element).data("entryXmlId");
        var bookId = $(element).data("bookId");

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/RemoveHeadwordBookmark",
            data: {
                bookId: bookId,
                entryXmlId: entryXmlId
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                
            }
        });
    }
}

interface IDictionaryFavoriteHeadword {
    Headword: string;
    EntryXmlId: string;
    BookId: string;
}