xquery version "3.0";
import module namespace search = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "../modules/searching.xqm";
import module namespace kwic="http://exist-db.org/xquery/kwic";
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

declare namespace output = "http://www.w3.org/2010/xslt-xquery-serialization";

declare option exist:serialize "highlight-matches=elements";


declare option output:cdata-section-elements "*:Notes *:Before *:After *:string";

declare function local:get-matches-mock($hits as node()*, 
	$kwic-start as xs:int, 
	$kwic-count as xs:int, 
	$kwic-context-length as xs:int) as node()* {
	
	let $kwic-options := <config width="{$kwic-context-length}" />
	
	let $relevant-hits := subsequence($hits, $kwic-start, $kwic-count)
	
	for $hit in $relevant-hits
		let $xml-id := string($hit/ancestor::tei:TEI/@n)
		let $version-id := substring-after($hit/ancestor::tei:TEI/@change, '#')
		
		let $matches := kwic:summarize($hit, $kwic-options)
			return 
				<Hit>
					<XmlId>{$xml-id}</XmlId>
					<VersionId>{$version-id}</VersionId>
					{for $match in $matches[1]
						return 	<Context>{local:prepare-match($match)}</Context>
					}
				</Hit>
	};

declare function local:prepare-match($match as element(p)) as node()* {
	let $before := $match/span[@class='previous']/text()
	let $hit := $match/span[@class='hi']/text()
	let $after := $match/span[@class='following']/text()
	return
		(<After>{$after}</After>,
		<Before>{$before}</Before>,
		<Match>{$hit}</Match>)
};


declare function local:get-match-count($hit as node()?) as xs:int {
(: text:match-count - funguje jenom pro původní fulltext, nikoli pro lucene :)
	let $expanded-hit := util:expand($hit)
	return count($expanded-hit//exist:match)
};

declare function local:get-match-summary($hits as node()*) as element() {
let $hits-count := count($hits)

let $temp-hits :=
	<hits hits-count="{$hits-count}">
	{
		for $hit at $position in $hits
			let $hit-matches-count := local:get-match-count($hit)
			return 
					<hit n="{$position}" matches="{$hit-matches-count}" />
	}
	</hits>

let $match-count := sum($temp-hits/hit/@matches)

return
	<hits hits-count="{$hits-count}" match-count="{$match-count}">
		{
			for $hit at $position in $temp-hits/hit
				let $hit-matches-count := $hit/@matches
				let $previous-matches-count :=
					if ($position = 1) then 
						0
					else
						sum($temp-hits/hit[position() < $position]/@matches)
(:							$temp-hits/hit[$position - 1]/@matches:)
			return 
				<hit n="{$position}" from="{$previous-matches-count + 1}" to="{$previous-matches-count + $hit-matches-count}"  matches="{$hit-matches-count}" />
		}
	</hits>
};


declare function local:get-match-summary-old($hits as node()*) as element() {
let $match-count := search:get-match-count($hits)
let $hits-count := count($hits)

return
	<hits hits-count="{$hits-count}" match-count="{$match-count}">
		{
			for $hit at $position in $hits
				let $hit-matches-count := search:get-match-count($hit)
				let $previous-matches-count := 0

(:				let $previous-matches-count :=
					if ($position = 1) then 
						0
					else
						search:get-match-count(subsequence($hits, 1, $position - 1))
:)
return 
				<hit n="{$position}" from="{$previous-matches-count + 1}" to="{$previous-matches-count + $hit-matches-count}"  matches="{$hit-matches-count}" />
		}
	</hits>
};


declare function local:get-hits-sequence($summary as element(), $from as xs:int, $count as xs:int) as node()* {
	let $match-count := $summary/@match-count
	let $to := $from + $count  
	let $hits := $summary/hit[(@from <= $to and @to <= $to and (@from >= $from or @to >= $from)) or (@from <= $to and @to >= $to)]
	return 	<hits from="{$from}" to="{$to}">{$hits}</hits>
};


declare function local:get-matches-from-summary ($hits as node()*, 
		$summary as element(hits),
		$kwic-context-length as xs:int) as node()* {
		
	let $kwic-options := <config width="40" />
	let $from := $summary/@from
	let $to := $summary/@to
	
	let $from-all := $summary/hit[1]/@from
	
	let $hits-sequense := subsequence($hits, $summary/hit[1]/@n, $summary/hit[last()]/@n)
	let $all-matches :=
	for $hit in $hits-sequense
		let $xml-id := $hit/ancestor::tei:TEI/@n
		let $version-id := $hit/ancestor::tei:TEI/@change
		let $matches := kwic:summarize($hit, $kwic-options)
			return 
				<Hit>
					<XmlId>{$xml-id}</XmlId>
					<VersionId>{$version-id}</VersionId>
					{for $match in $matches
						return 	<Context>{local:prepare-match($match)}</Context>
					}
				</Hit>
	let $result := subsequence($all-matches, ($from - $from-all) + 1, ($to - $from))   
	return $result
	(: subsequence($all-matches, ($form - $from-all) + 1, $to) :)
	
	};

declare function local:get-search-result-for-corpus($hits as node()*, 
	$summary as element(hits), 
	$kwic-context-length as xs:int) { 
	
		let $kwic-options := <config width="{$kwic-context-length}" />
	
		return
	
		<CorpusSearchResultContractList xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
			<SearchResults>
				{
					let $hits := local:get-matches-from-summary($hits, $summary,  $kwic-context-length)
					return $hits
				} 
			</SearchResults>
		</CorpusSearchResultContractList>
	};

declare function local:get-search-result-for-corpus-1($result-documents as node()*, $queries as element(), 
	$kwic-start as xs:int, $kwic-count as xs:int, $kwic-context-length as xs:int) { 
		<CorpusSearchResultContractList xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
			<SearchResults>
				{
					let $hits := search:get-document-search-hits($result-documents, $queries)
					return $hits
				} 
			</SearchResults>
		</CorpusSearchResultContractList>
	};
	
declare function local:get-search-result-for-corpus-0($result-documents as node()*, $queries as element(), 
	$kwic-start as xs:int, $kwic-count as xs:int, $kwic-context-length as xs:int) {
		<CorpusSearchResultContractList xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
<SearchResults>
	{
let $all-hits := 
for $document in $result-documents
	let $hits := search:get-document-search-matches-by-fragments($document, $queries, 1, 0, $kwic-context-length)
	for $hit at $pos in $hits
		return <CorpusSearchResultContract>
					<BookXmlId>{string($document/tei:TEI/@n)}</BookXmlId>
					{$hit}
					<VersionXmlId>{$document/tei:TEI/substring-after(@change, '#')}</VersionXmlId>
				</CorpusSearchResultContract>
	return if ($kwic-count = 0) then
			subsequence($all-hits, $kwic-start)
		else
			subsequence($all-hits, $kwic-start, $kwic-count)			
}
	
</SearchResults>
</CorpusSearchResultContractList>
	} ;
	
	declare function local:make-cdata($node-set  as node()*) as xs:string {
		"<" || "![CDATA[" ||  $node-set || "]]>"
	};
	
	declare function local:make-notes() as node()* 
	{
						if ($pos mod 3 eq 0) then
						<Notes xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">
							<a:string>{util:serialize(<div><![CDATA[<span><b></span>]]></div>, 'method=xml')}</a:string>
							<a:string>{util:serialize(<span><span class="superscript">{$pos}</span> poznámka textová</span>, 'method=xml')} </a:string>
							<a:string>{util:serialize(<span><span class="superscript">{$pos}</span> poznámka textová</span>, 'method=xml')} </a:string>
							<a:string><![CDATA[<span class="superscript">ac</span> <span class="italic">cestu</span>] cěstú]]></a:string>
						</Notes>
						else ()
					
	};


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
let $collection-path := $coll:collection-path
(:~ výchozí kolekce prohledávaných dokumentů :)
let $collection := collection($collection-path)

let $collection := $collection[./tei:TEI[@n = $book-ids][@change = $book-version-ids]] 

let $kwic-config := <config width="{$kwic-context-length}" preserve-space="yes" format="kwic"/>

(:~ dokumenty, které obsahují hledaný výraz :)
(:~ TODO: dodat řazení, více proledávaných elementů :)
let $documents := search:get-query-documents-matches($collection, $queries)
let $documents := for $document in $documents 
	order by $document/tei:TEI/tei:teiHeader//tei:origDate[@notBefore]
	return $document
	
let $result-documents := if($result-count = 0) then
		subsequence($documents, $result-start)
	else
		subsequence($documents, $result-start, $result-count)


let $hits := search:get-query-document-hits($result-documents, $queries)

let $matches := local:get-matches-mock($hits, $kwic-start, $kwic-count, $kwic-context-length)

(:
let $summary := local:get-match-summary($hits)
let $summary-sequence := local:get-hits-sequence($summary, $kwic-start, $kwic-count)
let $matches := local:get-search-result-for-corpus($hits, $summary-sequence, $kwic-context-length)
let $result := ($matches, $summary)
:)

let $result := $matches
return
 	$result
