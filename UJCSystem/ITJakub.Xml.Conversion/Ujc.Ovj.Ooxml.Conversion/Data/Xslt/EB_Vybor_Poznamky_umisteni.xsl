<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 5, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Přidá k poznámkám informaci o umístění</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
	<xsl:output indent="yes" />
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> EB_Vybor_Poznamky_umisteni </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="note">
	<xsl:element name="note">
			<xsl:attribute name="place"><xsl:text>bottom</xsl:text></xsl:attribute>
			<xsl:apply-templates select="@*"></xsl:apply-templates>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="@*" priority="2">
		<xsl:variable name="nazev">
			<xsl:choose>
				<xsl:when test="local-name() = 'id'"><xsl:text>xml:id</xsl:text></xsl:when>
				<xsl:when test="local-name() = 'lang'"><xsl:text>xml:lang</xsl:text></xsl:when>
				<xsl:otherwise><xsl:value-of select="local-name()"/></xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:if test="$nazev != 'xml:id'">
		<xsl:attribute name="{$nazev}">
			<xsl:value-of select="."/>
		</xsl:attribute>
		</xsl:if>
	</xsl:template>	
	
	<xsl:template match="@xml:id" priority="3">
		<xsl:if test="name() = 'xml:id'">
			<xsl:attribute name="xml:id">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>
	<xsl:template match="@xml:space" priority="3">
		<xsl:if test="name() = 'xml:space'">
			<xsl:attribute name="xml:space">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>