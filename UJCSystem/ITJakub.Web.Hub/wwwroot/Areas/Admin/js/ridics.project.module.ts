﻿$(document).ready(() => {
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
            $leftMenu.fadeToggle(null, () => {
                var buttonContent = $leftMenu.is(":visible") ? "<" : ">";
                $splitterButton.text(buttonContent);
            });
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
    public abstract getModuleType(): ProjectModuleType;
    public abstract getTabsId(): string;
    public abstract initModule(): void;
    public abstract getLoadTabPanelContentUrl(panelSelector: string): string;

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
                }
            });
    }
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
}

class ProjectResourceModule extends ProjectModuleBase {
    private addResourceDialog: BootstrapDialogWrapper;
    private createResourceVersionDialog: BootstrapDialogWrapper;
    private deleteResourceDialog: BootstrapDialogWrapper;
    private renameResourceDialog: BootstrapDialogWrapper;
    private duplicateResourceDialog: BootstrapDialogWrapper;

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

        var $iconUp = $("#project-resource-version-icon-up");
        var $iconDown = $("#project-resource-version-icon-down");
        $iconDown.hide();
        $("#project-resource-version-button").click(() => {
            if ($iconUp.is(":visible")) {
                $iconUp.hide();
                $iconDown.show();
            } else {
                $iconUp.show();
                $iconDown.hide();
            }
        });

        this.initDialogs();
        this.initMainResourceButtons();
    }
    
    getLoadTabPanelContentUrl(panelSelector: string): string {
        var tabPanelType = <ProjectModuleTabType>$(panelSelector).data("panel-type");
        return getBaseUrl() + "Admin/Project/ProjectModuleTab?tabType=" + tabPanelType;
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