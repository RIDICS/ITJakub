$(document.documentElement).ready(() => {
    var projectModule = new ProjectModule();
    projectModule.init();
});

class ProjectModule {
    private readonly projectId: number;
    private currentModule: ProjectModuleBase;

    constructor() {
        this.currentModule = null;
        this.projectId = Number($("#project-id").text());
    }

    public init() {
        const $splitterButton = $("#splitter-button");
        $splitterButton.on("click", () => {
            const $leftMenu = $("#left-menu");
            if ($leftMenu.is(":visible")) {
                $leftMenu.hide("slide", {direction: "left"});
                $splitterButton.html("<span class=\"glyphicon glyphicon-menu-right\"></span>");
            } else {
                $leftMenu.show("slide", {direction: "left"});
                $splitterButton.html("<span class=\"glyphicon glyphicon-menu-left\"></span>");
            }
        });

        const $projectNavigationLinks = $("#project-navigation a");
        $projectNavigationLinks.on("click", (e) => {
            e.preventDefault();
            $projectNavigationLinks.removeClass("active");
            const navigationLink = $(e.currentTarget);
            navigationLink.addClass("active");
            this.showModule(navigationLink.attr("id"));
        });

        const activeLink = $("#project-navigation a.active");
        this.showModule(activeLink.attr("id"));
    }

    public showModule(identificator: string) {
        $("#resource-panel").hide();
        switch (identificator) {
            case "project-navigation-image":
                this.currentModule = new ProjectImageViewerModule(this.projectId);
                break;
            case "project-navigation-text":
                this.currentModule = new ProjectTextPreviewModule(this.projectId);
                break;
            case "project-navigation-terms":
                this.currentModule = new ProjectTermEditorModule(this.projectId);
                break;
            default:
                this.currentModule = new ProjectWorkModule(this.projectId, identificator);
        }

        if (this.currentModule !== null) {
            this.currentModule.init();
        }
    }
}

abstract class ProjectModuleBase {
    protected readonly projectId: number;

    protected constructor(projectId: number) {
        this.projectId = projectId;
    }

    public abstract getModuleType(): ProjectModuleType;

    public abstract initModule(): void;

    public init() {
        const $contentContainer = $("#project-layout-content");
        const url = `${getBaseUrl()}Admin/Project/ProjectModule?moduleType=${Number(this.getModuleType())}&projectId=${this.projectId}`;

        $contentContainer
            .html("<div class=\"loader\"></div>")
            .load(url,
                null,
                (responseText, textStatus, xmlHttpRequest) => {
                    if (xmlHttpRequest.status === HttpStatusCode.Success) {
                        this.initModule();
                    } else {
                        var alert = new AlertComponentBuilder(AlertType.Error)
                            .addContent(localization.translate("ModuleError", "RidicsProject").value)
                            .buildElement();
                        $contentContainer.empty().append(alert);
                    }
                });
    }
}

class ProjectImageViewerModule extends ProjectModuleBase {

    constructor(projectId: number) {
        super(projectId);
    }

    getModuleType(): ProjectModuleType {
        return ProjectModuleType.ImageEditor;
    }

    initModule(): void {
        const imageViewer = new ImageViewerMain();
        imageViewer.init(this.projectId);
    }
}

class ProjectTextPreviewModule extends ProjectModuleBase {

    constructor(projectId: number) {
        super(projectId);
    }

    getModuleType(): ProjectModuleType {
        return ProjectModuleType.Preview;
    }

    initModule() {
        const main = new TextEditorMain();
        main.init(this.projectId);
    }
}

class ProjectTermEditorModule extends ProjectModuleBase {

    constructor(projectId: number) {
        super(projectId)
    }

    getModuleType(): ProjectModuleType {
        return ProjectModuleType.TermEditor;
    }

    initModule(): void {
        const termEditorMain = new TermEditorMain();
        termEditorMain.init(this.projectId);
    }
}

class ProjectWorkModule extends ProjectModuleBase {
    private moduleIdentificator: string;
    private moduleTab: ProjectModuleTabBase;

    constructor(projectId: number, moduleIdentificator: string) {
        super(projectId);
        this.moduleIdentificator = moduleIdentificator;
    }

    getModuleType(): ProjectModuleType {
        return null
    }

    initModule(): void {
    }

    getTabPanelType(panelSelector: string): ProjectModuleTabType {
        return <ProjectModuleTabType>$(`#${panelSelector}`).data("panel-type");
    }

    getLoadTabPanelContentUrl(tabPanelType: ProjectModuleTabType): string {
        return getBaseUrl() +
            "Admin/Project/ProjectWorkModuleTab?tabType=" +
            tabPanelType +
            "&projectId=" +
            this.projectId;
    }

    makeProjectModuleTab(tabPanelType: ProjectModuleTabType): ProjectModuleTabBase {
        switch (tabPanelType) {
            case ProjectModuleTabType.WorkMetadata:
                return new ProjectWorkMetadataTab(this.projectId, this);
            case ProjectModuleTabType.WorkPageList:
                return new ProjectWorkPageListTab(this.projectId);
            case ProjectModuleTabType.WorkPublications:
                return new ProjectWorkPublicationsTab(this.projectId);
            case ProjectModuleTabType.WorkCooperation:
                return new ProjectWorkCooperationTab(this.projectId);
            case ProjectModuleTabType.WorkHistory:
                return new ProjectWorkHistoryTab(this.projectId);
            case ProjectModuleTabType.WorkNote:
                return new ProjectWorkNoteTab(this.projectId);
            case ProjectModuleTabType.Forum:
                return new ProjectWorkForumTab(this.projectId);
            case ProjectModuleTabType.WorkCategorization:
                return new ProjectWorkCategorizationTab(this.projectId, this);
            case ProjectModuleTabType.WorkChapters:
                return new ProjectWorkChapterEditorTab(this.projectId);
            default:
                return null;
        }
    }

    init() {
        const tabPanelType = this.getTabPanelType(this.moduleIdentificator);
        const $contentContainer = $("#project-layout-content");
        const url = this.getLoadTabPanelContentUrl(tabPanelType);
        $contentContainer
            .html("<div class=\"loader\"></div>")
            .load(url,
                null,
                (responseText, textStatus, xmlHttpRequest) => {
                    if (xmlHttpRequest.status == HttpStatusCode.Success) {
                        this.moduleTab = this.makeProjectModuleTab(tabPanelType);
                        if (this.moduleTab != null) {
                            this.moduleTab.initTab();
                        }
                    }
                    else {
                        const errorDiv = new AlertComponentBuilder(AlertType.Error)
                            .addContent(localization.translate("BookmarkError", "RidicsProject").value)
                            .buildElement();
                        $contentContainer.empty().append(errorDiv);
                        this.moduleTab = null;
                    }                    
                });
    }

    loadTabPanel(moduleIdentificator: string) {
        this.moduleIdentificator = moduleIdentificator;
        this.init();
    }
}

abstract class ProjectModuleTabBase {
    public abstract initTab();
}

interface IProjectMetadataTabConfiguration {
    $panel: JQuery;
    $viewButtonPanel: JQuery;
    $editorButtonPanel: JQuery;
}

abstract class ProjectMetadataTabBase extends ProjectModuleTabBase {
    public abstract getConfiguration(): IProjectMetadataTabConfiguration;

    initTab() {
        this.disableEdit();
    }

    protected enabledEdit() {
        ($(".keywords-textarea") as any).tokenfield("enable");
        var config = this.getConfiguration();
        const copyrightTextarea = $("#work-metadata-copyright");
        var $inputs = $("input", config.$panel);
        var $selects = $("select", config.$panel);
        var $buttons = $("button", config.$panel);

        config.$viewButtonPanel.hide();
        config.$editorButtonPanel.show();
        $inputs.add($selects).add(copyrightTextarea).prop("disabled", false);
        $buttons.show();
    }

    protected disableEdit() {
        ($(".keywords-textarea") as any).tokenfield("disable");
        var config = this.getConfiguration();
        const copyrightTextarea = $("#work-metadata-copyright");
        var $inputs = $("input", config.$panel);
        var $selects = $("select", config.$panel);
        var $buttons = $("button", config.$panel);

        config.$viewButtonPanel.show();
        config.$editorButtonPanel.hide();
        $inputs.add($selects).add(copyrightTextarea).prop("disabled", true);
        $buttons.hide();
    }
}

enum ProjectModuleType {
    Resource = 0,
    Preview = 1,
    TermEditor = 2,
    ImageEditor = 3,
}

enum ProjectModuleTabType {
    WorkPublications = 0,
    WorkPageList = 1,
    WorkCooperation = 2,
    WorkMetadata = 3,
    WorkHistory = 4,
    WorkNote = 5,
    WorkCategorization = 6,
    WorkChapters = 7,
    ResourceDiscussion = 102,
    ResourceMetadata = 103,
    Forum = 200,
}