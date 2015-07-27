xquery version "3.0";
(:module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";:)
import module namespace paging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "../modules/paging.xqm";
import module namespace coll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";
import module namespace trans = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/transformation" at "../modules/transformation.xqm";
import module namespace search = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/search" at "../modules/searching.xqm";
import module namespace functx = "http://www.functx.com" at "../modules/functx.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
declare namespace exist = "http://exist.sourceforge.net/NS/exist"; 
declare namespace itj = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist";
declare namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0";


declare option exist:serialize "highlight-matches=elements";

let $document-id := request:get-parameter("bookId", "")
let $document-version-id := request:get-parameter("versionId", "")
let $start := request:get-parameter("start", "")
let $end := request:get-parameter("end", "")
let $page-position := request:get-parameter("page", 1)
let $page-xml-id := request:get-parameter("pageXmlId", "")
let $output-format := request:get-parameter("outputFormat", "")
let $xsl-path := request:get-parameter("_xsl", "")

let $query-criteria-param := request:get-parameter("serializedSearchCriteria", $search:default-search-criteria)
let $queries := search:get-queries-from-search-criteria-string($query-criteria-param)


let $collection := coll:get-collection("/db/apps/jacob/data", $document-id, $document-version-id)
(:let $page := $collection/id($page-xml-id)/parent::vw:fragment:)
let $page := $collection[vw:fragment[.//tei:pb[@xml:id = $page-xml-id]]]

let $page-hits := search:get-document-fragments-with-hits($page, $queries)
(: vracený prvek musí být obalen elementem s xmlns:exist, jinak se <exist:match> nevygeneruje do výstupu :)
let $page-hits-result :=
	if ($page-hits) then 
		$page-hits
	else 
		$page

let $page-hits-result :=
		<itj:result
			 xmlns="http://www.tei-c.org/ns/1.0"
			 xmlns:vw="http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0"
			xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist"
			xmlns:exist="http://exist.sourceforge.net/NS/exist">
				{$page-hits-result}
		</itj:result>

let $template := doc(escape-html-uri($xsl-path)) 
let $transformation := 
	if($output-format = "Xml") then
			$page-hits-result
	else if($output-format = "Html") 
	then transform:stream-transform($page-hits-result, $template, ())
	else if($output-format = "Rtf") 
		then trans:transform-document-to-rtf($page-hits-result, $template)
		else if($output-format = "Pdf") 
		then trans:transform-document-to-pdf($page-hits-result, $template)
		else()

return ($transformation)


(:return trans:transform-document($page-hits-result, "Xml", $xsl-path) :)
(:return trans:transform-document($page-hits-result, $output-format, $xsl-path) :)
(:let $template := doc(escape-html-uri($xsl-path)) 
return  ($template)
:)
(:return ($page-hits-result, $output-format, $xsl-path):)
