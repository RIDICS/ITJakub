<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:xml="http://www.w3.org/XML/1998/namespace"
    exclude-result-prefixes="xd"
    version="2.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Mar 22, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p></xd:p>
        </xd:desc>
    </xd:doc>
    
    
    <xsl:include href="Kopirovani_prvku.xsl"/>
    <xsl:output indent="yes"/>
    <xsl:strip-space elements="*"/>
    
    <xsl:template match="/">
        <xsl:comment> MOL_Seskupit_sousedni_prvky_Keywords </xsl:comment>
        <xsl:apply-templates/>
    </xsl:template>
    

    
    <xsl:template match="*[div[@type='keywords']]">
        <xsl:copy>
            <xsl:copy-of select="@*"/>
            <xsl:for-each-group select="*" group-adjacent="exists(self::div[@type='keywords'])">
                <xsl:choose>
                    <xsl:when test="current-grouping-key()">
                        <div type='keywords'>
                            <xsl:apply-templates select="current-group()/*"/>
                        </div>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:apply-templates select="current-group()"/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:for-each-group>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="div[@type='keywords']/p[contains(., 'Key')]">
        <xsl:element name="p">
            <xsl:attribute name="lang" namespace="http://www.w3.org/XML/1998/namespace" select="'en'" />
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="div[@type='keywords']/p[contains(., 'Klíčová slova')]">
        <xsl:element name="p">
            <xsl:attribute name="lang" namespace="http://www.w3.org/XML/1998/namespace" select="'cs'" />
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
    
<!--    <xsl:template match="*[div[@type='keywords']]">
        <xsl:copy>
            <xsl:copy-of select="@*"/>
            <xsl:for-each-group select="*" group-adjacent="exists(self::div[@type='keywords'])">
                <xsl:choose>
                    <xsl:when test="current-grouping-key()">
                        <div type='keywords'>
                            <xsl:apply-templates select="current-group()/*"/>
                        </div>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:apply-templates select="current-group()"/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:for-each-group>
        </xsl:copy>
    </xsl:template>-->
    
<!--    <xsl:template match="body">
        <xsl:copy>
            <xsl:choose>
                <xsl:when test="div[@type='resume']">
                    <xsl:for-each-group select="*"
                        group-adjacent="if(self::div[@type='resume']) then 0 else position()">
                        <xsl:apply-templates select="."/>
                    </xsl:for-each-group>
                </xsl:when>
                <xsl:when test="div[@type='keywords']">
                    <xsl:for-each-group select="*"
                        group-adjacent="if(self::div[@type='keywords']) then 0 else position()">
                        <xsl:apply-templates select="."/>
                    </xsl:for-each-group>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:apply-templates/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:copy>
        
    </xsl:template>
    
    <xsl:template match="body/div[@type='resume']">
        <div type="resume">
            <xsl:apply-templates select="current-group()" mode="group"/>
        </div>
    </xsl:template>
    
    <xsl:template match="div[@type='resume']" mode="group">
        <xsl:apply-templates select="*"/>
    </xsl:template>
    
    
    <xsl:template match="body/div[@type='keywords']">
        <div type="keywords">
            <xsl:apply-templates select="current-group()" mode="group"/>
        </div>
    </xsl:template>
    
    <xsl:template match="div[@type='keywords']" mode="group">
        <xsl:apply-templates select="*"/>
    </xsl:template>
    -->
    <!--	<xsl:template match="*" mode="group">
		<xsl:copy-of select="current-group()" />
	</xsl:template>
-->
    
<!--    <xsl:template match="@*" priority="2">
        <xsl:variable name="nazev">
            <xsl:choose>
                <xsl:when test="local-name() = 'id'">
                    <xsl:text>xml:id</xsl:text>
                </xsl:when>
                <xsl:when test="local-name() = 'lang'">
                    <xsl:text>xml:lang</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="local-name()"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:attribute name="{$nazev}">
            <xsl:value-of select="."/>
        </xsl:attribute>
    </xsl:template>
    
    <xsl:template match="@xml:id" priority="3">
        <xsl:if test="name() = 'xml:id'">
            <xsl:attribute name="xml:id">
                <xsl:value-of select="."/>
            </xsl:attribute>
        </xsl:if>
    </xsl:template>
    -->
    
</xsl:stylesheet>