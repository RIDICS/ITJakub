
interface IGroup {
    id: number;
    name: string;
    description: string;
}

interface ICategory {
    id: number;
    description: string;
}

interface ICategoryOrBookType extends ICategory {
    bookType: BookTypeEnum;
}

interface IBook {
    id: number;
    guid: string;
    title: string;
}

interface ISpecialPermission {
    id: number;
}


interface ICardFilePermission extends ISpecialPermission {
    cardFileId: string;
    cardFileName: string;
}

interface IAutoImportPermission extends ISpecialPermission {
    bookType: BookTypeEnum;
}

interface IBookCategory {
    id: number;
    description: string;
}

interface ICategoryContent {
    categories: ICategoryOrBookType[];
    books: IBook[];
}