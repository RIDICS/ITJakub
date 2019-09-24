class TextEditorMain {
    private numberOfPages: number = 0;
    private showPageNumber = false;

    getNumberOfPages(): number {
        return this.numberOfPages;
    }

    getShowPageNumbers(): boolean {
        return this.showPageNumber;
    }

    init(projectId: number) {
        const util = new EditorsApiClient();
        const projectAjax = util.getPagesList(projectId);
        projectAjax.done((data) => {
            if (data.length) {
                const connections = new Connections();
                const commentArea = new CommentArea(util);
                const commentInput = new CommentInput(commentArea, util);
                const pageTextEditor = new Editor(commentInput, util, commentArea);
                const pageStructure = new PageStructure(commentArea, util, this, pageTextEditor);
                const lazyLoad = new PageLazyLoading(pageStructure);
                const pageNavigation = new TextEditorPageNavigation(this);
                connections.init();
                const numberOfPages = data.length;
                this.numberOfPages = numberOfPages;
                for (let i = 0; i < numberOfPages; i++) {
                    const projectPage = data[i];
                    let commentAreaClass = "";
                    if (i % 2 === 0) {
                        commentAreaClass =
                            "comment-area-collapsed-even"; //style even and odd comment sections separately
                    } else {
                        commentAreaClass = "comment-area-collapsed-odd";
                    }
                    const compositionAreaDiv =
                        `<div class="col-xs-7 composition-area"><div class="loading composition-area-loading"></div><div class="page"><div class="viewer"><span class="rendered-text"></span></div></div></div>`;
                    const commentAreaDiv = `<div class="col-xs-5 comment-area ${
                        commentAreaClass}"></div>`;
                    
                    const pageToolbarDiv = `<div class="col-xs-12 page-toolbar">
                                                <div class="row">
                                                    <div class="col-xs-4">
                                                        <button type="button" class="btn btn-default edit-page" title="${localization.translate("EditPage", "RidicsProject").value}">
                                                            <i class="fa fa-pencil"></i>
                                                        </button>
                                                    </div>
                                                    <div class="col-xs-4 page-number text-center invisible">
                                                        [${projectPage.name}]
                                                    </div>
                                                    <div class="col-xs-4"></div>
                                                </div>
                                            </div>`;
                    $(".pages-start")
                        .append(
                            `<div class="page-splitter"></div>
                            <div class="page-row row lazyload comment-never-loaded" data-page-id="${projectPage.id}" data-page-name="${projectPage.name}">
                                ${pageToolbarDiv}
                                ${compositionAreaDiv}
                                ${commentAreaDiv}
                            </div>`);
                }
                pageTextEditor.init();
                lazyLoad.init();
                pageNavigation.init(data);
                this.attachEventShowPageCheckbox(pageNavigation);
                commentInput.init();
                commentArea.init();
                
            } else {
                const error = new AlertComponentBuilder(AlertType.Error)
                    .addContent(localization.translate("NoTextPages", "RidicsProject").value);
                $("#project-resource-preview").empty().append(error.buildElement());
            }
        });
        projectAjax.fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate("ProjectLoadFailed", "RidicsProject").value);
            $("#project-resource-preview").empty().append(error.buildElement());
        });
    }

    private attachEventShowPageCheckbox(pageNavigation: TextEditorPageNavigation) {
        $("#project-resource-preview").on("click",
            ".display-page-checkbox",
            () => {
                const isChecked = $(".display-page-checkbox").prop("checked");
                this.showPageNumber = isChecked;
                pageNavigation.togglePageNumbers(isChecked);
            });
    }
}