class MyStorageManager
{
    private static instance: MyStorageManager = new MyStorageManager();

    private storage:Array<IStorage>=[];
    
    constructor() {
        if (MyStorageManager.instance) {
            throw new Error("Error: Instantiation failed: Use MyStorageManager.getInstance() instead of new.");
        }

        MyStorageManager.instance = this;
    }

    public static getInstance(): MyStorageManager {
        return MyStorageManager.instance;
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
