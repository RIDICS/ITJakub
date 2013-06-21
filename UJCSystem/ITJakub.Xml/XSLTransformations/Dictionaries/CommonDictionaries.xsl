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
            <xd:p><xd:b>Created on:</xd:b> Jun 12, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p>Společné prvky slovníků.</xd:p>
        </xd:desc>
    </xd:doc>
    
    
    <xsl:output method="html"/>
    
    <xsl:include href="CommonExist.xsl"/>
    
    <xsl:template match="tei:entryFree">
        <div class="entryFree">
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    
    <xsl:template match="tei:orth[not(@xml:lang) and not(@type)]">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="tei:orth">
        
        <span>
            <xsl:choose>
                <xsl:when test="@xml:lang">
                    <xsl:attribute name="lang">
                        <xsl:value-of select="@xml:lang"/>
                    </xsl:attribute>
                </xsl:when>
                <xsl:when test="@type">
                    <xsl:attribute name="data-orth-type">
                        <xsl:value-of select="@type"/>
                    </xsl:attribute>
                </xsl:when>
            </xsl:choose>
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    
    <xsl:template match="tei:w">
        <span data-nlp-type="{name()}" data-nlp-lemma="{@nlp:lemma}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    
    <xsl:template match="tei:pc">
        <span data-nlp-type="{name()}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    
    <xsl:template match="tei:c[@type='space']">
        <xsl:text> </xsl:text>
    </xsl:template>
    
    <xsl:template match="tei:num">
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="tei:cit">
        <span lang="{@xml:lang}" data-orth-type="{@type}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    
    <xsl:template match="tei:pb">
        <span class="pb">
            <xsl:text>[</xsl:text><xsl:value-of select="@n" /><xsl:text>]</xsl:text>
        </span>
    </xsl:template>
    
    <xsl:template match="*[@rend]" priority="-1">
        <span class="{@rend}">
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    <xsl:template match="*[@rend='hidden']" priority="10">
        <span class="{name()}  {@rend}">
            <xsl:apply-templates />
        </span>
    </xsl:template>

    <xsl:template match="*[@type='hidden']" priority="10">
        <span class="{name()}  {@type}">
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    
    
</xsl:stylesheet>