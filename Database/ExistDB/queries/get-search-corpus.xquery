xquery version "3.0";
import module namespace search = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "../modules/searching.xqm";
import module namespace kwic="http://exist-db.org/xquery/kwic";
import module namespace coll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";
import module namespace trans = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/transformation" at "../modules/transformation.xqm";
import module namespace qry = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/querying" at "../modules/querying.xqm";

import module namespace tic="http://vokabular.ujc.cas.cz/ns/it-jakub/xquery/tic" at "../modules/tic.xqm";


declare namespace itji="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info";
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


(:declare option output:cdata-section-elements "*:Before *:After *:string";:)


    declare function local:get-matches-tic($hits as node()*, $query-terms as element(query-terms)) {
    	let $kwic := $query-terms/kwic
    	let $relevant-hits := subsequence($hits, $kwic/@start, $kwic/@count)
    	
    	let $xsl-path := $trans:transformation-path || "matchToCorpusHtml.xsl"
        let $template := doc(escape-html-uri($xsl-path)) 
        
        let $template-notes-path := $trans:transformation-path || "matchToCorpusHtmlNotes.xsl"
	    let $template-notes := doc($template-notes-path)


    	
	return 
	<CorpusSearchResultContractList xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results"
		xmlns:i="http://www.w3.org/2001/XMLSchema-instance"
		xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">
		<SearchResults>
		{
		for $hit at $position in $relevant-hits
		let $xml-id := string($hit/ancestor::tei:TEI/@n)
		let $version-id := substring-after($hit/ancestor::tei:TEI/@change, '#')
		
		let $match := tic:get-tic($hit, $kwic/@context-length)
			return 
				<CorpusSearchResultContract  xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
					<BookXmlId>{$xml-id}</BookXmlId>
						{	
						let $pb := local:prepare-pb($hit)
						let $l := local:prepare-l($hit)
						let $bible := local:prepare-bible($hit)
                        return (<HitResultContext  xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
								{local:prepare-match-from-info($match, $template, $template-notes)}
						</HitResultContext>, $pb, $l, $bible)								
					}
					<VersionId>{$version-id}</VersionId>
				</CorpusSearchResultContract>
		}
		</SearchResults>
		</CorpusSearchResultContractList>
    };

declare function local:get-matches-mock($hits as node()*, 
	$kwic-start as xs:int, 
	$kwic-count as xs:int, 
	$kwic-context-length as xs:int) as node()* {
	
	let $kwic-options := <config width="{$kwic-context-length}" />
	
	let $relevant-hits := subsequence($hits, $kwic-start, $kwic-count)
	return 
	<CorpusSearchResultContractList xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results"
		xmlns:i="http://www.w3.org/2001/XMLSchema-instance"
		xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">
		<SearchResults> {
	for $hit at $position in $relevant-hits
		let $xml-id := string($hit/ancestor::tei:TEI/@n)
		let $version-id := substring-after($hit/ancestor::tei:TEI/@change, '#')
		
		let $matches := kwic:summarize($hit, $kwic-options)
			return 
				<CorpusSearchResultContract  xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
					<BookXmlId>{$xml-id}</BookXmlId>
					{for $match in $matches[1]
						let $pb := local:prepare-pb($hit)
						let $l := local:prepare-l($hit)
						let $bible := local:prepare-bible($hit)
						(:return 	(<Context>{local:prepare-match($match)}</Context>, $pb, $l):)
						
						return if($position mod 3 eq 0) then
									local:get-match-with-notes-mock($match, $pb, $l, $bible)
								else
								(<HitResultContext  xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
								{local:prepare-match($match)}</HitResultContext>, $pb, $l, $bible)
					}
					<VersionId>{$version-id}</VersionId>
				</CorpusSearchResultContract>
		}
		</SearchResults>
		</CorpusSearchResultContractList>
	};
	
	declare function local:get-match-with-notes-mock($match, $pb, $l, $bible) {
		let $new-match := 
		<HitResultContext xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
				<After>tvój. Protož<span class="note-ref">1</span> konečně pravím to, že nikakež ode...</After>
				<Before> touto strašitedlnú nemocí<span class="note-ref">ac</span> a ranou ostříhati a brániti a své svaté</Before>
				<Match>slovo</Match>
				<Notes xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">
					<a:string><span class="note-ref">1</span> poznámka textová</a:string>
					<a:string><span class="note-ref">ac</span> <span class="italic">nemocí</span>] nemo </a:string>
				</Notes>
		</HitResultContext>
		
		return
		($new-match, $pb, $l, $bible)
	};


declare function local:prepare-bible($hit as node()?) as node()? {
	let $book := $hit//tei:anchor[@type='bible'][@subtype='book'][1]
	let $chapter := $hit//tei:anchor[@type='bible'][@subtype='chapter'][1]
	let $verse := $hit//tei:anchor[@type='bible'][@subtype='verse'][1]
	
	
	
	return if ($book) then
	<BibleVerseResultContext  xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
		<BibleBook>{string($book/@n)}</BibleBook>
		<BibleChapter>{string($chapter/@n)}</BibleChapter>
		<BibleVerse>{string($verse/@n)}</BibleVerse>
	</BibleVerseResultContext>
	else ()
};


declare function local:prepare-pb($hit as node()?) as node()? {
	let $element := $hit//tei:pb[1]
	let $element := if ($element) then
			$element
		else
			(:$hit/parent::*//tei:pb[1]:)
			$hit/preceding::tei:pb[1]
	return if ($element) then
	<PageResultContext  xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
		<PageName>{string($element/@n)}</PageName>
		<PageXmlId>{string($element/@xml:id)}</PageXmlId>
	</PageResultContext>
	else ()
};

declare function local:prepare-l($hit as node()?) as node()? {
	let $element := $hit/self::tei:l
	return if ($element and $element/@n) then
	<VerseResultContext  xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
		<VerseName>{string($element/@n)}</VerseName>
		<VerseXmlId>{string($element/@xml:id)}</VerseXmlId>
	</VerseResultContext>
	else ()
};

declare function local:prepare-match-from-info($match as element(itji:result),
$template as item(), $template-notes as node()) as node()* {
	let $before := $match/itji:before
	let $hit := $match/itji:match
	let $after := $match/itji:after
		
	let $notes-before := local:tranform-match-to-html($before, $template-notes)
	let $notes-after := local:tranform-match-to-html($after, $template-notes)
	
	let $before := local:tranform-match-to-html($before, $template)
    let $hit := local:tranform-match-to-html($hit, $template)
	let $after := local:tranform-match-to-html($after, $template)
	
	return ($after, $before, $hit, ($notes-before, $notes-after))
};

declare function local:tranform-match-to-html($element as element(), $template as item()) {
    let $result := transform:transform($element, $template, ())
    return $result
};



declare function local:prepare-match($match as element(p)) as node()* {
	let $before := $match/span[@class='previous']/text()
	let $hit := $match/span[@class='hi']/text()
	let $after := $match/span[@class='following']/text()
	return
		(<After xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">{$after}</After>,
		<Before xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">{$before}</Before>,
		<Match xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">{$hit}</Match>)
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

let $query-terms := qry:get-query-terms($query-criteria-param)

let $result := $query-terms

let $books := $query-terms/documents/document
(:~ sekvence všech identifikátorů zdrojů, které se mají prohledat :)
let $book-ids := $books/@id
(:~ sekvence všech verzí zdrojů, které se mají prohledat; upraveno tak, aby se dalo odkazovat na hodnotu @change :)
let $book-version-ids := $books/@version-id


(:~ relativní cesta k prohledávané kolekci :)
let $collection-path := $coll:collection-path
(:~ výchozí kolekce prohledávaných dokumentů :)
let $collection := collection($collection-path)

let $collection := $collection[./tei:TEI[@n = $book-ids][@change = $book-version-ids]] 

(:let $kwic-config := $query-terms/kwic/config:)

(:~ dokumenty, které obsahují hledaný výraz :)
(:~ TODO: dodat řazení, více proledávaných elementů :)
let $queries := $query-terms/queries
let $documents := search:get-query-documents-matches($collection, $queries)
let $sorted-documents := for $document in $documents 
	order by number($document/tei:TEI/tei:teiHeader//tei:origDate/@notBefore),
		number($document/tei:TEI/tei:teiHeader//tei:origDate/@notAfter) 
	return $document

let $result := $query-terms/result
(:
let $result-documents := if($result/@count = 0) then
		subsequence($sorted-documents, $result/@start)
	else
		subsequence($sorted-documents, $result/@start, $result/@count):)

let $hits := $documents//tei:w[ft:query(., $queries/query, $search:query-options)]

(:let $hits := if($result/@count = 0) then
		subsequence($hits, $result/@start)
	else
		subsequence($hits, $result/@start, $result/@count):)

(:let $hits := search:get-query-document-hits($result-documents, $queries):)

(:let $kwic := $query-terms/kwic:)
(:let $matches := local:get-matches-mock($hits, $kwic/@start, $kwic/@count, $kwic/@context-length):)
let $matches := local:get-matches-tic($hits, $query-terms)

(:let $matches := <empy />:)
let $result := $matches

(:
let $summary := local:get-match-summary($hits)
let $summary-sequence := local:get-hits-sequence($summary, $kwic-start, $kwic-count)
let $matches := local:get-search-result-for-corpus($hits, $summary-sequence, $kwic-context-length)
let $result := ($matches, $summary)
:)

let $xslt-path := $trans:transformation-path || "resultToContractCorpus.xsl"
let $template := doc(escape-html-uri($xslt-path))
let $step := transform:transform($matches, $template, ())

(:let $result := $step:)

let $xslt-path2 := $trans:transformation-path || "resultToContractCorpusHtml-2.xsl"

(:let $result := $query-terms:)

let $template := doc(escape-html-uri($xslt-path2))
let $step := transform:stream-transform($step, $template, ())

let $result := $step
(:let $result := trans:transform-document($step, "Html", $xslt-path2):)

(:let $result := $matches:)


(:let $result := $step:)

(:let $result := <dates xmlns="http://www.tei-c.org/ns/1.0">
	{for $document in $sorted-documents
		return $document/tei:TEI/tei:teiHeader//tei:origDate}
</dates>
:)
(:{for $document in $documents 
	order by number($document/tei:TEI/tei:teiHeader//tei:origDate/@notBefore),
		number($document/tei:TEI/tei:teiHeader//tei:origDate/@notAfter) 
	return $document/tei:TEI/tei:teiHeader//tei:origDate
	}:)
(:	{$sorted-documents/tei:TEI/tei:teiHeader//tei:origDate}:)

return
 	$result