class PageLazyLoading {
    private readonly pageStructure: PageStructure;

    constructor(pageStructure: PageStructure) {
        this.pageStructure = pageStructure;
    }

    lazyLoad() {
        $(document).on("lazybeforeunveil",
            (event) => {
                var target = $(event.target);
                var page = target.data("pageq");
                this.pageStructure.createPage(page);
            });
    }
}