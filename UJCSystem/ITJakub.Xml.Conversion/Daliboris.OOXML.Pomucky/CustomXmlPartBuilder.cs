using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.CustomXmlDataProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Ujc.Ovj.Editing.TextProcessor.MsWord.TemplateBuilding.Builders
{
	public class CustomXmlPartBuilder : BuilderBase
	{
		public CustomXmlPartBuilder() : base()
		{
		}

		public CustomXmlPartBuilder(object settings) : base(settings)
		{
		}

		private CustomXmlPartBuilderSettings TypedSetting {
			get { return Settings as CustomXmlPartBuilderSettings; }
		}
		public override void Build()
		{
			AddOrReplaceCustomxmlPath();
		}

		private void AddOrReplaceCustomxmlPath()
		{
			CheckIfTypedSettingIsNull();

			using (WordprocessingDocument document = WordprocessingDocument.Open(DocumentFilePath, true))
			{

				string dataStoreItemId = TypedSetting.DataStoreItemId;
				CustomXmlPart xmlPart = GetCustomXmlPart(document, dataStoreItemId);

				if (xmlPart != null)
				{
					FileStream fs = new FileStream(TypedSetting.XmlPartFilePath, FileMode.Open);
					xmlPart.FeedData(fs);
				}
				else
				{
					xmlPart = document.MainDocumentPart.AddCustomXmlPart(CustomXmlPartType.CustomXml);
					FileStream fs = new FileStream(TypedSetting.XmlPartFilePath, FileMode.Open);
					xmlPart.FeedData(fs);

					CustomXmlPropertiesPart xmlPropertiesPart = xmlPart.AddNewPart<CustomXmlPropertiesPart>(TypedSetting.ContentTypeId);
					DataStoreItem dataStoreItem = new DataStoreItem();
					dataStoreItem.ItemId = dataStoreItemId;
					xmlPropertiesPart.DataStoreItem = dataStoreItem;

				}
			}
		}

		private static CustomXmlPart GetCustomXmlPart(WordprocessingDocument document, string dataStoreItemId)
		{
			CustomXmlPart xmlPart = null;
			List<CustomXmlPart> xmlParts = document.MainDocumentPart.CustomXmlParts.ToList();


			foreach (CustomXmlPart customXmlPart in xmlParts)
			{
				if (customXmlPart.CustomXmlPropertiesPart != null &&
				    customXmlPart.CustomXmlPropertiesPart.DataStoreItem != null &&
				    customXmlPart.CustomXmlPropertiesPart.DataStoreItem.ItemId == dataStoreItemId)
					xmlPart = customXmlPart;
			}
			return xmlPart;
		}

		private void CheckIfTypedSettingIsNull()
		{
			if (TypedSetting == null) throw new NullReferenceException();
		}

		private XDocument GetXDocument(CustomXmlPart customXmlPart)
		{
			XDocument document = XDocument.Load(customXmlPart.GetStream());
			return document;
		}
	}
}