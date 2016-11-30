$(document).ready(() => {
    var projectModule = new ProjectModule();
    projectModule.init();
});

class ProjectModule {
    private currentModule: ProjectModuleBase;
    private addResourceDialog: BootstrapDialogWrapper;

    constructor() {
        this.currentModule = null;
        this.addResourceDialog = new BootstrapDialogWrapper({
            element: $("#new-resource-dialog"),
            autoClearInputs: true
        });
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

        $("#add-resource-button").click(() => {
            this.addResourceDialog.show();
        });

        var dropzoneOptions = DropzoneHelper.getFullConfiguration({
            //url: getBaseUrl() + "Admin/Project/UploadResource"
        });
        $("#new-resource-upload").dropzone(dropzoneOptions);
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

    protected initTabs() {
        $(`#${this.getTabsId()} a`).click(function (e) {
            e.preventDefault();
            $(this).tab('show');
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
}

class ProjectWorkModule extends ProjectModuleBase {
    getModuleType(): ProjectModuleType { return ProjectModuleType.Work; }
    getTabsId(): string { return "project-work-tabs" }

    initModule(): void {}
}

class ProjectResourceModule extends ProjectModuleBase {
    getModuleType(): ProjectModuleType { return ProjectModuleType.Resource; }
    getTabsId(): string { return "project-resource-tabs" }

    initModule(): void {}
}

enum ProjectModuleType {
    Work = 0,
    Resource = 1,
}