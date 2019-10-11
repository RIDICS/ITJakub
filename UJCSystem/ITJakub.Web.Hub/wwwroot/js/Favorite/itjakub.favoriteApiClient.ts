class FavoriteApiClient {
    public getLatestFavoriteLabels(callback: (favoriteLabels: IFavoriteLabel[]) => void) {
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

    public getFavorites(labelId: number, filterByType: number, filterByTitle: string, sort: number, start: number, count: number, callback: (favorites: IFavoriteBaseInfo[]) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteList",
            data: {
                labelId: labelId,
                filterByType: filterByType,
                filterByTitle: filterByTitle,
                sort: sort,
                start: start,
                count: count
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (favorites: Array<IFavoriteBaseInfo>) => {
                callback(favorites);
            }
        });
    }

    public getFavoritesCount(labelId: number, filterByType: number, filterByTitle: string, callback: (count: number) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteListCount",
            data: {
                labelId: labelId,
                filterByType: filterByType,
                filterByTitle: filterByTitle
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (count: number) => {
                callback(count);
            }
        });
    }

    public getFavoritesForBooks(orBookType: BookTypeEnum, orBookIds: Array<number>, callback: (favoriteBooks: Array<IFavoriteLabeledBook>) => void): void {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteLabeledBooks",
            data: JSON.stringify({
                bookType: orBookType,
                bookIds: orBookIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: (bookList: Array<IFavoriteLabeledBook>) => {
                callback(bookList);
            }
        });
    }

    public getFavoritesForCategories(categoryIds: Array<number>, callback: (favoriteCategories: Array<IFavoriteLabeledCategory>) => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteLabeledCategories",
            data: JSON.stringify({
                categoryIds: categoryIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: (bookList: Array<IFavoriteLabeledCategory>) => {
                callback(bookList);
            }
        });
    }

    public getFavoriteLabelsForBooksAndCategories(bookType: BookTypeEnum, callback: (favoriteLabels: IFavoriteLabelsWithBooksAndCategories[], error: string) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteLabelsWithBooksAndCategories",
            data: {
                bookType: bookType
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (favoriteLabels) => {
                callback(favoriteLabels, null);
            },
            error: (request, status) => {
                callback(null, status);
            }
        });
    }

    public getFavoriteQueries(labelId: number, bookType: BookTypeEnum, queryType: QueryTypeEnum, filterByTitle: string, start: number, count: number, callback: (favoriteQueries: IFavoriteQuery[]) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteQueries",
            data: {
                labelId: labelId,
                bookType: bookType,
                queryType: queryType,
                filterByTitle: filterByTitle,
                start: start,
                count: count
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (queries) => {
                callback(queries);
            }
        });
    }

    public getFavoriteQueriesCount(labelId: number, bookType: BookTypeEnum, queryType: QueryTypeEnum, filterByTitle: string, callback: (count: number) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetFavoriteQueriesCount",
            data: {
                labelId: labelId,
                bookType: bookType,
                queryType: queryType,
                filterByTitle: filterByTitle
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (count) => {
                callback(count);
            }
        });
    }

    public getPageBookmarks(bookId: number, callback: (bookmarks: IBookPageBookmark[]) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Favorite/GetPageBookmarks",
            data: {
                bookId: bookId
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (bookmarks) => {
                callback(bookmarks);
            }
        });
    }

    public createFavoriteLabel(labelName: string, colorHex: string, callback: (id: number, error: string) => void) {
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
                callback(id, null);
            },
            error: (request, status) => {
                callback(null, status);
            }
        });
    }

    public updateFavoriteLabel(labelId: number, labelName: string, colorHex: string, callback: (error: string) => void) {
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
                callback(null);
            },
            error: (request, status) => {
                callback(status);
            }
        });
    }

    public deleteFavoriteLabel(labelId: number, callback: (error: string) => void) {
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
                callback(null);
            },
            error: (request, status) => {
                callback(status);
            }
        });
    }

    public updateFavoriteItem(favoriteId: number, title: string, callback: (error: string) => void) {
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
                callback(null);
            },
            error: (request, status) => {
                callback(status);
            }
        });
    }

    public deleteFavoriteItem(favoriteId: number, callback: (error: string) => void) {
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
                callback(null);
            },
            error: (request, status) => {
                callback(status);
            }
        });
    }

    public createFavoriteBook(bookId: number, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreateFavoriteBook",
            data: JSON.stringify({
                bookId: bookId,
                title: favoriteTitle,
                labelIds: favoriteLabelIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: (resultIds) => {
                callback(resultIds, null);
            },
            error: (request, status) => {
                callback(null, status);
            }
        });
    }

    public createFavoriteCategory(categoryId: number, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreateFavoriteCategory",
            data: JSON.stringify({
                categoryId: categoryId,
                title: favoriteTitle,
                labelIds: favoriteLabelIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: (resultIds) => {
                callback(resultIds, null);
            },
            error: (request, status) => {
                callback(null, status);
            }
        });
    }

    public createFavoriteQuery(bookType: BookTypeEnum, queryType: QueryTypeEnum, query: string, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreateFavoriteQuery",
            data: JSON.stringify({
                bookType: bookType,
                queryType: queryType,
                query: query,
                title: favoriteTitle,
                labelIds: favoriteLabelIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: (resultIds) => {
                callback(resultIds, null);
            },
            error: (request, status) => {
                callback(null, status);
            }
        });
    }

    public createPageBookmark(bookId: number, pageId: number, favoriteTitle: string, favoriteLabelIds: number[], callback: (ids: number[], error: string) => void) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Favorite/CreatePageBookmark",
            data: JSON.stringify({
                bookId: bookId,
                pageId: pageId,
                title: favoriteTitle,
                labelIds: favoriteLabelIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: (resultIds) => {
                callback(resultIds, null);
            },
            error: (request, status) => {
                callback(null, status);
            }
        });
    }
}