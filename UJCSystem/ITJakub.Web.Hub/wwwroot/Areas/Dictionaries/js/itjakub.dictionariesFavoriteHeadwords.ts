class DictionaryFavoriteHeadwords {
    private readonly favoriteManager: FavoriteManager;
    private expandButton: string;
    private listContainer: string;
    private mainContainer: string;
    private headwordList: Array<IDictionaryFavoriteHeadword>;
    private headwordClickCallback: (headwordQuery: string) => void;
    private headwordListChangedCallback: (newList: Array<IDictionaryFavoriteHeadword>) => void;

    private inited = false;
    private afterInitCallbacks:Array<()=>any>=[];

    constructor(mainContainer: string, listContainer: string, expandButton: string) {
        this.favoriteManager = new FavoriteManager();
        this.expandButton = expandButton;
        this.listContainer = listContainer;
        this.mainContainer = mainContainer;
    }

    create(headwordClickCallback: (headwordQuery: string) => void, headwordListChangedCallback: (newList: Array<IDictionaryFavoriteHeadword>) => void = null) {
        this.headwordClickCallback = headwordClickCallback;
        this.headwordListChangedCallback = headwordListChangedCallback;
        var areaInitHeight = $(".dictionary-header", $(this.mainContainer)).innerHeight();
        $(this.mainContainer).height(areaInitHeight);

        var self = this;
        $(this.expandButton).on("click",function () {
            var area = $(self.mainContainer);
            if (!area.hasClass("uncollapsed")) {
                $(this).children().removeClass("glyphicon-collapse-down");
                $(this).children().addClass("glyphicon-collapse-up");
                area.addClass("uncollapsed");
                var actualHeight = area.height();
                let targetHeight = area.css("height", 'auto').height();
                area.height(actualHeight);
                area.animate({
                    height: targetHeight
                } as JQuery.PlainObject);

            } else {
                $(this).children().removeClass("glyphicon-collapse-up");
                $(this).children().addClass("glyphicon-collapse-down");
                area.removeClass("uncollapsed");
                let targetHeight = $(".dictionary-header", area).innerHeight();
                area.animate({
                    height: targetHeight
                } as JQuery.PlainObject);
            }
        });

        this.getAllHeadwords();
    }

    public getAllHeadwords() {
        $(this.listContainer).addClass("loading");
        this.favoriteManager.getAllHeadwords((favoriteHeadwords: Array<IDictionaryFavoriteHeadword>) => {
            $(this.listContainer).removeClass("loading");
            this.showHeadwordList(favoriteHeadwords);

            this.inited = true;
            while (this.afterInitCallbacks.length) {
                const callback = this.afterInitCallbacks.pop();
                callback();
            }
        });
    }
    
    public callAfterInit(callback:() => any) {
        if (this.inited) {
            callback();
        }
        else {
            this.afterInitCallbacks.push(callback);
        }
    }


    public addNewHeadword(title: string, headwordId: number, callback: (favoriteHeadwordId: number) => void) {
        this.favoriteManager.addNewHeadword(title, headwordId, (favoriteHeadwordId) => {
            this.getAllHeadwords();
            callback(favoriteHeadwordId);
        });
    }

    private showHeadwordList(list: Array<IDictionaryFavoriteHeadword>) {
        $(this.listContainer).empty();
        this.headwordList = list;
        this.headwordListChanged();
        var listDiv = document.createElement("div");

        for (var i = 0; i < list.length; i++) {
            var favoriteHeadword = list[i];

            var wordSpan = document.createElement("span");
            var removeWordSpan = document.createElement("span");
            var textWordSpan = document.createElement("span");

            $(wordSpan).addClass("saved-word");
            $(wordSpan).data("favorite-id", favoriteHeadword.id);
            $(wordSpan).data("saved-word-query", favoriteHeadword.query);
            $(wordSpan).data("saved-word-id", favoriteHeadword.headwordId);
            
            $(removeWordSpan).addClass("saved-word-remove")
                .addClass("glyphicon")
                .addClass("glyphicon-remove-circle");
            $(removeWordSpan).on("click",event => {
                this.removeHeadword(event.target.parentElement);
            });

            $(textWordSpan).addClass("saved-word-text");
            $(textWordSpan).text(favoriteHeadword.title);
            $(textWordSpan).on("click", event => {
                this.goToPageWithSelectedHeadword(event.target.parentElement);
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
        } as JQuery.PlainObject);
    }

    private goToPageWithSelectedHeadword(element: Element) {
        const headwordQuery = $(element).data("saved-word-query");
        this.headwordClickCallback(headwordQuery);
    }

    private removeHeadword(element: Element) {
        const favoriteId = $(element).data("favorite-id");
        
        this.favoriteManager.deleteFavoriteItem(favoriteId,() => {
            $(element).fadeOut(null, () => {
                $(element).remove();
                this.updateVisibleHeight();
            });

            for (var i = 0; i < this.headwordList.length; i++) {
                var headword = this.headwordList[i];
                if (headword.id === favoriteId) {
                    this.headwordList.splice(i, 1); // remove item from array
                    break;
                }
            }

            this.headwordListChanged();
        });        
    }

    public removeHeadwordById(favoriteId) {
        var elements = $(".saved-word", $(this.listContainer));
        for (var i = 0; i < elements.length; i++) {
            var element = elements.get(i);
            if ($(element as Node as Element).data("favorite-id") === favoriteId) {
                this.removeHeadword(element as Node as Element);
                return;
            }
        }
    }

    private headwordListChanged() {
        if (this.headwordListChangedCallback) {
            var listCopy = this.headwordList.slice();
            this.headwordListChangedCallback(listCopy);
        }
    }
}
