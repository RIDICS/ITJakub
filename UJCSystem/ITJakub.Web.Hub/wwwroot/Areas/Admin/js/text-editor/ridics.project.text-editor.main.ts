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
    private numberOfPages: number;
    private showPageNumber = false;

    getNumberOfPages(): number {
        return this.numberOfPages;
    }

    getShowPageNumbers(): boolean {
        return this.showPageNumber;
    }

    init() {
        const connections = new Connections();
        const util = new Util();
        const commentArea = new CommentArea(util);
        const commentInput = new CommentInput(commentArea, util);
        const gui = new TextEditorGui();
        const pageTextEditor = new Editor(commentInput, util, gui);
        const pageStructure = new PageStructure(commentArea, util, this, pageTextEditor);
        const lazyLoad = new PageLazyLoading(pageStructure);
        const pageNavigation = new PageNavigation(this);
        const projectAjax = util.getProjectContent(1);//TODO debug
        pageTextEditor.processAreaSwitch();
        connections.toggleConnections();
        projectAjax.done((data: ITextProjectPage[]) => {
            const numberOfPages = data.length;
            this.numberOfPages = numberOfPages;
            for (let i = 0; i < numberOfPages; i++) {
                $(".pages-start")
                    .append(
                        `<div class="row page-row lazyload" data-page="${data[i].id
                        }" data-page-name="${data[i].parentPage.name}"><div class="image-placeholder loading"></div></div>`);
            }

            lazyLoad.lazyLoad();
            pageNavigation.init(data);
            this.attachEventShowPageCheckbox(pageNavigation);
            commentInput.processRespondToCommentClick();
            commentArea.processToggleCommentAresSizeClick();
            commentArea.processToggleNestedCommentClick();
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