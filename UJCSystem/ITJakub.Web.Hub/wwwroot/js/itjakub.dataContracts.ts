interface IFavoriteBaseInfo {
    id: number;
    title: string;
    favoriteLabel: IFavoriteLabel;
    createTime: string;
    favoriteType: FavoriteType;
}

interface IFavoriteLabel {
    id: number;
    name: string;
    color: string;
    isDefault: boolean;
    lastUseTime: string;
}

interface IFavoriteLabelsWithBooksAndCategories {
    id: number;
    name: string;
    color: string;
    bookIdList: number[];
    categoryIdList: number[];
}

interface IFavoriteLabeledBook {
    id: number;
    favoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteLabeledCategory {
    id: number;
    favoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteQuery {
    id: number;
    title: string;
    createTime: string;
    query: string;
    favoriteLabel: IFavoriteLabel;
    bookType?: BookTypeEnum;
    queryType?: QueryTypeEnum;
}

interface IBookPageBookmark {
    id: number;
    pageXmlId: string;
    pagePosition: number;
    title: string;
    favoriteLabel: IFavoriteLabel;
}

enum FavoriteType {
    Unknown = 0,
    Book = 1,
    Category = 2,
    PageBookmark = 3,
    Query = 4,
    BookVersion = 5,
    HeadwordBookmark = 6,
}

enum QueryTypeEnum
{
    Search = 0,
    List = 1,
    Reader = 2,
}