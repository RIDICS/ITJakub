<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd"
	version="1.0">
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
		<xsl:comment> TEI_Seskupit_odstavce_bez_div </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="body/item">
		<xsl:variable name="name" select="local-name()"/>
		
		<!-- Is this the first element in a sequence? -->
		<xsl:if test="local-name(preceding-sibling::*[position()=1]) != $name">
		<!--	<xsl:element name="div">-->
				<xsl:element name="list">
					<xsl:element name="{$name}">
						<xsl:apply-templates />
							</xsl:element>
					
					<!-- Match the next sibling if it has the same name -->
					<xsl:apply-templates select="following-sibling::*[1][local-name()=$name]" mode="next"/>
				<!--</xsl:copy>-->
					</xsl:element>
			<!--</xsl:element>-->
		</xsl:if>
	</xsl:template>
	
	<!-- Recursive template used to match the next sibling if it has the same name -->
	<xsl:template match="body/item" mode="next">
		<xsl:variable name="name" select="local-name()"/>
		<xsl:element name="{$name}">
			<xsl:apply-templates />
			</xsl:element>
		<xsl:apply-templates select="following-sibling::*[1][local-name()=$name]" mode="next"/>
	</xsl:template>
	
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