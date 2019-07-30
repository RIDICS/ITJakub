using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Medal {
        public Medal() {
			GroupMedals = new List<GroupMedal>();
			UserMedals = new List<UserMedal>();
        }
        public int MedalID { get; set; }
        public int BoardID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public string MedalURL { get; set; }
        public string RibbonURL { get; set; }
        public string SmallMedalURL { get; set; }
        public string SmallRibbonURL { get; set; }
        public short SmallMedalWidth { get; set; }
        public short SmallMedalHeight { get; set; }
        public short? SmallRibbonWidth { get; set; }
        public short? SmallRibbonHeight { get; set; }
        public byte SortOrder { get; set; }
        public int Flags { get; set; }
        public IList<GroupMedal> GroupMedals { get; set; }
        public IList<UserMedal> UserMedals { get; set; }
    }
}
