using System;
using System.Diagnostics;

namespace Daliboris.Pomucky.Xml
{
	[DebuggerDisplay("Tag: {CurrentElementInfo}, Depth: {Depth}, Space: {EndsWithSpace}, As attribute: {SpaceAsAttribute}")]
	public class Space : ICloneable
	{

		#region Constructors

		public Space(int depth)
		{
			Depth = depth;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public Space(int depth, string text)
		{
			Text = text;
			Depth = depth;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public Space(int depth, string text, string currentTag)
		{
			Text = text;
			Depth = depth;
			CurrentTag = currentTag;
		}


		public Space(string text, XmlElementInfo currentTag)
		{
			Text = text;
			Depth = currentTag.Depth;
			CurrentTag = currentTag.Name;
			CurrentElementInfo = currentTag;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public Space(int depth, bool spaceAsAttribute, string currentTag)
		{
			Depth = depth;
			SpaceAsAttribute = spaceAsAttribute;
			CurrentTag = currentTag;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public Space(bool spaceAsAttribute, XmlElementInfo currentTag)
		{
			Depth = currentTag.Depth;
			SpaceAsAttribute = spaceAsAttribute;
			CurrentTag = currentTag.Name;
			CurrentElementInfo = currentTag;
		}

		#endregion



		#region Properties

		public string Text { get; set; }

		public bool EndsWithSpace
		{
			get
			{
				if (IsSpaceSet) return true;
				if(SpaceInvalidated) return false;
				if (SpaceAsAttribute) return true;
				if (String.IsNullOrEmpty(Text)) return false;
				return (Text[Text.Length - 1] == ' ');
			}
		}

		/// <summary>
		/// Text bez koncové mezery.
		/// </summary>
		public string TextWithoutTrailingSpace
		{
			get
			{
				if (EndsWithSpace)
				{
					if (SpaceAsAttribute || IsSpaceSet) return Text;
					return Text.Remove(Text.Length - 1);
				}
				else
				{
					return Text;
				}
			}
		}

		public int Depth { get; set; }
		public string ParentTag { get; set; }
		public string CurrentTag { get; set; }
		public XmlElementInfo CurrentElementInfo { get; set; }
		public bool SpaceAsAttribute { get; set; }
		public bool SpaceInvalidated { get; private set; }
		private bool IsSpaceSet { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Pokud byla koncová mezera použita v textu, anuluje se její existence,
		/// aby se nemohla vyskytnout navíc u nějakého následujícího elementu.
		/// </summary>
		public void InvalidateTrailingSpace()
		{
			IsSpaceSet = false;
			SpaceInvalidated = true;
		}

		public void ValidateTrailingSpace()
		{
			SpaceInvalidated = false;
		}

		/// <summary>
		/// Sets EndsWithSpace to true, even if there is no trailing space.
		/// </summary>
		public void SetTrailingSpace(bool value)
		{
			IsSpaceSet = value;
		}

		#endregion

		#region Helpers

		#endregion

		private Space CloneImpl()
		{
			return MemberwiseClone() as Space;
		}

		public Space Clone()
		{
			return CloneImpl();
		}

		#region Implementation of ICloneable

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		object ICloneable.Clone()
		{
			return CloneImpl();
		}

		#endregion
	}
}