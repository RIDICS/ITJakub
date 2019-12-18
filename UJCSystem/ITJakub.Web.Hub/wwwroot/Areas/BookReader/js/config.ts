class LayoutConfiguration {

    public getLayoutConfig(): ILayoutConfig[] {
        return [
            {
                minWidth: 0,
                maxWidth: 800,
                deviceType: Device.Mobile,
                goldenLayoutConfig: this.goldenLayoutMobileConfig()
            }, {
                minWidth: 801,
                maxWidth: Number.POSITIVE_INFINITY,
                deviceType: Device.Desktop,
                goldenLayoutConfig: this.goldenLayoutDesktopConfig()
            }
        ];
    }

    private goldenLayoutDesktopConfig(): GoldenLayout.Config {
        var layoutConfig = {
            dimensions: {
                headerHeight: 26,
                minItemWidth: 265
            },
            labels: {
                close: localization.translate("close", "BookReader").value,
                maximise: localization.translate("maximise", "BookReader").value,
                minimise: localization.translate("minimise", "BookReader").value,
                popout: localization.translate("popout", "BookReader").value
            },
            content: [{
                type: "row",
                isClosable: false,
                content: [{
                    type: "column",
                    id: "views"
                }]
            }]
        }
        return layoutConfig;
    }

    private goldenLayoutMobileConfig(): GoldenLayout.Config {
        var layoutConfig = {
            settings: {
                showPopoutIcon: false,
                showMaximiseIcon: false,
                reorderEnabled: false
            },
            dimensions: {
                headerHeight: 26,
                minItemWidth: 200
            },
            labels: {
                close: localization.translate("close", "BookReader").value,
                maximise: localization.translate("maximise", "BookReader").value,
                minimise: localization.translate("minimise", "BookReader").value,
                popout: localization.translate("popout", "BookReader").value
            },
            content: [
                
                    
            ]
        }
        return layoutConfig;
    }

    public viewPanelConfig(type: PanelType, id: string, title: string): GoldenLayout.ItemConfig {
        var itemConfig = {
            type: type,
            componentState: { label: id },
            id: id,
            componentName: 'viewTab',
            title: title
        };
        return itemConfig;
    }

    public toolPanelConfig(type: PanelType, id: string, title: string): GoldenLayout.ItemConfig {
        var itemConfig = {
            type: type,
            componentState: { label: id },
            id: id,
            componentName: 'toolTab',
            title: title
        };
        return itemConfig;
    }
}

enum PanelType {
    Column = "column",
    Row = "row",
    Stack = "stack",
    Component = "component"
}

interface ILayoutConfig {
    minWidth: number;
    maxWidth: number;
    deviceType: Device;
    goldenLayoutConfig: GoldenLayout.Config;
}