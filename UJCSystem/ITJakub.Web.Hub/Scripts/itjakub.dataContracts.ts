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

interface IFavoriteBook {
    Id: number;
    FavoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteCategory {
    Id: number;
    FavoriteInfo: Array<IFavoriteBaseInfo>;
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