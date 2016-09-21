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
            Name: "Všechny položky",
            Color: "#CC9900",
            IsDefault: true,
            LastUseTime: null
        };
    }

    private getFromStorage(key: string): any {
        var items = this.storage.get(key);
        if (!items) {
            items = [];
        }
        return items;
    }

    private generateLocalId(): number {
        return new Date().getTime();
    }

    private getCurrentTime(): string {
        return new Date().getTime().toString();
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
            throw new Error("Not supported for anonymous users");
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

    public getFavoritesForBooks(bookIds: Array<number>, callback: (favoriteBooks: Array<IFavoriteLabeledBook>) => void): void {
        if (!this.isUserLoggedIn) {
            var favoriteLabeledBooks: IFavoriteLabeledBook[] = this.getFromStorage("favoriteLabeledBooks");
            var bookIdSet = new Set();
            var resultArray = new Array<IFavoriteLabeledBook>();

            bookIdSet.addAll(bookIds);
            for (var i = 0; i < favoriteLabeledBooks.length; i++) {
                var favoriteLabeledBook = favoriteLabeledBooks[i];
                if (bookIdSet.contains(favoriteLabeledBook.Id)) {
                    resultArray.push(favoriteLabeledBook);
                }
            }

            callback(resultArray);
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
            success: (bookList: Array<IFavoriteLabeledBook>) => {
                callback(bookList);
            }
        });
    }

    public getFavoritesForCategories(categoryIds: Array<number>, callback: (favoriteCategories: Array<IFavoriteLabeledCategory>) => void) {
        if (!this.isUserLoggedIn) {
            var favoriteLabeledCategories: IFavoriteLabeledCategory[] = this.getFromStorage("favoriteLabeledCategories");
            var categoryIdSet = new Set();
            var resultArray = new Array<IFavoriteLabeledCategory>();

            categoryIdSet.addAll(categoryIds);
            for (var i = 0; i < favoriteLabeledCategories.length; i++) {
                var favoriteLabeledCategory = favoriteLabeledCategories[i];
                if (categoryIdSet.contains(favoriteLabeledCategory.Id)) {
                    resultArray.push(favoriteLabeledCategory);
                }
            }

            callback(resultArray);
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
            success: (bookList: Array<IFavoriteLabeledCategory>) => {
                callback(bookList);
            }
        });
    }
    
    public getFavoriteLabelsForBooksAndCategories(bookType: BookTypeEnum, callback: (favoriteLabels: IFavoriteLabelsWithBooksAndCategories[]) => void) {
        if (!this.isUserLoggedIn) {
            
            var favoriteLabeledBooks: IFavoriteLabeledBook[] = this.getFromStorage("favoriteLabeledBooks");
            var favoriteLabeledCategories: IFavoriteLabeledCategory[] = this.getFromStorage("favoriteLabeledCategories");
            var resultList = new Array<IFavoriteLabelsWithBooksAndCategories>();
            var defaultLabel = this.getDefaultFavoriteLabel();

            var resultLabel: IFavoriteLabelsWithBooksAndCategories = {
                Id: defaultLabel.Id,
                Name: defaultLabel.Name,
                Color: defaultLabel.Color,
                BookIdList: [],
                CategoryIdList: []
            }

            for (let i = 0; i < favoriteLabeledBooks.length; i++) {
                var favoriteLabeledBook = favoriteLabeledBooks[i];
                resultLabel.BookIdList.push(favoriteLabeledBook.Id);
            }
            for (let i = 0; i < favoriteLabeledCategories.length; i++) {
                var favoriteLabeledCategory = favoriteLabeledCategories[i];
                resultLabel.CategoryIdList.push(favoriteLabeledCategory.Id);
            }

            resultList.push(resultLabel);
            callback(resultList);
            return;
        }

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
        if (!this.isUserLoggedIn) {
            var favoriteQueries = this.getFromStorage("favoriteQueries");
            callback(favoriteQueries);
            return;
        }

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
        if (!this.isUserLoggedIn) {
            var favoritePageBookmarks: IPageBookmarkStorageItem[] = this.getFromStorage("favoritePageBookmarkItems");
            var resultList = new Array<IBookPageBookmark>();

            for (var i = 0; i < favoritePageBookmarks.length; i++) {
                var item = favoritePageBookmarks[i];
                if (item.bookXmlId === bookXmlId) {
                    for (var j = 0; j < item.bookmarks.length; j++) {
                        var bookmark = item.bookmarks[j];
                        resultList.push(bookmark);
                    }
                }
            }

            callback(resultList);
            return;
        }

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
        if (!this.isUserLoggedIn) {
            throw Error("Not supported for anonymous user");
        }

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
        if (!this.isUserLoggedIn) {
            throw Error("Not implemented yet"); //TODO!
        }

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
        if (!this.isUserLoggedIn) {
            throw Error("Not supported for anonymous user");
        }

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
        if (!this.isUserLoggedIn) {
            throw Error("Not implemented yet"); //TODO!
        }

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
        if (!this.isUserLoggedIn) {
            throw Error("Not implemented yet"); //TODO!
        }

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
        if (!this.isUserLoggedIn) {
            var favoriteLabeledBooks: IFavoriteLabeledBook[] = this.getFromStorage("favoriteLabeledBooks");
            var newItem: IFavoriteLabeledBook = {
                Id: bookId,
                FavoriteInfo: []
            };
            newItem.FavoriteInfo.push({
                Id: this.generateLocalId(),
                CreateTime: this.getCurrentTime(),
                FavoriteLabel: this.getDefaultFavoriteLabel(),
                FavoriteType: FavoriteType.Book,
                Title: favoriteTitle
            });

            favoriteLabeledBooks.push(newItem);
            this.storage.save("favoriteLabeledBooks", favoriteLabeledBooks);

            callback(newItem.Id);
            return;
        }

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
        if (!this.isUserLoggedIn) {
            var favoriteLabeledCategories: IFavoriteLabeledCategory[] = this.getFromStorage("favoriteLabeledCategories");
            var newItem: IFavoriteLabeledCategory = {
                Id: categoryId,
                FavoriteInfo: []
            };
            newItem.FavoriteInfo.push({
                Id: this.generateLocalId(),
                CreateTime: this.getCurrentTime(),
                FavoriteLabel: this.getDefaultFavoriteLabel(),
                FavoriteType: FavoriteType.Category,
                Title: favoriteTitle
            });

            favoriteLabeledCategories.push(newItem);
            this.storage.save("favoriteLabeledCategories", favoriteLabeledCategories);

            callback(newItem.Id);
            return;
        }

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
        if (!this.isUserLoggedIn) {
            var favoriteQueries = <IFavoriteQuery[]>this.getFromStorage("favoriteQueries");
            var favoriteQuery: IFavoriteQuery = {
                Id: this.generateLocalId(),
                CreateTime: this.getCurrentTime(),
                Title: favoriteTitle,
                Query: query,
                FavoriteLabel: this.getDefaultFavoriteLabel()
            }

            favoriteQueries.push(favoriteQuery);
            this.storage.save("favoriteQueries", favoriteQueries);

            callback(favoriteQuery.Id);
            return;
        }

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
            var favoritePageBookmarkItems: IPageBookmarkStorageItem[] = this.getFromStorage("favoritePageBookmarkItems");
            var item: IPageBookmarkStorageItem = null;
            for (var i = 0; i < favoritePageBookmarkItems.length; i++) {
                var storageItem = favoritePageBookmarkItems[i];
                if (storageItem.bookXmlId === bookXmlId) {
                    item = storageItem;
                    break;
                }
            }
            if (item == null) {
                item = {
                    bookXmlId: bookXmlId,
                    bookmarks: []
                };
                favoritePageBookmarkItems.push(item);
            }

            var bookmark: IBookPageBookmark = {
                Id: this.generateLocalId(),
                Title: favoriteTitle,
                PageXmlId: pageXmlId,
                PagePosition: null,
                FavoriteLabel: this.getDefaultFavoriteLabel()
            };
            item.bookmarks.push(bookmark);

            this.storage.save("favoritePageBookmarkItems", favoritePageBookmarkItems);
            callback(bookmark.Id);
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

interface IPageBookmarkStorageItem {
    bookXmlId: string;
    bookmarks: IBookPageBookmark[];
}