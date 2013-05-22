using System.Collections.Generic;

namespace ITJakub.Contracts.Categories
{
    public class SelectionBase
    {
        public string Name {get;set;}
        public string Id { get; set; }
    }

    public class Categorie : SelectionBase
    {
        public List<SelectionBase> Subitems { get; set; }
    }
}