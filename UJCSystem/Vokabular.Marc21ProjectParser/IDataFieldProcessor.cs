using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser
{
    public interface IDataFieldProcessor : IFieldProcessor
    {
        void Process(dataFieldType dataField, Project project);
    }
}
