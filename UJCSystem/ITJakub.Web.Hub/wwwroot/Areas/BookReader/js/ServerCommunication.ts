﻿class ServerCommunication {
    getEditionNote(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId, format: "Html" } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetEditionNote",
            dataType: "json",
            contentType: "application/json"            
        });
    }

    getBookDetail(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetProjectDetail",
            dataType: "json",
            contentType: "application/json"
        });
    }

    getBookContent(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: bookId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetBookContent",
            dataType: "json",
            contentType: "application/json"
        });
    }

    getTrack(bookId: string, trackId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId, trackId: trackId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetAudioBookTrack",
            dataType: "json",
            contentType: "application/json"
        });
    }
    getTerms(bookId: string, pageId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: bookId, pageId: pageId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetTermsOnPage",
            dataType: "json",
            contentType: "application/json"
        });
    }

    getBookPage(versionId: string, pageId: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { snapshotId: versionId, pageId: pageId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetBookPage",
            dataType: "json",
            contentType: "application/json"
        });
    }

    getBookPageSearch(versionId: string, pageId: number, queryIsJson: boolean, query: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { query: query, isQueryJson: queryIsJson, snapshotId: versionId, pageId: pageId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetBookSearchPageByXmlId",
            dataType: "json",
            contentType: "application/json"
        });
    }

    getAudioBook(bookId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { projectId: bookId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/GetAudioBook",
            dataType: "json",
            contentType: "application/json"
        });
    }

    hasBookPage(bookId: string, bookVersionId: string, ): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: bookId, snapshotId: bookVersionId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/HasBookText",
            dataType: "json",
            contentType: "application/json"
        });
    }

    textSearchBookCount(bookId: string, versionId: string, text: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BookReader/BookReader/TextSearchInBookCount",
            data: { text: text, projectId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    textSearchOldGrammar(bookId: string, versionId: string, text: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchInBook",
            data: { text: text, projectId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            dataType: 'json',
            contentType: 'application/json'
        });
    }
    
    advancedSearchBookCount(bookId: string, versionId: string, json: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BookReader/BookReader/AdvancedSearchInBookCount",
            data: { json: json, projectId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    advancedSearchOldGrammar(bookId: string, versionId: string, json: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "OldGrammar/OldGrammar/AdvancedSearchInBook",
            data: { json: json, projectId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    textSearchMatchHit(bookId: string, versionId: string, text: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BookReader/BookReader/TextSearchInBookPagesWithMatchHit",
            data: { text: text, projectId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            dataType: 'json',
            contentType: 'application/json'
        });

    }

    advancedSearchMatchHit(bookId: string, versionId: string, json: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BookReader/BookReader/AdvancedSearchInBookPagesWithMatchHit",
            data: { json: json, projectId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    textSearchBookPaged(bookId: string, versionId: string, text: string, start: number, count: number):
        JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BookReader/BookReader/TextSearchInBookPaged",
            data: { text: text, start: start, count: count, projectId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    advancedSearchBookPaged(bookId: string, versionId: string, json: string, start: number, count: number): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BookReader/BookReader/AdvancedSearchInBookPaged",
            data: { json: json, start: start, count: count, projectId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            dataType: 'json',
            contentType: 'application/json'
        });
    }

    hasBookImage(bookId: string, versionId: string): JQueryXHR {
        return $.ajax({
            type: "GET",
            traditional: true,
            data: { bookId: bookId, snapshotId: versionId } as JQuery.PlainObject,
            url: getBaseUrl() + "Reader/HasBookImage",
            dataType: "json",
            contentType: "application/json"
        });
    }

    getTrackDownloadUrl(recordingId: number, audioType: string): string {
        var trackDownloadUrl: string = getBaseUrl();
        trackDownloadUrl += "AudioBooks/AudioBooks/DownloadAudio?audioId=" +
            recordingId +
            "&audioType=" +
            audioType;
        return trackDownloadUrl;
    }
}