namespace Daliboris.Pomucky.Xml
{
	internal class VyclenenaMezera
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public VyclenenaMezera(int depth)
		{
			Depth = depth;
			ZapsatMezeru = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public VyclenenaMezera(int depth, string tag)
			: this(depth)
		{
			Tag = tag;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public VyclenenaMezera(int depth, string tag, bool jakoAtribut) : this(depth, tag)
		{
			JakoAtribut = jakoAtribut;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public VyclenenaMezera()
		{
		}

		/// <summary>
		/// Zano�en� elementu ve struktu�e XML
		/// </summary>
		public int Depth { get; set; }

		/// <summary>
		/// Zda se m� zapsat mezera do v�stupu (tj. p�edchoz� textov� element obsahoval mezeru a je pot�eba jej p�esunout
		/// </summary>
		public bool ZapsatMezeru { get; set; }

		/// <summary>
		/// N�zel elementu, kde se mezera vyskytla
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Informace o meze�e byla sou��st� atributu
		/// </summary>
		public bool JakoAtribut { get; set; }

		public bool JePreskocenTag { get; set; }
	}
}