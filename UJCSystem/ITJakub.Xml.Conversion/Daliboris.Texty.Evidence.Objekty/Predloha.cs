using System.Xml.Serialization;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	[XmlInclude(typeof(Edice))]
	[XmlInclude(typeof(Pramen))]
	[XmlInclude(typeof(Tisk))]
	[XmlInclude(typeof(Rukopis))]
	public class Predloha : IPredloha
	{
		private string mstrTypPredlohyText;
		private TypPredlohy mtpTyp;

		[XmlAttribute("typ")]
		public TypPredlohy Typ
		{
			get { return mtpTyp; }
			set { mtpTyp = value; /*mstrTypPredlohyText = TypPredlohyNaText(mtpTypPredlohy);*/ }
		}

		public virtual string Popis()
		{
			return "";
		}
		
	}
}
