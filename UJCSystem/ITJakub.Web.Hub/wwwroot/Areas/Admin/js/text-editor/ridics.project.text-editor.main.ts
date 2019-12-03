class TextEditorMain {
    private readonly workModule: ProjectModuleBase;
    private readonly client: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private projectId: number;
    private numberOfPages: number = 0;
    private maxPosition: number = 0;
    private resourcePreview: JQuery<HTMLElement>;

    constructor(workModule: ProjectModuleBase) {
        this.workModule = workModule;
        this.client = new EditorsApiClient();
        this.errorHandler = new ErrorHandler();
        this.resourcePreview = $("#project-resource-preview");
    }
    getNumberOfPages(): number {
        return this.numberOfPages;
    }

    isShowPageNumbers(): boolean {
        return this.resourcePreview.find(".display-page-checkbox").is(":checked");
    }

    init(projectId: number) {
        this.projectId = projectId;
        this.initAddPageDialog();
        const projectAjax = this.client.getPagesList(projectId);
        projectAjax.done((data) => {
            if (data.length) {
                const connections = new Connections();
                const commentArea = new CommentArea(this.client);
                const commentInput = new CommentInput(commentArea, this.client);
                const pageTextEditor = new Editor(commentInput, this.client, commentArea);
                const pageStructure = new PageStructure(commentArea, this.client, pageTextEditor);
                const lazyLoad = new PageLazyLoading(pageStructure);
                const pageNavigation = new TextEditorPageNavigation(this);
                connections.init();
                const numberOfPages = data.length;
                this.numberOfPages = numberOfPages;
                for (let i = 0; i < numberOfPages; i++) {
                    const projectPage = data[i];
                    if(projectPage.position > this.maxPosition)
                    {
                        this.maxPosition = projectPage.position;
                    }
                    
                    
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
                                                      <div class="page-toolbar-buttons">
                                                        <button type="button" class="btn btn-default refresh-text">
                                                            <i class="fa fa-refresh"></i>
                                                        </button>
                                                        <button type="button" class="btn btn-default create-text hidden" title="${localization.translate("CreateTextPage", "RidicsProject").value}">
                                                            <i class="fa fa-plus-circle"></i>
                                                            ${localization.translate("CreateText", "RidicsProject").value}
                                                        </button>
                                                        <button type="button" class="btn btn-default edit-page" title="${localization.translate("EditPage", "RidicsProject").value}">
                                                            <i class="fa fa-pencil"></i>
                                                            ${localization.translate("Edit", "RidicsProject").value}
                                                        </button>
                                                      </div>
                                                    </div>
                                                    <div class="col-xs-4 page-number text-center invisible">
                                                        [${projectPage.name}]
                                                    </div>
                                                    <div class="col-xs-4"></div>
                                                </div>
                                            </div>`;
                    
                    const alertHolder = `<div class="col-xs-12 alert-holder"></div>`;
                    
                    $(".pages-start")
                        .append(
                            `<div class="page-splitter"></div>
                            <div class="page-row row lazyload comment-never-loaded" data-page-id="${projectPage.id}" data-page-name="${projectPage.name}">
                                ${pageToolbarDiv}
                                ${alertHolder}
                                ${compositionAreaDiv}
                                ${commentAreaDiv}
                            </div>`);
                }
                pageTextEditor.init(pageStructure);
                lazyLoad.init();
                pageNavigation.init(data);
                pageNavigation.togglePageNumbers(this.isShowPageNumbers());
                this.attachEventShowPageCheckbox(pageNavigation);
                commentInput.init();
                commentArea.init();
                commentArea.initCommentsDeleting(pageTextEditor);

            } else {
                const error = new AlertComponentBuilder(AlertType.Error)
                    .addContent(localization.translate("NoPages", "RidicsProject").value);
                this.resourcePreview.empty().append(error.buildElement());
            }
        });
        projectAjax.fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate("ProjectLoadFailed", "RidicsProject").value);
            this.resourcePreview.empty().append(error.buildElement());
        });
        projectAjax.always(() => {
            this.resourcePreview.removeClass("hide");
        });
    }

    private attachEventShowPageCheckbox(pageNavigation: TextEditorPageNavigation) {
        this.resourcePreview.on("click",
            ".display-page-checkbox",
            () => {
                const isChecked = this.isShowPageNumbers();
                pageNavigation.togglePageNumbers(isChecked);
            });
    }

    private initAddPageDialog() {
        const dialog = $("#addPageDialog");

        $(".add-page-button").on("click", () => {
            dialog.modal("show");
        });

        $("#addPage").on("click", () => {
            const alertHolder = dialog.find(".alert-holder");
            alertHolder.empty();

            
            const newPageName = String(dialog.find("input[name=\"page-name\"]").val());
            if (newPageName !== "") {
                this.client.createPage(this.projectId, newPageName, this.maxPosition + 1).done(() => {
                    dialog.modal("hide").on("hidden.bs.modal", () => {
                        dialog.off("hidden.bs.modal");
                        this.reloadTab();
                    });                   
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error));
                    alertHolder.empty().append(alert.buildElement());
                });
            }
        });
    }

    private reloadTab() {
        this.workModule.init();
    }
}