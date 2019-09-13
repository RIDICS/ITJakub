///<reference path="./ridics.project.text-editor.connections.ts" />
///<reference path="./ridics.project.text-editor.editor.ts" />
///<reference path="../editors-common-base/ridics.project.editors.util.ts" />
///<reference path="./ridics.project.text-editor.comment-area.ts" />
///<reference path="./ridics.project.text-editor.comment-input.ts" />
///<reference path="./ridics.project.text-editor.page-structure.ts" />
///<reference path="./ridics.project.text-editor.page-navigation.ts" />
///<reference path="./ridics.project.text-editor.lazyloading.ts" />

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
        const util = new EditorsUtil();
        const projectAjax = util.getProjectContent(projectId);
        projectAjax.done((data: ITextWithPage[]) => {
            if (data.length) {
                const connections = new Connections();
                const commentArea = new CommentArea(util);
                const commentInput = new CommentInput(commentArea, util);
                const pageTextEditor = new Editor(commentInput, util, commentArea);
                const pageStructure = new PageStructure(commentArea, util, this, pageTextEditor);
                const lazyLoad = new PageLazyLoading(pageStructure);
                const pageNavigation = new TextEditorPageNavigation(this);
                pageTextEditor.init();
                connections.init();
                const numberOfPages = data.length;
                this.numberOfPages = numberOfPages;
                for (let i = 0; i < numberOfPages; i++) {
                    const textProjectPage = data[i];
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
                    const pageNameDiv = `<div class="page-number text-center invisible">[${textProjectPage.parentPage
                        .name
                        }]</div>`;
                    $(".pages-start")
                        .append(
                            `<div class="row page-row lazyload" data-page="${textProjectPage.id}" data-page-name="${
                            textProjectPage.parentPage.name
                            }">${pageNameDiv}${compositionAreaDiv}${
                            commentAreaDiv}</div>`);
                }
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