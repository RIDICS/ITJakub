xquery version "3.0";
module namespace coll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
declare namespace itj = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0";

declare variable $coll:collection-path := "apps/jacob/data/";

declare function coll:getDocument($document-id as xs:string, $document-version-id as xs:string?)
    as node() {
    let $result := if (string-length($document-version-id) > 0) then
    	coll:getDocumentVersion($document-id, $document-version-id)
    else
     (let $collection := collection($coll:collection-path)
    let $document := $collection//tei:TEI[tei:teiHeader/tei:fileDesc[@n=$document-id]]   
    return $document[1] (: get first version :)
    )
    return $result
};
(: identická funkce jako getDocument (kde je versionId volitelný parametr), časem odstranit :)
declare function coll:getDocumentVersion($document-id as xs:string, $document-version-id as xs:string)
    as node() { 
    let $collection := collection($coll:collection-path)
    let $document := $collection//tei:TEI[@change=fn:concat('#',$document-version-id) and tei:teiHeader/tei:fileDesc[@n=$document-id]]   
    return $document 
};


declare function coll:getDocumentMetadata($document-id as xs:string)
    as node() {    
    let $collection := collection($coll:collection-path)
    let $document := $collection//itj:document[tei:teiHeader/tei:fileDesc[@n=$document-id]]   
    return $document[1] (: get first version :)
};

declare function coll:getDocumentVersionMetadata($document-id as xs:string, $document-version-id as xs:string)
    as node() { 
    let $collection := collection($coll:collection-path)
    let $document := $collection//itj:document[@versionId=$document-version-id and tei:teiHeader/tei:fileDesc[@n=$document-id]]   
    return $document 
};

declare function coll:get-collection-path($initial-path as xs:string,
	$document-id as xs:string, $document-version-id as xs:string) as xs:string {

let $initial-path := if (ends-with($initial-path, "/")) then $initial-path else $initial-path || "/"
let $collection-path := $initial-path || $document-id || "/" || $document-version-id || "/"
let $collection-path := escape-html-uri($collection-path)
return $collection-path

} ;

declare function coll:get-collection($initial-path as xs:string,
$document-id as xs:string, $document-version-id as xs:string) as node()* {
	let $collection-path := coll:get-collection-path($initial-path, $document-id, $document-version-id)
	return collection($collection-path)
} ;