<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:p="http://www.daliboris.cz/schemata/prepisy.xsd"
    xmlns="http://www.daliboris.cz/schemata/prepisy.xsd"
    xmlns:xsd="http://www.w3.org/2001/XMLSchema"
    exclude-result-prefixes="xsl p"
    version="1.0"  >
    <xsl:output method="xml" standalone="yes" indent="yes"/>
    <xsl:template match="/">
            <xsl:apply-templates select="//p:Prepis[1]" />
    </xsl:template>
    
    <xsl:template match="p:Prepis[1]">
        <Prepis>   
            <xsl:apply-templates select="*"/>
        </Prepis>
    </xsl:template>
    
    <xsl:template match="*">
        <xsl:param name="uroven" />
        <xsl:variable name="destName" select="name()"/>
        <xsl:choose>
            <xsl:when test="count(child::*) = 0">
                <xsl:copy>
                    <xsl:choose>
                        <xsl:when test="text() != //p:Prepis[2]//*[name()=$destName]/text()">
                                <xsl:attribute name="prev"><xsl:value-of select="text()"/></xsl:attribute>
                            <xsl:copy-of select="//p:Prepis[2]//*[name()=$destName]/text()"/>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of select="."/>
                        </xsl:otherwise>
                    </xsl:choose>
                </xsl:copy>
            </xsl:when>
            <xsl:otherwise>
                <xsl:copy  >
                    <xsl:apply-templates >
                        <xsl:with-param name="uroven" select="$destName" />
                    </xsl:apply-templates>
                </xsl:copy>
            </xsl:otherwise>
        </xsl:choose>
        
    </xsl:template>
</xsl:stylesheet>
