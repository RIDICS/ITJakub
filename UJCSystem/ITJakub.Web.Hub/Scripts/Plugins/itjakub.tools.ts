interface IDictionary<T> {
    [key: number]: T;
    [key: string]: T;
}

class DictionaryWrapper<T>  {
    private data: IDictionary<T>;

    constructor() {
        this.data = {}
    }

    public add(key: (string|number), value: T) {
        this.setValue(key, value);
    }

    public setValue(key: (string|number), value: T) {
        this.data[key] = value;
    }

    public get(key: (string|number)): T {
        return this.data[key];
    }
}

class Set {
    private data: Object;

    constructor() {
        this.data = {};
    }

    public add(key: string|number) {
        this.data[key] = true;
    }

    public addAll(keyList: Array<string|number>) {
        for (var i = 0; i < keyList.length; i++) {
            this.add(keyList[i]);
        }
    }

    public contains(key: string|number): boolean {
        return this.data[key] === true;
    }

    public remove(key: string|number) {
        delete this.data[key];
    }

    public clear() {
        this.data = {};
    }

    public isEmpty(): boolean {
        return $.isEmptyObject(this.data);
    }
}