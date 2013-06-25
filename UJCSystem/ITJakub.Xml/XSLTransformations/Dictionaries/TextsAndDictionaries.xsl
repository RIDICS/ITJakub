<?xml version="1.0" encoding="UTF-8"?>
    <xsl:stylesheet 
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
        xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
        xmlns:tei="http://www.tei-c.org/ns/1.0" 
        xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" 
        xmlns:exist = "http://exist.sourceforge.net/NS/exist"
        exclude-result-prefixes="xd tei nlp exist"
        version="1.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Jun 24, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p></xd:p>
        </xd:desc>
    </xd:doc>
        
        
        <xsl:strip-space elements="*"/>
        <xsl:param name="searchedLemma" select="'pes'" />
        
        
        <xsl:include href="Dictionaries.xsl"/>
        <xsl:include href="Texts.xsl"/>
        
        <xsl:template match="context">
            <xsl:apply-templates />
        </xsl:template>
        
    
</xsl:stylesheet>