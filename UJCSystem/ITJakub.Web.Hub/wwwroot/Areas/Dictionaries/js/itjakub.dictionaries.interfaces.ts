interface IHeadwordBookInfo {
    bookXmlId: string;
    entryXmlId: string;
    image: string;
}

interface IHeadword {
    headword: string;
    dictionaries: Array<IHeadwordBookInfo>;
}

interface IHeadwordList {
    bookList: IBookListDictionary;
    headwordList: Array<IHeadword>;
}

interface IDictionaryContract {
    bookXmlId: string;
    bookVersionXmlId: string;
    bookAcronym: string;
    bookTitle: string;
}

interface IDictionaryFavoriteHeadword {
    headword: string;
    entryXmlId: string;
    bookId: string;
}