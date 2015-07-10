using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using ITJakub.Shared.Contracts.Searching;

namespace ITJakub.SearchService.Core.Exist
{
    [DataContract]
    public class ResultSearchCriteriaContract
    {
        [DataMember]
        public IList<BookVersionPairContract> ResultBooks { get; set; }

        [DataMember]
        public IList<SearchCriteriaContract> SearchCriterias { get; set; }


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
    }
}