<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="COMMON_Substring-after-last.xsl"/>
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
	
	<xsl:template match="/">
		<xsl:comment> Sloucit_corr_a_sic </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="corr">
		<xsl:element name="note">
			<xsl:element name="choice">
				<xsl:copy-of select="."/> <!-- kopie corr -->
				<xsl:if test="name(following-sibling::*[1]) != 'sic'">
					<xsl:message>
						<xsl:text>Za prvkem &lt;corr&gt; měl následovat prvek &lt;corr&gt;, ale místo toho následuje prvek &lt;</xsl:text><xsl:value-of select="name(following-sibling::*[1])"/><xsl:text>&gt;</xsl:text>
					</xsl:message>
					<chyba>
						<prvek><xsl:value-of select="name(.)"/></prvek>
						<popis>Měl by následovat prvek &lt;corr&gt;, místo toho následuje: <xsl:value-of select="name(following-sibling::*[1])"/>&gt;</popis>
					</chyba>
				</xsl:if>
				<xsl:copy-of select="following-sibling::*[1]" /> <!-- kopie sic -->
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="sic">
		<xsl:choose>
			<xsl:when test="name(preceding-sibling::*[1]) != 'corr'">
				<xsl:variable name="text">
					<xsl:value-of select="normalize-space(preceding::text()[1])"/>
				</xsl:variable>
				
				
					<xsl:element name="note">
					<xsl:element name="choice">
						<xsl:element name="corr">
							<xsl:choose>
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
						<xsl:copy-of select="." />
					</xsl:element>
					</xsl:element>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	
</xsl:stylesheet>