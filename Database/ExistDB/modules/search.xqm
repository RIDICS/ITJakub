module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

import module namespace kwic="http://exist-db.org/xquery/kwic";
import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "../modules/paging.xqm";

declare function vw:SearchQuery($document as node(), $query as xs:string?)
{
    let $hits := vw:do-query($query, $document)
    let $pagesWithHits := vw:get-pages-with-hits($hits, $document)
    let $hitsInPageContext := vw:get-hits-in-page-context($query, $pagesWithHits)
    return $hitsInPageContext
};

declare function vw:do-query($term as xs:string?, $document as node()*)
{
    let $query :=  <query> <term occur="must">{ $term } </term> </query>
    return $document//tei:w[ft:query(., $query)]
};


declare function vw:hit-count($hits as node()*)
{
    count($hits)
};

declare function vw:get-pages-with-hits( $hits as node()*,$document as node())
{
    let $pagesNames := distinct-values($hits/preceding::tei:pb[1]/@n) (: should return id:)
    for $pageName at $p in $pagesNames
    let $page := vwpaging:getPage($document, $pageName)
    return $page
};

declare function vw:get-hits-in-page-context( $query as xs:string?, $pages as node()*)
{
    for $page at $p in $pages    
    let $pageHits := vw:do-query($query, $page)
    return vw:show-hits($pageHits, $page) 
};

(:~
 : Output the actual search result as a div, using the kwic module to summarize full text matches.
 :)
declare function vw:show-hits( $hits as node()*, $page as node())
{
    for $hit at $p in $hits
    let $expanded := kwic:expand($hit/..)
    let $kwic := kwic:get-summary($expanded, ($expanded//exist:match), <config width="15" table="no"/>)
    return
        <div class="search-hit">
            <span class="orig">{$expanded}</span>
            <span class="pagePosition">{ $p - 1 }</span>
            <span class="page"> {$page/@n}</span>
            <span>{ $kwic }</span>
        </div>
};



