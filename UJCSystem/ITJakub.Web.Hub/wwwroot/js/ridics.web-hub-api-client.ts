class WebHubApiClient {
    public post(url: string, data: string, contentType = "application/json"): JQuery.jqXHR {
        return $.ajax({
            type: "POST",
            traditional: true,
            url: url,
            data: data,
            dataType: "json",
            contentType: contentType
        });
    }

    public get(url: string): JQuery.jqXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: url
        });
    }
}