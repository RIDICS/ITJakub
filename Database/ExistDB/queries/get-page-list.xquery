xquery version "3.0";
import module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0" at "../modules/paging.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

(: working example
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages-list.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}
 :)
 
let $documentId := request:get-parameter("document", "")
let $collectionPath := "apps/jacob-test/data"
return vw:getPageNamesList($collectionPath, $documentId)
