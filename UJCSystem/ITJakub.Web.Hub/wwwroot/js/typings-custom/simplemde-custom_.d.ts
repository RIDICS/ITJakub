interface IExtendedSimpleMDE extends SimpleMDE{
    defineMode?(name: string, token: Function): void;//HACK, can't extend class
}

declare namespace SimpleMDE {
    interface Options {
        mode?: string;
    }
}