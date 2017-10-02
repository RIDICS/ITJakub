///<reference path="../../../../../wwwroot/lib-custom/@types/simplemde/index.d.ts" />
///<reference path="../../../../../node_modules/@types/jquery/index.d.ts" />
///<reference path="./ridics.project.text-editor.connections.ts" />
///<reference path="./ridics.project.text-editor.editor.ts" />
///<reference path="./ridics.project.text-editor.util.ts" />
///<reference path="./ridics.project.text-editor.comment-area.ts" />
///<reference path="./ridics.project.text-editor.comment-input.ts" />
///<reference path="./ridics.project.text-editor.page-structure.ts" />
///<reference path="./ridics.project.text-editor.lazyloading.ts" />

class TextEditorMain {
    init() {
        const connections = new Connections();
        const util = new Util();
        const commentArea = new CommentArea(util);
        const commentInput = new CommentInput(commentArea, util);
        const pageTextEditor = new Editor(commentInput, util);
        const pageStructure = new PageStructure(commentArea, util);
        const lazyLoad = new PageLazyLoading(pageStructure);

        pageTextEditor.processPageModeSwitch();

        pageTextEditor.processAreaSwitch();

        connections.toggleConnections();
        for (let i = 1; i <= 2000; i++) { //TODO remove after debugging
            $(".pages-start").append(`<div class="row page-row lazyload" data-pageq="${i}"></div>`);
        }
        lazyLoad.lazyLoad();
        commentInput.processRespondToCommentClick();
        commentArea.processToggleCommentAresSizeClick();
        commentArea.processToggleNestedCommentClick();
    }

}