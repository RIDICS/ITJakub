///<reference path="../../../../../wwwroot/lib-custom/@types/simplemde/index.d.ts" />
///<reference path="../../../../../node_modules/@types/jquery/index.d.ts" />
///<reference path="./ridics.project.text-editor.connections.ts" />
///<reference path="./ridics.project.text-editor.editor.ts" />
///<reference path="./ridics.project.text-editor.util.ts" />
///<reference path="./ridics.project.text-editor.comment-area.ts" />
///<reference path="./ridics.project.text-editor.comment-input.ts" />
///<reference path="./ridics.project.text-editor.page-structure.ts" />

class TextEditorMain {
    init() {
        const connections = new Connections();
        const util = new Util();
        const commentArea = new CommentArea(util);
        const commentInput = new CommentInput(commentArea, util);
        const pageTextEditor = new Editor(commentInput, util);
        const pageStructure = new PageStructure(commentArea, util);

        pageTextEditor.processPageModeSwitch();

        pageTextEditor.processAreaSwitch();

        connections.toggleConnections();
        for (let i = 1; i <= 150; i++) { //TODO remove after debugging
            pageStructure.createPage(i);
        }
        commentInput.processRespondToCommentClick();
        commentArea.processToggleCommentAresSizeClick();
        commentArea.processToggleNestedCommentClick();
    }

}