xquery version "3.0";

import module namespace vwpaging = "http://vokabular.ujc.cas.cz/ns/it-jakub/tests/resources" at "resources.xqm";

let $templateId := "pageToHtml.xsl"
return t:get-resource-as-doc($templateId)


