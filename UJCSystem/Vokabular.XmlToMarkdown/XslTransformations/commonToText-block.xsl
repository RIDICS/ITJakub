<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd tei nlp exist" version="2.0">
    <xsl:output method="text" encoding="UTF-8"/>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro původní titul díla, uvedený v prameni.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:title">
        <xsl:text>&#xa;## </xsl:text><xsl:apply-templates/><xsl:text>&#xa;</xsl:text>
	</xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro nadpis.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:head">
        <xsl:text>&#xa;### </xsl:text><xsl:apply-templates/><xsl:text>&#xa;</xsl:text>
	</xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro ediční komentář.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='editorial' and @subtype='comment']">
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro informaci o grantové podpoře.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='editorial' and @subtype='grant']">
     
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro incipit a explicit.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='incipit' or @type='explicit']">
    
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro biblickou knihu.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='bible' and @subtype='book']">
    
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro biblickou kapitolu</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='bible' and @subtype='chapter']">
      
    </xsl:template>
    <xsl:template match="tei:p">
        <xsl:element name="p">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro seznam typu rejstřík.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:list[@type='index']">
       
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro tabulku</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:table">
        
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro řádek v tabulce</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:row">
       
    </xsl:template>
    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro buňku v tabulce</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:cell">
        
    </xsl:template>
    <xsl:template match="tei:note[@n]" mode="notes">
        
    </xsl:template>
</xsl:stylesheet>