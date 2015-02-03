namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel
{
    public class UpdateViewModel
    {
        public int SelectionStart { get; set; }

        public int SelectionLength { get; set; }

        public int CursorPosition { get; set; }

        protected bool Equals(UpdateViewModel other)
        {
            return SelectionStart == other.SelectionStart && SelectionLength == other.SelectionLength && CursorPosition == other.CursorPosition;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UpdateViewModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SelectionStart;
                hashCode = (hashCode*397) ^ SelectionLength;
                hashCode = (hashCode*397) ^ CursorPosition;
                return hashCode;
            }
        }
    }
}