using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser
{
    public interface IControlFieldProcessor : IFieldProcessor
    {
        void Process(controlFieldType dataField, ImportedProject importedProject);
    }
}
