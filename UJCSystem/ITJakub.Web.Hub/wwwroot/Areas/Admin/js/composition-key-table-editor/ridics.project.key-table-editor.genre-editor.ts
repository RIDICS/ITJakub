class KeyTableGenreEditor {
    private readonly util: EditorsUtil;

    constructor() {
        this.util = new EditorsUtil();
    }

    init() {
        this.util.getLitararyGenreList().done((data) => {
            console.log(data);
        });
    }
}