<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd"
	version="2.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 5, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
	<xsl:output indent="yes" />
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> TEI_Seskupit_odstavce_bez_div_2.0_x </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="body/div">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:choose>
				<xsl:when test="p">
					<xsl:for-each-group select="*" group-adjacent="if(self::p) then 0 else position()">
						<xsl:apply-templates select="." />
					</xsl:for-each-group>
				</xsl:when>
				<xsl:when test="l">
					<xsl:for-each-group select="*" group-adjacent="if(self::l) then 0 else position()">
						<xsl:apply-templates select="." />
					</xsl:for-each-group>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:copy>
		
	</xsl:template>
	
	<xsl:template match="body/div/p | body/div/l">
		<div>
			<xsl:apply-templates select="current-group()" mode="group" /> 
		</div>
	</xsl:template>

	<xsl:template match="l | p" mode="group">
		<xsl:element name="{name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:apply-templates select="*" />
		</xsl:element>
	</xsl:template>

	
<!--	<xsl:template match="*" mode="group">
		<xsl:copy-of select="current-group()" />
	</xsl:template>
-->	
	
	<xsl:template match="@*" priority="2">
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
	
	<xsl:template match="@xml:id" priority="3">
		<xsl:if test="name() = 'xml:id'">
			<xsl:attribute name="xml:id">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>
	
</xsl:stylesheet>