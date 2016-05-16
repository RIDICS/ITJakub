<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xs xd"
    version="2.0">
    <xsl:import href="Kopirovani_prvku.xsl"/>
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Dec 19, 2015</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p></xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:output indent="yes" />
    
    <xsl:template match="/">
        <xsl:text xml:space="preserve">
</xsl:text>
        <xsl:comment> Seskupit_obsah_shodnych_prvku </xsl:comment>
        <xsl:text xml:space="preserve">
</xsl:text>
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="tei:choice">
        <xsl:copy>
            <xsl:for-each-group select="*"
                group-adjacent="if (self::tei:orig) then 0 else position()">
                <xsl:apply-templates select="."/>
            </xsl:for-each-group>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="tei:choice/tei:orig">
        <xsl:copy>
            <xsl:copy-of select="@*" />
            <xsl:apply-templates select="current-group()" mode="grouping" />
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="tei:orig" mode="grouping">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="tei:entryFree">
        <xsl:copy>
            <xsl:copy-of select="@*" />
            <xsl:for-each-group select="*" group-adjacent="if (self::tei:cit) then 0 else position()">
                <xsl:apply-templates select="."/>
            </xsl:for-each-group>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="tei:entryFree/tei:cit">
        <xsl:copy>
            <xsl:copy-of select="@*" />
            <xsl:apply-templates select="current-group()" mode="grouping" />
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="tei:cit" mode="grouping">
        <xsl:apply-templates />
    </xsl:template>
    
</xsl:stylesheet>