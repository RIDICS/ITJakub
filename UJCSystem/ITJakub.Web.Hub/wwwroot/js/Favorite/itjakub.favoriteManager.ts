class FavoriteManager {
    public static maxTitleLength = 250;
    public static maxLabelLength = 50;
    private static localStorageVersion = 2;
    private storage: IStorage;
    private isUserLoggedIn: boolean;
    private apiClient: FavoriteApiClient;

    //private localization : Localization;
	private localizationScope = "FavoriteJs";

    constructor() {
        this.storage = MyStorageManager.getInstance().getStorage(StorageTypeEnum.Local);
        this.isUserLoggedIn = isUserLoggedIn();

        if (!this.isUserLoggedIn) {
            this.updateLocalStorage();
        }

        this.apiClient = new FavoriteApiClient();
       // this.localization = new Localization();
    }

    private getDefaultFavoriteLabel(): IFavoriteLabel {
        return {
            id: 0,
            name: localization.translate("AllItems", this.localizationScope).value,
            color: "#CC9900",
            isDefault: true,
            lastUseTime: null
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

    private updateLocalStorage() {
        var currentVersion = this.getFromStorage("favoriteStorageVersion");
        if (typeof currentVersion !== "number" || currentVersion < FavoriteManager.localStorageVersion) {
            this.storage.delete("favoritePageBookmarkItems");
            this.storage.delete("favoriteLabeledBooks");
            this.storage.delete("favoriteLabeledCategories");
            this.storage.delete("favoriteQueries");
            this.storage.save("favoriteStorageVersion", FavoriteManager.localStorageVersion);
        }
    }

    private findLocalItemById(id: number): IFavoriteStorageItem {
        var favoritePageBookmarkItems: IPageBookmarkStorageItem[] = this.getFromStorage("favoritePageBookmarkItems");
        for (let i = 0; i < favoritePageBookmarkItems.length; i++) {
            var item = favoritePageBookmarkItems[i];

            for (let j = 0; j < item.bookmarks.length; j++) {
                var bookmark = item.bookmarks[j];
                if (bookmark.id === id) {
                    return {
                        favoriteType: FavoriteType.Page,
                        storageItemIndex: j,
                        storageIndex: i,
                        storageItem: item,
                        storage: favoritePageBookmarkItems
                    };
                }
            }
        }

        var favoriteLabeledBooks: IFavoriteLabeledBook[] = this.getFromStorage("favoriteLabeledBooks");
        for (let i = 0; i < favoriteLabeledBooks.length; i++) {
            var favoriteLabeledBook = favoriteLabeledBooks[i];

            for (let j = 0; j < favoriteLabeledBook.favoriteInfo.length; j++) {
                var favoriteBook = favoriteLabeledBook.favoriteInfo[j];
                if (favoriteBook.id === id) {
                    return {
                        favoriteType: FavoriteType.Project,
                        storageItemIndex: j,
                        storageIndex: i,
                        storageItem: favoriteLabeledBook,
                        storage: favoriteLabeledBooks
                    };
                }
            }
        }

        var favoriteLabeledCategories: IFavoriteLabeledCategory[] = this.getFromStorage("favoriteLabeledCategories");
        for (let i = 0; i < favoriteLabeledCategories.length; i++) {
            var favoriteLabeledCategory = favoriteLabeledCategories[i];

            for (let j = 0; j < favoriteLabeledCategory.favoriteInfo.length; j++) {
                var favoriteCategory = favoriteLabeledCategory.favoriteInfo[j];
                if (favoriteCategory.id === id) {
                    return {
                        favoriteType: FavoriteType.Category,
                        storageItemIndex: j,
                        storageIndex: i,
                        storageItem: favoriteLabeledCategory,
                        storage: favoriteLabeledCategories
                    };
                }
            }
        }

        var favoriteQueries: IFavoriteQuery[] = this.getFromStorage("favoriteQueries");
        for (let i = 0; i < favoriteQueries.length; i++) {
            var favoriteQuery = favoriteQueries[i];
            if (favoriteQuery.id === id) {
                return {
                    favoriteType: FavoriteType.Query,
                    storageItemIndex: null,
                    storageIndex: i,
                    storageItem: favoriteQuery,
                    storage: favoriteQueries
                };
            }
        }

        return null;
    }

    public getLatestFavoriteLabels(callback: (favoriteLabels: IFavoriteLabel[]) => void) {
        if (!this.isUserLoggedIn) {
            var list = [this.getDefaultFavoriteLabel()];
            callback(list);
            return;
        }

        this.apiClient.getLatestFavoriteLabels(callback);
    }

    public getFavoriteLabels(callback: (favoriteLabels: IFavoriteLabel[]) => void) {
        if (!this.isUserLoggedIn) {
            var list = [this.getDefaultFavoriteLabel()];
            callback(list);
            return;
        }

        this.apiClient.getFavoriteLabels(callback);
    }

    public getFavorites(labelId: number, filterByType: number, filterByTitle: string, sort: number, start: number, count: number, callback: (favorites: IFavoriteBaseInfo[]) => void) {
        if (!this.isUserLoggedIn) {
            throw new Error("Not supported for anonymous users");
        }

        this.apiClient.getFavorites(labelId, filterByType, filterByTitle, sort, start, count, callback);
    }

    public getFavoritesCount(labelId: number, filterByType: number, filterByTitle: string, callback: (count: number) => void) {
        if (!this.isUserLoggedIn) {
            throw new Error("Not supported for anonymous users");
        }

        this.apiClient.getFavoritesCount(labelId, filterByType, filterByTitle, callback);
    }

    public getFavoritesForBooks(orBookType: BookTypeEnum, orBookIds: Array<number>, callback: (favoriteBooks: Array<IFavoriteLabeledBook>) => void): void {
        if (!this.isUserLoggedIn) {
            var favoriteLabeledBooks: IFavoriteLabeledBook[] = this.getFromStorage("favoriteLabeledBooks");
            var bookIdSet = new Set();
            var resultArray = new Array<IFavoriteLabeledBook>();

            bookIdSet.addAll(orBookIds);
            for (var i = 0; i < favoriteLabeledBooks.length; i++) {
                var favoriteLabeledBook = favoriteLabeledBooks[i];
                if (bookIdSet.contains(favoriteLabeledBook.id)) {
                    resultArray.push(favoriteLabeledBook);
                }
            }

            callback(resultArray);
            return;
        }

        if (orBookType != null) {
            orBookIds = null; // because max ID count limit
        }

        this.apiClient.getFavoritesForBooks(orBookType, orBookIds, callback);
    }

    public getFavoritesForCategories(categoryIds: Array<number>, callback: (favoriteCategories: Array<IFavoriteLabeledCategory>) => void) {
        if (!this.isUserLoggedIn) {
            var favoriteLabeledCategories: IFavoriteLabeledCategory[] = this.getFromStorage("favoriteLabeledCategories");
            var categoryIdSet = new Set();
            var resultArray = new Array<IFavoriteLabeledCategory>();

            categoryIdSet.addAll(categoryIds);
            for (var i = 0; i < favoriteLabeledCategories.length; i++) {
                var favoriteLabeledCategory = favoriteLabeledCategories[i];
                if (categoryIdSet.contains(favoriteLabeledCategory.id)) {
                    resultArray.push(favoriteLabeledCategory);
                }
            }

            callback(resultArray);
            return;
        }

        this.apiClient.getFavoritesForCategories(categoryIds, callback);
    }
    
    public getFavoriteLabelsForBooksAndCategories(bookType: BookTypeEnum, callback: (favoriteLabels: IFavoriteLabelsWithBooksAndCategories[], error: string) => void) {
        if (!this.isUserLoggedIn) {
            
            var favoriteLabeledBooks: IFavoriteLabeledBook[] = this.getFromStorage("favoriteLabeledBooks");
            var favoriteLabeledCategories: IFavoriteLabeledCategory[] = this.getFromStorage("favoriteLabeledCategories");
            var resultList = new Array<IFavoriteLabelsWithBooksAndCategories>();
            var defaultLabel = this.getDefaultFavoriteLabel();

            var resultLabel: IFavoriteLabelsWithBooksAndCategories = {
                id: defaultLabel.id,
                name: defaultLabel.name,
                color: defaultLabel.color,
                projectIdList: [],
                categoryIdList: []
            }

            for (let i = 0; i < favoriteLabeledBooks.length; i++) {
                var favoriteLabeledBook = favoriteLabeledBooks[i];
                resultLabel.projectIdList.push(favoriteLabeledBook.id);
            }
            for (let i = 0; i < favoriteLabeledCategories.length; i++) {
                var favoriteLabeledCategory = favoriteLabeledCategories[i];
                resultLabel.categoryIdList.push(favoriteLabeledCategory.id);
            }

            resultList.push(resultLabel);
            callback(resultList, null);
            return;
        }

        this.apiClient.getFavoriteLabelsForBooksAndCategories(bookType, callback);
    }

    public getFavoriteQueries(labelId: number, bookType: BookTypeEnum, queryType: QueryTypeEnum, filterByTitle: string, start: number, count: number, callback: (favoriteQueries: IFavoriteQuery[]) => void) {
        if (!this.isUserLoggedIn) {
            var favoriteQueries: IFavoriteQuery[] = this.getFromStorage("favoriteQueries");
            var filteredQueries = new Array<IFavoriteQuery>();
            for (let i = 0; i < favoriteQueries.length; i++) {
                let favoriteQuery = favoriteQueries[i];
                if (favoriteQuery.bookType === bookType && favoriteQuery.queryType === queryType) {
                    filteredQueries.push(favoriteQuery);
                }
            }

            if (!filterByTitle) {
                callback(filteredQueries);
                return;
            }

            var result = new Array<IFavoriteQuery>();
            for (let i = 0; i < filteredQueries.length; i++) {
                let favoriteQuery = filteredQueries[i];
                if (favoriteQuery.title.indexOf(filterByTitle) !== -1) {
                    result.push(favoriteQuery);
                }
            }
            callback(result);
            return;
        }

        this.apiClient.getFavoriteQueries(labelId, bookType, queryType, filterByTitle, start, count, callback);
    }

    public getFavoriteQueriesCount(labelId: number, bookType: BookTypeEnum, queryType: QueryTypeEnum, filterByTitle: string, callback: (count: number) => void) {
        if (!this.isUserLoggedIn) {
            callback(1); // paging not supported when using local storage
            return;
        }

        this.apiClient.getFavoriteQueriesCount(labelId, bookType, queryType, filterByTitle, callback);
    }

    public getPageBookmarks(bookId: number, callback: (bookmarks: IBookPageBookmark[]) => void) {
        if (!this.isUserLoggedIn) {
            var favoritePageBookmarks: IPageBookmarkStorageItem[] = this.getFromStorage("favoritePageBookmarkItems");
            var resultList = new Array<IBookPageBookmark>();

            for (var i = 0; i < favoritePageBookmarks.length; i++) {
                var item = favoritePageBookmarks[i];
                if (item.bookId === bookId) {
                    for (var j = 0; j < item.bookmarks.length; j++) {
                        var bookmark = item.bookmarks[j];
                        resultList.push(bookmark);
                    }
                }
            }

            callback(resultList);
            return;
        }

        this.apiClient.getPageBookmarks(bookId, callback);
    }

    public createFavoriteLabel(labelName: string, colorHex: string, callback: (id: number, error: string) => void) {
        if (!this.isUserLoggedIn) {
            throw Error("Not supported for anonymous user");
        }

        this.apiClient.createFavoriteLabel(labelName, colorHex, callback);
    }

    public updateFavoriteLabel(labelId: number, labelName: string, colorHex: string, callback: (error: string) => void) {
        if (!this.isUserLoggedIn) {
            throw Error("Not supported for anonymous user");
        }

        this.apiClient.updateFavoriteLabel(labelId, labelName, colorHex, callback);
    }

    public deleteFavoriteLabel(labelId: number, callback: (error: string) => void) {
        if (!this.isUserLoggedIn) {
            throw Error("Not supported for anonymous user");
        }

        this.apiClient.deleteFavoriteLabel(labelId, callback);
    }

    public updateFavoriteItem(favoriteId: number, title: string, callback: (error: string) => void) {
        if (!this.isUserLoggedIn) {
            var storageItemInfo = this.findLocalItemById(favoriteId);
            if (storageItemInfo) {
                switch (storageItemInfo.favoriteType) {
                    case FavoriteType.Project:
                        var bookStorage = <IFavoriteLabeledBook[]>storageItemInfo.storage;
                        var favoriteLabeledBook = <IFavoriteLabeledBook>storageItemInfo.storageItem;
                        var favoriteBook = favoriteLabeledBook.favoriteInfo[storageItemInfo.storageItemIndex];

                        favoriteBook.title = title;
                        this.storage.save("favoriteLabeledBooks", bookStorage);
                        break;
                    case FavoriteType.Category:
                        var categoryStorage = <IFavoriteLabeledCategory[]>storageItemInfo.storage;
                        var favoriteLabeledCategory = <IFavoriteLabeledCategory>storageItemInfo.storageItem;
                        var favoriteCategory = favoriteLabeledCategory.favoriteInfo[storageItemInfo.storageItemIndex];

                        favoriteCategory.title = title;
                        this.storage.save("favoriteLabeledCategories", categoryStorage);
                        break;
                    case FavoriteType.Query:
                        var queryStorage = <IFavoriteQuery[]>storageItemInfo.storage;
                        var favoriteQuery = <IFavoriteQuery>storageItemInfo.storageItem;

                        favoriteQuery.title = title;
                        this.storage.save("favoriteQueries", queryStorage);
                        break;
                    case FavoriteType.Page:
                        var bookmarkStorage = <IPageBookmarkStorageItem[]>storageItemInfo.storage;
                        var bookmarkStorageItem = <IPageBookmarkStorageItem>storageItemInfo.storageItem;
                        var bookmark = bookmarkStorageItem.bookmarks[storageItemInfo.storageItemIndex];

                        bookmark.title = title;
                        this.storage.save("favoritePageBookmarkItems", bookmarkStorage);
                        break;
                    default:
                        throw Error("Not supported FavoriteType");
                }
            }

            callback(null);
            return;
        }

        this.apiClient.updateFavoriteItem(favoriteId, title, callback);
    }

    public deleteFavoriteItem(favoriteId: number, callback: (error: string) => void) {
        if (!this.isUserLoggedIn) {
            var storageItemInfo = this.findLocalItemById(favoriteId);
            if (storageItemInfo) {
                switch (storageItemInfo.favoriteType) {
                    case FavoriteType.Project:
                        var bookStorage = <IFavoriteLabeledBook[]>storageItemInfo.storage;
                        var favoriteLabeledBook = <IFavoriteLabeledBook>storageItemInfo.storageItem;

                        favoriteLabeledBook.favoriteInfo.splice(storageItemInfo.storageItemIndex, 1);
                        if (favoriteLabeledBook.favoriteInfo.length === 0) {
                            bookStorage.splice(storageItemInfo.storageIndex, 1);
                        }

                        this.storage.save("favoriteLabeledBooks", bookStorage);
                        break;
                    case FavoriteType.Category:
                        var categoryStorage = <IFavoriteLabeledCategory[]>storageItemInfo.storage;
                        var favoriteLabeledCategory = <IFavoriteLabeledCategory>storageItemInfo.storageItem;

                        favoriteLabeledCategory.favoriteInfo.splice(storageItemInfo.storageItemIndex, 1);
                        if (favoriteLabeledCategory.favoriteInfo.length === 0) {
                            categoryStorage.splice(storageItemInfo.storageIndex, 1);
                        }

                        this.storage.save("favoriteLabeledCategories", categoryStorage);
                        break;
                    case FavoriteType.Query:
                        var queryStorage = <IFavoriteQuery[]>storageItemInfo.storage;

                        queryStorage.splice(storageItemInfo.storageIndex, 1);
                        this.storage.save("favoriteQueries", queryStorage);
                        break;
                    case FavoriteType.Page:
                        var bookmarkStorage = <IPageBookmarkStorageItem[]>storageItemInfo.storage;
                        var bookmarkStorageItem = <IPageBookmarkStorageItem>storageItemInfo.storageItem;
                        
                        bookmarkStorageItem.bookmarks.splice(storageItemInfo.storageItemIndex, 1);
                        if (bookmarkStorageItem.bookmarks.length === 0) {
                            bookmarkStorage.splice(storageItemInfo.storageIndex, 1);
                        }

                        this.storage.save("favoritePageBookmarkItems", bookmarkStorage);
                        break;
                    default:
                        throw Error("Not supported FavoriteType");
                }
            }

            callback(null);
            return;
        }

        this.apiClient.deleteFavoriteItem(favoriteId, callback);
    }

    public createFavoriteItem(itemType: FavoriteType, itemId: string, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        switch (itemType) {
            case FavoriteType.Project:
                this.createFavoriteBook(Number(itemId), favoriteTitle, favoriteLabelIds, callback);
                break;
            case FavoriteType.Category:
                this.createFavoriteCategory(Number(itemId), favoriteTitle, favoriteLabelIds, callback);
                break;
            default:
                throw new Error("Not supported favorite type");
        }
    }

    private createFavoriteBook(bookId: number, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        if (!this.isUserLoggedIn) {
            var favoriteLabeledBooks: IFavoriteLabeledBook[] = this.getFromStorage("favoriteLabeledBooks");
            var favoriteLabeledBook: IFavoriteLabeledBook = null;
            
            for (let i = 0; i < favoriteLabeledBooks.length; i++) {
                var item = favoriteLabeledBooks[i];
                if (item.id === bookId) {
                    favoriteLabeledBook = item;
                    break;
                }
            }
            if (favoriteLabeledBook == null) {
                favoriteLabeledBook = {
                    id: bookId,
                    favoriteInfo: []
                };
                favoriteLabeledBooks.push(favoriteLabeledBook);
            }

            var newFavoriteBook: IFavoriteBaseInfoWithLabel = {
                id: this.generateLocalId(),
                createTime: this.getCurrentTime(),
                favoriteLabel: this.getDefaultFavoriteLabel(),
                favoriteType: FavoriteType.Project,
                title: favoriteTitle
            };
            favoriteLabeledBook.favoriteInfo.push(newFavoriteBook);
            
            this.storage.save("favoriteLabeledBooks", favoriteLabeledBooks);
            callback([newFavoriteBook.id], null);
            return;
        }

        this.apiClient.createFavoriteBook(bookId, favoriteTitle, favoriteLabelIds, callback);
    }

    private createFavoriteCategory(categoryId: number, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        if (!this.isUserLoggedIn) {
            var favoriteLabeledCategories: IFavoriteLabeledCategory[] = this.getFromStorage("favoriteLabeledCategories");
            var favoriteLabeledCategory: IFavoriteLabeledCategory = null;

            for (let i = 0; i < favoriteLabeledCategories.length; i++) {
                var item = favoriteLabeledCategories[i];
                if (item.id === categoryId) {
                    favoriteLabeledCategory = item;
                    break;
                }
            }
            if (favoriteLabeledCategory == null) {
                favoriteLabeledCategory = {
                    id: categoryId,
                    favoriteInfo: []
                };
                favoriteLabeledCategories.push(favoriteLabeledCategory);
            }

            var newFavoriteCategory: IFavoriteBaseInfoWithLabel = {
                id: this.generateLocalId(),
                createTime: this.getCurrentTime(),
                favoriteLabel: this.getDefaultFavoriteLabel(),
                favoriteType: FavoriteType.Category,
                title: favoriteTitle
            };
            favoriteLabeledCategory.favoriteInfo.push(newFavoriteCategory);

            this.storage.save("favoriteLabeledCategories", favoriteLabeledCategories);
            callback([newFavoriteCategory.id], null);
            return;
        }

        this.apiClient.createFavoriteCategory(categoryId, favoriteTitle, favoriteLabelIds, callback);
    }

    public createFavoriteQuery(bookType: BookTypeEnum, queryType: QueryTypeEnum, query: string, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        if (!this.isUserLoggedIn) {
            var favoriteQueries = <IFavoriteQuery[]>this.getFromStorage("favoriteQueries");
            var favoriteQuery: IFavoriteQuery = {
                id: this.generateLocalId(),
                createTime: this.getCurrentTime(),
                title: favoriteTitle,
                query: query,
                bookType: bookType,
                queryType: queryType,
                favoriteLabel: this.getDefaultFavoriteLabel()
            }

            favoriteQueries.push(favoriteQuery);
            this.storage.save("favoriteQueries", favoriteQueries);

            callback([favoriteQuery.id], null);
            return;
        }

        this.apiClient.createFavoriteQuery(bookType, queryType, query, favoriteTitle, favoriteLabelIds, callback);
    }

    public createPageBookmark(bookId: number, pageId: number, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        if (!this.isUserLoggedIn) {
            var favoritePageBookmarkItems: IPageBookmarkStorageItem[] = this.getFromStorage("favoritePageBookmarkItems");
            var item: IPageBookmarkStorageItem = null;
            for (var i = 0; i < favoritePageBookmarkItems.length; i++) {
                var storageItem = favoritePageBookmarkItems[i];
                if (storageItem.bookId === bookId) {
                    item = storageItem;
                    break;
                }
            }
            if (item == null) {
                item = {
                    bookId: bookId,
                    bookmarks: []
                };
                favoritePageBookmarkItems.push(item);
            }

            var bookmark: IBookPageBookmark = {
                id: this.generateLocalId(),
                title: favoriteTitle,
                pageId: pageId,
                favoriteLabel: this.getDefaultFavoriteLabel()
            };
            item.bookmarks.push(bookmark);

            this.storage.save("favoritePageBookmarkItems", favoritePageBookmarkItems);
            callback([bookmark.id], null);
            return;
        }

        this.apiClient.createPageBookmark(bookId, pageId, favoriteTitle, favoriteLabelIds, callback);
    }
}

class FavoriteHelper {
    static isValidHexColor(color: string): boolean {
        return new HexColor(color).isValidHexColor();
    }
    
    static getFontColor(hexBackgroundColor: string): string {
        var color = new HexColor(hexBackgroundColor);
        if (!color.isValidHexColor()) {
            console.warn("Invalid color format: " + hexBackgroundColor);
            return "#000000";
        }

        var brightness = color.getColorBrightness();
        return brightness > 192 ? "#000000" : "#FFFFFF";
    }

    static getDefaultFontColor(color: HexColor): string {
        if (!color.isValidHexColor()) {
            return "#000000";
        }

        var brightness = color.getColorBrightness();
        return brightness > 192 ? "#000000" : "#FFFFFF";
    }

    static getInactiveFontColor(): string {
        return "#808080";
    }

    static getDefaultBorderColor(color: HexColor): string {
        if (!color.isValidHexColor()) {
            return "#000000";
        }

        return color.getDecreasedHexColor(30);
    }

    static getDefaultLabelColorData(color: string): IFavoriteLabelColorData {
        var hexColor = new HexColor(color);
        var borderColor = FavoriteHelper.getDefaultBorderColor(hexColor);
        var fontColor = FavoriteHelper.getDefaultFontColor(hexColor);
        return {
            backgroundColor: color,
            borderColor: borderColor,
            fontColor: fontColor
        };
    }
}

interface IFavoriteLabelColorData {
    backgroundColor: string;
    borderColor: string;
    fontColor: string;
}

interface IPageBookmarkStorageItem {
    bookId: number;
    bookmarks: IBookPageBookmark[];
}

interface IFavoriteStorageItem {
    favoriteType: FavoriteType;
    storageItemIndex: number;
    storageIndex: number;
    storageItem: any;
    storage: any;
}