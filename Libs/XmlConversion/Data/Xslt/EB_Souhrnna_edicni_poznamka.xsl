<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	xmlns="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:template name="edicniPoznamka">
	<div type="editorial" subtype="comment">
		<head>Souhrnná ediční poznámka</head>
		<p>
			Elektronické knihy starších česky psaných památek jsou pořizovány výhradně z rukopisů a starých tisků z období 13.–18. století. Při ediční práci je uplatňován kritický přístup k vydávanému textu. Texty jsou transkribovány, tj. převedeny do novočeského pravopisného systému, s tím, že jsou respektovány specifické rysy soudobého jazyka. Elektronické edice vznikají na akademickém pracovišti zabývajícím se lingvistickým výzkumem, proto je mimořádný důraz kladen na interpretaci a spolehlivý záznam jazyka památky.
		</p>
		<p>
			Transkripce textů se řídí obecně uznávanými edičními pravidly, jimiž jsou pro období staré a střední češtiny zejména texty Jiřího Daňhelky
			<hi rend="it">Směrnice pro vydávání starších českých textů </hi>
			(Husitský Tábor 8, 1985, s. 285–301), 
			<hi rend="it">Obecné zásady ediční a poučení o starém jazyce českém </hi>
			(in: Výbor z české literatury od počátků po dobu Husovu. Praha, Nakladatelství Československé akademie věd 1957, s. 25–35) a 
			<hi rend="it">Obecné zásady ediční a poučení o češtině 15. století </hi>
			(in: Výbor z české literatury doby husitské. Svazek první. Praha, Nakladatelství Československé akademie věd 1963, s. 31–41) a text Josefa Vintra 
			<hi rend="it">Zásady transkripce českých textů z barokní doby </hi>
			(Listy filologické 121, 1998, s. 341–346). Tato obecná pravidla jsou přizpůsobována stavu a vlastnostem konkrétního díla. Při transkripci textu editor dbá na to, aby svou interpretací nesetřel charakteristické rysy jazyka a textu, zároveň však nezaznamenává jevy, které nemají pro interpretaci textu či jazyka význam (tj. např. grafické zvláštnosti textu).
		</p>
		<p>
			Součástí elektronických knih je textověkritický a poznámkový aparát, jehož obsah a rozsah je zcela v kompetenci jednotlivých editorů. Bez výjimek jsou v kritickém aparátu zaznamenány všechny zásahy do textu, tj. emendace textu. Pravidelně jsou zaznamenávány informace týkající se poškození či fragmentárnosti předlohy, nejistoty při interpretaci textu atp. Naopak méně často jsou uvedeny věcné vysvětlivky a zřídka jsou zachyceny mezitextové vztahy.
		</p>
		<p>Elektronické edice neobsahují slovníček vykládající méně známá slova. K tomuto účelu slouží slovníky zapojené do jednotného vyhledávacího systému <hi rend="it">Vokabuláře webového</hi> (http://vokabular.ujc.cas.cz/hledani.aspx), případně též slovníky nezapojené do tohoto vyhledávání (http://vokabular.ujc.cas.cz/zdroje.aspx).</p>
		<div>
			<head>Struktura a forma elektronických edic</head>
			<p>
				Text edice je strukturován, tj. povinně je v něm zaznamenávána uzuální foliace či paginace. V závislosti na charakteru předlohového textu mohou být v elektronické knize uvedena čísla veršů (u veršovaných předloh) či označení kapitol a veršů (u biblických textů). 
			</p>
			<p>
				 Dále je naznačena i další struktura textu, tj. text je členěn pomocí nadpisů a podnadpisů na nižší celky.
			</p>
			<p>
				 Součástí elektronických edic je textověkritický a poznámkový aparát.
			</p>
			<p>
				Ve formátu PDF je tento aparát zachycen v poznámkách pod čarou umístěných na konkrétní stránce, ve formátu EPUB je textověkritický a poznámkový aparát uveden v poznámkách v závěru dokumentu: 
			</p>
			<list>
				<item>
					<hi rend="sup">a</hi>
					 – malé písmeno v horním indexu označuje emendace. U emendací je na prvním místě uvedeno kurzivou správné (tj. opravené) znění textu ve shodě s edicí, za grafickou značkou ] je uvedeno chybné znění pramenné předlohy
				</item>
				<item>
					<hi rend="sup">1</hi>
					 – arabská číslice označuje poznámky a komentáře nejrůznějšího druhu, které editor pokládal za důležité pro interpretaci textu
				</item>
			</list>
			<p>
				Formát EPUB umožňuje hypertextový přechod z různých částí textu (z položek obsahu, z indexových číslic a písmen textověkritického komentáře) na jiný text. Upozorňujeme, že některé elektronické čtečky nemusí podporovat všechny zamýšlené formáty a funkce.
			</p>
			<p>
				V elektronické edici je dále použito těchto typů závorek a dalšího grafického značení:
			</p>
			<list>
				<head>závorky</head>
				<item>
					{} ve složených závorkách je zaznamenán text, který má charakter přípisku. Rozlišujeme přípisky podle umístění v textu (marginální a interlineární) a přípisky podle stáří vzhledem k základnímu textu (soudobou rukou a pozdější rukou).
				</item>
				<item>
					[] v hranatých závorkách je zaznamenán text, který není součástí předlohového textu, ale který lze na základě pravděpodobnosti či jiného textu do edice doplnit; text v hranatých závorkách může být doplněn i o poznámku (značenou arabskou číslicí v horním indexu) s údajem, odkud je text doplněn. V hranatých závorkách jsou rovněž umístěna čísla a údaje určující strukturu textu (foliace, paginace).
				</item>
			</list>
			<list>
				<head>odlišné písmo</head>
				<item><hi rend="it">kurziva </hi>označuje text, který byl editorem interpretován jako text nečeský; dále nerozlišujeme, o jaký jazyk se jedná, avšak nejčastěji se vyskytuje latina</item>
				<item>větším písmem jsou označeny nadpisy a podnadpisy</item>
				<item>různou velikost písma či <hi rend="it">kurzivní </hi>řez některé čtečky nerozlišují 
				</item>
			</list>
		</div>
	</div>		
	</xsl:template>
</xsl:stylesheet>