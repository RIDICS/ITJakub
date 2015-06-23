xquery version "3.0";
import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "/db/apps/jacob/modules/paging.xqm";
import module namespace vwcollection = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "/db/apps/jacob/modules/collection.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

let $documentId := "{E494DBC5-F3C4-4841-B4D3-C52FE99839EB}"
let $versionId	:= "bac7c459-3bfe-4441-b086-c39cda0d9455"
let $pageXmlId 	:= "t-1.body-1.pb-1"
let $templateId := "pageToHtml.xsl"

let $pageXmlId 	:= "t-1.body-1.div-1.div-1.div-1.div-1.p-1.pb-2"
let $pageXmlId := "t-1.body-1.div-1.div-1.div-2.div-1.p-2.pb-1"
let $pageXmlId := "t-1.body-1.div-1.div-1.div-2.div-1.p-2.pb-2"

let $document := vwcollection:getDocument($documentId, $versionId)

let $result := vwpaging:get-document-fragment($document, "", "", $pageXmlId, "")
(:let $result := vwpaging:getPageByXmlId($document, $pageXmlId):)

let $templatePath := "/db/apps/jacob/transformations/" || $templateId
let $templatePath := "/db/apps/jacob/transformations/pageToHtml.xsl"
let $template := doc($templatePath) 
let $transformation := transform:transform($result, $template, ())
(:let $transformation := transform:stream-transform($result, $template, ()):)

(:return ($transformation, $result, $template):)
return ($transformation, $result)
