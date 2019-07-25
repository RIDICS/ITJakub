class WebHubApiClient {
    protected readonly formContentType = "application/x-www-form-urlencoded";
    protected readonly jsonContentType = "application/json";

    post(url: string, data: string, contentType = this.jsonContentType): JQuery.jqXHR {
        return $.ajax({
            type: "POST",
            traditional: true,
            url: url,
            data: data,
            dataType: "json",
            contentType: contentType
        });
    }

    get(url: string): JQuery.jqXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: url
        });
    }
}