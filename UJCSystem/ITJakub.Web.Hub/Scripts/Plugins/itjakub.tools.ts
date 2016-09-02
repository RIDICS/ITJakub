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
