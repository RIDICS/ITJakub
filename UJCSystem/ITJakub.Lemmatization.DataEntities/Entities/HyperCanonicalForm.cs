using System.Collections.Generic;

namespace ITJakub.Lemmatization.DataEntities.Entities
{
    public class HyperCanonicalForm
    {
        public virtual long Id { get; protected set; }

        public virtual string Text { get; set; }

        public virtual string Description { get; set; }

        public virtual HyperCanonicalFormType Type { get; set; }

        public virtual IList<CanonicalForm> CanonicalForms { get; set; }
    }

    public enum HyperCanonicalFormType : short
    {
        HyperLemma = 0,
        HyperStemma = 1,
    }
}