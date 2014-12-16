<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    exclude-result-prefixes="xd"
    version="1.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Mar 22, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p>Převede značku <xd:b>pagina</xd:b> na <xd:b>pb</xd:b> s odpovídajícím označením, zda je uprostřed slova.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:output method="xml" indent="yes"/>
    
    
    <xsl:include href="Kopirovani_prvku.xsl"/>
    
    <xsl:template match="/">
        <xsl:comment> MOL_Prevod_paginace_na_TEI </xsl:comment>
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:include href="TEI_Foliace_na_Pb.xsl"/>
    

    
    <xsl:template match="pagina">
        <xsl:call-template name="zpracujFoliaci">
            <xsl:with-param name="konciMezerou" select="substring(., string-length(.), 1) = ' '" />
            <xsl:with-param name="cislo" select="normalize-space(.)" />
        </xsl:call-template>
    </xsl:template>
</xsl:stylesheet>