<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xs xd"
	version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl" />
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Aug 19, 2016</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Přejmenuje element <xd:b>folio</xd:b> na <xd:b>pb</xd:b>, aby bylo možné přesunout elementy <xd:b>unclear</xd:b>.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:template match="/">
		<xsl:comment> TB_Prejmenovat_folio </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="folio">
		<xsl:element name="pb">
			<xsl:apply-templates select="@*" />
		</xsl:element>
	</xsl:template>
	
</xsl:stylesheet>