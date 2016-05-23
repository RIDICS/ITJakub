<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	xmlns="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd" version="1.0">
	<xsl:template match="pagina | foliace | Stranka">
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
			<!--<pb>
				<xsl:attribute name="n">
					<xsl:value-of select="normalize-space(translate(., '[]', ''))"/>
				</xsl:attribute>
				<xsl:if test="string-length(translate(., '[]', '')) &gt; string-length(normalize-space(translate(., '[]', '')))">
					<xsl:attribute name="rend"><xsl:text>space</xsl:text></xsl:attribute>
				</xsl:if>
			</pb>-->
			<xsl:choose>
				<xsl:when test="$column = 'b'">
					<cb n="{$column}">
						<xsl:call-template name="rend-space"/>
					</cb>
				</xsl:when>
				<xsl:otherwise>
					<pb>
						<xsl:attribute name="n">
							<xsl:value-of select="$page"/>
						</xsl:attribute>
						<xsl:call-template name="rend-space"/>
					</pb>
					<xsl:if test="$column != ''">
						<cb n="{$column}">
							<xsl:call-template name="rend-space"/>
						</cb>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
<!--			<pb>
				<xsl:attribute name="n">
					<xsl:value-of select="$page"/>
				</xsl:attribute>
				<xsl:if test="string-length(translate(., '[]', '')) &gt; string-length(normalize-space(translate(., '[]', '')))">
					<xsl:attribute name="rend"><xsl:text>space</xsl:text></xsl:attribute>
				</xsl:if>
			</pb>-->
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