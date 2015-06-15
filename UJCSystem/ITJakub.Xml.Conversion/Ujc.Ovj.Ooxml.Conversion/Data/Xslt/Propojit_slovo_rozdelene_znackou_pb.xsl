<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jan 14, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xd:doc>
		<xd:desc>
			<xd:p>Text před značkou hranice strany. Poslední výraz tvoří počáteční část slova.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="text()[following-sibling::node()[1]/self::pb[not(@rend)]]">
		
	</xsl:template>
	
	
	<xd:doc>
		<xd:desc>
			<xd:p>Text za značkou hranice strany. První výraz tvoří koncovou část slova.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="text()[preceding-sibling::node()[1]/self::pb[not(@rend)]]">
		
	</xsl:template>
	
</xsl:stylesheet>