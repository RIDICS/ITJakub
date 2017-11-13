class KeyTableKindEditor {
    private readonly util: EditorsUtil;

    constructor() {
        this.util = new EditorsUtil();
    }

    init() {
        this.util.getLitararyKindList().done((data) => {
            console.log(data);
        });
    }
}