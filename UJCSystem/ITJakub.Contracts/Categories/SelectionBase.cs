using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Contracts.Categories
{
    [DataContract]
    [KnownType(typeof(Categorie))]
    [KnownType(typeof(Book))]
    public class SelectionBase
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Id { get; set; }

    
    }

    [DataContract]
    public class Categorie : SelectionBase
    {
        public Categorie()
        {
            Subitems = new List<SelectionBase>();
        }

        [DataMember]
        public List<SelectionBase> Subitems { get; private set; }

    }

    [DataContract]
    public class Book : SelectionBase
    {
    }
}