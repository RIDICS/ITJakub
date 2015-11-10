xquery version "3.0";
import module namespace search = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "../modules/searching.xqm";
import module namespace coll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
declare namespace itj = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist";
declare namespace exist = "http://exist.sourceforge.net/NS/exist"; 

declare namespace a="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching";
declare namespace r="http://schemas.datacontract.org/2004/07/ITJakub.SearchService.Core.Search.DataContract";
declare namespace i="http://www.w3.org/2001/XMLSchema-instance";
declare namespace b="http://schemas.microsoft.com/2003/10/Serialization/Arrays";
declare namespace sc="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria";


declare option exist:serialize "highlight-matches=elements";

declare function local:get-match-count-mock($hits as node()*) as xs:int {
	count($hits)
};

(:let $query-criteria-param := if (system:function-available(request:get-parameter, 2)) then
	request:get-parameter("serializedSearchCriteria", "")
	else
	local:get-query-criteria-param()
:)	

let $defaultSearchCriteria := '<ResultSearchCriteriaContract xmlns="http://schemas.datacontract.org/2004/07/ITJakub.SearchService.Core.Search.DataContract"></ResultSearchCriteriaContract>'

let $query-criteria-param := request:get-parameter("serializedSearchCriteria", $defaultSearchCriteria)
let $query-criteria := util:parse($query-criteria-param) (: ve vyšších verzích parse-xml :)

let $queries := search:get-queries-from-search-criteria($query-criteria(://a:SearchCriteriaContract[a:Key = 'Fulltext']:))

(:~ od jakého záznamu se vracejí výsledky, tj. knihy :)
let $result-start := 1
(:~ kolik záznamů má být ve vraceném výsledku; 0 znamená všechny; pokud je číslo větší než celkový počet záznamů, vrátí se všechny :)
let $result-count := 0

(:~ od jakého záznamu se vracejí doklady s výskytem hledaného výrazu :)
let $kwic-start := 1
(:~ kolik záznamů s doklady s výskytem hledaného výrazu se vrací; pokud je číslo větší než celkový počet dokladů, vrátí se všechny :)
let $kwic-count := 3
(:~ počet znaků zleva a zprava pro zobrazení dokladů :)
let $kwic-context-length := 50

(:~ kriterium použité pro seřazení výsledků :)
let $sort-criterion := "datace"
(:~ kriterium použité pro seřazení výsledků :)
let $sort-direction := "Ascending" (: Descending :)


(:~ pomocná proměnná; vrací všechny požadované knihy z dotazu :)
let $books := $query-criteria/r:ResultSearchCriteriaContract/r:ResultBooks/a:BookVersionPairContract
(:~ sekvence všech identifikátorů zdrojů, které se mají prohledat :)
let $book-ids := $books/a:Guid/text()
(:~ sekvence všech verzí zdrojů, které se mají prohledat; upraveno tak, aby se dalo odkazovat na hodnotu @change :)
let $book-version-ids := $books/a:VersionId/concat('#', text())

(:~ relativní cesta k prohledávané kolekci :)
let $collection-path := $coll:collection-path
(:~ výchozí kolekce prohledávaných dokumentů :)
let $collection := collection($collection-path)
let $collection := $collection[./tei:TEI[@n = $book-ids][@change = $book-version-ids]] 

(:~ dokumenty, které obsahují hledaný výraz :)
(:~ TODO: dodat řazení, více proledávaných elementů :)
(:let $documents := search:get-query-documents-matches($collection, $queries):)

(:let $hits := search:get-query-document-hits($collection, $queries):)

let $hits := search:get-search-corpus-hits($collection, $queries)
let $hits-count := count($hits)
(:let $hits-count := search:get-match-count($hits):)

(:let $hits-count := local:get-match-count-mock($hits):)

return $hits-count