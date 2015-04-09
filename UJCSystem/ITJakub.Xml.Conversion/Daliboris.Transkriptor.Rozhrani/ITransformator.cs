using System;
using System.Runtime.InteropServices;
namespace Daliboris.Transkripce {
	[ComVisible(true)]
	[Guid("DAD60851-E3EB-4BD3-B175-50EA14E8E917")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ITransformator {
		string AplikujPravidla(string strText, string strJazyk);
		void ImportujPravidla(string strSoubor);
	}
}
