<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet exclude-result-prefixes="xs xd tei" version="2.0"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    xmlns="http://www.tei-c.org/ns/1.0"
    >
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Jan 8, 2012</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p />
        </xd:desc>
    </xd:doc>
    
    <xsl:output indent="yes" />
    
<!--    <xsl:strip-space elements="*" />-->
    
    <xsl:include href="Kopirovani_prvku.xsl" />
    
    <xd:doc>
        <xd:desc>
            <xd:p>Pokud tělo obsahuje alespoň jeden element <xd:b>head</xd:b> nebo <xd:b>head1</xd:b> (začínající na "head"), vytvoří se pro takto ohraničené oblasti element <xd:b>div</xd:b>.</xd:p>
            <xd:p>Bylo by vhodné elementy, které předcházejí, rovněž seskupit do <xd:b>div</xd:b>, pokud tyto elementy netvoří <xd:b>div</xd:b> nebo <xd:b>pb</xd:b>.</xd:p>
            <xd:p>Pokud tělo žádný element <xd:b>head</xd:b> nebo <xd:b>head1</xd:b> neobsahuje, elementy se zkopírují. (Nebo by se měla vytvořit alespon jedna úroveň <xd:b>div</xd:b>?)</xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:template match="/">
        <xsl:comment> Seskupit_prvky_do_div_2.0_head1 </xsl:comment>
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="tei:body/tei:div">
        <xsl:copy>
            <xsl:copy-of select="@*" />
                
            <xsl:for-each-group select="*" group-starting-with="tei:head1">
                <xsl:apply-templates select="." mode="group" />
            </xsl:for-each-group>
        </xsl:copy>
    </xsl:template>
    
    <!-- <xsl:template match="body/div[@type!='preface']">
        <xsl:copy>
        <xsl:for-each-group select="*" group-starting-with="head1">
                <xsl:apply-templates select="." mode="group" />
            </xsl:for-each-group>
        </xsl:copy>
    </xsl:template>-->
    
    <xsl:template match="tei:head1" mode="group">
        <xsl:element name="div" xmlns="http://www.tei-c.org/ns/1.0">
            <xsl:attribute name="xml:id">
                <xsl:value-of select="concat(parent::tei:div[1]/@xml:id,'.div-', count(preceding-sibling::tei:head1)+1)"/>
            </xsl:attribute>
            
            <xsl:element name="head">
                <xsl:copy-of select="@*" />
                <xsl:apply-templates />
            </xsl:element>
            <xsl:copy-of select="current-group() except ."></xsl:copy-of>
        </xsl:element>
    </xsl:template>
    
<!--    <xsl:template match="head0" mode="group">
        <xsl:apply-templates select="." />
    </xsl:template>-->
    

    
    <xsl:template match="tei:p | tei:entryFree | tei:item | tei:div | tei:titlePage | tei:head | tei:pb | tei:note | tei:ref" mode="group">
        <xsl:copy-of select="current-group()" />
    </xsl:template>
 
</xsl:stylesheet>
