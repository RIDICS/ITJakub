xquery version "3.0";
import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "../modules/paging.xqm";
import module namespace vwcollection = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

(: working examples
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&start=201r&end=205r
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&start=201r
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/get-pages.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&page=2
 :)
  
let $start := request:get-parameter("start", "")
let $end := request:get-parameter("end", "")
let $documentId := request:get-parameter("document", "")
let $pagePosition := request:get-parameter("page", 1)
let $document := vwcollection:getDocument($documentId)
let $result :=
    if (string-length($start) > 0) then
        if (string-length($end) > 0) then
            vwpaging:getPages($document, $start, $end)
        else
            vwpaging:getPage($document, $start)
    else
        vwpaging:getPageInPosition($document, $pagePosition)
return $result
