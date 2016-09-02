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

    getFavoritesForCategoriesLocal(categoryIds: number[], callback: (favoriteBooks: IFavoriteBook[]) => void) {
        throw new Error("Not implemented"); // TODO
    }
}

enum FavoriteType {
    Book,
    Category,
    PageBookmark,
    Query,
    BookVersion,
}