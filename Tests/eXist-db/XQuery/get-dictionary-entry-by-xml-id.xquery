xquery version "3.0";
import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "/db/apps/jacob/modules/paging.xqm";
import module namespace vwcoll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "/db/apps/jacob/modules/collection.xqm";
import module namespace vwtrans = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/transformation" at "/db/apps/jacob/modules/transformation.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

(:
let $documentId := request:get-parameter("bookId", "")
let $versionId := request:get-parameter("versionId", "")
let $entryXmlId := request:get-parameter("xmlEntryId", "")
let $outputFormat := request:get-parameter("outputFormat", "")
let $xslPath := request:get-parameter("_xsl", "")
:)


let $documentId := "4E5DB418-B49B-4AC0-AE9F-78A53E9BE4FE"
let $versionId	:= "d02671ac-cabd-4655-980c-bdaa1c435cff"
let $entryXmlId 	:= "en000001"
let $templateId := "dictionaryToHtml.xsl"
let $xslPath := "/db/apps/jacob/transformations/" || $templateId
let $outputFormat := "Html"

(:let $xslPath := "/db/apps/jacob/transformations/Xsl-fo/tei-to-xsl-fo-pdf.xsl"
let $outputFormat := "Pdf"
:)

let $document := vwcoll:getDocument($documentId, $versionId)


let $entryFragment := $document/id($entryXmlId)


let $template := doc($xslPath)
(:let $transformation := transform:transform($documentFragment, $template, ()):)
let $transformation := 
	if($outputFormat = "Html") 
	then transform:transform($entryFragment, $template, ())
	else if($outputFormat = "Rtf") 
		then vwtrans:transform-document-to-rtf($entryFragment, $template)
(:		then vwtrans:transform-document-to-pdf($documentFragment, $template, ()):)
		else if($outputFormat = "Pdf") 
		then vwtrans:transform-document-to-pdf($entryFragment, $template)
		(:then vwtrans:transform-document-to-pdf($documentFragment, $template, (), ()):)
		else()

return $transformation