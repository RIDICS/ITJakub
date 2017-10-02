class PageLazyLoading {
    private readonly pageStructure: PageStructure;

    constructor(pageStructure: PageStructure) {
        this.pageStructure = pageStructure;
    }

    lazyLoad() {
        $(document).on("lazybeforeunveil",
            (event) => {
                var target = $(event.target);
                var page = target.data("page");
                this.pageStructure.createPage(page);
                $(target).children(".image-placeholder").remove();
            });
    }
}