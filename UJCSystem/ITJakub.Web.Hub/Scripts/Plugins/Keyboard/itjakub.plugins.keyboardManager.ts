namespace KeyboardManager {
    var loadedKeyboards: Array<Keyboard> = new Array();
    var keyboardMap: { [key: string]: KeyboardComponent } = {};

    function findContainer(key: string) {
        return $(`.component.keyboard-component[data-keyboard-id="${key}"]`).get(0);
    }

    function getApiUrl(keyboardComponent: KeyboardComponent):string {
        return keyboardComponent.getComponent().attr("data-keyboard-api-url");
    }

    export function getKeyboard(keyboard: string): KeyboardComponent {
        if (typeof keyboardMap[keyboard] == "undefined") {
            keyboardMap[keyboard] = new KeyboardComponent(findContainer(keyboard), loadedKeyboards, "keyboard-", "/");
            keyboardMap[keyboard].getComponent().append(keyboardMap[keyboard].createKeyboardHtml(true));
            keyboardMap[keyboard].executeScripts(getApiUrl(keyboardMap[keyboard]));
        }

        return keyboardMap[keyboard];
    }

    export function unsetKeyboard(keyboard:string) {
        delete keyboardMap[keyboard];
    }
}