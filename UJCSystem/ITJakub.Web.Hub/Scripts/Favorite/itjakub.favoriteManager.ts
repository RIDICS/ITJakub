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
            Color: "#CC9900",
            IsDefault: true,
            LastUseTime: null
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

    public getFavoriteLabels(callback: (favoriteLabels: IFavoriteLabel[]) => void) {
        if (!this.isUserLoggedIn) {
            var list = [this.getDefaultFavoriteLabel()];
            callback(list);
            return;
        }

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetLabelList",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: (favoriteLabels: Array<IFavoriteLabel>) => {
                callback(favoriteLabels);
            }
        });
    }

    public getFavorites(labelId: number, filterByType: number, filterByTitle: string, sort: number, callback: (favorites: IFavoriteBaseInfo[]) => void) {
        if (!this.isUserLoggedIn) {
            throw new Error("Not implemented"); // TODO
        }
        
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteList",
            data: {
                labelId: labelId,
                filterByType: filterByType,
                filterByTitle: filterByTitle,
                sort: sort
            },
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

    public getFavoriteLabelsForBooksAndCategories(bookType: BookTypeEnum, callback: (favoriteLabels: IFavoriteLabelsWithBooksAndCategories[]) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteLabelsWithBooksAndCategories",
            data: {
                bookType: bookType
            },
            dataType: "json",
            contentType: "application/json",
            success: (favoriteLabels) => {
                callback(favoriteLabels);
            }
        });
    }

    public getFavoriteQueries(bookType: BookTypeEnum, queryType: QueryTypeEnum, callback: (favoriteQueries: IFavoriteQuery[]) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteQueries",
            data: {
                bookType: bookType,
                queryType: queryType
            },
            dataType: "json",
            contentType: "application/json",
            success: (queries) => {
                callback(queries);
            }
        });
    }

    public getPageBookmarks(bookXmlId: string, callback: (bookmarks: IBookPageBookmark[]) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetPageBookmarks",
            data: JSON.stringify({
                bookXmlId: bookXmlId
            }),
            dataType: "json",
            contentType: "application/json",
            success: (bookmarks) => {
                callback(bookmarks);
            }
        });
    }

    public createFavoriteLabel(labelName: string, colorHex: string, callback: (id: number) => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreateLabel",
            data: JSON.stringify({
                name: labelName,
                color: colorHex
            }),
            dataType: "json",
            contentType: "application/json",
            success: (id) => {
                callback(id);
            }
        });
    }

    public updateFavoriteLabel(labelId: number, labelName: string, colorHex: string, callback: () => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/UpdateLabel",
            data: JSON.stringify({
                labelId: labelId,
                name: labelName,
                color: colorHex
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                callback();
            }
        });
    }

    public deleteFavoriteLabel(labelId: number, callback: () => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/DeleteLabel",
            data: JSON.stringify({
                labelId: labelId
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                callback();
            }
        });
    }

    public updateFavoriteItem(favoriteId: number, title: string, callback: () => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/UpdateFavoriteItem",
            data: JSON.stringify({
                id: favoriteId,
                title: title
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                callback();
            }
        });
    }

    public deleteFavoriteItem(favoriteId: number, callback: () => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/DeleteFavoriteItem",
            data: JSON.stringify({
                id: favoriteId
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                callback();
            }
        });
    }

    public createFavoriteItem(itemType: FavoriteType, itemId: string, favoriteTitle: string, favoriteLabelId: number, callback: (id: number) => void) {
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

    private createFavoriteBook(bookId: number, favoriteTitle: string, favoriteLabelId: number, callback: (id: number) => void) {
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
            success: (resultId) => {
                callback(resultId);
            }
        });
    }

    private createFavoriteCategory(categoryId: number, favoriteTitle: string, favoriteLabelId: number, callback: (id: number) => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreateFavoriteCategory",
            data: JSON.stringify({
                categoryId: categoryId,
                title: favoriteTitle,
                labelId: favoriteLabelId
            }),
            dataType: "json",
            contentType: "application/json",
            success: (resultId) => {
                callback(resultId);
            }
        });
    }

    public createFavoriteQuery(bookType: BookTypeEnum, queryType: QueryTypeEnum, query: string, favoriteTitle: string, favoriteLabelId: number, callback: (id: number) => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreateFavoriteQuery",
            data: JSON.stringify({
                bookType: bookType,
                queryType: queryType,
                query: query,
                title: favoriteTitle,
                labelId: favoriteLabelId
            }),
            dataType: "json",
            contentType: "application/json",
            success: (resultId) => {
                callback(resultId);
            }
        });
    }

    public createPageBookmark(bookXmlId: string, pageXmlId: string, favoriteTitle: string, favoriteLabelId: number, callback: (id: number) => void) {
        if (!this.isUserLoggedIn) {
            // TODO save to local storage:
            // this.storage.update(`reader-bookmarks-${this.bookId}`, page.xmlId, { BookId: this.bookId, PageXmlId: page.xmlId });
            // callback();
            return;
        }

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreatePageBookmark",
            data: JSON.stringify({
                bookXmlId: bookXmlId,
                pageXmlId: pageXmlId,
                title: favoriteTitle,
                labelId: favoriteLabelId
            }),
            dataType: "json",
            contentType: "application/json",
            success: (resultId) => {
                callback(resultId);
            }
        });
    }
}

class FavoriteHelper {
    static getColorBrightness(red: number, green: number, blue: number): number {
        return ((red * 299) + (green * 587) + (blue * 114)) / 1000;
    }

    static getFontColor(hexBackgroundColor: string): string {
        if (hexBackgroundColor.length !== 7 && hexBackgroundColor.charAt(0) !== "#") {
            throw Error("Invalid color format");
        }

        var red = parseInt(hexBackgroundColor.substr(1, 2), 16);
        var green = parseInt(hexBackgroundColor.substr(3, 2), 16);
        var blue = parseInt(hexBackgroundColor.substr(5, 2), 16);
        var brightness = FavoriteHelper.getColorBrightness(red, green, blue);

        return brightness > 128 ? "#000000" : "#FFFFFF";
    }
}
