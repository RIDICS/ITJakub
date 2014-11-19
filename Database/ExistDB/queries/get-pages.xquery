xquery version "3.0";
import module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0" at "../modules/paging.xqm";

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
let $collectionPath := "apps/jacob-test/data"
let $result :=
    if (string-length($start) > 0) then
        if (string-length($end) > 0) then
            vw:getPages($collectionPath, $documentId, $start, $end)
        else
            vw:getPage($collectionPath, $documentId, $start)
    else
        vw:getPageInPosition($collectionPath, $documentId, $pagePosition)
return $result
