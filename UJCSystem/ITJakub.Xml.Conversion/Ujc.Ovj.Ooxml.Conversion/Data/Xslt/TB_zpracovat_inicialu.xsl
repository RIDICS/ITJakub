<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl"/>
	
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Feb 11, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> TB_zpracovat_inicialu </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	
	<xsl:template match="iniciala[not(contains(., ' '))]" />
	
	<xsl:template match="*[preceding-sibling::node()[1][self::iniciala][not(contains(., ' '))]]">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:value-of select="preceding-sibling::*[1]"/>
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>
	
</xsl:stylesheet>