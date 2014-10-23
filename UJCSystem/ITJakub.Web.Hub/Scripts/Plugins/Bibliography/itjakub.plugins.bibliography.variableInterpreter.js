var VariableInterpreter = (function () {
    function VariableInterpreter() {
    }
    VariableInterpreter.prototype.interpret = function (valueString, variables, bibItem) {
        if (typeof valueString === 'undefined' || typeof bibItem === 'undefined') {
            console.log("VariabeInterpreter: cannot interpret undefined type");
            return "";
        }
        return this.interpretPattern(valueString, variables, bibItem, true, "");
    };

    VariableInterpreter.prototype.resolveScope = function (interpretedVariable, scopedObject) {
        var scope = interpretedVariable["scope"];
        var actualScopedObject = scopedObject;
        if (typeof scope !== 'undefined') {
            if (scope === "{$parent}") {
                actualScopedObject = this.getParentScope(actualScopedObject);
            } else {
                actualScopedObject = this.changeToInnerScope(actualScopedObject, scope);
            }
        }
        return actualScopedObject;
    };

    VariableInterpreter.prototype.changeToInnerScope = function (actualScope, newScopeName) {
        var newScope = actualScope[newScopeName];
        this.setParentScope(newScope, actualScope);
        return newScope;
    };

    VariableInterpreter.prototype.changeToOuterScope = function (actualScope) {
        return this.getParentScope(actualScope);
    };

    VariableInterpreter.prototype.setParentScope = function (scope, parentScope) {
        scope['parentScope'] = parentScope;
    };

    VariableInterpreter.prototype.getParentScope = function (scope) {
        var parentScope = scope['parentScope'];
        if (typeof parentScope !== 'undefined') {
            return parentScope;
        }
        return scope;
    };

    VariableInterpreter.prototype.interpretPattern = function (pattern, variables, actualScopeObject, continueOnNullValue, replacementForNullValue) {
        var _this = this;
        var foundNullValue;
        var interpretedpattern = pattern.replace(/{(.+?)}/g, function (foundPattern, varName) {
            if (!continueOnNullValue && foundNullValue)
                return "";
            var result;
            if (varName.indexOf("$") === 0) {
                if (varName === "$this") {
                    result = actualScopeObject; //if config variable is this, return this as value (can be used for primitive types like array of numbers or strings)
                } else {
                    result = _this.interpretConfigurationVariable(varName, variables, actualScopeObject);
                }
            } else {
                result = actualScopeObject[varName];
                var tmpScope = actualScopeObject;
                while (typeof result === 'undefined' && _this.getParentScope(tmpScope) !== tmpScope) {
                    tmpScope = _this.getParentScope(tmpScope);
                    result = tmpScope[varName];
                }
            }

            if (typeof result !== 'undefined' && result !== null && result.length > 0) {
                return result;
            }

            foundNullValue = true;
            if (!continueOnNullValue)
                return "";
            return replacementForNullValue;
        });
        if (foundNullValue && !continueOnNullValue) {
            return null;
        }
        return interpretedpattern;
    };

    VariableInterpreter.prototype.interpretConfigurationVariable = function (varName, variables, actualScopeObject) {
        if (variables === 'undefined') {
            console.error("No variables are specified in bibliography configuration");
            return "";
        }

        var interpretedVariable = variables[varName];
        if (interpretedVariable === 'undefined') {
            console.error("Variable with name " + varName + " is not specidfied in bibliography configuration");
            return "";
        }

        var typeOfVariable = interpretedVariable['type'];
        if (typeOfVariable === "undefined") {
            console.error("Variable with name " + varName + " does not have specified type");
            return "";
        }

        switch (typeOfVariable) {
            case "basic":
                return this.interpretBasic(interpretedVariable, variables, actualScopeObject);
            case "array":
                return this.interpretArray(interpretedVariable, variables, actualScopeObject);
            case "table":
                return this.interpretTable(interpretedVariable, variables, actualScopeObject);
            default:
                console.error("Variable with name " + varName + " does not have correct type");
                return "";
        }
    };

    VariableInterpreter.prototype.interpretBasic = function (interpretedVariable, variables, scopedObject) {
        var printIfNull = interpretedVariable["printIfNullValue"];
        var replacementForNullValue = interpretedVariable["replaceNullValueBy"];
        var pattern = interpretedVariable["pattern"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        var value = this.interpretPattern(pattern, variables, actualScopedObject, printIfNull, replacementForNullValue);
        if ((value === null || value.length <= 0) && !printIfNull) {
            return "";
        } else {
            return value;
        }
    };

    VariableInterpreter.prototype.interpretArray = function (interpretedVariable, variables, scopedObject) {
        var _this = this;
        var pattern = interpretedVariable["pattern"];
        var delimeter = interpretedVariable["delimeter"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        var value = "";
        $.each(actualScopedObject, function (index, item) {
            _this.setParentScope(item, actualScopedObject);
            var itemValue = _this.interpretPattern(pattern, variables, item, true, "");
            if (index > 0) {
                itemValue = delimeter + itemValue;
            }
            value += itemValue;
        });

        return value;
    };

    VariableInterpreter.prototype.interpretTable = function (interpretedVariable, variables, scopedObject) {
        var _this = this;
        var printRowIfNullValue = interpretedVariable["printRowIfNullValue"];
        var replaceNullValueBy = interpretedVariable["replaceNullValueBy"];
        var rows = interpretedVariable["rows"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        var tableBuilder = new TableBuilder();

        $.each(rows, function (index, item) {
            var label = item["label"];
            var pattern = item["pattern"];
            var value = _this.interpretPattern(pattern, variables, actualScopedObject, true, "");
            if (typeof value !== 'undefined' && value !== null && value.length > 0) {
                tableBuilder.makeTableRow(label, value);
            } else {
                if (printRowIfNullValue) {
                    tableBuilder.makeTableRow(label, replaceNullValueBy);
                }
            }
        });

        return tableBuilder.build().outerHTML;
    };
    return VariableInterpreter;
})();

var TableBuilder = (function () {
    function TableBuilder() {
        this.m_tableDiv = document.createElement('div');
        $(this.m_tableDiv).addClass('table');
    }
    TableBuilder.prototype.makeTableRow = function (label, value) {
        var rowDiv = document.createElement('div');
        $(rowDiv).addClass('row');
        var labelDiv = document.createElement('div');
        $(labelDiv).addClass('cell label');
        labelDiv.innerHTML = label;
        rowDiv.appendChild(labelDiv);
        var valueDiv = document.createElement('div');
        $(valueDiv).addClass('cell');
        valueDiv.innerHTML = value;
        rowDiv.appendChild(valueDiv);
        this.m_tableDiv.appendChild(rowDiv);
    };

    TableBuilder.prototype.build = function () {
        return this.m_tableDiv;
    };
    return TableBuilder;
})();
//# sourceMappingURL=itjakub.plugins.bibliography.variableInterpreter.js.map
