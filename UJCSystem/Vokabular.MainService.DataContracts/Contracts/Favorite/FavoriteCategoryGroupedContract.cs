using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts.Favorite
{
    public class FavoriteCategoryGroupedContract
    {
        public int Id { get; set; }

        public IList<FavoriteBaseWithLabelContract> FavoriteInfo { get; set; }
    }
}
