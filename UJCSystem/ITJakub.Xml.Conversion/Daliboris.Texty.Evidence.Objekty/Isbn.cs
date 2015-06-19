using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Isbn : IIsbn
	{


	  public Isbn() {

	  }

	  public Isbn(string cislo, string format) {
		Cislo = cislo;
		Format = format;
	  }
		public string Cislo { get; set; }

		public string Format { get; set; }
		
	}
}
