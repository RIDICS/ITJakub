<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 20, 2014</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Seskupí prvky mezi dvěma elementy <xd:b>div</xd:b> do elementu <xd:b>div</xd:b>.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:output indent="yes" />
	
	<xsl:strip-space elements="*" />
	
	<xsl:include href="Kopirovani_prvku.xsl" />
	
	<xd:doc scope="component">
		<xd:desc>Výchozí prvek šablony</xd:desc>
	</xd:doc>
	<xsl:template match="/">
		<xsl:comment> Seskupit_prvky_mezi_div_do_div </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	<xd:doc>
		<xd:desc>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="div[div][not(div)]">
		
	</xsl:template>

</xsl:stylesheet>