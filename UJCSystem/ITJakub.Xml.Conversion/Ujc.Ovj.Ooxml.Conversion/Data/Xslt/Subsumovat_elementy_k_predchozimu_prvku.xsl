<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl"/>
	<xsl:include href="Subsumovat_elementy_k_predchozimu_prvku_rekonstrukce.xsl"/>
	
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jan 23, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Přesune vybrané elementy do předchozího prvku. Týká se např. emendace, transliterace, veřejné poznámky.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> Subsumovat_elementy_k_predchozimu_prvku </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	
	
	
	<xsl:template match="body/*/*[following-sibling::*[1]/self::transliterace]">
		<xsl:copy><xsl:copy-of select="@*"/><xsl:apply-templates />
			<xsl:choose>
				<xsl:when test="following-sibling::*[2]/self::poznamka">
					<xsl:element name="transliterace">
						<xsl:copy-of select="following-sibling::*[1]/text()"/>
						<xsl:copy-of select="following-sibling::*[2]"/>
					</xsl:element>
				</xsl:when>
				<xsl:otherwise>
					<xsl:copy-of select="following-sibling::*[1]"/>		
				</xsl:otherwise>
			</xsl:choose>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="body/*/transliterace[not(following-sibling::*[1]/self::transliterace)]" priority="10" />
	
	
	<xsl:template match="body/*/*[following-sibling::*[1]/self::emendace and following-sibling::*[2]/self::pramen]">
		<xsl:copy><xsl:copy-of select="@*"/><xsl:apply-templates /><xsl:copy-of select="following-sibling::*[1]"/><xsl:copy-of select="following-sibling::*[2]"/></xsl:copy>
	</xsl:template>

	<xsl:template match="body/*/*[following-sibling::*[1]/self::emendace and following-sibling::*[2]/self::pramen and following-sibling::*[3]/self::poznamka]" priority="7">
		<xsl:copy><xsl:copy-of select="@*"/><xsl:apply-templates /><xsl:copy-of select="following-sibling::*[1]"/><xsl:copy-of select="following-sibling::*[2]"/><xsl:copy-of select="following-sibling::*[3]"/></xsl:copy>
	</xsl:template>
	

	<xsl:template match="body/*/emendace[following-sibling::*[1]/self::pramen]" >
<!--		<xsl:message>x</xsl:message>-->
	</xsl:template>
	<xsl:template match="body/*/pramen[preceding-sibling::*[1]/self::emendace]" priority="10" >
<!--		<xsl:message>x</xsl:message>-->
	</xsl:template>


	<xsl:template match="body/*/*[not(self::bible_cislo_verse) and not(self::iniciala) and not(self::foliace) and not(self::relator) and not(self::pramen)][following-sibling::*[1]/self::poznamka]">
		<xsl:copy><xsl:copy-of select="@*"/><xsl:apply-templates /><xsl:copy-of select="following-sibling::*[1]"/></xsl:copy>
	</xsl:template>
	
	<xsl:template match="body/*/poznamka[preceding-sibling::*[1]/self::pramen][not(preceding-sibling::*[2]/self::emendace)]" priority="15">
		<xsl:copy-of select="."/>
	</xsl:template>
	
	<xsl:template match="body/*[not(self::bible_cislo_verse)]/poznamka[not(following-sibling::*[1]/self::poznamka)]" priority="10" >
		<!--<xsl:message>x</xsl:message>-->
	</xsl:template>
	
<!--	<xsl:template match="bible_cislo_verse[following-sibling::*[1]/self::poznamka[not(following-sibling::*[1]/self::poznamka)]]" >
		<xsl:copy-of select="."/>
	</xsl:template>
-->	

	<xsl:template match="poznamka[(preceding-sibling::*[1]/self::bible_cislo_verse or preceding-sibling::*[1]/self::iniciala or preceding-sibling::*[1]/self::foliace or preceding-sibling::*[1]/self::relator or preceding-sibling::*[1]/self::prmaen) and not(following-sibling::*[1]/self::poznamka)]" priority="15" >
		<xsl:copy-of select="."/>
	</xsl:template>
	

</xsl:stylesheet>