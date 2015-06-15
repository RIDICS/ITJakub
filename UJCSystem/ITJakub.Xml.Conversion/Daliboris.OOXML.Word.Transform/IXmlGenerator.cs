
namespace Daliboris.OOXML.Word.Transform {
	interface IXmlGenerator {
		string SouborDocx { get; set; }
		string SouborDoc2Xml { get; set; }
		string SouborXml { get; set; }
		void Read();
	}
}
