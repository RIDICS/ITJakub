namespace Vokabular.ProjectParsing.Model.Entities
{
    public class PairIdValue
    {
        public PairIdValue(string id, string value)
        {
            Id = id;
            Value = value;
        }

        public string Id { get; set; }

        public string Value { get; set; }
    }
}
