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

    declare function local:get-tick-mock-parent($hit as node()) as node() {
        let $result := if($hit/self::tei:p or $hit/self::tei:l) then
            $hit
        else
            local:get-tick-mock-parent($hit/parent::*[1])
    return $hit

    };
    
    
    declare function local:get-tick-mock-test($hit as node()) as node() {
    
(:    let $parent := local:get-tick-mock-parent($hit):)
    
    let $parent := $hit/ancestor::tei:p | $hit/ancestor::tei:l | $hit/ancestor::tei:head
    
    let $text := $hit/ancestor::tei:text

    let $following := if($hit/following-sibling::*[1]) then
            $hit/following-sibling::*[1]
        else 
            $hit
            
    let $starting-node := if($parent/node()[1] eq $parent/node()[last()]) then
            $parent/node()[1]/node()[1]
        else
            $parent/node()[1]

    let $ending-node := if($parent/node()[1] eq $parent/node()[last()]) then
            $parent/node()[1]/node()[last()]
        else
            $parent/node()[last()]

(:    let $before-count := count($hit/preceding-sibling::w |$hit/preceding-sibling::pc) :)

(:    let $before := tic:milestone-chunk-ns($parent/node()[1], $hit, $text)
    let $after := tic:milestone-chunk-ns($following, $parent/node()[last()], $text)
:)
(:    let $before := $parent:)
(:    let $after := $parent:)
    let $node := $hit
    
    return
    <itj:result xmlns="http://www.tei-c.org/ns/1.0" 
            xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" 
            xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info">
            <itj:starting-node>{$starting-node}</itj:starting-node>
            <itj:ending-node>{$ending-node}</itj:ending-node>
            <itj:parent>{$parent}</itj:parent>
            <itj:parent-first>{$parent/node()[1]}</itj:parent-first>
            <itj:parent-last>{$parent/node()[1]}</itj:parent-last>
            <itj:following>{$following}</itj:following>

</itj:result>
        
    };
    

    declare function local:get-tick-mock($hit as node()) as node() {
    
(:    let $parent := local:get-tick-mock-parent($hit):)
    
    let $parent := $hit/ancestor::tei:p | $hit/ancestor::tei:l | $hit/ancestor::tei:head
    
    let $text := $hit/ancestor::tei:text

    let $following := if($hit/following-sibling::*[1]) then
            $hit/following-sibling::*[1]
        else 
            $hit

(:    let $before-count := count($hit/preceding-sibling::w |$hit/preceding-sibling::pc) :)

    let $starting-node := if($parent/node()[1] eq $parent/node()[last()]) then
            $parent/node()[1]/node()[1]
        else
            $parent/node()[1]

    let $ending-node := if($parent/node()[1] eq $parent/node()[last()]) then
            $parent/node()[1]/node()[last()]
        else
            $parent/node()[last()]


    let $before := tic:milestone-chunk-ns($starting-node, $hit, $text)
    let $after := tic:milestone-chunk-ns($following, $ending-node, $text)
(:    let $before := $parent:)
(:    let $after := $parent:)
    let $node := $hit
    
    return
    <itj:result xmlns="http://www.tei-c.org/ns/1.0" 
            xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" 
            xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info">
        <itj:before>{$before}</itj:before>
        <itj:match>{$node}</itj:match>
        <itj:after>{$after}</itj:after>
        </itj:result>
        
    };

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
		
(:		let $match := tic:get-tic($hit, $kwic/@context-length):)
        let $match := local:get-tick-mock($hit)
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

    declare function local:get-hits-sequence($summary as element(), $from as xs:int, $count as xs:int) as node()* {
	let $match-count := $summary/@match-count
	let $to := $from + $count  
	let $hits := $summary/hit[(@from <= $to and @to <= $to and (@from >= $from or @to >= $from)) or (@from <= $to and @to >= $to)]
	return 	<hits from="{$from}" to="{$to}">{$hits}</hits>
    };
	
    declare function local:get-query-terms() {
    let $query-criteria-param := request:get-parameter("serializedSearchCriteria", $search:default-search-criteria)
    return qry:get-query-terms($query-criteria-param)
    };

    declare function local:get-documents($query-terms as item()) {
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

    let $queries := $query-terms/queries
    let $documents := search:get-query-documents-matches($collection, $queries)
    let $sorted-documents := for $document in $documents 
    	order by number($document/tei:TEI/tei:teiHeader//tei:origDate/@notBefore),
    		number($document/tei:TEI/tei:teiHeader//tei:origDate/@notAfter) 
    	return $document

(:    return $sorted-documents:)

    return $collection

    };

    declare function local:transform-results-to-html($matches as item()) {
    let $xslt-path := $trans:transformation-path || "resultToContractCorpus.xsl"
    let $template := doc(escape-html-uri($xslt-path))
    let $step := transform:transform($matches, $template, ())


    let $xslt-path2 := $trans:transformation-path || "resultToContractCorpusHtml-2.xsl"
    let $template := doc(escape-html-uri($xslt-path2))
    let $step := transform:stream-transform($step, $template, ())
    
    return $step
    };
    
    declare function local:get-hits($documents as item()*, $query-terms as element()) {
        let $queries := $query-terms/queries
        let $hits := $documents//tei:w[ft:query(., $queries/query, $search:query-options)][@xml:id]
        return $hits
    };


    let $query-terms := local:get-query-terms()
(:    return $query-terms:)

    let $documents := local:get-documents($query-terms)

    let $hits := local:get-hits($documents, $query-terms)
(:    let $hits := subsequence($hits, 45, 1)
    return <hits xmlns="http://www.tei-c.org/ns/1.0"  xmlns:exist="http://exist.sourceforge.net/NS/exist">
    {
    for $hit in $hits
        return local:get-tick-mock-test($hit)
     return <hit n="{$hit/ancestor::tei:TEI/@n}"> {$hit, $hit/ancestor::tei:p | $hit/ancestor::tei:l | $hit/ancestor::tei:head } </hit>
     }
    </hits>:)

(:    return $hits:)
    
    let $matches := local:get-matches-tic($hits, $query-terms)
(:    return $matches:)

    let $result := local:transform-results-to-html($matches)
    return $result
    