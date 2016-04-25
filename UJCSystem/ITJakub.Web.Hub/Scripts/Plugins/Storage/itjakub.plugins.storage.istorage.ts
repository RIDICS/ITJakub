interface IStorage {
    update: (key: string, fieladKey: string, value: any) => void;

    save: (key: string, value: any) => void;
    get: (key: string) => any;
    delete: (key: string) => void;
}