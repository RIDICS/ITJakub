class KeyTableKindEditor {
    private readonly util: KeyTableViewManager;

    constructor() {
        this.util = new KeyTableViewManager();
    }

    init() {
        this.util.getLitararyKindList().done((data) => {
            console.log(data);
        });
    }
}