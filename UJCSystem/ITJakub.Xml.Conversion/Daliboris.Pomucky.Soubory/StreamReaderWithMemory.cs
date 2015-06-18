using System.Text;
using System.IO;

namespace Daliboris.Pomucky.Soubory {
 public	class StreamReaderWithMemory : StreamReader {
		private string mstrCurrentLine;

		public StreamReaderWithMemory(Stream stream) : base(stream) {}
		public StreamReaderWithMemory(string path) : base(path) { }
		public StreamReaderWithMemory(Stream stream, bool detectEncodingFromByteOrderMarks) :
			base(stream, detectEncodingFromByteOrderMarks) { }
		public StreamReaderWithMemory( string path, bool detectEncodingFromByteOrderMarks) : 
			base(path, detectEncodingFromByteOrderMarks) { }
		public StreamReaderWithMemory(string path, Encoding encoding ) : 
			base(path, encoding) { }
		public StreamReaderWithMemory( Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : 
			base(stream, encoding, detectEncodingFromByteOrderMarks) { }
		public StreamReaderWithMemory(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks ) : 
			base(path, encoding, detectEncodingFromByteOrderMarks) { }
		public StreamReaderWithMemory(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize ) : 
			base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }
		public StreamReaderWithMemory( string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : 
			base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }

		public override string ReadLine() {
			mstrCurrentLine = base.ReadLine();
			return mstrCurrentLine;
		}

		/// <summary>
		/// Vrací text naposledy načteného řádku, aniž posouvá čtečku souboru.
		/// </summary>
		/// <returns>Vrací text naposledy načteného řádku</returns>
		public string CurrentLine {
			get {  return mstrCurrentLine; }
		}

	}
}
