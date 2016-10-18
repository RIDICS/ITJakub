interface IFavoriteBaseInfo {
    Id: number;
    Title: string;
    FavoriteLabel: IFavoriteLabel;
    CreateTime: string;
    FavoriteType: FavoriteType;
}

interface IFavoriteLabel {
    Id: number;
    Name: string;
    Color: string;
    IsDefault: boolean;
    LastUseTime: string;
}

interface IFavoriteLabelsWithBooksAndCategories {
    Id: number;
    Name: string;
    Color: string;
    BookIdList: number[];
    CategoryIdList: number[];
}

interface IFavoriteLabeledBook {
    Id: number;
    FavoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteLabeledCategory {
    Id: number;
    FavoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteQuery {
    Id: number;
    Title: string;
    CreateTime: string;
    Query: string;
    FavoriteLabel: IFavoriteLabel;
    BookType?: BookTypeEnum;
    QueryType?: QueryTypeEnum;
}

interface IBookPageBookmark {
    Id: number;
    PageXmlId: string;
    PagePosition: number;
    Title: string;
    FavoriteLabel: IFavoriteLabel;
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