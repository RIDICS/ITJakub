using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
	[XmlRoot(ElementName = "ruleset", Namespace = "http://vokabular.ujc.cas.cz/schema/changeEngine/v1")]
	public class ChangeRuleSet : SetBase
	{


		[XmlArray(ElementName = "exceptions", IsNullable = true)]
		[XmlArrayItem(ElementName = "exception")]
		public Changes Exceptions { get; set; }

		[XmlArray(ElementName = "changes", IsNullable = true)]
		[XmlArrayItem(ElementName = "change")]
		public Changes Changes { get; set; }

		public void Apply(ChangedToken changedToken)
		{
			string output = changedToken.Source.Correction ?? changedToken.Source.Text;

			changedToken.AppliedChanges = new Collection<AppliedChange>();

			int numberOfOccurencesChanges = NumberOfOccurences(Changes, output);

			if (Exceptions != null)
				foreach (Change exception in Exceptions)
				{
					if (exception.IsApplicable(output))
					{
						int numberOfOccurences = exception.NumberOfOccurences(output);
						output = Changes.ApplyChangeOnToken(changedToken, output, exception);
						changedToken.Text = output;
						if(numberOfOccurencesChanges == numberOfOccurences) return;
					}
				}

			IList<IChange> changes = new List<IChange>();
			if(Exceptions != null)
				foreach (Change change in Exceptions)
				{
					changes.Add(change);
				}
			if(Changes != null)
				foreach (Change change in Changes)
				{
					while (change.IsApplicable(output, changes))
					{
						Changes exceptions = Changes.GetRelevantExceptions(Exceptions, output);
						output = Changes.ApplyChangeOnToken(changedToken, output, change, exceptions);
					}
				}
			changedToken.Text = output;
		}

		private int NumberOfOccurences(Changes changes, string input)
		{
			int result = 0;
			foreach (Change change in changes)
			{
				result += change.NumberOfOccurences(input);
			}
			return result;
		}

		public List<string> Apply(List<string> input)
		{
			List<string> output = new List<string>(input.Count);

			foreach (string text in input)
			{
				output.Add(Apply(text));
			}

			return output;
		}

		public string Apply(string input)
		{
			string output = input;

			if (Exceptions != null)
			foreach (Change change in Exceptions)
			{
				if (change.IsApplicable(output))
				{
					output = change.Apply(output);
					return output;
				}
			}

			if(Changes != null)
			foreach (Change change in Changes)
			{
				while (change.IsApplicable(output))
				{
					output = change.Apply(output);
				}

			}

			return output;
		}

		public IList<AppliedChange> GetAppliedChanges(string input)
		{
			List<AppliedChange> appliedChanges = new List<AppliedChange>();
			string output = input;

			if (Changes != null)
			foreach (Change change in Changes)
			{
				while (change.IsApplicable(output))
				{
					AppliedChange applied = change.GetAppliedChange(output);
					appliedChanges.Add(applied);
					output = applied.Output;
				}
			}
			return appliedChanges;
		}

		public static ChangeRuleSet Load(string file)
		{
			FileManager manager = new FileManager();
			return manager.LoadChangeRuleSet(file);
		}

        public static ChangeRuleSet Load(Stream file)
		{
			FileManager manager = new FileManager();
			return manager.LoadChangeRuleSet(file);
		}

		public static void Save(ChangeRuleSet changes, string file, bool indent)
		{
			FileManager manager = new FileManager();
			manager.SaveChangeRuleSet(changes, file, true);
		}

		/// <summary>
		/// Sets Exploatation to 0 for each <see cref="Change"/> in collection.
		/// </summary>
		public void ClearExploitations()
		{
			foreach (Change change in Changes)
			{
				change.Exploitation = 0;
			}

			foreach (Change change in Exceptions)
			{
				change.Exploitation = 0;
			}

		}

	}
}