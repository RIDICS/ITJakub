function initFeedbackView(bookId: string, versionId: string, entryXmlId: string, headword: string, dictionary: string) {
    var isHeadwordFilled = bookId && versionId && entryXmlId;
    if (!isHeadwordFilled)
        return;

    if (headword)
        $("input[name='headword']").val(headword);

    if (dictionary)
        $("input[name='dictionary']").val(dictionary);
}
