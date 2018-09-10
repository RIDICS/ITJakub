using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Board {
        public Board() {
			AccessMasks = new List<AccessMask>();
			Actives = new List<Active>();
			BannedEmails = new List<BannedEmail>();
			BannedIPs = new List<BannedIP>();
			BannedNames = new List<BannedName>();
			BBCodes = new List<BBCode>();
			Categories = new List<Category>();
			Extensions = new List<Extension>();
			Groups = new List<Group>();
			NntpServers = new List<NntpServer>();
			Ranks = new List<Rank>();
			Registries = new List<Registry>();
			Smileys = new List<Smiley>();
			Users = new List<User>();
        }
        public int BoardID { get; set; }
        public string Name { get; set; }
        public bool AllowThreaded { get; set; }
        public string MembershipAppName { get; set; }
        public string RolesAppName { get; set; }
        public IList<AccessMask> AccessMasks { get; set; }
        public IList<Active> Actives { get; set; }
        public IList<BannedEmail> BannedEmails { get; set; }
        public IList<BannedIP> BannedIPs { get; set; }
        public IList<BannedName> BannedNames { get; set; }
        public IList<BBCode> BBCodes { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<Extension> Extensions { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<NntpServer> NntpServers { get; set; }
        public IList<Rank> Ranks { get; set; }
        public IList<Registry> Registries { get; set; }
        public IList<Smiley> Smileys { get; set; }
        public IList<User> Users { get; set; }
    }
}
