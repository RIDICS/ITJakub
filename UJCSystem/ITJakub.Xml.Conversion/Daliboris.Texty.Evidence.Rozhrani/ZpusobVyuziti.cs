using System;
using Campari.Software.Reflection;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	/// <summary>
	/// Informace o dalším využití přepsaného textu
	/// </summary>
	[Flags]
	public enum ZpusobVyuziti
	{
		/// <summary>
		/// Text určený pro projekt Manuscriptorium
		/// </summary>
		[EnumDescription("MNS")]
		Manuscriptorium = 1,
		/// <summary>
		/// Text určený pro staročeskou textovou banku, potažmo staročeský korpus
		/// </summary>
		[EnumDescription("STB")]
		StaroceskyKorpus = 2,
		/// <summary>
		/// Text určený pro středočeský korpus
		/// </summary>
		[EnumDescription("středočeský korpus")]
		StredoceskyKorpus = 4,

		/// <summary>
		/// Text určený pro ediční modul Vokabuláře webového
		/// </summary>
		[EnumDescription("EM")]
		EdicniModul = 8,

		/// <summary>
		/// Text určený pro elektronickou knihu
		/// </summary>
		[EnumDescription("e-kniha")]
		ElektronickaKniha = 16,


		/// <summary>
		/// Text určený pro Národní knihovnu, Z. Uhlíře
		/// </summary>
		[EnumDescription("NK")]
		NarodniKnihovna = 32,

		/// <summary>
		/// Text, nebo jeho úryvek, určený pro audioknihu
		/// </summary>
		[EnumDescription("Audio")]
		Audiokniha = 64,

		/// <summary>
		/// Text zpracovaný ve formě slovníku (pro modul slovníků)
		/// </summary>
		[EnumDescription("Slovník")]
		Slovnik = 128,

		/// <summary>
		/// Text určený pro modul odborné literatury
		/// </summary>
		[EnumDescription("MOL")]
		OdbornaLiteratura = 256,

        /// <summary>
		/// Text určený pro modul digitalizovaných mluvnic
		/// </summary>
		[EnumDescription("MDM")]
        Mluvnice = 512,

        /// <summary>
        /// Text určený pro ediční modul transliterovaných textů
        /// </summary>
        [EnumDescription("Transliterace")]
        Transliterace = 1024,

        /// <summary>
        /// Text určený pro interní textovou banku
        /// </summary>
        [EnumDescription("ITB")]
		InterniKorpus = 16384
	}
}