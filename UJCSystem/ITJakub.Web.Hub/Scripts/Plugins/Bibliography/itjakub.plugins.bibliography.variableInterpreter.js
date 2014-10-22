var VariableInterpreter = (function () {
    function VariableInterpreter() {
    }
    VariableInterpreter.prototype.interpret = function (valueString, variables, bibItem) {
        return this.interpretPattern(valueString, variables, bibItem, true, "");
        //return valueString.replace(/{(.+?)}/g, (foundPattern, varName: string) => {
        //    if (varName.indexOf("$") === 0) { //is config variable
        //        return this.interpretConfigurationVariable(varName, variables, bibItem);
        //    } else {
        //        return bibItem[varName];
        //    }
        //});
    };

    VariableInterpreter.prototype.interpretPattern = function (pattern, variables, bibItem, continueOnNullValue, replacementForNullValue) {
        var _this = this;
        var foundNullValue;
        var interpretedpattern = pattern.replace(/{(.+?)}/g, function (foundPattern, varName) {
            if (!continueOnNullValue && foundNullValue)
                return "";
            var result;
            if (varName.indexOf("$") === 0) {
                result = _this.interpretConfigurationVariable(varName, variables, bibItem);
            } else {
                result = bibItem[varName];
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

    VariableInterpreter.prototype.interpretConfigurationVariable = function (varName, variables, bookInfo) {
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
                return this.interpretPrimitive(interpretedVariable, variables, bookInfo);
            case "array":
                return this.interpretArray(interpretedVariable, variables, bookInfo);
            case "table":
                return this.interpretTable(interpretedVariable, variables, bookInfo);
            default:
                console.error("Variable with name " + varName + " does not have correct type");
                return "";
        }
    };

    VariableInterpreter.prototype.interpretPrimitive = function (interpretedVariable, variables, bookInfo) {
        var printIfNull = interpretedVariable["printIfNullValue"];
        var replacementForNullValue = interpretedVariable["replaceNullValueBy"];
        var pattern = interpretedVariable["pattern"];
        var value = this.interpretPattern(pattern, variables, bookInfo, printIfNull, replacementForNullValue);
        if ((value === null || value.length <= 0) && !printIfNull) {
            return "";
        } else {
            return value;
        }
    };

    VariableInterpreter.prototype.interpretArray = function (interpretedVariable, variables, bookInfo) {
        var pattern = interpretedVariable["pattern"];
        var delimeter = interpretedVariable["delimeter"];
    };

    VariableInterpreter.prototype.interpretTable = function (interpretedVariable, variables, bookInfo) {
        throw new Error("Not implemented");
    };
    return VariableInterpreter;
})();
//# sourceMappingURL=itjakub.plugins.bibliography.variableInterpreter.js.map
