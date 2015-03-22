﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class BookInfoContract
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string SubTitle { get; set; }
        
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string PublishDate { get; set; }

        [DataMember]
        public string PublishPlace { get; set; }

        [DataMember]
        public IList<BookPageContract> BookPages { get; set; }
    }
}