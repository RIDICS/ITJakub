/// <reference types="bootstrap"/>

declare namespace Types {
    interface Tree {
        collapse(node: any, cascade: boolean): Tree; // WORKAROUND this collapse method is not compatible with Bootstrap so specify all methods here again to fix the problem
        collapse(action: "toggle" | "show" | "hide"): this;
        collapse(options?: CollapseOptions): this;
    }
}