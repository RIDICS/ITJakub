class VariableInterpreter {
    private static _instance: VariableInterpreter = null;

    constructor() {
        if (VariableInterpreter._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        VariableInterpreter._instance = this;
    }

    public static getInstance(): VariableInterpreter {
        if (VariableInterpreter._instance === null) {
            VariableInterpreter._instance = new VariableInterpreter();
        }
        return VariableInterpreter._instance;
    }

    public interpret(valueString: string, variables: Object, bibItem: IBookInfo): string {
        if (typeof valueString === 'undefined' || typeof bibItem === 'undefined') {
            console.log("VariabeInterpreter: cannot interpret undefined type");
            return "";
        }
        return this.interpretPattern("bibItem", valueString, variables, bibItem, true, "");
    }

    private resolveScope(interpretedVariable: Object, scopedObject: Object): Object {
        var scope: string = interpretedVariable["scope"];
        var actualScopedObject: Object = scopedObject;
        if (typeof scope !== 'undefined') {
            if (scope === "{$parent}") {
                actualScopedObject = this.getParentScope(actualScopedObject);
            } else {
                actualScopedObject = this.changeToInnerScope(actualScopedObject, scope);
            }
        }
        return actualScopedObject;
    }

    private changeToInnerScope(actualScope: Object, newScopeName: string): Object {
        var newScope: Object = actualScope[newScopeName];
        this.setParentScope(newScope, actualScope);
        return newScope;
    }

    private changeToOuterScope(actualScope: Object): Object {
        return this.getParentScope(actualScope);
    }

    private setParentScope(scope: Object, parentScope: Object) {
        if (typeof scope === 'undefined' || scope === null) {
            console.log("cannot set parent scope to undefined or null scope");
            return;
        }
        scope['parentScope'] = parentScope;
    }

    private getParentScope(scope: Object): Object {
        var parentScope = scope['parentScope'];
        if (typeof parentScope !== 'undefined') {
            return parentScope;
        }
        return scope;
    }

    private interpretPattern(interpretedVariableName: string, pattern: string, variables: Object, actualScopeObject: Object, continueOnNullValue: boolean, replacementForNullValue: string): string {
        if (typeof actualScopeObject === 'undefined') {
            console.error("'" + interpretedVariableName + "' has undefined scoped");
            return "";
        }
        var foundNullValue: boolean;
        var interpretedpattern = pattern.replace(/{(.+?)}/g, (foundPattern, varName: string) => {
            if (!continueOnNullValue && foundNullValue) return "";
            var result: string;
            if (varName.indexOf("$") === 0) { //is config variable
                if (varName === "$this") {
                    result = <string>actualScopeObject; //if config variable is this, return this as value (can be used for primitive types like array of numbers or strings)
                } else {
                    result = this.interpretConfigurationVariable(varName, variables, actualScopeObject);
                }
            } else if (varName.indexOf("@") === 0) {
                result = localization.translate(varName.substring(1), "BibliographyModule").value;
            }
            else {
                result = actualScopeObject[varName];
                var tmpScope = actualScopeObject;
                while (typeof result === 'undefined' && this.getParentScope(tmpScope) !== tmpScope) { //if result is undefined bubble up to outer scope 
                    tmpScope = this.getParentScope(tmpScope);
                    result = tmpScope[varName];
                }
            }

            if (typeof result !== 'undefined' && result !== null) {
                result = String(result); //convert if typeof result was not string    
                if (result.length > 0) {
                    return result;
                }
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

    private interpretConfigurationVariable(varName: string, variables: Object, actualScopeObject: Object): string {
        if (typeof variables === 'undefined') {
            console.error("No variables are specified in bibliography configuration");
            return "";
        }
        if (typeof actualScopeObject === 'undefined') {
            console.log("Variable '" + varName + "' has null scope object");
            return "";
        }

        var interpretedVariable = variables[varName];
        if (typeof interpretedVariable === 'undefined') {
            console.error("Variable with name " + varName + " is not specidfied in bibliography configuration");
            return "";
        }

        var typeOfVariable = interpretedVariable['type'];
        if (typeof typeOfVariable === "undefined") {
            console.error("Variable with name " + varName + " does not have specified type");
            return "";
        }

        switch (typeOfVariable) {
        case "basic":
            return this.interpretBasic(varName, interpretedVariable, variables, actualScopeObject);
        case "replace":
            return this.interpretReplace(varName, interpretedVariable, variables, actualScopeObject);
        case "if":
            return this.interpretIfStatement(varName, interpretedVariable, variables, actualScopeObject);
        case "array":
            return this.interpretArray(varName, interpretedVariable, variables, actualScopeObject);
        case "table":
            return this.interpretTable(varName, interpretedVariable, variables, actualScopeObject);
        case "script":
            return this.interpretScript(varName, interpretedVariable, variables, actualScopeObject);
        default:
            console.error("Variable with name " + varName + " does not have correct type");
            return "";
        }
    }

    private interpretBasic(varName: string, interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var printIfNull: boolean = interpretedVariable["printIfNullValue"];
        var replacementForNullValue: string = localization.translate(interpretedVariable["replaceNullValueBy"], "BibliographyModule").value;
        var pattern: string = interpretedVariable["pattern"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        var value: string = this.interpretPattern(varName, pattern, variables, actualScopedObject, printIfNull, replacementForNullValue);
        if ((value === null || value.length <= 0) && !printIfNull) {
            return "";
        } else {
            return value;
        }
    }

    private interpretReplace(varName: string, interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var replacing: string = interpretedVariable["replacing"];
        var replacement: string = interpretedVariable["replacement"];
        var pattern: string = interpretedVariable["pattern"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        var value: string = this.interpretPattern(varName, pattern, variables, actualScopedObject, true, "");
        return value.replace(new RegExp(replacing, 'g'), replacement);
    }

    private interpretIfStatement(varName: string, interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var pattern: string = interpretedVariable["pattern"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        var truePattern: string = interpretedVariable["onTrue"];
        var falsePattern: string = interpretedVariable["onFalse"];
        var patternResult = this.interpretPattern(varName, pattern, variables, actualScopedObject, true, "");
        if (typeof patternResult === "undefined" || patternResult === null || patternResult === "" || patternResult === "false" || patternResult === "0") {
            return this.interpretPattern(varName, falsePattern, variables, actualScopedObject, true, "");
        }
        return this.interpretPattern(varName, truePattern, variables, actualScopedObject, true, "");
    }

    private interpretArray(varName: string, interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var pattern: string = interpretedVariable["pattern"];
        var delimeter: string = interpretedVariable["delimeter"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        if (!$.isArray(actualScopedObject)) {
            console.error("Variable with name " + varName + " must be scoped on array.");
            return "";
        }
        var value: string = "";
        var actualScopedObjectList = actualScopedObject as Array<Object>;
        $.each(actualScopedObjectList, (index: number, item: Object) => {
            if (typeof item === 'undefined' || item === null) {
                return true; //continue
            }
            this.setParentScope(item, actualScopedObject);
            var itemValue = this.interpretPattern(varName, pattern, variables, item, true, "");
            if (index > 0) {
                itemValue = delimeter + itemValue;
            }
            value += itemValue;
        });

        return value;

    }

    private interpretTable(varName: string, interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var printRowIfNullValue: boolean = interpretedVariable["printRowIfNullValue"];
        var replaceNullValueBy: string = localization.translate(interpretedVariable["replaceNullValueBy"], "BibliographyModule").value;
        var rows: Array<Object> = interpretedVariable["rows"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        var tableBuilder = new TableBuilder();

        $.each(rows, (index, item) => {
            var label: string = localization.translate(item["label"], "BibliographyModule").value;
            var pattern: string = item["pattern"];
            var value: string = this.interpretPattern(varName, pattern, variables, actualScopedObject, true, "");
            if (typeof value !== 'undefined' && value !== null && value.length > 0) {
                tableBuilder.makeTableRow(label, value);
            } else {
                if (printRowIfNullValue) {
                    tableBuilder.makeTableRow(label, replaceNullValueBy);
                }
            }
        });

        return tableBuilder.build().outerHTML;
    }

    private interpretScript(varName: string, interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var printIfNull: boolean = interpretedVariable["printIfNullValue"];
        var replaceNullValueBy: string = localization.translate(interpretedVariable["replaceNullValueBy"], "BibliographyModule").value;
        var pattern: string = interpretedVariable["pattern"];
        var actualScopedObject = this.resolveScope(interpretedVariable, scopedObject);
        var interpretedScript = this.interpretPattern(varName, pattern, variables, actualScopedObject, false, "");
        var patternResult = eval(interpretedScript);
        if (typeof patternResult === "undefined" || patternResult === null || patternResult === "" || patternResult === "false" || patternResult === "0") {
            if (printIfNull) {
                if (typeof replaceNullValueBy !== "undefined" || replaceNullValueBy !== null) {
                    return replaceNullValueBy;
                } else {
                    return patternResult;
                }

            } else {
                return "";
            }
        }

        return patternResult;
    }
}

class TableBuilder {
    private m_tableDiv: HTMLDivElement;

    constructor() {
        this.m_tableDiv = document.createElement('div');
        $(this.m_tableDiv).addClass('bib-table');
    }

    public makeTableRow(label: string, value: string): void {
        var rowDiv: HTMLDivElement = document.createElement('div');
        $(rowDiv).addClass('bib-table-row');
        var labelDiv: HTMLDivElement = document.createElement('div');
        $(labelDiv).addClass('bib-table-cell bib-label');
        labelDiv.innerHTML = label;
        rowDiv.appendChild(labelDiv);
        var valueDiv: HTMLDivElement = document.createElement('div');
        $(valueDiv).addClass('bib-table-cell');
        valueDiv.innerHTML = value;
        rowDiv.appendChild(valueDiv);
        this.m_tableDiv.appendChild(rowDiv);
    }

    public build(): HTMLDivElement {
        return this.m_tableDiv;
    }
}