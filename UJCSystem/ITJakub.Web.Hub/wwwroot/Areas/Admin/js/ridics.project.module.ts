$(document.documentElement).ready(() => {
    var projectModule = new ProjectModule();
    projectModule.init();
});

class ProjectModule {
    private readonly projectId: number;
    private readonly client: ProjectClient;
    private readonly errorHandler: ErrorHandler;
    private currentModule: ProjectModuleBase;
    private renameProjectDialog: BootstrapDialogWrapper;
    private projectNavigationLinks: JQuery;

    constructor() {
        this.currentModule = null;
        this.client = new ProjectClient();
        this.errorHandler = new ErrorHandler();
        this.projectId = Number($("#project-id").text());

        this.renameProjectDialog = new BootstrapDialogWrapper({
            element: $("#renameProjectDialog"),
            autoClearInputs: false,
            submitCallback: this.renameProject.bind(this)
        });
    }

    public init() {        
        this.projectNavigationLinks = $("#project-navigation a");

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


        this.projectNavigationLinks.on("click", (e) => {
            e.preventDefault();
            const navigationLink = $(e.currentTarget);
            
            if (this.currentModule.isEditModeOpened()) {
                bootbox.dialog({
                    title: localization.translate("Warning", "RidicsProject").value,
                    message: localization.translate("SwitchTabWithoutSaving", "RidicsProject").value,
                    buttons: {
                        cancel: {
                            label: localization.translate("Cancel", "RidicsProject").value,
                            className: "btn-default",
                            callback: () => {
                                return;
                            }
                        },
                        confirm: {
                            label: localization.translate("Continue", "RidicsProject").value,
                            className: "btn-default",
                            callback: () => {
                                this.switchToLink(navigationLink);
                            }
                        }
                    }
                });
            } else {
                this.switchToLink(navigationLink);
            }
        });

        const activeLink = $("#project-navigation a.active");
        this.showModule(activeLink.attr("id"));

        $(".rename-project-button").on("click", () => {
            this.renameProjectDialog.show();
        });
    }

    private switchToLink(link: JQuery) {
        this.projectNavigationLinks.removeClass("active");
        link.addClass("active");
        this.showModule(link.attr("id"));
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

    private renameProject() {
        const newProjectName = $("#renameProjectInput").val() as string;

        if (newProjectName.length === 0) {
            this.renameProjectDialog.showError(localization.translate("EmptyProjectNameError", "Admin").value);
            return;
        }

        const projectTitle = $(".project-title");
        this.client.renameProject(this.projectId, newProjectName).done(() => {
            projectTitle.text(newProjectName);
            this.renameProjectDialog.hide();
        }).fail(error => {
            this.renameProjectDialog.showError(
                this.errorHandler.getErrorMessage(error, localization.translate("ErrorDuringSave", "Admin").value)
            );
        });
    }
}

abstract class ProjectModuleBase {
    protected readonly projectId: number;

    protected constructor(projectId: number) {
        this.projectId = projectId;
    }

    public abstract getModuleType(): ProjectModuleType;

    public abstract isEditModeOpened(): boolean;

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
                        var errorHandler = new ErrorHandler();
                        var errorMessage = errorHandler.getErrorMessage(xmlHttpRequest, localization.translate("ModuleError", "RidicsProject").value);
                        var alert = new AlertComponentBuilder(AlertType.Error)
                            .addContent(errorMessage)
                            .buildElement();
                        $contentContainer.empty().append(alert);
                    }
                });
    }
}

class ProjectImageViewerModule extends ProjectModuleBase {
    private editor: ImageViewerMain;
    
    constructor(projectId: number) {
        super(projectId);
    }

    getModuleType(): ProjectModuleType {
        return ProjectModuleType.ImageEditor;
    }

    initModule(): void {
        this.editor = new ImageViewerMain();
        this.editor.init(this.projectId);
    }

    isEditModeOpened(): boolean {
        return false;
    }
}

class ProjectTextPreviewModule extends ProjectModuleBase {
    private editor: TextEditorMain;
    
    constructor(projectId: number) {
        super(projectId);
    }

    getModuleType(): ProjectModuleType {
        return ProjectModuleType.Preview;
    }

    initModule() {
        this.editor = new TextEditorMain(this);
        this.editor.init(this.projectId);
    }

    isEditModeOpened(): boolean {
        return this.editor.isEditModeEnabled();
    }
}

class ProjectTermEditorModule extends ProjectModuleBase {
    private editor: TermEditorMain;
    
    constructor(projectId: number) {
        super(projectId);
    }

    getModuleType(): ProjectModuleType {
        return ProjectModuleType.TermEditor;
    }

    initModule(): void {
        this.editor = new TermEditorMain();
        this.editor.init(this.projectId);
    }

    isEditModeOpened(): boolean {
        return false;
    }
}

class ProjectWorkModule extends ProjectModuleBase {
    private moduleIdentificator: string;
    private moduleTab: ProjectModuleTabBase;
    private errorHandler: ErrorHandler;

    constructor(projectId: number, moduleIdentificator: string) {
        super(projectId);
        this.moduleIdentificator = moduleIdentificator;
        this.errorHandler = new ErrorHandler();
        this.moduleTab = null;
    }

    getModuleType(): ProjectModuleType {
        return null;
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
                    } else {
                        const errorMessage = this.errorHandler.getErrorMessage(xmlHttpRequest, localization.translate("BookmarkError", "RidicsProject").value);
                        const errorDiv = new AlertComponentBuilder(AlertType.Error)
                            .addContent(errorMessage)
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

    isEditModeOpened(): boolean {
        if (this.moduleTab == null) {
            return false;
        }

        return this.moduleTab.isEditModeOpened();
    }
}

abstract class ProjectModuleTabBase {
    public abstract initTab();
    public abstract isEditModeOpened(): boolean;
}

interface IProjectMetadataTabConfiguration {
    $panel: JQuery;
    $viewButtonPanel: JQuery;
    $editorButtonPanel: JQuery;
}

abstract class ProjectMetadataTabBase extends ProjectModuleTabBase {
    protected editModeEnabled: boolean;

    public abstract getConfiguration(): IProjectMetadataTabConfiguration;
    
    initTab() {
        this.disableEdit();
    }

    isEditModeOpened() {
        return this.editModeEnabled;
    }

    protected enabledEdit() {
        this.editModeEnabled = true;
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
        this.editModeEnabled = false;
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

class ProjectPermissionsProvider {
    private readonly permissionsJq = $("#project-permissions");

    private hasPermission(name: string) {
        return this.permissionsJq.attr(name).toLowerCase() === "true";
    }

    public hasEditPermission() {
        return this.hasPermission("data-edit");
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