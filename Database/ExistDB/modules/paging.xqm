xquery version "3.0";

module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0";
(: declare default element namespace = "http://vokabular.ujc.cas.cz/ns/utils/1.0"; :) 

declare default collation "http://exist-db.org/collation?lang=CS-cz";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

declare namespace util = "http://exist-db.org/xquery/util";
declare namespace exist = "http://exist.sourceforge.net/NS/exist";


declare function vw:getPage(
    $collectionPath as xs:string, 
    $documentId as xs:string, 
    $startPb as xs:string)
    as element() {
    
    let $collection := collection($collectionPath)
    let $document := $collection//tei:TEI[tei:teiHeader/tei:fileDesc[@n=$documentId]]   
    let $startPoint := $document//tei:pb[@n=$startPb]
    let $endPoint := $document//tei:pb[@n=$startPb]/following::tei:pb[position()= 1]    
    return vw:getPagesProcess($startPoint, $endPoint)
    
};

declare function vw:getPages(
    $collectionPath as xs:string, 
    $documentId as xs:string, 
    $startPb as xs:string, 
    $endPb as xs:string)
    as element() {   

    let $collection := collection($collectionPath)
    let $document := $collection//tei:TEI[tei:teiHeader/tei:fileDesc[@n=$documentId]] 
    let $startPoint := $document//tei:pb[@n=$startPb]
    let $endPoint := $document//tei:pb[@n=$endPb]    
    return vw:getPagesProcess($startPoint, $endPoint)
}; 

    
declare function vw:getPagesProcess( 
    $startPoint as node()?,
    $endPoint as node()?)
    as element() {
    
    let $make-fragment := true()
    let $display-root-namespace := true()
    let $fragment := util:get-fragment-between(
    $startPoint, 
    $endPoint, 
    $make-fragment, 
    $display-root-namespace
    )
    let $result := util:parse($fragment)

    return
    
    <vw:itj-query-result 
           xmlns:exist="http://exist.sourceforge.net/NS/exist"
           xmlns:tei="http://www.tei-c.org/ns/1.0"
           xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0"
           xmlns:vw="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0">
           <result>
           {$result}
           </result>    
    </vw:itj-query-result>
    
};
    

    
    