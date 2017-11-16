class KeyTableUtilManager {
    //category section start
    getCategoryList(): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetCategoryList`);
        return ajax;
    }

    createNewCategory(description: string, parentCategoryId?: number): JQueryXHR {
        const id = 0;
        const externalId = "string";//TODO debug
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
        const externalId = "string";//TODO debug
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

    getLitararyGenreList() {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetLitararyGenreList`);
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
    //kind section end

    //responsible person editor section start

    //responsible person editor section end

    //responsible person section start

    //responsible person section end

    //literary original section start
    getLitararyOriginalList(): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/KeyTable/GetLiteraryKindList`);
        return ajax;
    }
    //literary original section end

    //original author section start

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