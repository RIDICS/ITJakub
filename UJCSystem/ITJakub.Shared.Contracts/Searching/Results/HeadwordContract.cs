using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts // TODO fix namespace together with change xquery
{
    [DataContract]
    public class HeadwordSearchResultContract
    {
        [DataMember]
        public int HeadwordCount { get; set; }

        [DataMember]
        public int FulltextCount { get; set; }
    }

    [DataContract]
    public class HeadwordContract
    {
        [DataMember]
        public string Headword { get; set; }
        
        [DataMember]
        public IList<HeadwordBookInfoContract> Dictionaries { get; set; }
    }

    [DataContract]
    public class HeadwordBookInfoContract
    {
        [DataMember]
        public string EntryXmlId { get; set; }

        [DataMember]
        public string BookXmlId { get; set; }

        [DataMember]
        public string Image { get; set; }
    }

    [DataContract]
    public class HeadwordListContract
    {
        [DataMember]
        public IDictionary<string, DictionaryContract> BookList { get; set; }

        [DataMember]
        public IList<HeadwordContract> HeadwordList { get; set; }

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

        public static HeadwordListContract FromXml(string xml)
        {
            using (Stream stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(xml);
                writer.Flush();

                stream.Position = 0;
                //object result = new XmlSerializer(typeof (Server)).Deserialize(stream); 

                object result = new DataContractSerializer(typeof(HeadwordListContract)).ReadObject(stream);
                return (HeadwordListContract)result;
            }
        }
    }

    [DataContract]
    public class DictionaryContract
    {
        [DataMember]
        public string BookXmlId { get; set; }

        [DataMember]
        public long BookId { get; set; }

        [DataMember]
        public string BookTitle { get; set; }

        [DataMember]
        public long BookVersionId { get; set; }

        [DataMember]
        public string BookVersionXmlId { get; set; }

        [DataMember]
        public string BookAcronym { get; set; }
    }
}