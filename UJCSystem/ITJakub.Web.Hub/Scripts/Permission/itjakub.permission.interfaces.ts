

interface IUser {
    Id: number;
    UserName: string;
    Email: string;
    FirstName: string;
    LastName: string;
}

interface IGroup {
    Id: number;
    Name: string;
    Description: string;
}

interface ICategory {
    Id: number;
    Description: string;
}

interface IBook {
    Id: number;
    Guid: string;
    Title: string;
}

interface ISpecialPermission {
    Id: number;
}


interface ICardFilePermission extends ISpecialPermission {
    CardFileId: string;
    CardFileName: string;
}

interface IAutoImportPermission extends ISpecialPermission {
    Category: IBookCategory;
}

interface IBookCategory {
    Id: number;
    Description: string;
}

interface ICategoryContent {
    Categories: ICategory[];
    Books: IBook[];
}