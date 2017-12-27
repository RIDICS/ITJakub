class Connections {
    private interval: number;

    /**
     * Checks whether the commentator's name of a comment with a certain id is not visible.
     * @param {string} commentId Unique id of a commentary
     * @returns Whether the connection is not visible
     */
    private checkIfOverFlowing(commentId: string): boolean {
        let overflowing: boolean = false;
        const textEl = $(`#${commentId}-text`);
        if (length in textEl) {
            const pageTextOffsetTop = textEl.offset().top;
            const commentEl = $(`#${commentId}-comment`);
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

    private drawConnections(commentId: string): void {
        const from = $(`#${commentId}-text`);
        from.addClass("highlighted-element");
        const to = $(`#${commentId}-comment`).children().children(".media-object");
        jqSimpleConnect.connect(from,
            to,
            { radius: 2, color: "red", anchorA: "vertical", anchorB: "horizontal", roundedCorners: true });
    }

    private connectionsOnEnter(): void {
        $(document.documentElement).on("mouseenter",
            ".media-list",
            (event: JQuery.Event) => {
                event.stopImmediatePropagation();
                const target = event.target as HTMLElement;
                var thread = $(target).parents(".media-list");
                var uniqueIdWithText = $(thread).children(".media").children(".main-comment").attr("id");
                var uniqueId = uniqueIdWithText.replace("-comment", "");
                if (uniqueId !== null) {
                    if (!this.checkIfOverFlowing(uniqueId)) {
                        this.drawConnections(uniqueId);
                        this.interval = window.setInterval(() => {
                            if (this.checkIfOverFlowing(uniqueId)) {
                                $(".highlighted-element").removeClass("highlighted-element");
                                    jqSimpleConnect.removeAll();
                                } else {
                                    if ($(".jqSimpleConnect").length) {
                                        jqSimpleConnect.repaintAll();
                                    } else {
                                        this.drawConnections(uniqueId);
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