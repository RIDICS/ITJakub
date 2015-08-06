<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl"/>

	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Feb 6, 2011</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Rozdělí foliaci na jednotlivé části oddělené mezerou; pro každou část vygeneruje (na základě zakončení textu) odpovídající číslo paginy.</xd:p> 
		</xd:desc>
	</xd:doc>
	
	<xsl:output method="xml" indent="yes"/>
	
	
	<xsl:template match="/">
		<xsl:comment> TB_Prejmenovat_Foliaci </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="foliace">
		<xsl:call-template name="zpracujFoliaci">
			<xsl:with-param name="konciMezerou" select="substring(., string-length(.), 1) = ' '" />
			<xsl:with-param name="cislo" select="normalize-space(.)" />
		</xsl:call-template>
		</xsl:template>
		
	
	<xsl:template name="zpracujFoliaci">
		<xsl:param name="konciMezerou" />
		<xsl:param name="cislo" />
		<xsl:choose>
			<xsl:when test="contains($cislo, ' ')">
				<xsl:call-template name="zpracujFoliaci">
					<xsl:with-param name="konciMezerou" select="$konciMezerou" />
					<xsl:with-param name="cislo" select="substring-before($cislo, ' ')" />
				</xsl:call-template>
				<xsl:call-template name="zpracujFoliaci">
					<xsl:with-param name="konciMezerou" select="$konciMezerou" />
					<xsl:with-param name="cislo" select="substring-after($cislo, ' ')" />
				</xsl:call-template>
			</xsl:when>
			
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="contains($cislo, 'ed.')">
						<xsl:call-template name="vlozCislo">
							<xsl:with-param name="prvek" select="'strana_edice'"/>
							<xsl:with-param name="cislo" select="substring-before($cislo, 'ed.')"/>
							<xsl:with-param name="konciMezerou" select="$konciMezerou"/>
						</xsl:call-template>
					</xsl:when>
					<xsl:when test="contains($cislo, 'st.')">
						<xsl:call-template name="vlozCislo">
							<xsl:with-param name="prvek" select="'strana'"/>
							<xsl:with-param name="cislo" select="substring-before($cislo, 'st.')"/>
							<xsl:with-param name="konciMezerou" select="$konciMezerou"/>
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="vlozCislo">
							<xsl:with-param name="prvek" select="'folio'"/>
							<xsl:with-param name="cislo" select="$cislo"/>
							<xsl:with-param name="konciMezerou" select="$konciMezerou"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
				
			</xsl:otherwise>
			
		</xsl:choose>
		
	</xsl:template>
	<xsl:template name="vlozCislo">
		<xsl:param name="prvek" />
		<xsl:param name="cislo"/>
		<xsl:param name="konciMezerou"/>
		<xsl:element name="{$prvek}">
			<xsl:attribute name="cislo">
				<xsl:value-of select="$cislo"/>
			</xsl:attribute>
			<xsl:if test="not($konciMezerou)">
				<xsl:attribute name="presunout">
					<xsl:value-of select="true()"/>
				</xsl:attribute>
			</xsl:if>
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>