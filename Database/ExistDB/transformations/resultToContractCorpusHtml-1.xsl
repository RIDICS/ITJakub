<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:b="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:c="http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results" xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xs xd c" version="2.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p>
                <xd:b>Created on:</xd:b> Aug 27, 2015</xd:p>
            <xd:p>
                <xd:b>Author:</xd:b> lehecka</xd:p>
            <xd:p>Escapování HTML znaků kvůli deserializaci kontraktu v C#</xd:p>
        </xd:desc>
    </xd:doc>
    
    
    
    <xsl:template match="/">
        <xsl:apply-templates/>
    </xsl:template>
    <xsl:template match="a:string | c:After | c:Before | c:Match">
        <xsl:copy>
            <xsl:copy-of select="@*"/>
            <xsl:if test="node()">
                <xsl:text disable-output-escaping="yes">&lt;![CDATA[</xsl:text>
                <xsl:apply-templates select="child::node()"/>
                <xsl:text disable-output-escaping="yes">]]&gt;</xsl:text>
            </xsl:if>
        </xsl:copy>
    </xsl:template>
    <xsl:template match="@* | node()" priority="-2">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>
    
    
</xsl:stylesheet>