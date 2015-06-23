xquery version "3.0";

module namespace res = "http://vokabular.ujc.cas.cz/ns/it-jakub/tests/resources";

declare function res:get-resource-as-doc($path as xs:string) {
	let $resource := doc($path)
	return $resource
};