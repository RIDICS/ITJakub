xquery version "3.0";
(:module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";:)
import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "../modules/paging.xqm";
import module namespace vwcoll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";
import module namespace vwtrans = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/transformation" at "../modules/transformation.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

(: working examples
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&start=201r&end=205r
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&start=201r
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&page=2
 :)



let $start := request:get-parameter("start", "")
let $end := request:get-parameter("end", "")
let $documentId := request:get-parameter("bookId", "")
let $versionId := request:get-parameter("versionId", "")
let $pagePosition := request:get-parameter("page", 1)
let $pageXmlId := request:get-parameter("pageXmlId", "")
let $outputFormat := request:get-parameter("outputFormat", "")
let $xslPath := request:get-parameter("_xsl", "")
let $document := vwcoll:getDocument($documentId, $versionId)

let $documentFragment := vwpaging:get-document-fragment($document, $start, $end, $pageXmlId, $pagePosition)

(:let $xslPath := "/db/apps/jacob/transformations/pageToHtml.xsl":)
let $template := doc(escape-html-uri($xslPath)) 
let $transformation := 
	if($outputFormat = "Html") 
	then transform:stream-transform($documentFragment, $template, ())
	else if($outputFormat = "Rtf") 
		then vwtrans:transform-document-to-rtf($documentFragment, $template)
		else if($outputFormat = "Pdf") 
		then vwtrans:transform-document-to-pdf($documentFragment, $template)
		else()

return $transformation
