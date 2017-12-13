declare class SimpleMDEExtended extends SimpleMDE {
    defineMode(name: string, token: any): void;
}

declare namespace SimpleMDE {
    interface Options {
        mode?: string;
    }
}