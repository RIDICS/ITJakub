enum ButtonType {
    Letter,
    CapsLock,
    Tab,
    Shift,
    Control,
    Backspace,
    Alt,
    Delete,
    Enter
}

class KeyboardCell {
    public capitalUnicode: string;
    public unicode: string;
    public fixedWidth: boolean;
    public title: string;
    public type: string;
    public width: number;
    public enabled: boolean;

    public customWidthPercentage: number = 0;
}

class KeyboardRow {
    public marginLeft: number;
    public marginRight: number;
    public cells: KeyboardCell[];
    public nonFixedWidthCellCount: number = 0;
    public emptySpacePx: number = 0;
}

class Keyboard {
    public cellHeight: number;
    public cellWidth: number;
    public name: string;
    public keyboardRows: KeyboardRow[];
    
    public html: string;
    public keyboardWidth: number;
    public keyboardHeight: number;
    
    public defaultCellHeightPercentage: number = 0;
    public defaultCellWidthPercentage: number = 0;

    public verticalCellMargin: number = 2;
    public horizontalCellMargin: number = 2;

    public keyboardLowerCase: boolean = false;
    public shiftUsed: boolean = false;
    public capsLockUsed: boolean = false;

    public easterEgg = true;
    public easterEggCounter = 0;
    public easterEggLimit = 10;

    constructor(theJsonData: Object, public id: string, private componentPrefix:string) {
        this.extractData(theJsonData);

        this.countCellsWidth();
        this.html = this.getHtml();
    }

    // fills data to this object properties
    private extractData(data) {

        this.name = data.Name;
        this.cellHeight = parseInt(data.CellHeight);
        this.cellWidth = parseInt(data.CellWidth);
        this.verticalCellMargin = parseInt(data.VerticalCellMargin);
        this.horizontalCellMargin = parseInt(data.HorizontalCellMargin);
        this.keyboardRows = [];       
        var keyRows = data.Rows.KeyboardRow;

        var cellWidth = this.cellWidth;
        var keyboardRows1 = this.keyboardRows;
        var rowCount: number = 0;

        var maxWidth: number = 0;
        var verticalCellMar = this.verticalCellMargin;
        var horizontalCellMar = this.horizontalCellMargin;

        keyRows.forEach(row => {
            rowCount++;
            var nRow = new KeyboardRow();
            nRow.marginLeft = parseInt(row.MarginLeft);
            nRow.marginRight = parseInt(row.MarginRight);

            nRow.cells = [];

            var nfCellCount: number = 0;
            var cells = row.Cells.KeyboardCell;

            var rowWidth: number = 0;

            cells.forEach(cell => {

                var nCell:KeyboardCell = new KeyboardCell();
                nCell.unicode = cell.Unicode;
                nCell.capitalUnicode = cell.CapitalUnicode;
                nCell.fixedWidth = (cell.FixedWidth === 'true');
                nCell.enabled = (cell.Enabled === 'true');
                nCell.title = cell.Title;
                nCell.type = cell.Type;
                nCell.width = parseInt(cell.Width);

                // counting empty space of each row in pixels
                if (!nCell.fixedWidth) {
                    nfCellCount++;
                    nRow.emptySpacePx = nRow.emptySpacePx + 2 * horizontalCellMar;
                }
                else {
                    if (nCell.width > 0) {
                        nRow.emptySpacePx = nRow.emptySpacePx + nCell.width + 2 * horizontalCellMar;
                    } else {
                        nRow.emptySpacePx = nRow.emptySpacePx + cellWidth + 2 * horizontalCellMar;
                    }
                }
                
                // counting table total width in pixels
                if (nCell.width > 0) {
                    rowWidth = rowWidth + nCell.width + 2 * horizontalCellMar;
                } else {
                    rowWidth = rowWidth + cellWidth + 2 * horizontalCellMar;
                }
                
           
                nRow.cells.push(nCell);
            });
            nRow.nonFixedWidthCellCount = nfCellCount;
            keyboardRows1.push(nRow);

            if (rowWidth > maxWidth) maxWidth = rowWidth;

        });
        this.keyboardWidth = maxWidth;
        this.keyboardHeight = rowCount * (this.cellHeight + 2 * verticalCellMar);

        this.keyboardRows.forEach(row => {
            row.emptySpacePx = maxWidth - row.emptySpacePx;
        });

    }

    private countCellsWidth() {
        this.defaultCellWidthPercentage = (this.cellWidth / this.keyboardWidth) * 100;
        this.defaultCellHeightPercentage = (this.cellHeight / this.keyboardHeight) * 100;

        var defaultX = this.defaultCellWidthPercentage;
        var tableX = this.keyboardWidth;
        
       
        this.keyboardRows.forEach(row => {

            var emptySpacePercentage = (row.emptySpacePx / tableX) * 100;
            var nonFixedWidth = emptySpacePercentage / row.nonFixedWidthCellCount;

            row.cells.forEach(cell => {
                if (cell.fixedWidth) {
                    if (cell.width > 0) {
                        var newPer: number = (cell.width / tableX) * 100;
                        if (newPer < defaultX) cell.customWidthPercentage = defaultX;
                        else cell.customWidthPercentage = newPer;
                    }
                    else cell.customWidthPercentage = defaultX;
                    
                }
                else cell.customWidthPercentage = nonFixedWidth;
            });     

        });
    }

    public getHtml(): string {

        var outHtml: string = "";
        var defaultHeight = this.defaultCellHeightPercentage;
        var defaultCellHeightPx = this.cellWidth;
        var verticalCellMar = this.verticalCellMargin;
        var horizontalCellMar = this.horizontalCellMargin;

        var id = this.id;

        outHtml = outHtml + "<div id=\"keyboard"+id+"\" style=\"width:"+this.keyboardWidth+"px; height:"+this.keyboardHeight+"px;\">";
        
        outHtml = outHtml + this.getRowHtml(defaultHeight, verticalCellMar, horizontalCellMar, defaultCellHeightPx, this.keyboardRows) + "</div>";

        return outHtml;
    }

    private getRowHtml(defaultHeight: number, verticalCellMar: number, horizontalCellMar: number, defaultCellHeightPx: number, rows: KeyboardRow[]): string {
        var outHtml: string = "";

        var objRef: Keyboard = this;

        rows.forEach(function (row) {

            outHtml = outHtml + "<div class=\"keyboard-row\" style=\"width: 100%; " +
            "height: " + defaultHeight + "%; text-align: center;" +
            "margin-top: " + verticalCellMar + "px;" +
            "margin-bottom: " + verticalCellMar + "px;" +
            "margin-left: " + row.marginLeft + "%;" +
            "margin-right: " + row.marginRight + "%;" +
            "background-color: #ffffff;\">";          

            outHtml = outHtml + objRef.getCellHtml(row.cells, horizontalCellMar, defaultCellHeightPx) + "</div>";

        });

        return outHtml;
    }

    private getCellHtml(cells: KeyboardCell[], horizontalCellMar: number, defaultCellHeightPx: number): string {
        var outHtml: string = "";

        cells.forEach(function (cell) {

            outHtml = outHtml + "<div ";

            var cellHead: string;

            if (cell.enabled) {
                var typeContent: string = (cell.type === "Letter") ? "data-unicode=\"" + cell.unicode + "\" data-capital=\"" + cell.capitalUnicode + "\"" : "data-action=\"" + cell.type + "\"";
                cellHead = "data-type=\"" + cell.type + "\"" +
                typeContent +
                "class=\"keyboard-cell " + cell.type.toLowerCase() + "\"";
            } else {
                cellHead = "class=\"keyboard-cell disabled-cell\"";
            }


            outHtml = outHtml +
            cellHead +
            "title=\"" + cell.title + "\" " +
            "style=\"display: inline-block; " +
            "width:" + cell.customWidthPercentage + "%; " +
            "height: 100%; overflow: hidden; " +
            "margin-left: " + horizontalCellMar + "px; " +
            "margin-right: " + horizontalCellMar + "px; " +
            "border: 1px solid #CCC;" +
            "font-size: " + defaultCellHeightPx / 3 + "px;\">";

            if (cell.type === "Letter") {
                outHtml = outHtml + "<div class=\"default-button\">" + String.fromCharCode(parseInt(cell.unicode, 16)) +
                "</div><div class=\"second-button\">" + String.fromCharCode(parseInt(cell.capitalUnicode, 16)) + "</div>" + "</div>";
            } else {
                outHtml = outHtml + "<div style=\"line-height: " + defaultCellHeightPx + "px; " +
                "text-align: center; vertical-align: middle; font-size: 80%;\">" + cell.title + "</div></div>";
            }

        });

        return outHtml;
    }

    // lowerCase - to change button text to capital letter or lowercase letter
    // element - $(".keyboard-cell") element
    public changeButtons(lowerCase: boolean, element) {
        if (!lowerCase && element.dataset.capital != undefined) {
            $(element).html(`<div class="default-button">${String.fromCharCode(parseInt(element.dataset.capital, 16))}</div>
                <div class="second-button">${String.fromCharCode(parseInt(element.dataset.unicode, 16))}</div>`);
        } else if (lowerCase && element.dataset.unicode != undefined) {
            $(element).html(`<div class="default-button">${String.fromCharCode(parseInt(element.dataset.unicode, 16))}</div>
                <div class="second-button">${String.fromCharCode(parseInt(element.dataset.capital, 16))}</div>`);
        }
         
    }

    public resizeAction(lowerCase: boolean, layout: Keyboard, cell:JQuery) {
        lowerCase = this.keyboardLowerCase;
        cell.each(function () {
            layout.changeButtons(lowerCase, this);
        });
        this.keyboardLowerCase = !this.keyboardLowerCase;
    }

    public cellClick(keyboardComponent: KeyboardComponent, componentId: string) {
        var cell: JQuery = $(`#${this.componentPrefix}content-${componentId}  .keyboard-row .keyboard-cell`);

        var thisComponent:Keyboard = this;

        var layout: Keyboard = this;
        cell.click(function () {
            var keyboardInput = keyboardComponent.getInput();
            
            if (this.dataset.type === "Letter") {
                var newChar: string;
                if (layout.keyboardLowerCase) {
                    newChar = String.fromCharCode(parseInt(this.dataset.capital, 16));
                } else {
                    newChar = String.fromCharCode(parseInt(this.dataset.unicode, 16));
                }

                keyboardInput.val(keyboardInput.val() + newChar);

                if (layout.shiftUsed) {
                    layout.resizeAction(layout.keyboardLowerCase, layout, cell);
                    $(`#${this.componentPrefix}content-${componentId} .shift`).css("background-color", "#ffffff");
                    layout.shiftUsed = !layout.shiftUsed;
                }

                thisComponent.easterEggCounter = 0;

            } else {
                switch (this.dataset.action) {
                    case "Backspace":
                        console.log("Backspace action");
                        keyboardInput.val(keyboardInput.val().slice(0, -1));
                        break;
                    case "Enter":
                        console.log("Enter action");
                        break;
                    case "Shift":
                        console.log("Shift action");

                        if (layout.shiftUsed) $(`#${thisComponent.componentPrefix}content-${componentId} .shift`).css("background-color", "#ffffff");
                        else $(`#${thisComponent.componentPrefix}content-${componentId} .shift`).css("background-color", "#FFFB80");

                        layout.resizeAction(layout.keyboardLowerCase, layout, cell);
                        layout.shiftUsed = !layout.shiftUsed;
                        break;
                    case "Control":
                        console.log("Ctrl action");
                        break;
                    case "Tab":
                        console.log("Tab action");
                        break;
                    case "Alt":
                        console.log("Alt action");
                        break;
                    case "CapsLock":
                        console.log("Caps Lock action");

                        if (layout.capsLockUsed) $(`#${thisComponent.componentPrefix}content-${componentId} .capslock`).css("background-color", "#ffffff");
                        else $(`#${thisComponent.componentPrefix}content-${componentId} .capslock`).css("background-color", "#FFFB80");

                        layout.resizeAction(layout.keyboardLowerCase, layout, cell);
                        layout.capsLockUsed = !layout.capsLockUsed;
                        break;
                    case "Delete":
                        console.log("Delete action");
                        break;
                    default:
                        console.log("Undefined action.");

                        thisComponent.easterEggCounter++;
                        if (thisComponent.easterEgg && thisComponent.easterEggCounter > thisComponent.easterEggLimit) {
                            window.open('http://roxik.com/cat/', '_blank');
                            thisComponent.easterEgg = false;
                        }

                        break;
                }
            }

        });
     

    }
}