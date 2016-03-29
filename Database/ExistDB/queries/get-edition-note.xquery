xquery version "3.0";
(:module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";:)
import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "../modules/paging.xqm";
import module namespace vwcoll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";
import module namespace vwtrans = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/transformation" at "../modules/transformation.xqm";
import module namespace search = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "../modules/searching.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
declare namespace exist = "http://exist.sourceforge.net/NS/exist"; 

declare option exist:serialize "highlight-matches=elements";


let $documentId := request:get-parameter("bookId", "")
let $versionId := request:get-parameter("versionId", "")
let $outputFormat := request:get-parameter("outputFormat", "")
let $xslPath := request:get-parameter("_xsl", "")


let $document := vwcoll:getDocument($documentId, $versionId)

let $fragment := $document//tei:div[@type='editorial']

(:return $fragment:)

let $template := doc(escape-html-uri($xslPath))

let $transformation := 
	if ($outputFormat = "Xml") then
		$fragment
	else if ($fragment) then
	    if ($xslPath = "") then
	    response:set-status-code(404)
	    else
		if($outputFormat = "Html") 
		then transform:stream-transform($fragment, $template, ())
		else if($outputFormat = "Rtf") 
			then vwtrans:transform-document-to-rtf($fragment, $template)
			else if($outputFormat = "Pdf") 
			then vwtrans:transform-document-to-pdf($fragment, $template)
			else()
		else ()


return if($transformation) then
	$transformation
	else if (not($fragment)) then response:set-status-code(204)
	else response:set-status-code(404)