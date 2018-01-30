interface IDropDownBookResult {
    id: number;
    title: string;
    categoryIds: Array<number>;
}

interface IDropDownCategoryResult {
    id: number;
    description: string;
    parentCategoryId: number;
}

interface IDropDownRequestResult {
    books: Array<IDropDownBookResult>;
    categories: Array<IDropDownCategoryResult>;
}

interface IDropdownFavoriteItem {
    id: number;
    favoriteInfo: Array<IFavoriteBaseInfoWithLabel>;
}
