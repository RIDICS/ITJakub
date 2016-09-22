class LocalStorage implements IStorage {
    update(key: string, fieladKey: string, value: any) {
        var previous = this.get(key);
        if (previous === null) {
            previous = {};
        }
        if (value === null) {
            delete previous[fieladKey];
        } else {
            previous[fieladKey] = value;
        }

        this.save(key, previous);
    }

    save(key: string, value: any) {
        localStorage.setItem(key, JSON.stringify(value));
    }

    get(key: string) {
        return JSON.parse(localStorage.getItem(key));
    }

    delete(key: string) {
        localStorage.removeItem(key);
    }
}
