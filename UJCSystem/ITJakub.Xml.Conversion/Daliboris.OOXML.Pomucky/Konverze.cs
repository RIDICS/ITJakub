using System;
using System.Text;
using System.IO;
using DIaLOGIKa.b2xtranslator.StructuredStorage;
using DIaLOGIKa.b2xtranslator.DocFileFormat;
using DIaLOGIKa.b2xtranslator.OpenXmlLib.WordprocessingML;
using DIaLOGIKa.b2xtranslator.WordprocessingMLMapping;
using DIaLOGIKa.b2xtranslator.ZipUtils;
using DIaLOGIKa.b2xtranslator.Tools;
using DIaLOGIKa.b2xtranslator.Shell;
using System.Xml;
using System.Globalization;
using DIaLOGIKa.b2xtranslator.StructuredStorage.Reader;
using DIaLOGIKa.b2xtranslator.OpenXmlLib;

namespace Daliboris.OOXML.Pomucky {
	public static class Konverze {
		private static string inputFile;
		private static string outputFile;

		public static bool Doc2Docx(string strVstup, string strVystup) {
			bool bUspech = false;
			System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
			try {
				//copy processing file
				inputFile = strVstup;
				outputFile = strVystup;
				ProcessingFile procFile = new ProcessingFile(inputFile);
				//make output file name
				if (outputFile == null) {
					if (inputFile.Contains(".")) {
						outputFile = inputFile.Remove(inputFile.LastIndexOf(".")) + ".docx";
					}
					else {
						outputFile = inputFile + ".docx";
					}
				}

				TraceLogger.Info("Converting file {0} into {1}", inputFile, outputFile);

				//start time
				DateTime start = DateTime.Now;

				using (StructuredStorageReader reader = new StructuredStorageReader(procFile.File.FullName)) {
					//parse the input document
					WordDocument doc = new WordDocument(reader);

					//prepare the output document
					OpenXmlPackage.DocumentType outType = Converter.DetectOutputType(doc);
					//string conformOutputFile = Converter.GetConformFilename(outputFile, outType);
					//WordprocessingDocument docx = WordprocessingDocument.Create(conformOutputFile, outType);
					WordprocessingDocument docx = WordprocessingDocument.Create(outputFile, outType);


					//start time
					//DateTime start = DateTime.Now;
					TraceLogger.Info("Converting file {0} into {1}", inputFile, outputFile);

					//convert the document
					Converter.Convert(doc, docx);

					DateTime end = DateTime.Now;
					TimeSpan diff = end.Subtract(start);
					TraceLogger.Info("Conversion of file {0} finished in {1} seconds", inputFile, diff.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				}
				bUspech = true;
			}
			/*
//parse the document
WordDocument doc = new WordDocument(reader);
																								
if (!doc.FIB.fComplex) {

using (WordprocessingDocument docx = WordprocessingDocument.Create(outputFile, WordprocessingDocumentType.Document)) {
//Setup the writer
XmlWriterSettings xws = new XmlWriterSettings();
xws.OmitXmlDeclaration = false;
xws.CloseOutput = true;
xws.Encoding = Encoding.UTF8;
xws.ConformanceLevel = ConformanceLevel.Document;

//Setup the context
ConversionContext context = new ConversionContext(doc);
context.WriterSettings = xws;
context.Docx = docx;

//Write styles.xml
doc.Styles.Convert(new StyleSheetMapping(context));

//Write numbering.xml
doc.ListTable.Convert(new NumberingMapping(context));

//Write fontTable.xml
doc.FontTable.Convert(new FontTableMapping(context));

//write document.xml and the header and footers
doc.Convert(new MainDocumentMapping(context));

//write the footnotes
doc.Convert(new FootnotesMapping(context));

//write settings.xml at last because of the rsid list
doc.DocumentProperties.Convert(new SettingsMapping(context));
}

DateTime end = DateTime.Now;
TimeSpan diff = end.Subtract(start);
//TraceLogger.Info("Conversion of file {0} finished in {1} seconds", inputFile, diff.TotalSeconds.ToString(CultureInfo.InvariantCulture));
				*/



			catch (DirectoryNotFoundException ex) {
				TraceLogger.Error("The input file does not exist.");
				TraceLogger.Debug(ex.ToString());
			}
			catch (FileNotFoundException ex) {
				TraceLogger.Error("The input file does not exist.");
				TraceLogger.Debug(ex.ToString());
			}
			/*
catch (ReadBytesAmountMismatchException ex) {
TraceLogger.Error("The input file is not a valid Microsoft Word 97-2003 file.");
TraceLogger.Debug(ex.ToString());
}
catch (MagicNumberException ex) {
TraceLogger.Error("The input file is not a valid Microsoft Word 97-2003 file.");
TraceLogger.Debug(ex.ToString());
}
				*/
			catch (UnspportedFileVersionException ex) {
				TraceLogger.Error("Doc2x doesn't support files older than Word 97.");
				TraceLogger.Debug(ex.ToString());
			}
			catch (ByteParseException ex) {
				TraceLogger.Error("The input file is not a valid Microsoft Word 97-2003 file.");
				TraceLogger.Debug(ex.ToString());
			}
			catch (ZipCreationException ex) {
				TraceLogger.Error("Could not create output file {0}.", outputFile);
				//TraceLogger.Error("Perhaps the specified outputfile was a directory or contained invalid characters.");
				TraceLogger.Debug(ex.ToString());
			}
			catch (Exception ex) {
				TraceLogger.Error("Conversion failed.");
				TraceLogger.Debug(ex.ToString());
			}
			finally {
				System.Threading.Thread.CurrentThread.CurrentCulture = ci;
			}
			return bUspech;
		}
	}
}
