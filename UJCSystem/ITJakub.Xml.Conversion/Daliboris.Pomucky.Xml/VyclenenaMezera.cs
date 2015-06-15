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
		/// Zanoøení elementu ve struktuøe XML
		/// </summary>
		public int Depth { get; set; }

		/// <summary>
		/// Zda se má zapsat mezera do výstupu (tj. pøedchozí textový element obsahoval mezeru a je potøeba jej pøesunout
		/// </summary>
		public bool ZapsatMezeru { get; set; }

		/// <summary>
		/// Názel elementu, kde se mezera vyskytla
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Informace o mezeøe byla souèástí atributu
		/// </summary>
		public bool JakoAtribut { get; set; }

		public bool JePreskocenTag { get; set; }
	}
}