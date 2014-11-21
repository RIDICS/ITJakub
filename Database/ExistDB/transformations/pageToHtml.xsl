<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd tei nlp exist" version="1.0">
    <xsl:template match="tei:w">
        <span data-nlp-type="{name()}" data-nlp-lemma="{@nlp:lemma}">
<!--            <xsl:if test="@nlp:lemma = $searchedLemma">
                <xsl:attribute name="class">
                    <xsl:text>match</xsl:text>
                </xsl:attribute>
            </xsl:if>-->
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="result">
        <div>
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    <xsl:template match="tei:pc">
        <span data-nlp-type="{name()}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:c[@type='space']">
        <xsl:text> </xsl:text>
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
                <xsl:text>pb</xsl:text>
                    <!-- pokud má býz za foliací mezera, bude se pomocí stylu zobrazovat v textu, jinak bude foliace bez mezer -->
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
    <xsl:template match="*[@rend]" priority="-1">
        <span class="{@rend}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="*[@rend='hidden']" priority="10">
        <span class="{name()}  {@rend}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="*[@type='hidden']" priority="10">
        <span class="{name()}  {@type}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
</xsl:stylesheet>