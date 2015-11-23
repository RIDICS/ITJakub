<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info" exclude-result-prefixes="xs xd itj tei nlp" version="2.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p>
                <xd:b>Created on:</xd:b> Oct 26, 2015</xd:p>
            <xd:p>
                <xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>
    <xsl:preserve-space elements="c"/>
    <xsl:strip-space elements="*"/>
    <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
    <xsl:template match="itj:before">
        <Before>
            <xsl:apply-templates/>
        </Before>
    </xsl:template>
    <xsl:template match="itj:after">
        <After>
            <xsl:apply-templates/>
        </After>
    </xsl:template>
    <xsl:template match="itj:match">
        <Match>
            <xsl:apply-templates/>
        </Match>
    </xsl:template>
    <xsl:template match="tei:w | tei:pc">
        <xsl:apply-templates/>
    </xsl:template>
    <xsl:template match="tei:c">
        <xsl:text> </xsl:text>
    </xsl:template>
    <xsl:template match="tei:note">
        <span class="note-ref">
            <xsl:value-of select="@n"/>
        </span>
    </xsl:template>
    <xsl:template match="itj:element">
        <span class="itj-structure-{@type}">
            <xsl:value-of select="@name"/>
        </span>
    </xsl:template>
    <xsl:template match="tei:pb">
        <span class="itj-structure-pb">
            <xsl:value-of select="@n"/>
        </span>
    </xsl:template>
    <xsl:template match="tei:foreign">
        <span class="itj-foreign">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
</xsl:stylesheet>