using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IIsbn
	{
		/// <summary>
		/// Přidělené ISBN
		/// </summary>
		string Cislo { get; set; }
		/// <summary>
		/// Formát, pro který bylo ISBN přiděleno (PDF, EPUB apod.)
		/// </summary>
		string Format { get; set; }
	}
}
