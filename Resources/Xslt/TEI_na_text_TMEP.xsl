<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xs xd"
    version="1.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> May 28, 2014</xd:p>
            <xd:p><xd:b>Author:</xd:b> lehecka</xd:p>
            <xd:p>Převod dokumentu TEI na prostý text (pro JavaScript), pro potřeby testování nového VW v NAKI.</xd:p>
        </xd:desc>
    </xd:doc>
	
	<xsl:strip-space elements="*"/>
    
    <xsl:output method="text" encoding="UTF-8" />
    <xsl:template match="/">
        <xsl:apply-templates select="doc" />
    </xsl:template>
    
    <xsl:template match="doc">
        <xsl:apply-templates />
    </xsl:template>
    
	<xsl:template match="odstavec | tei:item">
         <xsl:apply-templates />
<!--          <xsl:text>\r\n</xsl:text>-->
    <!--	<xsl:text> </xsl:text>-->
		<xsl:text>&#xa;</xsl:text>
    </xsl:template>
    
	<xsl:template match="emendace | pramen | folio | kniha | kapitola | vers | nadpis | incipit" />
	
	<xsl:template match="tei:div[@type='editorial']" />
    
</xsl:stylesheet>