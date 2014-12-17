// -----------------------------------------------------------------------
// <copyright file="AttributeInfo.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Ujc.Ovj.Xml.Tei.Splitting {
 using System;
 using System.Collections.Generic;
 using System.Text;

 /// <summary>
 /// TODO: Update summary.
 /// </summary>
 public class AttributeInfo {

 	public AttributeInfo()
 	{
 		
 	}

	public AttributeInfo(string prefix, string localName, string namespaceUri, string value) : this(localName, value)
	{
		Prefix = prefix;
		NamespaceUri = namespaceUri;
	}

 	public AttributeInfo(string localName, string value)
 	{
 		LocalName = localName;
 		Value = value;
 	}

 	public string Prefix { get; set; }
 	public string NamespaceUri { get; set; }
 	public string LocalName { get; set; }
 	public string Value { get; set; }

	//reader.Prefix, reader.LocalName, reader.NamespaceURI

 }
}
