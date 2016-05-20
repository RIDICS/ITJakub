<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    xmlns="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="tei" version="2.0">
            
    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
    <xsl:preserve-space elements="text"/>
    <xsl:variable name="vychozi-jazyk" select="'cs'"/>
    
    <xsl:include href="Kopirovani_prvku.xsl"/>
    
    <!--
    <xsl:template match="@*|*|text()">
        <xsl:copy>
            <xsl:apply-templates select="@*|*|text()"/>
        </xsl:copy>
    </xsl:template>-->
    
    <xsl:template match="/">
        <xsl:text xml:space="preserve">
        </xsl:text>
        <xsl:comment> PohlGramSlov_compute_missing_pb-id.xsl </xsl:comment>
        <xsl:text xml:space="preserve">
        </xsl:text>
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="tei:pb">
        <xsl:copy>
            <xsl:apply-templates select="@*"/>
            <xsl:attribute name="xml:id">
                <xsl:value-of select="concat('pb', @n)"/>
            </xsl:attribute> 
            <xsl:apply-templates select="node()"/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>