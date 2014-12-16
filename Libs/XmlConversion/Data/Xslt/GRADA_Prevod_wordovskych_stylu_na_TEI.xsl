<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Převod starých vordovských stylů na jejich nové ekvivalenty.</xd:p>
			<xd:p>Slouží k snadnému aktualizaci wordovské šablony, popř. zachování starších dokumentů, aniž by bylo nutné měnit následné transformační šablony.</xd:p>
		</xd:desc>
	</xd:doc>
	

	<xsl:include href="Kopirovani_prvku.xsl"/>

	<xsl:template match="/">
		<xsl:comment> GRADA_Prevod_wordovskych_stylu_na_TEI </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="Tiraz">
		<xsl:element name="Normalni">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="Autor">
		<xsl:element name="Normalni">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Obsah">
		<xsl:element name="Normalni">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="Odrazky">
		<xsl:element name="Normalni">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="Popisek_obrazku">
		<xsl:element name="Normalni">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
</xsl:stylesheet>