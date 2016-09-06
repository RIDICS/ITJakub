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

interface IFavoriteBook {
    Id: number;
    FavoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteCategory {
    Id: number;
    FavoriteInfo: Array<IFavoriteBaseInfo>;
}