<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jan 9, 2011</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Rozepíše kumulativní styly (např. <xd:b>poznamka_kurziva</xd:b>  na více jednochých stylů.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
		
	<xsl:template match="/">
		<xsl:comment> Prevest_sloucene_styly_na_elementy </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="poznamka_kurziva">
			<xsl:element name="poznamka">
				<xsl:element name="kurziva">
					<xsl:value-of select="."/>
				</xsl:element>
			</xsl:element>
		</xsl:template>
		
</xsl:stylesheet>