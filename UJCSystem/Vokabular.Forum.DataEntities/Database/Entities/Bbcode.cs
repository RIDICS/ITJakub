using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class BBCode {
        public int BBCodeID { get; set; }
        public Board Board { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OnClickJS { get; set; }
        public string DisplayJS { get; set; }
        public string EditJS { get; set; }
        public string DisplayCSS { get; set; }
        public string SearchRegex { get; set; }
        public string ReplaceRegex { get; set; }
        public string Variables { get; set; }
        public bool? UseModule { get; set; }
        public string ModuleClass { get; set; }
        public int ExecOrder { get; set; }
    }
}
