class KeyboardComponent {
    private id: string;
    private originalLeft: string;
    private origionalTop: string;
    private input = $("input:first-of-type");
    private jsonObject: Array<{
        Name: string;
        Id: string,
        Url: string;
    }> = [];
    private loaded = false;
    private overrideSetQueryCallback: (text: string) => void;
    
    private keyboardShowLeftSpace=40;

    constructor(private component: HTMLElement, private loadedKeyboards: Array<Keyboard>, private componentPrefix: string="keyboard-", private resourceRoot="") {
        this.id = component.getAttribute("data-keyboard-id");
        component.id = `${this.componentPrefix}component-${this.id}`;
    }

    getId() {
        return this.id;
    }

    createInto(container:JQuery, hidden: boolean=false) {
        container.append(this.getFormHtml());
        container.append(this.createKeyboardHtml(hidden));
    }

    getFormHtml(): HTMLFormElement {
        var form = document.createElement("form");
        form.id = `${this.componentPrefix}searchForm${this.id}`;
        form.name = `searchForm${this.id}`;

        var input = document.createElement('input');
        input.classList.add('form-control');
        input.classList.add('keyboard-input');
        input.classList.add(`${this.componentPrefix}input${this.id}`);

        form.appendChild(input);
        
        this.registerInputs($(input)); //TODO need check

        return form;
    }

    createKeyboardHtml(hidden: boolean = false): HTMLDivElement {
        var container=document.createElement('div');
        container.classList.add( 'keyboard_container');
        container.id= `${this.componentPrefix}container-${this.id}`;
        container.innerHTML = `<button type="button" class="close keyboardHideButton" id="${this.componentPrefix}hideButton-${this.id}">
                        <span aria-hidden="true">&times;</span><span class="sr-only">Close</span>
                    </button>
                    <button type="button" class="btn btn-link btn-sm pin-button" id="${this.componentPrefix}pin-button-${this.id}">
                        <span class="glyphicon glyphicon-pushpin"></span>
                    </button>
                    <div class="tabs" id="${this.componentPrefix}tabs-container-${this.id}">
                        <ul class="nav nav-tabs keyboard_tabs" id="${this.componentPrefix}tabs-${this.id}"><!--Dynamicly created keyboard tabs--></ul>
                        <div class="tab-content keyboard_content" id="${this.componentPrefix}content-${this.id}"><!--Dynamicly created keyboard tab panels--></div>
                    </div>`;

        if (hidden) {
            $(container).hide();
        }

        return container;
    }

    public setInput(input: JQuery): KeyboardComponent {
        this.input = input;

        return this;
    }

    public registerInputs(inputs: JQuery) {
        inputs.wrap(`<div class="input_container">`);

        var image = this.createImage();

        inputs.each((input: number, element: Element) => {
            this.registerInput(element, image);
        });
    }

    public createImage(): HTMLImageElement {
        const image = document.createElement("img");
        image.src = `${this.resourceRoot}Content/Images/Glyphs/klavesnice.gif`;
        image.alt = "Show keyboard";
        image.classList.add("keyboard-icon-img");
        
        return image;
    }

    public registerInput(element: Element, image: HTMLImageElement = null) {
        if (image === null || image === undefined) {
            image = this.createImage();
        }
        const clonedImage: HTMLImageElement = <HTMLImageElement>image.cloneNode(true);
        var jElement = $(element);

        element.addEventListener("focus", () => {
            if (!clonedImage.classList.contains("disabled")) {
                this.setInput(jElement);
            }
        });

        clonedImage.addEventListener("click", (event: JQueryEventObject) => {
            this.overrideSetQueryCallback = null;

            if (!clonedImage.classList.contains("disabled")) {
                this.setInput(jElement);
                this.toggleKeyboard(event);
            }
        });

        element.parentElement.appendChild(clonedImage);
    }

    public registerButton(buttonElement: HTMLButtonElement, inputElement: HTMLInputElement, overrideSetQueryCallback: (text: string) => void) {
        var jElement = $(inputElement);
        
        inputElement.addEventListener("focus", () => {
            if (!buttonElement.classList.contains("disabled")) {
                this.setInput(jElement);
            }
        });

        buttonElement.addEventListener("click", (event: JQueryEventObject) => {
            this.overrideSetQueryCallback = overrideSetQueryCallback;

            if (!buttonElement.classList.contains("disabled")) {
                this.setInput(jElement);
                this.toggleKeyboard(event);
            }

            $(inputElement).focus();
        });

        $(inputElement).blur(() => { // Fix: preserve cursor position on focus lost
            var cursorPositionBackup = this.getCursorPosition();
            setTimeout(() => this.setCursorPosition(cursorPositionBackup));
        });
    }

    protected toggleKeyboard(event?: JQueryEventObject) {
        const keyboard = this.getKeyboard();

        if (keyboard.css("display") == "none") {
            const input = this.getInput();

            if (!this.loaded) {
                this.executeScripts();
            }

            if (event) {
                const inputOffset = input.offset();
                const parentOffset = keyboard.parent().offset();
                const newTop = inputOffset.top - parentOffset.top + input.outerHeight();
                const newLeft = inputOffset.left - parentOffset.left;

                keyboard.css({
                    top: newTop + "px",
                    left: newLeft + "px"
                });
            }

            keyboard.show();
        } else {
            keyboard.hide();
            // reset div position
            keyboard.css({
                'left': this.originalLeft,
                'top': this.origionalTop
            });
        }
    }

    public getInput():JQuery {
        return this.input;
    }
    
    private getKeyboard() {
        return $(`#${this.componentPrefix}container-${this.id}`);
    }

    private getContent() {
        return $(`#${this.componentPrefix}content-${this.id}`);
    }

    public getComponent() {
        return $(`#${this.componentPrefix}component-${this.id}`);
    }

    getApiUrl(): string {
        return this.getComponent().attr("data-keyboard-api-url");
    }

    public setInputValue(value: string) {
        if (typeof this.overrideSetQueryCallback === "function") {
            this.overrideSetQueryCallback(value);
        } else {
            this.input.val(value);
        }
    }

    public executeScripts() {
        this.loaded = true;
        var listLink = this.getApiUrl();

        var keyboard = this.getKeyboard();

        var thisComponent: KeyboardComponent = this;

        keyboard.draggable(<JQueryUI.DraggableOptions>{
            start() {
                keyboard.css("cursor", "move");
            },
            stop() {
                keyboard.css("cursor", "default");
                keyboard.css("height", ""); //fix: unset fixed CSS height
            },
            containment: "body",
            appendTo: "body",
            cursor: "move" 
        });

        // data storage
        this.originalLeft = keyboard.css('left');
        this.origionalTop = keyboard.css('top');
        $(`#${this.componentPrefix}hideButton-${this.id}`).click(() => {
            this.toggleKeyboard(); //only possibility is hide
        });
        
        $(`#${this.componentPrefix}pin-button-${this.id}`).click(() => {
            var disabled = keyboard.draggable("option", "disabled");
            if (disabled) {
                keyboard.draggable("enable");
            } else {
                keyboard.draggable("disable");
            }

        });

        $.ajax({
            dataType: "json",
            url: listLink,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: data => {
                var dataJson = jQuery.parseJSON(data);
                var aHref = '';
                console.log(dataJson);
                
                for (var i in dataJson) {
                    aHref = `<li><a id="${this.componentPrefix}tabs-${this.id}-${dataJson[i].Id}" href="#${thisComponent.componentPrefix}tabs-${this.id}-${dataJson[i].Id}" data-json-id="${dataJson[i].Id}">${dataJson[i].Name}</a></li>`;
                    thisComponent.jsonObject[dataJson[i].Id] = dataJson[i];

                    if (<string>i =="0") {
                        $(`#${this.componentPrefix}tabs-${this.id}`).append(`<li class="active">${aHref}</li>`);
                    }
                    else {
                        $(`#${this.componentPrefix}tabs-${this.id}`).append(`<li>${aHref}</li>`);
                    }
                }
                //$("#tabs" + thisComponent.id).tabs();
                // getting default layout
                $.ajax({
                    dataType: "json",
                    url: dataJson[0].Url,
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    success: layoutData => {
                        var layoutJson = jQuery.parseJSON(layoutData);
                        
                        thisComponent.ajaxCall(layoutJson, this.loadedKeyboards, dataJson);
                    },
                    error: function (jqXHR, textStatus) {
                        console.error(textStatus);
                    }
                });

            },
            error: function (jqXHR, textStatus) {
                console.error(textStatus);
            }
        });
    }

    public ajaxCall(layoutJson, loadedKeyboards: Array<Keyboard>, dataJson) {
        var thisComponent: KeyboardComponent = this;

        var newId = dataJson[0].Id + thisComponent.id;
        
        var defaultLayout: Keyboard = new Keyboard(layoutJson.Keyboard, newId, this.componentPrefix);
        loadedKeyboards.push(defaultLayout);

        var content=this.getContent();
        var keyboard=this.getKeyboard();

        content.html(`<div class="tab-pane active" id="${thisComponent.componentPrefix}tabs-${defaultLayout.id}">${defaultLayout.html}</div>`);
        
        content.width(defaultLayout.keyboardWidth);
        keyboard.width(defaultLayout.keyboardWidth);
        
        defaultLayout.cellClick(thisComponent, thisComponent.id);

        // getting layout on click
        $(`#${thisComponent.componentPrefix}tabs-${thisComponent.id} a`).click(function (e) {
            e.preventDefault();
            $(this).parent().parent().find("li").removeClass("active");
            $(this).parent().addClass("active");

            var requestingLayout = thisComponent.jsonObject[this.getAttribute('data-json-id')];
            var layoutLocal: boolean = false;
            var loadedLayout: Keyboard;
            
            // if keyboard is loaded already we don't request it again
            for (var j in loadedKeyboards) {
                if (loadedKeyboards[j].id == requestingLayout.Id) {
                    layoutLocal = true;
                    loadedLayout = loadedKeyboards[j];
                    break;
                }
            }

            if (layoutLocal) {
                content.html(`<div class="tab-pane active" id="${thisComponent.componentPrefix}tabs-${thisComponent.id}-${loadedLayout.id}">${loadedLayout.html}</div>`);

                $(`#${thisComponent.componentPrefix}keyboard_container${thisComponent.id}`).width(loadedLayout.keyboardWidth);
                keyboard.width(loadedLayout.keyboardWidth);
                
                loadedLayout.cellClick(thisComponent, thisComponent.id);

            } else {
                $.ajax({
                    dataType: "json",
                    url: requestingLayout.Url,
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    success(newLayout) {
                        var newLayoutJson = jQuery.parseJSON(newLayout);

                        var newKeyboardId = requestingLayout.Id + thisComponent.id;

                        var newKeyboard: Keyboard = new Keyboard(newLayoutJson.Keyboard, newKeyboardId, thisComponent.componentPrefix);
                        loadedKeyboards.push(newKeyboard);

                        content.html(`<div class="tab-pane active" id="${thisComponent.componentPrefix}tabs-${newKeyboard.id}">${newKeyboard.html}</div>`);

                        $(`#${thisComponent.componentPrefix}keyboard_container${thisComponent.id}`).width(newKeyboard.keyboardWidth);
                        keyboard.width(newKeyboard.keyboardWidth);

                        newKeyboard.cellClick(thisComponent, thisComponent.id);
                    },
                    error: function (jqXHR, textStatus) {
                        console.error(textStatus);
                    }
                });
            }
        });
    }
    
    public getCursorPosition(): number {
        var el = <HTMLInputElement>this.input.get(0);
        var pos = el.value.length;
        if ('selectionStart' in el) {
            pos = el.selectionStart;
        }
        return pos;
    }

    public setCursorPosition(position: number) {
        var el = <HTMLInputElement>this.input.get(0);
        if ('selectionStart' in el) {
            el.selectionStart = position;
            el.selectionEnd = position;
        }
    }
}