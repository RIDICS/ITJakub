interface IFavoriteBaseInfo {
    Id: number;
    Title: string;
    FavoriteLabel: IFavoriteLabel;
    LastUseTime: string;
}

interface IFavoriteLabel {
    Id: number;
    Name: string;
    Color: string;
}

interface IFavoriteBook {
    Id: number;
    FavoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteCategory {
    Id: number;
    FavoriteInfo: Array<IFavoriteBaseInfo>;
}