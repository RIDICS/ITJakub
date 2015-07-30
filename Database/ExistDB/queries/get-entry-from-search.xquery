xquery version "3.0";
(:module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";:)
import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "../modules/paging.xqm";
import module namespace vwcoll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";
import module namespace vwtrans = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/transformation" at "../modules/transformation.xqm";
import module namespace search = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "../modules/searching.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
declare namespace exist = "http://exist.sourceforge.net/NS/exist"; 
declare namespace itj = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist";


declare option exist:serialize "highlight-matches=elements";

(: working examples
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&start=201r&end=205r
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&start=201r
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&page=2
 :)

let $document-id := request:get-parameter("bookId", "")
let $version-id := request:get-parameter("versionId", "")
let $entry-xml-id := request:get-parameter("xmlEntryId", "")
let $outputFormat := request:get-parameter("outputFormat", "")
let $xsl-path := request:get-parameter("_xsl", "")

let $query-criteria-param := request:get-parameter("serializedSearchCriteria", $search:default-search-criteria)
let $queries := search:get-queries-from-search-criteria-string($query-criteria-param)


let $document := vwcoll:getDocument($document-id, $version-id)

let $entry := $document/id($entry-xml-id)

let $entry-fragment := search:match-hits-for-entry-element($entry, $queries)
(: vracený prvek musí být obalen elementem s xmlns:exist, jinak se <exist:match> nevygeneruje do výstupu :)
let $entry-fragment :=
	if($entry-fragment) then
	<itj:result xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist" xmlns:exist="http://exist.sourceforge.net/NS/exist">{$entry-fragment}</itj:result>
	else ()


let $template := doc(escape-html-uri($xsl-path)) 
let $transformation := 
	if($outputFormat = "Xml") then
			$entry-fragment
	else if ($entry-fragment) then
		if($outputFormat = "Html") 
		then transform:stream-transform($entry-fragment, $template, ())
		else if($outputFormat = "Rtf") 
			then vwtrans:transform-document-to-rtf($entry-fragment, $template)
			else if($outputFormat = "Pdf") 
			then vwtrans:transform-document-to-pdf($entry-fragment, $template)
			else()
		else ()

return if($transformation) then
	$transformation
	else response:set-status-code(404)
