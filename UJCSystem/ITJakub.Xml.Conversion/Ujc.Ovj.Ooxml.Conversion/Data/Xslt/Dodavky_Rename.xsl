<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xd tei" version="1.0">
    <xsl:import href="COMMON_Unknown_element.xsl" />
    <xsl:import href="COMMON_Dictionaries_Rename.xsl"/>
    <xsl:import href="TEI_Foliace_na_Pb.xsl"/>
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>
    
    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
    <xsl:variable name="vychozi-jazyk" select="'cs'"/>
    
    <xsl:key name="poznamka-pod-carou" match="poznamka_pod_carou" use="@id"/>
    
    <xsl:template match="body">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="/">
        <xsl:comment> Dodavky_Rename </xsl:comment>
        <body>
            <xsl:apply-templates/>
        </body>
    </xsl:template>
    
    <xsl:template match="Pismeno">
        <head1>
            <xsl:apply-templates />
        </head1>
    </xsl:template>
    
    <xsl:template match="text">
        <xsl:apply-templates />
    </xsl:template>
     <xsl:template match="heslo">
         <xsl:element name="form">
             <xsl:attribute name="xml:id">
                 <xsl:value-of select="concat('en', substring(string(1000001 + count(preceding-sibling::Normalni)), 2), '.hw1')"/>
             </xsl:attribute>
             <orth><xsl:apply-templates /></orth>
         </xsl:element>
     </xsl:template>
    <xsl:template match="podheslo">
        <form type="derivative"><orth><xsl:apply-templates /></orth></form>
    </xsl:template>
    
    <xsl:template match="Normalni">
        <xsl:element name="entryFree">
            <xsl:attribute name="xml:id">
                <xsl:value-of select="concat('en', substring(string(1000001 + count(preceding-sibling::Normalni)), 2))"/>
            </xsl:attribute>
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
</xsl:stylesheet>