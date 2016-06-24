<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xs xd"
    version="1.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> May 10, 2016</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p></xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:output omit-xml-declaration="yes" indent="yes"/>
    <xsl:strip-space elements="*"/>
    
    <xsl:template match="/">
        <xsl:comment> Vokab1704_Prepare </xsl:comment>
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="body" priority="10">
        <xsl:element name="body" namespace="http://www.tei-c.org/ns/1.0">
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="pb[@n]" priority="10">
        <xsl:element name="{local-name()}" namespace="http://www.tei-c.org/ns/1.0">
            <xsl:copy-of select="@*" />
            
            <xsl:if test="number(@n)">
                <xsl:attribute name="ed">
                    <xsl:text>print</xsl:text>
                </xsl:attribute>
            </xsl:if>
            
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="*" priority="5">
        <xsl:element name="{local-name()}" namespace="http://www.tei-c.org/ns/1.0">
            <xsl:apply-templates select="@*" />
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
</xsl:stylesheet>