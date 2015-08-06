<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns="http://www.tei-c.org/ns/1.0"
	xmlns:t="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:param name="obecne" />
	<xsl:param name="konkretni" />
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> EP_Spojit_obecne_a_konkretni_zasady </xsl:comment>
		<TEI xmlns="http://www.tei-c.org/ns/1.0" xml:lang="cs">
			<teiHeader>
			<xsl:apply-templates select="document($konkretni)/t:TEI/t:teiHeader/*" mode="jmennyProstor" />
			</teiHeader>
			<text>
				<xsl:element name="body">
				<xsl:element name="div">
					<xsl:apply-templates select="document($obecne)/t:TEI/t:text/t:body/*" mode="jmennyProstor" />
				</xsl:element>
				<!--<xsl:apply-templates select="document($konkretni)/t:TEI/t:text/t:front/t:div[@type='editorial' and @subtype='comment']" mode="jmennyProstor" />-->
					<xsl:apply-templates select="document($konkretni)//t:div[@type='editorial' and @subtype='comment']" mode="jmennyProstor" />
					<!--<xsl:copy>
						<xsl:copy-of select="@*" />
						<xsl:apply-templates mode="jmennyProstor" />
					</xsl:copy>-->
				</xsl:element>
			</text>
			<!-- Doplněno 18. 11. 2010 -->
			</TEI>
	</xsl:template>
	
	<xsl:template match="*" mode="jmennyProstor">
		<xsl:element name="{local-name()}" namespace="http://www.tei-c.org/ns/1.0">
			<xsl:apply-templates select="@*|*|text()" mode="jmennyProstor"/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="@*" mode="jmennyProstor">
		<xsl:variable name="nazev">
		<xsl:choose>
			<xsl:when test="local-name() = 'id'"><xsl:text>xml:id</xsl:text></xsl:when>
			<xsl:when test="local-name() = 'lang'"><xsl:text>xml:lang</xsl:text></xsl:when>
			<xsl:otherwise><xsl:value-of select="local-name()"/></xsl:otherwise>
		</xsl:choose>
		</xsl:variable>
		<xsl:attribute name="{$nazev}">
			<xsl:value-of select="."/>
		</xsl:attribute>
	</xsl:template>
	
</xsl:stylesheet>