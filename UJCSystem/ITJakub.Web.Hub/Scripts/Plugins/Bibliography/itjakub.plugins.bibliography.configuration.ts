class ConfigurationManager {
    booksConfigurations: Object;

    constructor(config: Object) {
        this.booksConfigurations = config; //TODO booksConfigurations should be just subset of config, here should be iteration over of config for bookTypes
    }

    getBookTypeConfigurations() {
        return this.booksConfigurations;
    }
}

class BookTypeConfiguration {
    bookType: BookTypeEnum;
    rightPanelConfig: RightPanelConfiguration;
    middlePanelConfig: MiddlePanelConfiguration;
    bottomPanelConfig: BottomPanelConfiguration;

    constructor(bookType: BookTypeEnum, bookTypeConfig: Object) {
        this.bookType = bookType;
        this.rightPanelConfig = new RightPanelConfiguration(bookTypeConfig['right-panel']);
        this.middlePanelConfig = new MiddlePanelConfiguration(bookTypeConfig['middle-panel']);
        this.bottomPanelConfig = new BottomPanelConfiguration(bookTypeConfig['bottom-panel']);
    }

    containsMiddlePanel() { return this.middlePanelConfig.exist(); }

    containsBottomPanel() { return this.bottomPanelConfig.exist(); }

    containsRightPanel() { return this.rightPanelConfig.exist(); }

    getMidllePanelConfig() { return this.middlePanelConfig; }

    getBottomPanelConfig() { return this.bottomPanelConfig; }

    getRightPanelConfig() { return this.rightPanelConfig; }

}

class Configuration {
    configObject: Object;

    constructor(configObj: Object) {
        this.configObject = configObj;
    }

    exist(): boolean {
        return typeof this.configObject !== 'undefined';
    }

    interpret(interpretedString: string, bibItem: IBookInfo ): string {
        return VariableInterpreter.getInstance().interpret(interpretedString, this.getVariables(), bibItem);
    }

    private getVariables() {
        return this.configObject['variables'];
    }
}

class RightPanelConfiguration extends Configuration {

    constructor(configObj: Object) {
        super(configObj);
    }

    containsInfoButton() { return typeof this.configObject['info-button'] !== 'undefined'; }
    containsReadButton() { return typeof this.configObject['read-button'] !== 'undefined'; }

    getInfoButtonUrl(bibItem: IBookInfo): string { return this.interpret(getBaseUrl()+this.configObject['info-button']['url'], bibItem); }
    getInfoButtonOnClick(bibItem: IBookInfo): string { return this.interpret(this.configObject['info-button']['onclick'], bibItem); }
    getReadButtonUrl(bibItem: IBookInfo): string { return this.interpret(getBaseUrl()+this.configObject["read-button"]["url"], bibItem); }
    getReadButtonOnClick(bibItem: IBookInfo): string { return this.interpret(this.configObject["read-button"]["onclick"], bibItem); }


}

class MiddlePanelConfiguration extends Configuration {

    constructor(configObj: Object) {
        super(configObj);
    }

    containsBody() { return typeof this.configObject['body'] !== 'undefined'; }
    containsTitle() { return typeof this.configObject['title'] !== 'undefined'; }
    containsShortInfo() { return typeof this.configObject['short-info'] !== 'undefined'; }
    containsCustom() { return typeof this.configObject['custom'] !== 'undefined'; }

    getCustom(bibItem: IBookInfo): string { return this.interpret(this.configObject['custom'], bibItem); }
    getTitle(bibItem: IBookInfo): string { return this.interpret(this.configObject['title'], bibItem); }
    getShortInfo(bibItem: IBookInfo): string { return this.interpret(this.configObject['short-info'], bibItem); }
    getBody(bibItem: IBookInfo): string { return this.interpret(this.configObject['body'], bibItem); }
}

class BottomPanelConfiguration extends Configuration {

    constructor(configObj: Object) {
        super(configObj);
    }

    containsBody() { return typeof this.configObject['body'] !== 'undefined'; }
    containsCustom() { return typeof this.configObject['custom'] !== 'undefined'; }

    getCustom(bibItem: IBookInfo): string { return this.interpret(this.configObject['custom'], bibItem); }
    getBody(bibItem: IBookInfo): string { return this.interpret(this.configObject['body'], bibItem); }

}

