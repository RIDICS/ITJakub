var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ConfigurationManager = (function () {
    function ConfigurationManager(config) {
        this.booksConfigurations = config; //TODO booksConfigurations should be just subset of config, here should be iteration over of config for bookTypes
    }
    ConfigurationManager.prototype.getBookTypeConfigurations = function () {
        return this.booksConfigurations;
    };
    return ConfigurationManager;
})();
var BookTypeConfiguration = (function () {
    function BookTypeConfiguration(bookType, bookTypeConfig) {
        this.bookType = bookType;
        this.rightPanelConfig = new RightPanelConfiguration(bookTypeConfig['right-panel']);
        this.middlePanelConfig = new MiddlePanelConfiguration(bookTypeConfig['middle-panel']);
        this.bottomPanelConfig = new BottomPanelConfiguration(bookTypeConfig['bottom-panel']);
    }
    BookTypeConfiguration.prototype.containsMiddlePanel = function () {
        return this.middlePanelConfig.exist();
    };
    BookTypeConfiguration.prototype.containsBottomPanel = function () {
        return this.bottomPanelConfig.exist();
    };
    BookTypeConfiguration.prototype.containsRightPanel = function () {
        return this.rightPanelConfig.exist();
    };
    BookTypeConfiguration.prototype.getMidllePanelConfig = function () {
        return this.middlePanelConfig;
    };
    BookTypeConfiguration.prototype.getBottomPanelConfig = function () {
        return this.bottomPanelConfig;
    };
    BookTypeConfiguration.prototype.getRightPanelConfig = function () {
        return this.rightPanelConfig;
    };
    return BookTypeConfiguration;
})();
var Configuration = (function () {
    function Configuration(configObj) {
        this.configObject = configObj;
    }
    Configuration.prototype.exist = function () {
        return typeof this.configObject !== 'undefined';
    };
    Configuration.prototype.interpret = function (interpretedString, bibItem) {
        return VariableInterpreter.getInstance().interpret(interpretedString, this.getVariables(), bibItem);
    };
    Configuration.prototype.getVariables = function () {
        return this.configObject['variables'];
    };
    return Configuration;
})();
var RightPanelConfiguration = (function (_super) {
    __extends(RightPanelConfiguration, _super);
    function RightPanelConfiguration(configObj) {
        _super.call(this, configObj);
    }
    RightPanelConfiguration.prototype.containsInfoButton = function () {
        return typeof this.configObject['info-button'] !== 'undefined';
    };
    RightPanelConfiguration.prototype.containsReadButton = function () {
        return typeof this.configObject['read-button'] !== 'undefined';
    };
    RightPanelConfiguration.prototype.getInfoButton = function (bibItem) {
        return this.interpret(getBaseUrl() + this.configObject['info-button']['url'], bibItem);
    };
    RightPanelConfiguration.prototype.getReadButton = function (bibItem) {
        return this.interpret(getBaseUrl() + this.configObject["read-button"]["url"], bibItem);
    };
    return RightPanelConfiguration;
})(Configuration);
var MiddlePanelConfiguration = (function (_super) {
    __extends(MiddlePanelConfiguration, _super);
    function MiddlePanelConfiguration(configObj) {
        _super.call(this, configObj);
    }
    MiddlePanelConfiguration.prototype.containsBody = function () {
        return typeof this.configObject['body'] !== 'undefined';
    };
    MiddlePanelConfiguration.prototype.containsTitle = function () {
        return typeof this.configObject['title'] !== 'undefined';
    };
    MiddlePanelConfiguration.prototype.containsShortInfo = function () {
        return typeof this.configObject['short-info'] !== 'undefined';
    };
    MiddlePanelConfiguration.prototype.containsCustom = function () {
        return typeof this.configObject['custom'] !== 'undefined';
    };
    MiddlePanelConfiguration.prototype.getCustom = function (bibItem) {
        return this.interpret(this.configObject['custom'], bibItem);
    };
    MiddlePanelConfiguration.prototype.getTitle = function (bibItem) {
        return this.interpret(this.configObject['title'], bibItem);
    };
    MiddlePanelConfiguration.prototype.getShortInfo = function (bibItem) {
        return this.interpret(this.configObject['short-info'], bibItem);
    };
    MiddlePanelConfiguration.prototype.getBody = function (bibItem) {
        return this.interpret(this.configObject['body'], bibItem);
    };
    return MiddlePanelConfiguration;
})(Configuration);
var BottomPanelConfiguration = (function (_super) {
    __extends(BottomPanelConfiguration, _super);
    function BottomPanelConfiguration(configObj) {
        _super.call(this, configObj);
    }
    BottomPanelConfiguration.prototype.containsBody = function () {
        return typeof this.configObject['body'] !== 'undefined';
    };
    BottomPanelConfiguration.prototype.containsCustom = function () {
        return typeof this.configObject['custom'] !== 'undefined';
    };
    BottomPanelConfiguration.prototype.getCustom = function (bibItem) {
        return this.interpret(this.configObject['custom'], bibItem);
    };
    BottomPanelConfiguration.prototype.getBody = function (bibItem) {
        return this.interpret(this.configObject['body'], bibItem);
    };
    return BottomPanelConfiguration;
})(Configuration);
//# sourceMappingURL=itjakub.plugins.bibliography.configuration.js.map