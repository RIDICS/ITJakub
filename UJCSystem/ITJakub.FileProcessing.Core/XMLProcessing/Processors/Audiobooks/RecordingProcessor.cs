using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Audiobooks
{
    public class RecordingProcessor : ConcreteInstanceListProcessorBase<Track>
    {
        public RecordingProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "recording"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, Track track, XmlReader xmlReader)
        {
            if (track.Recordings == null)
                track.Recordings = new List<Recording>();


            var fileName = xmlReader.GetAttribute("url");
            track.Recordings.Add(
                new Recording
                {
                    Track = track,
                    FileName = fileName,
                    AudioType = ParseAudioType(fileName),
                    MimeType = xmlReader.GetAttribute("mimeType"),
                    Length = TimeSpan.Parse(xmlReader.GetAttribute("length"))
                });
        }

        private AudioType ParseAudioType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension))
                return AudioType.Unknown;

            extension = extension.ToLowerInvariant();

            switch (extension)
            {
                case "mp3":
                    return AudioType.Mp3;
                case "ogg":
                    return AudioType.Ogg;
                case "wav":
                    return AudioType.Wav;
                default:
                    return AudioType.Unknown;
            }
        }
    }
}