(:~
    KWIC module: formats query results to display <em>keywords in context</em> (KWIC). A configurable
    amount of text is displayed to the left and right of a matching keyword (or phrase).
    
    The module works with all indexes that support match highlighting (matches are tagged
    with an &lt;exist:match&gt; element). This includes the old full text index, the new 
    Lucene-based full text index, as well as the NGram index.
    
    The <b>kwic:summarize()</b> function represents the main entry point into the module.
    To have more control over the text extraction context, you can also call 
    <b>kwic:get-summary()</b> instead. For example, the following snippet will only print the
    first match within a given set of context nodes ($ancestor):
    
    <pre>
    let $matches := kwic:get-matches($hit)<br/>
    for $ancestor in $matches/ancestor::para | $matches/ancestor::title | $matches/ancestor::td<br/>
    return<br/>
        kwic:get-summary($ancestor, ($ancestor//exist:match)[1], $config)
    </pre>
:)  
module namespace kwic="http://exist-db.org/xquery/kwic";

declare variable $kwic:CHARS_SUMMARY := 120;
declare variable $kwic:CHARS_KWIC := 40;

(:~
	Retrieve the following and preceding text chunks for a given match.

	@param $match the text node containing the match
	@param $mode the selection mode: either "previous" or "following"
:)
declare function kwic:get-context($root as element(), $match as element(exist:match), $mode as xs:string) as node()* {
	let $sibs :=
		if ($mode eq 'previous') then 
			$match/preceding::text()
		else
			$match/text()/following::text()
	for $sib in $sibs[ancestor::* intersect $root]
	return
		if ($sib/parent::exist:match) then
			<span class="hi">{$sib}</span>
		else
			$sib
};

(:~
	Like fn:substring, but takes a node argument. If the node is an element,
	a new element is created with the same node-name as the old one and the
	shortened text content.
:)
declare function kwic:substring($node as item(), $start as xs:int, $count as xs:int) as item()? {
	let $str := substring($node, $start, $count)
	return
		if ($node instance of element()) then
			element { node-name($node) } { $str }
		else
			$str
};

declare function kwic:display-text($text as text()?) as node()? {
    if ($text/parent::exist:match) then
    	<span class="hi">{$text}</span>
    else
        $text
};

declare function kwic:callback($callback as function?, $node as node(), $mode as xs:string) as xs:string? {
    if (exists($callback)) then
        util:call($callback, $node, $mode)
    else
        $node
};

(:~
	Generate the left-hand context of the match. Returns a normalized string, 
	whose total string length is less than or equal to $width characters.

	Note: this function calls itself recursively until $node is empty or
	the returned sequence has the desired total string length.
:)
declare function kwic:truncate-previous($root as node()?, $node as node()?, $truncated as item()*, 
	$width as xs:int, $callback as function?) {
  let $nextProbe := $node/preceding::text()[1]
  let $next := if ($root[not(. intersect $nextProbe/ancestor::*)]) then () else $nextProbe  
  let $probe := 
    if (exists($callback)) then
      concat(for $a in $next return kwic:callback($callback, $nextProbe, "after"), $truncated)
    else concat($nextProbe, ' ', $truncated)
  return
    if (string-length($probe) gt $width) then
      let $norm := concat(normalize-space($probe), ' ')
      return 
        if (string-length($norm) le $width and $next) then
          kwic:truncate-previous($root, $next, $norm, $width, $callback)
        else if ($next) then
          concat('...', substring($norm, string-length($norm) - $width + 1))
        else 
          $norm
    else if ($next) then 
      kwic:truncate-previous($root, $next, $probe, $width, $callback)
    else for $str in normalize-space($probe)[.] return concat($str, ' ')
};

(:~
	Generate the left-hand context of the match. Returns a string which preserves the original whitespace, 
	whose total string length is less than or equal to $width characters.

	Note: this function calls itself recursively until $node is empty or
	the returned sequence has the desired total string length.
:)
declare function kwic:truncate-previous-ps($root as node()?, $node as node()?, $truncated as item()*, 
	$width as xs:int, $callback as function?) {
  let $nextProbe := $node/preceding::text()[1]
  let $next := if ($root[not(. intersect $nextProbe/ancestor::*)]) then () else $nextProbe  
  let $probe := 
    if ($callback) then
      concat(for $a in $next return kwic:callback($callback, $nextProbe, "after"), $truncated)
    else concat($next, $truncated)
  return
    if (string-length($probe) gt $width) then
      if ($next) then
        concat('...', substring($probe, string-length($probe) - $width + 1))
      else 
        $probe
    else if ($next) then 
      kwic:truncate-previous-ps($root, $next, $probe, $width, $callback)
    else $probe
};

(:~
	Generate the right-hand context of the match. Returns a normalized string, 
	whose total string length is less than or equal to $width characters.
	
	Note: this function calls itself recursively until $node is empty or
	the returned sequence has the desired total string length.
:)
declare function kwic:truncate-following($root as node()?, $node as node()?, $truncated as item()*, 
	$width as xs:int, $callback as function?) {
  let $nextProbe := $node/following::text()[1]
  let $next := if ($root[not(. intersect $nextProbe/ancestor::*)]) then () else $nextProbe  
  let $probe := 
    if (exists($callback)) then 
      concat($truncated, for $a in $next return kwic:callback($callback, $nextProbe, "after"))
    else concat($truncated, ' ', $nextProbe)
  return
    if (string-length($probe) gt $width) then
      let $norm := concat(' ', normalize-space($probe))
      return 
        if (string-length($norm) le $width and $next) then
          kwic:truncate-following($root, $next, $norm, $width, $callback)
        else if ($next) then
          concat(substring($norm, 1, $width), '...')
        else 
          $norm
    else if ($next) then 
      kwic:truncate-following($root, $next, $probe, $width, $callback)
    else for $str in normalize-space($probe)[.] return concat(' ', $str)
};

(:~
	Generate the right-hand context of the match. Returns a string which preserves the original whitespace, 
	whose total string length is less than or equal to $width characters.

	Note: this function calls itself recursively until $node is empty or
	the returned sequence has the desired total string length.
:)
declare function kwic:truncate-following-ps($root as node()?, $node as node()?, $truncated as item()*, 
	$width as xs:int, $callback as function?) {
  let $nextProbe := $node/following::text()[1]
  let $next := if ($root[not(. intersect $nextProbe/ancestor::*)]) then () else $nextProbe  
  let $probe := 
    if ($callback) then 
      concat($truncated, for $a in $next return kwic:callback($callback, $nextProbe, "after"))
    else concat($truncated, $nextProbe)
  return
    if (string-length($probe) gt $width) then
      if ($next) then
        concat(substring($probe, 1, $width), '...')
      else 
        $probe
    else if ($next) then 
      kwic:truncate-following-ps($root, $next, $probe, $width, $callback)
    else $probe  
};

(:~
	Computes the total string length of the nodes in the argument sequence
:)
declare function kwic:string-length($nodes as item()*) as xs:integer {
	if (exists($nodes)) then
		sum(for $n in $nodes return string-length($n))
	else
		0
};

(:~
	Print a summary of the match in $node. Output a predefined amount of text to
	the left and the right of the match.
	
	This 3-argument helper function (lacking the $callback argument) just passes 
	an empty $callback argument to the main 4-argument function kwic:get-summary(). 

	@param $root (optional) root element which should be used as context for the match. It defines the
	    boundaries for the text extraction. Text will be taken from this context. 
	@param $node the exist:match element to process.
	@param $config configuration element which determines the behaviour of the function
:)
declare function kwic:get-summary($root as node()?, $node as element(exist:match), 
	$config as element(config)) as element() {
	kwic:get-summary($root, $node, $config, ())
};

(:~
	Print a summary of the match in $node. Output a predefined amount of text to
	the left and the right of the match.

	@param $root (optional) root element which should be used as context for the match. It defines the
	    boundaries for the text extraction. Text will be taken from this context. 
	@param $node the exist:match element to process.
	@param $config configuration element which determines the behaviour of the function
	@param $callback (optional) reference to a callback function which will be called
	once for every text node before it is appended to the displayed text. The function
	should accept 2 parameters: 1) a single text node, 2) a string indicating the
	current direction in which text is appended, i.e. "before" or "after". The function
	may return the empty sequence if the current node should be ignore (e.g. if it belongs
	to a "footnote" which should not be displayed). Otherwise it should return a single
	string.
:)
declare function kwic:get-summary($root as node()?, $node as element(exist:match), 
	$config as element(config), $callback as function?) as element() {
	let $width := xs:int($config/@width)
	let $format := $config/@format
	let $ps := $config/@preserve-space = ('yes', 'true')
	
	let $prevTrunc := if ($ps) then kwic:truncate-previous-ps($root, $node, (), $width, $callback)
	  else kwic:truncate-previous($root, $node, (), $width, $callback)
	let $followingTrunc := if ($ps) then kwic:truncate-following-ps($root, $node, (), $width, $callback)
	  else kwic:truncate-following($root, $node, (), $width, $callback)
	return
		if ($format eq 'p') then
			<p>
				<span class="previous">{$prevTrunc}</span>
				{
					if ($config/@link) then
						<a class="hi" href="{$config/@link}">{ $node/text() }</a>
					else
						<span class="hi">{ $node/text() }</span>
				}
				<span class="following">{$followingTrunc}</span>
			</p>
		else if ($format eq 'table') then
			<tr>
				<td class="previous">{$prevTrunc}</td>
				<td class="hi">
				{
					if ($config/@link) then
						<a href="{$config/@link}">{$node/text()}</a>
					else
						$node/text()
				}
				</td>
				<td class="following">{$followingTrunc}</td>
			</tr>
		else 
		  <KWIC xmlns="http://exist-db.org/xquery/kwic">
		    <prev>{$prevTrunc}</prev>
		    <hit>{$node/text()}</hit>
		    <next>{$followingTrunc}</next>
		  </KWIC>
};

(:~
    Expand the element in $hit. Creates an in-memory copy of the element and marks
    all matches with an exist:match tag, which will be used by all other functions in
    this module. You need to call kwic:expand before kwic:get-summary. 
    kwic:summarize will call it automatically.
:)
declare function kwic:expand($hit as element()) as element() {
    util:expand($hit, "expand-xincludes=no")
};

(:~
    Return all matches within the specified element, $hit. Matches are returned as
    exist:match elements. The returned nodes are part of a new document whose
    root element is a copy of the specified $hit element.
    
    @param $hit an arbitrary XML element which has been selected by one of the full text
		operations or an ngram search.
:)
declare function kwic:get-matches($hit as element()) as element(exist:match)* {
    let $expanded := kwic:expand($hit)
	return $expanded//exist:match
};

(:~
	Takes the passed element and returns an XML fragment containing a chunk of 
	text before and after the first full text match in the node.

	This 2-argument helper function (lacking the $callback argument) just passes 
	an empty $callback argument to the main 3-argument function kwic:summarize(). 

	@param $hit an arbitrary XML element which has been selected by one of the full text
		operations or an ngram search.
	@param $config configuration element which determines the behaviour of the function
:)
declare function kwic:summarize($hit as element(), $config as element(config)) as element()* {
    kwic:summarize($hit, $config, ())
};

(:~
	Main function of the KWIC module: takes the passed element and returns an 
	XML fragment containing a chunk of text before and after the first full text
	match in the node.

	The optional config parameter is used to configure the behaviour of the function:
	
	&lt;config width="character width" format="KWIC|p|table" link="URL to which the match is linked"/&gt;

	By default, kwic:summarize returns an XML fragment with the following structure:

	&lt;KWIC xmlns="http://exist-db.org/xquery/kwic"&gt;
		&lt;prev&gt;Text before match&lt;/prev&gt;
		&lt;hit&gt;The highlighted term&lt;/hit&gt;
		&lt;next&gt;Text after match&lt;/next&gt;
	&lt;/KWIC&gt;

	If format=p is passed with the config element, an XHTML &lt;p> element will be returned, with &lt;span> 
	elements identifying the three chunks by means of @class attributes:
	
	&lt;p xmlns="http://www.w3.org/1999/xhtml"&gt;
		&lt;span class="previous"&gt;Text before match&lt;/span&gt;
		&lt;a href="passed URL if any" class="hi"&gt;The highlighted term&lt;/a&gt;
		&lt;span class="following"&gt;Text after match&lt;/span&gt;
	&lt;/p&gt;
	
	If format=table is passed with the config element, an XHTML tr> element will be returned, with &lt;td>
	elements identifying the three chunks with the same class names.

	@param $hit an arbitrary XML element which has been selected by one of the full text
		operations or an ngram search.
	@param $config configuration element to configure the behaviour of the function
	@param $callback (optional) reference to a callback function which will be called
	once for every text node before it is appended to the displayed text. The function
	should accept 2 parameters: 1) a single text node, 2) a string indicating the
	current direction in which text is appended, i.e. "before" or "after". The function
	may return the empty sequence if the current node should be ignore (e.g. if it belongs
	to a "footnote" which should not be displayed). Otherwise it should return a single
	string.
:)
declare function kwic:summarize($hit as element(), $config as element(config), 
    $callback as function?) as element()* {
    let $expanded := util:expand($hit, "expand-xincludes=no")
	for $match in $expanded//exist:match
	return
		kwic:get-summary((), $match, $config, $callback)
};