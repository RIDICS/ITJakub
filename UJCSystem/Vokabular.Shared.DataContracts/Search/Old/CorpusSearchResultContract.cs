using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Search.Corpus;

namespace Vokabular.Shared.DataContracts.Search.Old
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results", Name = "CorpusSearchResultContract")]
    public class CorpusSearchResultContract
    {
        [DataMember]
        public long BookId { get; set; }

        [DataMember]
        public string BookXmlId { get; set; }

        [DataMember]
        public string VersionXmlId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string OriginDate { get; set; }

        [DataMember]
        public string Acronym { get; set; }

        [DataMember]
        public IList<string> Notes { get; set; }

        [DataMember]
        public PageResultContext PageResultContext { get; set; }

        [DataMember]
        public VerseResultContext VerseResultContext { get; set; }

        [DataMember]
        public BibleVerseResultContext BibleVerseResultContext { get; set; }


        public virtual string ToXml()
        {
            using (Stream stream = new MemoryStream())
            {
                //Serialize the Record object to a memory stream using DataContractSerializer. 
                DataContractSerializer serializer = new DataContractSerializer(GetType());
                serializer.WriteObject(stream, this);


                stream.Position = 0;
                string result = new StreamReader(stream).ReadToEnd();

                return result;
            }
        }

        public static CorpusSearchResultContract FromXml(string xml)
        {
            using (Stream stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(xml);
                writer.Flush();

                stream.Position = 0;
                //object result = new XmlSerializer(typeof (Server)).Deserialize(stream); 

                object result = new DataContractSerializer(typeof(CorpusSearchResultContract)).ReadObject(stream);
                return (CorpusSearchResultContract)result;
            }
        }
    }
}