<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 1, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Přiřadí značce pro vers <xd:b>l</xd:b> atribut obsahující číslo verše.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>

	
	<xsl:include href="Kopirovani_prvku.xsl"/>

	<xsl:template match="/">
		<xsl:comment> Odstranit_mezery_mezi_elementy </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	
</xsl:stylesheet>