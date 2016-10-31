<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl" />

	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Feb 11, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Přejmenuje styly pro rekonstrukci a doplněný text, aby bylo možnévyužít transformační šablonu pro EM.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:template match="/">
		<xsl:comment> TB_Prejmenovat_unclear </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="supplied">
		<xsl:element name="doplneny_text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="unclear[supplied]">
		<xsl:element name="rekonstrukce">
			<xsl:copy-of select=".//node()[not(self::supplied)]" />
		</xsl:element>
	</xsl:template>
	
	
	
</xsl:stylesheet>