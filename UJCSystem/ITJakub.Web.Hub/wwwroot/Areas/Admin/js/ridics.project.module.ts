$(document.documentElement).ready(() => {
    var projectModule = new ProjectModule();
    projectModule.init();
});

class ProjectModule {
    private currentModule: ProjectModuleBase;
    private projectId: number;

    constructor() {
        this.currentModule = null;
        this.projectId = Number($("#project-id").text());
    }

    public init() {
        var self = this;
        var $splitterButton = $("#splitter-button");
        $splitterButton.click(() => {
            var $leftMenu = $("#left-menu");
            if ($leftMenu.is(":visible")) {
                $leftMenu.hide("slide", { direction: "left" });
                $splitterButton.html("<span class=\"glyphicon glyphicon-menu-right\"></span>");
            } else {
                $leftMenu.show("slide", { direction: "left" });
                $splitterButton.html("<span class=\"glyphicon glyphicon-menu-left\"></span>");
            }
        });

        var $projectNavigationLinks = $("#project-navigation a");
        $projectNavigationLinks.click(function(e) {
            e.preventDefault();
            $projectNavigationLinks.removeClass("active");
            $(this).addClass("active");
            self.showModule($(e.currentTarget as Node as Element).attr("id"));
        });
        
        this.showModule(null);
    }

    public showModule(identificator: string) {
        $("#resource-panel").hide();
        switch (identificator) {
        case "project-navigation-root":
            this.currentModule = new ProjectWorkModule(this.projectId);
            break;
        case "project-navigation-image": //TODO
            this.currentModule = null;
            const imageViewer = new ProjectImageViewerModule(this.projectId);
            imageViewer.init();
            break;
        case "project-navigation-text": //TODO
            this.currentModule = null;
            const textPreview = new ProjectTextPreviewModule(this.projectId);
            textPreview.init();
            break;
        case "project-navigation-audio":
            this.currentModule = null;
            break;
        default:
            this.currentModule = new ProjectWorkModule(this.projectId);
        }
        if (this.currentModule !== null) {
            this.currentModule.init();
        }
    }
}

abstract class ProjectModuleBase {
    protected moduleTab: ProjectModuleTabBase;

    public abstract getModuleType(): ProjectModuleType;

    public abstract getTabsId(): string;

    public abstract initModule(): void;

    public abstract getTabPanelType(panelSelector: string): ProjectModuleTabType;

    public abstract getLoadTabPanelContentUrl(panelType: ProjectModuleTabType): string;

    public abstract makeProjectModuleTab(panelType: ProjectModuleTabType): ProjectModuleTabBase;

    protected initTabs() {
        var self = this;
        $(`#${this.getTabsId()} a`).click(function(e) {
            e.preventDefault();
            $(this).tab('show');
            var tabPanelSelector = $(this).attr("href");
            self.loadTabPanel(tabPanelSelector);
        });
    }

    public init() {
        var $contentContainer = $("#project-layout-content");
        var url = getBaseUrl() + "Admin/Project/ProjectModule?moduleType=" + Number(this.getModuleType());

        $contentContainer
            .empty()
            .addClass("loading")
            .load(url,
                null,
                (responseText, textStatus, xmlHttpRequest) => {
                    $contentContainer.removeClass("loading");
                    if (xmlHttpRequest.status === HttpStatusCode.Success) {
                        this.initTabs();
                        this.initModule();
                    } else {
                        var alert = new AlertComponentBuilder(AlertType.Error)
                            .addContent(localization.translate("ModuleError", "RidicsProject").value)
                            .buildElement();
                        $contentContainer.append(alert);
                    }
                });
    }

    loadTabPanel(tabPanelSelector: string) {
        var tabPanelType = this.getTabPanelType(tabPanelSelector);
        var $tabPanel = $(tabPanelSelector);
        var url = this.getLoadTabPanelContentUrl(tabPanelType);
        $tabPanel
            .html("<div class=\"loader\"></div>")
            .load(url,
                null,
                (responseText, textStatus, xmlHttpRequest) => {
                    if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                        var errorDiv = new AlertComponentBuilder(AlertType.Error)
                            .addContent(localization.translate("BookmarkError", "RidicsProject").value)
                            .buildElement();
                        $tabPanel.empty().append(errorDiv);
                        this.moduleTab = null;
                        return;
                    }

                    this.moduleTab = this.makeProjectModuleTab(tabPanelType);
                    if (this.moduleTab != null) {
                        this.moduleTab.initTab();
                    }
                });
    }
}

class ProjectImageViewerModule {
    private readonly projectId: number;

    constructor(projectId: number) {
        this.projectId = projectId;
    }

    init() {
        const url = getBaseUrl() + `Admin/Project/GetImageViewer?projectId=${this.projectId}`;
        const loadingSpinner = $(`<div class="loading"></div>`);
        const projectLayoutEl = $("#project-layout-content");
        projectLayoutEl.empty();
        projectLayoutEl.append(loadingSpinner);
        projectLayoutEl.load(url,
            (response, status, xhr) => {
                if (status === "error") {
                    const error = new AlertComponentBuilder(AlertType.Error).addContent("Image viewer loading error");
                    $("#project-layout-content").empty().append(error.buildElement());
                } else {
                    loadingSpinner.hide();
                    $("#project-resource-images").off();
                    const imageViewer = new ImageViewerMain();
                    imageViewer.init(this.projectId);
                }
            });
    }
}

class ProjectTextPreviewModule {
    private readonly projectId: number;

    constructor(projectId: number) {
        this.projectId = projectId;
    }

    init() {
        const url = getBaseUrl() + "Admin/Project/GetTextPreview";
        const projectLayoutEl = $("#project-layout-content");
        const loadingSpinner = $(`<div class="loading"></div>`);
        projectLayoutEl.empty();
        projectLayoutEl.append(loadingSpinner);
        projectLayoutEl.load(url,
            (response, status, xhr) => {
                if (status === "error") {
                    const error = new AlertComponentBuilder(AlertType.Error).addContent("Text preview loading error");
                    $("#project-layout-content").empty().append(error.buildElement());
                } else {
                    loadingSpinner.hide();
                    $("#project-resource-preview").off();
                    const main = new TextEditorMain();
                    main.init(this.projectId);
                }
            });
    }
}

class ProjectWorkModule extends ProjectModuleBase {
    private projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    getModuleType(): ProjectModuleType { return ProjectModuleType.Work; }

    getTabsId(): string { return "project-work-tabs" }

    initModule(): void {
        $("#resource-panel").hide();
        $("#project-work-tabs .active a").trigger("click");
    }

    getTabPanelType(panelSelector: string): ProjectModuleTabType {
        return <ProjectModuleTabType>$(panelSelector).data("panel-type");
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
        ($(".keywords-textarea")as any).tokenfield("enable");
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
        ($(".keywords-textarea")as any).tokenfield("disable");
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
    Work = 0,
    Resource = 1,
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

interface IProjectResource {
    id: number;
    name: string;
}