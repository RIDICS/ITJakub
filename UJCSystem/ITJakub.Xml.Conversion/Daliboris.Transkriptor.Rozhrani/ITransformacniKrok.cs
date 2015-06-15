using Daliboris.Transkripce.Objekty;

namespace Daliboris.Transkripce
{
    public interface ITransformacniKrok
    {
        IPravidlo Pravidlo { get; set; }
        string Vstup { get; set; }
        string Vystup { get; set; }
    }
}