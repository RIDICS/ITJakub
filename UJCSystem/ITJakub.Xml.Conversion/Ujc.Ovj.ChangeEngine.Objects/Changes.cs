using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
	[XmlRoot("changes")]
	public class Changes : Collection<Change>
	{

		public void Apply(ChangedToken changedToken)
		{
			string output = changedToken.Source.Correction ?? changedToken.Source.Text;

			changedToken.AppliedChanges = new Collection<AppliedChange>();


			foreach (Change change in this)
			{
				while (change.IsApplicable(output))
				{
					output = ApplyChangeOnToken(changedToken, output, change);
				}
			}

			changedToken.Text = output;
		}

		public static string ApplyChangeOnToken(ChangedToken changedToken, string output, Change change)
		{
			return ApplyChangeOnToken(changedToken, output, change, new Changes());
		}

		public static string ApplyChangeOnToken(ChangedToken changedToken, string output, Change change, IList<Change> exceptions)
		{
			string input = output;
			if (exceptions.Count == 0)
			{

				output = change.Apply(input);
				change.Exploitation++;
				changedToken.AppliedChanges.Add(new AppliedChange(input, output, change));
			}
			else
			{
				output = change.Apply(input, exceptions);
				
			}
			return output;
		}

		public string Apply(string input)
		{
			string output = input;
			foreach (Change change in this)
			{
				while (change.IsApplicable(output))
				{
					output = change.Apply(output);    
				}
				
			}

			return output;
		}

		public IList<string> Apply(IList<string> input)
		{
			IList<string> output = input;
			for (int i = 0; i < input.Count; i++)
			{
				foreach (Change change in this)
				{
					while (change.IsApplicable(output[i]))
					{
						output[i] = change.Apply(output[i]);
					}
				}

			}

			return output;
		}

		public IList<AppliedChange> GetAppliedChanges(string input)
		{
			List<AppliedChange> appliedChanges = new List<AppliedChange>();
			string output = input;
			foreach (Change change in this)
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

		public static Changes Load(string file)
		{
			FileManager manager = new FileManager();
			return manager.LoadChanges(file);
		}

		public static void Save(Changes changes, string file, bool indent)
		{
			FileManager manager = new FileManager();
			manager.SaveChanges(changes, file, true);
		}

		/// <summary>
		/// Sets Exploatation to 0 for each <see cref="Change"/> in collection.
		/// </summary>
		public void ClearExploitations()
		{
			foreach (Change change in this)
			{
				change.Exploitation = 0;
			}
		}

		public static Changes GetRelevantExceptions(Changes changes, string input)
		{
			Changes result = new Changes();
			foreach (Change change in changes)
			{
				if(change.IsApplicable(input))
					result.Add(change);
			}

			return result;
		}
	}
}
