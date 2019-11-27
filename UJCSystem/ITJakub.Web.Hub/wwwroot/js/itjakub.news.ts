class News {
    private paginator: Pagination;
    private newsOnPage = 5;
    private newsContainer: HTMLElement;
    private loaderElement = lv.create(null, "lv-dots md lv-mid lvt-3 lvb-3");
    private errorHandler: ErrorHandler;

    public initNews() {
        this.paginator = new Pagination({
            container: document.getElementById("news-paginator") as HTMLDivElement,
            pageClickCallback: this.paginatorClickedCallback.bind(this),
            callPageClickCallbackOnInit: false
        });

        this.errorHandler = new ErrorHandler();

        this.newsContainer = document.getElementById("news-container");

        this.sendGetNewsRequest(0, response => {
            this.paginator.make(response.totalCount, this.newsOnPage);
            this.showNews(response.list);
        });
    }

    private sendGetNewsRequest(start: number, callback: (response: IPagedResultArray<INewsSyndicationItemContract>) => void) {
        this.showLoading();
        var self = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "News/GetSyndicationItems",
            data: {
                start: start,
                count: this.newsOnPage
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: callback,
            error: function(error) {
                $("#news-container").empty();
                var emptyElement = document.createElement('div');
                $(emptyElement).addClass("bib-listing-empty");
                $(emptyElement).text(self.errorHandler.getErrorMessage(error));
                $("#news-container").append(emptyElement);
            }
    });
    }

    private paginatorClickedCallback(pageNumber: number) {
        var start = (pageNumber - 1) * this.newsOnPage;
        this.loadNews(start);
    }

    private loadNews(start: number) {
        $(this.newsContainer).empty();

        this.sendGetNewsRequest(start, response => {
            this.showNews(response.list);
        });
    };

    private showNews(items: Array<INewsSyndicationItemContract>) {
        $(this.newsContainer).empty();
        if (items.length === 0) {
            var noNews = document.createElement('div');
            $(noNews).addClass("home-no-news");
            $(noNews).text(localization.translate("NoResultsToShow", "PluginsJs").value);
            $(this.newsContainer).append(noNews);
        } else {
            for (var i = 0; i < items.length; i++) {
                var item = items[i];

                var itemDiv = document.createElement("div");
                $(itemDiv).addClass("message");
                
                var titleHeader = document.createElement("h2");
                titleHeader.innerHTML = item.title;

                itemDiv.appendChild(titleHeader);

                var dateDiv = document.createElement("div");
                $(dateDiv).addClass("news-date");
                                
                dateDiv.innerHTML = item.createTimeString.split(" ")[0];

                itemDiv.appendChild(dateDiv);

                var itemMessage = document.createElement("p");
                itemMessage.innerHTML = item.text;

                itemDiv.appendChild(itemMessage);

                $(this.newsContainer).append(itemDiv);
            }
        }
    }

    private showLoading() {
        $(this.newsContainer).append(this.loaderElement.getElement());
    }
}
