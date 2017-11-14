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

    renameGenre(name: string, id:number): JQueryXHR {
        const url = `${getBaseUrl()}Admin/Project/CreateLiteraryGenre`;//TODO change address
        const payload: ILiteraryGenreContract = {
            name: name,
            id: id
        };
        return $.ajax({
            type: "POST",//TODO needs PUT
            url: url,
            data: JSON.stringify(payload),
            dataType: "json",
            contentType: "application/json; charset=UTF-8"
        });
    }

    deleteGenre(id: number): JQueryXHR {
        const url = `${getBaseUrl()}Admin/Project/CreateLiteraryGenre`;//TODO change address
        return $.ajax({
            type: "POST",//TODO needs DELETE
            url: url,
            data: JSON.stringify(id),
            dataType: "json",
            contentType: "application/json; charset=UTF-8"
        });
    }
}