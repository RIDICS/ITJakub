<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd" version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 5, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Pokud prvek <xd:b>body</xd:b> obsahuje více prvků <xd:b>div</xd:b>, vytvořínadřazený element pro hlavní text.</xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:include href="Kopirovani_prvku.xsl"/>
	<xsl:output indent="yes"/>
	<xsl:strip-space elements="*"/>

	<xsl:template match="/">
		<xsl:comment> EB_Subsumovat_vice_divu </xsl:comment>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="body">
		<xsl:copy>
			<xsl:apply-templates select="//div[@type='editorial']" mode="samostatne" />
			<xsl:choose>
				<xsl:when test="count(div[not(@type='editorial')]) &gt; 2">
					<xsl:element name="div">
						<xsl:apply-templates />
					</xsl:element>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:copy>

	</xsl:template>

	<xsl:template match="div[@type='editorial']" />
	
	<xsl:template match="div[@type='editorial']" mode="samostatne">
		<xsl:copy>
			<xsl:apply-templates select="@*" />
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="@*" priority="2">
		<xsl:variable name="nazev">
			<xsl:choose>
				<xsl:when test="local-name() = 'id'">
					<xsl:text>xml:id</xsl:text>
				</xsl:when>
				<xsl:when test="local-name() = 'lang'">
					<xsl:text>xml:lang</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="local-name()"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:attribute name="{$nazev}">
			<xsl:value-of select="."/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="@xml:id" priority="3">
		<xsl:if test="name() = 'xml:id'">
			<xsl:attribute name="xml:id">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>
