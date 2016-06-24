<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:a="http://vokabular.ujc.cas.cz/ns/anotace"
    xmlns:xml = "http://www.w3.org/XML/1998/namespace"
    xmlns:t="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="t a"
    version="1.0" >
    
    <xsl:param name="faksimile" />
    
    <xsl:template match="t:TEI">
    	<TEI xmlns="http://www.tei-c.org/ns/1.0"  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
            <xsl:apply-templates select="@*" />
        <xsl:apply-templates select="*" />
        <xsl:apply-templates select="document($faksimile)"/>
        </TEI>
    </xsl:template>
    
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()" />
        </xsl:copy>
    </xsl:template>
    
    
    <xsl:template match="/">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="t:facsimile/a:title" />
	<xsl:template match="t:facsimile/t:surface[@n='××']" />
    <xsl:template match="comment()" priority="10" />
    
	<xsl:template match="t:term[not(@id)]" priority="10" >
		<xsl:copy>
			<xsl:attribute name="id"><xsl:value-of select="@n"/></xsl:attribute>
			<xsl:apply-templates select="@*" />
			<xsl:apply-templates select="node()"></xsl:apply-templates>
		</xsl:copy>
	</xsl:template>
    
    <xsl:template match="t:graphic">
        <xsl:copy>
            <xsl:attribute name="url">
                <xsl:call-template name="substring-after-last">
                    <xsl:with-param name="input"><xsl:value-of select="@url"/></xsl:with-param>
                    <xsl:with-param name="marker"><xsl:value-of select="'\'"/></xsl:with-param>
                    
                </xsl:call-template>
            </xsl:attribute>
        </xsl:copy>
    </xsl:template>
    
    
    <xsl:template name="substring-after-last">
        <xsl:param name="input" />
        <xsl:param name="marker" />
        
        <xsl:choose>
            <xsl:when test="contains($input,$marker)">
                <xsl:call-template name="substring-after-last">
                    <xsl:with-param name="input" 
                        select="substring-after($input,$marker)" />
                    <xsl:with-param name="marker" select="$marker" />
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$input" />
            </xsl:otherwise>
        </xsl:choose>
        
    </xsl:template>
    
    
</xsl:stylesheet>