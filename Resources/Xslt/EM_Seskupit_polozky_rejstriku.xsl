<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
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
	
	<xsl:template match="/">
		<xsl:comment> EM_Seskupit_polozky_rejstriku </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="item">
		<xsl:variable name="name" select="local-name()"/>
		
		<!-- Is this the first element in a sequence? -->
		<xsl:if test="local-name(preceding-sibling::*[position()=1]) != $name">
			<xsl:element name="list">
				<xsl:attribute name="type"><xsl:text>index</xsl:text></xsl:attribute>
				<!--<xsl:copy>-->
					<xsl:element name="{$name}">
						<xsl:apply-templates />
					</xsl:element>
					<!-- Match the next sibling if it has the same name -->
					<xsl:apply-templates select="following-sibling::*[1][local-name()=$name]" mode="next"/>
				<!--</xsl:copy>-->
			</xsl:element>
		</xsl:if>
	</xsl:template>
	
	<!-- Recursive template used to match the next sibling if it has the same name -->
	<xsl:template match="item" mode="next">
		<xsl:variable name="name" select="local-name()"/>
		<xsl:element name="{$name}">
			<xsl:apply-templates />
			</xsl:element>
		<xsl:apply-templates select="following-sibling::*[1][local-name()=$name]" mode="next"/>
	</xsl:template>
	
	<!--
	<xsl:template match="item">
		<xsl:if test="not(name(parent::*) = 'list')">
		<xsl:element name="list">
			<xsl:attribute name="type"><xsl:text>index</xsl:text></xsl:attribute>
			<xsl:apply-templates select="."/>
			<xsl:apply-templates select="following-sibling::*[1]" mode="seskupit" />
		</xsl:element>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="item" mode="seskupit">
		<xsl:apply-templates select="."/>
		<xsl:if test="not(following-sibling::*[1]/self::item)">
			<xsl:apply-templates select="following-sibling::*[1]" mode="seskupit"/>
		</xsl:if>
		</xsl:template>-->
	
</xsl:stylesheet>