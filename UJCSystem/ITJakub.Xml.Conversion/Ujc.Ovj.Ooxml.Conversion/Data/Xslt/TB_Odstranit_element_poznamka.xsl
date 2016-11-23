<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd" version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl"/>
	<xd:doc scope="stylesheet">

		
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris Lehečka</xd:p>
			<xd:p>Odstraní element <xd:b>poznamka</xd:b>; veřjné poznámky se do textové banky nedostanou.</xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:template match="/">
		<xsl:comment> TB_Odstranit_element_poznamka </xsl:comment>
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:template match="poznamka" />
	
</xsl:stylesheet>