abstract class ServerCommunication {
    public static getEditionNote(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId, format: "Html" },
            url: getBaseUrl() + "Reader/GetEditionNote",
            dataType: "json",
            contentType: "application/json"            
        });
    }

    public static getBookDetail(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId },
            url: getBaseUrl() + "Reader/GetProjectDetail",
            dataType: "json",
            contentType: "application/json"
        });
    }

    public static getBookContent(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: bookId },
            url: getBaseUrl() + "Reader/GetBookContent",
            dataType: "json",
            contentType: "application/json"
        });
    }

    public static getTrack(bookId: string, trackId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId, trackId: trackId },
            url: getBaseUrl() + "Reader/GetAudioBookTrack",
            dataType: "json",
            contentType: "application/json"
        });
    }
    public static getTerms(bookId: string, pageId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: bookId, pageId: pageId },
            url: getBaseUrl() + "Reader/GetTermsOnPage",
            dataType: "json",
            contentType: "application/json"
        });
    }

    public static getBookPage(versionId: string, pageId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: versionId, pageId: pageId },
            url: getBaseUrl() + "Reader/GetBookPage",
            dataType: "json",
            contentType: "application/json"
        });
    }

    public static getBookPageSearch(versionId: string, pageId: number, queryIsJson: boolean, query: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { query: query, isQueryJson: queryIsJson, snapshotId: versionId, pageId: pageId },
            url: getBaseUrl() + "Reader/GetBookSearchPageByXmlId",
            dataType: "json",
            contentType: "application/json"
        });
    }

    public static getAudioBook(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId },
            url: getBaseUrl() + "Reader/GetAudioBook",
            dataType: "json",
            contentType: "application/json"
        });
    }

    public static hasBookPage(bookId: string, bookVersionId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: bookId, snapshotId: bookVersionId },
            url: document.getElementsByTagName("body")[0].getAttribute("data-has-book-text-url"),
            dataType: "json",
            contentType: "application/json"
        });
    }
}