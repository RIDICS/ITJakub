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