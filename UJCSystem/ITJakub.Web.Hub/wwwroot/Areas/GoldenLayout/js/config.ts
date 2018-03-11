class LayoutConfiguration {

    public goldenLayoutDesktopConfig(): GoldenLayout.Config {
        var layoutConfig = {
            settings: {
                showPopoutIcon: false
            },
            dimensions: {
                headerHeight: 26,
                minItemWidth: 200
            },
            content: [{
                type: "row",
                isClosable: false,
                content: [{
                    type: "column",
                    id: "views",
                    isClosable: false,
                    content: [{
                        type: "row",
                        id: "viewsRow",
                        isClosable: false
                    }]
                }]
            }]
        }
        return layoutConfig;
    }

    public goldenLayoutMobileConfig(): GoldenLayout.Config {
        var layoutConfig = {
            settings: {
                showPopoutIcon: false,
                reorderEnabled: false
            },
            dimensions: {
                headerHeight: 26,
                minItemWidth: 200
            }
        }
        return layoutConfig;
    }

    public viewPanelConfig(closable: boolean, type: PanelType, id: string, title: string): GoldenLayout.ItemConfig {
        var itemConfig = {
            isClosable: closable,
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