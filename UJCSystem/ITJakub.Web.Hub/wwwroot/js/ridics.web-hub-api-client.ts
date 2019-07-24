class WebHubApiClient {
    formContentType = "application/x-www-form-urlencoded";
    jsonContentType = "application/json";

    post(url: string, data: string, contentType = this.jsonContentType): JQuery.Promise<any> {
        return $.ajax({
            type: "POST",
            traditional: true,
            url: url,
            data: data,
            dataType: "json",
            contentType: contentType
        });
    }

    get(url: string): JQuery.Promise<any> {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: url
        });
    }
}