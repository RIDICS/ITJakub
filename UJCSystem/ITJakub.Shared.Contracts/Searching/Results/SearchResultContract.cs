using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Results
{
    [DataContract]
    public class SearchResultContract
    {
        [DataMember]
        public string BookXmlId { get; set; }

        [DataMember]
        public string VersionXmlId { get; set; }

        [DataMember]
        public BookTypeEnumContract BookType { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string SubTitle { get; set; }

        [DataMember]
        public string PublishPlace { get; set; }

        [DataMember]
        public string PublishDate { get; set; }

        [DataMember]
        public List<AuthorContract> Authors { get; set; }

        [DataMember]
        public PublisherContract Publisher { get; set; }

        [DataMember]
        public List<string> Keywords { get; set; }

        [DataMember]
        public string Copyright { get; set; }

        [DataMember]
        public int PageCount { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public string CreateTimeString { get; set; }

        [DataMember]
        public List<EditorContract> Editors { get; set; }

        [DataMember]
        public List<ManuscriptContract> Manuscripts { get; set; }

        [DataMember]
        public int TotalHitCount { get; set; }

        [DataMember]
        public IList<PageResultContext> Results { get; set; }


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

        public static SearchResultContract FromXml(string xml)
        {
            using (Stream stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(xml);
                writer.Flush();

                stream.Position = 0;
                //object result = new XmlSerializer(typeof (Server)).Deserialize(stream); 

                object result = new DataContractSerializer(typeof(SearchResultContract)).ReadObject(stream);
                return (SearchResultContract)result;
            }
        }
    }
}