abstract class ServerCommunication {
    static getEditionNote(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId, format: "Html" },
            url: getBaseUrl() + "Reader/GetEditionNote",
            dataType: "json",
            contentType: "application/json"            
        });
    }

    static getBookDetail(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId },
            url: getBaseUrl() + "Reader/GetProjectDetail",
            dataType: "json",
            contentType: "application/json"
        });
    }

    static getBookContent(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: bookId },
            url: getBaseUrl() + "Reader/GetBookContent",
            dataType: "json",
            contentType: "application/json"
        });
    }

    static getTrack(bookId: string, trackId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId, trackId: trackId },
            url: getBaseUrl() + "Reader/GetAudioBookTrack",
            dataType: "json",
            contentType: "application/json"
        });
    }
    static getTerms(bookId: string, pageId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: bookId, pageId: pageId },
            url: getBaseUrl() + "Reader/GetTermsOnPage",
            dataType: "json",
            contentType: "application/json"
        });
    }

    static getBookPage(versionId: string, pageId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: versionId, pageId: pageId },
            url: getBaseUrl() + "Reader/GetBookPage",
            dataType: "json",
            contentType: "application/json"
        });
    }

    static getBookPageSearch(versionId: string, pageId: number, queryIsJson: boolean, query: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { query: query, isQueryJson: queryIsJson, snapshotId: versionId, pageId: pageId },
            url: getBaseUrl() + "Reader/GetBookSearchPageByXmlId",
            dataType: "json",
            contentType: "application/json"
        });
    }

    static getAudioBook(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId },
            url: getBaseUrl() + "Reader/GetAudioBook",
            dataType: "json",
            contentType: "application/json"
        });
    }

    static hasBookPage(bookId: string, bookVersionId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: bookId, snapshotId: bookVersionId },
            url: document.getElementsByTagName("body")[0].getAttribute("data-has-book-text-url"),
            dataType: "json",
            contentType: "application/json"
        });
    }

    static textSearchBookCount(bookId: string, versionId: string, text: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookCount",
            data: { text: text, projectId: bookId, snapshotId: versionId },
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    static advancedSearchBookCount(bookId: string, versionId: string, json: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookCount",
            data: { json: json, projectId: bookId, snapshotId: versionId },
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    static textSearchMatchHit(bookId: string, versionId: string, text: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookPagesWithMatchHit",
            data: { text: text, projectId: bookId, snapshotId: versionId },
            dataType: 'json',
            contentType: 'application/json'
        });

    }

    static advancedSearchMatchHit(bookId: string, versionId: string, json: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookPagesWithMatchHit",
            data: { json: json, projectId: bookId, snapshotId: versionId },
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    static textSearchBookPaged(bookId: string, versionId: string, text: string, start: number, count: number):
        JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookPaged",
            data: { text: text, start: start, count: count, projectId: bookId, snapshotId: versionId },
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    static advancedSearchBookPaged(bookId: string, versionId: string, json: string, start: number, count: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookPaged",
            data: { json: json, start: start, count: count, projectId: bookId, snapshotId: versionId },
            dataType: 'json',
            contentType: 'application/json'
        });
    }
}