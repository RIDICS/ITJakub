class Connections {
    private interval: number;

    /**
     * Checks whether the commentator's name of a comment with a certain id is not visible.
     * @param {string} commentId Unique id of a commentary
     * @returns Whether the connection is not visible
     */
    private checkIfOverFlowing(commentId: string, container: JQuery): boolean {
        let overflowing: boolean = false;
        const textEl = container.find(".composition-area").find(`span[data-text-reference-id="${commentId}"]`);
        if (length in textEl) {
            const pageTextOffsetTop = textEl.offset().top;
            const commentEl = container.find(".comment-area").find(`div[data-text-reference-id="${commentId}"]`);
            const commentName = commentEl.siblings(".media-body").find(".media-heading");
            const pageContainer = $(".editor-areas");
            const pageBottom = pageContainer.offset().top + pageContainer.height();
            if (pageTextOffsetTop < pageContainer.offset().top ||
                pageTextOffsetTop > pageBottom ||
                commentName.position().top < 0 ||
                commentName.offset().top > pageBottom) { //related text is offscreen or commenters name is offscreen
                overflowing = true;
            }
        }
        return overflowing;
    }

    private drawConnections(commentId: string, container: JQuery): void {
        const from = container.find(".composition-area").find(`span[data-text-reference-id="${commentId}"]`);
        from.addClass("highlighted-element");
        const to = container.find(".comment-area").find(`div[data-text-reference-id="${commentId}"]`).children().children(".media-object");
        jqSimpleConnect.connect(from,
            to,
            { radius: 2, color: "red", anchorA: "vertical", anchorB: "horizontal", roundedCorners: true });
    }

    private connectionsOnEnter(): void {
        $(document.documentElement).on("mouseenter",
            ".media-list",
            (event) => {
                event.stopImmediatePropagation();
                const target = event.target as HTMLElement;
                const pageRow = $(target).parents(".page-row");
                var thread = $(target).parents(".media-list");
                var uniqueId = $(thread).children(".media").children(".main-comment").data("text-reference-id");
                if (uniqueId !== null) {
                    if (!this.checkIfOverFlowing(uniqueId, pageRow)) {
                        this.drawConnections(uniqueId, pageRow);
                        this.interval = window.setInterval(() => {
                            if (this.checkIfOverFlowing(uniqueId, pageRow)) {
                                $(".highlighted-element").removeClass("highlighted-element");
                                    jqSimpleConnect.removeAll();
                                } else {
                                    if ($(".jqSimpleConnect").length) {
                                        jqSimpleConnect.repaintAll();
                                    } else {
                                        this.drawConnections(uniqueId, pageRow);
                                    }
                                }
                            },
                            25);
                    }
                } else {
                    console.log("Something is wrong. This comment doesn't have an id.");
                }
            });
    }

    private connectionsOnLeave(): void {
        $(document.documentElement).on("mouseleave",
            ".media-list",
            () => {
                if (typeof this.interval !== "undefined") {
                    clearInterval(this.interval);
                }
                $(".highlighted-element").removeClass("highlighted-element");
                jqSimpleConnect.removeAll();
            });
    }

    init(): void {
        this.connectionsOnEnter();
        this.connectionsOnLeave();
    }
}