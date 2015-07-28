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


let $query-criteria-param := request:get-parameter("serializedSearchCriteria", $search:default-search-criteria)
let $query-criteria := util:parse($query-criteria-param) (: ve vyšších verzích parse-xml :)

let $queries := search:get-queries-from-search-criteria($query-criteria)


let $result-params := $query-criteria/r:ResultSearchCriteriaContract/r:ResultSpecifications

(:~ od jakého záznamu se vracejí výsledky, tj. knihy :)
let $result-start := if($result-params/sc:Start) then
		if($result-params/sc:Start[@i:nil='true']) then
			1 
		else xs:int($result-params/sc:Start) else 
			1
(:~ kolik záznamů má být ve vraceném výsledku; 0 znamená všechny; pokud je číslo větší než celkový počet záznamů, vrátí se všechny :)
let $result-count := if($result-params/sc:Count) then
	if($result-params/sc:Count[@i:nil='true']) then
			1
	else xs:int($result-params/sc:Count) else 
			20

(:~ od jakého záznamu se vracejí doklady s výskytem hledaného výrazu :)
let $kwic-start := if($result-params/sc:HitSettingsContract/sc:Start) then xs:int($result-params/sc:HitSettingsContract/sc:Start) else 1
(:~ kolik záznamů s doklady s výskytem hledaného výrazu se vrací; pokud je číslo větší než celkový počet dokladů, vrátí se všechny :)
let $kwic-count := if($result-params/sc:HitSettingsContract/sc:Count) then xs:int($result-params/sc:HitSettingsContract/sc:Count) else 3
(:~ počet znaků zleva a zprava pro zobrazení dokladů :)
let $kwic-context-length := if($result-params/sc:HitSettingsContract/sc:ContextLength) then xs:int($result-params/sc:HitSettingsContract/sc:ContextLength) else 50


let $query-criteria-param := request:get-parameter("serializedSearchCriteria", $search:default-search-criteria)
let $query-criteria := util:parse($query-criteria-param) (: ve vyšších verzích parse-xml :)
let $queries := search:get-queries-from-search-criteria($query-criteria)


(:~ pomocná proměnná; vrací všechny požadované knihy z dotazu :)
let $books := $query-criteria/r:ResultSearchCriteriaContract/r:ResultBooks/a:BookVersionPairContract
(:~ první identifikátor v sekvenci všech identifikátorů zdrojů, které se mají prohledat; používá se k sestavení cesty ke kolekci  :)
let $document-id := $books[1]/a:Guid/text()
(:~ první verze v sekvenci verzí, které se mají prohledat; používá se k sestavení cesty ke kolekci :)
let $document-version-id := $books[1]/a:VersionId/text()

(:return ($query-criteria, $queries, $books, $document-id, $document-version-id):)

let $collection := coll:get-collection("/db/apps/jacob/data", $document-id, $document-version-id)


(:~ dokumenty, které obsahují hledaný výraz :)
(:~ TODO: dodat řazení, více proledávaných elementů :)
let $page-hits := search:get-document-hits-pages-using-fragments($collection, $queries)
return
<PageListContract xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
	<PageList>
		{
		for $page-hit in $page-hits
			return <PageDescriptionContract>
			<PageName>{string($page-hit/@n)}</PageName>
			<PageXmlId>{string($page-hit/@xml:id)}</PageXmlId>
		</PageDescriptionContract>
		}
	</PageList>
</PageListContract>