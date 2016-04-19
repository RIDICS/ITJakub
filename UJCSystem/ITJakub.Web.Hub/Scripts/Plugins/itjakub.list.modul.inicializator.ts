class ListModulInicializator extends ModulInicializator {
    protected configuration: IListModulInicializatorConfiguration;

    constructor(configuration: IListModulInicializatorConfiguration) {
        super(configuration);
    }
}

interface IListModulInicializatorConfiguration extends IModulInicializatorConfiguration {
    
}
