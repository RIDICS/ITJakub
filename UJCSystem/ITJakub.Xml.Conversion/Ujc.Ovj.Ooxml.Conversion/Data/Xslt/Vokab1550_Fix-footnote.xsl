<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
    xmlns="http://www.tei-c.org/ns/1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei" version="2.0">
    
    <xsl:include href="Kopirovani_prvku.xsl"/>
    
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>

    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
    <xsl:preserve-space elements="footnote_reference delimitator_jazyka text annotation_reference"/>
    
    <xsl:key name="poznamka-pod-carou" match="poznamka_pod_carou" use="@id"/>

    <xsl:template match="/">
        <xsl:text xml:space="preserve">
        </xsl:text>
        <xsl:comment> Vokab1550_Fix-footnote </xsl:comment>
        <xsl:text xml:space="preserve">
        </xsl:text>
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="*/footnote_reference[not(parent::footnote_text)]">
        <xsl:apply-templates select="key('poznamka-pod-carou', normalize-space(.))" mode="make-inline" />
    </xsl:template>
    
    <xsl:template match="poznamka_pod_carou" mode="make-inline">
        <poznamka_pod_carou n="{@id}" id="{@id}" xmlns="">
            <xsl:apply-templates/>
        </poznamka_pod_carou>
    </xsl:template>
    
    <xsl:template match="poznamka_pod_carou" />
    
    <xsl:template match="footnote_text/footnote_reference">
        <footnote_reference xmlns="" />    
    </xsl:template>
    
    <xsl:template match="annotation_reference" />
</xsl:stylesheet>
