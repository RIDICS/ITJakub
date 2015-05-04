class RegExEditor {
    container: HTMLDivElement;
    searchBox: HTMLElement;

    constructor(container: HTMLDivElement, searchBox: HTMLElement) {
        this.container = container;
        this.searchBox = searchBox;
    }

    public makeRegExEditor() {
        var testDiv: HTMLElement = document.createElement("strong");
        testDiv.innerHTML = "TEST";
        $(this.container).empty();
        $(this.container).append(testDiv);
    }
} 