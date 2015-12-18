<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:c="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xs xd c" version="2.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p>
                <xd:b>Created on:</xd:b> Aug 27, 2015</xd:p>
            <xd:p>
                <xd:b>Author:</xd:b> lehecka</xd:p>
            <xd:p>Transformace interního formátu s korpusovými výskty do podoby kontraktu</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:strip-space elements="*"/>
    <xsl:output method="xml" indent="yes"/>
    <xsl:template match="/">
        <xsl:apply-templates/>
    </xsl:template>
    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>
    <xsl:template match="c:BookXmlId">
        <xsl:copy-of select="parent::c:CorpusSearchResultContract/c:BibleVerseResultContext"/>
        <xsl:copy>
            <xsl:apply-templates/>
        </xsl:copy>
    </xsl:template>
    <xsl:template match="c:HitResultContext">
        <xsl:copy-of select="c:Notes"/>
        <PageResultContext xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results">
            <xsl:copy-of select="@*"/>
            <ContextStructure>
                <xsl:apply-templates select="*"/>
            </ContextStructure>
            <xsl:copy-of select="parent::c:CorpusSearchResultContract/c:PageResultContext/*"/>
        </PageResultContext>
    </xsl:template>
    <xsl:template match="c:PageResultContext"/>
    <xsl:template match="c:Notes"/>
    <xsl:template match="c:BibleVerseResultContext"/>
</xsl:stylesheet>