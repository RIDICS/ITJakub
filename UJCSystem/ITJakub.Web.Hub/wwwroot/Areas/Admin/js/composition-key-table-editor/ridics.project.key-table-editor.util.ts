class KeyTableUtilManager {
    //category section start
    getCategoryList(): JQuery.jqXHR<ICategoryContract[]> {
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
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/CreateCategory`, { category: category } as JQuery.PlainObject);
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
            } as JQuery.PlainObject);
        return ajax;
    }

    deleteCategory(categoryId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteCategory`, { categoryId: categoryId } as JQuery.PlainObject);
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
        return $.post(url, { request: payload } as JQuery.PlainObject);
    }

    getLiteraryGenreList(): JQuery.jqXHR<IGenreResponseContract[]> {
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
            } as JQuery.PlainObject);
        return ajax;
    }

    deleteGenre(literaryGenreId: number): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/DeleteLiteraryGenre`;
        const ajax = $.post(url,
            {
                literaryGenreId: literaryGenreId
            } as JQuery.PlainObject);
        return ajax;
    }
    //genre section end

    //kind section start
    getLitararyKindList(): JQuery.jqXHR<ILiteraryKindContract[]> {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetLiteraryKindList`);
        return ajax;
    }

    deleteLiteraryKind(literaryKindId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteLiteraryKind`, { literaryKindId: literaryKindId } as JQuery.PlainObject);
        return ajax;
    }

    createNewLiteraryKind(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateLiteraryKind`;
        const id = 0; //keyword doesn't have an id yet
        const payload: ILiteraryKindContract = {
            name: name,
            id: id
        };
        return $.post(url, { request: payload } as JQuery.PlainObject);
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
            } as JQuery.PlainObject);
        return ajax;
    }
    //kind section end

    //responsible person type section start
    getResponsiblePersonTypeList(): JQuery.jqXHR<IResponsibleType[]> {
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
            } as JQuery.PlainObject);
        return ajax;
    }
    deleteResponsiblePersonType(responsibleTypeId: number): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/DeleteResponsibleType`;
        const ajax = $.post(url,
            {
                responsibleTypeId: responsibleTypeId
            } as JQuery.PlainObject);
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
        return $.post(url, { request: payload } as JQuery.PlainObject);
    }
    //responsible person type section end
    //responsible person section start
    getResponsiblePersonList(start: number, count: number): JQuery.jqXHR<IPagedResult<IResponsiblePerson>> {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetResponsiblePersonList`,
            {
                start: start,
                count: count
            } as JQuery.PlainObject);
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
        return $.post(url, { request: payload } as JQuery.PlainObject);
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
            } as JQuery.PlainObject);
        return ajax;
    }
    deleteResponsiblePerson(responsiblePersonId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteResponsiblePerson`, { responsiblePersonId: responsiblePersonId } as JQuery.PlainObject);
        return ajax;
    }
    //responsible person section end

    //literary original section start
    getLiteraryOriginalList(): JQuery.jqXHR<ILiteraryOriginalContract[]> {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetLiteraryOriginalList`);
        return ajax;
    }

    deleteLiteraryOriginal(literaryOriginalId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteLiteraryOriginal`,
            { literaryOriginalId: literaryOriginalId } as JQuery.PlainObject);
        return ajax;
    }

    createNewLiteraryOriginal(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateLiteraryOriginal`;
        const id = 0; //keyword doesn't have an id yet
        const payload: ILiteraryOriginalContract = {
            name: name,
            id: id
        };
        return $.post(url, { request: payload } as JQuery.PlainObject);
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
            } as JQuery.PlainObject);
        return ajax;
    }

    //literary original section end
    //original author section start
    getOriginalAuthorList(start?: number, count?: number): JQuery.jqXHR<IPagedResult<IOriginalAuthor>> {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetOriginalAuthorList`,
            {
                start: start,
                count: count
            } as JQuery.PlainObject);
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
        return $.post(url, { request: payload } as JQuery.PlainObject);
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
            } as JQuery.PlainObject);
        return ajax;
    }
    deleteOriginalAuthor(authorId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteOriginalAuthor`, { authorId: authorId } as JQuery.PlainObject);
        return ajax;
    }
    //original author section end

    //keyword section start
    getKeywordList(start?: number, count?: number): JQuery.jqXHR<IPagedResult<IKeywordContract>> {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetKeywordList`, {
            start: start,
            count: count
        } as JQuery.PlainObject);
        return ajax;
    }

    deleteKeyword(keywordId: number): JQueryXHR {
        const ajax = $.post(`${getBaseUrl()}Admin/KeyTable/DeleteKeyword`, { keywordId: keywordId } as JQuery.PlainObject);
        return ajax;
    }

    createNewKeyword(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/KeyTable/CreateKeyword`;
        const id = 0; //keyword doesn't have an id yet
        const payload: IKeywordContract = {
            name: name,
            id: id
        };
        return $.post(url, { request: payload } as JQuery.PlainObject);
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
            } as JQuery.PlainObject);
        return ajax;
    }

    //keyword section end
}