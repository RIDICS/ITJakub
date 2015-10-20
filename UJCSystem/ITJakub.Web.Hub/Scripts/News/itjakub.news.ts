var paginator: Pagination;
var newsOnPage = 5;
var newsContainer;


function initNews() {
    paginator = new Pagination("#news-paginator", newsOnPage);
    newsContainer = document.getElementById("news-container");
    getNewsCount();
}


function getNewsCount() {

    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "News/GetSyndicationItemCount",        
        dataType: "json",
        contentType: "application/json",
        success: response => {
            var count = response;            
            paginator.createPagination(count, newsOnPage, paginatorClickedCallback);
        }
    });
}

function paginatorClickedCallback(pageNumber: number) {
    var start = (pageNumber - 1) * newsOnPage;
    loadNews(start, newsOnPage);
}


function loadNews(start: number, count: number) {
    $(newsContainer).empty();
    showLoading();
    
    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "News/GetSyndicationItems",
        data: { start: start, count: count },
        dataType: 'json',
        contentType: 'application/json',
        success: response => {
            
            this.showNews(response);
        }
    });
};

function showNews(items: Array<INewsSyndicationItemContract>) {
    //var newsContainer = document.getElementById("news-container");
    clearLoading();
    
    for (var i = 0; i < items.length; i++) {
        var item = items[i];

        var itemDiv = document.createElement("div");
        $(itemDiv).addClass("message");

        var date = convertDate(item.CreateDate);
        var titleHeader = document.createElement("h2");
        titleHeader.innerHTML = item.Title;

        itemDiv.appendChild(titleHeader);

        var dateDiv = document.createElement("div");
        $(dateDiv).addClass("news-date");
        dateDiv.innerHTML = date.toLocaleDateString();

        itemDiv.appendChild(dateDiv);

        var itemMessage = document.createElement("p");
        itemMessage.innerHTML = item.Text;

        itemDiv.appendChild(itemMessage);

        $(newsContainer).append(itemDiv);
    }
}

function clearLoading() {
    $(this.newsContainer).removeClass("loader");
}

function showLoading() {
    $(this.newsContainer).addClass("loader");
}


interface INewsSyndicationItemContract {

    Title: string;
    Text: string;
    Url: string;
    UserEmail: string;
    CreateDate: string;
    UserFirstName: string;
    UserLastName: string;
}