var DictionaryFavoriteHeadwords = (function () {
    function DictionaryFavoriteHeadwords(mainContainer, listContainer, expandButton) {
        this.expandButton = expandButton;
        this.listContainer = listContainer;
        this.mainContainer = mainContainer;
    }
    DictionaryFavoriteHeadwords.prototype.create = function (headwordClickCallback) {
        this.headwordClickCallback = headwordClickCallback;
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
            }
            else {
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
    };
    DictionaryFavoriteHeadwords.prototype.getAllHeadwords = function () {
        var _this = this;
        $(this.listContainer).addClass("loading");
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordBookmarks",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                $(_this.listContainer).removeClass("loading");
                _this.showHeadwordList(response);
            }
        });
    };
    DictionaryFavoriteHeadwords.prototype.addNewHeadword = function (bookId, entryXmlId) {
        var _this = this;
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
            success: function (response) {
                _this.getAllHeadwords();
            }
        });
    };
    DictionaryFavoriteHeadwords.prototype.showHeadwordList = function (list) {
        var _this = this;
        $(this.listContainer).empty();
        var listDiv = document.createElement("div");
        var self = this;
        for (var i = 0; i < list.length; i++) {
            var favoriteHeadword = list[i];
            var wordSpan = document.createElement("span");
            var removeWordSpan = document.createElement("span");
            var textWordSpan = document.createElement("span");
            $(wordSpan).addClass("saved-word");
            $(removeWordSpan).addClass("saved-word-remove").addClass("glyphicon").addClass("glyphicon-remove-circle");
            $(removeWordSpan).data("entryXmlId", favoriteHeadword.EntryXmlId);
            $(removeWordSpan).data("bookId", favoriteHeadword.BookId);
            $(removeWordSpan).click(function (event) {
                var element = event.target;
                $(element).parent(".saved-word").fadeOut(function () {
                    $(element).remove();
                    self.updateVisibleHeight();
                });
                _this.removeHeadword(element);
            });
            $(textWordSpan).addClass("saved-word-text");
            $(textWordSpan).text(favoriteHeadword.Headword);
            $(textWordSpan).data("entryXmlId", favoriteHeadword.EntryXmlId);
            $(textWordSpan).data("bookId", favoriteHeadword.BookId);
            $(textWordSpan).click(function (event) {
                _this.goToPageWithSelectedHeadword(event.target);
            });
            wordSpan.appendChild(removeWordSpan);
            wordSpan.appendChild(textWordSpan);
            listDiv.appendChild(wordSpan);
        }
        $(this.listContainer).append(listDiv);
        this.updateVisibleHeight();
    };
    DictionaryFavoriteHeadwords.prototype.updateVisibleHeight = function () {
        var area = $(this.mainContainer);
        if (!area.hasClass("uncollapsed"))
            return;
        var actualHeight = area.height();
        var targetHeight = area.css("height", 'auto').height();
        area.height(actualHeight);
        area.animate({
            height: targetHeight
        });
    };
    DictionaryFavoriteHeadwords.prototype.goToPageWithSelectedHeadword = function (element) {
        var entryXmlId = $(element).data("entryXmlId");
        var bookId = $(element).data("bookId");
        this.headwordClickCallback(bookId, entryXmlId);
    };
    DictionaryFavoriteHeadwords.prototype.removeHeadword = function (element) {
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
            success: function (response) {
            }
        });
    };
    return DictionaryFavoriteHeadwords;
})();
//# sourceMappingURL=itjakub.dictionariesFavoriteHeadwords.js.map