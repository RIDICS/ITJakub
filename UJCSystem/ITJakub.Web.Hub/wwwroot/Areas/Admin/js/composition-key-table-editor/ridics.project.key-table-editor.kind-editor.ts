class KeyTableKindEditor {
    private readonly util: KeyTableUtilManager;

    constructor() {
        this.util = new KeyTableUtilManager();
    }

    init() {
        this.util.getLitararyKindList().done((data) => {
            console.log(data);
        });
    }
}