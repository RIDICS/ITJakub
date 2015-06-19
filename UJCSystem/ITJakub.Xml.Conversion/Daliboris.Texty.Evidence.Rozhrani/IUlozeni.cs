namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IUlozeni
	{
		/// <summary>
		/// Země uložení rukopisu/tisku
		/// </summary>
		string Zeme { get; set; }

		/// <summary>
		/// Město uložení rukopisu/tisku
		/// </summary>
		string Mesto { get; set; }

		/// <summary>
		/// Instituce uložení rukopisu/tisku
		/// </summary>
		string Instituce { get; set; }

		/// <summary>
		/// Signatura rukopisu/tisku
		/// </summary>
		string Signatura { get; set; }

		/// <summary>
		/// Foliace/paginace rukopisu/tisku
		/// </summary>
		string FoliacePaginace { get; set; }

		string Popis();
	}
}