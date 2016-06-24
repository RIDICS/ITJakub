using System.IO;
using Daliboris.OOXML.Pomucky;
using Ujc.Ovj.Editing.TextProcessor.MsWord.TemplateBuilding.Builders;

namespace Test
{
    public class CustomXmlPartTest
    {
        string folder = @"V:\MDM\Testy\Conversion";
        string abbreviation = @"Alph1718";
        public void TestAdding()
        {

            CustomXmlPartBuilderSettings settings = new CustomXmlPartBuilderSettings();
            settings.XmlPartFilePath = Path.Combine(folder, abbreviation + ".xml");
            settings.ContentTypeId = "CD052837-B125-4941-B89D-25B5995A92D6";
            settings.DataStoreItemId = "5AD717C3-0CB1-42C1-BEB6-34308825C1B6";
            settings.XsdFilePath = Path.Combine(folder, "TEI.xsd");
            CustomXmlPartBuilder builder = new CustomXmlPartBuilder(settings);

            builder.DocumentFilePath = Path.Combine(folder, abbreviation + ".docx");

            builder.Build();
        }

        public void TestExtracting()
        {
            string facsimileFolder = Path.Combine(folder, "Faksimile");
            string docxFile = Path.Combine(folder, abbreviation + ".docx");
            string facsimileFile = Path.Combine(facsimileFolder, abbreviation + ".xml");

            CustomXmlPartExtractorSettings settings = new CustomXmlPartExtractorSettings();
            settings.ContentTypeId = "CD052837-B125-4941-B89D-25B5995A92D6";
            settings.DataStoreItemId = "5AD717C3-0CB1-42C1-BEB6-34308825C1B6";
            settings.DocxFilePath = docxFile;
            settings.CustomXmlFilePath = facsimileFile;
            CustomXmlPartExtractor extractor = new CustomXmlPartExtractor(settings);
            extractor.Extract();
        }
    }
}