interface IFavoriteItem {
    Id: number;
    Title: string;
    FavoriteLabel: IFavoriteLabel;
}

interface IFavoriteLabel {
    Id: number;
    Name: string;
    Color: string;
}