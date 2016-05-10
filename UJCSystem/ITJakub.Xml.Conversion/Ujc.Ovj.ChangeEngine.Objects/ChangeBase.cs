using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
	/// <summary>
	/// Base class for applying changes.
	/// </summary>
	public class ChangeBase : Expression, IChange
	{
		
		


		public ChangeBase(string pattern, string replace, ChangeFormat format)
			: this(pattern, replace, format, new ChangeOptions())
		{
		}

		public ChangeBase(string pattern, string replace, ChangeFormat format, ChangeOptions options)
		{
			Pattern = pattern;
			Replace = replace;
			Format = format;
			Options = options;
		}


	   
		/// <summary>
		/// How is pattern replaced in input text, if it is founded.
		/// </summary>
		[XmlAttribute("replace")]
		public string Replace { get; set; }


		/// <summary>
		/// Option applied during change.
		/// </summary>
		[XmlElement("options")]
		public ChangeOptions Options { get; set; }


		public override bool IsApplicable(string input)
		{
			int numberOfOccurences = NumberOfOccurences(input);
			if (numberOfOccurences == 0) return false;
			if (Options.Exceptions != null)
			{
				foreach (Expression expression in Options.Exceptions)
				{
					if (expression.IsApplicable(input))
					{
						if(expression.NumberOfOccurences(input) >= numberOfOccurences)
						return false;
					}
						
				}
			}
			return true;
		}

		/// <summary>
		/// Applies change defined by <see cref="Replace"/> to input string.
		/// </summary>
		/// <param name="input">String on which change is applied.</param>
		/// <returns>Returns new version of string if <see cref="Pattern"/> in <see cref="input"/> string is found.</returns>
		/// <remarks>Applied changes depends on <see cref="Options"/> defined for change.</remarks>
		public virtual string Apply(string input)
		{
			if (!IsApplicable(input)) return input;
			switch (Format)
			{
				case ChangeFormat.String:
					return ApplyString(input, Options);
				case ChangeFormat.RegularExpression:
					return ApplyRegex(input, Options);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Applies change defined by <see cref="ChangeBase.Replace"/> to input string excluding occurences in exceptions.
		/// </summary>
		/// <param name="input">String on which change is applied.</param>
		/// <param name="exceptions">Exceptions where pattern cannot be applied.</param>
		/// <returns>Returns new version of string if <see cref="ChangeBase.Pattern"/> in <see cref="input"/> string is found.</returns>
		/// <remarks>Applied changes depends on <see cref="ChangeBase.Options"/> defined for change.</remarks>
		public string Apply(string input, IList<Change> exceptions)
		{
			if (!IsApplicable(input)) return input;
			if (exceptions == null || exceptions.Count == 0) return Apply(input);

			switch (Format)
			{
				case ChangeFormat.String:
					return ApplyString(input, Options, exceptions);
				case ChangeFormat.RegularExpression:
					return ApplyRegex(input, Options, exceptions);
				default:
					throw new ArgumentOutOfRangeException();
			}

		}

		/// <summary>
		/// Applies change in regular expression format to input string.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		private string ApplyRegex(string input, ChangeOptions options)
		{
			return ApplyRegex(input, options, new Changes());
		}

		/// <summary>
		/// Applies change in regular expression format to input string.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		/// <param name="exceptions"></param>
		/// <returns></returns>
		private string ApplyRegex(string input, ChangeOptions options, IList<Change> exceptions)
		{

			if (!IsReplacementApplicable()) return input; //nedochází k žádné změně

			//pravidla počtu nahrazení pro Regex: -1 = všechny výskyty; 0 = žádný výskyt; 1 a více = zadaný počet výskytů
			//pravdila Cahnge: null, 0 = žádný výskyt

			string output = input;
			int replacementCount = GetReplacementCount(options);

			Occurrences exceptionMatches = ExceptionMatches(input, exceptions);
			if (exceptionMatches.Count == 0)
				return base.Regex.Replace(input, Replace, replacementCount);


			MatchCollection patternMatches = base.Regex.Matches(input);
			int count = 0;
			output = input;
			foreach (Match match in patternMatches)
			{
				Occurrence occurrence = new Occurrence(match.Index, match.Index + match.Length);
				if (!exceptionMatches.IsOccurrenceWithin(occurrence))
				{
					output = base.Regex.Replace(output, Replace, 1, match.Index);
					count++;
				}
				if (count == options.ReplacementCount) return output;
			}

			return output;
		}

		private static int GetReplacementCount(ChangeOptions options)
		{
			int replacementCount = -1;
			if (options.ReplacementCount.HasValue)
				replacementCount = options.ReplacementCount.Value;
			return replacementCount;
		}

		private static Occurrences ExceptionMatches(string input, IList<Change> exceptions)
		{
			Occurrences exceptionMatches = new Occurrences();
			if (exceptions == null || (exceptions.Count == 0)) return exceptionMatches;
			foreach (Change exception in exceptions)
			{
				foreach (Occurrence pointer in exception.Occurrences(input))
				{
					exceptionMatches.Add(pointer);
				}
			}
			return exceptionMatches;
		}

		private static Occurrences ExceptionMatches(string input, IList<Expression> exceptions)
		{
			Occurrences exceptionMatches = new Occurrences();
			if (exceptions == null || (exceptions.Count == 0)) return exceptionMatches;
			foreach (Expression exception in exceptions)
			{
				foreach (Occurrence pointer in exception.Occurrences(input))
				{
					exceptionMatches.Add(pointer);
				}
			}
			return exceptionMatches;
		}

		private bool IsReplacementApplicable()
		{
			if(Replace == null || Options.ReplacementCount.HasValue && Options.ReplacementCount.Value < 0) return false;
			return true;
		}

		/// <summary>
		/// Applies change to input string as simple string.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		private string ApplyString(string input, ChangeOptions options)
		{
			return ApplyString(input, options, new Changes());
		}

		private string ApplyString(string input, ChangeOptions options, IList<Change> exceptions)
		{
			string output = input;
			
			if (!IsReplacementApplicable()) return output;
			
			Occurrences exceptionMatches = ExceptionMatches(input, exceptions);
			
			if(options.Exceptions != null && options.Exceptions.Count > 0)
			{
				Occurrences innerExceptionMatches = ExceptionMatches(input, options.Exceptions);
				if (innerExceptionMatches.Count > 0)
				{
					exceptionMatches.AddRange(innerExceptionMatches);
				}
			}

			int replacementCount = GetReplacementCount(options);
			
			if (replacementCount== 0)
				return output;
			

			if (exceptionMatches.Count == 0)
			{
				if(replacementCount == -1)
					output = input.Replace(Pattern, Replace);
				else
				{
					int count = replacementCount;
					output = Output(options, count, exceptionMatches, output);
				}
			}
			else
			{
				
				
					int count = (replacementCount == -1) ? input.Length : replacementCount;
				output = Output(options, count, exceptionMatches, output);
				
			}

			return output;
		}

		private string Output(ChangeOptions options, int count, Occurrences exceptionMatches, string output)
		{
			int start = 0;
			for (int i = 0; i < count; i++)
			{
				int loc = output.IndexOf(Pattern, start, StringComparison.Ordinal);
				Occurrence occurrence = new Occurrence(loc, loc + Pattern.Length);

				while (exceptionMatches.IsOccurrenceWithin(occurrence))
				{
					loc = output.IndexOf(Pattern, loc + 1, StringComparison.Ordinal);
					occurrence = new Occurrence(loc, loc + Pattern.Length);
				}
				if (loc == -1) return output;

				output = ReplaceFirstOccurrence(output, Pattern, Replace, options, loc);
				start = loc + 1;
			}
			return output;
		}

		public static string ReplaceFirstOccurrence(string original, string oldValue, string newValue, ChangeOptions options)
		{
			return ReplaceFirstOccurrence(original, oldValue, newValue, options, 0);
		}

		public static string ReplaceFirstOccurrence(string original, string oldValue, string newValue, 
			ChangeOptions options, int startIndex)
		{
			if (String.IsNullOrEmpty(original))
				return String.Empty;
			if (String.IsNullOrEmpty(oldValue))
				return original;
			if (String.IsNullOrEmpty(newValue))
				newValue = String.Empty;
			int loc = original.IndexOf(oldValue, startIndex, StringComparison.Ordinal);
			if (loc == -1) return original;

			return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
		}
	}
}
