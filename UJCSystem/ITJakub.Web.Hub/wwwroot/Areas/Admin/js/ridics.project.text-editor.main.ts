///<reference path="../../../../wwwroot/lib-custom/@types/simplemde/index.d.ts" />
///<reference path="../../../../node_modules/@types/jquery/index.d.ts" />
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
        for (let i = 1; i <= 15; i++) { //TODO remove after debugging
            pageStructure.createPage(i);
        }
        this.processRespondToCommentClick(commentInput);
        commentArea.processToggleCommentAresSizeClick();
        commentArea.processToggleNestedCommentClick();
    }

    processRespondToCommentClick(commentInput: CommentInput) {
        const commentButton = $(".respond-to-comment");
        commentButton.click(
            (event: JQueryEventObject) => { // Process click on "Respond" button
                const target = event.target as HTMLElement;
                const compositionAreaPage =
                    $(target).parents(".comment-area").siblings(".composition-area").children(".page");
                var page = $(compositionAreaPage).data("page") as number;
                const uniqueIdWithText = $(target).parent().siblings(".main-comment").attr("id");
                var uniqueId = uniqueIdWithText.replace("-comment", "");
                if (uniqueId !== null && typeof uniqueId !== "undefined") {
                    const responses = $(target).siblings(".media").length;
                    commentInput.addCommentFromCommentArea(uniqueId, page, responses + 1);
                } else {
                    console.log("Something is wrong. This comment doesn't have an id.");
                }
            });
    }
}