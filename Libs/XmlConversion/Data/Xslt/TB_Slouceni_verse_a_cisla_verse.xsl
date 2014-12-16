<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 8, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>

<xsl:include href="Kopirovani_prvku.xsl"/>


	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/doc/vers[child::*[1] = cislo_verse]">
		<xsl:element name="vers">
			<xsl:attribute name="cislo"><xsl:value-of select="normalize-space(cislo_verse/@n)"/></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="/doc/podnadpis[child::*[1] = cislo_verse]">
		<xsl:element name="nadpis">
		<xsl:element name="vers">
			<xsl:attribute name="cislo"><xsl:value-of select="normalize-space(cislo_verse/@n)"/></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
		</xsl:element>
	</xsl:template>
	

	<xsl:template match="cislo_verse" />
	
</xsl:stylesheet>