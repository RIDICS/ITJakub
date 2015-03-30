<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd tei nlp exist" version="1.0">
    <xsl:output method="html"/>
    
    <xsl:strip-space elements="*"/>
    
    <xsl:include href="pageToHtml.xsl"/>
    
    <xsl:template match="exist:match">
        <span class="match">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    
    <xsl:template match="tei:entryFree">
        <div class="entryFree">
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    
    <xsl:template match="tei:orth[not(@xml:lang) and not(@type)]">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="tei:orth">
        
        <span>
            <xsl:choose>
                <xsl:when test="@xml:lang">
                    <xsl:attribute name="lang">
                        <xsl:value-of select="@xml:lang"/>
                    </xsl:attribute>
                </xsl:when>
                <xsl:when test="@type">
                    <xsl:attribute name="data-orth-type">
                        <xsl:value-of select="@type"/>
                    </xsl:attribute>
                </xsl:when>
            </xsl:choose>
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    
    
    
    <xsl:template match="tei:num">
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="tei:cit">
        <span lang="{@xml:lang}" data-orth-type="{@type}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    
    <xsl:template match="context[tei:entryFree]">
        <xsl:apply-templates select="tei:entryFree" />
    </xsl:template>
    
</xsl:stylesheet>