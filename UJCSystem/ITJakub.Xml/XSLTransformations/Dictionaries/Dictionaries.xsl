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
            <xd:p><xd:b>Created on:</xd:b> Jun 12, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p>Společná šablona zastřešující jednotlivé slovníky.</xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:output method="html"/>
    
    <xsl:strip-space elements="*"/>
    
    
    <xsl:include href="Common/CommonDictionaries.xsl"/>
    <xsl:include href="DDBW.xsl"/>
    <xsl:include href="ESSC.xsl"/>
    
    <xsl:template match="/">
          <xsl:apply-templates select="//tei:entryFree" />
    </xsl:template>
    
</xsl:stylesheet>