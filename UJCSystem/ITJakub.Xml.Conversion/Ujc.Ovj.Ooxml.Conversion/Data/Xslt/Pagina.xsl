<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei" version="1.0">
	<xsl:template match="pagina | foliace">
		<xsl:variable name="number" select="normalize-space(.)"/>
		<xsl:variable name="column">
			<xsl:choose>
				<xsl:when test="substring($number, string-length($number), 1) = 'b'">
					<xsl:value-of select="'b'"/>
				</xsl:when>
				<xsl:when test="substring($number, string-length($number), 1) = 'a'">
					<xsl:value-of select="'a'"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="page">
			<xsl:choose>
				<xsl:when test="$column = ''">
					<xsl:value-of select="normalize-space(translate(., '[]', ''))"/>					
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="substring(normalize-space(translate(., '[]', '')), 1, string-length(normalize-space(translate(., '[]', ''))) -1)"/>
				</xsl:otherwise>
			</xsl:choose>
			
		</xsl:variable>
		<xsl:if test="string-length(normalize-space(.)) &gt; 0">
			<!--<tei:pb>
				<xsl:attribute name="n">
					<xsl:value-of select="normalize-space(translate(., '[]', ''))"/>
				</xsl:attribute>
				<xsl:if test="string-length(translate(., '[]', '')) &gt; string-length(normalize-space(translate(., '[]', '')))">
					<xsl:attribute name="rend"><xsl:text>space</xsl:text></xsl:attribute>
				</xsl:if>
			</tei:pb>-->
			<xsl:choose>
				<xsl:when test="$column = 'b'">
					<tei:cb n="{$column}">
						<xsl:call-template name="rend-space"/>
					</tei:cb>
				</xsl:when>
				<xsl:otherwise>
					<tei:pb>
						<xsl:attribute name="n">
							<xsl:value-of select="$page"/>
						</xsl:attribute>
						<xsl:call-template name="rend-space"/>
					</tei:pb>
					<xsl:if test="$column != ''">
						<tei:cb n="{$column}">
							<xsl:call-template name="rend-space"/>
						</tei:cb>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
<!--			<tei:pb>
				<xsl:attribute name="n">
					<xsl:value-of select="$page"/>
				</xsl:attribute>
				<xsl:if test="string-length(translate(., '[]', '')) &gt; string-length(normalize-space(translate(., '[]', '')))">
					<xsl:attribute name="rend"><xsl:text>space</xsl:text></xsl:attribute>
				</xsl:if>
			</tei:pb>-->
		</xsl:if>
	</xsl:template>
	<xsl:template name="rend-space">
		<xsl:if test="string-length(translate(., '[]', '')) &gt; string-length(normalize-space(translate(., '[]', '')))">
			<xsl:attribute name="rend">
				<xsl:text>space</xsl:text>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>