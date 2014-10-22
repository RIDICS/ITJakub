
class VariableInterpreter {


    public interpret(valueString: string, variables: Object, bibItem: IBookInfo): string {
        return this.interpretPattern(valueString, variables, bibItem, true, "");
        //return valueString.replace(/{(.+?)}/g, (foundPattern, varName: string) => {
        //    if (varName.indexOf("$") === 0) { //is config variable
        //        return this.interpretConfigurationVariable(varName, variables, bibItem);
        //    } else {
        //        return bibItem[varName];
        //    }
        //});
    }

    private interpretPattern(pattern: string, variables: Object, bibItem: IBookInfo, continueOnNullValue: boolean, replacementForNullValue: string): string {
        var foundNullValue: boolean;
        var interpretedpattern = pattern.replace(/{(.+?)}/g, (foundPattern, varName: string) => {
            if (!continueOnNullValue && foundNullValue) return "";
            var result: string;
            if (varName.indexOf("$") === 0) { //is config variable
                result = this.interpretConfigurationVariable(varName, variables, bibItem);
            } else {
                result = bibItem[varName];
            }

            if (typeof result !== 'undefined' && result !==null && result.length > 0) {
                return result;
            }

            foundNullValue = true;
            if (!continueOnNullValue) return "";
            return replacementForNullValue;
        });
        if (foundNullValue && !continueOnNullValue) {
            return null;
        }
        return interpretedpattern;
    }

    private interpretConfigurationVariable(varName: string, variables: Object, bookInfo: IBookInfo): string {
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
    }

    interpretPrimitive(interpretedVariable: Object, variables: Object, bookInfo: IBookInfo): string {
        var printIfNull: boolean = interpretedVariable["printIfNullValue"];
        var replacementForNullValue = interpretedVariable["replaceNullValueBy"];
        var pattern = interpretedVariable["pattern"];
        var value: string = this.interpretPattern(pattern, variables, bookInfo, printIfNull,replacementForNullValue);
        if ( (value === null || value.length <= 0) && !printIfNull) {
            return "";
        } else {
            return value;
        }
    }

    interpretArray(interpretedVariable: Object, variables: Object, bookInfo: IBookInfo): string {
        var pattern = interpretedVariable["pattern"];
        var delimeter = interpretedVariable["delimeter"];

    }

    interpretTable(interpretedVariable: Object, variables: Object, bookInfo: IBookInfo): string {
        throw new Error("Not implemented");

    }
}