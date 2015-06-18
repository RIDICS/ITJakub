xquery version "3.0";

module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging";

declare default collation "http://exist-db.org/collation?lang=CS-cz";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

declare namespace util = "http://exist-db.org/xquery/util";
declare namespace exist = "http://exist.sourceforge.net/NS/exist";


declare function vw:getPageByXmlId($document as node(), $pageXmlId as xs:string)
    as element() {
    let $startPoint := $document/descendant::tei:pb[@xml:id=$pageXmlId]    
    let $endPoint := $startPoint/following::tei:pb[1]    
    return vw:getPagesProcess($startPoint, $endPoint)
    
};

declare function vw:getPageInPosition($document as node(), $pagePosition as xs:integer)
    as element() {
    let $startPoint := $document/descendant::tei:pb[$pagePosition]    
    let $endPoint := $startPoint/following::tei:pb[1]    
    return vw:getPagesProcess($startPoint, $endPoint)
    
};

declare function vw:getPage($document as node(),$pageName as xs:string)
    as element() {
    let $startPoint := $document//tei:pb[@n=$pageName]
    let $endPoint := $document//tei:pb[@n=$pageName]/following::tei:pb[1]    
    return vw:getPagesProcess($startPoint, $endPoint)
    
};

declare function vw:getPages($document as node(), $startPageName as xs:string, $endPageName as xs:string)
    as element() {   
    let $startPoint := $document//tei:pb[@n=$startPageName]
    let $endPoint := $document//tei:pb[@n=$endPageName]    
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
    
    let $resultXml :=  <query-result 
           xmlns:exist="http://exist.sourceforge.net/NS/exist"
           xmlns:tei="http://www.tei-c.org/ns/1.0"
           xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0"
           xmlns:vw="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0">
           <result>
           {$result}
           </result>    
    </query-result>

    return $resultXml    
};

declare function vw:getPageNamesList($document as node())
    as node()* {    
    let $result := $document//tei:pb
    return $result 
};
    

    
    