using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Audiobooks
{
    public class RecordingProcessor : ConcreteInstanceListProcessorBase<TrackData>
    {
        public RecordingProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "recording"; }
        }

        protected override void ProcessElement(BookData bookData, TrackData track, XmlReader xmlReader)
        {
            if (track.Recordings == null)
                track.Recordings = new List<TrackRecordingData>();


            var fileName = xmlReader.GetAttribute("url");
            track.Recordings.Add(
                new TrackRecordingData
                {
                    FileName = fileName,
                    AudioType = ParseAudioType(fileName),
                    MimeType = xmlReader.GetAttribute("mimeType"),
                    Length = TimeSpan.Parse(xmlReader.GetAttribute("length"))
                });
        }

        private AudioTypeEnum ParseAudioType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension))
                return AudioTypeEnum.Unknown;

            extension = extension.ToLowerInvariant();

            switch (extension)
            {
                case ".mp3":
                    return AudioTypeEnum.Mp3;
                case ".ogg":
                    return AudioTypeEnum.Ogg;
                case ".wav":
                    return AudioTypeEnum.Wav;
                default:
                    return AudioTypeEnum.Unknown;
            }
        }
    }
}