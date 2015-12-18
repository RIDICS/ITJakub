xquery version "3.0";

    (: Token in Context :)
    module namespace tic="http://vokabular.ujc.cas.cz/ns/it-jakub/xquery/tic";
    
    declare namespace tei = "http://www.tei-c.org/ns/1.0";
    declare namespace itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info";
    

    declare function tic:make-info($count as xs:double, $max-count as xs:double) as element (itj:info) {
        <itj:info xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info" count="{$count}" max-count="{$max-count}" />
    };

    declare function tic:get-next-nodes-before($root-node as node(), $current-node as node(), 
        $position as xs:integer, $previous-nodes as node()*) as node()* {
        let $next-nodes :=
        if (not($current-node/ancestor-or-self::tei:body)) then
            ($previous-nodes)
        else
        if($root-node/preceding-sibling::*[$position]) then
                    ($previous-nodes, $root-node/preceding-sibling::*[$position])
                else
                    if($root-node/preceding::*[$position][not(node())]) then (: <pb /> :)
                            tic:get-next-nodes-before($root-node, 
                                $root-node/preceding::*[$position], 
                                $position + 1, 
                                ($previous-nodes, $root-node/preceding::*[$position])
                                )
                        else
                            ($previous-nodes, $root-node/preceding::*[$position])
    
        return $next-nodes
    };
    
    declare function tic:get-next-nodes-after($root-node as node(), $current-node as node(), 
        $position as xs:integer, $previous-nodes as node()*) as node()* {
        let $next-nodes :=
        if (not($current-node/ancestor-or-self::tei:body)) then
            ($previous-nodes)
         else if($root-node/following-sibling::*[$position]) then
                    ($previous-nodes, $root-node/following-sibling::*[$position])
                else
                    if($root-node/following::*[$position][not(node())]) then (: <pb /> :)
                            tic:get-next-nodes-after($root-node, 
                            $root-node/following::*[$position], 
                            $position + 1, 
                            ($previous-nodes, $root-node/following::*[$position])
                            )
                            (: dodělat - výběr nejbližšího možného :)
                        else if($root-node/following::*[$position]/(*//*)[1]) then
                            ($previous-nodes, $root-node/following::*[$position]/(*//*)[1])
                            else
                            ($previous-nodes, $root-node/following::*[$position]/*[1])
    
        return $next-nodes
    };

    declare function tic:get-after($node as node(), $chain as node()*) as node()* {
        let $old-info := $chain[1]
        let $info := $chain[1]
        let $count := number($info/@count)
        let $max-count := number($info/@max-count)
        let $rest-chain := $chain[position() > 1]
        
        let $next-nodes := if ($count >= $max-count) then
              ()
            else
                tic:get-next-nodes-after($node, $node, 1, ())
    
        let $next-node := $next-nodes[last()]
        let $new-count := if($next-node/self::tei:w or $next-node/self::tei:pc) then
            $count + 1
            else 
            $count
        let $info := tic:make-info($new-count, $max-count)
        let $new-chain := ($info, $rest-chain)
    
        let $parent-node := $node/parent::node()
        let $next-parent-node := $next-node/parent::node()
        
        let $new-chain := if($next-node and not (exists($parent-node intersect $next-parent-node))) then
            ($new-chain,
                <itj:element type="end" name="{$parent-node/name()}" />, 
                <itj:element type="start" name="{$next-parent-node/name()}" />,
                $next-nodes
                )
            else
            ($new-chain, (), $next-nodes)
        
        let $result := if(($new-count < $max-count) and $next-node) then
                tic:get-after($next-node, $new-chain)
            else
                $new-chain
                
        return $result
    };

    declare function tic:get-before($node as node(), $chain as node()*) as node()* {
        let $old-info := $chain[1]
        let $info := $chain[1]
        let $count := number($info/@count)
        let $max-count := number($info/@max-count)
        let $new-chain := $chain[position() > 1]
        
        let $next-nodes := if ($count >= $max-count) then
              ()
            else
                tic:get-next-nodes-before($node, $node, 1, ())
        
        let $next-node := $next-nodes[last()]
        let $new-count := if($next-node/self::tei:w or $next-node/self::tei:pc) then
            $count + 1
            else 
            $count
        let $info := tic:make-info($new-count, $max-count)
    
        let $parent-node := $node/..
        let $next-parent-node := $next-node/..
        
        let $new-chain := if($next-node and not(exists($parent-node intersect $next-parent-node))) then
            ($info,
            reverse($next-nodes),
            <itj:element type="end" name="{$next-parent-node/name()}" />,
            <itj:element type="start" name="{$parent-node/name()}" />,
            $new-chain)
            else
            ($info,
            reverse($next-nodes),
            $new-chain, 
            ())
        
        let $result := if(($new-count < $max-count) and $next-node) then
                tic:get-before($next-node, $new-chain)
            else
                $new-chain
                
        return $result
    };

    declare function tic:get-chunk-before($node as node(), $context-length as xs:integer) as node()* {
       
       let $next-node := subsequence(reverse($node/preceding-sibling::tei:w), 1, $context-length)[last()]
       let $text := $node/ancestor::tei:text
       let $fragment := if($next-node) then
            tic:milestone-chunk-ns($next-node, $node, $text)
        else ()
       return $fragment
    };

    declare function tic:get-chunk-after($node as node(), $context-length as xs:integer) as node()* {
       let $next-node := subsequence($node/following-sibling::tei:w, 1, $context-length)[last()] (:subsequence($node/following-sibling::tei:w, 1, $context-length)[last()]:)
       let $text := $node/ancestor::tei:text
       let $fragment := if($next-node) then
        tic:milestone-chunk-ns($node, $next-node, $text)
       else ()
       return $fragment
    };
    
     declare function tic:milestone-chunk-ns(
      $ms1 as node()?,
      $ms2 as node()?,
      $node as node()*
    ) as node()* {
        let $frg-string := util:get-fragment-between($ms1, $ms2, true(), true())
        let $fragment := util:parse(replace($frg-string,"&amp;","&amp;amp;"))
        return $fragment
    };
    
   declare function tic:milestone-chunk(
      $ms1 as element(),
      $ms2 as element(),
      $node as node()*
    ) as node()*
    {
      typeswitch ($node)
        case element() return
          if ($node is $ms1) then $node
          else if ( some $n in $node/descendant::* satisfies ($n is $ms1 or $n is $ms2) )
          then
            element { name($node) }
                    { for $i in ( $node/node() | $node/@* )
                      return tic:milestone-chunk($ms1, $ms2, $i) }
          else if ( $node >> $ms1 and $node << $ms2 ) then $node
          else ()
        case attribute() return $node (: will never match attributes outside non-returned elements :)
        default return 
          if ( $node >> $ms1 and $node << $ms2 ) then $node
          else ()
    };

    declare function tic:milestone-chunk-ns-old(
      $ms1 as element(),
      $ms2 as element(),
      $node as node()*
    ) as node()*
    {
      typeswitch ($node)
        case element() return
          if ($node is $ms1) then $node
          else if ( some $n in $node/descendant::* satisfies ($n is $ms1 or $n is $ms2) )
          then
            (: element { name($node) } :)
               element {QName (namespace-uri($node), name($node))}
                    { for $i in ( $node/node() | $node/@* )
                      return tic:milestone-chunk-ns($ms1, $ms2, $i) }
          else if ( $node >> $ms1 and $node << $ms2 ) then $node
          else ()
        case attribute() return $node (: will never match attributes outside non-returned elements :)
        default return 
          if ( $node >> $ms1 and $node << $ms2 ) then $node
          else ()
    };

    declare function tic:get-tic-old($node as node(), $max-length as xs:integer) {
        let $info := tic:make-info(0, $max-length)
(:        let $after := tic:get-after($node, $info)
        let $before := tic:get-before($node, $info)
        let $parent := $node/parent::*[1]/name()
:)        
(:         let $after := tic:get-after($node, $info):)
        let $after :=   <w xmlns="http://www.tei-c.org/ns/1.0" /> 
        let $before := tic:get-before($node, $info)
        let $parent := $node/parent::*[1]/name()
        
        return
        <itj:result xmlns="http://www.tei-c.org/ns/1.0" 
            xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" 
            xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info">
        <itj:before>{$before}</itj:before>
        <itj:match>{$node}</itj:match>
        <itj:after>{$after}</itj:after>
        </itj:result>
    };
    
       declare function tic:get-tic($node as node(), $context-length as xs:integer) {
        let $after-node := $node/following::node()[1]
        let $after := tic:get-chunk-after($after-node, $context-length)
        let $before := tic:get-chunk-before($node, $context-length)

        return
        <itj:result xmlns="http://www.tei-c.org/ns/1.0" 
            xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" 
            xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info">
        <itj:before>{$before}</itj:before>
        <itj:match>{$node}</itj:match>
        <itj:after>{$after}</itj:after>
        </itj:result>
    };
