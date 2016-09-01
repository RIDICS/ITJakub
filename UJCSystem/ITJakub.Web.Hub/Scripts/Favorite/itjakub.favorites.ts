class Favorites {
    private storage: IStorage;
    private isUserLoggedIn: boolean;

    constructor(storage: IStorage) {
        this.storage = storage;
        this.isUserLoggedIn = isUserLoggedIn();
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