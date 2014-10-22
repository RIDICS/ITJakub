var VariableInterpreter = (function () {
    function VariableInterpreter() {
    }
    VariableInterpreter.prototype.interpret = function (valueString, variables, bibItem) {
        return this.interpretPattern(valueString, variables, bibItem, true, "");
    };

    VariableInterpreter.prototype.interpretPattern = function (pattern, variables, actualScopeObject, continueOnNullValue, replacementForNullValue) {
        var _this = this;
        var foundNullValue;
        var interpretedpattern = pattern.replace(/{(.+?)}/g, function (foundPattern, varName) {
            if (!continueOnNullValue && foundNullValue)
                return "";
            var result;
            if (varName.indexOf("$") === 0) {
                result = _this.interpretConfigurationVariable(varName, variables, actualScopeObject);
            } else {
                result = actualScopeObject[varName];
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
            case "primitive":
                return this.interpretPrimitive(interpretedVariable, variables, actualScopeObject);
            case "array":
                return this.interpretArray(interpretedVariable, variables, actualScopeObject);
            case "table":
                return this.interpretTable(interpretedVariable, variables, actualScopeObject);
            default:
                console.error("Variable with name " + varName + " does not have correct type");
                return "";
        }
    };

    VariableInterpreter.prototype.interpretPrimitive = function (interpretedVariable, variables, scopedObject) {
        var printIfNull = interpretedVariable["printIfNullValue"];
        var replacementForNullValue = interpretedVariable["replaceNullValueBy"];
        var pattern = interpretedVariable["pattern"];
        var scope = interpretedVariable["scope"];
        var actualScopedObject = scopedObject;
        if (typeof scope !== 'undefined') {
            actualScopedObject = actualScopedObject[scope];
        }
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
        var scope = interpretedVariable["scope"];
        var actualScopedObject = scopedObject;
        if (typeof scope !== 'undefined') {
            actualScopedObject = actualScopedObject[scope];
        }
        var value = "";
        $.each(actualScopedObject, function (index, item) {
            value += _this.interpretPattern(pattern, variables, item, true, "");
            if (index > 0) {
                value = delimeter + value;
            }
        });

        return value;
    };

    VariableInterpreter.prototype.interpretTable = function (interpretedVariable, variables, scopedObject) {
        var _this = this;
        var printRowIfNullValue = interpretedVariable["printRowIfNullValue"];
        var replaceNullValueBy = interpretedVariable["replaceNullValueBy"];
        var rows = interpretedVariable["rows"];
        var scope = interpretedVariable["scope"];
        var actualScopedObject = scopedObject;
        if (typeof scope !== 'undefined') {
            actualScopedObject = actualScopedObject[scope];
        }
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

        return tableBuilder.build();
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
