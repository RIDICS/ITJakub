using System;
using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Audiobooks
{
    public class TrackProcessor : ConcreteInstanceProcessorBase<Track>
    {
        private readonly RecordingProcessor m_recordingProcessor;

        public TrackProcessor(XsltTransformationManager xsltTransformationManager, IKernel container, RecordingProcessor recordingProcessor)
            : base(xsltTransformationManager, container)
        {
            m_recordingProcessor = recordingProcessor;
        }

        protected override string NodeName
        {
            get { return "track"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<Track>> ConcreteSubProcessors
        {
            get { return new List<ConcreteInstanceProcessorBase<Track>> {m_recordingProcessor}; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            var track = new Track
            {
                BookVersion = bookVersion,
                Name = xmlReader.GetAttribute("title"),
                Position = Convert.ToInt32(xmlReader.GetAttribute("n")),
                Recordings = new HashSet<TrackRecording>()
            };
            bookVersion.Tracks.Add(track);

            base.ProcessElement(bookVersion,track, xmlReader);
        }
    }
}