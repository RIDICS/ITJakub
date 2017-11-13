class KeyTableViewManager {
    loadEditor(editorType: KeyTableEditorType) {
        const url = `${getBaseUrl()}Admin/Project/KeyTableType`;
        $("#project-layout-content").load(url, { editorType: editorType });
    }
}