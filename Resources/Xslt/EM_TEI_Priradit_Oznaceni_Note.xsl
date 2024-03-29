<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 10, 2014</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Přiřadí jenotlivám významonosným prvkům jedinečné ID spočívacící v identifikaci předchozích prvků.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
	
	
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> TEI_Priradit_Oznaceni_Note </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="tei:note[not(parent::tei:fw)][not(parent::tei:del)][not(ancestor::*/self::tei:note)]">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:attribute name="n">
				<xsl:call-template name="calculateFootnoteNumber" />
			</xsl:attribute>
			<xsl:attribute name="xml:id">
				<xsl:text>note-</xsl:text><xsl:number count="tei:note" format="1" from="tei:text" level="any"/>
			</xsl:attribute>
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>
	
	<xsl:template name="calculateFootnoteNumber">
		<xsl:choose>
			<xsl:when test="tei:choice[tei:sic]">
				<xsl:number count="tei:note[tei:choice[tei:sic]]" format="a" from="tei:front | tei:body" level="any"/>
			</xsl:when>
			<!--			<xsl:when test="name(.) = 'add'">
				<xsl:number format="I"/>
			</xsl:when>
-->
			<xsl:otherwise>
				<!-- V edičním modulu se přípisek jako poznámka pod čarou neuvádí -->
				<xsl:number count="tei:note[not(tei:choice[tei:sic])][not(ancestor::*/self::tei:note)] | tei:note[not(tei:choice)][not(ancestor::*/self::tei:note)]" format="1" from="tei:front | tei:body"
					level="any"/>
				<!--<xsl:number count="tei:note[not(tei:choice)] | tei:add" format="1" from="tei:text"
					level="any"/>-->
			</xsl:otherwise>
		</xsl:choose>
		
		
	</xsl:template>
	
</xsl:stylesheet>