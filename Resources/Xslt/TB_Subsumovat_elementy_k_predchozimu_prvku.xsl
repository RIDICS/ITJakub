<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl"/>
	<xsl:import href="Subsumovat_elementy_k_predchozimu_prvku_rekonstrukce.xsl"/>
	
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
		<xsl:comment> TB_Subsumovat_elementy_k_predchozimu_prvku </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	
	<xd:doc>
		<xd:desc>
			<xd:p>Vybere pouze prvky třetí úrovně (znakové styly) v rámci odstavce, z nimiž následuje element, který by měl být sloučen.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="body/*/*[starts-with(name(), 'pripisek') and contains(name(), 'mladsi')][following-sibling::*[1]/self::transliterace]">
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
	
	
	
	<xsl:template match="body/*/transliterace[preceding-sibling::*[1][contains(name(), '_mladsi')]][not(following-sibling::*[1]/self::transliterace)]" priority="10" />
	
	<xsl:template match="body/*/popisek_k_obrazku[following-sibling::*[1]/self::transliterace]">
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
	
	<xsl:template match="body/*/transliterace[preceding-sibling::*[1]/self::popisek_k_obrazku][not(following-sibling::*[1]/self::transliterace)]" priority="10" />
	
	<xsl:template match="body/*/*[contains(name(), '_mladsi')][following-sibling::*[1]/self::emendace and following-sibling::*[2]/self::pramen]">
		<xsl:copy><xsl:copy-of select="@*"/><xsl:apply-templates /><xsl:copy-of select="following-sibling::*[1]"/><xsl:copy-of select="following-sibling::*[2]"/></xsl:copy>
	</xsl:template>
	
	<xsl:template match="body/*/emendace[following-sibling::*[1]/self::pramen][preceding-sibling::*[1][contains(name(), '_mladsi')]]" />
	<xsl:template match="body/*/pramen[preceding-sibling::*[1]/self::emendace][preceding-sibling::*[2][contains(name(), '_mladsi')]]" priority="10" />


	<xsl:template match="body/*/*[not(self::bible_cislo_verse) and not(self::iniciala) and not(self::foliace) and not(self::relator)][following-sibling::*[1]/self::poznamka]">
		<xsl:copy><xsl:copy-of select="@*"/><xsl:apply-templates /><xsl:copy-of select="following-sibling::*[1]"/></xsl:copy>
	</xsl:template>
	
	<xsl:template match="body/*[not(self::bible_cislo_verse)]/poznamka[not(following-sibling::*[1]/self::poznamka)]" priority="10" />
	
	<xsl:template match="poznamka[(preceding-sibling::*[1]/self::bible_cislo_verse or preceding-sibling::*[1]/self::iniciala or preceding-sibling::*[1]/self::foliace or preceding-sibling::*[1]/self::relator) and not(following-sibling::*[1]/self::poznamka)]" priority="15" >
		<xsl:copy-of select="."/>
	</xsl:template>

</xsl:stylesheet>