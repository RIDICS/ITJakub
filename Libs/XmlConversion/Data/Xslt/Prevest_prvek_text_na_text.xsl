<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Převede prvek <xd:b>text</xd:b> na obsah.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
	
	<xsl:template match="/">
		<xsl:comment> Prevest_prvek_text_na_text </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="text">
		<xsl:apply-templates />
	</xsl:template>
	
</xsl:stylesheet>