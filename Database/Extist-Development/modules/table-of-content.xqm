xquery version "3.0";

module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/tableOfContent";

declare default collation "http://exist-db.org/collation?lang=CS-cz";
declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
declare namespace itj = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0";

declare namespace util = "http://exist-db.org/xquery/util";
declare namespace exist = "http://exist.sourceforge.net/NS/exist";

declare function vw:getTableOfContent($document as node())
    as node()? {    
    let $result := $document//itj:tableOfContent
    return $result 
};
    

    
    