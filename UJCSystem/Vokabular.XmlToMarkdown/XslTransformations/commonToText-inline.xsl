<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd tei nlp exist" version="2.0">
    <xsl:output method="text" encoding="UTF-8"/>
    <xd:doc>
        <xd:desc>Šablona pro interpunkci.</xd:desc>
    </xd:doc>
    <xsl:template match="tei:pc">
        <xsl:apply-templates/>        
    </xsl:template>
    <xd:doc>
        <xd:desc>Šablona pro jednoznakovou mezeru.</xd:desc>
    </xd:doc>
    <xsl:template match="tei:c[@type='space']">
        <xsl:text> </xsl:text>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro zvýraznění textu.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:hi">        
		<xsl:text>**</xsl:text>
            <xsl:apply-templates/>
        <xsl:text>**</xsl:text>        
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro kapitálku (obvykle první písmeno v knize, kapitole nebo odstavci).</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:c">
        <xsl:apply-templates/>        
    </xsl:template>
	
    <xsl:template match="tei:note[@n]">        
    </xsl:template>
    <xsl:template match="tei:corr" mode="notes">
        <xsl:apply-templates/>             
    </xsl:template>
    <xsl:template match="tei:sic" mode="notes">
        <span class="sic">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro hranice stran originálního textu.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:pb">
        
    </xsl:template>
    <xsl:template match="tei:*[@rend]" priority="-1">        
            <xsl:apply-templates/>        
    </xsl:template>
    <xsl:template match="tei:*[@rend='hidden']" priority="10">        
            <xsl:apply-templates/>
    </xsl:template>
    <xsl:template match="tei:*[@type='hidden']" priority="10">
		<xsl:apply-templates/>
    </xsl:template>
</xsl:stylesheet>