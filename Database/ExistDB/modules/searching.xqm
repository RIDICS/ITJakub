xquery version "3.0";
module namespace search="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search";
declare default collation "?lang=cs-CZ";

import module namespace functx = "http://www.functx.com" at "functx.xqm";


declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

import module namespace kwic="http://exist-db.org/xquery/kwic";

declare namespace a="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching";
declare namespace r="http://schemas.datacontract.org/2004/07/ITJakub.SearchService.Core.Search.DataContract";
declare namespace i="http://www.w3.org/2001/XMLSchema-instance";
declare namespace b="http://schemas.microsoft.com/2003/10/Serialization/Arrays";

declare namespace exist = "http://exist.sourceforge.net/NS/exist"; 
declare option exist:serialize "highlight-matches=elements";


declare variable $search:query-options := <query-options>
     <default-operator>or</default-operator>
     <phrase-slop>1</phrase-slop>
     <leading-wildcard>no</leading-wildcard>
     <filter-rewrite>yes</filter-rewrite>
  </query-options>;


declare function search:get-query-documents-matches-fulltext($root as node()*, $queries as element()) as node()* { 
	let $query := $queries//query[@type='Fulltext']
	return search:match-documents-by-fulltext-elements($root, $query)
} ;

declare function search:get-query-documents-matches-heading($root as node()*, $queries as element()) as node()* { 
	let $query := $queries//query[@type='Heading']
	return search:match-documents-by-heading-elements($root, $query)
} ;

declare function search:get-query-documents-matches-sentence($root as node()*, $queries as element()) as node()* { 
	let $query := $queries//query[@type='Sentence']
	return search:match-documents-by-fulltext-elements($root, $query)
} ;

declare function search:get-query-documents-matches-token-distance($root as node()*, $queries as element()) as node()* { 
	let $query := $queries//query[@type='TokenDistance']								
	return search:match-documents-by-fulltext-elements($root, $query)
} ;

declare function search:match-documents-by-heading-elements($root as node()*, $query as element()?) as node()* {
	if ($root and $query) then
		$root[.//tei:TEI[.//(tei:titlePart | tei:head) [ft:query(., $query, $search:query-options)]]]
	else
		()
} ;

declare function search:match-documents-by-fulltext-elements($root as node()*, $query as element()?) as node()* {
	if ($root and $query) then
		$root[.//tei:TEI[.//(tei:l | tei:p) [ft:query(., $query, $search:query-options)]]]
	else
		()
} ;



declare function search:get-query-document-hits-fulltext($root as node()*, $queries as element()) as item()* { 
	let $query := $queries//query[@type='Fulltext']
	return search:match-hits-by-fulltext-elements($root, $query)
} ;

declare function search:get-query-document-hits-heading($root as node()*, $queries as element()) as item()* { 
	let $query := $queries//query[@type='Heading']
	return search:match-hits-by-heading-elements($root, $query)
} ;

declare function search:get-query-document-hits-sentence($root as node()*, $queries as element()) as item()* { 
	let $query := $queries//query[@type='Sentence']
	return search:match-hits-by-fulltext-elements($root, $query)
} ;

declare function search:get-query-document-hits-token-distance($root as node()*, $queries as element()) as item()* { 
	let $query := $queries//query[@type='TokenDistance']								
	return search:match-hits-by-fulltext-elements($root, $query)
} ;

declare function search:match-hits-by-heading-elements($root as node()*, $query as element()?) as item()* {
	if ($root and $query) then
		$root/tei:TEI//(tei:titlePart | tei:head) [ft:query(., $query, $search:query-options)]
	else
		()
} ;

declare function search:match-hits-by-fulltext-elements($root as node()*, $query as element()?) as item()* {
	if ($root and $query) then
		$root/tei:TEI//(tei:l | tei:p) [ft:query(., $query, $search:query-options)]
	else
		()
} ;

declare function search:get-query-documents-matches-headwords($root as node()*, $queries as element()) as item()* {
	let $query := $queries//query[@type='Headword']
	return search:match-documents-by-headword-elements($root, $query)
} ;

declare function search:match-hits-by-headwords($root as node()*, $queries as element()) as item()* {
	let $query := $queries//query[@type='Headword']
	return search:match-hits-by-headword-elements($root, $query)
} ;

declare function search:get-query-documents-matches-entry($root as node()*, $queries as element()) as item()* {
	let $query := $queries//query[@type='HeadwordDescription']
	return search:match-documents-by-entry-elements($root, $query)
} ;


declare function search:match-hits-by-entry($root as node()*, $queries as element()) as item()* {
	let $query := $queries//query[@type='HeadwordDescription']
	return search:match-hits-by-entry-elements($root, $query)
} ;

declare function search:match-hits-by-entry-elements($root as node()*, $query as element()) as item()* {
	if ($root and $query) then
(:		for $root1 in $root
			for $entry in $root1/tei:TEI//tei:entryFree[ft:query(., $query, $search:query-options)]
			return <hit book-xml-id="{string($root1/tei:TEI/@xml:id)}" entry-id="{$entry/@xml:id}" hw="{subsequence($entry//tei:orth, 1, 1)}"  />
:)
			(:$root/tei:TEI//tei:entryFree[ft:query(., $query, $search:query-options)]:)
			$root/tei:TEI//tei:entryFree[contains(., 'aldran')]
	else
		()
	
};

declare function search:match-hits-by-headword-elements($root as node()*, $query as element()) as item()* {
	if ($root and $query) then
(:		for $root1 in $root
			for $entry in $root1/tei:TEI//tei:entryFree[ft:query(.//tei:form, $query, $search:query-options)]
			return <hit book-xml-id="{string($root1/tei:TEI/@xml:id)}" entry-id="{$entry/@xml:id}" hw="{subsequence($entry//tei:orth, 1, 1)}"  />
:)	
		$root/tei:TEI//tei:entryFree[ft:query(.//tei:form, $query, $search:query-options)]
	else
		()
};

declare function search:get-query-documents-matches($root as node()*, $queries as element()) as node()* {
	let $fulltext := search:get-query-documents-matches-fulltext($root, $queries)
	let $heading := search:get-query-documents-matches-heading($root, $queries)
	let $sentence := search:get-query-documents-matches-sentence($root, $queries)
	let $token-distance := search:get-query-documents-matches-token-distance($root, $queries)
	let $headwords := search:get-query-documents-matches-headwords($root, $queries)
	let $entry := search:get-query-documents-matches-entry($root, $queries)
	
	let $union := $fulltext | $heading | $sentence | $token-distance |  $headwords | $entry
	let $results :=
    (if ($fulltext) then $fulltext else $union)  intersect
    (if ($heading) then $heading else $union)  intersect
    (if ($sentence) then $sentence else $union)  intersect
    (if ($token-distance) then $token-distance else $union)  intersect
    (if ($headwords) then $headwords else $union)  intersect
    (if ($entry) then $entry else $union)
	
	return $results
		
} ;

declare function search:intersect-sequences($item1 as item()*, $item2 as item()*) as item()* {
	let $union := $item1 | $item2
	let $results := (if ($item1) then $item1 else $union)  intersect
    (if ($item2) then $item2 else $union)
	return $results
} ;

declare function search:match-documents-by-headword-elements($root as node()*, $query as element()?) as node()* {
	if ($root and $query) then
		$root[.//tei:TEI[.//tei:entryFree//tei:form[ft:query(., $query, $search:query-options)]]]
	else
		()
} ;

declare function search:match-documents-by-entry-elements($root as node()*, $query as element()?) as node()* {
	if ($root and $query) then
		$root[.//tei:TEI[.//tei:entryFree[ft:query(., $query, $search:query-options)]]]
	else
		()
} ;


(:~
vrací dentifikátor a číslo strany, kde se doklad nachází;
pokud není číslo strany v předávaném elementu, najde se nejbližší předchozí strana v dokumentu (podle xml:id elementu)
:)
declare function search:get-pb-for-hit($document as node(), $expanded-hit as element()) as element()? {
	let $pb := $expanded-hit/preceding::tei:pb[1]
	let $pb := if($pb) then
				$pb
			else
				$document/id(string($expanded-hit/@xml:id))/preceding::tei:pb[1]
	return $pb
} ;

declare function search:get-document-search-hits($document as node()?, 
	$queries as item()?, 
	$kwic-start as xs:double,
	$kwic-count as xs:double,
	$kwic-context-length as xs:int) as element()* {
	
	let $root := $document
	
	let $fulltext := search:get-query-document-hits-fulltext($root, $queries)
	let $heading := search:get-query-document-hits-heading($root, $queries)
	let $sentence := search:get-query-document-hits-sentence($root, $queries)
	let $token-distance := search:get-query-document-hits-token-distance($root, $queries)

	let $hits := ($fulltext, $heading, $sentence, $token-distance)
	
(:	return ($root):)
	
	let $hits-count := count($hits)
	let $return-hits :=
		if($kwic-count > 0) then
			subsequence($hits, $kwic-start, $kwic-count)
		else
			subsequence($hits, $kwic-start)
		
		let $kwic-options := <config width="{$kwic-context-length}" />
		return (<Results xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
		{ for $hit in $return-hits
			
			let $expanded := kwic:expand($hit)
			
			let $pb := search:get-pb-for-hit($document, $expanded)
			
			let $matches := kwic:get-matches($hit)
			let $summary := kwic:summarize($hit, $kwic-options)
			
			return 
			<PageResultContext>
				<ContextStructure>
					<Before>{search:get-kwic-summary-content($summary, 'previous')}</Before>
					<Match>{search:get-kwic-summary-content($summary, 'hi')}</Match>
					<After>{search:get-kwic-summary-content($summary, 'following')}</After>
				</ContextStructure>
				<PageName>{string($pb/@n)}</PageName>
				<PageXmlId>{string($pb/@xml:id)}</PageXmlId>
			</PageResultContext>
		}
		</Results>,
		<TotalHitCount xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">{$hits-count}</TotalHitCount>)	
} ;

declare function search:get-kwic-summary-content($summary as node()*, $class as xs:string) as xs:string {
	subsequence($summary, 1, 1)//*[@class=$class]
} ;

(:~ vrací seznam nalezených výskytů včetně příslušného okolí, identifikátoru a čísla strany, kde se doklad nachází :)
declare function search:get-search-hits($document as node()?, 
	$query as item()?, 
	$query-options as node()?, 
	$kwic-start as xs:double,
	$kwic-count as xs:double,
	$kwic-context-length as xs:int) as element()* {
	
	let $hits := $document//tei:body//(tei:l | tei:p)[ft:query(., $query, $query-options)]
	let $hits-count := count($hits)
	let $return-hits :=
		if($kwic-count > 0) then
			subsequence($hits, $kwic-start, $kwic-count)
		else
			subsequence($hits, $kwic-start)
		
		let $kwic-options := <config width="{$kwic-context-length}" />
		return (<Results xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
		{ for $hit in $return-hits
			
			let $expanded := kwic:expand($hit)
			
			let $pb := search:get-pb-for-hit($document, $expanded)
			
			let $matches := kwic:get-matches($hit)
			let $summary := kwic:summarize($hit, $kwic-options)
			
			return 
			<PageResultContext>
				<ContextStructure>
					<Before>{string($summary//*[@class='previous'][1])}</Before>
					<Match>{string($summary//*[@class='hi'][1])}</Match>
					<After>{string($summary//*[@class='following'][1])}</After>
				</ContextStructure>
				<PageName>{string($pb/@n)}</PageName>
				<PageXmlId>{string($pb/@xml:id)}</PageXmlId>
			</PageResultContext>
			
			
		}
		</Results>,
		<TotalHitCount xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">{$hits-count}</TotalHitCount>)
} ;

declare function search:get-dictionaries-search-hits($dictionaries as node()*, 
$queries as element()?, 
$result-start as xs:double, $result-count as xs:double) as item()* {
	let $headwords := search:match-hits-by-entry($dictionaries, $queries)
	let $entries := search:match-hits-by-headwords($dictionaries, $queries)
(:	let $query := <query><term>našiti</term></query>:)
	(:let $temp := $dictionaries//tei:entryFree[ft:query(., $query, $search:query-options)]:)
	(:let $temp := $dictionaries/tei:TEI//tei:form[ft:query(., 'našiti')]
	let $temp2 := $dictionaries/tei:TEI//tei:entryFree[ft:query(., $query)]
	(\:let $temp := $dictionaries//tei:entryFree[contains(., 'aby')]:\)
	return ($temp, $temp2, $entries):)
	
	let $documents := search:intersect-sequences($headwords, $entries)
	
(:	return ($documents, functx:node-kind($documents)):)
	
		for $document at $position in $documents
			return $document/parent::*
(:			return <result n="{$position}" bookXmlId="{string($document/ancestor::tei:TEI/@xml:id)}" entry-id="{$document/@xml:id}" hw="{subsequence($document//tei:orth, 1, 1)}" />:)
	(:let $entries := for $document in $documents
			return <result bookXmlId="{string($document/preceding::tei:TEI/@xml:id)}" entry-id="{$document/@xml:id}" hw="{subsequence($document//tei:orth, 1, 1)}" />
	let $entries := for $result in $entries
		order by $result/@hw return $result
		
		let $entries := if($result-count < 1) then 
				subsequence($entries, $result-start)
			else
				subsequence($entries, $result-start, $result-count)
		
		return $entries:)

} ;

(:~ převede vyhledávací kritéria na seznam dotazů query pro fulltextové vyhledávání :)
declare function search:get-queries-from-search-criteria($search-criteria  as node()*) as element() {
	let $xslt :=
	<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:dc="http://schemas.datacontract.org/2004/07/ITJakub.SearchService.Core.Search.DataContract"
	xmlns:a="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria"
	xmlns:b="http://schemas.microsoft.com/2003/10/Serialization/Arrays"
	xmlns:i="http://www.w3.org/2001/XMLSchema-instance" exclude-result-prefixes="xs xd dc a b i" version="2.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jul 13, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> lehecka</xd:p>
			<xd:p/>
		</xd:desc>
	</xd:doc>

	<xsl:output method="xml" indent="yes" />
	<xsl:strip-space elements="*"/>

	<xsl:template match="/">
		<queries>
			<xsl:attribute name="types">
				<xsl:value-of select="distinct-values(//a:Key)" separator=" "/>
			</xsl:attribute>
			<xsl:for-each-group select="//a:SearchCriteriaContract" group-by="a:Key">
				<xsl:variable name="key" select="current-grouping-key()"/>
				<query type="{{$key}}">
					<xsl:choose>
						<xsl:when test="count(//a:SearchCriteriaContract[a:Key[. = $key]]) &gt; 1">
							<bool>
								<xsl:apply-templates select="//a:SearchCriteriaContract[a:Key[. = $key]]" />						
							</bool>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates select="//a:SearchCriteriaContract[a:Key[. = $key]]" />
						</xsl:otherwise>
					</xsl:choose>
				</query>
			</xsl:for-each-group>
		</queries>
	</xsl:template>

	<xsl:template match="a:SearchCriteriaContract">
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="a:Disjunctions">
		<xsl:choose>
			<xsl:when test="count(b:string) > 1">
				<bool>
					<xsl:apply-templates select="b:string"/>
				</bool>
			</xsl:when>
			<xsl:when test="count(a:RegexTokenDistanceCriteriaContract) &gt; 1">
				<bool>
					<xsl:apply-templates select="a:RegexTokenDistanceCriteriaContract" />
				</bool>
			</xsl:when>
			<xsl:when test="a:RegexTokenDistanceCriteriaContract">
				<xsl:apply-templates select="a:RegexTokenDistanceCriteriaContract" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="b:string"/>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>
	
	<xsl:template match="a:RegexTokenDistanceCriteriaContract">
		<near slop="{{a:Distance}}">
			<xsl:apply-templates />
		</near>
	</xsl:template>
	
	<xsl:template match="a:Distance" />

	<xsl:template match="b:string | a:FirstRegex | a:SecondRegex">
		<xsl:variable name="result">
			<xsl:call-template name="change-regex">
				<xsl:with-param name="regex" select="."/>
			</xsl:call-template>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="matches($result, '[\?\.\*\[\]\(\)]')">
				<regex><xsl:value-of select="$result"/></regex>
			</xsl:when>
			<xsl:otherwise>
				<term><xsl:value-of select="$result"/></term>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="change-regex">
		<xsl:param name="regex"/>
		<xsl:variable name="result">
			<xsl:value-of select="replace(replace($regex, '^\^(\.\*)?', ''), '(\.\*)?\$$', '')"/>
		</xsl:variable>
		<xsl:variable name="result2">
			<xsl:choose>
					<xsl:when test="matches($result, '^\([^\)\(]+\)$')">
					<xsl:value-of select="replace($result, '[\(\)]', '')"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$result"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:value-of select="$result2"/>
	</xsl:template>
	<xsl:template match="a:Key"/>

	<xsl:template match="dc:ResultBooks" />
</xsl:stylesheet>

	return transform:transform($search-criteria, $xslt, ())

} ;

