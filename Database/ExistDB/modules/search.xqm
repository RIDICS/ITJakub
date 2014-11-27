module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
import module namespace kwic="http://exist-db.org/xquery/kwic";

declare function vw:SearchQuery($document as node(), $query as xs:string?)
{
    let $hits := vw:do-query($query, $document)
    return <result>
            <hitsCount>{vw:hit-count($hits)}</hitsCount>  
            <hits>{vw:show-hits($hits)}</hits>
            </result>
};

declare function vw:do-query($queryStr as xs:string?, $document as node()*)
{
    let $query :=  <query> {for $term in tokenize($queryStr, '\s') return <term occur="must">{ $term }</term> } </query>
    for $hit in $document//tei:w[ft:query(., $query)]
    return $hit
};




declare function vw:hit-count($hits as node()*)
{
    count($hits)
};

(:~
 : Output the actual search result as a div, using the kwic module to summarize full text matches.
 :)
declare function vw:show-hits( $hits as node()*)
{
    for $hit at $p in $hits
    let $expanded := kwic:expand($hit/..)
    let $kwic := kwic:get-summary($expanded, ($expanded//exist:match), <config width="15" table="no"/>)
    let $page := $hit/preceding::tei:pb[1]
    return
        <div class="search-hit">
            <span class="number">{ $p - 1 }</span>
            <span class="page"> {$page/@n}</span>
            <span>{ $kwic }</span>
        </div>
};



