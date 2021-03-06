﻿using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Data
{
    public class CategoryData
    {
        public string Description { get; set; }
        public string XmlId { get; set; }
        //public CategoryData ParentCategory { get; set; }
        public List<CategoryData> SubCategories { get; set; }
    }
}