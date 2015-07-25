xquery version "3.0";

module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging";

declare default collation "http://exist-db.org/collation?lang=CS-cz";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

declare namespace util = "http://exist-db.org/xquery/util";
declare namespace exist = "http://exist.sourceforge.net/NS/exist";


declare function vw:getPageByXmlId($document as node(), $page-xml-id as xs:string)
    as element() {
    let $start-point := $document/id($page-xml-id)    
    let $end-point := $start-point/following::tei:pb[1][not(@ed)] (: pouze originální stránky, nikoli strany edice :)
    let $end-point := if($end-point) then $end-point else $start-point/following::tei:pb[1] 
    return vw:getPagesProcess($start-point, $end-point)
    
};

declare function vw:getPageInPosition($document as node(), $page-position as xs:integer)
    as element() {
    let $start-point := $document/descendant::tei:pb[$page-position]    
    let $end-point := $start-point/following::tei:pb[1][not(@ed)] (: pouze originální stránky, nikoli strany edice :)
    let $end-point := if($end-point) then $end-point else $start-point/following::tei:pb[1] 
    return vw:getPagesProcess($start-point, $end-point)
    
};

declare function vw:getPage($document as node(),$pageName as xs:string)
    as element() {
    let $start-point := $document//tei:pb[@n=$pageName]
    let $end-point := $document//tei:pb[@n=$pageName]/following::tei:pb[1]    
    return vw:getPagesProcess($start-point, $end-point)
    
};

declare function vw:getPages($document as node(), $startPageName as xs:string, $endPageName as xs:string)
    as element() {   
    let $start-point := $document//tei:pb[@n=$startPageName]
    let $end-point := $document//tei:pb[@n=$endPageName]    
    return vw:getPagesProcess($start-point, $end-point)
}; 

    
declare function vw:getPagesProcess( 
    $start-point as node()?,
    $end-point as node()?)
    as element() {
    
    let $make-fragment := true()
    let $display-root-namespace := true()
    let $fragment := util:get-fragment-between(
    $start-point, 
    $end-point, 
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

declare function vw:get-pages-only-process( 
    $start-point as node()?,
    $end-point as node()?)
    as node() {
    
    let $make-fragment := true()
    let $display-root-namespace := true()
    let $fragment := util:get-fragment-between(
    $start-point, 
    $end-point, 
    $make-fragment, 
    $display-root-namespace
    )
    let $result := util:parse($fragment)
    return $result
  } ;

declare function vw:getPageNamesList($document as node())
    as node()* {    
    let $result := $document//tei:pb
    return $result 
};

declare function vw:get-document-fragment($document as node(), $start as xs:string, 
$end as xs:string, $page-xml-id as xs:string, $page-position as xs:string) as node() {
	let $documentFragment :=
	if (string-length($start) > 0) then
        if (string-length($end) > 0) then
            vw:getPages($document, $start, $end)
        else
            vw:getPage($document, $start)
    else
		if(string-length($page-xml-id) > 0) then
			vw:getPageByXmlId($document, $page-xml-id)
		else
			vw:getPageInPosition($document, $page-position)
			
	return $documentFragment
};

declare function vw:get-pages-only($document as node(), $start as xs:string, $end as xs:string) as node() {
	<empty />
};
declare function vw:get-page-only($document as node(), $start as xs:string) as node() {
<empty />
};
declare function vw:get-page-by-xml-id-only($document as node(), $page-xml-id as xs:string) as node() {
let $start-point := $document/id($page-xml-id)    
    let $end-point := $start-point/following::tei:pb[1][not(@ed)] (: pouze originální stránky, nikoli strany edice :)
    let $end-point := if($end-point) then $end-point else $start-point/following::tei:pb[1]
    return vw:get-pages-only-process($start-point, $end-point)
};
declare function vw:get-page-in-position-only($document as node(), $page-position as xs:integer) as node() {
<empty />
};


declare function vw:get-document-fragment-only($document as node(), $start as xs:string, 
$end as xs:string, $page-xml-id as xs:string, $page-position as xs:string) as node() {
	let $documentFragment :=
	if (string-length($start) > 0) then
        if (string-length($end) > 0) then
            vw:get-pages-only($document, $start, $end)
        else
            vw:get-page-only($document, $start)
    else
		if(string-length($page-xml-id) > 0) then
			vw:get-page-by-xml-id-only($document, $page-xml-id)
		else
			vw:get-page-in-position-only($document, $page-position)
			
	return $documentFragment
};
    

    
    