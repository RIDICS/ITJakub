using System;
using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Audiobooks
{
    public class TrackProcessor : ConcreteInstanceProcessorBase<TrackData>
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

        protected override IEnumerable<ConcreteInstanceProcessorBase<TrackData>> ConcreteSubProcessors
        {
            get { return new List<ConcreteInstanceProcessorBase<TrackData>> {m_recordingProcessor}; }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            var track = new TrackData
            {
                Name = xmlReader.GetAttribute("title"),
                Text = xmlReader.GetAttribute("source"),
                Position = Convert.ToInt32(xmlReader.GetAttribute("n")),
                Recordings = new List<TrackRecordingData>()
            };
            bookData.Tracks.Add(track);

            base.ProcessElement(bookData,track, xmlReader);
        }
    }
}