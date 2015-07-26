xquery version "3.0";
import module namespace search = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "../modules/searching.xqm";

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

(:let $query-criteria-param := if (system:function-available(request:get-parameter, 2)) then
	request:get-parameter("serializedSearchCriteria", "")
	else
	local:get-query-criteria-param()
:)	


declare function local:get-search-result-old($result-documents as node()*, $queries as element(), 
	$kwic-start as xs:int, $kwic-count as xs:int, $kwic-context-length as xs:int) {
		<SearchResultContractList xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
<SearchResults>
	{
for $document in $result-documents
	let $hits := search:get-document-search-hits($document, $queries, $kwic-start, $kwic-count, $kwic-context-length)
	return <SearchResultContract>
					<BookXmlId>{string($document/tei:TEI/@n)}</BookXmlId>
					{$hits}
					<VersionXmlId>{$document/tei:TEI/substring-after(@change, '#')}</VersionXmlId>
				</SearchResultContract>
} </SearchResults>
</SearchResultContractList>
	} ;
	
	declare function local:get-search-result-new($result-documents as node()*, $queries as element(), 
	$kwic-start as xs:int, $kwic-count as xs:int, $kwic-context-length as xs:int) {
		<SearchResultContractList xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
<SearchResults>
	{
for $document in $result-documents
	let $hits := search:get-document-search-hits-by-fragments($document, $queries, $kwic-start, $kwic-count, $kwic-context-length)
	return <SearchResultContract>
					<BookXmlId>{string($document/tei:TEI/@n)}</BookXmlId>
					{$hits}
					<VersionXmlId>{$document/tei:TEI/substring-after(@change, '#')}</VersionXmlId>
				</SearchResultContract>
} </SearchResults>
</SearchResultContractList>
	} ;

let $query-criteria-param := request:get-parameter("serializedSearchCriteria", $search:default-search-criteria)
let $query-criteria := util:parse($query-criteria-param) (: ve vyšších verzích parse-xml :)

let $queries := search:get-queries-from-search-criteria($query-criteria(://a:SearchCriteriaContract[a:Key = 'Fulltext']:))


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

(:~ kriterium použité pro seřazení výsledků :)
let $sort-criterion := if($result-params/sc:Sorting) then
	if($result-params/sc:Sorting[@i:nil='true']) then
		"Title"
	else $result-params/sc:Sorting/text() 
	else "Title"
	
(:~ kriterium použité pro seřazení výsledků :)
let $sort-direction := if($result-params/sc:Direction) then $result-params/sc:Direction/text() else "Ascending" (: Descending :)




(:~ pomocná proměnná; vrací všechny požadované knihy z dotazu :)
let $books := $query-criteria/r:ResultSearchCriteriaContract/r:ResultBooks/a:BookVersionPairContract
(:~ sekvence všech identifikátorů zdrojů, které se mají prohledat :)
let $book-ids := $books/a:Guid/text()
(:~ sekvence všech verzí zdrojů, které se mají prohledat; upraveno tak, aby se dalo odkazovat na hodnotu @change :)
let $book-version-ids := $books/a:VersionId/concat('#', text())

(:~ relativní cesta k prohledávané kolekci :)
let $collection-path := "/apps/jacob/data/"
(:~ výchozí kolekce prohledávaných dokumentů :)
let $collection := collection($collection-path)

let $collection := $collection[./tei:TEI[@n = $book-ids][@change = $book-version-ids]] 


(:~ dokumenty, které obsahují hledaný výraz :)
(:~ TODO: dodat řazení, více proledávaných elementů :)
let $documents := search:get-query-documents-matches($collection, $queries)
let $documents := for $document in $documents 
	(:order by $document/tei:TEI/tei:teiHeader//tei:origDate[@notBefore]:)
		order by $document/tei:TEI/tei:teiHeader//tei:titleStmt/tei:title
	return $document
	
let $result-documents := if($result-count = 0) then
		subsequence($documents, $result-start)
	else
		subsequence($documents, $result-start, $result-count)

return local:get-search-result-new($result-documents, $queries, $kwic-start, $kwic-count, $kwic-context-length)
