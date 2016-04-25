class StorageManager
{
    private static instance: StorageManager = new StorageManager();

    private storage:Array<IStorage>=[];
    
    constructor() {
        if (StorageManager.instance) {
            throw new Error("Error: Instantiation failed: Use StorageManager.getInstance() instead of new.");
        }

        StorageManager.instance = this;
    }

    public static getInstance(): StorageManager {
        return StorageManager.instance;
    }

    public getStorage(type: StorageTypeEnum = StorageTypeEnum.Any):IStorage {
        if (this.storage[type] === undefined) {
            switch (type) {
                case StorageTypeEnum.Any:
                    try {
                        this.storage[type] = this.getStorage(StorageTypeEnum.Local);
                    }
                    catch (error) {
                        this.storage[type] = this.getStorage(StorageTypeEnum.Cookie);
                    }

                    break;

                case StorageTypeEnum.Local:
                    this.storage[type] = this.createLocalStorage();

                    break;
                case StorageTypeEnum.Cookie:
                    this.storage[type] = this.createCookieStorage();

                    break;
            }
        }

        return this.storage[type];
    }

    private createLocalStorage():IStorage {
        if (typeof (Storage) !== "undefined") {
            return new LocalStorage;
        } else {
            throw new StorageError("Browser not support local storage");
        }
    }

    private createCookieStorage(): IStorage {
        return new CookieStorage;
    }
}

enum StorageTypeEnum {
    Any,
    Local,
    Cookie
}

class StorageError extends Error {
}
