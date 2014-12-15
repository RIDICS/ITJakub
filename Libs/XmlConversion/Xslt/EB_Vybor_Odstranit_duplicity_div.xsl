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
	<xsl:output indent="no" />
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> EB_Vybor_Odstranit_duplicity_div </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="/body/div/div[head1/text[starts-with(text(),  'Kterak Eilenšpig')]]" />
</xsl:stylesheet>