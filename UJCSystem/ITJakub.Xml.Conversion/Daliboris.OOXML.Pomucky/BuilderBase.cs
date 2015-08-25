namespace Ujc.Ovj.Editing.TextProcessor.MsWord.TemplateBuilding
{
	public abstract class BuilderBase : IBuilder
	{
		protected BuilderBase()
		{
		}

		protected BuilderBase(object settings)
		{
			Settings = settings;
		}

		public string DocumentFilePath { get; set; }
		public object Settings { get; set; }
		public abstract void Build();
	}
}