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
		<xsl:comment> Prevod_starych_wordovskych_stylu_na_nove </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="pripisek_marginalni_soucasny">
		<xsl:element name="pripisek_marginalni_soudoby">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>


	<xsl:template match="pripisek_interlinearni_soucasny">
		<xsl:element name="pripisek_interlinearni_soudoby">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

<xsl:template match="Bible_Incipit">
		<xsl:element name="Incipit">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	

	<xsl:template match="Bible_Explicit">
		<xsl:element name="Explicit">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="bible_iniciala">
		<xsl:element name="iniciala">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="Bible_Nadpis_kapitoly">
		<xsl:element name="Nadpis">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	
</xsl:stylesheet>