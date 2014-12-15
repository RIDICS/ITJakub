<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 5, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
	<xsl:output indent="yes" />
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> EB_Vybor_Oznacit_bibli </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="body/div[thead/text[text() = 'Bible kladrubská, kniha Genesis']]">
		<xsl:variable name="name" select="local-name()"/>
		<xsl:copy>
			<xsl:attribute name="type"><xsl:value-of select="'bible'"/></xsl:attribute>
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>
	
</xsl:stylesheet>