$(document).ready(() => {
    var projectModule = new ProjectModule();
    projectModule.init();
});

class ProjectModule {
    private currentModule: ProjectModuleBase;
    
    constructor() {
        this.currentModule = null;
    }

    public init() {
        var self = this;
        var $splitterButton = $("#splitter-button");
        $splitterButton.click(() => {
            var $leftMenu = $("#left-menu");
            if ($leftMenu.is(":visible")) {
                $leftMenu.hide("slide", { direction: "left" });
                $splitterButton.text(">");
            } else {
                $leftMenu.show("slide", { direction: "left" });
                $splitterButton.text("<");
            }
        });

        var $projectNavigationLinks = $("#project-navigation a");
        $projectNavigationLinks.click(function (e) {
            e.preventDefault();
            $projectNavigationLinks.removeClass("active");
            $(this).addClass("active");
            self.showModule(e.currentTarget.id);
        });

        
        var dropzoneOptions = DropzoneHelper.getFullConfiguration({
            //url: getBaseUrl() + "Admin/Project/UploadResource"
        });
        $("#new-resource-upload").dropzone(dropzoneOptions);
        $("#new-resource-version-upload").dropzone(dropzoneOptions);
    }
    
    public showModule(identificator: string) {
        switch (identificator) {
            case "project-navigation-root":
                this.currentModule = new ProjectWorkModule();
                break;
            case "project-navigation-image":
                this.currentModule = new ProjectResourceModule();
                break;
            case "project-navigation-text":
                this.currentModule = new ProjectResourceModule();
                break;
            case "project-navigation-audio":
                this.currentModule = new ProjectResourceModule();
                break;
            case "project-navigation-video":
                this.currentModule = new ProjectResourceModule();
                break;
            default:
                this.currentModule = new ProjectWorkModule();
        }
        this.currentModule.init();
    }
}

abstract class ProjectModuleBase {
    protected moduleTab: ProjectModuleTabBase;

    public abstract getModuleType(): ProjectModuleType;
    public abstract getTabsId(): string;
    public abstract initModule(): void;
    public abstract getLoadTabPanelContentUrl(panelSelector: string): string;
    public abstract makeProjectModuleTab(panelSelector: string): ProjectModuleTabBase;

    protected initTabs() {
        var self = this;
        $(`#${this.getTabsId()} a`).click(function (e) {
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
            .load(url, null, (responseText, textStatus, xmlHttpRequest) => {
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
        var $tabPanel = $(tabPanelSelector);
        var url = this.getLoadTabPanelContentUrl(tabPanelSelector);
        $tabPanel
            .html("<div class=\"loader\"></div>")
            .load(url, null, (responseText, textStatus, xmlHttpRequest) => {
                if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                    var errorDiv = new AlertComponentBuilder(AlertType.Error).addContent("Chyba při načítání záložky.")
                        .buildElement();
                    $tabPanel.empty().append(errorDiv);
                    this.moduleTab = null;
                    return;
                }

                this.moduleTab = this.makeProjectModuleTab(tabPanelSelector);
                if (this.moduleTab != null) {
                    this.moduleTab.initTab();
                }
            });
    }
}

abstract class ProjectModuleTabBase {
    public abstract initTab();
}

class ProjectWorkModule extends ProjectModuleBase {
    getModuleType(): ProjectModuleType { return ProjectModuleType.Work; }
    getTabsId(): string { return "project-work-tabs" }

    initModule(): void {
        $("#resource-panel").hide();
    }

    getLoadTabPanelContentUrl(panelSelector: string): string {
        var tabPanelType = <ProjectModuleTabType>$(panelSelector).data("panel-type");
        return getBaseUrl() + "Admin/Project/ProjectModuleTab?tabType=" + tabPanelType;
    }

    makeProjectModuleTab(panelSelector: string): ProjectModuleTabBase {
        switch (panelSelector) {
            case "#project-work-metadata":
                return new ProjectWorkMetadataTab();
            case "#project-work-page-list":
                return new ProjectWorkPageListTab();
            default:
                return null;
        }
    }
}

class ProjectResourceModule extends ProjectModuleBase {
    private addResourceDialog: BootstrapDialogWrapper;
    private createResourceVersionDialog: BootstrapDialogWrapper;
    private deleteResourceDialog: BootstrapDialogWrapper;
    private renameResourceDialog: BootstrapDialogWrapper;
    private duplicateResourceDialog: BootstrapDialogWrapper;
    private resourceVersionModule: ProjectResourceVersionModule;

    constructor() {
        super();
        this.resourceVersionModule = new ProjectResourceVersionModule(0);
    }

    getModuleType(): ProjectModuleType { return ProjectModuleType.Resource; }
    getTabsId(): string { return "project-resource-tabs" }

    initModule(): void {
        $("#resource-panel").show();
        $("#resource-list").empty().off().append("<option>Mock 1</option><option>Mock 2</option>");

        var $selectionDependentButtons = $("#delete-resource-button, #rename-resource-button, #duplicate-resource-button, #create-resource-version-button");
        $selectionDependentButtons.prop("disabled", true);

        $("#resource-list").change(function() {
            var resourceList = <HTMLSelectElement>this;
            console.log(resourceList.selectedIndex);

            $selectionDependentButtons.prop("disabled", resourceList.selectedIndex < 0);
        });

        this.initDialogs();
        this.initMainResourceButtons();
        this.resourceVersionModule.init();
    }
    
    getLoadTabPanelContentUrl(panelSelector: string): string {
        var tabPanelType = <ProjectModuleTabType>$(panelSelector).data("panel-type");
        return getBaseUrl() + "Admin/Project/ProjectModuleTab?tabType=" + tabPanelType;
    }

    makeProjectModuleTab(panelSelector: string): ProjectModuleTabBase {
        switch (panelSelector) {
            case "#project-resource-metadata":
                return new ProjectResourceMetadataTab();
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
            autoClearInputs: true
        });
        this.createResourceVersionDialog = new BootstrapDialogWrapper({
            element: $("#new-resource-version-dialog"),
            autoClearInputs: true
        });
        this.renameResourceDialog = new BootstrapDialogWrapper({
            element: $("#rename-resource-dialog"),
            autoClearInputs: true
        });
        this.deleteResourceDialog = new BootstrapDialogWrapper({
            element: $("#delete-resource-dialog"),
            autoClearInputs: false
        });
        this.duplicateResourceDialog = new BootstrapDialogWrapper({
            element: $("#duplicate-resource-dialog"),
            autoClearInputs: false
        });
    }

    private initMainResourceButtons() {
        $("#resource-panel button").off();

        $("#add-resource-button").click(() => {
            this.addResourceDialog.show();
        });
        $("#create-resource-version-button").click(() => {
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
}

class ProjectResourceVersionModule {
    private resourceId: number;
    private $iconUp: JQuery;
    private $iconDown: JQuery;
    private versionPanelHeight: number;

    constructor(resourceId: number) {
        this.resourceId = resourceId;
    }

    public init() {
        this.$iconUp = $("#project-resource-version-icon-up");
        this.$iconDown = $("#project-resource-version-icon-down");
        this.$iconDown.hide();
        $("#project-resource-version-button").click(() => {
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

        var url = getBaseUrl() + "Admin/Project/ProjectResourceVersion";
        $resourceVersionPanel
            .slideToggle()
            .append("<div class=\"loading\"></div>")
            .load(url, null, (responseText, textStatus, xmlHttpRequest) => {
                if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                    var error = new AlertComponentBuilder(AlertType.Error).addContent("Chyba při načítání seznamu verzí");
                    $resourceVersionPanel.empty().append(error.buildElement());
                }
            });
        
        $resourceTabContent.animate({
            height: "-=" + this.versionPanelHeight + "px"
        });


        this.$iconUp.hide();
        this.$iconDown.show();
    }

    private hide() {
        var $resourceVersionPanel = $("#resource-version-panel");
        var $resourceTabContent = $("#resource-tab-content");
        
        $resourceVersionPanel.slideToggle(null, () => $resourceVersionPanel.empty());

        $resourceTabContent.animate({
            height: "+=" + this.versionPanelHeight + "px"
        });

        this.$iconUp.show();
        this.$iconDown.hide();
    }
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
        var config = this.getConfiguration();
        var $inputs = $("input", config.$panel);

        config.$viewButtonPanel.hide();
        config.$editorButtonPanel.show();
        $inputs.removeClass("input-as-text")
            .prop("disabled", false);
    }

    protected disableEdit() {
        var config = this.getConfiguration();
        var $inputs = $("input", config.$panel);

        config.$viewButtonPanel.show();
        config.$editorButtonPanel.hide();
        $inputs.addClass("input-as-text")
            .prop("disabled", true);
    }
}

class ProjectWorkMetadataTab extends ProjectMetadataTabBase {
    getConfiguration(): IProjectMetadataTabConfiguration {
        return {
            $panel: $("#project-work-metadata"),
            $viewButtonPanel: $("#work-metadata-view-button-panel"),
            $editorButtonPanel: $("#work-metadata-editor-button-panel")
        };
    }

    initTab(): void {
        super.initTab();

        $("#work-metadata-edit-button").click(() => {
            this.enabledEdit();
        });

        $("#work-metadata-cancel-button, #work-metadata-save-button").click(() => {
            this.disableEdit();
        });
    }
}

class ProjectResourceMetadataTab extends ProjectMetadataTabBase {
    getConfiguration(): IProjectMetadataTabConfiguration {
        return {
            $panel: $("#project-resource-metadata"),
            $viewButtonPanel: $("#resource-metadata-view-button-panel"),
            $editorButtonPanel: $("#resource-metadata-editor-button-panel")
        };
    }

    initTab(): void {
        super.initTab();

        $("#resource-metadata-edit-button").click(() => {
            this.enabledEdit();
        });

        $("#resource-metadata-cancel-button, #resource-metadata-save-button").click(() => {
            this.disableEdit();
        });
    }
}

class ProjectWorkPageListTab extends ProjectModuleTabBase {
    private editDialog: BootstrapDialogWrapper;

    initTab() {
        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-pages-dialog"),
            autoClearInputs: false
        });

        $("#project-pages-edit-button").click(() => {
            this.editDialog.show();
        });
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
    ResourcePreview = 101,
    ResourceDiscussion = 102,
    ResourceMetadata = 103,
}