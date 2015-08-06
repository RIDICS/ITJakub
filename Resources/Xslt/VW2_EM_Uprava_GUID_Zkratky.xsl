<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0"
    xmlns:xml="http://www.w3.org/XML/1998/namespace"
    exclude-result-prefixes="xd tei nlp"
    version="1.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> May 29, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p>Přidává do existujícího výstupu v XML TEI P5 informace z evidence textů, např. zkratku, kategorii, jedinečný identifikátor ap.</xd:p>
        </xd:desc>
    </xd:doc>
    
    
    <xsl:include href="TEI_Common.xsl"/>
    
    <xsl:output indent="yes" />
    
    <xsl:strip-space elements="*"/>
    
    <xsl:template match="comment()"  priority="10" />
    
    <xd:doc>
        <xd:desc>
            <xd:p>Jedinečný identifikátor pramene. Použije se v případě, kdy neexistuje v textu samotném.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:param name="guid" />
    
    <xd:doc>
        <xd:desc>
            <xd:p>Identifikátor kategorie, do níž text patří.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:param name="category" />
    
    <xd:doc>
        <xd:desc>
            <xd:p>Seznam termínů chrakterizujících text, např. próza, bible ap.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:param name="terms" />
    
    <xd:doc>
        <xd:desc>
            <xd:p>Zkratka pramene</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:param name="sourceAcronym" />
    
    <xd:doc>
        <xd:desc>
            <xd:p>Zkratka památky</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:param name="monumentAcronym" />
    
    
    <xsl:template match="/">
        <xsl:copy>
            <xsl:apply-templates />
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="tei:TEI">
        <xsl:element name="TEI" namespace="http://www.tei-c.org/ns/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0">
            <xsl:copy-of select="@*" />
            <xsl:attribute name="n">
                <xsl:choose>
                    <xsl:when test="tei:teiHeader/tei:fileDesc/@n">
                        <xsl:value-of select="translate(tei:teiHeader/tei:fileDesc/@n, '{}', '')" />                        
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="$guid"/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="tei:fileDesc">
        <xsl:copy>
            <xsl:if test="not(@n)">
                <xsl:attribute name="n">
                    <xsl:value-of select="$guid"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:copy-of select="@*" />
            
            <xsl:apply-templates />
        </xsl:copy>
        <xsl:if test="not(../tei:encodingDesc)">
            <xsl:call-template name="InsertEndocingDesc"/>
        </xsl:if>
        <xsl:if test="not(../tei:profileDesc)">
            <xsl:call-template name="InsertProfileDesc"/>
        </xsl:if>
    </xsl:template>
    
    <xsl:template match="tei:sourceDesc">
        <xsl:copy>
            <xsl:copy-of select="@*" />
        <xsl:if test="not(tei:listBibl) and ($monumentAcronym or $sourceAcronym)">
            <xsl:call-template name="InsertListBiblAcronyms">
                <xsl:with-param name="sourceAcronym" select="$sourceAcronym"/>
                <xsl:with-param name="monumentAcronym" select="$monumentAcronym"/>
            </xsl:call-template>
        </xsl:if>
            <xsl:apply-templates />
        </xsl:copy>
    </xsl:template>
    
    <xsl:template name="InsertListBiblAcronyms" xmlns="http://www.tei-c.org/ns/1.0">
        <xsl:param name="sourceAcronym"/>
        <xsl:param name="monumentAcronym"/>
        <listBibl>
            <xsl:if test="$sourceAcronym">
                <bibl type="acronym" subtype="source">
                    <xsl:value-of select="$sourceAcronym"/>
                </bibl>
            </xsl:if>
            <xsl:if test="$monumentAcronym">
                <bibl type="acronym" subtype="monument">
                    <xsl:value-of select="$monumentAcronym"/>
                </bibl>
            </xsl:if>
        </listBibl>
    </xsl:template>
    
    <xsl:template name="InsertProfileDesc" xmlns="http://www.tei-c.org/ns/1.0">
        <xsl:choose>
        <xsl:when test="not($category) and not($terms)">
        </xsl:when>
            <xsl:otherwise>
                <profileDesc>
                    <textClass>
                        <catRef>
                            <xsl:attribute name="target">
                                <xsl:value-of select="concat('#', $category)"/>
                            </xsl:attribute>
                        </catRef>
                        <xsl:if test="$terms">
                            <keywords scheme="http://vokabular.ujc.cas.cz/scheme/classification/secondary">
                                <term><xsl:value-of select="$terms"/></term>
                            </keywords>
                        </xsl:if>
                    </textClass>
                </profileDesc>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    
</xsl:stylesheet>