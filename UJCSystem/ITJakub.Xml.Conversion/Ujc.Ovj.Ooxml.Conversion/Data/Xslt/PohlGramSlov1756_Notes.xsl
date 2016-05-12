<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xs xd"
    version="1.0">
    <xsl:import href="Kopirovani_prvku.xsl"/>
    
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Dec 7, 2015</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p>Cleans notes: removes over-generated notes, moves notes to the text content.</xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:template match="/">
        <xsl:comment> PohlSlov1756_Notes </xsl:comment>
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="orig[following-sibling::*[1]/self::note]">
        <xsl:copy>
            <xsl:copy-of select="@*"/>
            <xsl:apply-templates />
            <xsl:copy-of select="following-sibling::note"/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="quote[following::*[1]/self::note]">
        <xsl:copy>
            <xsl:copy-of select="@*"/>
            <xsl:apply-templates />
            <xsl:copy-of select="following::*[1]/self::note"/>
        </xsl:copy>
    </xsl:template>
    

    <xsl:template match="note[not(preceding-sibling::*[1]/self::pb)]" />
        
    
    
    
<!--    <xsl:template match="note[preceding-sibling::*[1]/self::form]" />-->
    
</xsl:stylesheet>