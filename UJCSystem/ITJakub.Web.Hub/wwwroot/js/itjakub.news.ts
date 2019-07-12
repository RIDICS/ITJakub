class News {
    private paginator: Pagination;
    private newsOnPage = 5;
    private newsContainer: HTMLElement;

    public initNews() {
        this.paginator = new Pagination({
            container: document.getElementById("news-paginator") as HTMLDivElement,
            pageClickCallback: this.paginatorClickedCallback.bind(this),
            callPageClickCallbackOnInit: false
        });

        this.newsContainer = document.getElementById("news-container");

        this.sendGetNewsRequest(0, response => {
            this.paginator.make(response.totalCount, this.newsOnPage);
            this.showNews(response.list);
        });
    }

    private sendGetNewsRequest(start: number, callback: (response: IPagedResultArray<INewsSyndicationItemContract>) => void) {
        this.showLoading();
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
            success: callback
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
        this.clearLoading();

        for (var i = 0; i < items.length; i++) {
            var item = items[i];

            var itemDiv = document.createElement("div");
            $(itemDiv).addClass("message");

            var date = new Date(item.createTime);
            var titleHeader = document.createElement("h2");
            titleHeader.innerHTML = item.title;

            itemDiv.appendChild(titleHeader);

            var dateDiv = document.createElement("div");
            $(dateDiv).addClass("news-date");
            dateDiv.innerHTML = date.toLocaleDateString();

            itemDiv.appendChild(dateDiv);

            var itemMessage = document.createElement("p");
            itemMessage.innerHTML = item.text;

            itemDiv.appendChild(itemMessage);

            $(this.newsContainer).append(itemDiv);
        }
    }

    private clearLoading() {
        $(this.newsContainer).removeClass("loader");
    }

    private showLoading() {
        $(this.newsContainer).addClass("loader");
    }
}
