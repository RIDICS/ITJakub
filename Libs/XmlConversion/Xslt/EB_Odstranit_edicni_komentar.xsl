<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:t="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd t"
	version="1.0">
	
	
	
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 22, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Odstrani ediční komentáře z těla dokumentu.</xd:p>
		</xd:desc>
	</xd:doc>

<xsl:output method="xml" indent="yes"/>
	<!--<xsl:strip-space elements="*"/>-->
	
	<xsl:include href="Kopirovani_prvku.xsl"/>

	<xsl:template match="/">
		<xsl:comment> EB_Odstranit_edicni_komentar </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="div[@type='editorial' and @subtype='comment']" />
	<xsl:template match="pb[@ed]" />
</xsl:stylesheet>