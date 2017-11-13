class KeyTableViewManager {
    loadEditor(editorType: KeyTableEditorType) {
        const url = `${getBaseUrl()}Admin/Project/KeyTableType`;
        $("#project-layout-content").load(url, { editorType: editorType });
    }

    createNewGenre(name: string): JQueryXHR {
        const url = `${getBaseUrl()}Admin/Project/CreateLiteraryGenre`;
        const id = 0; //genre doesn't have an id yet
        const payload: ILiteraryGenreContract = {
            id: id,
            name: name
        };
        return $.post(url, { request: payload });//TODO use plain ajax, returns 415 otherwise
    }
}