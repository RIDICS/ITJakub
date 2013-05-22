using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Contracts.Categories
{
    [DataContract]
    [KnownType(typeof (Categorie))]
    [KnownType(typeof (Book))]
    public abstract class SelectionBase
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public abstract bool IsBook { get; }
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


        public override bool IsBook
        {
            get { return false; }
        }
    }

    [DataContract]
    public class Book : SelectionBase
    {
        public Book(string name, bool isRoot)
        {
            Name = name;
            IsRoot = isRoot;
        }


        [DataMember]
        public bool IsRoot { get; set; }

        public override bool IsBook
        {
            get { return true; }
        }
    }
}