using System;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Editor: IComparable<IEditor>, IEditor
	{
		/// <summary>
		/// Jméno editora
		/// </summary>
		public string Jmeno { get; set; }

		/// <summary>
		/// E-mail na editora
		/// </summary>
		public string Email { get; set; }

		#region IComparable<Editor> Members

		public int CompareTo(IEditor other)
		{
			return String.Compare(this.Jmeno, other.Jmeno);
		}

		#endregion

	}
}
