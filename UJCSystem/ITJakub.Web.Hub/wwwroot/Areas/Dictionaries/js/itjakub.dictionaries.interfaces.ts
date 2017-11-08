interface IHeadwordBookInfo {
    headwordId: number;
    headwordVersionId: number;
    bookId: number;
    pageId?: number;
}

interface IHeadword {
    headword: string;
    dictionaries: Array<IHeadwordBookInfo>;
}

interface IHeadwordList {
    bookList: IBookListDictionary;
    headwordList: Array<IHeadword>;
}

interface IDictionaryFavoriteHeadword {
    headword: string;
    entryXmlId: string;
    bookId: string;

    fakeHeadwordId?: number; // TODO update this DataContract according to new API
    fakeBookId?: number;
}
