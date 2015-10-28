xquery version "3.0";

module namespace qry = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/querying";
import module namespace search = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "searching.xqm";

declare default collation "?lang=cs-CZ";

declare namespace a="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching";
declare namespace r="http://schemas.datacontract.org/2004/07/ITJakub.SearchService.Core.Search.DataContract";
declare namespace i="http://www.w3.org/2001/XMLSchema-instance";
declare namespace b="http://schemas.microsoft.com/2003/10/Serialization/Arrays";
declare namespace sc="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria";

declare function qry:get-query-terms($search-criteria-string as xs:string) as element(query-terms) {

    let $query-criteria := util:parse($search-criteria-string) (: ve vyšších verzích parse-xml :)
    
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
    			0
    	else xs:int($result-params/sc:Count) else 
    			25
    
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
    (:let $book-ids := $books/a:Guid/text():)
    (:~ sekvence všech verzí zdrojů, které se mají prohledat; upraveno tak, aby se dalo odkazovat na hodnotu @change :)
    (:let $book-version-ids := $books/a:VersionId/concat('#', text()):)
    
    let $documents := <documents>
        {
            for $book in $books
            return <document id="{$book/a:Guid/text()}" version-id="{$book/a:VersionId/concat('#', text())}" />
        }
    </documents>
    
    return <query-terms>
        {$queries}
        <result start="{$result-start}" count="{$result-count}" />
        <kwic start="{$kwic-start}" count="{$kwic-count}" context-length="{$kwic-context-length}">
            <config width="{$kwic-context-length}" preserve-space="yes" format="kwic"/>
        </kwic>
        <sorting criterion="{$sort-criterion}" direction="{$sort-direction}" />
        {$documents}
    </query-terms>

};