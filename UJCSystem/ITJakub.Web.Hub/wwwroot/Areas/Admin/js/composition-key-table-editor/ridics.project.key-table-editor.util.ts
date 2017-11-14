class KeyTableViewManager {
    loadEditor(editorType: KeyTableEditorType) {
        const url = `${getBaseUrl()}Admin/Project/KeyTableType`;
        $("#project-layout-content").load(url, { editorType: editorType });
    }

    createNewGenre(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/Project/CreateLiteraryGenre`;
        const id = 0; //genre doesn't have an id yet
        const payload: ILiteraryGenreContract = {
            name: name,
            id: id
        };
        return $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(payload),
            dataType: "json",
            contentType: "application/json; charset=UTF-8"
        });
    }

    renameGenre(name: string, id: number): JQueryXHR {
        const url = `${getBaseUrl()}Admin/Project/CreateLiteraryGenre`; //TODO change address
        const payload: ILiteraryGenreContract = {
            name: name,
            id: id
        };
        return $.ajax({
            type: "POST", //TODO needs PUT
            url: url,
            data: JSON.stringify(payload),
            dataType: "json",
            contentType: "application/json; charset=UTF-8"
        });
    }

    deleteGenre(id: number): JQueryXHR {
        const url = `${getBaseUrl()}Admin/Project/CreateLiteraryGenre`; //TODO change address
        return $.ajax({
            type: "POST", //TODO needs DELETE
            url: url,
            data: JSON.stringify(id),
            dataType: "json",
            contentType: "application/json; charset=UTF-8"
        });
    }

    getLitararyGenreList() {
        const ajax = $.get(`${getBaseUrl()}Admin/ContentEditor/GetLitararyGenreList`);
        return ajax;
    }

    getLitararyKindList() {
        const ajax = $.get(`${getBaseUrl()}Admin/ContentEditor/GetLiteraryKindList`);
        return ajax;
    }

    getCategoryList(): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/ContentEditor/GetCategoryList`);
        return ajax;
    }

    createNewCategory(description: string, parentCategoryId?: number): JQueryXHR {
        const id = 0;
        const externalId = "string";//TODO debug
        const category: ICategoryContract = {
            id: id,
            externalId: externalId,
            parentCategoryId: parentCategoryId,
            description:description
        };
        const ajax = $.post(`${getBaseUrl()}Admin/ContentEditor/CreateCategory`, { category: category });
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
        const ajax = $.post(`${getBaseUrl()}Admin/ContentEditor/RenameCategory`,
            {
                categoryId: categoryId,
                category: category
            });
        return ajax;
    }

    deleteCategory(categoryId: number): JQueryXHR {
        console.log(categoryId);
        const ajax = $.post(`${getBaseUrl()}Admin/ContentEditor/DeleteCategory`, { categoryId: categoryId });
        return ajax;
    }

    getLitararyOriginalList(): JQueryXHR {
        const ajax = $.get(`${getBaseUrl()}Admin/ContentEditor/GetLiteraryKindList`);
        return ajax;
    }
}