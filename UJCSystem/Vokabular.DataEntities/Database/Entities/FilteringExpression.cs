using System;

namespace Vokabular.DataEntities.Database.Entities
{
    public class FilteringExpression : IEquatable<FilteringExpression>
    {
        public virtual int Id { get; set; }

        public virtual string Field { get; set; }

        public virtual string Value { get; set; }

        public virtual FilteringExpressionSet FilteringExpressionSet { get; set; }

        public virtual bool Equals(FilteringExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FilteringExpression) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}