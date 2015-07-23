using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;

namespace ITJakub.SearchService.Core.Search.DataContract
{
    [DataContract]
    public class ResultSearchCriteriaContract
    {
        [DataMember]
        public IList<BookVersionPairContract> ResultBooks { get; set; }

        [DataMember]
        public IList<SearchCriteriaContract> ConjunctionSearchCriterias { get; set; }

        [DataMember]
        public ResultCriteriaContract ResultSpecifications { get; set; }


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