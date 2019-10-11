class WebHubApiClient {
    protected readonly formContentType = "application/x-www-form-urlencoded";
    protected readonly jsonContentType = "application/json";

    protected readonly jsonDataType = "json";
    protected readonly htmlDataType = "html";

    post(url: string, data: string, contentType = this.jsonContentType, dataType = this.jsonDataType): JQuery.jqXHR {
        return $.ajax({
            type: "POST",
            traditional: true,
            url: url,
            data: data,
            dataType: dataType,
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

    public getHtmlPageByUrl(url: string): JQuery.jqXHR<string> {
        return this.get(url);
    }
}