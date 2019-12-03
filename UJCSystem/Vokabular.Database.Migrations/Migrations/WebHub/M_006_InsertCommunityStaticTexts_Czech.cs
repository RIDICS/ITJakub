using System;
using FluentMigrator;
using FluentMigrator.SqlServer;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.WebHub
{
    [DatabaseTags(DatabaseTagTypes.VokabularWebDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(006)]
    public class M_006_InsertCommunityStaticTexts_Czech : ForwardOnlyMigration
    {
        public override void Up()
        {
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 24,
                Culture = 1,
                DictionaryScope = 13,
                Name = "support",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Podpora
Podpora Vokabuláře webového: 

2012–2015 projekt MK ČR č. DF12P01OVV028 *Informační technologie ve službách jazykového kulturního bohatství (IT JAKUB)*  
2010–2015 projekt MŠMT LINDAT-CLARIN č. LM2010013 *Vybudování a provoz českého uzlu pan-evropské infrastruktury pro výzkum*  
2010–2014 projekt GA ČR č. P406/10/1140 *Výzkum historické češtiny (na základě nových materiálových bází)*  
2010–2014 projekt GA ČR č. P406/10/1153 *Slovní zásoba staré češtiny a její lexikografické zpracování*  
2005–2011 projekt MŠMT ČR LC 546 *Výzkumné centrum vývoje staré a střední češtiny (od praslovanských kořenů po současný stav)*  
",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 25,
                Culture = 1,
                DictionaryScope = 13,
                Name = "about",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# O Vokabuláři webovém
*Vokabulář webový* jsou internetové stránky, které od listopadu 2006 postupně zpřístupňují textové, obrazové a zvukové zdroje k poznání historické češtiny. Tvůrcem a provozovatelem *Vokabuláře webového* je [oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.](http://www.ujc.cas.cz/zakladni-informace/oddeleni/oddeleni-vyvoje-jazyka/) (dále ÚJČ). *Vokabulář webový* je autorské dílo chráněné ve smyslu aktuálního znění zákona č. 121/2000 Sb., o právu autorském, a je určen pouze k nekomerčnímu využití. Veškeré materiály poskytujeme se souhlasem nositelů autorských a reprodukčních práv.

Na stránky *Vokabuláře webového* jsme umístili různorodé zdroje. K vyhledávání informací o slovní zásobě hstorické češtiny slouží především oddíl [Slovníky](/Dictionaries), který tvoří různorodé novodobé i dobové lexikální zdroje. V oddílu [Edice](/Editions) jsou prameny zaznamenané starší češtinou prezentovány jako souvislý text společně s textově-kritickým komentářem. Souhrnně lze starší české texty prohledávat v části [Korpusy](/BohemianTextBank), [staročeský korpus](#) obsahuje texty staročeských pramenů z cca 13. až 15. století a zároveň může do určité míry nahradit dokladovou část u těch staročeských lexikografických zdrojů, které ji neobsahují. Do [staročeského korpusu](#) zahrnujeme texty z období 16. až 18. století. Díla starší české literatury poskytujeme též ve formě [audioknih](/AudioBooks).

Poučení o historické češtině najde uživatel také v oddílu [Mluvnice](/OldGrammar), který prezentuje digitalizované verze mluvnic a obdobných příruček z 16. až počátku 20. století. Mluvnice slouží nejen ke studiu dobového stavu a proměn českého jazykového systému, ale též k bádání o vývoji českého mluvnictví. V části [Odborná literatura](/ProfessionalLiterature) jsou zveřejněny digitalizované verze odborných textů, které se věnují problematice historické češtiny (*Historická mluvnice jazyka českého* Jana Gebauera).

V oddíle [Bibliografie](/Bibliographies) nabízíme vyhledávání v bibliografických záznamech odborné literatury k problematice staršího českého jazyka a literatury.

V části [Kartotéky](/CardFiles) zpřístupňujme digitalizovanou podobu dvou kartoték Jana Gebauera: kartotéky excerpce ze staročeské literatury a kartotéky pramenů k této excerpci.

V oddíle [Pomůcky](/Tools) nabízíme softwarové nástroje a pomůcky pro práci s digitalizovanými zdroji. Tyto nástroje si mohou zájemci zdarma stáhnout a používat, a to včetně zdrojových kódů.

Snažíme se, aby se uživatelům s *Vokabulářem webovým* dobře pracovalo a aby jeho prostřednictvím získávali informace rychle a spolehlivě. Ačkoliv usilujeme o bezproblémový chod, jsme si vědomi toho, že se zřejmě nevyhneme nedostatkům a obtížím, které se mohou vyskytnout. Budeme rádi, když nás uživatelé budou informovat formou [připomínek](/Home/Feedback) nejen o své zkušenosti s fungováním *Vokabuláře webového*, ale také o chybějících či nesprávných údajích a informacích, a to buď prostřednictvím odkazu *Připomínky* v jednotlivých oddílech, nebo pomocí odkazu *Připomínky* v zápatí internetových stránek.

V budoucnosti se bude *Vokabulář webový* rozrůstat o další zdroje. Časový harmonogram „naplňování“ záměrně neuvádíme – rozvoj bude postupný, závislý na mnohých okolnostech.

Přístup ke stránkám *Vokabuláře webového* je bezplatný. Pokud budete užívat nalezené informace ve svých publikacích, citujte je podle [návodu](/Home/HowToCite).

Tvorba *Vokabuláře webového* je [podporována](/Home/Support) z různých zdrojů.",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 26,
                Culture = 1,
                DictionaryScope = 13,
                Name = "copyright",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Copyright
Copyright © 2006–2015, oddělení vývoje jazyka, Ústav pro jazyk český AV ČR, v. v. i.

*Podmínky užití*  
*Vokabulář webový* je autorské dílo chráněné ve smyslu aktuálního znění zákona č. 121/2000 Sb., o právu autorském, a slouží výhradně k nekomerčnímu využití. Bez předchozí konzultace s oddělením vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i., je zakázáno rozšiřovat jakoukoliv jeho část, ať již samostatně, či v rámci jiného projektu. Při [citaci](/Home/HowToCite) *Vokabuláře webového* či jeho součástí je nutné postupovat podle obecně uznávaných citačních pravidel.",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 27,
                Culture = 1,
                DictionaryScope = 13,
                Name = @"contacts",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Kontakty
*adresa:*  
oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.  
Valentinská 1  
116 46 Praha 1  
[http://www.ujc.cas.cz/zakladni-informace/oddeleni/oddeleni-vyvoje-jazyka/](http://www.ujc.cas.cz/zakladni-informace/oddeleni/oddeleni-vyvoje-jazyka/)

*e-mail:*  
[vokabular@ujc.cas.cz](mailto:vokabular@ujc.cas.cz)

*telefon:*  
+420 225 391 452

*mapa:*  
<script type=""text/javascript"" src=""https://maps.google.com/maps/api/js?sensor=false""></script>
<div style=""overflow: hidden; height: 500px; width: 600px;"">
    <div id=""gmap_canvas"" style=""height: 500px; width: 600px;""></div>
    <style>
        #gmap_canvas img {
            max-width: none !important;
            background: none !important;
        }
    </style><a class=""google-map-code"" href=""https://www.map-embed.com"" id=""get-map-data"">https://www.map-embed.com</a>
</div>
<script type=""text/javascript"">
    function init_map()
    {
        var myOptions = {
            zoom: 18,
            center: new google.maps.LatLng(50.0874414, 14.416664099999934),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById(""gmap_canvas""), myOptions);
        marker = new google.maps.Marker({
            map: map,
            position: new google.maps.LatLng(50.0874414, 14.416664099999934)
        });
        infowindow = new google.maps.InfoWindow({
            content:
                ""<b>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.</b><br/>Valentinsk&aacute; 1<br/>12800 Praha""
        });
        google.maps.event.addListener(marker, ""click"", function() { infowindow.open(map, marker); });
        infowindow.open(map, marker);
    }
    google.maps.event.addDomListener(window, ""load"", init_map);
</script>",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 28,
                Culture = 1,
                DictionaryScope = 13,
                Name = "links",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Odkazy
Ústav pro jazyk český AV ČR, v. v. i., Letenská 4, Praha 1, 118 51 [http://www.ujc.cas.cz](http://www.ujc.cas.cz)

Manuscriptorium, virtuální badatelské prostředí pro oblast historických fondů [http://www.manuscriptorium.com/](http://www.manuscriptorium.com/)  
Centrum medievistických studií (digitalizované edice a jiné zdroje k českým středověkým dějinám) [http://cms.flu.cas.cz/](http://cms.flu.cas.cz/)  
Virtuální archiv listin střední Evropy [http://monasterium.net/mom/home?_lang=ces](http://monasterium.net/mom/home?_lang=ces)  
Český národní korpus [http://ucnk.ff.cuni.cz](http://ucnk.ff.cuni.cz)  
Elektronická verze Příručního slovníku jazyka českého a naskenovaný lístkový archiv k PSJČ [http://psjc.ujc.cas.cz](http://psjc.ujc.cas.cz)  
Elektronická verze Slovníku spisovného jazyka českého [http://ssjc.ujc.cas.cz](http://ssjc.ujc.cas.cz)  
Lexikální databáze humanistické a barokní češtiny, dostupná po registraci z adresy [http://madla.ujc.cas.cz](http://madla.ujc.cas.cz)  
LEXIKO – webové hnízdo o novodobé české slovní zásobě a výkladových slovnících [http://lexiko.ujc.cas.cz/](http://lexiko.ujc.cas.cz/)  
Latinsko-české slovníky mistra Klareta (elektronická edice knihy Klaret a jeho družina) [http://titus.uni-frankfurt.de/texte/etcs/slav/acech/klaret/klare.htm](http://titus.uni-frankfurt.de/texte/etcs/slav/acech/klaret/klare.htm)  
Elektroniczny słownik języka polskiego XVII i XVIII wieku [http://sxvii.pl/](http://sxvii.pl/)  
Historický slovník slovenského jazyka V (R—Š), Bratislava 2000 [http://slovnik.juls.savba.sk/?d=hssjV](http://slovnik.juls.savba.sk/?d=hssjV)  
Staročeská sbírka (lístkový katalog) Ústavu pro českou literaturu AV ČR, v. v. i. [http://starocech.ucl.cas.cz](http://starocech.ucl.cas.cz)  

Moravská zemská knihovna v Brně [http://www.mzk.cz](http://www.mzk.cz)  
Národní knihovna České republiky v Praze [http://www.nkp.cz](http://www.nkp.cz)  
Vědecká knihovna v Olomouci [http://www.vkol.cz](http://www.vkol.cz)  
Knihovna Strahovského kláštera v Praze [http://www.strahovskyklaster.cz/webmagazine/page.asp?idk=282](http://www.strahovskyklaster.cz/webmagazine/page.asp?idk=282)  
Knihovna Národního muzea v Praze [http://www.nm.cz/Knihovna-NM/](http://www.nm.cz/Knihovna-NM/)  
Archiv Pražského hradu (knihovna Metropolitní kapituly u sv. Víta) [http://old.hrad.cz/castle/archiv/index.html](http://old.hrad.cz/castle/archiv/index.html)  
Městská knihovna v Praze [http://www.mlp.cz](http://www.mlp.cz)  
Knihovna Akademie věd ČR [http://www.lib.cas.cz](http://www.lib.cas.cz)  
",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 29,
                Culture = 1,
                DictionaryScope = 13,
                Name = "howtocite",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Jak citovat
*Vokabulář webový* je médium proměnlivé. Jednak postupně zpřístupňujeme další zdroje, jednak u děl nedokončených (jako je například *Elektronický slovník staré češtiny*) informace doplňujeme a v neposlední řadě také opravujeme chyby, které se ve zdrojích zpřístupněných prostřednictvím *Vokabuláře webového* podaří najít. Také z těchto důvodů nemají publikované korpusy povahu referenčních korpusů.

Při citaci zveřejněných děl nebo celého *Vokabuláře webového* uvádějte datum citování. Pokud budete citovat konkrétní zdroj nebo heslovou stať, používejte pro detailnější určení také datum poslední změny textu. Tento údaj se u slovníků zobrazuje v závěru každé heslové stati, u ostatních děl v informacích o zdroji.

Bibliografická citace jednotlivých děl podle normy ČSN ISO 690 je dostupná <span style=""background-color: yellow"">pod odkazem Jak citovat</span> na stránce s detailními informacemi o jednotlivých zdrojích.

### Při citování údajů z *Vokabuláře webového* doporučujeme následující způsoby:
&nbsp;

#### Při odkazování na webové stránky jako celek:

Vokabulář webový. *Vokabulář webový* [online]. Praha: oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v.v.i., 2015[cit. 2015-06-21]. Dostupné z: http://vokabular.ujc.cas.cz 

#### Při odkazování na konkrétní zdroj (slovník, edici, mluvnici, odbornou literaturu aj.):

BĚLIČ, Jaromír, Adolf KAMIŠ a Karel KUČERA. *Malý staročeský slovník* [online]. 1. vyd. Praha: Státní pedagogické nakladatelství, 1978, 2014-03-12, 707 s.
[cit. 2015-06-21]. Dostupné z: http://vokabular.ujc.cas.cz/slovniky/mss 

#### Při odkazování na korpus:

Staročeský korpus. *Staročeský korpus* [on-line]. Praha: oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v.v.i., 2015-03-12 [cit. 2015-06-21]. Dostupné z: \<http://vokabular.ujc.cas.cz/banka.aspx\>. ",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 30,
                Culture = 1,
                DictionaryScope = 13,
                Name = "feedback",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Připomínky
Na této stránce nám můžete napsat své připomínky k provozu či k informacím *Vokabuláře webového*. Pro připomínky ke konkrétním částem *Vokabuláře webového* (např. slovníkům, edicím, mluvnicím, korpusu atd.) používejte laskavě *Připomínek* na stranách jednotlivých typů informačních zdrojů.

Pokud si přejete, abychom na Vaši připomínku odpověděli, uveďte tuto skutečnost v textu Vaší zprávy a vyplňte laskavě kolonku *Jméno* a *E-mail*. Vynasnažíme se, abychom na Vaši připomínku reagovali co nejdříve. Upozorňujeme, že **neřešíme domácí úkoly a další školní práce**. V těchto případech Vám může pomoci dostupná [odborná literatura](/ProfessionalLiterature) či [další zdroje](/Home/Links). Pokud má Vaše připomínka charakter dotazu, upozorňujeme, že **odpovědi na dotazy v Ústavu pro jazyk český AV ČR mohou být zpoplatněny**.

[Provozovatel](http://www.ujc.cas.cz/zakladni-informace/oddeleni/oddeleni-vyvoje-jazyka/) stránek si vyhrazuje právo nereagovat na připomínky, které jsou pro provoz *Vokabuláře webového* zcela nepřínosné či které jsou v rozporu s dobrými mravy.",
                ModificationTime = DateTime.Parse("2016-07-14 15:44:49.000"),
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 31,
                Culture = 1,
                DictionaryScope = 14,
                Name = "info",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Informace
V tomto oddílu představujeme zájemcům elektronické edice česky psaných textů z období 13.–18. století, s přesahem do začátku 19. století v případech, kdy se jedná o opis staršího textu. Výchozím textem (pramenem) pro elektronickou edici je rukopis, prvotisk či starý tisk. Jen výjimečně používáme jako pramen novodobou edici, a to tehdy, je-li originální pramen nedostupný či obtížně dostupný nebo je-li novodobá edice jen těžko překonatelná (mj. vzhledem k jejímu rozsahu, jako je tomu např. u edice V. Kyase a kol., *Staročeská Bible drážďanská a olomoucká*).

Elektronické edice jsou jedním z výsledků vědecké práce oddělení vývoje jazyka a slouží především jako materiálová báze pro následný jazykový výzkum, proto je jazykový přístup k textu upřednostněn ku příkladu před literárními či historickými aspekty. Edice jsou dále začleňovány do textové báze budovaného *staročeského a středněčeského korpusu*. Spolu s digitálními kopiemi rovněž spoluvytvářejí virtuální badatelské prostředí *Manuscriptorium*.

Při editaci literárních památek jsou dodržovány zásady vědeckého zpřístupňování historických textů, a dále nezbytné formální aspekty, které umožňují prezentovat texty prostřednictvím internetu při zachování nezbytných náležitostí kritické edice. Ačkoliv jsou editoři oddělení vývoje jazyka vedeni snahou aplikovat vědecké i formální přístupy maximálně jednotně a uplatňují obecné ediční zásady, nelze tento jednotný přístup nadřadit obsahově-formálním specifikům jednotlivých pramenů. Každý z nich je originální a svébytné dílo, jehož zvláštnosti musí editor ve své práci plně respektovat.

Elektronické edice vznikají již po několik let a způsob značení textověkritického aparátu se postupně rozvíjí a upřesňuje. Při srovnání některých edic lze tedy dojít ke zjištění, že stejný jev je v jedné edici značen jinak než v jiné (např. ve starších edicích je formát poznámky užíván i pro zachycení marginálních přípisků, živého záhlaví atp.; v novějších edicích jsou tyto části signalizovány jiným způsobem, přímo v textu).

Pro připomínky k elektronickým edicím lze užít nabídku [Připomínky](/Editions/Editions/Feedback).

Edice jsou vystaveny pouze pro studijní a badatelské účely a nelze rozšiřovat jakoukoliv jejich část, ať již samostatně, či v rámci jiného projektu. V případě zájmu nás kontaktujte elektronicky na adrese [vokabular@ujc.cas.cz](mailto:vokabular@ujc.cas.cz?subject=zajem) nebo písemně: oddělení vývoje jazyka, Ústav pro jazyk český AV ČR, v. v. i., Valentinská 1, 116 46, Praha 1. Veškeré přejímané informace podléhají citačnímu úzu vědecké literatury.",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 32,
                Culture = 1,
                DictionaryScope = 14,
                Name = "help",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Nápověda",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 33,
                Culture = 1,
                DictionaryScope = 14,
                Name = "principles",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Obecné ediční zásady
Při vytváření elektronických edic je uplatňován kritický přístup k vydávanému textu. Texty jsou transkribovány, tj. převedeny do novočeského pravopisného systému, s tím, že jsou respektovány specifické rysy soudobého jazyka. Elektronické edice vznikají na akademickém pracovišti zabývajícím se lingvistickým výzkumem, proto je kladen mimořádný důraz na interpretaci a spolehlivý záznam jazyka pramenného textu.

Transkripce textů se řídí obecně uznávanými edičními pravidly, jimiž jsou pro období staré a střední češtiny zejména texty Jiřího Daňhelky [Směrnice pro vydávání starších českých textů](#) (Husitský Tábor 8, 1985, s. 285–301), [Obecné zásady ediční a poučení o starém jazyce českém](#) (in: Výbor z české literatury od počátků po dobu Husovu. Praha, Nakladatelství Československé akademie věd 1957, s. 25–35) a [Obecné zásady ediční a poučení o češtině 15. století](#) (in: Výbor z české literatury doby husitské. Svazek první. Praha, Nakladatelství Československé akademie věd 1963, s. 31–41) a text Josefa Vintra [Zásady transkripce českých textů z barokní doby](#) (Listy filologické 121, 1998, s. 341–346). Na základě těchto pravidel vznikly interní *Pokyny pro tvorbu elektronických edic*. Tato obecná pravidla jsou přizpůsobována stavu a vlastnostem konkrétního textu. Při transkripci textu editor dbá na to, aby svou interpretací nesetřel charakteristické rysy jazyka a textu, zároveň však nezaznamenává jevy, které nemají pro interpretaci textu či jazyka význam (např. grafické zvláštnosti textu). Na základě uvážení editora jsou v některých případech tyto obecné ediční zásady doplněny o specifickou ediční poznámku vážící se ke konkrétnímu textu.

Součástí elektronických edic je textověkritický a poznámkový aparát, jehož obsah a rozsah je zcela v kompetenci jednotlivých editorů. Bez výjimek jsou v kritickém aparátu zaznamenány všechny zásahy do textu, tj. emendace textu (uvozené grafickou značkou ]). Uzná-li to editor za vhodné, může k vybraným úsekům uvádět také transliterované znění předlohy (v tomto případě následuje transliterovaná podoba za dvojtečkou :). Pravidelně jsou zaznamenávány informace týkající se poškození či fragmentárnosti předlohy, nejistoty při interpretaci textu atp. Naopak výjimečně jsou zachyceny mezitextové vztahy.

Elektronické edice neobsahují slovníček vykládající méně známá slova. K tomuto účelu slouží mj. slovníky zveřejněné ve *Vokabuláři webovém*.

# Struktura a forma elektronických edic
<span style=""color:red"">nutno aktualizovat dle budoucího stavu</span>

V přehledovém záznamu o elektronické edici je na prvním místě uvedeno jméno autora originálního textu (známe-li jej) a následuje název dokumentu; pokud se jedná o název nepůvodní, uzuální, je uzavřen v hranatých závorkách []. Dále uvádíme informace o rukopisu či tisku, z něhož byla edice pořízena (uložení, signatura, stránkové určení a datace). Tyto informace se zobrazují i při „listování“ textem edice v záhlaví dokumentu.

Další informace o elektronické edici poskytuje nabídka Detail edice, která je skryta pod ikonkou otazníku. Zde je dílo přiřazeno k literárnímu druhu a žánru a jsou zde uvedeny uzuální zkratky literární památky a pramene. Dále se zde nachází jméno editora elektronické edice a označení nositele osobnostních a majetkových autorských práv. V případě, že editor k textu vypracoval ještě specifickou ediční poznámku, vede k ní odkaz Ediční poznámka.

Text edice se zobrazí po kliknutí na ikonku otevřené knihy. Text je strukturován, tj. povinně je v nich zaznamenávána uzuální foliace či paginace. Ve výjimečných případech je v textu uvedena červenou barvou i foliace či paginace variantní (např. stránkování novodobé edice). Naznačena je i další struktura textu - text je členěn pomocí nadpisů a podnadpisů na nižší celky. V závislosti na charakteru předlohového textu mohou být v elektronické edici uvedena čísla veršů (u veršovaných předloh) či označení kapitol a veršů (u biblických textů). Zkratky označující kapitoly jsou uváděny v souladu s územ Staročeského slovníku, srov. Staročeský slovník. Úvodní stati, soupis pramenů a zkratek, Praha, Academia 1968, s. 119.

Kliknutím na zelenou ikonku stránky je možné zobrazit obsah dokumentu, tj. členění textu na kapitoly či podobné celky. Kliknutím na červenou ikonku stránky se tento obsah skryje.

Součástí elektronických edic je textověkritický a poznámkový aparát, který je v textech zachycen trojím způsobem: přímo v základním textu edice, jako rozvinovací poznámka pod čarou nebo jako bublinková nápověda k odpovídající značce poznámky pod čarou. Poznámkový aparát je možné skrýt kliknutím na červenou ikonku stránky s terčíkem v dolní části. Skryje se tak nejen textověkritický komentář, ale i proznačení stránkování. V textu zůstává vysvětlení, že text zapsaný kurzivou je text cizojazyčný, a dále vysvětlení užití různého typu závorek (pro přípisky různého typu, pro doplněný text).

Tučným písmem v horním indexu v hranatých závorkách jsou v edici signalizovány vnořené informace, které se po kliknutí zobrazí v dolní části obrazovky, kde jsou umístěny všechny vnořené informace ze zobrazené stránky; vybraná poznámka je graficky zvýrazněna. K označení vnořených informací je užito písmene či arabské číslice v hranatých závorkách:

* [a] – malé písmeno označuje emendace. U emendací je na prvním místě uvedeno kurzivou správné (tj. opravené) znění textu ve shodě s edicí, za grafickou značkou ] je uvedeno chybné znění pramenné předlohy
* [1] – arabská číslice označuje poznámky a komentáře nejrůznějšího druhu, které editor pokládal za důležité pro interpretaci textu. Mj. zde uvádíme transkripci popisků k obrázkům umístěným v textu, a to včetně případných emendací a dalších poznámek editora
* [A] – velké písmeno označuje živé záhlaví. V poznámkovém aparátu je umístěn samotný text živého záhlaví doplněný o případné poznámky editora, popř. emendace nebo transliterace

V elektronické edici je dále použito těchto typů závorek a dalšího grafického značení:

* závorky; při umístění kurzoru nad textem v níže uvedených závorkách se objeví tzv. bublinková nápověda, která objasní jejich funkci (viz níže) 
  - {} ve složených závorkách je zaznamenán text, který má charakter přípisku. Rozlišujeme přípisky co do umístění v textu (marginální a interlineární) a přípisky co do stáří vzhledem k základnímu textu (soudobou rukou a mladší rukou, dále textové orientátory a tištěné marginální přípisky.
  - [] v hranatých závorkách je zaznamenán text, který není součástí předlohového textu, ale který lze na základě pravděpodobnosti či jiného textu do edice doplnit; text v hranatých závorkách může být doplněn i o poznámku (značenou arabskou číslicí v horním indexu) s údajem, odkud je text doplněn. V hranatých závorkách jsou rovněž umístěna čísla a údaje určující strukturu textu (např. foliace, paginace, značka pro obrázek nebo jiný grafický prvek).
* odlišné písmo 
  - kurziva označuje text, který byl editorem interpretován jako text nečeský; dále nerozlišujeme, o jaký jazyk se jedná, avšak nejfrekventovanější je latina
  - větším písmem tmavězelené barvy jsou označeny nadpisy a podnadpisy
  - velké tučné písmeno v základním textu znamená, že v pramenném textu je (iluminovaná) iniciála
  - červené písmo je vyhrazeno pro variantní biblický překlad; zelené písmo pro adresáta textu (listu) a modré písmo pro tiskařské impresum (a informace s ním spojené)
* grafické znaky 
  - [#]– tzv. ležatý křížek v dvojitých lomených závorkách zastupuje obrázky, schémata či tabulky, které se nacházejí v předlohovém textu a které nelze z technických důvodů jednoduše převést do elektronické podoby. Zároveň se jedná o obsah, který nemá na interpretaci textu závažný dopad, a lze je proto vynechat. Znak [#] může být doplněn poznámkou (značenou arabskou číslicí v horním indexu), v níž je stručně popsána vynechaná pasáž
* bublinková nápověda 
  - při umístění kurzoru na text odlišující se podtečkováním, barvou, velikostí či řezem od základního textu (tučné, kurziva) se zobrazí bližší informace o důvodu užití odlišného písma. Bublinková nápověda je k dispozici i u textu v závorkách ({}, []) a u trojtečky (…), pokud se jedná o torzovité, doplněné či rekonstruované slovo
  - nápověda je k dispozici také u emendací a poznámek a slouží k rychlému zobrazení informace, která je uvedena v poznámkovém aparátu na konci stránky. Tato nápověda se však zobrazuje pouze v případě, že komentovaný úsek netvoří součást jiného delšího úseku, který je sám o sobě vybaven bublinkovou nápovědou (tj. nejde např. o emendační poznámku v rámci cizojazyčného textu nebo přípisku)",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 34,
                Culture = 1,
                DictionaryScope = 15,
                Name = "info",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Informace
*Staročeský a středněčeský korpus* (dříve staročeská a středněčeská textová banka) vznikají v rámci textologické a ediční činnosti oddělení vývoje jazyka. Jsou budovány od roku 2006, odkdy jsou texty psané historickou češtinou, jejichž transkribované edice v oddělení vznikají, formálně upravovány rovněž s ohledem na korpusové zpřístupnění prostřednictvím *Vokabuláře webového*. Zprvu se hlavní důraz kladl na staročeské období – *staročeský korpus* je k dipozici veřejnosti již od r. 2008; od roku 2015 zpřístupňujeme též *středněčeský korpus*.

*Staročeský korpus* zahrnuje texty z období od nejstarších počátků historické češtiny přibližně do konce 15. století, *středněčeský korpus* zpřístupňuje texty z doby od 16. století do konce 18. století. Zařazené texty zřídka mírně přesahují stanovený horní limit – a to v případech, kdy lze předpokládat, že text vznikl ve starším období. Texty jsou do korpusu zařazovány výhradně v transkripci do novočeského pravopisu. Výchozím textem (pramenem) je rukopis, prvotisk či starý tisk. Jen výjimečně používáme jako pramen novodobou edici, a to tehdy, je-li originální pramen nedostupný či obtížně dostupný nebo je-li novodobá edice jen těžko překonatelná (mj. vzhledem k jejímu rozsahu, jako je tomu např. u edice V. Kyase a kol., *Staročeská Bible drážďanská a olomoucká*).

Zařazené texty prošly při transkripci podrobnou lingvistickou analýzou, proto je lze prezentovat s doprovodnými informacemi, a to alespoň v té míře, jakou webová prezentace dovolí. Tyto informace se týkají pramenného textu jako artefaktu a charakteristiky jazyka, jímž je pramen zaznamenán. Pro zveřejnění těchto informací bylo třeba vytvořit speciální korpusový manažer, jehož autorem je PhDr. Pavel Květoň, Ph.D. Přehledný návod k užívání manažeru je obsažen v aplikaci pod záložkou [Nápověda](/BohemianTextBank/BohemianTextBank/Help). Protože jsou korpusy postupně doplňovány, upravovány a opravovány, je při citaci korpusových dat nezbytné uvádět verzi korpusu, tj. datum, které je uvedeno v nabídkovém menu korpusového manažeru.

Korpusy nejsou anotované; až na výjimky neobsahují lemmatizaci ani morfologické charakteristiky.

Pro připomínky ke korpusům lze užít nabídku [Připomínky](/BohemianTextBank/BohemianTextBank/Feedback).",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 35,
                Culture = 1,
                DictionaryScope = 15,
                Name = "help",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Nápověda",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 36,
                Culture = 1,
                DictionaryScope = 16,
                Name = "info",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Informace
V oddílu Mluvnic *Vokabuláře webového* (dříve též *modul digitalizovaných mluvnic, MDM*) poskytujeme zájemcům digitalizované verze historických mluvnic a podobných jazykových příruček z období 16. až 19. století. Tuto část *Vokabuláře webového* jsme uvedli v užívání mj. na počest stého výročí založení *Kanceláře Slovníku jazyka českého*, která dala základy dnešnímu *Ústavu pro jazyk český*. V roce 2011 byl *MDM* spuštěn v testovacím provozu s několika mluvnicemi na ukázku, v roce 2012 jsme zahájili plný provoz. Zvýšil se nejen počet prezentovaných mluvnic, ale podstatně jsme též upravili a rozšířili informace o těchto významných památkách české jazykovědy.

Na projektu *Mluvnic* se podílejí pracovníci oddělení vývoje jazyka PhDr. Alena Černá, Ph.D., Mgr. Barbora Hanzová, Boris Lehečka, Mgr. Kateřina Voleková, Ph.D., s nimiž spolupracují zejména (bývalí) studenti bohemistiky a jiných oborů FF UK Praha Martina Černá, Hana Enderlová, Hana Gabrielová, Lucie Hrabalová, Petr Valenta, Anna Zitová a Zuzana Žďárská a další. Autorem charakteristik k mluvnicím je PhDr. Ondřej Koupil, Ph.D. Původní aplikaci naprogramoval Lukáš Kubis.

Digitalizované mluvnice je možné využívat pro výzkum českého mluvnictví i jako důležitý sekundární zdroj k poznání historické češtiny období 16. až 19. století. Zejména starší mluvnice se nezřídka dochovaly v nečetných exemplářích, které jsou uloženy v institucích v České republice a v zahraničí. Zpřístupněním jejich digitalizované podoby prostřednictvím internetu se výrazně rozšiřuje okruh jejich uživatelů. *Mluvnice* se stávají důležitou pomůckou nejen při bádání o historické češtině, ale též při výuce českého jazyka na středních, a zvláště na vysokých školách domácích i zahraničních.

Pro usnadnění základní orientace byly digitalizované mluvnice tzv. anotovány, tj. jednotlivé obrazy (většinou dvoustran) byly orientačně označeny lingvistickými termíny, o nichž se na příslušné (dvou)stránce pojednává. Tyto termíny vycházejí z novočeského mluvnického pojetí a v některých případech přesně neodpovídají stavu v historické gramatice (např. jako „hláska měkká“ označujeme pojednání o hlásce „c“, přestože v dobovém pojetí byla chápána jako hláska tvrdá atp.). Ve výjimečných případech, kdy kniha nemá charakter mluvnice, avšak přesto ji chceme zájemcům o vývoj českého mluvnictví zprostředkovat, zveřejňujeme publikaci bez anotace. Jedná se například o tyto knížky: *Hlasové o potřebě jednoty spisovného jazyka pro Čechy, Moravany a Slováky*. Praha, 1846; Josef Dobrovský, *Abhandlung über den Ursprung des Namens Tschech (Cžech), Tschechen*. Praha, Vídeň, 1782 a Josef Dobrovský, *Institutiones linguae Slavicae dialecti veteris, quae quum apud Russos, Serbos aliosque ritus graeci, tum apud Dalmatas glagolitas ritus latini Slavos in libris sacris obtinet: cum tabulis aeri incisis quatuor*. Vídeň, 1822.

V charakteristikách mluvnic se přihlíží také k tomu, jakých písem bylo v jednotlivých tiscích použito. Písma nejsou rozlišena detailně, např. podle velikostních stupňů, ale jen podle základního charakteru. Je zavedena tato grafická konvence:

* **polotučný nekurzivní řez** = švabach
* ***polotučný kurzivní řez*** = fraktura
* základní řez nekurzivní = antikva
* *kurziva (italika)* = polokurziva

Vzácněji užité typy (textura, grotesk) jsou přepisovány jedním z konvenčních přepisů a komentovány v poznámce. K písmům více v příslušných heslech Petr VOIT. *Encyklopedie knihy: starší knihtisk a příbuzné obory mezi polovinou 15. a počátkem 19. století I–II*. 2. vyd. Praha: Libri, 2008.

Digitalizované mluvnice, které zpřístupňujeme veřejnosti, pocházejí především z fondu knihovny ÚJČ. Některé knihy nemá naše knihovna k dispozici, a proto byly naskenovány z exemplářů jiných knihoven, jako *Knihovny Národního muzea* v Praze, *Moravské zemské knihovny* v Brně, *Národní knihovny České republiky* v Praze, *Strahovské knihovny* v Praze, *Vědecké knihovny v Olomouci*. *Státnímu oblastnímu archivu v Třeboni* jsme zavázáni za poskytnutí digitálních kopií a souhlasu se zveřejněním v případě tzv. Husovy *Abecedy* a traktátu *Orthographia Bohemica*. Některé knihy pocházejí ze soukromých sbírek. Děkujeme těmto institucím za laskavý souhlas se zveřejněním digitalizovaných kopií v rámci *Vokabuláře webového*. O nositeli reprodukčních práv k daným snímkům je možné se dočíst v detailním popisu příslušné knihy a také prostřednictvím vodoznaku, kterým jsou digitalizované obrazy opatřeny, např. ÚJČ = kniha pochází z fondu knihovny *Ústavu pro jazyk český AV ČR, v. v. i.*; KNM = kniha pochází z fondu *Knihovny Národního muzea* v Praze; MZK = kniha pochází z fondu *Moravské zemské knihovny* v Brně; NK ČR = kniha pochází z fondu *Národní knihovny ČR* v Praze; VKOL = kniha pochází z fondu *Vědecké knihovny v Olomouci*; Strahov = kniha pochází z fondu *Strahovské knihovny* v Praze; TŘEBOŇ = rukopis pochází z fondu *Státního oblastního archivu v Třeboni*; SOUKROMÉ = kniha pochází ze sbírky soukromé osoby.

Obrazové i textové materiály jsou vystaveny pouze pro studijní a badatelské účely a nelze je bez souhlasu oddělení vývoje jazyka publikovat v žádných dalších textech. V případě zájmu nás kontaktujte elektronicky na adrese [vokabular@ujc.cas.cz](mailto:vokabular@ujc.cas.cz?subject=zajem) nebo písemně: oddělení vývoje jazyka, Ústav pro jazyk český AV ČR, v. v. i., Valentinská 1, 116 46 Praha 1. Veškeré přejímané informace podléhají citačnímu úzu vědecké literatury.",
                ModificationUser = "Admin"
            });
            Insert.IntoTable("BaseText").WithIdentityInsert().Row(new
            {
                Id = 37,
                Culture = 1,
                DictionaryScope = 16,
                Name = "help",
                Format = 1,
                Discriminator = "StaticText",
                Text = @"# Nápověda
Oddíl *Mluvnice* umožňuje prohlížet knihy dvěma způsoby: prvním způsobem je tzv. listování v digitalizované mluvnici vyhledané v seznamu zveřejněných mluvnic. Potřebné informace je možné získat i vyhledáváním napříč všemi mluvnicemi; pro tento způsob byly digitalizované mluvnice tzv. anotovány, tj. jednotlivé obrazy (většinou dvoustran) byly orientačně označeny lingvistickými termíny, o nichž se na příslušné (dvou)stránce pojednává. Tyto termíny vycházejí z novočeského mluvnického pojetí a v některých případech neodpovídají přesně stavu v historické gramatice (např. jako „hláska měkká“ označujeme pojednání o hlásce „c“, přestože v dobovém pojetí byla chápána jako hláska tvrdá atp.). Snažili jsme se, aby vyhledávání bylo maximálně vstřícné k uživateli: do vyhledávání lze tedy zadávat jak termíny české (např. *jméno podstatné*), tak běžně užívané termíny internacionální (např. *substantivum*). Vyhledávat lze nejen podle termínů, nýbrž také podle jiných údajů (slov z názvu mluvnice a jména autora); tyto údaje lze ve složitém modu vyhledávání kombinovat. Výsledkem vyhledávání je seznam všech mluvnic, které odpovídají zadaným kritériím. Každá mluvnice se zobrazuje vždy ve zvláštním panelu, přičemž strany proznačené požadovanými lingvistickými termíny se zobrazují zvýrazněné. Pod aktuálně zobrazenou (dvou)stranou mluvnice je uveden seznam všech lingvistických termínů, které jsou ke (dvou)straně přiřazeny.",
                ModificationUser = "Admin"
            });
           
        }
    }
}