using System.Threading.Tasks.Dataflow;

namespace Vokabular.ProjectImport.ImportPipeline
{
    public class ImportPipeline
    {
        public IDataflowBlock LastBlock { get; set; }
        public BufferBlock<object> BufferBlock { get; set; }
    }
}