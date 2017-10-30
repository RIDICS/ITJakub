///<reference path="../../../../../wwwroot/lib-custom/@types/simplemde/index.d.ts" />
///<reference path="../../../../../node_modules/@types/jquery/index.d.ts" />
///<reference path="./ridics.project.text-editor.connections.ts" />
///<reference path="./ridics.project.text-editor.editor.ts" />
///<reference path="./ridics.project.text-editor.util.ts" />
///<reference path="./ridics.project.text-editor.comment-area.ts" />
///<reference path="./ridics.project.text-editor.comment-input.ts" />
///<reference path="./ridics.project.text-editor.page-structure.ts" />
///<reference path="./ridics.project.text-editor.page-navigation.ts" />
///<reference path="./ridics.project.text-editor.lazyloading.ts" />
///<reference path="./ridics.project.text-editor.gui.ts" />

declare var lazySizes: any;

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
        const gui = new TextEditorGui();
        const connections = new Connections();
        const util = new Util();
        const commentArea = new CommentArea(util, gui);
        const commentInput = new CommentInput(commentArea, util, gui);
        const pageTextEditor = new Editor(commentInput, util, gui, commentArea);
        const pageStructure = new PageStructure(commentArea, util, this, pageTextEditor, gui);
        const lazyLoad = new PageLazyLoading(pageStructure, pageTextEditor);
        const pageNavigation = new PageNavigation(this, gui);
        const projectAjax = util.getProjectContent(projectId);
        pageTextEditor.processAreaSwitch();
        connections.toggleConnections();
        projectAjax.done((data: ITextProjectPage[]) => {
            const numberOfPages = data.length;
            this.numberOfPages = numberOfPages;
            for (let i = 0; i < numberOfPages; i++) {
                const textProjectPage = data[i];
                var commentAreaClass = "";
                if (i % 2 === 0) {
                    commentAreaClass = "comment-area-collapsed-even"; //style even and odd comment sections separately
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
            lazyLoad.lazyLoad();
            pageNavigation.init(data);
            this.attachEventShowPageCheckbox(pageNavigation);
            commentInput.processRespondToCommentClick();
            commentArea.processToggleCommentAresSizeClick();
            commentArea.processToggleNestedCommentClick();
        });
        projectAjax.fail(() => {
            gui.showMessageDialog("Error", "Failed to get project information.");
        });
    }

    private attachEventShowPageCheckbox(pageNavigation: PageNavigation) {
        $("#project-resource-preview").on("click",
            ".display-page-checkbox",
            () => {
                const isChecked = $(".display-page-checkbox").prop("checked");
                this.showPageNumber = isChecked;
                pageNavigation.togglePageNumbers(isChecked);
            });
    }
}