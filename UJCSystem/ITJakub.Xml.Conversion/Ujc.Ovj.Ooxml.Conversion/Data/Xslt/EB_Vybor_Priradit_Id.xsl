<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	xmlns="http://www.tei-c.org/ns/1.0"
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 5, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
	<xsl:output indent="yes" omit-xml-declaration="no" />
<!--	<xsl:strip-space elements="*"/>-->
	
	<xsl:template match="/">
		<xsl:comment> EB_Vybor_Priradit_Id </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="tei:div | tei:head">
		<xsl:element name="{local-name()}" xmlns="http://www.tei-c.org/ns/1.0">
			<xsl:copy-of select="@*" />
			<xsl:attribute name="xml:id"><xsl:value-of select="concat(generate-id(.), '.', position(), '.', local-name())"/></xsl:attribute>
<!--			<xsl:if test="tei:head">
				<xsl:attribute name="n"><xsl:number/></xsl:attribute>
			</xsl:if>
-->
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="@*" priority="2">
		<xsl:variable name="nazev">
			<xsl:choose>
				<xsl:when test="local-name() = 'id'"><xsl:text>xml:id</xsl:text></xsl:when>
				<xsl:when test="local-name() = 'lang'"><xsl:text>xml:lang</xsl:text></xsl:when>
				<xsl:when test="local-name() = 'space'"><xsl:text>xml:space</xsl:text></xsl:when>
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
	
</xsl:stylesheet>