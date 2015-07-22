xquery version "3.0";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
import module namespace kwic="http://exist-db.org/xquery/kwic";
import module namespace vwsearch = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "../modules/search.xqm";
import module namespace vwcollection = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";

(: working examples
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/testMatch.xquery?document={8688926F-9106-4A70-9440-673779415D07}&term=Psi
 :)
 
let $documentId := request:get-parameter("bookId", "")
let $versionId := request:get-parameter("versionId", "")
let $term := request:get-parameter("term", "")
let $document := vwcollection:getDocument($documentId, $versionId)
let $hits := vwsearch:SearchQuery($document, $term)
return <result>{$hits}</result>
