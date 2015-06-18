<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns:p="http://www.daliboris.cz/schemata/prepisy.xsd">
    <xsl:output method="text"/>
    <xsl:template match="/">
        <xsl:apply-templates select="/p:Prepisy/p:Prepis[p:Zpracovani/p:Exporty/p:Export/p:ZpusobVyuziti/text() = 'Manuscriptorium']" />
    </xsl:template>
    
    <xsl:template match="p:Prepis">
        <xsl:value-of select="p:Soubor/p:Nazev/text()"/>
        <xsl:text>&#9;</xsl:text>
        <xsl:apply-templates select="p:Zpracovani/p:Exporty/p:Export[p:ZpusobVyuziti/text() = 'Manuscriptorium']"></xsl:apply-templates>
        <xsl:text>&#9;</xsl:text>
        <xsl:text>(první export </xsl:text>
        <xsl:call-template name="FormatDate">
            <xsl:with-param name="DateTime">
                <xsl:value-of select="p:Zpracovani/p:PrvniExporty/p:Export[p:ZpusobVyuziti/text() = 'Manuscriptorium']/p:CasExportu/text()"/>
            </xsl:with-param>
        </xsl:call-template>
        
<!--         <xsl:apply-templates select="p:Zpracovani/p:PrvniExporty/p:Export[p:ZpusobVyuziti/text() = 'Manuscriptorium']"></xsl:apply-templates>-->
        <xsl:text>)</xsl:text>
        <!-- 
        <xsl:value-of select="p:Zpracovani/p:Exporty/p:Export[last()]/p:ZpusobVyuziti/text()"/>
        
        <xsl:call-template name="FormatDate">
            <xsl:with-param name="DateTime">
                <xsl:value-of select="p:Zpracovani/p:Exporty/p:Export[last()]/p:CasExportu/text()"/>
            </xsl:with-param>
        </xsl:call-template>
        -->
        <xsl:text>&#13;</xsl:text>
    </xsl:template>
    
    <xsl:template match="p:Export[p:ZpusobVyuziti/text() = 'Manuscriptorium']">
        <xsl:if test="position() = last()">
            <xsl:value-of select="p:ZpusobVyuziti/text()"/>
            <xsl:text>&#9;</xsl:text>
            <xsl:call-template name="FormatDate">
                <xsl:with-param name="DateTime">
                    <xsl:value-of select="p:CasExportu/text()"/>
                </xsl:with-param>
            </xsl:call-template>
        </xsl:if>
    </xsl:template>
    
    
    <xsl:template name="FormatDate">
        <xsl:param name="DateTime" />
        
        <xsl:variable name="year">
            <xsl:value-of select="substring($DateTime,1,4)" />
        </xsl:variable>
        
        <xsl:variable name="month-temp">
            <xsl:value-of select="substring-after($DateTime,'-')" />
        </xsl:variable>
        
        <xsl:variable name="month">
            <xsl:value-of select="substring-before($month-temp,'-')" />
        </xsl:variable>
        
        <xsl:variable name="day-temp">
            <xsl:value-of select="substring-after($month-temp,'-')" />
        </xsl:variable>
        
        <xsl:variable name="day">
            <xsl:value-of select="substring($day-temp,1,2)" />
        </xsl:variable>
        
        
        
        <xsl:variable name="time">
            <xsl:value-of select="substring-after($DateTime,'T')" />
        </xsl:variable>
        
        <xsl:variable name="hh">
            <xsl:value-of select="substring($time,1,2)" />
        </xsl:variable>
        
        <xsl:variable name="mm">
            <xsl:value-of select="substring($time,4,2)" />
        </xsl:variable>
        
        <xsl:variable name="ss">
            <xsl:value-of select="substring($time,7,2)" />
        </xsl:variable>
        
        
        <xsl:value-of select="$day"/>
        <xsl:value-of select="'. '"/>
        <!--18.-->
        <xsl:value-of select="$month"/>
        <xsl:value-of select="'. '"/>
        <!--18.03.-->
        <xsl:value-of select="$year"/>
        <xsl:value-of select="' '"/>
        <!--18.03.1976 -->
        <!--
            <xsl:value-of select="$hh"/>
            <xsl:value-of select="':'"/>
        -->
        <!--18.03.1976 13: -->
        <!--
            <xsl:value-of select="$mm"/>
        -->
        <!--18.03.1976 13:24 -->
        <!--
            <xsl:value-of select="':'"/>
            <xsl:value-of select="$ss"/>
        -->
        <!--18.03.1976 13:24:55 -->
        
    </xsl:template>
    
</xsl:stylesheet>
