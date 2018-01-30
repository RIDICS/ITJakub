namespace Vokabular.DataEntities.Database.Entities
{
    public class HeadwordItem
    {
        public virtual long Id { get; set; }
        public virtual string Headword { get; set; }
        public virtual string HeadwordOriginal { get; set; }
        public virtual Resource ResourcePage { get; set; }
        public virtual HeadwordResource HeadwordResource { get; set; }
    }
}