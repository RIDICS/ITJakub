using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Daliboris.Pomucky.Texty;
using Daliboris.Pomucky.Xml;
using System.Reflection;
using System.Collections;
using Daliboris.Pomucky.Soubory;

namespace Daliboris.Slovniky {
	public class HeslarInfo {
		public string Id { get; set; } //r.GetAttribute("id");
		public string Nazev { get; set; }
	}
}
