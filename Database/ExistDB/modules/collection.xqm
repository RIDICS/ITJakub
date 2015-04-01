xquery version "3.0";
module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

declare variable $vw:collectionPath := "apps/jacob/data";

declare function vw:getDocument($documentId as xs:string)
    as node() {    
    let $collection := collection($vw:collectionPath)
    let $document := $collection//tei:TEI[tei:teiHeader/tei:fileDesc[@n=$documentId]]   
    return $document[1] (: get first version :)
};

declare function vw:getDocumentVersion($documentId as xs:string, $documentVersionId as xs:string)
    as node() { 
    let $collection := collection($vw:collectionPath)
    let $document := $collection//tei:TEI[@change=fn:concat('#',$documentVersionId) and tei:teiHeader/tei:fileDesc[@n=$documentId]]   
    return $document 
};