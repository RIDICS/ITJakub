
interface IUser {
    id: number;
    userName: string;
    email: string;
    firstName: string;
    lastName: string;
}

interface IGroup {
    id: number;
    name: string;
    description: string;
}

interface ICategory {
    id: number;
    description: string;
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
    category: IBookCategory;
}

interface IBookCategory {
    id: number;
    description: string;
}

interface ICategoryContent {
    categories: ICategory[];
    books: IBook[];
}