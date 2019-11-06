class TextApiClient extends WebHubApiClient  {

    saveText(data: IStaticTextViewModel): JQuery.jqXHR<IModificationUpdateViewModel> {
        return this.post(this.getTextControllerUrl() + "SaveText",
            JSON.stringify(data));
    }

    renderPreview(text: string, inputTextFormat: string): JQuery.jqXHR<string> {
        return this.post(this.getTextControllerUrl() + "RenderPreview", JSON.stringify({
            text: text,
            inputTextFormat: inputTextFormat}));
    }
    
    private getTextControllerUrl(): string {
        return getBaseUrl() + "Text/";
    }
}