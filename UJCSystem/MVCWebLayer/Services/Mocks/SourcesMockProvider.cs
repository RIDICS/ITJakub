using System;
using System.Collections.Generic;
using ITJakub.MVCWebLayer.Enums;
using ITJakub.MVCWebLayer.Services.DTOs;

namespace ITJakub.MVCWebLayer.Services.Mocks
{
    public class MockSourcesProvider : ISourcesProvider
    {
        public IEnumerable<Source> GetSearchResult()
        {
            return new List<Source>
                {
                    { new Source {Title = "Bible olomoucká, Genesis–Esdráš", Author = "", Datation = "1417", Perex = "Elektronická edice Olomoucké bible vznikla naskenováním a pozdějším rozpoznáním textu tištěné edice Vladimíra Kyase (Kyas, Vladimír: Staročeská bible Drážďanská a Olomoucká III. Praha, Academia 1988). Do textu Kyasovy edice nebylo zasahováno, pouze v místech, kde V. Kyas doplňoval chybějící text Olomoucké bible textem jiné biblické památky, byly tyto přejímky vypuštěny. Naopak informace o emendacích a dalších úpravách textu, které obsahovala v tištěné edici ediční poznámka, byly pro elektronickou verzi zapracovány přímo do textu." } }
                };
        }

        public Source GetDetail(string id)
        {
            return new Source { Title = "Bible olomoucká, Genesis–Esdráš", Author = "", Datation = "1417", Perex = "Elektronická edice Olomoucké bible vznikla naskenováním a pozdějším rozpoznáním textu tištěné edice Vladimíra Kyase (Kyas, Vladimír: Staročeská bible Drážďanská a Olomoucká III. Praha, Academia 1988). Do textu Kyasovy edice nebylo zasahováno, pouze v místech, kde V. Kyas doplňoval chybějící text Olomoucké bible textem jiné biblické památky, byly tyto přejímky vypuštěny. Naopak informace o emendacích a dalších úpravách textu, které obsahovala v tištěné edici ediční poznámka, byly pro elektronickou verzi zapracovány přímo do textu." };
        }

        public IEnumerable<Source> GetSources(string query, SourcesViewMode mode)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            query = query.Trim();

            if (string.Equals(query, "A", StringComparison.InvariantCultureIgnoreCase) && mode == SourcesViewMode.Name)
            {
                return new List<Source>
                {
                    { new Source{ Title = "[Alexandreida. Zlomek budějovicko-muzejní.]", Author = "", Datation = "polovina 14. století", Perex = "Alexandreida, epická báseň o Alexandru Velikém" } },
                    { new Source{ Title = "[Alexandreida. Zlomek budějovický.]", Author = "", Datation = "polovina 14. století", Perex = "Alexandreida, epická báseň o Alexandru Velikém" } },
                    { new Source{ Title = "[Alexandreida. Zlomek budějovický druhý.]", Author = "", Datation = "polovina 14. století", Perex = "Alexandreida, epická báseň o Alexandru Velikém" } }
                };
            }
            else if (string.Equals(query, "B", StringComparison.InvariantCultureIgnoreCase) && mode == SourcesViewMode.Name)
            {
                return new List<Source>
                {
                    { new Source{ Title = "[Bible kladrubská, kniha Jozue]", Author = "", Datation = "1471", Perex = "Bible kladrubská" } },
                    { new Source{ Title = "	[Bible olomoucká, Genesis–Esdráš]", Author = "", Datation = "1417", Perex = "	Bible olomoucká" } }
                };
            }
            else if (string.Equals(query, "B", StringComparison.InvariantCultureIgnoreCase) && mode == SourcesViewMode.Author)
            {
                return new List<Source>
                {
                    { new Source{ Title = "[Sborník traktátů Jana Bechyňky (neuberský)]", Author = "Bechyňka, Jan", Datation = "okolo roku 1500", Perex = "Sborník náboženských a mystických skladeb kněze Jana Bechyňky (Neuberský)" } },
                    { new Source{ Title = "Překlad kroniky Twingerovy", Author = "Beneš z Hořovic", Datation = "okolo roku 1445", Perex = "Martiniani (Martimiani) aneb Římská kronika" } }
                };
            }

            return new List<Source>();
        }
    }
}