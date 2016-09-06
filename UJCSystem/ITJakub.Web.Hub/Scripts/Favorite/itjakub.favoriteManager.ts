class FavoriteManager {
    private storage: IStorage;
    private isUserLoggedIn: boolean;

    constructor(storage: IStorage) {
        this.storage = storage;
        this.isUserLoggedIn = isUserLoggedIn();
    }

    private getDefaultFavoriteLabel(): IFavoriteLabel {
        return {
            Id: 0,
            Name: "Anonymní",
            Color: "#CC9900"
        };
    }

    public getLatestFavoriteLabels(callback: (favoriteLabels: IFavoriteLabel[]) => void) {
        if (!this.isUserLoggedIn) {
            var list = [this.getDefaultFavoriteLabel()];
            callback(list);
            return;
        }

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetLatestLabelList",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: (favoriteLabels: Array<IFavoriteLabel>) => {
                callback(favoriteLabels);
            }
        });
    }

    public getFavorites(callback: (favorites: IFavoriteBaseInfo[]) => void) {
        if (!this.isUserLoggedIn) {
            throw new Error("Not implemented"); // TODO
        }

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteList",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: (favorites: Array<IFavoriteBaseInfo>) => {
                callback(favorites);
            }
        });
    }

    public getFavoritesForBooks(bookIds: Array<number>, callback: (favoriteBooks: Array<IFavoriteBook>) => void): void {
        if (!this.isUserLoggedIn) {
            this.getFavoritesForBooksLocal(bookIds, callback);
            return;
        }

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteLabeledBooks",
            data: {
                bookIds: bookIds
            },
            dataType: "json",
            contentType: "application/json",
            success: (bookList: Array<IFavoriteBook>) => {
                callback(bookList);
            }
        });
    }

    public getFavoritesForCategories(categoryIds: Array<number>, callback: (favoriteCategories: Array<IFavoriteCategory>) => void) {
        if (!this.isUserLoggedIn) {
            this.getFavoritesForCategoriesLocal(categoryIds, callback);
            return;
        }

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteLabeledCategories",
            data: {
                categoryIds: categoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: (bookList: Array<IFavoriteCategory>) => {
                callback(bookList);
            }
        });
    }

    private getFavoritesForBooksLocal(bookIds: number[], callback: (favoriteBooks: IFavoriteBook[]) => void) {
        throw new Error("Not implemented"); // TODO
    }

    private getFavoritesForCategoriesLocal(categoryIds: number[], callback: (favoriteBooks: IFavoriteBook[]) => void) {
        throw new Error("Not implemented"); // TODO
    }

    public createFavoriteItem(itemType: FavoriteType, itemId: string, favoriteTitle: string, favoriteLabelId: number, callback: () => void) {
        switch (itemType) {
            case FavoriteType.Book:
                this.createFavoriteBook(Number(itemId), favoriteTitle, favoriteLabelId, callback);
                break;
            case FavoriteType.Category:
                this.createFavoriteCategory(Number(itemId), favoriteTitle, favoriteLabelId, callback);
                break;
            default:
                throw new Error("Not supported favorite type");
        }
    }

    private createFavoriteBook(bookId: number, favoriteTitle: string, favoriteLabelId: number, callback: () => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreateFavoriteBook",
            data: JSON.stringify({
                bookId: bookId,
                title: favoriteTitle,
                labelId: favoriteLabelId
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                callback();
            }
        });
    }

    private createFavoriteCategory(categoryId: number, favoriteTitle: string, favoriteLabelId: number, callback: () => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreateFavoriteBook",
            data: JSON.stringify({
                categoryId: categoryId,
                title: favoriteTitle,
                labelId: favoriteLabelId
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                callback();
            }
        });
    }
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