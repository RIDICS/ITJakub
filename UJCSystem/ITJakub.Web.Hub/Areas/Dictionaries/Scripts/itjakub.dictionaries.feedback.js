function initFeedbackView(bookId, versionId, entryXmlId, headword, dictionary) {
    var isHeadwordFilled = bookId && versionId && entryXmlId;
    if (!isHeadwordFilled)
        return;
    if (headword)
        $("input[name='headword']").val(headword);
    if (dictionary)
        $("input[name='dictionary']").val(dictionary);
}
//# sourceMappingURL=itjakub.dictionaries.feedback.js.map