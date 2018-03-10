class LayoutConfiguration {
    private configuration: GoldenLayout.Config;

    constructor() {
        this.configuration = this.createConfig();
    }

    public getConfiguration(): GoldenLayout.Config {
        return this.configuration;
    }

    private createConfig(): GoldenLayout.Config {
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
}