xquery version "3.0";
module namespace functx = "http://www.functx.com";

declare function functx:path-to-node
  ( $nodes as node()* )  as xs:string* {

$nodes/string-join(ancestor-or-self::*/name(.), '/')
 } ;

declare function functx:distinct-element-names
  ( $nodes as node()* )  as xs:string* {
   distinct-values($nodes/descendant-or-self::*/name(.))
 } ;
 
 declare function functx:distinct-element-paths
  ( $nodes as node()* )  as xs:string* {

   distinct-values(functx:path-to-node($nodes/descendant-or-self::*))
 } ;
 
 declare function functx:node-kind
  ( $nodes as node()* )  as xs:string* {

 for $node in $nodes
 return
 if ($node instance of element()) then 'element'
 else if ($node instance of attribute()) then 'attribute'
 else if ($node instance of text()) then 'text'
 else if ($node instance of document-node()) then 'document-node'
 else if ($node instance of comment()) then 'comment'
 else if ($node instance of processing-instruction())
         then 'processing-instruction'
 else 'unknown'
 } ;