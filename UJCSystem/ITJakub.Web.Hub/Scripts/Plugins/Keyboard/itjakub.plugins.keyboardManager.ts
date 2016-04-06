namespace KeyboardManager {
    var loadedKeyboards: Array<Keyboard> = new Array();
    var keyboardMap: { [key: string]: KeyboardComponent } = {};

    function findContainer(key: string) {
        return $(`.component.keyboard-component[data-keyboard-id="${key}"]`).get(0);
    }

    

    export function getKeyboard(keyboard: string, lazy: boolean=true): KeyboardComponent {
        if (typeof keyboardMap[keyboard] == "undefined") {
            keyboardMap[keyboard] = new KeyboardComponent(findContainer(keyboard), loadedKeyboards, "keyboard-", "/");
            keyboardMap[keyboard].getComponent().append(keyboardMap[keyboard].createKeyboardHtml(true));

            if (!lazy) {
                keyboardMap[keyboard].executeScripts();
            }
        }

        return keyboardMap[keyboard];
    }

    export function unsetKeyboard(keyboard:string) {
        delete keyboardMap[keyboard];
    }
}