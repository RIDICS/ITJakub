<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd tei nlp exist" version="2.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p>
                <xd:b>Created on:</xd:b> Jun 18, 2015</xd:p>
            <xd:p>
                <xd:b>Author:</xd:b>Boris Lehečka</xd:p>
            <xd:p>Obecně použitelné blokové (odstavcové) prvky transformace z formátu TEI na HTML.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:output method="html" encoding="UTF-8"/>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro původní titul díla, uvedený v prameni.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:title">
        <xsl:element name="h2">
            <xsl:attribute name="style">
                <xsl:text>title</xsl:text>
            </xsl:attribute>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro nadpis.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:head">
        <xsl:element name="h3">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro ediční komentář.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='editorial' and @subtype='comment']">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:text>editorial</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro incipit a explicit.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='incipit' or @type='explicit']">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:value-of select="@type"/>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro biblickou knihu.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='bible' and @subtype='book']">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:text>book</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro biblickou kapitolu</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='bible' and @subtype='chapter']">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:text>chapter</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xsl:template match="tei:p">
        <xsl:element name="p">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro seznam typu rejstřík.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:list[@type='index']">
        <xsl:element name="ul">
            <xsl:attribute name="class">
                <xsl:text>index</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro tabulku</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:table">
        <xsl:element name="table">
            <xsl:element name="tbody">
                <xsl:apply-templates/>
            </xsl:element>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro řádek v tabulce</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:row">
        <xsl:element name="tr">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro buňku v tabulce</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:cell">
        <xsl:element name="td">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xsl:template match="tei:note[@n]" mode="notes">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:text>itj-note</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="data-note-id">
                <xsl:value-of select="@xml:id"/>
            </xsl:attribute>
            <xsl:element name="span">
                <xsl:attribute name="class">
                    <xsl:text>note-ref</xsl:text>
                </xsl:attribute>
                <xsl:value-of select="@n"/>
            </xsl:element>
            <xsl:text> </xsl:text>
            <xsl:apply-templates mode="notes"/>
        </xsl:element>
    </xsl:template>
</xsl:stylesheet>