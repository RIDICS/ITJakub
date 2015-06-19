using System.Collections.Generic;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IEditori : IList<IEditor>
	{
		string Popis();
	}
}