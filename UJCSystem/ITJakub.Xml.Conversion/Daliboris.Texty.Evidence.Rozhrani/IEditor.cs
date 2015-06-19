namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IEditor
	{
		/// <summary>
		/// Jméno editora
		/// </summary>
		string Jmeno { get; set; }

		/// <summary>
		/// E-mail na editora
		/// </summary>
		string Email { get; set; }

		int CompareTo(IEditor other);
	}
}