<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	
	<xsl:template match="body/*/*[2][preceding-sibling::*[1]/self::rekonstrukce][not(self::doplneny_text)]">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:copy-of select="preceding-sibling::*[1]"/>
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>
	
	<xd:doc>
		<xd:desc>
			<xd:p>Vybere pouze prvky třetí úrovně (znakové styly) v rámci odstavce, z nimiž následuje element, který by měl být sloučen.</xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xsl:template match="body/*/*[following-sibling::*[1]/self::rekonstrukce][not(self::doplneny_text)][not(self::foliace)][not(self::cislo_verse)]">
		<!--<xsl:template match="body/*/popisek_k_obrazku[following-sibling::*[1]/self::rekonstrukce]">-->
		<xsl:copy><xsl:copy-of select="@*"/><xsl:apply-templates />
			<xsl:choose>
				<xsl:when test="following-sibling::*[2]/self::poznamka">
					<xsl:element name="rekonstrukce">
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
	
	
<!--	<xsl:template match="body/*/rekonstrukce[preceding-sibling::*[1]/self::popisek_k_obrazku]" priority="10" />-->
	<xsl:template match="body/*/rekonstrukce[not(following-sibling::*[1]/self::rekonstrukce)][not(preceding-sibling::*[1]/self::doplneny_text)][not(preceding-sibling::*[1]/self::poznamka)][not(preceding-sibling::*[1]/self::foliace)][not(preceding-sibling::*[1]/self::cislo_verse)]" priority="10" >
<!--		<xsl:message> vynechaný text: <xsl:value-of select="."/> </xsl:message>-->
	</xsl:template>
</xsl:stylesheet>