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
    id: number;
    title: string;
    query: string;
    headwordId: number;
}

interface IHeadwordContract {
    id: number;
    versionId: number;
    versionNumber: number;
    projectId: number;

    //externalId: string;
    defaultHeadword: string;
    sorting: string;
    headwordItems: Array<IHeadwordItemContract>;
}

interface IHeadwordItemContract {
    id: number;
    headword: string;
    headwordOriginal: string;
    resourcePageId?: number;
}
