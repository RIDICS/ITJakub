xquery version "3.0";
module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

declare function vw:getDocument($documentId as xs:string)
    as node() {    
    let $collectionPath := "apps/jacob-test/data"
    let $collection := collection($collectionPath)
    let $document := $collection//tei:TEI[tei:teiHeader/tei:fileDesc[@n=$documentId]]   
    return $document 
};