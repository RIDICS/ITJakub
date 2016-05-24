<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns="http://www.tei-c.org/ns/1.0"
	xmlns:uuid="http://www.uuid.org"
	exclude-result-prefixes="xs xd tei uuid"
	version="2.0">
	<xd:doc scope="stylesheet">
		
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Sep 17, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> lehecka</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:strip-space elements="*"/>
	<xsl:output method="xml" encoding="UTF-8" indent="yes"/>
	
	<xsl:include href="UUID.xsl"/>
	<xsl:include href="TEI_Common.xsl"/>
	
	<xsl:variable name="version" select="lower-case(uuid:get-uuid())"/>
	<xsl:variable name="guid" select="tei:TEI/tei:teiHeader/tei:fileDesc/@n"/>
	
	<xsl:template match="node()|@*">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="tei:TEI">
		<xsl:copy>
			<xsl:call-template name="generate-guide-attribute" />
			<xsl:attribute name="change" select="concat('#', $version)" />
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>

	<xsl:template name="generate-guide-attribute">
		<xsl:attribute name="n">
			<xsl:choose>
				<xsl:when test="contains($guid, '{')">
					<xsl:value-of select="upper-case($guid)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="concat('{', upper-case($guid), '}')"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
	</xsl:template>
	
	<xsl:template match="tei:fileDesc">
		<xsl:copy>
			<xsl:call-template name="generate-guide-attribute" />
			<xsl:apply-templates />
		</xsl:copy>
		<xsl:call-template name="InsertEndocingDesc"/>
	</xsl:template>
	
	<xsl:template match="tei:teiHeader">
		<xsl:copy>
			<xsl:apply-templates select="@*" />
			<xsl:apply-templates />
			<revisionDesc>
				<change n="{$version}" when="{current-dateTime()}" />
			</revisionDesc>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="tei:sourceDesc[not(@n)]">
		<xsl:copy>
			<listBibl>
				<bibl type="acronym" subtype="source"><xsl:value-of select="/tei:TEI/tei:teiHeader/@n"/></bibl>
			</listBibl>
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="tei:profileDesc">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:apply-templates />
			<textClass xmlns="http://www.tei-c.org/ns/1.0">
				<catRef target="#taxonomy-digitized-grammar #output-digitized-grammar" />
			</textClass>
		</xsl:copy>
	</xsl:template>
	
</xsl:stylesheet>