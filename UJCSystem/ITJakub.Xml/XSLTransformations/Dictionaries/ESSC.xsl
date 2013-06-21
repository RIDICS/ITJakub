<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
    xmlns:tei="http://www.tei-c.org/ns/1.0" 
    xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" 
    xmlns:exist = "http://exist.sourceforge.net/NS/exist"
    exclude-result-prefixes="xd tei nlp exist"
    version="1.0">
    
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Jun 11, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p>Šablona pro převod ESSČ na formát HTML.</xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:output method="html"/>
    
   
    <!--<xsl:include href="Common/CommonDictionaries.xsl"/>-->
    
    <xsl:template match="tei:entryFree[@type]">
        <div class="{name()} {@type}">
            <xsl:apply-templates />
        </div>
    </xsl:template>
    
    <xsl:template match="tei:hi[@rend]">
        <span class="{@rend}">
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    <xsl:template match="pos">
        <span title="{@norm}"><xsl:apply-templates /></span>
    </xsl:template>
    
    <xsl:template match="tei:abbr[@rend]">
        <span class="{@rend}">
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    <xsl:template match="tei:note">
        <div class="{name()}">
            <xsl:apply-templates />
        </div>
    </xsl:template>
    
    <xsl:template match="tei:sense[tei:sense]">
        <div class="{concat(name(), 's')}">
            <xsl:apply-templates />
        </div>
    </xsl:template>
    
    <xsl:template match="tei:sense">
        <div class="{name()}">
            <xsl:apply-templates />
        </div>
    </xsl:template>
    
    <xsl:template match="tei:term">
        <span class="{name()}">
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    <xsl:template match="tei:def">
        <span class="{name()}">
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    <xsl:template match="tei:xr">
        <span class="{name()}">
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    <xsl:template match="tei:etym">
        <div class="{name()}">
            <xsl:apply-templates />
        </div>
    </xsl:template>
    
    <xsl:template match="tei:form[@type]">
        <span class="{@type}">
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
</xsl:stylesheet>