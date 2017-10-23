class PageLazyLoading {
    private readonly pageStructure: PageStructure;

    constructor(pageStructure: PageStructure) {
        this.pageStructure = pageStructure;
    }

    lazyLoad() {
        this.initConfig();
        $(".pages-start").on("lazybeforeunveil",
            (event) => {
                var target = $(event.target);
                var page = target.data("page");
                this.pageStructure.createPage(page);
            });
    }

    initConfig() {
        var lazyConfig: any = (window as any).lazySizesConfig;
        lazyConfig.loadMode = 1; //only load visible elements
    }
}