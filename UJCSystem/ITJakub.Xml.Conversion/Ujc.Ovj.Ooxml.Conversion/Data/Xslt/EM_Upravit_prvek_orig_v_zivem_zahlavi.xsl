<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="COMMON_Substring-after-last.xsl"/>
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jan 23, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl" />
	
	<xsl:output method="xml" indent="yes" />
	
	<xsl:template match="/">
		<xsl:comment> EM_Upravit_prvek_orig_v_zivem_zahlavi </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	
	<xsl:template match="note[orig]">
		<xsl:variable name="text">
			<xsl:value-of select="normalize-space(preceding::text()[1])"/>
		</xsl:variable>
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:element name="choice">
				<xsl:element name="reg">
				<xsl:choose>
					<xsl:when test="contains($text, '(')">
						<xsl:variable name="zbytek">
							<xsl:call-template name="substring-after-last">
								<xsl:with-param name="string" select="$text" />
								<xsl:with-param name="delimiter" select="'('" />
							</xsl:call-template>
						</xsl:variable>
						<xsl:choose>
							<xsl:when test="contains($zbytek, ' ')">
								<xsl:call-template name="substring-after-last">
									<xsl:with-param name="string" select="$text" />
									<xsl:with-param name="delimiter" select="' '" />
								</xsl:call-template>
							</xsl:when>
							<xsl:otherwise>
								<xsl:call-template name="substring-after-last">
									<xsl:with-param name="string" select="$text" />
									<xsl:with-param name="delimiter" select="'('" />
								</xsl:call-template>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="contains($text, ' ')">
						<xsl:call-template name="substring-after-last">
							<xsl:with-param name="string" select="$text" />
							<xsl:with-param name="delimiter" select="' '" />
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$text"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
	<xsl:apply-templates />
			</xsl:element>
		</xsl:copy>
	</xsl:template>
	

	
</xsl:stylesheet>