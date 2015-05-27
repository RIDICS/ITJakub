<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl"/>

	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Feb 6, 2011</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Převede obsah prvku <xd:b>text</xd:b> na obyčejný text.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:strip-space elements="*"/>
	<xsl:output method="xml" indent="no"/>
	
	
	<xsl:template match="/">
		<xsl:comment> TB_Odstranit_Prvek_text </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	
	<xsl:template match="text">
		<xsl:apply-templates />
	</xsl:template>
</xsl:stylesheet>