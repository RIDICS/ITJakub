xquery version "3.0";
(:module namespace vw = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection";:)
import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/paging" at "../modules/paging.xqm";
import module namespace vwcoll = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/collection" at "../modules/collection.xqm";
import module namespace vwtrans = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/transformation" at "../modules/transformation.xqm";
import module namespace s = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/searching" at "../modules/searching.xqm";

declare namespace tei = "http://www.tei-c.org/ns/1.0";
declare namespace nlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";

declare namespace a="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching";
declare namespace r="http://schemas.datacontract.org/2004/07/ITJakub.SearchService.Core.Search.DataContract";
declare namespace i="http://www.w3.org/2001/XMLSchema-instance";

let $query-criteria-param := request:get-parameter("serializedSearchCriteria", "")
let $query-criteria := parse-xml($query-criteria-param)

let $books := $query-criteria/r:ResultSearchCriteriaContract/r:ResultBooks/a:BookVersionPairContract

let $results-count := 3
let $results := <Results>
			<PageResultContext>
				<ContextStructure>
					<After>, ktery nemel rad kocky...</After>
					<Before>...zacalo to pred malym </Before>
					<Match>psem</Match>
				</ContextStructure>
				<PageName>2r</PageName>
				<PageXmlId>div1.pb2</PageXmlId>
			</PageResultContext>
			<PageResultContext>
				<ContextStructure>
					<After>, ktery nikdy nebyl venku...</After>
					<Before>...zacalo to po malem </Before>
					<Match>psu</Match>
				</ContextStructure>
				<PageName>145r</PageName>
				<PageXmlId>div145.pb55</PageXmlId>
			</PageResultContext>
			<PageResultContext>
				<ContextStructure>
					<After>, ktery byl stasten...</After>
					<Before>...skoncilo to </Before>
					<Match>psem</Match>
				</ContextStructure>
				<PageName>210v</PageName>
				<PageXmlId>div5.pb45</PageXmlId>
			</PageResultContext>
		</Results>

let $result := <ArrayOfSearchResultContract xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
	{
		for $book in $books
			return
				<SearchResultContract>
					<BookXmlId>{$book/a:Guid/text()}</BookXmlId>
					{$results}
					<TotalHitCount>{$results-count}</TotalHitCount>
					<VersionXmlId>{$book/a:VersionId/text()}</VersionXmlId>
				</SearchResultContract>
	} </ArrayOfSearchResultContract>
(:let $result := $query-criteria:)
return $result