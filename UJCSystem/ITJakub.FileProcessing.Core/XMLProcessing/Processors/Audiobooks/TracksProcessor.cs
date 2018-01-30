using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Audiobooks
{
    public class TracksProcessor : ProcessorBase
    {
        public TracksProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "tracks"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase> {Container.Resolve<TrackProcessor>()}; }
        }

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.Tracks == null)
            {
                bookData.Tracks = new List<TrackData>();
            }
        }
    }
}