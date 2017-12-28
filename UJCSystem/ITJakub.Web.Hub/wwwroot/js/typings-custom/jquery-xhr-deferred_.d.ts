interface JQueryXHR {
    done: any;
    fail: any;
    always: any;
}

declare namespace JQuery {
    interface jqXHR {
        done: any;
        fail: any;
        always: any;
    }
}