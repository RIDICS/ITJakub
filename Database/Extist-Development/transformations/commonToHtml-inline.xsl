<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd tei nlp exist" version="2.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p>
                <xd:b>Created on:</xd:b> Jun 18, 2015</xd:p>
            <xd:p>
                <xd:b>Author:</xd:b>Boris Lehečka</xd:p>
            <xd:p>Obecně použitelné inline (řádkové) prvky transformace z formátu TEI na HTML.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:output method="html" indent="yes" encoding="UTF-8"/>
    <xsl:variable name="skoba" select="']'"/>
    <xsl:variable name="skobaSMezerou" select="concat($skoba, ' ')"/>
    <xsl:variable name="zacatekRelace" select="'‹'"/>
    <xsl:variable name="konecRelace" select="'›'"/>
    <xd:doc>
        <xd:desc>Šablona pro interpunkci.</xd:desc>
    </xd:doc>
    <xsl:template match="tei:pc">
        <span data-nlp-type="{name()}">
            <xsl:apply-templates/>
        </span>
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
        <xsl:element name="span">
            <xsl:attribute name="class">
                <xsl:value-of select="@rend"/>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro kapitálku (obvykle první písmeno v knize, kapitole nebo odstavci).</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:c">
        <span class="initial">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:note[@n]">
        <xsl:element name="span">
            <xsl:attribute name="class">
                <xsl:text>note-ref</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="data-note-id">
                <xsl:value-of select="@xml:id"/>
            </xsl:attribute>
            <xsl:value-of select="@n"/>
        </xsl:element>
    </xsl:template>
    <xsl:template match="tei:corr" mode="notes">
        <span class="corr">
            <xsl:apply-templates/>
        </span>
        <xsl:choose>
            <xsl:when test="following-sibling::*[1][self::tei:sic]">
                <xsl:value-of select="$skobaSMezerou"/>
            </xsl:when>
            <xsl:otherwise> </xsl:otherwise>
        </xsl:choose>
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
        <xsl:element name="span">
            <xsl:attribute name="class">
                <xsl:text>info</xsl:text>
                <xsl:text> </xsl:text>
                <xsl:text>itj-pb</xsl:text>
                <xsl:if test="@rend='space'">
                    <xsl:text> space</xsl:text>
                </xsl:if>
                <xsl:if test="@ed">
                    <xsl:text> </xsl:text>
                    <xsl:text>additional</xsl:text>
                </xsl:if>
            </xsl:attribute>
            <xsl:attribute name="data-title">
                <xsl:choose>
                    <xsl:when test="@ed = 'edition'">číslo strany edice</xsl:when>
                    <xsl:when test="@ed = 'print'">číslo strany tisku</xsl:when>
                    <xsl:otherwise>číslo strany rukopisu</xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
            <xsl:attribute name="data-page-name">
                <xsl:value-of select="@n"/>
            </xsl:attribute>
        </xsl:element>
    </xsl:template>
    <xsl:template match="tei:*[@rend]" priority="-1">
        <span class="{@rend}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:*[@rend='hidden']" priority="10">
        <span class="{name()}  {@rend}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:*[@type='hidden']" priority="10">
        <span class="{name()}  {@type}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
</xsl:stylesheet>