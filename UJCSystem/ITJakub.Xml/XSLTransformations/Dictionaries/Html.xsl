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
            <xd:p><xd:b>Created on:</xd:b> Jun 11, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris Lehečka</xd:p>
            <xd:p>Šablona pro generování výstupu slovníkových hesel ve formátu HTML (pro vizuální kontrolu).</xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:strip-space elements="*"/>
    
    <xsl:include href="Common/CommonDictionaries.xsl"/>
    <xsl:include href="DDBW.xsl"/>
    <xsl:include href="ESSC.xsl"/>
    <xsl:include href="StcS.xsl"/>
    
    <xsl:template match="/">
        <html>
            <head>
                <title>Slovník</title>
                <link type="text/css" rel="stylesheet" href="dictionaries.css"></link>
            </head>
            <body>
                <div>
                    <xsl:apply-templates select="//tei:entryFree" />
                </div>
            </body>    
        </html>
    </xsl:template>
    
</xsl:stylesheet>