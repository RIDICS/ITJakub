<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/info" exclude-result-prefixes="xs xd itj tei nlp" version="2.0">
    <xsl:import href="commonToHtml-inline.xsl"/>
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
    <xsl:template match="/">
        <xsl:if test="//tei:note">
            <Notes>
                <xsl:apply-templates select="//tei:note"/>
            </Notes>
        </xsl:if>
    </xsl:template>
    <xsl:template match="tei:note">
        <a:string>
            <span class="note-ref">
                <xsl:value-of select="@n"/>
            </span>
            <xsl:apply-templates mode="notes"/>
        </a:string>
    </xsl:template>
</xsl:stylesheet>