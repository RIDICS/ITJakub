class KeyTableUtilManager {
    //category section start
    getCategoryList(): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetCategoryList`);
        return ajax;
    }

    createNewCategory(description: string, parentCategoryId?: number): JQueryXHR {
        const id = 0;
        const externalId = "string"; //TODO debug
        const category: ICategoryContract = {
            id: id,
            externalId: externalId,
            parentCategoryId: parentCategoryId,
            description: description
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/CreateCategory`, { category: category });
        return ajax;
    }

    renameCategory(categoryId: number, description: string, parentCategoryId?: number): JQueryXHR {
        const externalId = "string"; //TODO debug
        const category: ICategoryContract = {
            id: categoryId,
            externalId: externalId,
            parentCategoryId: parentCategoryId,
            description: description
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/RenameCategory`,
            {
                categoryId: categoryId,
                category: category
            });
        return ajax;
    }

    deleteCategory(categoryId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteCategory`, { categoryId: categoryId });
        return ajax;
    }
    //category section end

    //genre section start
    createNewGenre(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateLiteraryGenre`;
        const id = 0; //genre doesn't have an id yet
        const payload: ILiteraryGenreContract = {
            name: name,
            id: id
        };
        return $.post(url, { request: payload });
    }

    getLiteraryGenreList() {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetLiteraryGenreList`);
        return ajax;
    }

    renameGenre(genreId: number, name: string): JQueryXHR {
        const genre: ILiteraryGenreContract = {
            id: genreId,
            name: name
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/RenameLiteraryGenre`,
            {
                literaryGenreId: genreId,
                data: genre
            });
        return ajax;
    }

    deleteGenre(literaryGenreId: number): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/DeleteLiteraryGenre`;
        const ajax = $.post(url,
            {
                literaryGenreId: literaryGenreId
            });
        return ajax;
    }
    //genre section end

    //kind section start
    getLitararyKindList() {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetLiteraryKindList`);
        return ajax;
    }

    deleteLiteraryKind(literaryKindId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteLiteraryKind`, { literaryKindId: literaryKindId });
        return ajax;
    }

    createNewLiteraryKind(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateLiteraryKind`;
        const id = 0; //keyword doesn't have an id yet
        const payload: ILiteraryKindContract = {
            name: name,
            id: id
        };
        return $.post(url, { request: payload });
    }

    renameLiteraryKind(literaryKindId: number, name: string): JQueryXHR {
        const literaryKind: ILiteraryKindContract = {
            id: literaryKindId,
            name: name
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/RenameLiteraryKind`,
            {
                literaryKindId: literaryKindId,
                request: literaryKind
            });
        return ajax;
    }
    //kind section end

    //responsible person type section start
    getResponsiblePersonTypeList(): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetResponsibleTypeList`);
        return ajax;
    }
    renameResponsiblePersonType(responsibleTypeId: number, responsibleType: ResponsibleTypeEnum, text: string): JQueryXHR {
        const responsibleTypeBody: IResponsibleType = {
            id: responsibleTypeId,
            text: text,
            type: responsibleType
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/RenameResponsibleType`,
            {
                responsibleTypeId: responsibleTypeId,
                data: responsibleTypeBody
            });
        return ajax;
    }
    deleteResponsiblePersonType(responsibleTypeId: number): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/DeleteResponsibleType`;
        const ajax = $.post(url,
            {
                responsibleTypeId: responsibleTypeId
            });
        return ajax;
    }
    createNewResponsiblePersonType(responsibleType: ResponsibleTypeEnum, text: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateResponsibleType`;
        const id = 0; //genre doesn't have an id yet
        const payload: IResponsibleType = {
            text: text,
            id: id,
            type: responsibleType
        };
        return $.post(url, { request: payload });
    }
    //responsible person type section end
    //responsible person section start
    getResponsiblePersonList(start?: number, count?: number): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetResponsiblePersonList`,
            {
                start: start,
                count: count
            });
        return ajax;
    }

    createResponsiblePerson(name: string, surname: string) {//TODO
        const url = `${getBaseUrl()}Admin/KeyTable/CreateResponsiblePerson`;
        const id = 0; //keyword doesn't have an id yet
        const payload: IResponsiblePerson = {
            id: id,
            firstName: name,
            lastName: surname
        };
        return $.post(url, { request: payload });
    }
    renameResponsiblePerson(responsiblePersonId: number, name: string, surname: string): JQueryXHR {
        const responsiblePerson: IResponsiblePerson = {
            id: responsiblePersonId,
            firstName: name,
            lastName: surname
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/RenameResponsiblePerson`,
            {
                responsiblePersonId: responsiblePersonId,
                request: responsiblePerson
            });
        return ajax;
    }
    deleteResponsiblePerson(responsiblePersonId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteResponsiblePerson`, { responsiblePersonId: responsiblePersonId });
        return ajax;
    }
    //responsible person section end

    //literary original section start
    getLiteraryOriginalList(): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetLiteraryOriginalList`);
        return ajax;
    }

    deleteLiteraryOriginal(literaryOriginalId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteLiteraryOriginal`,
            { literaryOriginalId: literaryOriginalId });
        return ajax;
    }

    createNewLiteraryOriginal(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateLiteraryOriginal`;
        const id = 0; //keyword doesn't have an id yet
        const payload: ILiteraryOriginalContract = {
            name: name,
            id: id
        };
        return $.post(url, { request: payload });
    }

    renameLiteraryOriginal(literaryOriginalId: number, name: string): JQueryXHR {
        const literaryOriginal: ILiteraryOriginalContract = {
            id: literaryOriginalId,
            name: name
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/RenameLiteraryOriginal`,
            {
                literaryOriginalId: literaryOriginalId,
                request: literaryOriginal
            });
        return ajax;
    }

    //literary original section end
    //original author section start
    getOriginalAuthorList(start?: number, count?: number): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetOriginalAuthorList`,
            {
                start: start,
                count: count
            });
        return ajax;
    }

    createOriginalAuthor(name: string, surname: string) {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateAuthor`;
        const id = 0; //keyword doesn't have an id yet
        const payload: IOriginalAuthor = {
            id: id,
            firstName: name,
            lastName: surname
        };
        return $.post(url, { request: payload });
    }
    renameOriginalAuthor(authorId: number, name: string, surname: string): JQueryXHR {
        const originalAuthor: IOriginalAuthor = {
            id: authorId,
            firstName: name,
            lastName: surname
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/RenameOriginalAuthor`,
            {
                authorId: authorId,
                request: originalAuthor
            });
        return ajax;
    }
    deleteOriginalAuthor(authorId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteOriginalAuthor`, { authorId: authorId });
        return ajax;
    }
    //original author section end

    //keyword section start
    getKeywordList(): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetKeywordList`);
        return ajax;
    }

    deleteKeyword(keywordId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteKeyword`, { keywordId: keywordId });
        return ajax;
    }

    createNewKeyword(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateKeyword`;
        const id = 0; //keyword doesn't have an id yet
        const payload: IKeywordContract = {
            name: name,
            id: id
        };
        return $.post(url, { request: payload });
    }

    renameKeyword(keywordId: number, name: string): JQueryXHR {
        const keyword: ILiteraryGenreContract = {
            id: keywordId,
            name: name
        };
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/RenameKeyword`,
            {
                keywordId: keywordId,
                request: keyword
            });
        return ajax;
    }

    //keyword section end
}