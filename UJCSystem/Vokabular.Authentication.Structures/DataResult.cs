namespace Vokabular.Authentication.Structures
{
    public class DataResult<T>
    {
        public T Result { get; set; }

        public bool HasError { get; set; }

        public string Error { get; set; }
    }
}