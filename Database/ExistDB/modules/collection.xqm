xquery version "3.0";
module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
declare namespace itj = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0";

declare variable $vw:collectionPath := "apps/jacob/data";

declare function vw:getDocument($document-id as xs:string, $document-version-id as xs:string?)
    as node() {
    let $result := if (string-length($document-version-id) > 0) then
    	vw:getDocumentVersion($document-id, $document-version-id)
    else
     (let $collection := collection($vw:collectionPath)
    let $document := $collection//tei:TEI[tei:teiHeader/tei:fileDesc[@n=$document-id]]   
    return $document[1] (: get first version :)
    )
    return $result
};
(: identická funkce jako getDocument (kde je versionId volitelný parametr), časem odstranit :)
declare function vw:getDocumentVersion($document-id as xs:string, $document-version-id as xs:string)
    as node() { 
    let $collection := collection($vw:collectionPath)
    let $document := $collection//tei:TEI[@change=fn:concat('#',$document-version-id) and tei:teiHeader/tei:fileDesc[@n=$document-id]]   
    return $document 
};


declare function vw:getDocumentMetadata($document-id as xs:string)
    as node() {    
    let $collection := collection($vw:collectionPath)
    let $document := $collection//itj:document[tei:teiHeader/tei:fileDesc[@n=$document-id]]   
    return $document[1] (: get first version :)
};

declare function vw:getDocumentVersionMetadata($document-id as xs:string, $document-version-id as xs:string)
    as node() { 
    let $collection := collection($vw:collectionPath)
    let $document := $collection//itj:document[@versionId=$document-version-id and tei:teiHeader/tei:fileDesc[@n=$document-id]]   
    return $document 
};

declare function vw:get-collection-path($initial-path as xs:string,
	$document-id as xs:string, $document-version-id as xs:string) as xs:string {

let $initial-path := if (ends-with($initial-path, "/")) then $initial-path else $initial-path || "/"
let $collection-path := $initial-path || $document-id || "/" || $document-version-id || "/"
let $collection-path := escape-html-uri($collection-path)
return $collection-path

} ;

declare function vw:get-collection($initial-path as xs:string,
$document-id as xs:string, $document-version-id as xs:string) as node()* {
	let $collection-path := vw:get-collection-path($initial-path, $document-id, $document-version-id)
	return collection($collection-path)
} ;