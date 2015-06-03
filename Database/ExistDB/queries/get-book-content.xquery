xquery version "3.0";

import module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/tableOfContent" at "../modules/table-of-content.xqm";
import module namespace vwcollection = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

(: working example
 : http://localhost:8080/exist/rest/db/apps/jacob/queries/get-book-content.xquery?bookId={125A0032-03B5-40EC-B68D-80473CC5653A}
 :)
 
let $documentId := request:get-parameter("bookId", "")
let $versionId := request:get-parameter("versionId", "")
let $document := if (string-length($versionId) > 0) then 
                    vwcollection:getDocumentVersionMetadata($documentId, $versionId)
                 else
                    vwcollection:getDocumentMetadata($documentId)
return <result>{vw:getTableOfContent($document)}</result>
