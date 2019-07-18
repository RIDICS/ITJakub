class WebHubApiClient {
    public post(url: string, data: string, contentType = "application/json"): Promise<JQuery.jqXHR> {
        return $.ajax({
            type: "POST",
            traditional: true,
            url: url,
            data: data,
            dataType: "json",
            contentType: contentType
        });
    }

    public get(url: string): Promise<JQuery.jqXHR> {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: url
        });
    }
}