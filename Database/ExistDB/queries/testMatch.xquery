xquery version "3.0";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
import module namespace kwic="http://exist-db.org/xquery/kwic";
import module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0" at "../modules/search.xqm";

(: working examples
 : http://localhost:8080/exist/rest/db/apps/jacob-test/queries/testMatch.xquery?document={125A0032-03B5-40EC-B68D-80473CC5653A}&term=Psi
 :)
 
let $documentId := request:get-parameter("document", "")
let $term := request:get-parameter("term", "")
let $collectionPath := "apps/jacob-test/data"
let $collection := collection($collectionPath)

let $document := $collection//tei:TEI[tei:teiHeader/tei:fileDesc[@n=$documentId]]   
let $hits := vw:SearchQuery($document, $term)
return $hits
