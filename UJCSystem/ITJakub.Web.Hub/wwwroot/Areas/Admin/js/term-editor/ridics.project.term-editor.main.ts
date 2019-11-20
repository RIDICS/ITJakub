class TermEditorMain {
    private readonly client: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly termEditor: TermEditor;
    private navigation: PageNavigation;
    private projectId: number;

    constructor() {
        this.client = new EditorsApiClient();
        this.errorHandler = new ErrorHandler();
        this.termEditor = new TermEditor();
    }

    init(projectId: number) {
        this.projectId = projectId;
        const gui = new EditorsGui();
        
        this.navigation = new PageNavigation(gui, (pageId: number) => {
            this.loadTerms(pageId);
            this.loadPage(pageId);
        });

        this.navigation.init();
        this.termEditor.init([]);
    }

    private loadTerms(pageId: number) {
        const pageDetail = $(".content-terms");
        const alertHolder = pageDetail.find(".alert-holder");
        const content = pageDetail.find(".body-content");
        alertHolder.empty();

        if (typeof pageId == "undefined") {
            const alert = new AlertComponentBuilder(AlertType.Info)
                .addContent(localization.translate("EmptyPage", "RidicsProject").value).buildElement();
            alertHolder.empty().append(alert);
            content.empty();
            pageDetail.removeClass("hide");
            return;
        }

        content.empty().html("<div class=\"sub-content\"></div>");
        const subcontent = content.find(".sub-content");
        subcontent.html("<div class=\"loader\"></div>");
        pageDetail.removeClass("hide");

        this.client.getTermsByPageId(pageId).done((response) => {
            subcontent.html(response);
        }).fail((error) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
            alertHolder.empty().append(alert);
            subcontent.empty();
        });
    }

    private loadPage(pageId: number) {
        const pageDetail = $(".content-text");
        const textIcon = pageDetail.find(".fa-file-text-o");
        const imageIcon = pageDetail.find(".fa-image");
        const alertHolder = pageDetail.find(".alert-holder");
        const content = pageDetail.find(".body-content");
        alertHolder.empty();
        

        content.empty().html("<div class=\"sub-content\"></div>");
        const subcontent = content.find(".sub-content");
        subcontent.html("<div class=\"loader\"></div>");
        pageDetail.removeClass("hide");

        this.client.getPageDetail(pageId).done((response) => {
            subcontent.html(response);

            if (content.find(".page-text").length > 0) {
                textIcon.removeClass("hide");
            } else {
                textIcon.addClass("hide");
            }

            if (content.find(".image-preview").length > 0) {
                imageIcon.removeClass("hide");
            } else {
                imageIcon.addClass("hide");
            }
        }).fail((error) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
            alertHolder.empty().append(alert);
            subcontent.empty();
            textIcon.addClass("hide");
            imageIcon.addClass("hide");
        });
    }
}