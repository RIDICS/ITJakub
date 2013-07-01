using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Contracts.Categories
{

    

    [DataContract]
    [KnownType(typeof(Category))]
    [KnownType(typeof(Book))]
    public class SelectionBase
    {
       
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Id { get; set; }

    
    }

    [DataContract]
    public class Category : SelectionBase
    {
        public Category()
        {
            Subitems = new List<SelectionBase>();
        }

        [DataMember]
        public string TextValue { get; set; }

        [DataMember]
        public List<SelectionBase> Subitems { get; set; }

        [DataMember]
        public CategoryShowType ShowType { get; set; }

        [DataMember]
        public bool HasChildren { get; set; }

        public Category Parrent { get; set; }
    }

    [DataContract]
    public class Book : SelectionBase
    {
        public Book()
        {
            TextCategoriesClassification = new List<string>();
        }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public List<string> TextCategoriesClassification { get; set; }

    }
}