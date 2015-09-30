namespace Ujc.Ovj.Editing.TextProcessor.MsWord.TemplateBuilding
{
	public interface IBuilder
	{
		string DocumentFilePath { get; set; }

		object Settings { get; set; }

		void Build();
	}
}