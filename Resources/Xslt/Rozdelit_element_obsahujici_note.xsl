<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:import href="Kopirovani_prvku.xsl"/>
	
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Feb 6, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Rozdělí element obsahující vnořený prvek <xd:b>note</xd:b> tak, aby byl prvek <xd:b>note</xd:b> pouze na konci nadřazeného elementu (nikoli uprostřed).</xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xsl:template match="/">
		<xsl:comment> Rozdelit_element_obsahujici_note </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xd:doc>
		<xd:desc>
			<xd:p>Elementy, které obsahují note, ale na posledním místě je textový prvek.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="*/*/*[note][node()[last()]/self::text()]">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="node()[not(preceding-sibling::*/self::note)]" />
		</xsl:copy>
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="node()[not(following-sibling::*/self::note) and not(self::note)]" />
		</xsl:copy>
	</xsl:template>
	
</xsl:stylesheet>