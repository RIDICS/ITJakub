<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:variable name="lomitko" select="'/'"/>
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
			<xsl:when test="contains($cislo, $lomitko)">
				<xsl:call-template name="zpracujFoliaci">
					<xsl:with-param name="cislo" select="substring-before($cislo, $lomitko)" />
					<xsl:with-param name="konciMezerou" select="$konciMezerou" />
				</xsl:call-template>
				<xsl:call-template name="zpracujFoliaci">
					<xsl:with-param name="cislo" select="substring-after($cislo, $lomitko)" />
					<xsl:with-param name="konciMezerou" select="$konciMezerou" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$cislo = 'b' or $cislo = 'a'">
				<xsl:call-template name="vlozCislo">
					<xsl:with-param name="cislo" select="$cislo" />
					<xsl:with-param name="prvek" select="'cb'" />
					<xsl:with-param name="mezera" select="$konciMezerou" />
				</xsl:call-template>
			</xsl:when>
			<!-- DODĚLAT i ostatní případy, kdy může obsahovat 'st.' a 'ed.' -->
			<xsl:when test="not(contains($cislo, 'ed.')) and (contains($cislo, ' bis ') or contains($cislo, ' ter ')) ">
				<xsl:call-template name="vlozCislo">
					<!-- folio -->
					<xsl:with-param name="prvek" select="'pb'"/>
					<xsl:with-param name="cislo" select="$cislo"/>
					<xsl:with-param name="mezera" select="$konciMezerou" />
				</xsl:call-template>
			</xsl:when>
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
							<xsl:with-param name="prvek" select="'pb'"/>
							<xsl:with-param name="cislo" select="substring-before($cislo, 'ed.')"/>
							<xsl:with-param name="typ" select="'edition'"/>
							<xsl:with-param name="mezera" select="$konciMezerou" />
						</xsl:call-template>
					</xsl:when>
					<xsl:when test="contains($cislo, 'st.')">
						<xsl:call-template name="vlozCislo">
							<xsl:with-param name="prvek" select="'pb'"/>
							<xsl:with-param name="cislo" select="substring-before($cislo, 'st.')"/>
							<xsl:with-param name="typ" select="'print'"/>
							<xsl:with-param name="mezera" select="$konciMezerou" />
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="vlozCislo">
							<!-- folio -->
							<xsl:with-param name="prvek" select="'pb'"/>
							<xsl:with-param name="cislo" select="$cislo"/>
							<xsl:with-param name="mezera" select="$konciMezerou" />
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
				
			</xsl:otherwise>
			
		</xsl:choose>
		
	</xsl:template>
	<xsl:template name="vlozCislo">
		<xsl:param name="prvek" />
		<xsl:param name="cislo"/>
		<xsl:param name="typ"/>
		<xsl:param name="mezera"/>
		<xsl:element name="{$prvek}">
			<xsl:attribute name="n">
				<xsl:value-of select="$cislo"/>
			</xsl:attribute>
			<xsl:if test="$typ">
				<xsl:attribute name="ed">
					<xsl:value-of select="$typ"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="$mezera">
				<xsl:attribute name="rend">
					<xsl:text>space</xsl:text>
				</xsl:attribute>
				</xsl:if>
		</xsl:element>
		<xsl:if test="$mezera">
			<xsl:text> </xsl:text>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>