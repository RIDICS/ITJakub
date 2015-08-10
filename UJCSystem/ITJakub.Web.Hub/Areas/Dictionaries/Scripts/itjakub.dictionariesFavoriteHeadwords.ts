class DictionaryFavoriteHeadwords {
    private expandButton: string;
    private listContainer: string;
    private mainContainer: string;
    private headwordList: Array<IDictionaryFavoriteHeadword>;
    private headwordClickCallback: (bookId: string, entryXmlId: string) => void;
    private headwordListChangedCallback: (newList: Array<IDictionaryFavoriteHeadword>) => void;

    constructor(mainContainer: string, listContainer: string, expandButton: string) {
        this.expandButton = expandButton;
        this.listContainer = listContainer;
        this.mainContainer = mainContainer;
    }

    public create(headwordClickCallback: (bookId: string, entryXmlId: string) => void, headwordListChangedCallback: (newList: Array<IDictionaryFavoriteHeadword>) => void = null) {
        this.headwordClickCallback = headwordClickCallback;
        this.headwordListChangedCallback = headwordListChangedCallback;
        var areaInitHeight = $(".dictionary-header", $(this.mainContainer)).innerHeight();
        $(this.mainContainer).height(areaInitHeight);

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
                var targetHeight = $(".dictionary-header", area).innerHeight();
                area.animate({
                    height: targetHeight
                });
            }
        });

        this.getAllHeadwords();
    }

    public getAllHeadwords() {
        $(this.listContainer).addClass("loading");
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordBookmarks",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                $(this.listContainer).removeClass("loading");
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
        this.headwordList = list;
        this.headwordListChanged();
        var listDiv = document.createElement("div");
        var self = this;

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
                    self.updateVisibleHeight();
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
        this.updateVisibleHeight();
    }

    private updateVisibleHeight() {
        var area = $(this.mainContainer);
        if (!area.hasClass("uncollapsed"))
            return;

        var actualHeight = area.height();
        var targetHeight = area.css("height", 'auto').height();
        area.height(actualHeight);
        area.animate({
            height: targetHeight
        });
    }

    private goToPageWithSelectedHeadword(element: Element) {
        var entryXmlId = $(element).data("entryXmlId");
        var bookId = $(element).data("bookId");

        this.headwordClickCallback(bookId, entryXmlId);
    }

    private removeHeadword(element: Element) {
        var entryXmlId = $(element).data("entryXmlId");
        var bookId = $(element).data("bookId");

        for (var i = 0; i < this.headwordList.length; i++) {
            var headword = this.headwordList[i];
            if (headword.BookId === bookId && headword.EntryXmlId === entryXmlId) {
                this.headwordList.splice(i, 1); // remove item from array
                break;
            }
        }

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
        this.headwordListChanged();
    }

    removeHeadwordById(bookId: string, entryXmlId: string) {
        throw new Error("Not implemented");
    }

    private headwordListChanged() {
        if (this.headwordListChangedCallback) {
            var listCopy = this.headwordList.slice();
            this.headwordListChangedCallback(listCopy);
        }
    }
}

interface IDictionaryFavoriteHeadword {
    Headword: string;
    EntryXmlId: string;
    BookId: string;
}