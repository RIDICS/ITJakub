namespace Ujc.Ovj.ChangeEngine.Objects
{
	public enum SpellcheckStatus
	{
		/// <summary>
		/// Výraz dosud nebyl zkontrolován
		/// </summary>
		Unchecked,
		/// <summary>
		/// Výraz byl kontrolou pravopisu vyhodnocen jako chybný
		/// </summary>
		Misspelled,

		/// <summary>
		/// Výraz byl kontrolou pravopisu vyhodnocen jako správný
		/// </summary>
		Correct,

		/// <summary>
		/// Výraz byl editorem vyhodnocen jako chybný
		/// </summary>
		RejectedByEditor,

		/// <summary>
		/// Výraz byl editorem vyohodnocen jako správný
		/// </summary>
		ApprovedByEditor
	}
}
