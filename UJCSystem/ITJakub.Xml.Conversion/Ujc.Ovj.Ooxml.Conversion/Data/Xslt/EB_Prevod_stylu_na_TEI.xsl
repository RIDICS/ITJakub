<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
  exclude-result-prefixes="xd"
  version="1.0">
  <xd:doc scope="stylesheet">
    <xd:desc>
      <xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
      <xd:p><xd:b>Author:</xd:b> boris</xd:p>
      <xd:p>Převodník stylů specifický pro ediční modul VW</xd:p>
    </xd:desc>
  </xd:doc>
  <!--<xsl:include href="Prevod_stylu_na_TEI.xsl"/>-->
	
	<xsl:param name="exportovatTransliteraci" select="'false()'" />
	
	<xsl:include href="Prevod_stylu_na_TEI.xsl"/>
	<xsl:include href="EM+EB_Prevod_stylu_na_TEI_spolecne.xsl"/>
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
  <xsl:template match="/">
  	<xsl:comment> EB_Prevod_stylu_na_TEI; parameters: exportovatTransliteraci = '<xsl:value-of select="$exportovatTransliteraci"/>'  </xsl:comment>
    <xsl:apply-templates />
  </xsl:template>
  
	<xsl:template match="Anotace">
		<xsl:element name="div">
			<xsl:attribute name="type">
				<xsl:text>editorial</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>annotation</xsl:text>
			</xsl:attribute>
			<xsl:element name="p">
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
  
	<xsl:template match="Komercni_titul">
		<xsl:element name="div">
			<xsl:attribute name="type">
				<xsl:text>editorial</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>title</xsl:text>
			</xsl:attribute>
			<xsl:element name="title">
				<xsl:attribute name="type">
					<xsl:text>commercial</xsl:text>
				</xsl:attribute>
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Titulek_obrazku">
		<xsl:variable name="poradi">
			<xsl:number count="Titulek_obrazku" level="any"/>
		</xsl:variable>
		<div type="editorial" subtype="image">
			<figure type="cover" subtype="A4"><graphic url="Obrazek-{$poradi}_A4.jpg" /></figure>
			<figure type="cover" subtype="A6"><graphic url="Obrazek-{$poradi}_A6.jpg" /></figure>
			<p><xsl:apply-templates /></p>
		</div>
	</xsl:template>
	
	<xsl:template match="Normalni/footnote_reference | Vers//footnote_reference | Titul//footnote_reference | Podnadpis/footnote_reference | Nadpis/footnote_reference">
		<xsl:element name="note">
			<xsl:attribute name="type">
				<xsl:text>footnote</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates select="key('poznamka', .)" mode="note" />
			<!--<xsl:apply-templates select='../../poznamka_pod_carou[@id="{text()}"]' />-->
		</xsl:element>
	</xsl:template>

	
</xsl:stylesheet>