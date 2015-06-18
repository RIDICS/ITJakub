<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:xml="http://www.w3.org/XML/1998/namespace"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    xmlns:b="#default"
    exclude-result-prefixes="xd b tei"
    version="1.0">
	<xsl:include href="TEI_ClassificationDeclarations.xsl"/>
	
	<xsl:template name="InsertEndocingDesc" xmlns="http://www.tei-c.org/ns/1.0">
		<encodingDesc>
			<xsl:call-template name="classificationDeclarations" />
		</encodingDesc>
	</xsl:template>
</xsl:stylesheet>