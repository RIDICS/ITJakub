var DictionaryFavoriteHeadwords = (function () {
    function DictionaryFavoriteHeadwords(mainContainer, listContainer, expandButton) {
        this.expandButton = expandButton;
        this.listContainer = listContainer;
        this.mainContainer = mainContainer;
    }
    DictionaryFavoriteHeadwords.prototype.create = function () {
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
                area.animate({
                    height: "100%"
                });
            }
        });
        //$(".saved-word-remove").click(function () {
        //    $(this).parent(".saved-word").fadeOut(function () {
        //        $(this).remove();
        //    }); //TODO populate request to remove on server
        //});
        //$(".saved-word-text").click(function () {
        //    alert("here should be request for new search with word: " + $(this).text());
        //}); //TODO populate request to add word on server
        this.getAllHeadwords();
    };
    DictionaryFavoriteHeadwords.prototype.getAllHeadwords = function () {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordBookmarks",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.showHeadwordList(response);
            }
        });
    };
    DictionaryFavoriteHeadwords.prototype.showHeadwordList = function (list) {
        $(this.listContainer).empty();
        var listDiv = document.createElement("div");
        for (var i = 0; i < list.length; i++) {
            var favoriteHeadword = list[i];
            var wordSpan = document.createElement("span");
            var removeWordSpan = document.createElement("span");
            var textWordSpan = document.createElement("span");
            $(wordSpan).addClass("saved-word");
            $(removeWordSpan).addClass("saved-word-remove").addClass("glyphicon").addClass("glyphicon-remove-circle");
            $(removeWordSpan).data("xmlId", favoriteHeadword.XmlEntryId);
            $(removeWordSpan).click(function (event) {
                var element = event.target;
                $(element).parent(".saved-word").fadeOut(function () {
                    $(element).remove();
                }); //TODO populate request to remove on server
            });
            $(textWordSpan).addClass("saved-word-text");
            $(textWordSpan).text(favoriteHeadword.Headword);
            $(textWordSpan).data("xmlId", favoriteHeadword.XmlEntryId);
            $(textWordSpan).click(function (event) {
                alert("here should be request for new search with word: " + $(event.target).text());
            });
            wordSpan.appendChild(removeWordSpan);
            wordSpan.appendChild(textWordSpan);
            listDiv.appendChild(wordSpan);
        }
        $(this.listContainer).append(listDiv);
    };
    return DictionaryFavoriteHeadwords;
})();
//# sourceMappingURL=itjakub.dictionariesFavoriteHeadwords.js.map