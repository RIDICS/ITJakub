
class VariableInterpreter {


    public interpret(valueString: string, variables: Object, bibItem: IBookInfo): string {
        if (typeof valueString === 'undefined' || typeof bibItem === 'undefined') {
            console.log("VariabeInterpreter: cannot interpret undefined type");
            return "";
        }
        return this.interpretPattern(valueString, variables, bibItem, true, "");
    }

    private changeScope(actualScope: Object, newScopeName: string):Object {
        var newScope: Object = actualScope[newScopeName];
        this.setParentScope(newScope, actualScope);
        return newScope;
    }

    private setParentScope(scope: Object, parentScope: Object) {
        scope['parentScope'] = parentScope;
    }

    private getParentScope(scope:Object):Object {
        return scope['parentScope'];
    }

    private interpretPattern(pattern: string, variables: Object, actualScopeObject: Object, continueOnNullValue: boolean, replacementForNullValue: string): string {
        var foundNullValue: boolean;
        var interpretedpattern = pattern.replace(/{(.+?)}/g, (foundPattern, varName: string) => {
            if (!continueOnNullValue && foundNullValue) return "";
            var result: string;
            if (varName.indexOf("$") === 0) { //is config variable
                result = this.interpretConfigurationVariable(varName, variables, actualScopeObject);
            } else {
                result = actualScopeObject[varName]; //TODO if result is undefined bubble up in scope via call tempScope = this.getParentScope(actualScopeObject)
            }

            if (typeof result !== 'undefined' && result !== null && result.length > 0) {
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

    private interpretConfigurationVariable(varName: string, variables: Object, actualScopeObject: Object): string {
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
    }

    interpretPrimitive(interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var printIfNull: boolean = interpretedVariable["printIfNullValue"];
        var replacementForNullValue: string = interpretedVariable["replaceNullValueBy"];
        var pattern: string = interpretedVariable["pattern"];
        var scope: string = interpretedVariable["scope"];
        var actualScopedObject: Object = scopedObject;
        if (typeof scope !== 'undefined') {
            actualScopedObject = this.changeScope(actualScopedObject,scope);
        }
        var value: string = this.interpretPattern(pattern, variables, actualScopedObject, printIfNull, replacementForNullValue);
        if ((value === null || value.length <= 0) && !printIfNull) {
            return "";
        } else {
            return value;
        }
    }

    interpretArray(interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var pattern: string = interpretedVariable["pattern"];
        var delimeter: string = interpretedVariable["delimeter"];
        var scope: string = interpretedVariable["scope"];
        var actualScopedObject: Object = scopedObject;
        if (typeof scope !== 'undefined') {
            actualScopedObject = this.changeScope(actualScopedObject, scope);
        }
        var value: string = "";
        $.each(actualScopedObject, (index:number, item:Object) => {
            this.setParentScope(item, actualScopedObject);
            var itemValue = this.interpretPattern(pattern, variables, item, true, "");
            if (index > 0) {
                itemValue = delimeter + itemValue;
            }
            value += itemValue;
        });

        return value;

    }

    interpretTable(interpretedVariable: Object, variables: Object, scopedObject: Object): string {
        var printRowIfNullValue: boolean = interpretedVariable["printRowIfNullValue"];
        var replaceNullValueBy: string = interpretedVariable["replaceNullValueBy"];
        var rows: Array<Object> = interpretedVariable["rows"];
        var scope: string = interpretedVariable["scope"];
        var actualScopedObject: Object = scopedObject;
        if (typeof scope !== 'undefined') {
            actualScopedObject = this.changeScope(actualScopedObject, scope);
        }
        var tableBuilder = new TableBuilder();

        $.each(rows, (index, item) => {
            var label: string = item["label"];
            var pattern: string = item["pattern"];
            var value: string = this.interpretPattern(pattern, variables, actualScopedObject, true, "");
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
}

class TableBuilder {
    private m_tableDiv: HTMLDivElement;

    constructor() {
        this.m_tableDiv = document.createElement('div');
        $(this.m_tableDiv).addClass('table');
    }

    public makeTableRow(label: string, value: string): void {
        var rowDiv: HTMLDivElement = document.createElement('div');
        $(rowDiv).addClass('row');
        var labelDiv: HTMLDivElement = document.createElement('div');
        $(labelDiv).addClass('cell label');
        labelDiv.innerHTML = label;
        rowDiv.appendChild(labelDiv);
        var valueDiv: HTMLDivElement = document.createElement('div');
        $(valueDiv).addClass('cell');
        valueDiv.innerHTML = value;
        rowDiv.appendChild(valueDiv);
        this.m_tableDiv.appendChild(rowDiv);
    }

    public build(): HTMLDivElement {
        return this.m_tableDiv;
    }
}