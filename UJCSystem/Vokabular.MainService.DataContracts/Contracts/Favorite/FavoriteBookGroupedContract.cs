using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts.Favorite
{
    public class FavoriteBookGroupedContract
    {
        public long Id { get; set; }

        public IList<FavoriteBaseWithLabelContract> FavoriteInfo { get; set; }
    }
}