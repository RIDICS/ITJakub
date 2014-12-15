<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 21, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Vypíše do výstupního dokumentu chybu v případě, že nebyla definována šablona pro daný prvek.</xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xsl:template match="*">
		<xsl:message>
			<xsl:text>Prvek '</xsl:text>
			<xsl:value-of select="name(.)"/>
			<xsl:text>' nemá definvanou šablonu pro zpracování.</xsl:text>
		</xsl:message>
		<chyba>
<!--			<sablona>
				<xsl:value-of select=""/>
			</sablona>
-->
			<prvek>
				<xsl:value-of select="name(.)"/>
			</prvek>
			<obsah>
				<xsl:value-of select="."/>
			</obsah>
		</chyba>
	</xsl:template>
</xsl:stylesheet>