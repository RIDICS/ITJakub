<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
    xmlns:tei="http://www.tei-c.org/ns/1.0" 
    xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" 
    xmlns:exist = "http://exist.sourceforge.net/NS/exist"
    exclude-result-prefixes="xd tei nlp exist"
    version="1.0">
    
    <xsl:template match="tei:w" priority="5">
            <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="tei:pc" priority="5">
            <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="tei:c[@type='space']" priority="5">
        <xsl:text> </xsl:text>
    </xsl:template>
    
    <xsl:template match="*[@rend]" priority="5">
            <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="*[@rend='hidden']" priority="10">
            <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="*[@type='hidden']" priority="10">
            <xsl:apply-templates />
    </xsl:template>

  <xsl:template match="*/*">
  </xsl:template>
  
</xsl:stylesheet>