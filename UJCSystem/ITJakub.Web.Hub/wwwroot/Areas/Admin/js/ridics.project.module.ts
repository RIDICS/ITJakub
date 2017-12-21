$(document).ready(() => {
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
            self.showModule(e.currentTarget.id);
        });


        var dropzoneOptions = DropzoneHelper.getFullConfiguration({
            //url: getBaseUrl() + "Admin/Project/UploadResource"
            error: DropzoneHelper.getErrorFunction()
        });
        $("#new-resource-upload").dropzone(dropzoneOptions);
        $("#new-resource-version-upload").dropzone(dropzoneOptions);

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
            this.currentModule = new ProjectResourceModule(this.projectId, ResourceType.Audio);
            break;
        case "project-navigation-video":
            this.currentModule = new ProjectResourceModule(this.projectId, ResourceType.Video);
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
                            .addContent("Chyba při načítání modulu")
                            .buildElement();
                        $contentContainer.append(alert);
                    }
                });
    }

    private loadTabPanel(tabPanelSelector: string) {
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
                            .addContent("Chyba při načítání záložky.")
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
        const url = getBaseUrl() + "Admin/Project/GetImageViewer";
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
            return new ProjectWorkMetadataTab(this.projectId);
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
        default:
            return null;
        }
    }
}

class ProjectResourceModule extends ProjectModuleBase {
    private projectClient: ProjectClient;
    private resourceType: ResourceType;
    private projectId: number;
    private currentResourceId: number;
    private addResourceDialog: BootstrapDialogWrapper;
    private createResourceVersionDialog: BootstrapDialogWrapper;
    private deleteResourceDialog: BootstrapDialogWrapper;
    private renameResourceDialog: BootstrapDialogWrapper;
    private duplicateResourceDialog: BootstrapDialogWrapper;
    private resourceVersionModule: ProjectResourceVersionModule;

    constructor(projectId: number, resourceType: ResourceType) {
        super();
        this.projectId = projectId;
        this.resourceType = resourceType;
        this.projectClient = new ProjectClient();
    }

    getModuleType(): ProjectModuleType { return ProjectModuleType.Resource; }

    getTabsId(): string { return "project-resource-tabs" }

    initModule(): void {
        var self = this;

        $("#resource-panel").show();

        this.toggleElementsVisibility(false, null);
        ProjectResourceVersionModule.staticInit();
        this.initDialogs();
        this.initMainResourceButtons();
    }

    getTabPanelType(panelSelector: string): ProjectModuleTabType {
        return <ProjectModuleTabType>$(panelSelector).data("panel-type");
    }

    getLoadTabPanelContentUrl(tabPanelType: ProjectModuleTabType): string {
        return getBaseUrl() +
            "Admin/Project/ProjectResourceModuleTab?tabType=" +
            tabPanelType +
            "&resourceId=" +
            this.currentResourceId;
    }

    makeProjectModuleTab(tabPanelType: ProjectModuleTabType): ProjectModuleTabBase {
        switch (tabPanelType) {
        case ProjectModuleTabType.ResourceMetadata:
            return new ProjectResourceMetadataTab(this.currentResourceId);
        case ProjectModuleTabType.ResourceDiscussion:
            return new ProjectResourceDiscussionTab(this.currentResourceId);
        default:
            return null;
        }
    }

    private getSelectedResourceName(): string {
        var resourceList = <HTMLSelectElement>document.getElementById("resource-list");
        if (resourceList.selectedIndex < 0) {
            return null;
        }

        var selectedOption = resourceList.options[resourceList.selectedIndex];
        return $(selectedOption).text();
    }

    private initDialogs() {
        this.addResourceDialog = new BootstrapDialogWrapper({
            element: $("#new-resource-dialog"),
            submitCallback: this.addResource.bind(this),
            autoClearInputs: true
        });
        this.createResourceVersionDialog = new BootstrapDialogWrapper({
            element: $("#new-resource-version-dialog"),
            submitCallback: this.createResourceVersion.bind(this),
            autoClearInputs: true
        });
        this.renameResourceDialog = new BootstrapDialogWrapper({
            element: $("#rename-resource-dialog"),
            submitCallback: this.renameResource.bind(this),
            autoClearInputs: true
        });
        this.deleteResourceDialog = new BootstrapDialogWrapper({
            element: $("#delete-resource-dialog"),
            submitCallback: this.deleteResource.bind(this),
            autoClearInputs: false
        });
        this.duplicateResourceDialog = new BootstrapDialogWrapper({
            element: $("#duplicate-resource-dialog"),
            submitCallback: this.duplicateResource.bind(this),
            autoClearInputs: false
        });
    }

    private initMainResourceButtons() {
        $("#resource-panel button").off();

        $("#add-resource-button").click(() => {
            $("#new-resource-session-id").val(Guid.generate());
            this.addResourceDialog.show();
        });
        $("#create-resource-version-button").click(() => {
            $("#new-resource-version-session-id").val(Guid.generate());
            $("#new-resource-version-original").text(this.getSelectedResourceName());
            this.createResourceVersionDialog.show();
        });
        $("#rename-resource-button").click(() => {
            $("#rename-resource-original").val(this.getSelectedResourceName());
            this.renameResourceDialog.show();
        });
        $("#delete-resource-button").click(() => {
            $("#delete-resource-name").text(this.getSelectedResourceName());
            this.deleteResourceDialog.show();
        });
        $("#duplicate-resource-button").click(() => {
            $("#duplicate-resource-name").text(this.getSelectedResourceName());
            this.duplicateResourceDialog.show();
        });
    }

    private toggleElementsVisibility(isResourceSelected: boolean, resourceId: number) {
        var $tabs = $("#" + this.getTabsId());
        var $tabPanels = $("#resource-tab-content");
        var $selectionDependentButtons =
            $(
                "#delete-resource-button, #rename-resource-button, #duplicate-resource-button, #create-resource-version-button, #project-resource-version-button");
        this.currentResourceId = resourceId;

        $selectionDependentButtons.prop("disabled", !isResourceSelected);

        if (isResourceSelected) {
            $tabs.show();
            $tabPanels.show();
            $(".active a", $tabs).trigger("click");
        } else {
            $tabs.hide();
            $tabPanels.hide();
        }

        if (this.resourceVersionModule) {
            this.resourceVersionModule.directHide();
        }

        if (resourceId != null) {
            this.resourceVersionModule = new ProjectResourceVersionModule(resourceId);
            this.resourceVersionModule.init();
        } else {
            this.resourceVersionModule = null;
        }
    }

    private addResource() {
        var sessionId = $("#new-resource-session-id").val() as string;
        var comment = $("#new-resource-comment").val() as string;
        this.projectClient.processUploadedResources(this.projectId,
            sessionId,
            comment,
            errorCode => {
                if (errorCode != null) {
                    this.addResourceDialog.showError();
                    return;
                }

                this.toggleElementsVisibility(false, null);

                this.addResourceDialog.hide();
            });
    }

    private createResourceVersion() {
        var resourceId = this.currentResourceId;
        var sessionId = $("#new-resource-version-session-id").val() as string;
        var comment = $("#new-resource-version-comment").val() as string;
        this.projectClient.processUploadedResourceVersion(resourceId,
            sessionId,
            comment,
            errorCode => {
                if (errorCode != null) {
                    this.createResourceVersionDialog.showError();
                    return;
                }

                this.toggleElementsVisibility(false, null);

                this.createResourceVersionDialog.hide();
            });
    }

    private deleteResource() {
        var resourceId = this.currentResourceId;
        this.projectClient.deleteResource(resourceId,
            errorCode => {
                if (errorCode != null) {
                    this.deleteResourceDialog.showError();
                    return;
                }

                this.toggleElementsVisibility(false, null);

                this.deleteResourceDialog.hide();
            });
    }

    private renameResource() {
        var resourceId = this.currentResourceId;
        var newName = $("#rename-resource-new").val() as string;
        this.projectClient.renameResource(resourceId,
            newName,
            errorCode => {
                if (errorCode != null) {
                    this.renameResourceDialog.showError();
                    return;
                }

                this.renameResourceDialog.hide();
            });
    }

    private duplicateResource() {
        var resourceId = this.currentResourceId;
        var resourceName = $("#duplicate-resource-name").text();
        this.projectClient.duplicateResource(resourceId,
            (newResourceId, errorCode) => {
                if (errorCode != null) {
                    this.duplicateResourceDialog.showError();
                    return;
                }

                this.toggleElementsVisibility(false, null);

                this.duplicateResourceDialog.hide();
            });
    }
}

class ProjectResourceVersionModule {
    private resourceId: number;
    private $iconUp: JQuery;
    private $iconDown: JQuery;
    private versionPanelHeight: number;

    constructor(resourceId: number) {
        this.resourceId = resourceId;
    }

    public static staticInit() {
        $("#project-resource-version-button, #resource-version-panel").hide();
    }

    public init() {
        this.$iconUp = $("#project-resource-version-icon-up");
        this.$iconDown = $("#project-resource-version-icon-down");
        this.$iconDown.hide();
        $("#project-resource-version-button").show().off().click(() => {
            if (this.$iconUp.is(":visible")) {
                this.show();
            } else {
                this.hide();
            }
        });

        var $resourceVersionPanel = $("#resource-version-panel");
        $resourceVersionPanel.hide();
        this.versionPanelHeight = $resourceVersionPanel.innerHeight();
    }

    private show() {
        var $resourceVersionPanel = $("#resource-version-panel");
        var $resourceTabContent = $("#resource-tab-content");

        var url = getBaseUrl() + "Admin/Project/ProjectResourceVersion?resourceId=" + this.resourceId;
        $resourceVersionPanel
            .slideToggle()
            .append("<div class=\"loading\"></div>")
            .load(url,
                null,
                (responseText, textStatus, xmlHttpRequest) => {
                    if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                        var error = new AlertComponentBuilder(AlertType.Error).addContent(
                            "Chyba při načítání seznamu verzí");
                        $resourceVersionPanel.empty().append(error.buildElement());
                    }
                });

        $resourceTabContent.animate({
            height: "-=" + this.versionPanelHeight + "px"
        });


        this.$iconUp.hide();
        this.$iconDown.show();
    }

    private hide(animation: boolean = true) {
        var $resourceVersionPanel = $("#resource-version-panel");
        var $resourceTabContent = $("#resource-tab-content");

        if (animation) {
            $resourceVersionPanel.slideToggle(null, () => $resourceVersionPanel.empty());

            $resourceTabContent.animate({
                height: "+=" + this.versionPanelHeight + "px"
            });
        } else {
            $resourceVersionPanel.hide().empty();
            $resourceTabContent.height("");
        }

        this.$iconUp.show();
        this.$iconDown.hide();
    }

    public directHide() {
        this.hide(false);
        $("#project-resource-version-button").hide();
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
    ResourceDiscussion = 102,
    ResourceMetadata = 103,
}

interface IProjectResource {
    id: number;
    name: string;
}