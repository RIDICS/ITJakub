declare var jqSimpleConnect: any; //TODO add type definitions

class Connections {
    private interval: number;

    /**
     * Checks whether the commentator's name of a comment with a certain id is not visible.
     * @param {string} id Unique id of a commentary
     * @returns Whether the connection is not visible
     */
    private checkIfOverFlowing(id: string): boolean {
        let overflowing: boolean = false;
        const name = $(`#${id}-comment`).siblings(".media-body").children(".comment-body");
        const commentAreaHeight = $(`#${id}-comment`).parents(".comment-area").height();
        if (name.position().top > commentAreaHeight || name.position().top < 15
        ) { //commenter's name is higher or lower than comment area
            overflowing = true;
        }
        return overflowing;
    }

    private drawConnections(id: string): void {//TODO investigate possibility of setting initial opacity
        const from = $(`#${id}-text`);
        const to = $(`#${id}-comment`).children().children(".media-object");
        jqSimpleConnect.connect(from,
            to,
            { radius: 2, color: "cyan", anchorA: "vertical", anchorB: "horizontal", roundedCorners: true });
    }

    private connectionsOnEnter(): void {
        $(document.documentElement).on("mouseenter",
            ".media-list",
            (event: JQueryEventObject) => {
                const target = event.target as HTMLElement;
                var thread = $(target).parents(".media-list");
                var uniqueIdWithText = $(thread).children(".media").children(".main-comment").attr("id");
                var uniqueId = uniqueIdWithText.replace("-comment", "");
                if (uniqueId !== null) {
                    if (!this.checkIfOverFlowing(uniqueId)) {
                        this.drawConnections(uniqueId);
                        this.interval = setInterval(() => {
                            if (this.checkIfOverFlowing(uniqueId)) {
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
                jqSimpleConnect.removeAll();
            });
    }

    toggleConnections(): void {
        this.connectionsOnEnter();
        this.connectionsOnLeave();
    }
}