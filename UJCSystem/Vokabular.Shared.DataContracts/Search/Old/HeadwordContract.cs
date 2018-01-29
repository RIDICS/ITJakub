using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.ITJakubService.DataContracts", Name = "HeadwordContract")]
    public class HeadwordContract
    {
        [DataMember]
        public string Headword { get; set; }
        
        [DataMember]
        public IList<HeadwordBookInfoContract> Dictionaries { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.ITJakubService.DataContracts", Name = "HeadwordBookInfoContract")]
    public class HeadwordBookInfoContract
    {
        [DataMember]
        public string EntryXmlId { get; set; }

        [DataMember]
        public string BookXmlId { get; set; }

        [DataMember]
        public string Image { get; set; }

        [DataMember]
        public long? PageId { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.ITJakubService.DataContracts", Name = "HeadwordListContract")]
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

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.ITJakubService.DataContracts", Name = "DictionaryContract")]
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