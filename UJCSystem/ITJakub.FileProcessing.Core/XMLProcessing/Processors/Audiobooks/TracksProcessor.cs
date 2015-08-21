using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
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

        protected override void PreprocessSetup(BookVersion bookVersion)
        {
            if (bookVersion.Tracks == null)
            {
                bookVersion.Tracks = new List<Track>();
            }
        }
    }
}